using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewNetwork.Transport.ENetTransport {

    public class ENetServer : Server {

        private Dictionary<ushort, ENet.Peer> enetPeers = new Dictionary<ushort, ENet.Peer>();
        private object enetPeers_Lock = new object();

        private ENet.Host server;
        private ServerStartConfig config;

        public ENetServer() {
            Instance = this;            
        }

        public void Init(ServerStartConfig _config) {
            config = _config;

            if (!ENetCommon.libraryIsInitialized) {
                if (ENet.Library.Initialize()) {
                    ENetCommon.libraryIsInitialized = true;
                }
            }
        }

        public override void Start() {
            server = new ENet.Host();

            ENet.Address address = new ENet.Address();

            address.Port = (ushort)config.port;
            server.Create(address, config.maxClients);
        }

        public override void Tick() {
            ProcessENet();
            server.Flush();
        }

        private void ProcessENet() {
            ENet.Event netEvent;

            bool polled = false;

            while (!polled) {

                int eventsCount = server.CheckEvents(out netEvent);

                if (eventsCount <= 0) {
                    if (server.Service(0, out netEvent) <= 0)
                        break;

                    polled = true;
                }

                switch (netEvent.Type) {
                    case ENet.EventType.None:
                        break;
                    case ENet.EventType.Connect:
                        CrewNetDebug.Log("Client connected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);

                        lock(enetPeers_Lock) {
                            CrewNetPeer_Ref peerRef = CrewNetPeer_Ref.FromID((ushort)netEvent.Peer.ID);

                            enetPeers.Add((ushort)netEvent.Peer.ID, netEvent.Peer);

                            OnPeerConnected?.Invoke(peerRef);
                        }
                        break;
                    case ENet.EventType.Disconnect:
                        CrewNetDebug.Log("Client disconnected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);

                        lock (enetPeers_Lock) {
                            if (enetPeers.ContainsKey((ushort)netEvent.Peer.ID)) {
                                CrewNetPeer_Ref peerRef = CrewNetPeer_Ref.FromID((ushort)netEvent.Peer.ID);

                                enetPeers.Remove((ushort)netEvent.Peer.ID);

                                OnPeerDisconnected?.Invoke(peerRef);
                            }
                        }                        
                        break;
                    case ENet.EventType.Timeout:
                        CrewNetDebug.Log("Client timeout - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                        break;
                    case ENet.EventType.Receive:
                        CrewNetDebug.Log("Packet received from - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP + ", Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);

                        PacketReader packet = PacketReader.Get(netEvent.Packet.Data, netEvent.Packet.Length);

                        OnPacket?.Invoke(CrewNetPeer_Ref.FromID((ushort)netEvent.Peer.ID), packet);                       

                        packet.Recycle();

                        netEvent.Packet.Dispose();
                        break;
                }
            }
        }

        public override void Send(PacketWriter packet, CrewNetPeer_Ref toPeer) {
            lock (enetPeers_Lock) {
                if (enetPeers.ContainsKey(toPeer.UID)) {
                    ENet.Packet enetPacket = packet.ToENetPacket();

                    enetPeers[toPeer].Send(0, ref enetPacket);
                } else {
                    CrewNetDebug.LogError($"Peer {toPeer.UID} not found!");
                }
            }
        }

        public override void Send(IPacket packet, CrewNetPeer_Ref peer, SendOption sendOption) {
            PacketWriter packetWriter = PacketWriter.Get(sendOption, packet);
            Send(packetWriter, peer);
        }


        public override void Disconnect(CrewNetPeer_Ref peer) {
            
        }

        public override void Shutdown() {
            server.Flush();

            if (ENetCommon.libraryIsInitialized) {
                ENet.Library.Deinitialize();
                ENetCommon.libraryIsInitialized = false;
            }
        }

    }

    [System.Serializable]
    public struct ServerStartConfig {
        public int port;
        public int maxClients;
    }

}
