using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrewNetwork.Transport.ENetTransport {

    public class ENetClient : Client {

        private ENet.Host client;
        private ENet.Peer peer;

        private ClientConnectionRequest connectionRequest;

        public ENetClient() {
            Instance = this;            
        }

        public void Init(ClientConnectionRequest _connectionRequest) {
            connectionRequest = _connectionRequest;

            if (!ENetCommon.libraryIsInitialized) {
                if (ENet.Library.Initialize()) {
                    ENetCommon.libraryIsInitialized = true;
                }
            }
        }

        public override void Connect() {
            client = new ENet.Host();

            ENet.Address address = new ENet.Address();

            address.SetHost(connectionRequest.ip);
            address.Port = connectionRequest.port;
            client.Create();

            peer = client.Connect(address);
        }

        public override void Tick() {
            ProcessENet();
            client.Flush();
        }

        public override void Send(PacketWriter packet) {
            ENet.Packet enetPacket = packet.ToENetPacket();

            peer.Send(0, ref enetPacket);
        }

        public override void Send(IPacket packet, SendOption sendOption = SendOption.None) {
            PacketWriter packetWriter = PacketWriter.Get(sendOption, packet);
            Send(packetWriter);
        }

        public override void Disconnect() {
            
        }

        public override void Shutdown() {
            client.Flush();

            if (ENetCommon.libraryIsInitialized) {
                ENet.Library.Deinitialize();
                ENetCommon.libraryIsInitialized = false;
            }            
        }

        private void ProcessENet() {
            ENet.Event netEvent;

            bool pooled = false;

            while (!pooled) {

                int eventsCount = client.CheckEvents(out netEvent);

                if (eventsCount <= 0) {
                    if (client.Service(0, out netEvent) <= 0)
                        break;

                    pooled = true;
                }

                switch (netEvent.Type) {
                    case ENet.EventType.None:
                        break;

                    case ENet.EventType.Connect:
                        CrewNetDebug.Log("Client connected to server");

                        OnConnected?.Invoke();
                        break;

                    case ENet.EventType.Disconnect:
                        CrewNetDebug.Log("Client disconnected from server");

                        OnDisconnected?.Invoke();
                        break;

                    case ENet.EventType.Timeout:
                        CrewNetDebug.Log("Client connection timeout");
                        break;

                    case ENet.EventType.Receive:
                        CrewNetDebug.Log("Packet received from - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP + ", Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);

                        PacketReader packet = PacketReader.Get(netEvent.Packet.Data, netEvent.Packet.Length);

                        OnPacket?.Invoke(packet);

                        packet.Recycle();

                        /*
                        Packet packet = Packet.Create(netEvent.Packet);

                        if (packet is PacketS2C_AcceptJoin pJoinAccept) {
                            Debug.Log("Got from server: " + pJoinAccept.serverWelcomeMessage);
                        }
                        */

                        netEvent.Packet.Dispose();
                        break;
                }
            }
        }
    }

    [System.Serializable]
    public struct ClientConnectionRequest {
        public string ip;
        public ushort port;
    }

}
