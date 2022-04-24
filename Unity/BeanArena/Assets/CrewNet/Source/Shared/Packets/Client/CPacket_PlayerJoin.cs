using CrewNetwork;

public struct CPacket_PlayerJoin : IPacket {

    public MessageType type => MessageType.CPacket_PlayerJoin; 

    public string username;
    public int level;

    public void Read(PacketReader packet) {
        username = packet.ReadString_UNICODE(ref CrewNetCommon.StringBuilder);
        level = packet.ReadInt();
    }

    public void Write(PacketWriter packet) {
        packet.WriteString_UNICODE(username);
        packet.WriteInt(level);
    }

}
