using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public enum PROTOCOL
{
    PT_VERSION = 0x1000000,
    PT_SIGNUP,
    PT_SIGNUP_SUCC,
    PT_SIGNUP_FAIL,
    PT_LOGIN,
    PT_LOGIN_SUCC,
    PT_LOGIN_FAIL,
    PT_END
};

public enum ERROR_CODE
{
    EC_SIGNUP_ID_ALREADY_REGIST = 1,
    EC_SIGNUP_NAME_ALREADY_REGIST,
    EC_SIGNUP_ID_ERROR,
    EC_SIGNUP_PW_ERROR,
    EC_SIGNUP_NAME_ERROR,
    EC_LOGIN_ID_AND_PASS_ERROR,
};

public class PacketStructure : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class PacketHeader 
{
    public int PackSize;
    public int PackNum;
    public PROTOCOL code;
    public PacketHeader() { }

    //헤더
    public PacketHeader(int size, int num, PROTOCOL type)
    {
        PackSize = size;
        PackNum = num;
        code = type;
    }
}

// PT_SIGNUP
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class PT_SIGNUP : PacketHeader
{
    public PT_SIGNUP(string id, string pw, string name, int packNum)
        : base(Marshal.SizeOf(typeof(PT_SIGNUP)), packNum, PROTOCOL.PT_SIGNUP)
    {
        ID = id;
        PW = pw;
        UserName = name;
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    private string ID;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    private string PW;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    private string UserName;
}

// PT_SIGNUP_SUCC
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_SIGNUP_SUCC : PacketHeader
{
    public PT_SIGNUP_SUCC()
        : base(Marshal.SizeOf(typeof(PT_SIGNUP_SUCC)), sizeof(int), PROTOCOL.PT_SIGNUP_SUCC)
    { }
}

// PT_SIGNUP_FAIL
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_SIGNUP_FAIL : PacketHeader
{
    public PT_SIGNUP_FAIL() { }
    public PT_SIGNUP_FAIL(int errorcode)
        : base(Marshal.SizeOf(typeof(PT_SIGNUP_FAIL)), sizeof(int), PROTOCOL.PT_SIGNUP_FAIL)
    {
        ErrorCode = errorcode;
    }
    [MarshalAs(UnmanagedType.I4)]
    private int ErrorCode;

    public int errorCode
    {
        get
        {
            return ErrorCode;
        }
        set
        {
            ErrorCode = value;
        }
    }
}

// PT_LOGIN 
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class PT_LOGIN : PacketHeader
{
    public PT_LOGIN(string id, string pw, int packNum)
        : base(Marshal.SizeOf(typeof(PT_LOGIN)), packNum, PROTOCOL.PT_LOGIN)
    {
        ID = id;
        PW = pw;
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    private string ID;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    private string PW;
}

// PT_LOGIN_SUCC
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_LOGIN_SUCC : PacketHeader
{
    public PT_LOGIN_SUCC() { }
    public PT_LOGIN_SUCC(string name, int point)
        : base(Marshal.SizeOf(typeof(PT_LOGIN_SUCC)), sizeof(int), PROTOCOL.PT_LOGIN_SUCC)
    {
        NickName = name;
        LifePoint = point;
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
    public string NickName;
    [MarshalAs(UnmanagedType.I4)]
    public int LifePoint;
}

// PT_LOGIN_FAIL 
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_LOGIN_FAIL : PacketHeader
{
    public PT_LOGIN_FAIL() { }
    public PT_LOGIN_FAIL(int errorcode)
        : base(Marshal.SizeOf(typeof(PT_LOGIN_FAIL)), sizeof(int), PROTOCOL.PT_LOGIN_FAIL)
    {
        ErrorCode = errorcode;
    }
    [MarshalAs(UnmanagedType.I4)]
    private int ErrorCode;
    
    public int errorCode
    {
        get
        {
            return ErrorCode;
        }
        set
        {
            ErrorCode = value;
        }
    } 
}