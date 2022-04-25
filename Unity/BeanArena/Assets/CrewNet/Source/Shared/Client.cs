using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewNetwork {
    public abstract class Client : ITickable {

        public static Client Instance;

        public Action OnConnected;
        public Action OnDisconnected;
        public Action<PacketReader> OnPacket;

        public abstract void Connect();
        public abstract void Tick();
        public abstract void Send(PacketWriter packet);
        public abstract void Send(IPacket packet, SendOption sendOption = SendOption.Reliable);
        public abstract void Disconnect();
        public abstract void Shutdown();
    }

    /*
    public delegate void OnConnected();
    public delegate void OnDisconnected();
    public delegate void OnMessageFromServer(PacketReader packet);
    */

}
