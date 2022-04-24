using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewNetwork {

    public interface IPacket {     
        MessageType type { get; }
        void Write(PacketWriter packet);
        void Read(PacketReader packet);
    }

}
