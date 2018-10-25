using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override bool[,] PosibleMove()
    {
        bool[,] position = new bool[8, 8];

        KingMove(CurrentX - 1, CurrentY + 1, ref position);
        KingMove(CurrentX, CurrentY + 1, ref position);
        KingMove(CurrentX + 1, CurrentY + 1, ref position);
        KingMove(CurrentX - 1, CurrentY, ref position);
        KingMove(CurrentX + 1, CurrentY, ref position);
        KingMove(CurrentX - 1, CurrentY - 1, ref position);
        KingMove(CurrentX, CurrentY - 1, ref position);
        KingMove(CurrentX + 1, CurrentY - 1, ref position);

        return position;
    }

    public void KingMove(int x, int y, ref bool[,] pos)
    {
        ChessPiece piece;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            piece = BoardManager.Instance.chessPieces[x, y];
            if (piece == null)
                pos[x, y] = true;
            if (piece != null)
            {
                if (isWhite != piece.isWhite)
                    pos[x, y] = true;
            }
        }
    }
}
