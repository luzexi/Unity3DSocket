

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

//  SocketAsyncEventArgsAccept.cs
//  Lu Zexi
//  2012-10-03


namespace Game.Network.Sever
{

    /// <summary>
    /// 套接字异步参数
    /// </summary>
    public abstract class CSocketAsyncEventArgs : SocketAsyncEventArgs
    {
        public CSocketAsyncEventArgs()
        { 
            //
        }

        /// <summary>
        /// 调度
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="transferredNum"></param>
        public abstract bool Dispatch(object arg, int transferredNum);

    }

    /// <summary>
    /// 监听异步参数类
    /// </summary>
    public class CSocketAsyncEventArgsAccept : CSocketAsyncEventArgs
    {
        public CSocketAsyncEventArgsAccept()
        { 
            //
        }

        /// <summary>
        /// 调度方法
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="transferredNum"></param>
        /// <returns></returns>
        public override bool Dispatch(object arg, int transferredNum)
        {
            SeverSessionManager mgr = (SeverSessionManager)arg;
            SeverSession session = (SeverSession)mgr.CreateSession(this.AcceptSocket);
            session.Receive();
            return true;
        }
    }

    /// <summary>
    /// 接收异步参数类
    /// </summary>
    public class CSocketAsyncEventArgsRecv : CSocketAsyncEventArgs
    {
        public SeverSession m_cSession;    //套接字

        public CSocketAsyncEventArgsRecv()
        { 
            //
        }

        /// <summary>
        /// 调度方法
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="transferredNum"></param>
        /// <returns></returns>
        public override bool Dispatch(object arg, int transferredNum)
        {
            return true;
        }
    }

    /// <summary>
    /// 发送异步参数类
    /// </summary>
    public class CSocketAsyncEventArgsSend : CSocketAsyncEventArgs
    {
        public CSocketAsyncEventArgsSend()
        { 
            //
        }

        /// <summary>
        /// 调度方法
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="transferredNum"></param>
        /// <returns></returns>
        public override bool Dispatch(object arg, int transferredNum)
        {
            return true;
        }
    }


    /// <summary>
    /// 异步参数类工厂
    /// </summary>
    public class CSocketAsyncEventArgsFactory
    {
        private static CSocketAsyncEventArgsFactory m_cInstance;

        /// <summary>
        /// 获取静态实例对象
        /// </summary>
        /// <returns></returns>
        public static CSocketAsyncEventArgsFactory GetInstance()
        {
            if (m_cInstance == null)
            {
                m_cInstance = new CSocketAsyncEventArgsFactory();
            }

            return m_cInstance;
        }


        /// <summary>
        /// 创建监听异步参数
        /// </summary>
        /// <returns></returns>
        public CSocketAsyncEventArgsAccept CreateAccept()
        {
            CSocketAsyncEventArgsAccept e = new CSocketAsyncEventArgsAccept();
            return e;
        }

        /// <summary>
        /// 创建接收异步参数
        /// </summary>
        /// <returns></returns>
        public CSocketAsyncEventArgsRecv CreateRecv()
        {
            CSocketAsyncEventArgsRecv e = new CSocketAsyncEventArgsRecv();
            return e;
        }

        /// <summary>
        /// 创建发送异步参数
        /// </summary>
        /// <returns></returns>
        public CSocketAsyncEventArgsSend CreateSend()
        {
            CSocketAsyncEventArgsSend e = new CSocketAsyncEventArgsSend();
            return e;
        }


    }

}

