using CrewNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServer : MonoBehaviour, IClientPacketListener {   

    public void Init() {
        Server.Instance.OnPeerConnected += OnPeerConnected;
        Server.Instance.OnPeerDisconnected += OnPeerDisconnected;
    }

    public void ProcessPacket(CrewNetPeer_Ref peer, MessageType msgType, PacketReader packet) {
        switch (msgType) {
            case MessageType.SPacket_ClicksInfo:
                CPacket_SendClick cpSendClick = default; cpSendClick.Read(packet);
                OnSendClicks(peer, cpSendClick);
                break;
        }
    }

    public void OnSendClicks(CrewNetPeer_Ref peer, CPacket_SendClick packet) {

    }

    private void OnPeerConnected(CrewNetPeer_Ref peer) {

    }

    private void OnPeerDisconnected(CrewNetPeer_Ref peer) {

    }

}
