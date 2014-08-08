

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Game.Network.Tool;

//  ClientSession.cs
//  Lu Zexi
//  2012-8-23


namespace Game.Network
{

    /// <summary>
    /// 网络会话类
    /// </summary>
    public class ClientSession : IClientSession
    {
        private Socket m_cSocket;   //网络嵌套字

        private bool m_bStartReConnect; //启动重连标志
        private long m_lStartReConnectTime; //开始重连时间
        private int m_iReConnectSecond = -1; //重连秒数:-1会不重连

        private StreamBuffer m_cReceiveBuffer;  //接收缓存
        private StreamBuffer m_cSendBuffer;     //发送缓存
        private NetQueue<PacketBase> m_cSendQueue;  //发送包队列

        private Dispatch m_cDispatch;   //调度类
        private DispatchFactoryBase m_cDispatchFactory; //调度类工厂

        private string m_strAddress;    //连接地址
        private int m_iPort;       //连接端口

        private SESSION_STATUS m_cStatus;   //会话状态

        public ClientSession( DispatchFactoryBase dispatchFactory )
        {
            this.m_cStatus = SESSION_STATUS.NO_CONNECT;
            this.m_cDispatchFactory = dispatchFactory;

            this.m_cReceiveBuffer = new StreamBuffer();
            this.m_cSendBuffer = new StreamBuffer();

            this.m_cSendQueue = new NetQueue<PacketBase>(64 * 256);
            this.m_cSendQueue.Clear();

            this.m_bStartReConnect = false;
        }

        /// <summary>
        /// 获取连接状态
        /// </summary>
        /// <returns></returns>
        public override SESSION_STATUS GetStatus()
        {
            return this.m_cStatus;
        }

        /// <summary>
        /// 设置重连时间
        /// </summary>
        /// <param name="second"></param>
        public override void SetReConnectSecond(int second)
        {
            this.m_iReConnectSecond = second;
        }

        /// <summary>
        /// 更改会话状态
        /// </summary>
        /// <param name="status"></param>
        protected override void ChangeStatus(SESSION_STATUS status)
        {
            this.m_cStatus = status;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public override void Connect(string address, int port)
        {
            try
            {
                this.m_strAddress = address;
                this.m_iPort = port;

                IPHostEntry iphostInfo = Dns.GetHostEntry(address);
                IPAddress ipAddress = iphostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                this.m_cSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.m_cSocket.NoDelay = false;
                if (this.m_cSocket == null)
                {
                    //ERROR
                    throw new Exception("Socket create error.");
                }
                this.m_cSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallBack), this.m_cSocket);

            }
            catch (Exception e)
            {
                WriteFiles.WritFile.Log(LogerType.ERROR, e.StackTrace);
                DisConnect();
                ChangeStatus(SESSION_STATUS.CONNECT_FAILED_CONNECT_ERROR);
            }
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        public override void ReConnect()
        {
            try
            {
                Connect(this.m_strAddress, this.m_iPort);
                ChangeStatus(SESSION_STATUS.RE_CONNECT);
                this.m_cDispatch.OnReConnect();
            }
            catch (Exception ex)
            {
                WriteFiles.WritFile.Log(LogerType.ERROR, ex.StackTrace);
                DisConnect();
                ChangeStatus(SESSION_STATUS.CONNECT_FAILED_CONNECT_ERROR);
            }
        }

        /// <summary>
        /// 异步连接回调
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallBack(IAsyncResult ar)
        {
            try
            {
                this.m_cSocket = (Socket)ar.AsyncState;
                if (this.m_cSocket == null)
                {
                    //ERROR
                    throw new Exception("Socket connect Failed.");
                }
                this.m_cSocket.EndConnect(ar);
                ChangeStatus(SESSION_STATUS.CONNECT_SUCCESS);
                this.m_cDispatch = this.m_cDispatchFactory.Create(this);
                this.m_cDispatch.OnConnect();
                Receive();
            }
            catch (Exception e)
            {
                WriteFiles.WritFile.Log(LogerType.ERROR, e.StackTrace);
                DisConnect();
                ChangeStatus(SESSION_STATUS.CONNECT_FAILED_CONNECT_ERROR);
                
            }
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
                this.m_iReConnectSecond = -1;   //主动断开连接不进行重连
                this.m_cSocket = null;
                if (this.m_cDispatch != null)
                {
                    this.m_cDispatch.OnDisconnect();
                }
                ChangeStatus(SESSION_STATUS.CONNECT_EXIT);
            }
            catch (Exception e)
            {
                WriteFiles.WritFile.Log(LogerType.ERROR, e.StackTrace);
                ChangeStatus(SESSION_STATUS.CONNECT_FAILED_CONNECT_ERROR);
            }
        }

        /// <summary>
        /// 异步接受数据
        /// </summary>
        /// <returns></returns>
        private bool Receive()
        {
            try
            {
                if (this.m_cSocket == null)
                {
                    throw new Exception("Error , the socket is null.");
                }
                this.m_cSocket.BeginReceive(this.m_cReceiveBuffer.m_lstBuffer, this.m_cReceiveBuffer.WriteIndex, 
                    this.m_cReceiveBuffer.GetLength() - this.m_cReceiveBuffer.WriteIndex, 
                    SocketFlags.None, new AsyncCallback(ReceiveCallBack), this.m_cReceiveBuffer);
                return true;
            }
            catch (Exception e)
            {
                WriteFiles.WritFile.Log(LogerType.ERROR, e.StackTrace);
                DisConnect();
                if (this.m_iReConnectSecond >= 0)   //设置重连
                {
                    this.m_bStartReConnect = true;
                    this.m_lStartReConnectTime = DateTime.Now.Ticks;
                }
            }
            return false;
        }

