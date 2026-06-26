using UnityEngine;

public class ObstaclePiece : MonoBehaviour
{
    [SerializeField] private PieceType type;

    [SerializeField] private Transform bottomAttach;
    [SerializeField] private Transform topAttach;

    public PieceType Type => type;

    public Transform BottomAttach => bottomAttach;
    public Transform TopAttach => topAttach;
}
public enum PieceType
{
    Bottom,
    Body,
    Top
}