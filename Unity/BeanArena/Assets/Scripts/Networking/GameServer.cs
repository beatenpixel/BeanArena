using CrewNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServer : MonoBehaviour, IClientPacketListener {

    public static Dictionary<uint, NetworkPlayer> players;

    public void Init() {
        players = new Dictionary<uint, NetworkPlayer>();

        Server.Instance.OnPeerConnected += OnPeerConnected;
        Server.Instance.OnPeerDisconnected += OnPeerDisconnected;        
    }

    public void ProcessPacket(CrewNetPeer_Ref peer, MessageType msgType, PacketReader packet) {
        switch (msgType) {
            case MessageType.SPacket_ClicksInfo:
                CPacket_SendClick cpSendClick = default; cpSendClick.Read(packet);
                OnSendClicks(peer, cpSendClick);
                break;
            case MessageType.CPacket_PlayerJoin:
                CPacket_PlayerJoin cpPlayerJoin = default; cpPlayerJoin.Read(packet);
                OnPlayerJoin(peer, cpPlayerJoin);
                break;
            default:
                Debug.LogError("Unknown packet: " + msgType);
                break;
        }
    }

    private void OnPeerConnected(CrewNetPeer_Ref peer) {
        Debug.Log("[GameServer] OnPeerConnected " + peer.UID);

        NetworkPlayer networkPlayer = new NetworkPlayer();
        networkPlayer.peerUID = peer.UID;
        players.Add(networkPlayer.peerUID, networkPlayer);
    }

    private void OnPeerDisconnected(CrewNetPeer_Ref peer) {
        Debug.Log("[GameServer] OnPeerDisconnected " + peer.UID);

        if (players.ContainsKey(peer.UID)) {
            players.Remove(peer.UID);
        }
    }

    public void OnSendClicks(CrewNetPeer_Ref peer, CPacket_SendClick packet) {

    }

    public void OnPlayerJoin(CrewNetPeer_Ref peer, CPacket_PlayerJoin packet) {
        Debug.Log("HEY! " + packet.username);

        SPacket_JoinAnswer pJoinAnswer = new SPacket_JoinAnswer();
        Server.Instance.Send(pJoinAnswer, peer);
    }
}

public class NetworkPlayer {
    public uint peerUID;
}
