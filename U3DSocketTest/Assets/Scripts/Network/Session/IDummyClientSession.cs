



//  IDummyClientSession.cs
//  Lu Zexi
//  2012-10-06



namespace Game.Network
{

    /// <summary>
    /// 虚拟客户端会话抽象类
    /// </summary>
    public abstract class IDummyClientSession : IClientSession , IDummySession
    {
        public abstract void Receive(PacketBase pb);    //接收数据接口
        public abstract void Distroy();                 //销毁
    }

}



