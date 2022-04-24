using CrewNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClient : MonoBehaviour, IServerPacketListener {

    public void Init() {

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

}
