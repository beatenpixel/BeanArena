using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CrewNetwork {

    public static class MessageListener {

        private static Dictionary<MessageType, MessageFromServer_Handler> messageFromServerHandlers;
        private static Dictionary<MessageType, MessageFromClient_Handler> messageFromClientHandlers;

        private static Dictionary<MessageType, Func<IPacket>> packetsCreatorDict;
        private static Dictionary<MessageType, MethodInfo> packetsMethods;

        private static bool isInitialized;

        private static IServerPacketListener serverPacketListener;
        private static IClientPacketListener clientPacketListener;

        public static void Init() {
            if(isInitialized) {
                return;
            }

            Assembly assembly = Assembly.GetCallingAssembly();
            CreateMessageHandlersDictionary(assembly);
            //CreatePacketHandlers(assembly);
        }

        public static void InitPacketListeners(IServerPacketListener _serverPacketListener, IClientPacketListener _clientPacketListener) {
            serverPacketListener = _serverPacketListener;
            clientPacketListener = _clientPacketListener;
        }

        public static void OnPacket(MessageType messageType, PacketReader packetReader) {
            if(packetsCreatorDict.ContainsKey(messageType)) {
                IPacket packet = packetsCreatorDict[messageType]();
                packet.Read(packetReader);
                packetsMethods[messageType].Invoke(null, new object[] { packet });
            }
        }

        public static void OnPacketFromClient(CrewNetPeer_Ref peer, PacketReader packet) {
            while (packet.ReadNextMessage(out MessageType msgType)) {
                if (msgType < MessageType.ServerPackets) {
                    OnMessageFromClient(peer, msgType, packet);
                } else if (msgType < MessageType.InternalMessages) {
                    clientPacketListener.ProcessPacket(peer, msgType, packet);
                }
            }
        }

        public static void OnPacketFromServer(PacketReader packet) {
            while (packet.ReadNextMessage(out MessageType msgType)) {
                if (msgType < MessageType.ServerPackets) {
                    OnMessageFromServer(msgType, packet);
                } else if (msgType < MessageType.InternalMessages) {
                    serverPacketListener.ProcessPacket(msgType, packet);
                }
            }
        }

        private static void CreatePacketHandlers(Assembly assembly) {
            packetsCreatorDict = new Dictionary<MessageType, Func<IPacket>>();
            packetsMethods = new Dictionary<MessageType, MethodInfo>();

            foreach (Type type in assembly.GetTypes()) {
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
                    var methodParams = method.GetParameters().Where(x => typeof(IPacket).IsAssignableFrom(x.ParameterType)).ToArray();

                    if(methodParams.Length > 0) {
                        CrewNetDebug.Log("Packet Method: " + method.Name);

                        Type packetType = methodParams[0].ParameterType;

                        var tmp = (IPacket)Activator.CreateInstance(packetType);
                        var packetMessageType = tmp.type;

                        //ParameterExpression param1 = Expression.Parameter(t);
                        ConstantExpression param1 = Expression.Constant(packetType);

                        var lambda = Expression.Lambda<Func<CPacket_PlayerJoin>>(
                            Expression.Convert(Expression.Call(typeof(Activator).GetMethod("CreateInstance", new Type[] { typeof(Type) }), param1), packetType)
                            ).Compile();

                        CrewNetDebug.Log(lambda().GetType());

                        //packetsCreatorDict.Add(packetMessageType, lambda);
                        packetsMethods.Add(packetMessageType, method);

                        /*
                        Delegate packetHandler = Delegate.CreateDelegate(typeof(PacketHandler3), method, false);

                        var lambda = Expression.Lambda(Expression.Block(
                            Expression.Variable(methodParams.First().ParameterType, "argType")
                            //Expression.Call(method, Expre)
                        ), Array.Empty<ParameterExpression>()).Compile();

                        if(packetHandler != null) {
                            CrewNetDebug.Log("Success!");
                        } else {
                            CrewNetDebug.Log("Failed!");
                        }
                        */
                    }
                }                
            }
        }

        public delegate T PacketHandlerOfType<out T>();

        public static void OnMessageFromServer(MessageType msgType, PacketReader packet) {
            if(messageFromServerHandlers.TryGetValue(msgType, out MessageFromServer_Handler handler)) {
                handler(packet);
            } else {
                CrewNetDebug.Log($"No message handler for MessageType.{msgType}");
            }
        }

        public static void OnMessageFromClient(CrewNetPeer_Ref peer, MessageType msgType, PacketReader packet) {
            if (messageFromClientHandlers.TryGetValue(msgType, out MessageFromClient_Handler handler)) {
                handler(peer,packet);
            } else {
                CrewNetDebug.Log($"No message handler for MessageType.{msgType}");
            }
        }

        private static void CreateMessageHandlersDictionary(Assembly assembly) {
            MethodInfo[] methods = assembly.GetTypes()
                                           .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)) // Include instance methods in the search so we can show the developer an error instead of silently not adding instance methods to the dictionary
                                           .Where(m => m.GetCustomAttributes(typeof(MessageHandlerAttribute), false).Length > 0)
                                           .ToArray();

            messageFromServerHandlers = new Dictionary<MessageType, MessageFromServer_Handler>(methods.Length);
            messageFromClientHandlers = new Dictionary<MessageType, MessageFromClient_Handler>(methods.Length);

            for (int i = 0; i < methods.Length; i++) {
                MessageHandlerAttribute attribute = methods[i].GetCustomAttribute<MessageHandlerAttribute>();

                if (!methods[i].IsStatic) {
                    throw new Exception($"Message handler methods should be static, but '{methods[i].DeclaringType}.{methods[i].Name}' is an instance method!");
                }

                Delegate serverMessageHandler = Delegate.CreateDelegate(typeof(MessageFromServer_Handler), methods[i], false);

                if (serverMessageHandler != null) {
                    // It's a message handler for Server instances

                    if (messageFromServerHandlers.ContainsKey(attribute.m_MessageType)) {
                        MethodInfo otherMethodWithId = messageFromServerHandlers[attribute.m_MessageType].GetMethodInfo();
                        throw new Exception($"Server-side message handler methods '{methods[i].DeclaringType}.{methods[i].Name}' and '{otherMethodWithId.DeclaringType}.{otherMethodWithId.Name}' are both set to handle messages with ID {attribute.m_MessageType}! Only one handler method is allowed per message ID!");
                    } else {
                        messageFromServerHandlers.Add(attribute.m_MessageType, (MessageFromServer_Handler)serverMessageHandler);
                        CrewNetDebug.Log($"Added server message handler <{attribute.m_MessageType}> = {serverMessageHandler.Method.Name}");
                    }
                } else {
                    // It's not a message handler for Server instances, but it might be one for Client instances

                    Delegate clientMessageHandler = Delegate.CreateDelegate(typeof(MessageFromClient_Handler), methods[i], false);

                    if (messageFromClientHandlers.ContainsKey(attribute.m_MessageType)) {
                        MethodInfo otherMethodWithId = messageFromClientHandlers[attribute.m_MessageType].GetMethodInfo();
                        throw new Exception($"Server-side message handler methods '{methods[i].DeclaringType}.{methods[i].Name}' and '{otherMethodWithId.DeclaringType}.{otherMethodWithId.Name}' are both set to handle messages with ID {attribute.m_MessageType}! Only one handler method is allowed per message ID!");
                    } else {
                        messageFromClientHandlers.Add(attribute.m_MessageType, (MessageFromClient_Handler)clientMessageHandler);
                        CrewNetDebug.Log($"Added client message handler <{attribute.m_MessageType}> = {clientMessageHandler.Method.Name}");
                    }                        
                }
            }
        }

        public delegate void MessageFromClient_Handler(CrewNetPeer_Ref from, PacketReader packet);
        public delegate void MessageFromServer_Handler(PacketReader packet);
        public delegate void PacketHandler(dynamic packet);
        public delegate void PacketHandler2(IPacket packet);
        public delegate void PacketHandler3(CPacket_PlayerJoin packet);
        public delegate void PacketHandler4<T>(T packet);
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class MessageHandlerAttribute : Attribute {
        public MessageType m_MessageType { get; private set; }
        public MessageHandlerAttribute(MessageType messageType) {
            m_MessageType = messageType;
        }
    }

}
