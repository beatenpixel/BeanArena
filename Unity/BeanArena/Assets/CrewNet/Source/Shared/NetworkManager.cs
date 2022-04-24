using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewNetwork {

    public abstract class NetworkManager : ITickable {

        public static NetworkManager Instance { get; protected set; }

        public NetworkRole role { get; protected set; }

        public bool IsHost => role == NetworkRole.Host;
        public bool IsClient => (role & NetworkRole.Client) != 0;
        public bool IsServer => (role & NetworkRole.Server) != 0;

        public CrewNetPeer_Ref ServerPeerRef { get; protected set; }

        public virtual void Init() {
            Instance = this;
            MessageListener.Init();
        }

        public abstract void Tick();
        public abstract void Shutdown();
    }

}
