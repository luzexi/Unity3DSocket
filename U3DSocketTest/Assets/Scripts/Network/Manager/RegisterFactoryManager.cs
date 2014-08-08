


//  RegisterFactoryManager.cs
//  Lu Zexi
//  2012-8-23


//namespace Game.Network
//{
//    /// <summary>
//    /// 注册工厂管理类
//    /// </summary>
//    public class RegisterFactoryManager
//    {
//        private static RegisterFactoryManager m_cInstance;  //静态实例
//        public static RegisterFactoryManager GetInstance()
//        {
//            if (m_cInstance == null)
//            {
//                m_cInstance = new RegisterFactoryManager();
//            }
//            return m_cInstance;
//        }

//        /// <summary>
//        /// 注册工厂
//        /// </summary>
//        /// <param name="factory"></param>
//        /// <param name="handle"></param>
//        public void RegistFactory(PacketFactory factory, HandlerBase handle)
//        {
//            if (factory != null)
//            {
//                //注册包
//                PacketFactoryManager.GetInstance().AddFactory(factory);
//            }

//            if (handle != null)
//            {
//                //注册包的句柄
//                HandlerManager.GetInstance().AddHandle(handle);
//            }
//        }
//    }

//}

