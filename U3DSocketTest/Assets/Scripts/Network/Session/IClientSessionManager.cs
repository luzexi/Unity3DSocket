



//  IClientSessionManager.cs
//  Lu Zexi
//  2012-10-05


namespace Game.Network
{

    /// <summary>
    /// 客户端会话管理接口
    /// </summary>
    public interface IClientSessionManager : ISessionManager
    {
        ISession CreateSession(int define, DispatchFactoryBase dispatchFactory);
        void Connect(int define, string address, int port);
        void DisConnect(int define);
        void Send(int define, PacketBase pb);
    }

}


