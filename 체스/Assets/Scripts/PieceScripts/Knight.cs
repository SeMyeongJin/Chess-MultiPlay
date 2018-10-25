using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public override bool[,] PosibleMove()
    {
        bool[,] position = new bool[8, 8];

        // Up...
        KnightMove(CurrentX - 1, CurrentY + 2, ref position);
        KnightMove(CurrentX + 1, CurrentY + 2, ref position);

        // Right...
        KnightMove(CurrentX + 2, CurrentY + 1, ref position);
        KnightMove(CurrentX + 2, CurrentY - 1, ref position);

        // Down...
        KnightMove(CurrentX - 1, CurrentY - 2, ref position);
        KnightMove(CurrentX + 1, CurrentY - 2, ref position);

        // Left...
        KnightMove(CurrentX - 2, CurrentY + 1, ref position);
        KnightMove(CurrentX - 2, CurrentY - 1, ref position);

        return position;
    }

    public void KnightMove(int x, int y, ref bool[,] position)
    {
        ChessPiece piece;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            piece = BoardManager.Instance.chessPieces[x, y];
            if (piece == null)
            {
                position[x, y] = true;
            }
            if (piece != null)
            {
                if (isWhite != piece.isWhite)
                {
                    position[x, y] = true;
                }
            }
        }
    }
}
