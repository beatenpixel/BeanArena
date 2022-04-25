using CrewNetwork;
using CrewNetwork.Transport;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonServer : Server {

    private PhotonNetworkManager manager;

    public PhotonServer(PhotonNetworkManager manager) {
        Instance = this;

        this.manager = manager;
    }

    public override void Start() {

    }

    public override void Tick() {

    }

    public override void Send(PacketWriter packet, CrewNetPeer_Ref peer) {
        manager.SendAsServer(packet, peer);
    }

    public override void Send(IPacket packet, CrewNetPeer_Ref peer, SendOption sendOption = SendOption.None) {
        PacketWriter packetWriter = PacketWriter.Get(sendOption, packet);
        Send(packetWriter, peer);
    }

    public override void Disconnect(CrewNetPeer_Ref peer) {

    }

    public override void Shutdown() {
        
    }    
    
}
