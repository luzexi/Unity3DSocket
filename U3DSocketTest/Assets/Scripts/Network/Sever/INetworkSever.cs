



//  INetworkSever.cs
//  Lu Zexi
//  2012-10-05


namespace Game.Network.Sever
{

    /// <summary>
    /// 网络服务端接口
    /// </summary>
    public interface INetworkSever
    {
        void Initialize();  //初始化
        bool Update();  //逻辑更新
    }

}



