


using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


//  Accept.cs
//  Lu Zexi
//  2012-10-01



namespace Game.Network.Sever
{

    /// <summary>
    /// 监听SOCKET类
    /// </summary>
    public class Accept : IAccept
    {
        protected Socket m_cListener;   //监听套接字
        protected int m_iPort;  //监听端口
        protected DispatchFactoryBase m_cDispatchFactory;    //调度工厂对象
        private SeverSessionManager m_cSeverMgr;    //会话管理类

        public Accept(int port, DispatchFactoryBase dispatchFactory)
        {
            this.m_iPort = port;
            this.m_cDispatchFactory = dispatchFactory;
            this.m_cSeverMgr = new SeverSessionManager( this.m_cDispatchFactory );
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            this.m_cListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint point = new IPEndPoint(IPAddress.Any, m_iPort);
            this.m_cListener.Bind(point);
            this.m_cListener.Listen(10);
            StartAccept();
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        private void StartAccept()
        {
            CSocketAsyncEventArgsAccept e;
            e = CSocketAsyncEventArgsFactory.GetInstance().CreateAccept();
            e.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptCompleteCallBack);

            bool done = this.m_cListener.AcceptAsync(e);
            if (!done)
            {
                AcceptProcess(e);
            }
        }

        /// <summary>
        /// 接受处理过程
        /// </summary>
        /// <param name="arg"></param>
        private void AcceptProcess(SocketAsyncEventArgs arg)
        {
            CSocketAsyncEventArgs tmp = (CSocketAsyncEventArgs)arg;
            tmp.Dispatch(this.m_cSeverMgr, tmp.BytesTransferred);
            StartAccept();
        }

        /// <summary>
        /// 接受SOCKET回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptCompleteCallBack(object key, SocketAsyncEventArgs e)
        {
            AcceptProcess(e);
        }

        /// <summary>
        /// 逻辑更新
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            if (this.m_cSeverMgr != null)
            {
                this.m_cSeverMgr.Update();
            }
            return true;
        }

    }

}

