


//  HandlerBase.cs
//  Lu Zexi
//  2012-8-23


namespace Game.Network
{

    /// <summary>
    /// 句柄基础类
    /// </summary>
    public abstract class HandlerBase
    {
        public abstract int GetPacketID();  //获取包ID
        public abstract PACKET_EXC_RES Execute( Dispatch dispatch , PacketBase packet);  //执行包
    }

}

