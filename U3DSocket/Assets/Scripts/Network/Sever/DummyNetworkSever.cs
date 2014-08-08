



//  DummyNetworkSever.cs
//  Lu Zexi
//  2012-10-05


namespace Game.Network.Sever
{

    /// <summary>
    /// 虚拟网络服务端类
    /// </summary>
    public class DummyNetworkSever : INetworkSever
    {
        protected DummyAcceptManager m_cAcceptMgr;    //监听管理对象

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize()
        {
            this.m_cAcceptMgr = DummyAcceptManager.GetInstance();
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



