using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class GamePacketStructure : MonoBehaviour {
	void Start () {
		
	}

	void Update () {
		
	}
}

public enum TCP_PROTOCOL
{
    PT_VERSION = 0x1000000,
    PT_REQ_USER_INFO,
    PT_RES_USER_INFO,
    PT_OFFICIAL_GAME_START,
    PT_OFFICIAL_GAME_START_SUCC,
    PT_OFFICIAL_GAME_START_FAIL,
    PT_FRIENDSHIP_GAME_START,
    PT_FRIENDSHIP_GAME_START_SUCC,
    PT_FRIENDSHIP_GAME_START_FAIL,
    PT_ROOM_LEAVE,
    PT_ROOM_LEAVE_SUCC,
    PT_ROOM_LEAVE_FAIL,
    PT_GAME_START_ALL,
    PT_GAME_END_ALL,
    PT_CHAT,
    PT_DELIVERY_CHAT,
    PT_PIECE_MOVE,
    PT_PIECE_PROMOTION,
    PT_OFFICIAL_GAME_WIN,
    PT_OFFICIAL_GAME_LOSE,
    PT_FRIENDSHIP_GAME_WIN,
    PT_RESIGNS,
    PT_DELIVERY_RESIGNS,
    PT_END
};

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class GamePacketHeader
{
    public int PackSize;
    public int PackNum;
    public TCP_PROTOCOL code;
    public GamePacketHeader() { }

    //헤더
    public GamePacketHeader(int size, int num, TCP_PROTOCOL type)
    {
        PackSize = size;
        PackNum = num;
        code = type;
    }
}

