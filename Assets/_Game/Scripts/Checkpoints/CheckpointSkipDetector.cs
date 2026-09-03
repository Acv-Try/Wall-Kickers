using UnityEngine;

public class CheckpointSkipDetector
{
    public void Evaluate(Level previous, Level current, Vector3 playerPosition)
    {
        if (previous == null || current == null) return;
        if (playerPosition.y <= previous.TopMarker.position.y) return;
        if (!current.IsFirstWallOccupied) return;
        if (!previous.IsCheckpointSkipped) return;

        CheckpointManager.Instance.CompleteSkippedCheckpoint(previous.LevelCheckpoint);
        current.ClearFirstWallSignal();
    }
}
