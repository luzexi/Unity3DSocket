


//  ISeverSession.cs
//  Lu Zexi
//  2012-10-06


namespace Game.Network.Sever
{

    /// <summary>
    /// 服务端会话抽象类
    /// </summary>
    public abstract class ISeverSession : ISession
    {
        public abstract SESSION_STATUS GetStatus(); //获取会话状态
        protected abstract void ChangeStatus(SESSION_STATUS status);    //更换状态
        public abstract void Connect(string address, int port); //建立连接
        public abstract void ReConnect();   //重新连接
        public abstract void DisConnect();  //断开连接
        public abstract void Send(PacketBase pb);   //发送数据包
        public abstract bool Update();  //逻辑更新
    }

}


