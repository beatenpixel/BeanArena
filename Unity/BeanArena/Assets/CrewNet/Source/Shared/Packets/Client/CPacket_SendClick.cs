using CrewNetwork;

public struct CPacket_SendClick : IPacket {

    public MessageType type => MessageType.CPacket_SendClick;

    public int sendClicksCount;

    public CPacket_SendClick(int _sendClicksCount) {
        sendClicksCount = _sendClicksCount;
    }

    public void Write(PacketWriter packet) {
        packet.WriteByte((byte)sendClicksCount);
    }

    public void Read(PacketReader packet) {
        sendClicksCount = packet.ReadByte();
    }    
}
