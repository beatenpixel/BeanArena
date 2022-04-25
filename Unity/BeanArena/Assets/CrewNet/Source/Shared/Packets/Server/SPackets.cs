using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrewNetwork;

public class SPackets
{

}

public struct SPacket_JoinAnswer : IPacket {

    public MessageType type => MessageType.SPacket_PlayerJoinAnswer;



    public void Write(PacketWriter packet) {
        
    }

    public void Read(PacketReader packet) {
        
    }
}