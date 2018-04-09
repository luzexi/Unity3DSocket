

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Game.Network
{
    public class TCPSession
    {
        public enum SESSION_STATUS
        { 
            NO_CONNECT,
            CONNECT_SUCCESS,
            CONNECT_FAILED_CONNECT_ERROR,
            CONNECT_FAILED_TIME_OUT,
            CONNECT_EXIT,
        }

        public enum EVENT
        {
            CONNECTED,
            CONNECT_FAILED,
            CONNECT_DISCONNECT,
            DATA_RECEIVE,
        }

        // encode 8 bits unsigned int
        public static int tcpsession_encode8u(byte[] p, int offset, byte c)
        {
            if(p == null || p.Length < 1 || p.Length - offset < 1) return 0;
            p[0 + offset] = c;
            return 1;
        }

        // decode 8 bits unsigned int
        public static int tcpsession_decode8u(byte[] p, int offset, ref byte c)
        {
            if(p == null || p.Length < 1 || p.Length - offset < 1) return 0;
            c = p[0 + offset];
            return 1;
        }

         /* encode 16 bits unsigned int (lsb) */
        public static int tcpsession_encode16u(byte[] p, int offset, UInt16 w) 
        {
            if(p == null || p.Length < 2 || p.Length - offset < 2) return 0;
            p[0 + offset] = (byte)(w >> 0);
            p[1 + offset] = (byte)(w >> 8);
            return 2;
        }

        /* decode 16 bits unsigned int (lsb) */
        public static int tcpsession_decode16u(byte[] p, int offset, ref UInt16 c)  
        {
            if(p == null || p.Length < 2 || p.Length - offset < 2) return 0;
            UInt16 result = 0;
            result |= (UInt16)p[0 + offset];
            result |= (UInt16)(p[1 + offset] << 8);
            c = result;
            return 2;
        }

        /* encode 32 bits unsigned int (lsb) */
        public static int tcpsession_encode32u(byte[] p, int offset, UInt32 l)
        {
            if(p == null || p.Length < 4 || p.Length - offset < 4) return 0;
            p[0 + offset] = (byte)(l >> 0);
            p[1 + offset] = (byte)(l >> 8);
            p[2 + offset] = (byte)(l >> 16);
            p[3 + offset] = (byte)(l >> 24);
            return 4;
        }

        /* decode 32 bits unsigned int (lsb) */
        public static int tcpsession_decode32u(byte[] p, int offset, ref UInt32 c) 
        {
            if(p == null || p.Length < 4 || p.Length - offset < 4) return 0;
            UInt32 result = 0;
            result |= (UInt32)p[0 + offset];
            result |= (UInt32)(p[1 + offset] << 8);
            result |= (UInt32)(p[2 + offset] << 16);
            result |= (UInt32)(p[3 + offset] << 24);
            c = result;
            return 4;
        }

        private const int RECEIVE_MAX_NUM = 1024;
        private const int SEND_MAX_SIZE = 14000;
        private const float SEND_INTERVAL_TIME = 0.2f;
        private const int HEAD_SIZE = 6;

        private Socket m_cSocket;

        private byte[] mReceiveArray;
        private StreamBuffer m_cReceiveBuffer;  //receive buffer
        private StreamBuffer m_cSendBuffer;     //send buffer
        private List<byte[]> mlstSend;  //send queue
        private SwitchQueue<byte[]> m_cReceiveQueue;  //receive queue

        private string m_strAddress;    //address of server
        private int m_iPort;       //port

        private float mSendStartTime;

        private SESSION_STATUS m_cStatus;   //status

        private System.Action<EVENT, byte[], string> mCallback;

        public TCPSession(System.Action<EVENT, byte[], string> _callback)
        {
            mReceiveArray = new byte[RECEIVE_MAX_NUM];

            m_cStatus = SESSION_STATUS.NO_CONNECT;

            m_cReceiveBuffer = new StreamBuffer();
            m_cSendBuffer = new StreamBuffer();

            mlstSend = new List<byte[]>();
            m_cReceiveQueue = new SwitchQueue<byte[]>(128);

            mCallback = _callback;
        }

        public SESSION_STATUS GetStatus()
        {
            return m_cStatus;
        }

        private void ChangeStatus(SESSION_STATUS status)
        {
            m_cStatus = status;
            if(status == SESSION_STATUS.CONNECT_SUCCESS)
            {
                if(mCallback != null)
                {
                    mCallback(EVENT.CONNECTED, null , null);
                }
            }

            if(status == SESSION_STATUS.CONNECT_FAILED_CONNECT_ERROR)
            {
                if(mCallback != null)
                {
                    mCallback(EVENT.CONNECT_FAILED, null , "connect error");
                }
            }

            if(status == SESSION_STATUS.CONNECT_FAILED_TIME_OUT)
            {
                if(mCallback != null)
                {
                    mCallback(EVENT.CONNECT_FAILED, null , "connect time out");
                }
            }

            if(status == SESSION_STATUS.CONNECT_EXIT)
            {
                if(mCallback != null)
                {
                    mCallback(EVENT.CONNECT_DISCONNECT, null , "disconnect");
                }
            }
        }

        public void Connect(string address, int port)
        {
            try
            {
                m_strAddress = address;
                m_iPort = port;

                IPHostEntry iphostInfo = Dns.GetHostEntry(address);
                IPAddress ipAddress = iphostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                m_cSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (m_cSocket == null)
                {
                    //ERROR
                    throw new Exception("Socket create error.");
                }
                m_cSocket.NoDelay = true;
                m_cSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallBack), m_cSocket);
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
                ChangeStatus(SESSION_STATUS.CONNECT_FAILED_CONNECT_ERROR);
            }
        }

        private void ConnectCallBack(IAsyncResult ar)
        {
            try
            {
                m_cSocket = (Socket)ar.AsyncState;
                if (m_cSocket == null)
                {
                    //ERROR
                    throw new Exception("Socket connect Failed.");
                }
                m_cSocket.EndConnect(ar);
                ChangeStatus(SESSION_STATUS.CONNECT_SUCCESS);
                Receive();
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
                ChangeStatus(SESSION_STATUS.CONNECT_FAILED_CONNECT_ERROR);
                
            }
        }

        public void DisConnect()
        {
            try
            {
                if (m_cSocket != null)
                {
                    m_cSocket.Shutdown(SocketShutdown.Both);
                    m_cSocket.Close();
                }
                m_cSocket = null;
                ChangeStatus(SESSION_STATUS.CONNECT_EXIT);
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
                ChangeStatus(SESSION_STATUS.CONNECT_FAILED_CONNECT_ERROR);
            }
        }

        private bool Receive()
        {
            try
            {
                if(m_cStatus != SESSION_STATUS.CONNECT_SUCCESS) return false;

                if (m_cSocket == null)
                {
                    throw new Exception("Error , the socket is null.");
                }
                m_cSocket.BeginReceive(mReceiveArray, 0, 
                    RECEIVE_MAX_NUM, 
                    SocketFlags.None, new AsyncCallback(ReceiveCallBack), mReceiveArray);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
                ChangeStatus(SESSION_STATUS.CONNECT_FAILED_CONNECT_ERROR);
            }
            return false;
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                int bytesCount = m_cSocket.EndReceive(ar);
                if (bytesCount > 0)
                {
                    byte[] data = new byte[bytesCount];
                    Array.Copy(mReceiveArray, 0 , data, 0 , bytesCount);
                    m_cReceiveQueue.Push(data);

                    Receive();
                }
                else
                {
                    throw new Exception("Receive bytes error.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
                ChangeStatus(SESSION_STATUS.CONNECT_FAILED_CONNECT_ERROR);
            }
        }

        public void Update()
        {
            ProcessReceive();
            ProcessSend();
        }

        public void Send(byte[] send_buffer)
        {
            mlstSend.Add(send_buffer);
        }

        private void ProcessSend()
        {
            try
            {
                if(m_cStatus != SESSION_STATUS.CONNECT_SUCCESS) return;
                if( Time.time - mSendStartTime < SEND_INTERVAL_TIME ) return;

                int _packsize = 0;
                int _packindex = -1;
                m_cSendBuffer.Clear();
                for( int i = 0 ; i < mlstSend.Count ; i++ )
                {
                    if(mlstSend[i].Length + _packsize > SEND_MAX_SIZE)
                    {
                        break;
                    }
                    _packsize += mlstSend[i].Length;
                    _packindex = i;
                    m_cSendBuffer.Write(mlstSend[i], mlstSend[i].Length);
                }
                if (m_cSendBuffer.GetSize() > 0)
                {
                    if (m_cSocket == null)
                    {
                        throw new Exception("The socket is null.");
                    }
                    byte[] buffer = m_cSendBuffer.Read(m_cSendBuffer.GetSize());
                    if (buffer == null)
                    {
                        throw new Exception("The the send buffer is null.");
                    }

                    //加密
                    //buffer = Packing.PackingEncode(buffer, pb.GetSize() );

                    mSendStartTime = Time.time;

                    m_cSocket.BeginSend(buffer, 0, buffer.Length,
                        SocketFlags.DontRoute, new AsyncCallback(SendCallBack), m_cSocket);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
                ChangeStatus(SESSION_STATUS.CONNECT_FAILED_CONNECT_ERROR);
            }
        }

        private void SendCallBack(IAsyncResult ar)
        {
            int bytesSend = m_cSocket.EndSend(ar);
            if (bytesSend <= 0)
            {
                Debug.LogError("send failed");
                ChangeStatus(SESSION_STATUS.CONNECT_FAILED_CONNECT_ERROR);
            }
        }

        private void ProcessReceive()
        {
            m_cReceiveQueue.Switch();

            while(!m_cReceiveQueue.Empty())
            {
                m_cReceiveBuffer.Write(m_cReceiveQueue.Pop());
            }

            for( ; m_cReceiveBuffer.GetSize() >= HEAD_SIZE ; )
            {
                UInt16 pack_size = 0;
                int offset = 0;
                offset += tcpsession_decode16u(m_cReceiveBuffer.m_lstBuffer, offset, ref pack_size);
                if (m_cReceiveBuffer.GetSize() >= HEAD_SIZE + pack_size)
                {
                    byte[] buffer = m_cReceiveBuffer.Read(HEAD_SIZE + pack_size);

                    //decrypt
                    //buffer = Packing.PackingDecode(buffer, head.m_usPacketSize);

                    if (buffer != null)
                    {
                        if(mCallback != null)
                        {
                            mCallback(EVENT.DATA_RECEIVE, buffer, null);
                        }
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

    }
}

