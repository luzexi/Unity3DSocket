


//  ISession.cs
//  Lu Zexi
//  2012-9-18


namespace Game.Network
{

    /// <summary>
    /// 会话接口
    /// </summary>
    public interface ISession
    {
        SESSION_STATUS GetStatus(); //获取状态
        void Connect(string address, int port); //连接
        void ReConnect();   //重连
        void DisConnect();  //断开连接
        void Send(PacketBase pb);   //发送数据包
        bool Update();  //逻辑更新

    }

}



