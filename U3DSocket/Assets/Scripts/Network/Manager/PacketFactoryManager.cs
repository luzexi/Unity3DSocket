


//  packetFactoryManager.cs
//  Lu Zexi
//  2012-8-23


namespace Game.Network
{

    /// <summary>
    /// 包工厂管理类
    /// </summary>
    public class PacketFactoryManager
    {
        protected static PacketFactoryManager m_cInstance;  //静态实例
        public static PacketFactoryManager GetInstance()
        {
            if (m_cInstance == null)
            {
                m_cInstance = new PacketFactoryManager();
            }
            return m_cInstance;
        }

        protected PacketFactory[] m_lstPacketFactorys = null; //包工厂列表

        public PacketFactoryManager()
        { 
            //
        }

        /// <summary>
        /// 初始化包列表大小
        /// </summary>
        /// <param name="count"></param>
        public void Init(int count)
        {
            this.m_lstPacketFactorys = new PacketFactory[count];
        }

        /// <summary>
        /// 获取包工厂数量
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            if (this.m_lstPacketFactorys == null)
                return 0;
            return this.m_lstPacketFactorys.Length;
        }

        /// <summary>
        /// 增加工厂
        /// </summary>
        /// <param name="factory"></param>
        public void AddFactory(PacketFactory factory)
        {
            if (factory.GetPacketId() < 0 || this.m_lstPacketFactorys == null ||
                this.m_lstPacketFactorys.Length <= factory.GetPacketId() || this.m_lstPacketFactorys[factory.GetPacketId()] != null)
            {
                return;
            }
            this.m_lstPacketFactorys[factory.GetPacketId()] = factory;
        }

        /// <summary>
        /// 创建指定包实例
        /// </summary>
        /// <param name="packetId"></param>
        /// <returns></returns>
        public PacketBase CreatePacket(int packetId)
        {
            if ( this.m_lstPacketFactorys == null || packetId < 0 || packetId >= this.m_lstPacketFactorys.Length || this.m_lstPacketFactorys[packetId] == null)
            {
                return null;
            }
            return this.m_lstPacketFactorys[packetId].Create();
        }

        /// <summary>
        /// 获取指定包大小
        /// </summary>
        /// <param name="packetId"></param>
        /// <returns></returns>
        public int GetPackSize(int packetId)
        {
            if ( this.m_lstPacketFactorys == null || packetId <0 || packetId >= this.m_lstPacketFactorys.Length || this.m_lstPacketFactorys[packetId] == null)
            {
                return 0;
            }
            return this.m_lstPacketFactorys[packetId].GetPacketSize();
        }


    }


}
