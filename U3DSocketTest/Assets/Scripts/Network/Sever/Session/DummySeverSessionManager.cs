

using System.Collections;
using System.Collections.Generic;


//  DummySeverSessionManager.cs
//  Lu Zexi
//  2012-10-04


namespace Game.Network.Sever
{


    /// <summary>
    /// 虚拟服务端会话管理类
    /// </summary>
    public class DummySeverSessionManager : ISeverSessionManager
    {
        private DispatchFactoryBase m_cDispatchFactory; //调度工厂对象
        private List<IDummySeverSession> m_lstSessions = new List<IDummySeverSession>();  //会话集合
        private int m_iPort;    //监听端口

        public DummySeverSessionManager( DispatchFactoryBase dispatchFactory , int port )
        {
            this.m_iPort = port;
            this.m_cDispatchFactory = dispatchFactory;
        }

        /// <summary>
        /// 创建会话对象
        /// </summary>
        /// <returns></returns>
        public IDummySeverSession CreateSession()
        {
            DummySeverSession session = new DummySeverSession(this.m_cDispatchFactory, this.m_iPort);
            this.m_lstSessions.Add(session);
            return session;
        }

        /// <summary>
        /// 逻辑更新
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            foreach (IDummySeverSession item in this.m_lstSessions)
            {
                item.Update();
            }
            return true;
        }

        /// <summary>
        /// 删除会话对象
        /// </summary>
        /// <param name="session"></param>
        public void RemoveSession(IDummySeverSession session)
        { 
            this.m_lstSessions.Remove(session);
        }

    }

}


