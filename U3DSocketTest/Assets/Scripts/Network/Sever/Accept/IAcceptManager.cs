


using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

//  IAcceptManager.cs
//  Lu Zexi
//  2012-10-03


namespace Game.Network.Sever
{

    /// <summary>
    /// 监听管理接口
    /// </summary>
    public interface IAcceptManager
    { 
        void CreateAccept(int port, DispatchFactoryBase dispatchFactory);   //创建监听
        bool Update();  //逻辑更新
    }

}



