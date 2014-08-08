

using System.Collections;
using System.Collections.Generic;


//  DummyAccept.cs
//  Lu Zexi
//  2012-10-04


namespace Game.Network.Sever
{

    /// <summary>
    /// 虚拟监听类
    /// </summary>
    public class DummyAccept : IAccept
    {
        private int m_iPort;        //端口
        private DispatchFactoryBase m_cDispatchFactory;   //调度工厂对象
        private Dispatch m_cDispatch;   //调度工厂
        private Dictionary<IDummySession, IDummySession> m_mapLink;
        private DummySeverSessionManager m_cSessionMgr;

        public DummyAccept(int port , DispatchFactoryBase dispatchFactory)
        {
            this.m_iPort = port;
            this.m_cDispatchFactory = dispatchFactory;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            this.m_mapLink = new Dictionary<IDummySession, IDummySession>();
            this.m_cSessionMgr = new DummySeverSessionManager(this.m_cDispatchFactory , this.m_iPort);
        }

        /// <summary>
        /// 创建虚拟连接
        /// </summary>
        /// <param name="session"></param>
        public void Connect(IDummySession session)
        {
            if (this.m_mapLink.ContainsKey(session))
            {
                return;
            }
            IDummySession severSession = this.m_cSessionMgr.CreateSession();
            this.m_mapLink.Add(session, severSession);
            this.m_mapLink.Add(severSession, session);
        }
        
        /// <summary>
        /// 断开虚拟连接
        /// </summary>
        /// <param name="session"></param>
        public void DisConnect(IDummySession session)
        {
            if (!this.m_mapLink.ContainsKey(session))
            {
                return;
            }
            IDummySession tmpSession = this.m_mapLink[session];
            this.m_mapLink.Remove(session);
            this.m_mapLink.Remove(tmpSession);
        }

        /// <summary>
        /// 接收信息
        /// </summary>
        /// <param name="session"></param>
        public bool Receive(IDummySession session, PacketBase pb)
        {
            if (!this.m_mapLink.ContainsKey(session))
            {
                return false;
            }
            this.m_mapLink[session].Receive(pb);
            return true;
        }

        ///// <summary>
        ///// 发送数据
        ///// </summary>
        ///// <param name="session"></param>
        ///// <param name="pb"></param>
        ///// <returns></returns>
        //public bool Send(IDummySession session, PacketBase pb)
        //{
        //    if (!this.m_mapLink.ContainsKey(session))
        //    {
        //        return false;
        //    }
        //    this.m_mapLink[session].Send(pb);
        //    return true;
        //}

        /// <summary>
        /// 逻辑更新
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            if (this.m_cSessionMgr != null)
            {
                this.m_cSessionMgr.Update();
            }
            return true;
        }
    }

}




