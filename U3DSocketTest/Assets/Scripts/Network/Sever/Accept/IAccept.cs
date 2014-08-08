


//  IAccept.cs
//  Lu Zexi
//  2012-10-04


namespace Game.Network.Sever
{

    /// <summary>
    /// 监听接口
    /// </summary>
    public interface IAccept
    {
        void Initialize();  //初始化
        bool Update();  //逻辑更新

    }

}


