using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrewNetwork.Transport.ENetTransport {
    public static class ENetCommon {

        public static bool libraryIsInitialized;

        private static ConcurrentDictionary<IntPtr, PacketWriter> freeCallbackPackets
        = new ConcurrentDictionary<IntPtr, PacketWriter>(4, 1024);

        private static IntPtr FreePacketCallback = Marshal.GetFunctionPointerForDelegate((ENet.PacketFreeCallback)OnFreePacket);

        public static ENet.Packet ToENetPacket(this PacketWriter packet) {
            ENet.Packet enetPacket = default(ENet.Packet);
            packet.ToByteArray_NoCopy(out byte[] packetArray, out int packetOffset, out int packetLength);

            ENet.PacketFlags flags = ENet.PacketFlags.None | ENet.PacketFlags.NoAllocate;

            if (packet.sendOption.HasFlag(SendOption.Reliable)) {
                flags |= ENet.PacketFlags.Reliable;
            }

            if (packet.sendOption.HasFlag(SendOption.Instant)) {
                flags |= ENet.PacketFlags.Instant;
            }

            enetPacket.Create(packetArray, packetOffset, packetLength, flags);

            freeCallbackPackets.TryAdd(enetPacket.NativeData, packet);
            enetPacket.SetFreeCallback(FreePacketCallback);

            return enetPacket;
        }

        private static void OnFreePacket(ENet.Packet enetPacket) {
            if (freeCallbackPackets.TryRemove(enetPacket.NativeData, out PacketWriter packet)) {
                packet.Recycle();
                CrewNetDebug.Log("Free packet: " + enetPacket.NativeData.ToInt64() + " isNull: " + (packet == null));
            } else {
                CrewNetDebug.Log("Packet to free not found " + enetPacket.NativeData.ToInt64() + " we are leaking memory!");
            }
        }

    }
}
