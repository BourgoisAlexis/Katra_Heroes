using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "CUSTOM/Ability/Position")]
public class AbilityPosition : Ability
{
    #region Variables
    [Header("Position")]
    public e_positionEffects Effect;
    public int Distance;

    private Square[,] board;
    private BoardUtility utility;
    #endregion


    public override void Set()
    {
        abilityType = e_abilityType.Position;
        board = GameplayManager.Instance.BoardManager.Board;
        utility = GameplayManager.Instance.BoardManager.Utility;
    }

    public override void Use(Square _target, HeroPiece _user)
    {
        Vector2Int direction = _target.Position - _user.Position;
        int x = direction.x != 0 ? (int)Mathf.Sign(direction.x) : 0;
        int y = direction.y != 0 ? (int)Mathf.Sign(direction.y) : 0;
        direction = new Vector2Int(x, y);

        switch (Effect)
        {
            case e_positionEffects.Teleport:
                if (_target.Piece != null)
                    _target.Piece.MovePiece(board[_user.Position.x, _user.Position.y], false);

                _user.MovePiece(_target, false);
                break;

            case e_positionEffects.Pull:
                if (_target.Piece != null)
                {
                    Square square = utility.GetOneSquareInDirection(_target, direction, Distance);
                    _target.Piece.MovePiece(square, false);
                }
                break;

            case e_positionEffects.Push:
                if (_target.Piece != null)
                {
                    Square square = utility.GetOneSquareInDirection(_target, direction, Distance);
                    _target.Piece.MovePiece(square, false);
                }
                break;
        }

    }
}