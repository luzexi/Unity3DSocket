

//  PacketFactory.cs
//  Lu Zexi
//  2012-8-23


namespace Game.Network
{

    /// <summary>
    /// 包工厂
    /// </summary>
    public abstract class PacketFactory
    {
        public abstract PacketBase Create();    //创建包
        public abstract int GetPacketId();      //获取包ID
        public abstract int GetPacketSize();    //获取包大小
    }

}