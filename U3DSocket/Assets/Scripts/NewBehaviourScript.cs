using UnityEngine;
using System.Collections;
using System;


using Game;
using Game.Network.Tool;


public class NewBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        //ushort tmp = 0x3;

        //byte[] buf = System.BitConverter.GetBytes(9876543210L);
        //Array.Reverse(buf);

        //Debug.Log("len: " + buf.Length);
        //for (int i = 0; i < buf.Length; i++)
        //{
        //    Debug.Log(buf[i]);
        //}

        WriteFiles.WritFile.LogCallBack = LogCallBackFunc;
	}

    /// <summary>
    /// 输出回调
    /// </summary>
    /// <param name="type"></param>
    /// <param name="format"></param>
    private void LogCallBackFunc(LogerType type, string format)
    {
        Debug.Log(format);
    }

	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// 逻辑更新
    /// </summary>
    void FixedUpdate()
    { 
        //
        ClientSessionManager.GetInstance().Update();
    }

    /// <summary>
    /// GUI
    /// </summary>
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 30), "Connect"))
        {
            Debug.Log("connect");
            ClientSessionManager.GetInstance().Connect(0, "192.168.0.243", 3010);
        }

        if (GUI.Button(new Rect(200, 10, 150, 30), "DisConnect"))
        {
            Debug.Log("disconnect");
            ClientSessionManager.GetInstance().DisConnect(0);
        }

    }

    /// <summary>
    /// 销毁
    /// </summary>
    void OnApplicationQuit()
    {
        ClientSessionManager.GetInstance().DisConnect(0);
    }


}
