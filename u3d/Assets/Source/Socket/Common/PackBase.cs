

using System;
using System.IO;


//  PackBase.cs
//  Lu Zexi
//  2012-8-23


namespace Game.Network
{
    /// <summary>
    /// 基础包
    /// </summary>
    public class PacketBase
    {
        protected ushort m_usPacketId;   //包ID
        public ushort m_usPacketSize;   //包大小

        //private long m_lPublicKey;   //公钥
        //private long m_lPrivateKey;  //密钥

        public PacketBase()
        {
            this.m_usPacketId = 0;
            this.m_usPacketSize = 0;
        }

        /// <summary>
        /// 获取包ID
        /// </summary>
        /// <returns></returns>
        public ushort GetPacketID()
        {
            return this.m_usPacketId;
        }

        /// <summary>
        /// 获取包大小
        /// </summary>
        /// <returns></returns>
        public virtual ushort GetSize()
        {
            return (sizeof(ushort) + sizeof(ushort));
        }

        /// <summary>
        /// 获取数据比特流
        /// </summary>
        /// <returns></returns>
        public virtual void Write(StreamBuffer stream)
        {   
            stream.WriteData(this.m_usPacketSize);
            stream.WriteData(this.m_usPacketId);
        }

        /// <summary>
        /// 设置由比特流而得到的数据
        /// </summary>
        /// <param name="data"></param>
        public virtual void Load(StreamBuffer stream)
        {
            this.m_usPacketId = stream.ReadUInt16();
            this.m_usPacketSize = stream.ReadUInt16();
        }
    }

}
