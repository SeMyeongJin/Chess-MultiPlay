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

public class AsyncSocket : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

namespace AsyncSockets
{
    public enum ClientState
    {
        DISCONNECTED,
        CONNECTING,
        CONNECTED
    }

    public class AsyncCallbackClient
    {
        static AsyncCallbackClient client;

        Socket clientSocket;

        public ClientState state = ClientState.DISCONNECTED;

        public byte[] recvByte = new byte[256];

        public Queue<string> dataQueue = new Queue<string>();

        public static AsyncCallbackClient Instance()
        {
            if (client == null)
            {
                client = new AsyncCallbackClient();
            }
            return client;
        }

        public void Connect(string ip, int port)
        {
            try
            {
                state = ClientState.CONNECTING;
                IPAddress ipAddress = IPAddress.Parse(ip);
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                clientSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), clientSocket);
            }
            catch
            {
                state = ClientState.DISCONNECTED;
            }
        }
        void ConnectCallback(IAsyncResult iAR)
        {
            try
            {
                clientSocket.EndConnect(iAR);
                recvByte = new byte[clientSocket.ReceiveBufferSize];
                clientSocket.BeginReceive(recvByte, 0, recvByte.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
                state = ClientState.CONNECTED;
            }
            catch
            {
                state = ClientState.DISCONNECTED;
            }
        }

        public void SendData(byte[] data)
        {
            try
            {
                clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), clientSocket);
            }
            catch
            {
                Close();
            }
        }
        void SendCallback(IAsyncResult iAR)
        {
            Socket handler = (Socket)iAR.AsyncState;
            int byteSent = handler.EndSend(iAR);
        }

        public void RecvData()
        {
            try
            {
                clientSocket.BeginReceive(recvByte, 0, recvByte.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
            }
            catch
            {
                Close();
            }
        }
        void ReceiveCallback(IAsyncResult iAR)
        {
            try
            {
                int recv = clientSocket.EndReceive(iAR);
                if (recv > 0)
                {
                    string data = Encoding.UTF8.GetString(recvByte, 0, recv);
                    data = data.Trim('\0');
                    dataQueue.Enqueue(data);
                    clientSocket.BeginReceive(recvByte, 0, recvByte.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
                }
            }
            catch
            {
                Close();
            }
        }

        public void Close()
        {
            state = ClientState.DISCONNECTED;
            if (clientSocket != null)
            {
                try
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                }
                catch
                { }
                clientSocket.Close();
            }
        }
    }
}