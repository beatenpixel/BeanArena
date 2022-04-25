using CrewNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClient : MonoBehaviour, IServerPacketListener {

    public void Init() {
        Client.Instance.OnConnected += OnConnected;
        Client.Instance.OnDisconnected += OnDisconnected;
    }

    public void OnClicksInfo(SPacket_ClicksInfo packet) {
        
    }

    public void ProcessPacket(MessageType msgType, PacketReader packet) {
        switch (msgType) {
            case MessageType.SPacket_ClicksInfo:
                SPacket_ClicksInfo spClicksInfo = default; spClicksInfo.Read(packet);
                OnClicksInfo(spClicksInfo);
                break;
        }
    }

    private void OnConnected() {
        CPacket_PlayerJoin joinPacket = new CPacket_PlayerJoin() {
            username = "Player " + Random.Range(0, 100000)
        };

        Client.Instance.Send(joinPacket, SendOption.Reliable);
    }

    private void OnDisconnected() {

    }

}
