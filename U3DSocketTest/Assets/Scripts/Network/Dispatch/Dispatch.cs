

using System.Collections;
using System.Collections.Generic;

//  Dispatch.cs
//  Lu Zexi
//  2012-8-25



namespace Game.Network
{

    /// <summary>
    /// 调度类
    /// </summary>
    public abstract class Dispatch
    {
        protected Dictionary<int, HandlerBase> m_mapHandlers = new Dictionary<int,HandlerBase>();   //句柄

        protected ISession m_cSession;  //会话对象
        protected NetQueue<PacketBase> m_cReceiveQueue;   //接收包队列

        public Dispatch()
        {
            this.m_cReceiveQueue = new NetQueue<PacketBase>(64 * 256);
            this.m_cReceiveQueue.Clear();
        }

        /// <summary>
        /// 获取会话
        /// </summary>
        /// <returns></returns>
        public ISession GetSession()
        {
            return this.m_cSession;
        }

        /// <summary>
        /// 设置会话类
        /// </summary>
        /// <param name="session"></param>
        public void SetSession( ISession session )
        {
            this.m_cSession = session;
        }

        /// <summary>
        /// 连接事件
        /// </summary>
        public virtual void OnConnect()
        { 
            //
        }

        /// <summary>
        /// 重连事件
        /// </summary>
        public virtual void OnReConnect()
        { 
            //
        }

        /// <summary>
        /// 断开连接事件
        /// </summary>
        public virtual void OnDisconnect()
        { 
            //
        }

        /// <summary>
        /// 接受消息
        /// </summary>
        /// <param name="pb"></param>
        public virtual bool AckPacket(PacketBase pb)
        {
            this.m_cReceiveQueue.Enqueue(pb);
            return true;
        }

        /// <summary>
        /// 增加句柄
        /// </summary>
        /// <param name="define"></param>
        /// <param name="handler"></param>
        protected void AddHandler(int define, HandlerBase handler)
        {
            if (this.m_mapHandlers.ContainsKey((int)define))
            {
                //Error;
                return;
            }
            this.m_mapHandlers.Add((int)define, handler);
        }

        /// <summary>
        /// 逻辑更新
        /// </summary>
        /// <returns></returns>
        public virtual bool Update()
        {
            for (int i = 0; i < 5; i++)
            {
                PacketBase pb;
                bool done = this.m_cReceiveQueue.Dequeue( out pb);
                if (done && this.m_mapHandlers.ContainsKey(pb.GetPacketID()))
                {
                    this.m_mapHandlers[pb.GetPacketID()].Execute(this, pb);
                }
            }

            return true;
        }

    }

}
