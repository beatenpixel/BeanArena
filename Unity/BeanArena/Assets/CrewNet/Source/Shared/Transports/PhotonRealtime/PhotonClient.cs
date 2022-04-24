using CrewNetwork;
using CrewNetwork.Transport;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonClient : Client {

    private PhotonNetworkManager manager;

    public PhotonClient(PhotonNetworkManager manager) {
        Instance = this;

        this.manager = manager;
    }

    public override void Connect() {
        
    }

    public override void Tick() {

    }

    public override void Send(PacketWriter packet) {
        manager.SendAsClient(packet);
    }

    public override void Disconnect() {

    }

    public override void Shutdown() {
        
    }    
}
