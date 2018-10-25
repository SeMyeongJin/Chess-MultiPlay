using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class BoardManager : MonoBehaviour {

    public static BoardManager Instance { get; set; }
    private bool[,] posiblePositions { get; set; }

    public ChessPiece[,] chessPieces { get; set; }
    private ChessPiece selectedPiece;

    private GameObject canvas;
    private int iPromotion;
    private static int x, y;

    private const float tile_size = 1.0f;
    private const float tile_offset = 0.5f;

    private int selectionX = -1;
    private int selectionY = -1;

    public List<GameObject> piecePrefabs;
    private List<GameObject> activePiece;

    private Material previousMat;
    public Material selectedMat;

    // 특수 룰
    public int[] enPassant { get; set; }

    private Quaternion orientation = Quaternion.Euler(0, 0, 0);

    public bool isWhiteTurn = true;

    private bool isWhiteTeam;
    private bool isBlackTeam;

    private static int bx, by;

    byte[] sendByte;

    GameObject whiteCamera;
    GameObject blackCamera;
    GameObject mainCamera;

    GameObject ResultObj;
    public Text Result;

    GameObject Canvas2;

    GameObject ResignCanvas;

    // Use this for initialization
    void Start () {
        Instance = this;
        SpawnAllPieces();
        canvas = GameObject.Find("Canvas");
        canvas.SetActive(false);

        whiteCamera = GameObject.Find("WhiteCamera");
        blackCamera = GameObject.Find("BlackCamera");
        mainCamera = GameObject.Find("Main Camera");

        isWhiteTeam = LobbyScene.isWhite;
        isBlackTeam = !LobbyScene.isWhite;

        ResultObj = GameObject.Find("ResultCanvas");
        ResultObj.SetActive(false);

        ResignCanvas = GameObject.Find("ResignCanvas");
        ResignCanvas.SetActive(false);
        Canvas2 = GameObject.Find("Canvas2");
        Canvas2.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
        UpdateSelection();
        DrawChessBoard();

        int dataCount = AsyncSockets.AsyncCallbackClient.Instance().dataQueue.Count;
        if (dataCount > 0)
        {
            for (int i = 0; i < dataCount; i++)
            {
                AsyncSockets.AsyncCallbackClient.Instance().dataQueue.Dequeue();
                ReadPacket();
            }
        }

        if (canvas.activeSelf != true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (selectionX >= 0 && selectionY >= 0)
                {
                    if (selectedPiece == null)
                    {
                        x = selectionX;
                        y = selectionY;
                        SelectPiece(selectionX, selectionY);
                    }
                    else
                    {
                        bx = selectedPiece.CurrentX;
                        by = selectedPiece.CurrentY;
                        x = selectionX;
                        y = selectionY;
                        MovePiece(selectionX, selectionY);
                        SendPacket(bx, by, selectionX, selectionY, GValue.packetNumber++);
                    }
                }
            }
        }
	}

    // 체스 말 클릭
    private void SelectPiece(int x, int y)
    {
        if (chessPieces[x, y] == null)
            return;

        if (chessPieces[x, y].isWhite != isWhiteTurn)
            return;

        // 이동 가능한 포지션이 없을 경우 다른 말 바로 선택
        bool hasAtleastOneMove = false;
        posiblePositions = chessPieces[x, y].PosibleMove();
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (posiblePositions[i, j])
                    hasAtleastOneMove = true;

        if (!hasAtleastOneMove)
            return;

        // 선택된 말 표시
        selectedPiece = chessPieces[x, y];
        previousMat = selectedPiece.GetComponent<MeshRenderer>().material;
        selectedMat.mainTexture = previousMat.mainTexture;
        selectedPiece.GetComponent<MeshRenderer>().material = selectedMat;
        PieceMovement.Instance.PosibleMovePosition(posiblePositions);
    }

    // 체스 말 이동
    private void MovePiece(int x, int y)
    {
        if (posiblePositions[x,y] == true)
        {
            ChessPiece piece = chessPieces[x, y];
            if (piece != null && piece.isWhite != isWhiteTurn)
            {
                if (piece.GetType() == typeof(King))
                {
                    EndGame();
                    return;
                }
                activePiece.Remove(piece.gameObject);
                Destroy(piece.gameObject);
            }

            if (x == enPassant[0] && y == enPassant[1])
            {
                if (isWhiteTurn)
                    piece = chessPieces[x, y - 1];
                else
                    piece = chessPieces[x, y + 1];

                activePiece.Remove(piece.gameObject);
                Destroy(piece.gameObject);
            }
            enPassant[0] = -1;
            enPassant[1] = -1;
            if (selectedPiece.GetType() == typeof(Pawn))
            {
                if (y == 7 && isWhiteTeam)
                {
                    activePiece.Remove(selectedPiece.gameObject);
                    Destroy(selectedPiece.gameObject);
                    canvas.SetActive(true);
                }
                if (y == 7)
                {
                    activePiece.Remove(selectedPiece.gameObject);
                    Destroy(selectedPiece.gameObject);
                }
                if (y == 0 && isBlackTeam)
                {
                    activePiece.Remove(selectedPiece.gameObject);
                    Destroy(selectedPiece.gameObject);
                    canvas.SetActive(true);
                }
                if (y == 0)
                {
                    activePiece.Remove(selectedPiece.gameObject);
                    Destroy(selectedPiece.gameObject);
                }
                if (selectedPiece.CurrentY == 1 && y == 3)
                {
                    enPassant[0] = x;
                    enPassant[1] = y - 1;
                }
                else if (selectedPiece.CurrentY == 6 && y == 4)
                {
                    enPassant[0] = x;
                    enPassant[1] = y + 1;
                }
            }
            chessPieces[selectedPiece.CurrentX, selectedPiece.CurrentY] = null;
            selectedPiece.transform.position = GetTileCenter(x, y);
            selectedPiece.SetPosition(x, y);
            chessPieces[x, y] = selectedPiece;

            isWhiteTurn = !isWhiteTurn;
        }

        selectedPiece.GetComponent<MeshRenderer>().material = previousMat;
        PieceMovement.Instance.ExcludePosition();
        selectedPiece = null;
    }

    private void UpdateSelection()
    {
        if (isWhiteTeam)
        {
            whiteCamera.SetActive(true);
            blackCamera.SetActive(false);

            if (!Camera.main)
                return;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessField")))
            {
                selectionX = (int)hit.point.x;
                selectionY = (int)hit.point.z;
                if (selectedPiece == null && chessPieces[selectionX, selectionY] != null && chessPieces[selectionX, selectionY].gameObject.tag == "Black")
                {
                    selectionX = -1;
                    selectionY = -1;
                }
            }
            else
            {
                selectionX = -1;
                selectionY = -1;
            }
        }

        if (isBlackTeam)
        {
            blackCamera.SetActive(true);
            whiteCamera.SetActive(false);
            mainCamera.transform.position = blackCamera.transform.position;
            mainCamera.transform.rotation = blackCamera.transform.rotation;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessField")))
            {
                selectionX = (int)hit.point.x;
                selectionY = (int)hit.point.z;
                if (selectedPiece == null && chessPieces[selectionX, selectionY] != null && chessPieces[selectionX, selectionY].gameObject.tag == "White")
                {
                    selectionX = -1;
                    selectionY = -1;
                }
            }
            else
            {
                selectionX = -1;
                selectionY = -1;
            }
        }
    }

    // 체스 말 스폰
    private void SpawnPieces(int index, int x, int y)
    {
        if (index == 10)
        {
            GameObject obj = Instantiate(piecePrefabs[index], GetTileCenter(x, y), Quaternion.Euler(0, 180.0f, 0)) as GameObject;
            obj.transform.SetParent(transform);
            chessPieces[x, y] = obj.GetComponent<ChessPiece>();
            chessPieces[x, y].SetPosition(x, y);
            activePiece.Add(obj);
        }
        else
        {
            GameObject obj = Instantiate(piecePrefabs[index], GetTileCenter(x, y), orientation) as GameObject;
            obj.transform.SetParent(transform);
            chessPieces[x, y] = obj.GetComponent<ChessPiece>();
            chessPieces[x, y].SetPosition(x, y);
            activePiece.Add(obj);
        }
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 newVec = Vector3.zero;

        newVec.x += (tile_size * x) + tile_offset;
        newVec.z += (tile_size * y) + tile_offset;

        return newVec;
    }

    // 체스 말 배치 초기화
    private void SpawnAllPieces()
    {
        activePiece = new List<GameObject>();
        chessPieces = new ChessPiece[8, 8];
        enPassant = new int[2] { -1, -1 };

        // Spawn White Team
        // King
        SpawnPieces(0, 4, 0);

        // Queen
        SpawnPieces(1, 3, 0);

        // Rooks
        SpawnPieces(2, 0, 0);
        SpawnPieces(2, 7, 0);

        // Bishops
        SpawnPieces(3, 2, 0);
        SpawnPieces(3, 5, 0);

        // Knights
        SpawnPieces(4, 1, 0);
        SpawnPieces(4, 6, 0);

        // Pawns
        for (int i = 0; i < 8; i++)
            SpawnPieces(5, i, 1);

        // Spawn Black Team
        // King
        SpawnPieces(6, 4, 7);

        // Queen
        SpawnPieces(7, 3, 7);

        // Rooks
        SpawnPieces(8, 0, 7);
        SpawnPieces(8, 7, 7);

        // Bishops
        SpawnPieces(9, 2, 7);
        SpawnPieces(9, 5, 7);

        // Knights
        SpawnPieces(10, 1, 7);
        SpawnPieces(10, 6, 7);

        // Pawns
        for (int i = 0; i < 8; i++)
            SpawnPieces(11, i, 6);
    }

    private void DrawChessBoard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for (int  i = 0; i < 9; i++)
        {
            Vector3 startVector = Vector3.forward * i;
            Debug.DrawLine(startVector, startVector + widthLine);
            for (int j = 0; j < 9; j++)
            {
                startVector = Vector3.right * j;
                Debug.DrawLine(startVector, startVector + heightLine);
            }
        }

        // Draw selected
        if (selectionX >= 0 && selectionY >=0)
        {
            Debug.DrawLine(Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));

            Debug.DrawLine(Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
                Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
        }
    }

    private void EndGame()
    {
        if (LobbyScene.isOfficialGame)
        {
            if (isWhiteTeam && isWhiteTeam == isWhiteTurn)
            {
                PT_OFFICIAL_GAME_WIN gameWinPack = new PT_OFFICIAL_GAME_WIN(LoginScene.NickName, GValue.packetNumber++);

                sendByte = Serialize(gameWinPack);

                AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);

                Result.text = "승리";

                ResultObj.SetActive(true);
            }
            if (isWhiteTeam && isWhiteTeam != isWhiteTurn)
            {
                PT_OFFICIAL_GAME_LOSE gameLosePack = new PT_OFFICIAL_GAME_LOSE(LoginScene.NickName, GValue.packetNumber++);

                sendByte = Serialize(gameLosePack);

                AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);

                Result.text = "패배";

                ResultObj.SetActive(true);
            }
            if (isBlackTeam && isBlackTeam != isWhiteTurn)
            {
                PT_OFFICIAL_GAME_WIN gameWinPack = new PT_OFFICIAL_GAME_WIN(LoginScene.NickName, GValue.packetNumber++);

                sendByte = Serialize(gameWinPack);

                AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);

                Result.text = "승리";

                ResultObj.SetActive(true);
            }
            if (isBlackTeam && isBlackTeam == isWhiteTurn)
            {
                PT_OFFICIAL_GAME_LOSE gameLosePack = new PT_OFFICIAL_GAME_LOSE(LoginScene.NickName, GValue.packetNumber++);

                sendByte = Serialize(gameLosePack);

                AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);

                Result.text = "패배";

                ResultObj.SetActive(true);
            }
        }
        if (!LobbyScene.isOfficialGame)
        {
            if (isWhiteTeam && isWhiteTeam == isWhiteTurn)
            {
                PT_FRIENDSHIP_GAME_WIN gameWinPack = new PT_FRIENDSHIP_GAME_WIN(LoginScene.NickName, GValue.packetNumber++);

                sendByte = Serialize(gameWinPack);

                AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);

                Result.text = "승리";

                ResultObj.SetActive(true);
            }
            if (isWhiteTeam && isWhiteTeam != isWhiteTurn)
            {
                Result.text = "패배";

                ResultObj.SetActive(true);
            }
            if (isBlackTeam && isBlackTeam != isWhiteTurn)
            {
                PT_FRIENDSHIP_GAME_WIN gameWinPack = new PT_FRIENDSHIP_GAME_WIN(LoginScene.NickName, GValue.packetNumber++);

                sendByte = Serialize(gameWinPack);

                AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);

                Result.text = "승리";

                ResultObj.SetActive(true);
            }
            if (isBlackTeam && isBlackTeam == isWhiteTurn)
            {
                Result.text = "패배";

                ResultObj.SetActive(true);
            }
        }

        foreach (GameObject obj in activePiece)
            Destroy(obj);

        isWhiteTurn = true;
        PieceMovement.Instance.ExcludePosition();
        SpawnAllPieces();
    }

    public void OnResignBtn()
    {
        ResignCanvas.SetActive(true);
    }

    public void NoBtn()
    {
        ResignCanvas.SetActive(false);
    }

    public void ResignGame()
    {
        PT_RESIGNS resPack = new PT_RESIGNS(GValue.packetNumber++);

        sendByte = Serialize(resPack);

        AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);

        Result.text = "패배";

        ResultObj.SetActive(true);
        ResignCanvas.SetActive(false);

        if (LobbyScene.isOfficialGame)
        {
            PT_OFFICIAL_GAME_LOSE gameLosePack = new PT_OFFICIAL_GAME_LOSE(LoginScene.NickName, GValue.packetNumber++);

            sendByte = Serialize(gameLosePack);

            AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);
        }
    }

    private void EnemyResignGame()
    {
        Result.text = "승리";

        ResultObj.SetActive(true);

        if (LobbyScene.isOfficialGame)
        {
            PT_OFFICIAL_GAME_WIN gameWinPack = new PT_OFFICIAL_GAME_WIN(LoginScene.NickName, GValue.packetNumber++);

            sendByte = Serialize(gameWinPack);

            AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);
        }
        if (!LobbyScene.isOfficialGame)
        {
            PT_FRIENDSHIP_GAME_WIN gameWinPack = new PT_FRIENDSHIP_GAME_WIN(LoginScene.NickName, GValue.packetNumber++);

            sendByte = Serialize(gameWinPack);

            AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);
        }
    }

    public void OnQueenBtn()
    {
        if (y == 7)
            iPromotion = 1;
        else
            iPromotion = 7;

        SpawnPieces(iPromotion, x, y);

        PT_PIECE_PROMOTION promotionPack = new PT_PIECE_PROMOTION(iPromotion, x, y, GValue.packetNumber++);
        sendByte = Serialize(promotionPack);
        AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);

        canvas.SetActive(false);
    }

    public void OnRookBtn()
    {
        if (y == 7)
            iPromotion = 2;
        else
            iPromotion = 8;

        SpawnPieces(iPromotion, x, y);

        PT_PIECE_PROMOTION promotionPack = new PT_PIECE_PROMOTION(iPromotion, x, y, GValue.packetNumber++);
        sendByte = Serialize(promotionPack);
        AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);

        canvas.SetActive(false);
    }

    public void OnBishopBtn()
    {
        if (y == 7)
            iPromotion = 3;
        else
            iPromotion = 9;

        SpawnPieces(iPromotion, x, y);

        PT_PIECE_PROMOTION promotionPack = new PT_PIECE_PROMOTION(iPromotion, x, y, GValue.packetNumber++);
        sendByte = Serialize(promotionPack);
        AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);

        canvas.SetActive(false);
    }

    public void OnKnightBtn()
    {
        if (y == 7)
            iPromotion = 4;
        else
            iPromotion = 10;

        SpawnPieces(iPromotion, x, y);

        PT_PIECE_PROMOTION promotionPack = new PT_PIECE_PROMOTION(iPromotion, x, y, GValue.packetNumber++);
        sendByte = Serialize(promotionPack);
        AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);

        canvas.SetActive(false);
    }

    private void SendPacket(int bx, int by, int ax, int ay, int packNum)
    {
        PT_PIECE_MOVE movePack = new PT_PIECE_MOVE(bx, by, selectionX, selectionY, packNum);

        sendByte = Serialize(movePack);

        AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);
    }

    private void ReadPacket()
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
        if (recvPacket.code == TCP_PROTOCOL.PT_PIECE_MOVE)
        {
            PT_PIECE_MOVE movePacket = new PT_PIECE_MOVE();
            movePacket = (PT_PIECE_MOVE)Deserialize(AsyncSockets.AsyncCallbackClient.Instance().recvByte, typeof(PT_PIECE_MOVE));
            ProcMovePacket(movePacket.bx, movePacket.by, movePacket.ax, movePacket.ay);
            Debug.Log("" + movePacket.bx.ToString() + ", " + movePacket.by.ToString() + ", " + movePacket.ax.ToString() + ", " + movePacket.ay.ToString());
        }
        if (recvPacket.code == TCP_PROTOCOL.PT_PIECE_PROMOTION)
        {
            PT_PIECE_PROMOTION promotionPacket = new PT_PIECE_PROMOTION();
            promotionPacket = (PT_PIECE_PROMOTION)Deserialize(AsyncSockets.AsyncCallbackClient.Instance().recvByte, typeof(PT_PIECE_PROMOTION));
            SpawnPieces(promotionPacket.type, promotionPacket.xpos, promotionPacket.ypos);
        }
        if (recvPacket.code == TCP_PROTOCOL.PT_DELIVERY_RESIGNS)
        {
            EnemyResignGame();
        }
    }

    private void ProcMovePacket(int bx, int by, int ax, int ay)
    {
        SelectPiece(bx, by);
        MovePiece(ax, ay);
    }

    public void OnConfirmBtn()
    {
        PT_ROOM_LEAVE leavePack = new PT_ROOM_LEAVE(GValue.packetNumber++);

        sendByte = Serialize(leavePack);

        AsyncSockets.AsyncCallbackClient.Instance().SendData(sendByte);

        SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }

    //바이트 배열로 직렬화 시켜주는거
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

    //배열을 구조체에 비직렬화시킴
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
