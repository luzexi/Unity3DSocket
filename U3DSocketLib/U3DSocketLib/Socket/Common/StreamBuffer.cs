
using System;
using System.IO;
using System.Runtime.InteropServices;
using Game.Network.Tool;

//  StreamBuffer.cs
//  Lu Zexi
//  2012-8-23


namespace Game.Network
{
    /// <summary>
    /// 缓冲流
    /// </summary>
    public class StreamBuffer
    {
        private const int DEFAULT_BUFFER_SIZE = 64 * 256;
        public byte[] m_lstBuffer; //缓存
        private int m_iRead;    //读坐标
        public int ReadIndex
        {
            get { return this.m_iRead; }
        }
        private int m_iWrite;   //写坐标
        public int WriteIndex
        {
            get { return this.m_iWrite; }
        }
        private int m_iSize;    //缓存大小

        public StreamBuffer()
        {
            this.m_lstBuffer = new byte[DEFAULT_BUFFER_SIZE];
            this.m_iSize = 0;
            this.m_iRead = this.m_iWrite = 0;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        public void Init(byte[] buffer, int size)
        {
            this.m_lstBuffer = buffer;
            this.m_iSize = size;
        }

        /// <summary>
        /// 获取缓冲长度
        /// </summary>
        /// <returns></returns>
        public int GetLength()
        {
            if (this.m_lstBuffer == null)
                return 0;
            return this.m_lstBuffer.Length;
        }

        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool Write(byte[] buffer, int size)
        {
            if (this.m_iWrite + size >= this.m_lstBuffer.Length)
            {
                //ERROR
                WriteFiles.WritFile.LogCallBack(LogerType.ERROR, "Buffer is not enough.");
                return false;
            }
            //Array.Reverse(buffer);//大小端转换
            Array.Copy(buffer, 0, this.m_lstBuffer, this.m_iWrite, size);
            Write(size);
            return true;
        }

        /// <summary>
        /// 增加已写入BUFF数据的写头索引
        /// </summary>
        /// <param name="size"></param>
        public void Write(int size)
        {
            this.m_iWrite += size;
            this.m_iSize += size;
        }

        #region WriteData重载

        /// <summary>
        /// 写入int数据
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(int data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        /// <summary>
        /// 写入 uint 数据
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(uint data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        /// <summary>
        /// 写入float数据
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(float data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        /// <summary>
        /// 写入short数据
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(short data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        /// <summary>
        /// 写入ushort数据
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(ushort data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        /// <summary>
        /// 写入long数据
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(long data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        /// <summary>
        /// 写入ulong数据
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(ulong data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        /// <summary>
        /// 写入byte数据
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(byte data)
        {
            byte[] res = new byte[1];
            res[0] = data;
            Write(res, res.Length);
        }

        /// <summary>
        /// 写入double数据
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(double data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        /// <summary>
        /// 写入bool数据
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(bool data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        /// <summary>
        /// 写入string数据
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(string data)
        {
            foreach (char item in data.ToCharArray())
            {
                byte[] res = BitConverter.GetBytes(item);
                Write(res, res.Length);
            }
        }

        /// <summary>
        /// 写入char数据
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(char data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        /// <summary>
        /// 写入char[]数据
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(char[] data)
        {
            foreach (char item in data)
            {
                byte[] res = BitConverter.GetBytes(item);
                Write(res, res.Length);
            }
        }

        #endregion

        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public byte[] Read(int size)
        {
            if (this.m_iRead + size > this.m_iWrite)
            {
                //ERROR
                WriteFiles.WritFile.LogCallBack(LogerType.ERROR, "Buffer is not have enough content.");
                return null;
            }

            byte[] tmpBuffer = new byte[size];

            Array.Copy(this.m_lstBuffer, this.m_iRead, tmpBuffer, 0, size);
            this.m_iRead += size;
            this.m_iSize -= size;

            return tmpBuffer;
        }

        #region Read 方法集合
        /// <summary>
        /// 读取 int
        /// </summary>
        /// <returns></returns>
        public int ReadInt32()
        {
            byte[] res = Read(sizeof(int));
            return BitConverter.ToInt32(res, 0);
        }

        /// <summary>
        /// 读取 uint
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt32()
        {
            byte[] res = Read(sizeof(uint));
            return BitConverter.ToUInt32(res, 0);
        }

        /// <summary>
        /// 读取 short
        /// </summary>
        /// <returns></returns>
        public short ReadInt16()
        {
            byte[] res = Read(sizeof(short));
            return BitConverter.ToInt16(res, 0);
        }

        /// <summary>
        /// 读取 ushort
        /// </summary>
        /// <returns></returns>
        public ushort ReadUInt16()
        {
            byte[] res = Read(sizeof(ushort));
            return BitConverter.ToUInt16(res, 0);
        }

        /// <summary>
        /// 读取 long
        /// </summary>
        /// <returns></returns>
        public long ReadInt64()
        {
            byte[] res = Read(sizeof(long));
            return BitConverter.ToInt64(res, 0);
        }

        /// <summary>
        /// 读取 ulong
        /// </summary>
        /// <returns></returns>
        public ulong ReadUInt64()
        {
            byte[] res = Read(sizeof(long));
            return BitConverter.ToUInt64(res, 0);
        }

        /// <summary>
        /// 读取 bool
        /// </summary>
        /// <returns></returns>
        public bool ReadBool()
        {
            byte[] res = Read(sizeof(bool));
            return BitConverter.ToBoolean(res, 0);
        }

        /// <summary>
        /// 读取 float
        /// </summary>
        /// <returns></returns>
        public float ReadFloat()
        {
            byte[] res = Read(sizeof(float));
            return BitConverter.ToSingle(res, 0);
        }

        /// <summary>
        /// 读取 double
        /// </summary>
        /// <returns></returns>
        public double ReadDouble()
        {
            byte[] res = Read(sizeof(double));
            return BitConverter.ToDouble(res, 0);
        }

        /// <summary>
        /// 读取 byte
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            byte[] res = Read(sizeof(byte));
            return res[0];
        }

        /// <summary>
        /// 读取 char
        /// </summary>
        /// <returns></returns>
        public char ReadChar()
        {
            byte[] res = Read(sizeof(char));
            return BitConverter.ToChar(res, 0);
        }

        /// <summary>
        /// 读取 char串
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 读取 char串
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 格式化缓存
        /// </summary>
        private void FormationBuffer()
        {
            Array.Copy(this.m_lstBuffer, this.m_iRead, this.m_lstBuffer, 0, this.m_iSize);
            this.m_iRead = 0;
            this.m_iWrite = this.m_iRead + this.m_iSize;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns></returns>
        public byte[] GetBuffer()
        { 
            byte[] buf = new byte[this.m_iSize];
            Array.Copy(this.m_lstBuffer, this.m_iRead, buf , 0 , this.m_iSize);
            return buf;
        }

    }

}

