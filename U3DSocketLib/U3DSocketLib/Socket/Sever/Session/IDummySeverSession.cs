




//  IDummySeverSession.cs
//  Lu Zexi
//  2012-10-06


namespace Game.Network.Sever
{

    /// <summary>
    /// 虚拟服务端会话抽象类
    /// </summary>
    public abstract class IDummySeverSession : ISeverSession , IDummySession
    {
        public abstract void Receive(PacketBase pb);    //接收数据接口
        public abstract void Distroy();                 //销毁
    }

}