        /// <summary>
        /// 接受回调
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                int bytesCount = this.m_cSocket.EndReceive(ar);
                if (bytesCount > 0)
                {
                    this.m_cReceiveBuffer.Write(bytesCount);

                    //处理包
                    ProcessPacket();
                    Receive();
                }
                else
                {
                    if (bytesCount == 0)
                    {
                        throw new Exception("Receive bytes error.");
                    }
                    else
                    {
                        throw new Exception("Receive bytes error.");
                    }
                }
            }
            catch (Exception e)
            {
                WriteFiles.WritFile.Log(LogerType.ERROR, e.StackTrace);
                DisConnect();
                if (this.m_iReConnectSecond >= 0)   //设置重连
                {
                    this.m_bStartReConnect = true;
                    this.m_lStartReConnectTime = DateTime.Now.Ticks;
                }
            }
        }

        /// <summary>
        /// 发送数据包
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
                WriteFiles.WritFile.Log(LogerType.ERROR, e.StackTrace);
                DisConnect();
                if (this.m_iReConnectSecond >= 0)   //设置重连
                {
                    this.m_bStartReConnect = true;
                    this.m_lStartReConnectTime = DateTime.Now.Ticks;
                }
            }
        }

        /// <summary>
        /// 发送列表包中的数据
        /// </summary>
        public void Send()
        {
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
                    pb.m_usPacketSize = pb.GetSize();
                    pb.Write(sb);
                    buffer = sb.GetBuffer();
                    if (buffer == null)
                    {
                        throw new Exception("The the send buffer is null.");
                    }

                    //加密
                    //buffer = Packing.PackingEncode(buffer, pb.GetSize() );

                    this.m_cSocket.BeginSend(buffer, 0, buffer.Length,
                        SocketFlags.DontRoute, new AsyncCallback(SendCallBack), this.m_cSocket);
                }
            }
            catch (Exception e)
            {
                WriteFiles.WritFile.Log(LogerType.ERROR, e.StackTrace);
                DisConnect();
                if (this.m_iReConnectSecond >= 0)   //设置重连
                {
                    this.m_bStartReConnect = true;
                    this.m_lStartReConnectTime = DateTime.Now.Ticks;
                }
            }
        }

        /// <summary>
        /// 发送回调
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallBack(IAsyncResult ar)
        {
            try
            {
                int bytesSend = this.m_cSocket.EndSend(ar);
                if (bytesSend <= 0)
                {
                    throw new Exception("The packet is not be send.");
                }
            }
            catch( Exception e )
            {
                WriteFiles.WritFile.Log(LogerType.ERROR, e.StackTrace);
                DisConnect();
                if (this.m_iReConnectSecond >= 0)   //设置重连
                {
                    this.m_bStartReConnect = true;
                    this.m_lStartReConnectTime = DateTime.Now.Ticks;
                }
            }
        }

        /// <summary>
        /// 处理包
        /// </summary>
        private void ProcessPacket()
        {
            try
            {
                for ( ; true ; )
                {
                    PacketBase head = Packing.GetPacketHead(this.m_cReceiveBuffer.m_lstBuffer, this.m_cReceiveBuffer.ReadIndex, this.m_cReceiveBuffer.WriteIndex);
                    if (head != null)
                    {
                        byte[] buffer = this.m_cReceiveBuffer.Read(head.m_usPacketSize);

                        //解密
                        //buffer = Packing.PackingDecode(buffer, head.m_usPacketSize);

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
                WriteFiles.WritFile.Log(LogerType.ERROR, e.StackTrace);
                DisConnect();
            }
        }

        /// <summary>
        /// 逻辑更新
        /// </summary>
        /// <returns></returns>
        public override bool Update()
        {
            if (this.m_cStatus == SESSION_STATUS.CONNECT_SUCCESS)
            {
                if (this.m_cDispatch != null)
                {
                    this.m_cDispatch.Update();
                }
            }

            //判断连接异常断开
            if (this.m_cStatus == SESSION_STATUS.CONNECT_SUCCESS && !this.m_cSocket.Connected)
            {
                this.m_cDispatch.OnDisconnect();
                this.m_cStatus = SESSION_STATUS.NO_CONNECT;
                if (this.m_iReConnectSecond >= 0)   //设置重连
                {
                    this.m_bStartReConnect = true;
                    this.m_lStartReConnectTime = DateTime.Now.Ticks;
                }
            }

            //判断重连
            if (this.m_bStartReConnect)
            {
                if (DateTime.Now.Ticks - this.m_lStartReConnectTime >= this.m_iReConnectSecond)
                {
                    this.m_bStartReConnect = false;
                    ReConnect();
                }
            }

            for (int i = 0; i < 5; i++)
            {
                Send();
            }

            return true;
        }

    }
}

