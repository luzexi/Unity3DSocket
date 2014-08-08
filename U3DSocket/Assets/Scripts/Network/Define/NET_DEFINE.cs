


//  NET_DEFINE.cs
//  Lu Zexi
//  2012-8-23


namespace Game.Network
{
    /// <summary>
    /// 网络定义
    /// </summary>
    public class NET_DEFINE
    {
        public const string LOCAL_IP = "127.0.0.1";
    }

    /// <summary>
    /// 网络状态
    /// </summary>
    public enum SESSION_STATUS
    { 
        NO_CONNECT = 0,     //无连接
        CONNECT_SUCCESS = 1,    //连接成功
        CONNECT_FAILED_CONNECT_ERROR = 2,   //连接失败
        CONNECT_FAILED_TIME_OUT = 3,        //连接超时
        CONNECT_EXIT = 4,                   //连接退出
        RE_CONNECT = 5,     //重新连接
    }

    /// <summary>
    /// 包执行结果
    /// </summary>
    public enum PACKET_EXC_RES
    { 
        PACKET_EXC_ERROR = 0,       //执行错误
        PACKET_EXC_BREAK,           //执行终止
        PACKET_EXC_CONTINUE,        //执行继续
        PACKET_EXC_NOTREMOVE,       //不删除
        PACKET_EXC_NOTREMOVE_ERROR, //不删除错误
        PACKET_EXC_CANNOT_FIND_HANDLE,  //找不到对应执行句柄
    }

    /// <summary>
    /// 请求操作结果
    /// </summary>
    public enum REQUIRE_RESULT
    {
        REQUIRE_SUCCESS,   //成功 
        REQUIRE_OP_FAILS,   //操作失败
        REQUIRE_SERVER_BUSY,    //服务器忙，重试
        REQUIRE_OP_TIMES,   //操作过于频繁
    }

}

