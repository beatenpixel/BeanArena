using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace CrewNetwork {

    public unsafe struct WriteReadBuffer {

        private byte* m_Buffer;
        private int m_Capacity;
        private int m_Pos;

        public int WrittenMessageLength => m_Pos;

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

        public WriteReadBuffer(ref byte[] buffer) {
            fixed (byte* bufferPtr = buffer) {
                this.m_Buffer = bufferPtr;
                this.m_Capacity = buffer.Length;
                m_Pos = 0;
            }
        }

        public WriteReadBuffer(byte* bufferPtr, int capacity) {
            this.m_Buffer = bufferPtr;
            this.m_Capacity = capacity;
            m_Pos = 0;
        }

        // ================== Write ========================

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

        // ================== Read ========================

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

    }

}