

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

//  ISessionManager.cs
//  Lu Zexi
//  2012-10-03



namespace Game.Network
{

    /// <summary>
    /// 会话管理类
    /// </summary>
    public interface ISessionManager
    {
        bool Update();  //逻辑更新
    }

}


