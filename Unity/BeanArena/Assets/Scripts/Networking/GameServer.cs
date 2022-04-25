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
        }
    }

    public void OnSendClicks(CrewNetPeer_Ref peer, CPacket_SendClick packet) {

    }

    private void OnPeerConnected(CrewNetPeer_Ref peer) {
        NetworkPlayer networkPlayer = new NetworkPlayer();
        networkPlayer.peerUID = peer.UID;
        players.Add(networkPlayer.peerUID, networkPlayer);
    }

    private void OnPeerDisconnected(CrewNetPeer_Ref peer) {
        if(players.ContainsKey(peer.UID)) {
            players.Remove(peer.UID);
        }
    }

}

public class NetworkPlayer {
    public uint peerUID;
}
