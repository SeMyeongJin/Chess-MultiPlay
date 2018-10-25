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

public class LobbyScene : MonoBehaviour
{
    byte[] sendByte;

    public static bool isWhite;
    public static bool isOfficialGame;

    public Text userInfo;

    GameObject obj;
    GameObject obj2;
    GameObject obj3;
    GameObject obj4;

    // Use this for initialization
    void Start()
    {
        obj = GameObject.Find("UserInfoCanvas");
        obj.SetActive(false);
        obj2 = GameObject.Find("LobbyCanvas");
        obj3 = GameObject.Find("AutoMatchingCanvas");
        obj3.SetActive(false);
        obj4 = GameObject.Find("PowerCanvas");
        obj4.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        int dataCount = AsyncSockets.AsyncCallbackClient.Instance().dataQueue.Count;
        if (dataCount > 0)
        {
            for (int i = 0; i < dataCount; i++)
            { 
                AsyncSockets.AsyncCallbackClient.Instance().dataQueue.Dequeue();
                ReadSocket();
            }
        }
    }

    public void OGameStartBtn()
    {
        OGameStart();
    }

    public void FGameStartBtn()
    {
        FGameStart();
    }

    public void UserInfoBtn()
    {
        UserInfo();
    }

    public void BackBtn()
    {
        Back();
    }

    public void CancelBtn()
    {
        Cancel();
    }

    void ReadSocket()
    {
        try
        {
            AsyncSockets.AsyncCallbackClient.Instance().RecvData();
        }
        catch
        {
            Debug.Log("RecvSocket fail");
        }

        GamePacketHeader recvPacket = new GamePacketHeader();
        recvPacket = (GamePacketHeader)Deserialize(AsyncSockets.AsyncCallbackClient.Instance().recvByte, typeof(GamePacketHeader));
        if (recvPacket.code == TCP_PROTOCOL.PT_OFFICIAL_GAME_START_SUCC || recvPacket.code == TCP_PROTOCOL.PT_FRIENDSHIP_GAME_START_SUCC)
        {
            Debug.Log("Recv Protocol : " + recvPacket.code);
            //SceneManager.LoadScene("ChessGameScene", LoadSceneMode.Single);
        }
        if (recvPacket.code == TCP_PROTOCOL.PT_GAME_START_ALL)
        {
            Debug.Log("Recv Protocol : " + recvPacket.code);

            PT_GAME_START_ALL startPacket = new PT_GAME_START_ALL();
            startPacket = (PT_GAME_START_ALL)Deserialize(AsyncSockets.AsyncCallbackClient.Instance().recvByte, typeof(PT_GAME_START_ALL));

            isWhite = startPacket.isWhite;
            isOfficialGame = startPacket.isOfficialGame;
            SceneManager.LoadScene("ChessGameScene", LoadSceneMode.Single);
        }
        if (recvPacket.code == TCP_PROTOCOL.PT_RES_USER_INFO)
        {
            Debug.Log("Recv Protocol : " + recvPacket.code);

            PT_RES_USER_INFO userInfoPacket = new PT_RES_USER_INFO();
            userInfoPacket = (PT_RES_USER_INFO)Deserialize(AsyncSockets.AsyncCallbackClient.Instance().recvByte, typeof(PT_RES_USER_INFO));
            float per = (float)(userInfoPacket.off_win / (float)(userInfoPacket.off_win + userInfoPacket.off_lose)) * 100;
            string sPer = per.ToString("N1");
            if (userInfoPacket.off_win == 0) sPer = "0";
            Debug.Log("USER INFO : " + userInfoPacket.rating + ", " + userInfoPacket.off_win + ", " + userInfoPacket.off_lose + ", " + userInfoPacket.fri_win);

            userInfo.text = "Name : " + LoginScene.NickName +
                "\n" + "Rating : " + userInfoPacket.rating +
                "\n" + "Official Game Record : " + userInfoPacket.off_win + "win " + userInfoPacket.off_lose + "lose " + "(" + sPer + "%)" +
                "\n" + "Friendship Game Record : " + userInfoPacket.fri_win + "win";

            obj.SetActive(true);
        }
        if (recvPacket.code == TCP_PROTOCOL.PT_ROOM_LEAVE_SUCC)
        {
            Debug.Log("Recv Protocol : " + recvPacket.code);

            PT_ROOM_LEAVE_SUCC leaveSuccPacket = new PT_ROOM_LEAVE_SUCC();
            leaveSuccPacket = (PT_ROOM_LEAVE_SUCC)Deserialize(AsyncSockets.AsyncCallbackClient.Instance().recvByte, typeof(PT_ROOM_LEAVE_SUCC));

            obj2.SetActive(true);
            obj3.SetActive(false);
        }
    }

    public void OGameStart()
    {
        PT_OFFICIAL_GAME_START OGStartPack = new PT_OFFICIAL_GAME_START(GValue.packetNumber++);
        
        sendByte = Serialize(OGStartPack);
        
        AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);
        
        obj3.SetActive(true);
        obj2.SetActive(false);
        obj.SetActive(false);
    }

    public void FGameStart()
    {
        PT_FRIENDSHIP_GAME_START FGStartPack = new PT_FRIENDSHIP_GAME_START(GValue.packetNumber++);

        sendByte = Serialize(FGStartPack);

        AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);

        obj3.SetActive(true);
        obj2.SetActive(false);
        obj.SetActive(false);
    }

    public void UserInfo()
    {
        PT_REQ_USER_INFO requestUserInfoPack = new PT_REQ_USER_INFO(LoginScene.NickName, GValue.packetNumber++);

        sendByte = Serialize(requestUserInfoPack);

        AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);
    }

    public void Back()
    {
        obj.SetActive(false);
    }

    public void Cancel()
    {
        PT_ROOM_LEAVE cancelPack = new PT_ROOM_LEAVE(GValue.packetNumber++);

        sendByte = Serialize(cancelPack);

        AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);
    }

    public void PowerBtn()
    {
        obj4.SetActive(true);
    }

    public void PowerBackBtn()
    {
        obj4.SetActive(false);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        AsyncSockets.AsyncCallbackClient.Instance().Close();
        UnityEditor.EditorApplication.isPlaying = false;
#else
        AsyncSockets.AsyncCallbackClient.Instance().Close();
        Application.Quit();
#endif
    }

    public void LogOut()
    {
        AsyncSockets.AsyncCallbackClient.Instance().Close();
        SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
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