// PT_REQ_USER_INFO
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class PT_REQ_USER_INFO : GamePacketHeader
{
    public PT_REQ_USER_INFO() { }
    public PT_REQ_USER_INFO(string username, int packNum)
        : base(Marshal.SizeOf(typeof(PT_REQ_USER_INFO)), packNum, TCP_PROTOCOL.PT_REQ_USER_INFO)
    {
        userName = username;
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    private string userName;
}

// PT_RES_USER_INFO
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class PT_RES_USER_INFO : GamePacketHeader
{
    public PT_RES_USER_INFO() { }
    public PT_RES_USER_INFO(int rating, int off_win, int off_lose, int fri_win)
        : base(Marshal.SizeOf(typeof(PT_RES_USER_INFO)), sizeof(int), TCP_PROTOCOL.PT_RES_USER_INFO)
    {
        Rating = rating;
        OFF_WIN = off_win;
        OFF_LOSE = off_lose;
        FRI_WIN = fri_win;
    }
    [MarshalAs(UnmanagedType.I4)]
    private int Rating;
    [MarshalAs(UnmanagedType.I4)]
    private int OFF_WIN;
    [MarshalAs(UnmanagedType.I4)]
    private int OFF_LOSE;
    [MarshalAs(UnmanagedType.I4)]
    private int FRI_WIN;

    public int rating
    {
        get
        {
            return Rating;
        }
        set
        {
            Rating = value;
        }
    }
    public int off_win
    {
        get
        {
            return OFF_WIN;
        }
        set
        {
            OFF_WIN = value;
        }
    }
    public int off_lose
    {
        get
        {
            return OFF_LOSE;
        }
        set
        {
            OFF_LOSE = value;
        }
    }
    public int fri_win
    {
        get
        {
            return FRI_WIN;
        }
        set
        {
            FRI_WIN = value;
        }
    }
}

// PT_OFFICIAL_GAME_START 
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class PT_OFFICIAL_GAME_START : GamePacketHeader
{
    public PT_OFFICIAL_GAME_START(int packNum)
        : base(Marshal.SizeOf(typeof(PT_OFFICIAL_GAME_START)), packNum, TCP_PROTOCOL.PT_OFFICIAL_GAME_START)
    { }
}

// PT_OFFICIAL_GAME_START_SUCC
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_OFFICIAL_GAME_START_SUCC : GamePacketHeader
{
    public PT_OFFICIAL_GAME_START_SUCC()
        : base(Marshal.SizeOf(typeof(PT_OFFICIAL_GAME_START_SUCC)), sizeof(int), TCP_PROTOCOL.PT_OFFICIAL_GAME_START_SUCC)
    { }
}

// PT_OFFICIAL_GAME_START_FAIL 
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_OFFICIAL_GAME_START_FAIL : GamePacketHeader
{
    public PT_OFFICIAL_GAME_START_FAIL() { }
    public PT_OFFICIAL_GAME_START_FAIL(int errorcode)
        : base(Marshal.SizeOf(typeof(PT_OFFICIAL_GAME_START_FAIL)), sizeof(int), TCP_PROTOCOL.PT_OFFICIAL_GAME_START_FAIL)
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

// PT_FRIENDSHIP_GAME_START 
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class PT_FRIENDSHIP_GAME_START : GamePacketHeader
{
    public PT_FRIENDSHIP_GAME_START(int packNum)
        : base(Marshal.SizeOf(typeof(PT_FRIENDSHIP_GAME_START)), packNum, TCP_PROTOCOL.PT_FRIENDSHIP_GAME_START)
    { }
}

// PT_FRIENDSHIP_GAME_START_SUCC
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_FRIENDSHIP_GAME_START_SUCC : GamePacketHeader
{
    public PT_FRIENDSHIP_GAME_START_SUCC()
        : base(Marshal.SizeOf(typeof(PT_FRIENDSHIP_GAME_START_SUCC)), sizeof(int), TCP_PROTOCOL.PT_FRIENDSHIP_GAME_START_SUCC)
    { }
}

// PT_FRIENDSHIP_GAME_START_FAIL 
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_FRIENDSHIP_GAME_START_FAIL : GamePacketHeader
{
    public PT_FRIENDSHIP_GAME_START_FAIL() { }
    public PT_FRIENDSHIP_GAME_START_FAIL(int errorcode)
        : base(Marshal.SizeOf(typeof(PT_FRIENDSHIP_GAME_START_FAIL)), sizeof(int), TCP_PROTOCOL.PT_FRIENDSHIP_GAME_START_FAIL)
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

// PT_ROOM_LEAVE
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class PT_ROOM_LEAVE : GamePacketHeader
{
    public PT_ROOM_LEAVE(int packNum)
        : base(Marshal.SizeOf(typeof(PT_ROOM_LEAVE)), packNum, TCP_PROTOCOL.PT_ROOM_LEAVE)
    { }
}

// PT_ROOM_LEAVE_SUCC
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_ROOM_LEAVE_SUCC : GamePacketHeader
{
    public PT_ROOM_LEAVE_SUCC()
        : base(Marshal.SizeOf(typeof(PT_ROOM_LEAVE_SUCC)), sizeof(int), TCP_PROTOCOL.PT_ROOM_LEAVE_SUCC)
    { }
}

// PT_ROOM_LEAVE_FAIL 
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_ROOM_LEAVE_FAIL : GamePacketHeader
{
    public PT_ROOM_LEAVE_FAIL() { }
    public PT_ROOM_LEAVE_FAIL(int errorcode)
        : base(Marshal.SizeOf(typeof(PT_ROOM_LEAVE_FAIL)), sizeof(int), TCP_PROTOCOL.PT_ROOM_LEAVE_FAIL)
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

//  PT_GAME_START_ALL
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_GAME_START_ALL : GamePacketHeader
{
    public PT_GAME_START_ALL() { }
    public PT_GAME_START_ALL(bool isWhite, bool isOfficialGame)
        : base(Marshal.SizeOf(typeof(PT_GAME_START_ALL)), sizeof(int), TCP_PROTOCOL.PT_GAME_START_ALL)
    {
        IsWhite = isWhite;
        IsOfficialGame = isOfficialGame;
    }
    [MarshalAs(UnmanagedType.Bool)]
    private bool IsWhite;
    [MarshalAs(UnmanagedType.Bool)]
    private bool IsOfficialGame;

    public bool isWhite
    {
        get
        {
            return IsWhite;
        }
        set
        {
            IsWhite = value;
        }
    }
    public bool isOfficialGame
    {
        get
        {
            return IsOfficialGame;
        }
        set
        {
            IsOfficialGame = value;
        }
    }
}

//  PT_GAME_END_ALL
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_GAME_END_ALL : GamePacketHeader
{
    public PT_GAME_END_ALL(int packNum)
        : base(Marshal.SizeOf(typeof(PT_GAME_END_ALL)), packNum, TCP_PROTOCOL.PT_GAME_END_ALL)
    { }
}

// PT_CHAT
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class PT_CHAT : GamePacketHeader
{
    public PT_CHAT(string name, string message, int packNum)
        : base(Marshal.SizeOf(typeof(PT_CHAT)), packNum, TCP_PROTOCOL.PT_CHAT)
    {
        UserName = name;
        Message = message;
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    private string UserName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
    private string Message;
}

// PT_DELIVERY_CHAT
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_DELIVERY_CHAT : GamePacketHeader
{
    public PT_DELIVERY_CHAT(string message)
        : base(Marshal.SizeOf(typeof(PT_DELIVERY_CHAT)), sizeof(int), TCP_PROTOCOL.PT_DELIVERY_CHAT)
    {
        Message = message;
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
    private string Message;
}

// PT_PIECE_MOVE
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class PT_PIECE_MOVE : GamePacketHeader
{
    public PT_PIECE_MOVE() { }
    public PT_PIECE_MOVE(int bx, int by, int ax, int ay)
        : base(Marshal.SizeOf(typeof(PT_PIECE_MOVE)), sizeof(int), TCP_PROTOCOL.PT_PIECE_MOVE)
    {
        Bx = bx;
        By = by;
        Ax = ax;
        Ay = ay;
    }
    public PT_PIECE_MOVE(int bx, int by, int ax, int ay, int packNum)
        : base(Marshal.SizeOf(typeof(PT_PIECE_MOVE)), packNum, TCP_PROTOCOL.PT_PIECE_MOVE)
    {
        Bx = bx;
        By = by;
        Ax = ax;
        Ay = ay;
    }
    [MarshalAs(UnmanagedType.I4)]
    private int Bx;
    [MarshalAs(UnmanagedType.I4)]
    private int By;
    [MarshalAs(UnmanagedType.I4)]
    private int Ax;
    [MarshalAs(UnmanagedType.I4)]
    private int Ay;

    public int bx
    {
        get
        {
            return Bx;
        }
        set
        {
            Bx = value;
        }
    }
    public int by
    {
        get
        {
            return By;
        }
        set
        {
            By = value;
        }
    }
    public int ax
    {
        get
        {
            return Ax;
        }
        set
        {
            Ax = value;
        }
    }
    public int ay
    {
        get
        {
            return Ay;
        }
        set
        {
            Ay = value;
        }
    }
}

// PT_PIECE_PROMOTION
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class PT_PIECE_PROMOTION : GamePacketHeader
{
    public PT_PIECE_PROMOTION() { }
    public PT_PIECE_PROMOTION(int type, int xpos, int ypos)
        : base(Marshal.SizeOf(typeof(PT_PIECE_PROMOTION)), sizeof(int), TCP_PROTOCOL.PT_PIECE_PROMOTION)
    {
        Type = type;
        Xpos = xpos;
        Ypos = ypos;
    }
    public PT_PIECE_PROMOTION(int type, int xpos, int ypos, int packNum)
        : base(Marshal.SizeOf(typeof(PT_PIECE_PROMOTION)), packNum, TCP_PROTOCOL.PT_PIECE_PROMOTION)
    {
        Type = type;
        Xpos = xpos;
        Ypos = ypos;
    }
    [MarshalAs(UnmanagedType.I4)]
    private int Type;
    [MarshalAs(UnmanagedType.I4)]
    private int Xpos;
    [MarshalAs(UnmanagedType.I4)]
    private int Ypos;

    public int type
    {
        get
        {
            return Type;
        }
        set
        {
            Type = value;
        }
    }
    public int xpos
    {
        get
        {
            return Xpos;
        }
        set
        {
            Xpos = value;
        }
    }
    public int ypos
    {
        get
        {
            return Ypos;
        }
        set
        {
            Ypos = value;
        }
    }
}

// PT_OFFICIAL_GAME_WIN
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class PT_OFFICIAL_GAME_WIN : GamePacketHeader
{
    public PT_OFFICIAL_GAME_WIN() { }
    public PT_OFFICIAL_GAME_WIN(string username, int packNum)
        : base(Marshal.SizeOf(typeof(PT_OFFICIAL_GAME_WIN)), packNum, TCP_PROTOCOL.PT_OFFICIAL_GAME_WIN)
    {
        userName = username;
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    private string userName;
}

// PT_OFFICIAL_GAME_LOSE
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class PT_OFFICIAL_GAME_LOSE : GamePacketHeader
{
    public PT_OFFICIAL_GAME_LOSE() { }
    public PT_OFFICIAL_GAME_LOSE(string username, int packNum)
        : base(Marshal.SizeOf(typeof(PT_OFFICIAL_GAME_LOSE)), packNum, TCP_PROTOCOL.PT_OFFICIAL_GAME_LOSE)
    {
        userName = username;
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    private string userName;
}

// PT_FRIENDSHIP_GAME_WIN
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class PT_FRIENDSHIP_GAME_WIN : GamePacketHeader
{
    public PT_FRIENDSHIP_GAME_WIN() { }
    public PT_FRIENDSHIP_GAME_WIN(string username, int packNum)
        : base(Marshal.SizeOf(typeof(PT_FRIENDSHIP_GAME_WIN)), packNum, TCP_PROTOCOL.PT_FRIENDSHIP_GAME_WIN)
    {
        userName = username;
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    private string userName;
}

// PT_RESIGNS
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public class PT_RESIGNS : GamePacketHeader
{
    public PT_RESIGNS(int packNum)
        : base(Marshal.SizeOf(typeof(PT_RESIGNS)), packNum, TCP_PROTOCOL.PT_RESIGNS)
    { }
}

// PT_DELIVERY_RESIGNS
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_DELIVERY_RESIGNS : GamePacketHeader
{
    public PT_DELIVERY_RESIGNS()
        : base(Marshal.SizeOf(typeof(PT_DELIVERY_RESIGNS)), sizeof(int), TCP_PROTOCOL.PT_DELIVERY_RESIGNS)
    { }
}