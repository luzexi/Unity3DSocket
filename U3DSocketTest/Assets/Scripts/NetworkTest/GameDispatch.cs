using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Game.Network;

//  GameDispatch.cs
//  Author: Lu zexi
//  2013-11-07



namespace Game
{

    /// <summary>
    /// 网络数据包调度
    /// </summary>
	class GameDispatch : Dispatch
	{
        public GameDispatch()
        { 

            //

        }

        /// <summary>
        /// 连接事件
        /// </summary>
        public override void OnConnect()
        {
            base.OnConnect();
            UnityEngine.Debug.Log("OnConnect");
            SendAgent.SendVerifyCMsg();
        }

        /// <summary>
        /// 重连事件
        /// </summary>
        public override void OnReConnect()
        {
            base.OnReConnect();
            UnityEngine.Debug.Log("OnReConnect");
        }

        /// <summary>
        /// 断开事件
        /// </summary>
        public override void OnDisconnect()
        {
            base.OnDisconnect();
            UnityEngine.Debug.Log("OnDisConnect");
        }

	}
}
