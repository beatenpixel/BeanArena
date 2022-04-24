using CrewNetwork;

public interface IClientPacketListener {
    void ProcessPacket(CrewNetPeer_Ref peer, MessageType msgType, PacketReader packet);
    void OnSendClicks(CrewNetPeer_Ref peer, CPacket_SendClick packet);
}
