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

public class SignupScene : MonoBehaviour
{
    [Header("LoginField")]
    public InputField ID;
    public InputField PW;
    public InputField UserName;

    string Id;
    string Pw;
    string NickName;

    byte[] sendByte;

    bool recvReady = false;

    GameObject obj;
    GameObject obj2;
    GameObject obj3;
    public Text ResSignupText;
    public Text ResSignupComplete;

    public Button SignupButton;
    public Sprite SignupImage;
    public Sprite SignupImage2;

    public Button OKButton;
    public Sprite OKImage;

    public Button ConfirmButton;
    public Sprite ConfirmImage;
    public Sprite ConfirmImage2;

    public Button BackButton;
    public Sprite BackImage;

    // Use this for initialization
    void Start()
    {
        ID.text = "";
        PW.text = "";
        UserName.text = "";

        obj = GameObject.Find("Canvas");
        obj2 = GameObject.Find("ResSignupCanvas");
        obj3 = GameObject.Find("ResSignupComplete");
        obj2.SetActive(false);
        obj3.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SignupBtn()
    {
        Signup();

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

        if (recvpack.code == PROTOCOL.PT_SIGNUP_SUCC)
        {
            PT_SIGNUP_SUCC signupSuccPack = new PT_SIGNUP_SUCC();

            signupSuccPack = (PT_SIGNUP_SUCC)Deserialize(dataBuffer, typeof(PT_SIGNUP_SUCC));

            ResSignupComplete.text = "회원가입을 축하드립니다.";
            obj3.SetActive(true);
            obj.SetActive(false);
        }
        if (recvpack.code == PROTOCOL.PT_SIGNUP_FAIL)
        {
            PT_SIGNUP_FAIL signupFailPack = new PT_SIGNUP_FAIL();

            signupFailPack = (PT_SIGNUP_FAIL)Deserialize(dataBuffer, typeof(PT_SIGNUP_FAIL));

            if (signupFailPack.errorCode == 1)
                ResSignupText.text = "이미 존재하는 ID입니다.";
            obj2.SetActive(true);
            obj.SetActive(false);
        }
        if (recvpack.code != PROTOCOL.PT_SIGNUP_SUCC && recvpack.code != PROTOCOL.PT_SIGNUP_FAIL)
        {
            recvReady = false;

            SignupBtn();
        }
        recvReady = false;
    }

    public void Signup()
    {
        Id = ID.text;
        Pw = PW.text;
        NickName = UserName.text;

        Debug.Log("Send Id : " + Id);
        Debug.Log("Send PassWord : " + Pw);
        Debug.Log("Send UserName : " + NickName);

        if (Id.Length > 20 || Id.Length < 6)
        {
            ResSignupText.text = "6 ~ 20자 이내로 아이디를 설정해주시기 바랍니다.";
            obj2.SetActive(true);
            obj.SetActive(false);
            return;
        }
        if (Pw.Length > 20 || Pw.Length < 6)
        {
            ResSignupText.text = "6 ~ 20자 이내로 패스워드를 설정해주시기 바랍니다.";
            obj2.SetActive(true);
            obj.SetActive(false);
            return;
        }
        if (NickName.Length > 20 || NickName.Length < 6)
        {
            ResSignupText.text = "6 ~ 20자 이내로 닉네임을 설정해주시기 바랍니다.";
            obj2.SetActive(true);
            obj.SetActive(false);
            return;
        }
        //패킷 구조체에 담고 마샬링해서 바이트 배열에 담기
        PT_SIGNUP Signup_Pack = new PT_SIGNUP(Id, Pw, NickName, GValue.packetNumber++);

        //직렬화시켜 보내기
        sendByte = Serialize(Signup_Pack);

        Connect.Instance.socket.Send(sendByte, sendByte.Length, 0);

        recvReady = true;
    }

    public void Confirm()
    {
        obj.SetActive(true);
        obj2.SetActive(false);
        obj3.SetActive(false);
    }

    public void SignupComplete()
    {
        obj.SetActive(true);
        obj2.SetActive(false);
        obj3.SetActive(false);
        SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
    }

    public void OnSignup()
    {
        SignupButton.image.overrideSprite = SignupImage;
        ConfirmButton.image.overrideSprite = ConfirmImage2;
    }

    public void OnOK()
    {
        OKButton.image.overrideSprite = OKImage;
    }

    public void OnBack()
    {
        BackButton.image.overrideSprite = BackImage;
    }

    public void OnConfirm()
    {
        ConfirmButton.image.overrideSprite = ConfirmImage;
        SignupButton.image.overrideSprite = SignupImage2;
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