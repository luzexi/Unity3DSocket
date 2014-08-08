

//  HandlerFactory.cs
//  Lu Zexi
//  2012-8-23


//namespace Game.Network
//{
//    /// <summary>
//    /// 句柄工厂类
//    /// </summary>
//    public class HandlerManager
//    {
        
//        private HandlerBase[] m_lstHandles = null;  //句柄队列


//        private static HandlerManager m_cInstance;  //静态实例
//        public static HandlerManager GetInstance()
//        {
//            if (m_cInstance == null)
//            {
//                m_cInstance = new HandlerManager();
//            }
//            return m_cInstance;
//        }

//        /// <summary>
//        /// 初始化句柄队列大小
//        /// </summary>
//        /// <param name="count"></param>
//        public void Init(int count)
//        {
//            this.m_lstHandles = new HandlerBase[count];
//            for (int i = 0; i < this.m_lstHandles.Length; i++)
//            {
//                this.m_lstHandles[i] = null;
//            }
//        }

//        /// <summary>
//        /// 增加句柄
//        /// </summary>
//        /// <param name="handle"></param>
//        public void AddHandle(HandlerBase handle)
//        {
//            if ( this.m_lstHandles == null|| handle.GetPacketID() < 0 || handle.GetPacketID() >= this.m_lstHandles.Length || this.m_lstHandles[handle.GetPacketID()] != null)
//            {
//                return;
//            }
//            this.m_lstHandles[handle.GetPacketID()] = handle;
//        }

//        /// <summary>
//        /// 执行包对应句柄
//        /// </summary>
//        /// <param name="packId"></param>
//        /// <param name="pack"></param>
//        /// <returns></returns>
//        public PACKET_EXC_RES Execute(PacketBase pack)
//        {
//            short packId = pack.GetPacketID();
//            if ( this.m_lstHandles == null || packId <0 || packId >= this.m_lstHandles.Length || this.m_lstHandles[packId] == null)
//            {
//                return PACKET_EXC_RES.PACKET_EXC_CANNOT_FIND_HANDLE;
//            }
//            return this.m_lstHandles[packId].Execute(pack);
//        }

//    }

//}
