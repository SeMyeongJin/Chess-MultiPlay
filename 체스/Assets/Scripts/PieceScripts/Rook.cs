using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    public override bool[,] PosibleMove()
    {
        bool[,] position = new bool[8, 8];

        ChessPiece piece;

        int file = CurrentX;
        int rank = CurrentY;
        while(true)
        {
            file++;
            if (file >= 8)
                break;

            piece = BoardManager.Instance.chessPieces[file, CurrentY];
            if (piece == null)
                position[file, CurrentY] = true;
            if (piece != null)
            {
                if (piece.isWhite != isWhite)
                    position[file, CurrentY] = true;

                break;
            }
        }

        file = CurrentX;
        while (true)
        {
            file--;
            if (file < 0)
                break;

            piece = BoardManager.Instance.chessPieces[file, CurrentY];
            if (piece == null)
                position[file, CurrentY] = true;
            if (piece != null)
            {
                if (piece.isWhite != isWhite)
                    position[file, CurrentY] = true;

                break;
            }
        }

        rank = CurrentY;
        while (true)
        {
            rank++;
            if (rank >= 8)
                break;

            piece = BoardManager.Instance.chessPieces[CurrentX, rank];
            if (piece == null)
                position[CurrentX, rank] = true;
            if (piece != null)
            {
                if (piece.isWhite != isWhite)
                    position[CurrentX, rank] = true;

                break;
            }
        }

        rank = CurrentY;
        while (true)
        {
            rank--;
            if (rank < 0)
                break;

            piece = BoardManager.Instance.chessPieces[CurrentX, rank];
            if (piece == null)
                position[CurrentX, rank] = true;
            if (piece != null)
            {
                if (piece.isWhite != isWhite)
                    position[CurrentX, rank] = true;

                break;
            }
        }

        return position;
    }
}
