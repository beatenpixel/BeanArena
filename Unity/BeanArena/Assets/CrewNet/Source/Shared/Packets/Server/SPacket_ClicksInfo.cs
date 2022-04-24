using CrewNetwork;

public struct SPacket_ClicksInfo : IPacket {

    public MessageType type => MessageType.SPacket_ClicksInfo;

    public int playerTotalClicks;

    public SPacket_ClicksInfo(int _playerTotalClicks) {
        playerTotalClicks = _playerTotalClicks;
    }

    public void Write(PacketWriter packet) {
        packet.WriteInt(playerTotalClicks);
    }

    public void Read(PacketReader packet) {
        playerTotalClicks = packet.ReadInt();
    }
}
