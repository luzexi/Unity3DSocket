
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

//  StreamBuffer.cs
//  Lu Zexi
//  2012-8-23


namespace Game.Network
{
    public class StreamBuffer
    {
        private const int DEFAULT_BUFFER_SIZE = 256;
        private const int ADDITION_SIZE = 256;

        public byte[] m_lstBuffer; //buffer
        private int m_iRead;    //read index
        public int ReadIndex
        {
            get{ return m_iRead; }
        }
        private int m_iWrite;   //write index
        public int WriteIndex
        {
            get{ return m_iWrite;}
        }
        private int m_iSize;    //buff size

        public StreamBuffer()
        {
            this.m_lstBuffer = new byte[DEFAULT_BUFFER_SIZE];
            this.m_iSize = 0;
            this.m_iRead = this.m_iWrite = 0;
        }

        public StreamBuffer(byte[] buffer)
        {
            this.m_lstBuffer = buffer;
            this.m_iSize = buffer.Length;
            this.m_iRead = this.m_iWrite = 0;
        }

        //clear buffer
        public void Clear()
        {
            this.m_lstBuffer = new byte[16];
            this.m_iSize = 0;
            this.m_iRead = 0;
            this.m_iWrite = 0;
        }

        //get size
        public int GetSize()
        {
            return this.m_iSize;
        }

        //write data
        public void Write(byte[] buffer, int size)
        {
            if (this.m_iWrite + size >= this.m_lstBuffer.Length)
            {
                FormationBuffer();
            }
            if (this.m_iWrite + size >= this.m_lstBuffer.Length)
            {
                int size_num = size/ADDITION_SIZE + 1;
                AdditionSize(size_num);
            }
            //Array.Reverse(buffer);//大小端转换
            Array.Copy(buffer, 0, this.m_lstBuffer, this.m_iWrite, size);
            Write(size);
        }

        public void Write(byte[] buffer)
        {
            Write(buffer, buffer.Length);
        }

        //write index
        private void Write(int size)
        {
            this.m_iWrite += size;
            this.m_iSize += size;
        }

        #region Write data override

        //write data
        public void WriteData(int data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(uint data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(float data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(short data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(ushort data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(long data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(ulong data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(byte data)
        {
            byte[] res = new byte[1];
            res[0] = data;
            Write(res, res.Length);
        }

        public void WriteData(double data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(bool data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(string data)
        {
            foreach (char item in data.ToCharArray())
            {
                byte[] res = BitConverter.GetBytes(item);
                Write(res, res.Length);
            }
        }

        public void WriteData(char data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(char[] data)
        {
            foreach (char item in data)
            {
                byte[] res = BitConverter.GetBytes(item);
                Write(res, res.Length);
            }
        }

        #endregion

        #region Read

        //read data
        public byte[] Read(int size)
        {
            if (this.m_iRead + size > this.m_iWrite)
            {
                Debug.LogError("Buffer is smaller than read, must be something error.");
                return null;
            }

            byte[] tmpBuffer = new byte[size];

            Array.Copy(this.m_lstBuffer, this.m_iRead, tmpBuffer, 0, size);
            this.m_iRead += size;
            this.m_iSize -= size;

            return tmpBuffer;
        }

        public int ReadInt32()
        {
            byte[] res = Read(sizeof(int));
            return BitConverter.ToInt32(res, 0);
        }

        public uint ReadUInt32()
        {
            byte[] res = Read(sizeof(uint));
            return BitConverter.ToUInt32(res, 0);
        }

        public short ReadInt16()
        {
            byte[] res = Read(sizeof(short));
            return BitConverter.ToInt16(res, 0);
        }

        public ushort ReadUInt16()
        {
            byte[] res = Read(sizeof(ushort));
            return BitConverter.ToUInt16(res, 0);
        }

        public long ReadInt64()
        {
            byte[] res = Read(sizeof(long));
            return BitConverter.ToInt64(res, 0);
        }

        public ulong ReadUInt64()
        {
            byte[] res = Read(sizeof(long));
            return BitConverter.ToUInt64(res, 0);
        }

        public bool ReadBool()
        {
            byte[] res = Read(sizeof(bool));
            return BitConverter.ToBoolean(res, 0);
        }

        public float ReadFloat()
        {
            byte[] res = Read(sizeof(float));
            return BitConverter.ToSingle(res, 0);
        }

        public double ReadDouble()
        {
            byte[] res = Read(sizeof(double));
            return BitConverter.ToDouble(res, 0);
        }

        public byte ReadByte()
        {
            byte[] res = Read(sizeof(byte));
            return res[0];
        }

        public char ReadChar()
        {
            byte[] res = Read(sizeof(char));
            return BitConverter.ToChar(res, 0);
        }

        public char[] ReadChars(int num)
        {
            char[] data = new char[num];
            for (int i = 0; i < num; i++)
            {
                byte[] res = Read(sizeof(char));
                data[i] = BitConverter.ToChar(res, 0);
            }
            return data;
        }

        public string ReadStr(int num)
        {
            string data = "";
            for (int i = 0; i < num; i++)
            {
                byte[] res = Read(sizeof(char));
                data += BitConverter.ToChar(res, 0);
            }
            return data;
        }

        #endregion


        //addition size
        private void AdditionSize(int num = 1)
        {
            byte[] array = new byte[m_lstBuffer.Length + ADDITION_SIZE * num];
            Array.Copy(this.m_lstBuffer, this.m_iRead, array, 0, this.m_iSize);
            m_lstBuffer = array;
            this.m_iRead = 0;
            this.m_iWrite = this.m_iRead + this.m_iSize;
        }
        
        //format buff
        private void FormationBuffer()
        {
            int index = 0;
            for(int i = 0 ; i<m_iSize ; i++)
            {
                m_lstBuffer[index] = m_lstBuffer[m_iRead + i];
                index++;
            }
            this.m_iRead = 0;
            this.m_iWrite = this.m_iRead + this.m_iSize;
        }

    }

}

