using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//  SendAgent.cs
//  Author: Lu Zexi
//  2013-11-07



namespace Game
{
    /// <summary>
    /// 发送数据代理
    /// </summary>
	class SendAgent
	{
        public static void SendVerifyCMsg()
        {
            UnityEngine.Debug.Log("sendverify");
            VerifyCMsg pack = new VerifyCMsg();
            ClientSessionManager.GetInstance().Send(0, pack);
        }
	}
}
