using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//  ClientSessionManager.cs
//  Author: Lu Zexi
//  2013-11-07

using Game.Network;



namespace Game
{
    /// <summary>
    /// 客户端会话接口管理
    /// </summary>
	class ClientSessionManager : IClientSessionManager
	{
        private ClientSession[] m_vecSession = new ClientSession[5];    //会话数组
        private Dispatch[] m_vecDispatch = new Dispatch[5]; //调度类

        private static ClientSessionManager s_cInstance;    //单例


        public ClientSessionManager()
        { 
            CreateSession( 0 , new DispatchFactory<GameDispatch>() );
        }

        /// <summary>
        /// 获取单例
        /// </summary>
        /// <returns></returns>
        public static ClientSessionManager GetInstance()
        {
            if (s_cInstance == null)
                s_cInstance = new ClientSessionManager();
            return s_cInstance;
        }

        /// <summary>
        /// 创建会话
        /// </summary>
        /// <param name="define"></param>
        /// <param name="dispatchFactory"></param>
        /// <returns></returns>
        public ISession CreateSession(int define, DispatchFactoryBase dispatchFactory)
        {
            if (this.m_vecSession.Length <= define)
            {
                //Error
                return null;
            }

            this.m_vecSession[define] = new ClientSession(dispatchFactory);

            return this.m_vecSession[define];
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="define"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public void Connect(int define, string address, int port)
        {
            if (this.m_vecSession.Length <= define)
            {
                //Error
                return;
            }

            if (this.m_vecSession[define] == null)
            {
                //Error
                return;
            }

            if (this.m_vecSession[define].GetStatus() == SESSION_STATUS.CONNECT_SUCCESS)
            {
                //Error
                return;
            }

            this.m_vecSession[define].Connect(address, port);

        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="define"></param>
        public void DisConnect(int define)
        {
            if (this.m_vecSession.Length <= define)
            {
                //Error
                return;
            }

            if (this.m_vecSession[define] == null)
            {
                //Error
                return;
            }

            this.m_vecSession[define].DisConnect();
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="define"></param>
        /// <param name="pb"></param>
        public void Send(int define, PacketBase pb)
        {
            if (this.m_vecSession.Length <= define)
            {
                //Error
                return;
            }

            if (this.m_vecSession[define] == null)
            {
                //Error
                return;
            }

            if (this.m_vecSession[define].GetStatus() != SESSION_STATUS.CONNECT_SUCCESS)
            {
                //Error
                return;
            }

            this.m_vecSession[define].Send(pb);
        }

        /// <summary>
        /// 逻辑更新
        /// </summary>
        public bool Update()
        {

            for (int i = 0; i < this.m_vecSession.Length; i++)
            {
                if ( this.m_vecSession[i] != null)
                {
                    this.m_vecSession[i].Update();
                }
                if ( i< this.m_vecDispatch.Length && this.m_vecDispatch[i] != null)
                {
                    this.m_vecDispatch[i].Update();
                }
            }

            return true;
        }
	}
}
