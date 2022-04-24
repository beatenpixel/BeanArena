using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewNetwork {
    public struct CrewNetPeer_Ref : IEquatable<CrewNetPeer_Ref> {
        public ushort UID;
        public static implicit operator ushort(CrewNetPeer_Ref p) => p.UID;

        public static CrewNetPeer_Ref FromID(ushort _UID) {
            return new CrewNetPeer_Ref() {
                UID = _UID
            };
        }

        public bool Equals(CrewNetPeer_Ref other) {
            return UID == other.UID;
        }
    }

    public struct CrewNetUID_Ref {
        public uint UID;
        public static implicit operator uint(CrewNetUID_Ref p) => p.UID;
    }
}
