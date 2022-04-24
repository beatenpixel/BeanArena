using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewNetwork {

    public static class CrewNetCommon {

        public static StringBuilder StringBuilder = new StringBuilder();

        public const int MAX_PACKET_SIZE = 1200;
        public const int MAX_MESSAGE_SIZE = MAX_PACKET_SIZE;

        public const byte MAX_PLAYERS_COUNT = 8;

        public const string ROOM_KEY_MAP = "map";
        public const string ROOM_KEY_GAME_UID = "guid";

    }

    public interface ITickable {
        void Tick();
    }

    public enum CrewNetTransport {
        None,
        ENet,
        PhotonRealtime
    }

    [Flags]
    public enum NetworkRole {
        None = 0,
        Server = 1 << 0,
        Client = 1 << 2,
        Host = Server | Client
    }

    [Flags]
    public enum SendOption : byte {
        None = 0,
        Reliable = 1 << 0,
        Instant = 1 << 1
    }

    public enum MessageType : ushort {

        S_PlayerJoinAnswer = 0x0100,
        S_PlayerClickInfo = 0x0101,

        C_PlayerJoin = 0x0200,
        C_Click = 0x0201,
        C_PlayerUpdate = 0x0202,

        ServerPackets = 0x1000,
        SPacket_PlayerJoinAnswer = 0x1001,
        SPacket_ClicksInfo = 0x1002,

        ClientPackets = 0x2000,
        CPacket_PlayerJoin = 0x2001,
        CPacket_SendClick = 0x2002,

        InternalMessages = 0xF000,

        Connect = 0xFFFA,
        Disconnect = 0xFFFB,
        Setup = 0xFFFC,

        Ping = 0xFFFD,
        Pong = 0xFFFE,

        None = 0xFFFF
    }

}
