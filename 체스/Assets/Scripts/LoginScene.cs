using System;
using System.IO;                     //입출력 stream
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Text;

//Marshaling
using System.Runtime.InteropServices;

//tcp를 위해 사용해야하는 namespace
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class GValue
{
    public static int packetNumber = 1;
}


public class LoginScene : MonoBehaviour
{
    [Header("LoginField")]
    public InputField ID;
    public InputField PW;

    string Id;
    string Pw;

    public static Socket socket;

    public static string NickName;

    byte[] sendByte;

    static bool recvReady = false;

    GameObject obj;
    GameObject obj2;
    public Text ResLoginText;

    // Use this for initialization
    void Start()
    {
        ID.text = "";
        PW.text = "";

        obj = GameObject.Find("Canvas");
        obj2 = GameObject.Find("ResLoginCanvas");
        obj2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SigninBtn()
    {
        Signin();
        
        if (recvReady == true)
        {
            ReadSocket();
        }
    }
    
    void ReadSocket()
    {
        byte[] dataBuffer = new byte[256];
               
        try
        {
            Connect.Instance.socket.Receive(dataBuffer, dataBuffer.Length, 0);
        }
        catch
        {
            Debug.Log("RecvSocket fail");
        }

        PacketHeader recvpack = new PacketHeader();

        recvpack = (PacketHeader)Deserialize(dataBuffer, typeof(PacketHeader));
        
        Debug.Log("Recv Protocol : " + recvpack.code);
        
        if (recvpack.code == PROTOCOL.PT_LOGIN_SUCC)
        {
            PT_LOGIN_SUCC loginSuccPack = new PT_LOGIN_SUCC();

            loginSuccPack = (PT_LOGIN_SUCC)Deserialize(dataBuffer, typeof(PT_LOGIN_SUCC));

            NickName = loginSuccPack.NickName;

            SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
            AsyncSockets.AsyncCallbackClient.Instance().Connect("127.0.0.1", 3500);
        }
        if (recvpack.code == PROTOCOL.PT_LOGIN_FAIL)
        {
            PT_LOGIN_FAIL loginFailPack = new PT_LOGIN_FAIL();

            loginFailPack = (PT_LOGIN_FAIL)Deserialize(dataBuffer, typeof(PT_LOGIN_FAIL));

            if (loginFailPack.errorCode == 6)
                ResLoginText.text = "아이디와 일치하는 비밀번호가 없습니다.";
            obj2.SetActive(true);
            obj.SetActive(false);
        }
        if (recvpack.code != PROTOCOL.PT_LOGIN_SUCC && recvpack.code != PROTOCOL.PT_LOGIN_FAIL)
        {
            recvReady = false;

            SigninBtn();
        }
    }

    public void Signin()
    {
        Id = ID.text;
        Pw = PW.text;

        //패킷 구조체에 담고 마샬링해서 바이트 배열에 담기
        PT_LOGIN Login_Pack = new PT_LOGIN(Id, Pw, GValue.packetNumber++);

        //직렬화시켜 보내기
        sendByte = Serialize(Login_Pack);

        Connect.Instance.socket.Send(sendByte, sendByte.Length, 0);

        recvReady = true;
    }

    public void Confirm()
    {
        obj.SetActive(true);
        obj2.SetActive(false);
    }

    public byte[] Serialize(object data)
    {
        // 메모리 크기 구하기
        int RawSize = Marshal.SizeOf(data);
        IntPtr buffer = Marshal.AllocHGlobal(RawSize); //메모리 할당(임시공간)

        //마샬링 하는 부분(구조체를 바꿈)
        //(마샬링할 데이터,메모리 블럭 포인터,데이터 넣을때 false)
        Marshal.StructureToPtr(data, buffer, false);

        //최종 바이트형 배열 정의
        byte[] RawData = new byte[RawSize];

        //최종 배열에 데이터 넣기
        Marshal.Copy(buffer, RawData, 0, RawSize);
        Marshal.FreeHGlobal(buffer); //해제

        return RawData;
    }
    
    public object Deserialize(byte[] data, Type dataType)
    {
        int RawSize = Marshal.SizeOf(dataType);
        if (RawSize > data.Length)
        {
            return null;
        }
        IntPtr buffer = Marshal.AllocHGlobal(RawSize);
        Marshal.Copy(data, 0, buffer, RawSize);

        object objData = Marshal.PtrToStructure(buffer, dataType);
        Marshal.FreeHGlobal(buffer);
        return objData;
    }
}