

using Game.Network.Sever;

//  DummySession.cs
//  Lu Zexi
//  2012-9-18


namespace Game.Network
{
    /// <summary>
    /// 虚拟会话类
    /// </summary>
    public class DummyClientSession : IDummyClientSession
    {
        private int m_iPort;    //连接端口
        private Dispatch m_cDispatch;   //调度类
        private DispatchFactoryBase m_cDispatchFactory; //调度工厂对象
        private SESSION_STATUS m_cStatus;   //会话状态

        public DummyClientSession(DispatchFactoryBase dispatchFactory)
        {
            this.m_cDispatchFactory = dispatchFactory;
        }

        /// <summary>
        /// 获取连接状态
        /// </summary>
        /// <returns></returns>
        public override SESSION_STATUS GetStatus()
        {
            return this.m_cStatus;
        }

        /// <summary>
        /// 设置重连时间
        /// </summary>
        /// <param name="second"></param>
        public override void SetReConnectSecond(int second)
        {
            //
        }

        /// <summary>
        /// 更换
        /// </summary>
        /// <param name="status"></param>
        protected override void ChangeStatus(SESSION_STATUS status)
        {
            this.m_cStatus = status;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public override void Connect(string address, int port)
        {
            this.m_iPort = port;
            this.m_cDispatch = this.m_cDispatchFactory.Create(this);
            DummyAcceptManager.GetInstance().Connect(this, this.m_iPort);
            this.m_cDispatch.OnConnect();

            ChangeStatus(SESSION_STATUS.CONNECT_SUCCESS);
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        public override void ReConnect()
        {
            ChangeStatus(SESSION_STATUS.RE_CONNECT);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public override void DisConnect()
        {
            DummyAcceptManager.GetInstance().DisConnect(this, this.m_iPort);

            if( this.m_cDispatch != null)
                this.m_cDispatch.OnDisconnect();

            ChangeStatus(SESSION_STATUS.CONNECT_EXIT);
            return;
        }
        
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="pb"></param>
        public override void Send(PacketBase pb)
        {
            DummyAcceptManager.GetInstance().Send(this, this.m_iPort, pb);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="pb"></param>
        public override void Receive(PacketBase pb)
        { 
            if( this.m_cDispatch != null)
                this.m_cDispatch.AckPacket(pb);
        }

        /// <summary>
        /// 逻辑更新
        /// </summary>
        /// <returns></returns>
        public override bool Update()
        {
            if (this.m_cDispatch != null)
            {
                this.m_cDispatch.Update();
            }
            return true;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public override void Distroy()
        { 
            //
        }
    }

}
