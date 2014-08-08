
using Game.Network;

//  IDummySession.cs
//  Lu Zexi
//  2012-10-04


namespace Game.Network
{

    /// <summary>
    /// 虚拟会话接口
    /// </summary>
    public interface IDummySession
    {
        void Receive(PacketBase pb);    //接收数据接口
        void Distroy();                 //销毁
    }

}


