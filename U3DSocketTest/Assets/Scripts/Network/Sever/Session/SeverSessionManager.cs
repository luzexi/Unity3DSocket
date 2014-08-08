

using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

//  SeverSessionManager.cs
//  Lu Zexi
//  2012-10-03


namespace Game.Network.Sever
{

    /// <summary>
    /// 服务端会话管理类
    /// </summary>
    public class SeverSessionManager : ISeverSessionManager
    {
        private List<ISession> m_lstSessions = new List<ISession>();    //会话对象集合
        private DispatchFactoryBase m_cDispatchFactory; //调度工厂对象

        public SeverSessionManager( DispatchFactoryBase dispatchFactory )
        {
            this.m_cDispatchFactory = dispatchFactory;
        }

        /// <summary>
        /// 创建服务端会话对象
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="dispatch"></param>
        /// <returns></returns>
        public ISession CreateSession( Socket socket )
        {
            SeverSession session = new SeverSession(socket, this.m_cDispatchFactory);
            this.m_lstSessions.Add(session);
            return session;
        }


        /// <summary>
        /// 删除会话对象
        /// </summary>
        /// <param name="session"></param>
        public void RemoveSession(ISession session)
        {
            this.m_lstSessions.Remove(session);
        }

        /// <summary>
        /// 逻辑更新
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            foreach (ISession item in this.m_lstSessions)
            {
                item.Update();
            }
            return true;
        }

    }

}
