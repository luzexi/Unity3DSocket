
using Game.Network;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

//  NetworkSever.cs
//  Lu Zexi
//  2012-8-23



namespace Game.Network.Sever
{
    public class NetworkSever : INetworkSever
    {
        protected IAcceptManager m_cAcceptMgr; //监听管理

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize()
        { 
            this.m_cAcceptMgr = new AcceptManager();
        }

        /// <summary>
        /// 逻辑更新
        /// </summary>
        /// <returns></returns>
        public virtual bool Update()
        {
            if (this.m_cAcceptMgr != null)
            {
                this.m_cAcceptMgr.Update();
            }
            return true;
        }

    }



}
