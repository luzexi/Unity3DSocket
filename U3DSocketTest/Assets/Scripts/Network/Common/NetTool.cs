using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnityEngine;

//  NetTool.cs
//  Lu Zexi
//  2012-8-25


namespace Game.Network.Tool
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogerType
    {
        OFF = -1,
        ERROR,
        WARN,
        INFO,
        DEBUG,
    };

    /// <summary>
    /// log的回调函数
    /// </summary>
    /// <param name="type">log类型</param>
    /// <param name="format">信息</param>
    internal delegate void LogCallBackFunc(LogerType type, string format);

    /// <summary>
    /// 写文件类
    /// </summary>
    internal class WriteFiles
    {
        /// <summary>
        /// 唯一的实例
        /// </summary>
        static readonly WriteFiles sInstance = new WriteFiles();

        /// <summary>
        /// log回调函数
        /// </summary>
        public LogCallBackFunc LogCallBack;

        static public WriteFiles WritFile
        {
            get { return sInstance; }
        }

        /// <summary>
        /// 输出log
        /// </summary>
        /// <param name="type">log类型</param>
        /// <param name="format">信息</param>
        public void Log(LogerType type, string format)
        {
            //Directory.CreateDirectory(@"Log");
            //FileStream fs = new FileStream(@"Log\Log.txt", FileMode.Append, FileAccess.Write, FileShare.Write);
            //StreamWriter streamWriter = new StreamWriter(fs);
            //streamWriter.WriteLine("[" + DateTime.Now.ToString() + "]" + ":" + format);
            //streamWriter.Flush();
            //streamWriter.Close();
            //fs.Close();

            if( this.LogCallBack != null)
                LogCallBack(type, "[" + DateTime.Now.ToString() + "]:" + format);
        }

    }

}
