using CrewNetwork;

public struct CPacket_PlayerJoin : IPacket {

    public MessageType type => MessageType.CPacket_PlayerJoin; 

    public string username;

    public void Read(PacketReader packet) {
        username = packet.ReadString_UNICODE(ref CrewNetCommon.StringBuilder);
    }

    public void Write(PacketWriter packet) {
        packet.WriteString_UNICODE(username);
    }

}
