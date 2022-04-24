using CrewNetwork.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CrewNetwork {

    public unsafe class PacketWriter : IRecycable {

        private static readonly ObjectPool<PacketWriter> Pool = new ObjectPool<PacketWriter>(() => new PacketWriter(), 1024);

        private byte[] data;

        private byte* m_Buffer;
        private int m_Capacity;
        private int m_Pos;

        public int WrittenPacketLength => m_Pos;
        public SendOption sendOption;

        private PacketWriter() {
            data = new byte[CrewNetCommon.MAX_PACKET_SIZE];
        }

        public void Clear(SendOption _sendOption) {
            sendOption = _sendOption;
            Clear(ref data);
        }

        public void StartMessage(MessageType type) {
            WriteUShort((ushort)type);
        }

        public static PacketWriter Get(SendOption sendOption, MessageType startMessageType) {
            PacketWriter output = Pool.Get();
            output.Clear(sendOption);
            if(startMessageType != MessageType.None) {
                output.StartMessage(startMessageType);
            }
            return output;
        }

        public static PacketWriter Get(SendOption sendOption, IPacket packet) {
            PacketWriter output = Pool.Get();
            output.Clear(sendOption);
            output.WritePacket(packet);
            return output;
        }

        public void Recycle() {
            Pool.Return(this);
        }

        public Span<byte> ToByteSpan() {
            return new Span<byte>(data, 0, WrittenPacketLength);
        }

        public ArraySegment<byte> ToArraySegment() {
            return new ArraySegment<byte>(data, 0, WrittenPacketLength);
        }

        public (IntPtr intPtr, int length) ToIntPtr() {
            return (new IntPtr(m_Buffer), WrittenPacketLength);
        }

        public void ToPointer(out byte* outputData, out int outputOffset, out int outputLength) {
            outputData = m_Buffer;
            outputOffset = 0;
            outputLength = WrittenPacketLength;
        }

        public void ToByteArray_NoCopy(out byte[] outputArray, out int outputOffset, out int outputLength) {
            outputArray = data;
            outputOffset = 0;
            outputLength = WrittenPacketLength;
        }

        public byte[] CopyToNewByteArray() {
            byte[] arr = new byte[WrittenPacketLength];
            Buffer.BlockCopy(data, 0, arr, 0, WrittenPacketLength);
            return arr;
        }

        #region Write

        public void Clear(ref byte[] buffer) {
            fixed (byte* bufferPtr = buffer) {
                this.m_Buffer = bufferPtr;
                this.m_Capacity = buffer.Length;
                m_Pos = 0;
            }
        }

        public void Clear(byte* bufferPtr, int capacity) {
            this.m_Buffer = bufferPtr;
            this.m_Capacity = capacity;
            m_Pos = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(byte* data, int bytes) {
            if (m_Capacity - m_Pos < bytes) {
                return;
            }

            Buffer.MemoryCopy(data, m_Buffer + m_Pos, m_Capacity - m_Pos, bytes);
            m_Pos += bytes;
        }

        public void WriteByte(byte value) {
            *(m_Buffer + m_Pos) = value;
            m_Pos += 1;
        }

        public void WriteByteArray(byte[] data) {
            ushort len = (ushort)data.Length;
            WriteUShort(len);

            fixed (byte* dataPtr = data) {
                WriteBytes(dataPtr, len);
            }
        }

        public void WriteShort(short value) {
            *(short*)(m_Buffer + m_Pos) = value;
            m_Pos += 2;
        }

        public void WriteUShort(ushort value) {
            *(ushort*)(m_Buffer + m_Pos) = value;
            m_Pos += 2;
        }

        public void WriteInt(int value) {
            *(int*)(m_Buffer + m_Pos) = value;
            m_Pos += 4;
        }

        public void WriteUInt(uint value) {
            *(uint*)(m_Buffer + m_Pos) = value;
            m_Pos += 4;
        }

        public void WriteLong(long value) {
            *(long*)(m_Buffer + m_Pos) = value;
            m_Pos += 4;
        }

        public void WriteULong(ulong value) {
            *(ulong*)(m_Buffer + m_Pos) = value;
            m_Pos += 4;
        }

        public void WriteFloat(float value) {
            *(float*)(m_Buffer + m_Pos) = value;
            m_Pos += 4;
        }

        public void WriteString_UNICODE(string value) {
            byte lengthInCharacters = (byte)value.Length;
            fixed (char* c = value) {
                WriteByte(lengthInCharacters);
                WriteBytes((byte*)c, lengthInCharacters * 2);
            }
        }

        public void WritePacket(IPacket packet) {
            WriteUShort((ushort)packet.type);
            packet.Write(this);
        }

        #endregion

    }

    public interface IRecycable {
        void Recycle();
    }

}
