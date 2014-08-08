


using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


//  Packing.cs
//  Lu Zexi
//  2012-8-30


namespace Game.Network.Tool
{
    public class Packing
    {
        /// <summary>
        /// 从缓存流中获取包头
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static PacketBase GetPacketHead(byte[] buffer, int offset, int end)
        {
            try
            {
                if (end - offset < 6)
                {
                    return null;
                }
                short packetId = (short)(buffer[offset + 1] << 8 | buffer[offset]);
                int packetSize = (buffer[offset + 5] | (buffer[offset + 4] << 8) | (buffer[offset + 3] << 16) | (buffer[offset + 2] << 24));
                byte[] tmpbuffer = new byte[6];
                Array.Copy(buffer, offset, tmpbuffer, 0, 6);

                MemoryStream ms = new MemoryStream(tmpbuffer);
                BinaryFormatter bf = new BinaryFormatter();
                PacketBase pb = (PacketBase)bf.Deserialize(ms);
                return pb;
            }
            catch (Exception e)
            {
                WriteFiles.WritFile.Log(LogerType.ERROR, e.ToString());
            }
            return null;
        }

        /// <summary>
        /// 打包加密
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static byte[] PackingEncode(byte[] buffer, int len)
        {
            CEncrypt.Encoding(ref buffer, Packing.GetPacketHeadSize(), CEncrypt.CLIENT_TO_GAMESERVER_KEY, 0, len);
            return buffer;
        }

        /// <summary>
        /// 打包解密
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="len"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static byte[] PackingDecode(byte[] buffer, int len)
        {
            CEncrypt.Decoding(ref buffer, Packing.GetPacketHeadSize(), CEncrypt.GAMESERVER_TO_CLIENT_KEY, 0, len);
            return buffer;
        }

        /// <summary>
        /// 获取包头大小
        /// </summary>
        /// <returns></returns>
        public static int GetPacketHeadSize()
        {
            //return System.Runtime.InteropServices.Marshal.SizeOf(Type.GetType("PacketBase"));
            return sizeof(short) + sizeof(int);
        }
    }

}
