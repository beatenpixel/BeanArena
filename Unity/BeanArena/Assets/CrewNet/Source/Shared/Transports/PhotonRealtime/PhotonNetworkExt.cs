using CrewNetwork;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhotonNetworkExt {
    
    public static SendOptions ToPhotonSendOption(this SendOption sendOption) {
        if(sendOption.HasFlag(SendOption.Reliable)) {
            return SendOptions.SendReliable;
        } else {
            return SendOptions.SendUnreliable;
        }
    }

}
