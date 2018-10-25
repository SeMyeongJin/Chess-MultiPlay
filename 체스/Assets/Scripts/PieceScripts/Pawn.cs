using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
	public override bool[,] PosibleMove()
    {
        bool[,] position = new bool[8, 8];

        ChessPiece piece1, piece2;

        int[] iArray = BoardManager.Instance.enPassant;

        if (isWhite)
        {
            if (CurrentX != 0 && CurrentY != 7)
            {
                if (iArray[0] == CurrentX - 1 && iArray[1] == CurrentY + 1)
                    position[CurrentX - 1, CurrentY + 1] = true;

                piece1 = BoardManager.Instance.chessPieces[CurrentX - 1, CurrentY + 1];
                if (piece1 != null && !piece1.isWhite)
                    position[CurrentX - 1, CurrentY + 1] = true;
            }
            if (CurrentX != 7 && CurrentY != 7)
            {
                if (iArray[0] == CurrentX + 1 && iArray[1] == CurrentY + 1)
                    position[CurrentX + 1, CurrentY + 1] = true;

                piece1 = BoardManager.Instance.chessPieces[CurrentX + 1, CurrentY + 1];
                if (piece1 != null && !piece1.isWhite)
                    position[CurrentX + 1, CurrentY + 1] = true;
            }
            if (CurrentY != 7)
            {
                piece1 = BoardManager.Instance.chessPieces[CurrentX, CurrentY + 1];
                if (piece1 == null)
                    position[CurrentX, CurrentY + 1] = true;
            }
            if (CurrentY == 1)
            {
                piece1 = BoardManager.Instance.chessPieces[CurrentX, CurrentY + 1];
                piece2 = BoardManager.Instance.chessPieces[CurrentX, CurrentY + 2];
                if (piece1 == null && piece2 == null)
                    position[CurrentX, CurrentY + 2] = true;
            }
        }
        if (!isWhite)
        {
            if (CurrentX != 0 && CurrentY != 0)
            {
                if (iArray[0] == CurrentX - 1 && iArray[1] == CurrentY - 1)
                    position[CurrentX - 1, CurrentY - 1] = true;

                piece1 = BoardManager.Instance.chessPieces[CurrentX - 1, CurrentY - 1];
                if (piece1 != null && piece1.isWhite)
                    position[CurrentX - 1, CurrentY - 1] = true;
            }
            if (CurrentX != 7 && CurrentY != 0)
            {
                if (iArray[0] == CurrentX + 1 && iArray[1] == CurrentY - 1)
                    position[CurrentX + 1, CurrentY - 1] = true;

                piece1 = BoardManager.Instance.chessPieces[CurrentX + 1, CurrentY - 1];
                if (piece1 != null && piece1.isWhite)
                    position[CurrentX + 1, CurrentY - 1] = true;
            }
            if (CurrentY != 0)
            {
                piece1 = BoardManager.Instance.chessPieces[CurrentX, CurrentY - 1];
                if (piece1 == null)
                    position[CurrentX, CurrentY - 1] = true;
            }
            if (CurrentY == 6)
            {
                piece1 = BoardManager.Instance.chessPieces[CurrentX, CurrentY - 1];
                piece2 = BoardManager.Instance.chessPieces[CurrentX, CurrentY - 2];
                if (piece1 == null && piece2 == null)
                    position[CurrentX, CurrentY - 2] = true;
            }
        }

        return position;
    }
}
