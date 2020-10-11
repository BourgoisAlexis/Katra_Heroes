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
        board = GameplayManager.Instance.BoardManager.Board;
        utility = GameplayManager.Instance.BoardManager.Utility;
    }

    public override void Use(Square[] _targets, HeroPiece _user)
    {
        Square target = _targets[0];

        Vector2Int direction = target.Position - _user.Position;
        int x = direction.x != 0 ? (int)Mathf.Sign(direction.x) : 0;
        int y = direction.y != 0 ? (int)Mathf.Sign(direction.y) : 0;
        direction = new Vector2Int(x, y);

        switch (Effect)
        {
            case e_positionEffects.Teleport:
                if (target.Piece != null)
                    target.Piece.MovePiece(board[_user.Position.x, _user.Position.y], false);

                _user.MovePiece(target, false);
                break;

            case e_positionEffects.Pull:
                if (target.Piece != null)
                {
                    Square square = utility.GetOneSquareInDirection(target, direction, Distance);
                    target.Piece.MovePiece(square, false);
                }
                break;

            case e_positionEffects.Push:
                if (target.Piece != null)
                {
                    Square square = utility.GetOneSquareInDirection(target, direction, Distance);
                    target.Piece.MovePiece(square, false);
                }
                break;
        }

    }
}