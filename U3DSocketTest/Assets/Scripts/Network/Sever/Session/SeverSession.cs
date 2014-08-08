
using Game.Network;
using Game.Network.Tool;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


//  SeverSession.cs
//  Lu Zexi
//  2012-10-2


namespace Game.Network.Sever
{

    /// <summary>
    /// 服务端会话
    /// </summary>
    public class SeverSession : ISeverSession
    {
        private Socket m_cSocket;   //套接字
        private DispatchFactoryBase m_cDispatchFactory; //调度工厂对象
        private Dispatch m_cDispatch;   //调度对象
        private StreamBuffer m_cReceiveBuffer;  //接收缓存
        private StreamBuffer m_cSendBuffer;     //发送缓存
        private NetQueue<PacketBase> m_cSendQueue;  //发送包队列
        private SESSION_STATUS m_cStatus;   //会话状态

        public SeverSession(Socket socket, DispatchFactoryBase dispatchFactory)
        {
            this.m_cDispatchFactory = dispatchFactory;
            this.m_cDispatch = this.m_cDispatchFactory.Create( this );
            this.m_cSocket = socket;

            this.m_cReceiveBuffer = new StreamBuffer();
            this.m_cSendBuffer = new StreamBuffer();

            this.m_cSendQueue = new NetQueue<PacketBase>(64 * 256);
            this.m_cSendQueue.Clear();

        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public override void Connect(string address, int port)
        { 
            //
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        public override void ReConnect()
        {
            //
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public override void DisConnect()
        {
            try
            {
                if (this.m_cSocket != null)
                {
                    this.m_cSocket.Shutdown(SocketShutdown.Both);
                    this.m_cSocket.Close();
                }
                this.m_cSocket = null;
                ChangeStatus(SESSION_STATUS.CONNECT_EXIT);
            }
            catch (Exception e)
            {
                WriteFiles.WritFile.Log(LogerType.ERROR, e.ToString());
                ChangeStatus(SESSION_STATUS.CONNECT_FAILED_CONNECT_ERROR);
            }
        }

        public override bool Update()
        {
            if (this.m_cDispatch != null)
            {
                this.m_cDispatch.Update();
            }

            for (int i = 0; i < 5; i++)
            {
                Send();
            }

            return true;
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <returns></returns>
        public override SESSION_STATUS GetStatus()
        {
            return this.m_cStatus;
        }

        /// <summary>
        /// 更换状态
        /// </summary>
        /// <param name="status"></param>
        protected override void ChangeStatus(SESSION_STATUS status)
        {
            this.m_cStatus = status;
        }

        /// <summary>
        /// 接收
        /// </summary>
        public void Receive()
        {
            try
            {
                CSocketAsyncEventArgsRecv e = new CSocketAsyncEventArgsRecv();
                e.Completed += new EventHandler<SocketAsyncEventArgs>(RecvComplete);
                e.SetBuffer(this.m_cReceiveBuffer.m_lstBuffer, this.m_cReceiveBuffer.WriteIndex, this.m_cReceiveBuffer.GetLength() - this.m_cReceiveBuffer.WriteIndex);

                bool done = this.m_cSocket.ReceiveAsync(e);

                if (!done)
                {
                    ReceiveProcess(e);
                }
            }
            catch (Exception e)
            { 
                //
            }
        }

        /// <summary>
        /// 接收数据处理
        /// </summary>
        /// <param name="e"></param>
        private void ReceiveProcess(SocketAsyncEventArgs e)
        {
            try
            {
                CSocketAsyncEventArgs tmpE = (CSocketAsyncEventArgs)e;
                int transferredNum = tmpE.BytesTransferred;
                if (transferredNum == 0)
                {
                    Receive();
                }
                else
                {
                    this.m_cReceiveBuffer.Write(transferredNum);
                    tmpE.Dispatch(this, transferredNum);
                    ProcessPacket();
                    Receive();
                }
            }
            catch (Exception ee)
            { 
                //
            }
        }

        /// <summary>
        /// 完成接收
        /// </summary>
        /// <param name="e"></param>
        private void RecvComplete(object key, SocketAsyncEventArgs e)
        {
            ReceiveProcess(e);
        }

        /// <summary>
        /// 处理包
        /// </summary>
        private void ProcessPacket()
        {
            try
            {
                for (; true; )
                {
                    PacketBase head = Packing.GetPacketHead(this.m_cReceiveBuffer.m_lstBuffer, this.m_cReceiveBuffer.ReadIndex, this.m_cReceiveBuffer.WriteIndex);
                    if (head != null)
                    {
                        byte[] buffer = this.m_cReceiveBuffer.Read(head.m_usPacketSize);
                        buffer = Packing.PackingDecode(buffer, head.m_usPacketSize);
                        if (buffer != null)
                        {
                            //MemoryStream ms = new MemoryStream(buffer);
                            //BinaryFormatter bf = new BinaryFormatter();
                            //PacketBase pb = (PacketBase)bf.Deserialize(ms);
                            PacketBase pb = PacketFactoryManager.GetInstance().CreatePacket(head.GetPacketID());
                            StreamBuffer sb = new StreamBuffer();
                            sb.Init(buffer, buffer.Length);
                            pb.Load(sb);
                            this.m_cDispatch.AckPacket(pb);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                WriteFiles.WritFile.Log(LogerType.ERROR, e.ToString());
                DisConnect();
            }
        }

        /// <summary>
        /// 发送
        /// </summary>
        public void Send()
        { 
            //
            try
            {
                PacketBase pb;
                if (this.m_cSendQueue.Dequeue(out pb))
                {
                    if (this.m_cSocket == null)
                    {
                        throw new Exception("The socket is null.");
                    }
                    //MemoryStream ms = new MemoryStream();
                    //BinaryFormatter bf = new BinaryFormatter();
                    //byte[] buffer = null;
                    //bf.Serialize(ms, pb);
                    //buffer = ms.ToArray();
                    byte[] buffer = null;
                    StreamBuffer sb = new StreamBuffer();
                    pb.Write(sb);
                    buffer = sb.m_lstBuffer;
                    if (buffer == null)
                    {
                        throw new Exception("The the send buffer is null.");
                    }
                    buffer = Packing.PackingEncode(buffer, pb.GetSize());

                    SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                    e.Completed += this.SendCompleteCallBack;
                    e.SetBuffer(buffer, 0, Packing.GetPacketHeadSize() + pb.GetSize());
                    bool done = this.m_cSocket.SendAsync(e);
                    if (!done)
                    {
                        SendProcess(e);
                    }
                    //this.m_cSocket.BeginSend(buffer, 0, Packing.GetPacketHeadSize() + pb.GetSize(),
                    //    SocketFlags.DontRoute, new AsyncCallback(SendCallBack), this.m_cSocket);
                }
            }
            catch (Exception ee)
            {
                WriteFiles.WritFile.Log(LogerType.ERROR, ee.ToString());
                DisConnect();
            }
        }

        /// <summary>
        /// 发送处理
        /// </summary>
        /// <param name="e"></param>
        private void SendProcess( SocketAsyncEventArgs e )
        {
            try
            {
                int transferredNum = e.BytesTransferred;
                if (transferredNum == 0)
                {
                    //
                }
                else
                { 
                    //this.m_cSendBuffer.ReadIndex += transferredNum;
                }
            }
            catch (Exception ee)
            { 
                //
            }

        }

        /// <summary>
        /// 发送完成
        /// </summary>
        /// <param name="key"></param>
        /// <param name="e"></param>
        private void SendCompleteCallBack(object key, SocketAsyncEventArgs e)
        {
            SendProcess(e);
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="pb"></param>
        public override void Send(PacketBase pb)
        {
            try
            {
                if (pb != null)
                {
                    this.m_cSendQueue.Enqueue(pb);
                }
            }
            catch (Exception e)
            {
                WriteFiles.WritFile.Log(LogerType.ERROR, e.ToString());
                DisConnect();
            }
        }

    }


}
