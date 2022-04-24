using CrewNetwork.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CrewNetwork {
    public unsafe class PacketReader : IRecycable {

        private static readonly ObjectPool<PacketReader> Pool = new ObjectPool<PacketReader>(() => new PacketReader(), 1024);

        private byte[] data;

        private byte* m_Buffer;
        private int m_Capacity;
        private int m_Pos;

        public int WrittenPacketLength => m_Pos;

        public PacketReader() {
            data = new byte[CrewNetCommon.MAX_PACKET_SIZE];
        }

        public void Recycle() {
            Pool.Return(this);
        }

        public static PacketReader Get(byte[] packetData) {
            PacketReader output = Pool.Get();
            output.Clear(ref packetData);
            return output;
        }

        public static PacketReader Get(IntPtr packetData, int packetLength) {
            PacketReader output = Pool.Get();
            output.Clear((byte*)packetData.ToPointer(), packetLength);
            return output;
        }

        public bool ReadNextMessage(out MessageType messageType) {
            if(WrittenPacketLength < m_Capacity) {
                messageType = (MessageType)ReadUShort();
                return true;
            }

            messageType = MessageType.None;
            return false;
        }

        #region Read

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
        public bool ReadBytes(byte* data, int length) {
            if (m_Pos >= m_Capacity)
                return false;

            Buffer.MemoryCopy(m_Buffer + m_Pos, data, length, length);
            m_Pos += length;
            return true;
        }

        public byte ReadByte() {
            byte data = *(m_Buffer + m_Pos);
            m_Pos += 1;
            return data;
        }

        public void ReadIntoByteArray(byte[] target) {
            ushort arrayLength = ReadUShort();
            fixed (byte* arrayPtr = target) {
                ReadBytes(arrayPtr, arrayLength);
            }
        }

        public byte[] ReadAndCreateByteArray() {
            ushort arrayLength = ReadUShort();
            byte[] array = new byte[arrayLength];
            fixed (byte* arrayPtr = array) {
                ReadBytes(arrayPtr, arrayLength);
            }
            return array;
        }

        public short ReadShort() {
            short data = *((short*)(m_Buffer + m_Pos));
            m_Pos += 2;
            return data;
        }

        public ushort ReadUShort() {
            ushort data = *((ushort*)(m_Buffer + m_Pos));
            m_Pos += 2;
            return data;
        }

        public int ReadInt() {
            int data = *((int*)(m_Buffer + m_Pos));
            m_Pos += 4;
            return data;
        }

        public uint ReadUInt() {
            uint data = *((uint*)(m_Buffer + m_Pos));
            m_Pos += 4;
            return data;
        }

        public long ReadLong() {
            long data = *((long*)(m_Buffer + m_Pos));
            m_Pos += 8;
            return data;
        }

        public ulong ReadULong() {
            ulong data = *((ulong*)(m_Buffer + m_Pos));
            m_Pos += 8;
            return data;
        }

        public float ReadFloat() {
            float data = *((float*)(m_Buffer + m_Pos));
            m_Pos += 4;
            return data;
        }

        public string ReadString_UNICODE(ref StringBuilder stringBuilder) {
            byte lengthInCharacters = ReadByte();
            stringBuilder.Clear();
            stringBuilder.Append((char*)(m_Buffer + m_Pos), lengthInCharacters);
            m_Pos += lengthInCharacters * 2;
            return stringBuilder.ToString();
        }

        public T ReadPacket<T>(MessageType msgType = MessageType.None) where T : IPacket {
            MessageType messageType = (msgType == MessageType.None)?(MessageType)ReadUShort():msgType;
            IPacket packet;

            switch(messageType) {
                case MessageType.CPacket_PlayerJoin: packet = new CPacket_PlayerJoin(); break;
                default:
                    packet = null;                    
                    break;
            }

            packet.Read(this);
            return (T)packet;
        }

        #endregion

    }
}
