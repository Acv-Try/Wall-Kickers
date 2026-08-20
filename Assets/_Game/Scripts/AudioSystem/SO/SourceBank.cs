using UnityEngine;

[CreateAssetMenu(fileName = "SourceBank", menuName = "Scriptable Objects/Audio/SourceBank")]
public class SourceBank : ScriptableObject
{
    public LibrarySource LibrarySource;
    public SoundData GetData(EType_SourceDataType type)
    {
        if (LibrarySource == null)
        {
            Debug.LogWarning($"No source registered");
            return null;
        }
        return LibrarySource.Get(type);
    }
}
