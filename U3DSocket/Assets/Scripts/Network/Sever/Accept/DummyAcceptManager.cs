

using System.Collections;
using System.Collections.Generic;
using Game.Network;


//  DummyAcceptManager.cs
//  Lu Zexi
//  2012-10-04


namespace Game.Network.Sever
{

    /// <summary>
    /// 虚拟监听管理类
    /// </summary>
    public class DummyAcceptManager : IAcceptManager
    {
        private static DummyAcceptManager m_cInstance;  //静态实例

        private Dictionary<int, DummyAccept> m_mapAccept = new Dictionary<int, DummyAccept>();

        public DummyAcceptManager()
        { 
            //
        }

        /// <summary>
        /// 获取静态实例
        /// </summary>
        /// <returns></returns>
        public static DummyAcceptManager GetInstance()
        {
            if (m_cInstance == null)
            {
                m_cInstance = new DummyAcceptManager();
            }
            return m_cInstance;
        }

        /// <summary>
        /// 创建监听对象
        /// </summary>
        /// <param name="port"></param>
        /// <param name="dispatchFactory"></param>
        public void CreateAccept(int port, DispatchFactoryBase dispatchFactory)
        {
            DummyAccept accept = new DummyAccept(port, dispatchFactory);
            if (this.m_mapAccept.ContainsKey(port))
            {
                //ERROR
                return;
            }
            this.m_mapAccept.Add(port, accept);
            accept.Initialize();
        }


        /// <summary>
        /// 连接
        /// </summary>
        public bool Connect(IDummySession session, int port)
        {
            if (this.m_mapAccept.ContainsKey(port))
            {
                this.m_mapAccept[port].Connect(session);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="session"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool DisConnect(IDummySession session, int port)
        {
            if (this.m_mapAccept.ContainsKey(port))
            {
                this.m_mapAccept[port].DisConnect(session);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="session"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Send(IDummySession session, int port, PacketBase pb)
        {
            if (this.m_mapAccept.ContainsKey(port))
            {
                this.m_mapAccept[port].Receive(session, pb);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 逻辑更新
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            foreach (DummyAccept item in this.m_mapAccept.Values)
            {
                item.Update();
            }
            return true;
        }

    }


}





