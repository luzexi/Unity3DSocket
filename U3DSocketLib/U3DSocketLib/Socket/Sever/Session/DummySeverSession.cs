

using System;
using Game.Network;

//  DummySeverSession.cs
//  Lu Zexi
//  2012-10-04


namespace Game.Network.Sever
{

    /// <summary>
    /// 虚拟服务端会话类
    /// </summary>
    public class DummySeverSession : IDummySeverSession
    {
        private SESSION_STATUS m_cStatus;   //会话状态
        private Dispatch m_cDispatch;   //调度对象
        private DispatchFactoryBase m_cDispatchFactory; //调度工厂对象
        private int m_iPort;

        public DummySeverSession(DispatchFactoryBase dispatchFactory, int port)
        {
            this.m_cDispatchFactory = dispatchFactory;
            this.m_cDispatch = dispatchFactory.Create(this);
            this.m_iPort = port;
        }

        /// <summary>
        /// 获取会话状态
        /// </summary>
        /// <returns></returns>
        public override SESSION_STATUS GetStatus()
        {
            return this.m_cStatus;
        }

        /// <summary>
        /// 更换会话状态
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
            //
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
            //
            DummyAcceptManager.GetInstance().DisConnect(this, this.m_iPort);
            ChangeStatus(SESSION_STATUS.CONNECT_EXIT);
        }
        
        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="pb"></param>
        public override void Send(PacketBase pb)
        {
            DummyAcceptManager.GetInstance().Send(this, this.m_iPort, pb);
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
        /// 接收信息包
        /// </summary>
        /// <param name="pb"></param>
        public override void Receive(PacketBase pb)
        {
            if (this.m_cDispatch != null)
            {
                this.m_cDispatch.AckPacket(pb);
            }
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

