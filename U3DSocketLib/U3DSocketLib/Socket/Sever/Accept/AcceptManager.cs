

using System.Collections;
using System.Collections.Generic;


//  AcceptManager.cs
//  Lu Zexi
//  2012-10-04



namespace Game.Network.Sever
{

    /// <summary>
    /// 服务端监听管理类
    /// </summary>
    public class AcceptManager : IAcceptManager
    {
        private List<Accept> m_lstAccepts = new List<Accept>();

        /// <summary>
        /// 创建监听对象
        /// </summary>
        /// <param name="port"></param>
        /// <param name="dispatchFactory"></param>
        public void CreateAccept(int port, DispatchFactoryBase dispatchFactory)
        { 
            Accept accept = new Accept(port, dispatchFactory);
            accept.Initialize();
            this.m_lstAccepts.Add(accept);
        }

        /// <summary>
        /// 逻辑更新
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            foreach (Accept item in this.m_lstAccepts)
            {
                item.Update();
            }
            return true;
        }

    }

}



