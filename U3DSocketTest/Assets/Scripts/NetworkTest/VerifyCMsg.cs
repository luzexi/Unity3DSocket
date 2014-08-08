using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.Network;

//  VerifyCMsg.cs
//  Author: Lu Zexi
//  2013-11-07


namespace Game
{
    /// <summary>
    /// 验证数据包
    /// </summary>
	class VerifyCMsg : PacketBase
	{
        public short m_sTaskType;   //连接任务类型
        public uint m_uiVersion;    //版本号
        public ulong m_ulVerifyCode;    //验证码

        public VerifyCMsg()
            :base()
        {
            this.m_usPacketId = 0x3;
            this.m_sTaskType = 1;
            this.m_uiVersion = 100001;
            this.m_ulVerifyCode = 9876543210L;
        }

        /// <summary>
        /// 获取包大小
        /// </summary>
        /// <returns></returns>
        public override ushort GetSize()
        {
            return (ushort)(sizeof(short) + sizeof(uint) + sizeof(ulong) + base.GetSize());
        }

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="stream"></param>
        public override void Write(StreamBuffer stream)
        {
            base.Write(stream);
            stream.WriteData(this.m_sTaskType);
            stream.WriteData(this.m_uiVersion);
            stream.WriteData(this.m_ulVerifyCode);
        }
	}
}
