using CrewNetwork;

public interface IServerPacketListener {
    void ProcessPacket(MessageType msgType, PacketReader packet);
    void OnClicksInfo(SPacket_ClicksInfo packet);
    void OnJoinAnswer(SPacket_JoinAnswer packet);
}