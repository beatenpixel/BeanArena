using CrewNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClient : MonoBehaviour, IServerPacketListener {

    public void Init() {
        Client.Instance.OnConnected += OnConnected;
        Client.Instance.OnDisconnected += OnDisconnected;
    }

    public void ProcessPacket(MessageType msgType, PacketReader packet) {
        switch (msgType) {
            case MessageType.SPacket_ClicksInfo:
                SPacket_ClicksInfo spClicksInfo = default; spClicksInfo.Read(packet);
                OnClicksInfo(spClicksInfo);
                break;
            case MessageType.SPacket_PlayerJoinAnswer:
                SPacket_JoinAnswer spJoinAnswer = default; spJoinAnswer.Read(packet);
                OnJoinAnswer(spJoinAnswer);
                break;
            default:
                Debug.LogError("Unknown packet: " + msgType);
                break;
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(2)) {
            CPacket_PlayerJoin joinPacket = new CPacket_PlayerJoin() {
                username = Game.data.player.nickname
            };

            Client.Instance.Send(joinPacket, SendOption.Reliable);
        }
    }

    private void OnConnected() {
        Debug.Log("[GameClient] OnConnected");

        CPacket_PlayerJoin joinPacket = new CPacket_PlayerJoin() {
            username = Game.data.player.nickname
        };

        Client.Instance.Send(joinPacket, SendOption.Reliable);
    }

    private void OnDisconnected() {
        Debug.Log("[GameClient] OnDisconnected");
    }

    public void OnClicksInfo(SPacket_ClicksInfo packet) {

    }

    public void OnJoinAnswer(SPacket_JoinAnswer packet) {
        Debug.Log("HEY! Got from server JoinAnswer packet");
    }

}
