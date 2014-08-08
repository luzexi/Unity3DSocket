



//  Overlapped.cs
//  Lu Zexi
//  2012-10-1


namespace Game.Network.Sever
{

    /// <summary>
    /// 重叠结构类
    /// </summary>
    public abstract class Overlapped
    {
        /// <summary>
        /// 调度
        /// </summary>
        /// <param name="keyObject"></param>
        /// <param name="transferredNum"></param>
        public abstract void Dispatch(object keyObject, ulong transferredNum);

        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void Destory()
        { 
            //
        }
    }

}


