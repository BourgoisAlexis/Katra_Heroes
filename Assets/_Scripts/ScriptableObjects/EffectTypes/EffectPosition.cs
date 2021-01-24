using UnityEngine;
using System;

[Serializable]
public class EffectPosition : Effect
{
    #region Variables
    [Header("Position")]
    public e_positionEffects Effect;
    public int Distance;

    private Square[,] board => utility.Board;
    private BoardUtility utility;
    #endregion


    public override void Set()
    {
        utility = GameplayManager.Instance.Utility;
    }

    public override void Use(Square[] _targets, HeroPiece _user)
    {
        Square desti = _targets[0];

        if (desti == null)
            return;

        if (Effect == e_positionEffects.Teleport)
        {
            HeroPiece piece = desti.Piece;

            piece.NotifyMovement(board[_user.Position.x, _user.Position.y], false);
            _user.NotifyMovement(desti, false);
        }

        else
        {
            Vector2Int direction = desti.Position - _user.Position;
            int x = direction.x != 0 ? (int)Mathf.Sign(direction.x) : 0;
            int y = direction.y != 0 ? (int)Mathf.Sign(direction.y) : 0;
            direction = new Vector2Int(x, y);


            if (Effect == e_positionEffects.Push)
            {
                Square square = utility.GetFinalSquareInDirection(desti, direction, Distance + 1);
                desti.Piece.NotifyMovement(square, false);
            }
            else if (Effect == e_positionEffects.Pull)
            {
                Square square = utility.GetFinalSquareInDirection(desti, -direction, Distance + 1);
                desti.Piece.NotifyMovement(square, false);
            }
        }
    }
}