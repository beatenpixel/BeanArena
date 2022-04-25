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
        //packet.WriteInt(playerTotalClicks);
    }

    public void Read(PacketReader packet) {
        //playerTotalClicks = packet.ReadInt();
    }
}