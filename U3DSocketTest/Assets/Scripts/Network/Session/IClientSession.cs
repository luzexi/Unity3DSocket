



//  IClientSession.cs
//  Lu Zexi
//  2012-10-06


namespace Game.Network
{

    /// <summary>
    /// 客户端会话接口
    /// </summary>
    public abstract class IClientSession : ISession
    {
        //public delegate void ChangeStatusCallBack(SESSION_STATUS status);
        public abstract void SetReConnectSecond(int second);  //设置重连时间
        public abstract SESSION_STATUS GetStatus(); //获取当前状态
        protected abstract void ChangeStatus(SESSION_STATUS status);    //更换状态
        public abstract void Connect(string address, int port); //连接
        public abstract void ReConnect();   //重新连接
        public abstract void DisConnect();  //断开连接
        public abstract void Send(PacketBase pb);   //发送数据包
        public abstract bool Update();  //逻辑更新
    }
}

