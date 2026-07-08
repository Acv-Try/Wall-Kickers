using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Scriptable Objects/GameConfig")]
public class GameConfig : ScriptableObject
{
    public int progressSaveCheckpoint = 20;
    public int maxDeathsBeforeFullRestart = 4;
    public Vector3 cameraCenter;
}
