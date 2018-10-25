using System;
using System.IO;                     //입출력 stream
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;

//tcp를 위해 사용해야하는 namespace
using System.Net;
using System.Net.Sockets;
using System.Threading;
//Marshaling
using System.Runtime.InteropServices;
//UI
using UnityEngine.UI;


public class Connect : MonoBehaviour
{
    public static Connect Instance
    {
        get
        {
            return instance;
        }
    }
    private static Connect instance = null;

    void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public Socket socket;

    string ipAdress = "127.0.0.1";
    public int port = 9000;
    
    // Use this for initialization
    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //서버와 연결
        try  // 실행될 코드
        {
            IPAddress ipAddr = IPAddress.Parse(ipAdress);
            IPEndPoint ipendPoint = new IPEndPoint(ipAddr, port);
            socket.Connect(ipendPoint);
        }
        catch (SocketException se)  // 받아낼 예외
        {
            Debug.Log("Socket Connect Error!  : " + se.ToString());
            return;
        }
        //throw는 예외를 발생시킬때 사용
    }

    //소켓삭제
    private void OnApplicationQuit()
    {
        socket.Disconnect(true);
        socket.Close();
    }
}