using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewNetwork {
    public abstract class Server : ITickable {

        public static Server Instance;

        public Action<CrewNetPeer_Ref> OnPeerConnected;
        public Action<CrewNetPeer_Ref> OnPeerDisconnected;
        public Action<CrewNetPeer_Ref, PacketReader> OnPacket;

        public abstract void Start();
        public abstract void Tick();
        public abstract void Send(PacketWriter packet, CrewNetPeer_Ref peer);
        public abstract void Send(IPacket packet, CrewNetPeer_Ref peer, SendOption sendOption);
        public abstract void Disconnect(CrewNetPeer_Ref peer);
        public abstract void Shutdown();
    }
}
