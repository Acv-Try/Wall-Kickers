using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private BackgroundGenerator backgroundGenerator;

    public Level SpawnLevel(Level level, Vector3Int origin)
    {
        backgroundGenerator.Paint(level, origin);

        Vector3 worldOrigin = backgroundGenerator.CellToWorld(origin) + level.SpawnOffset;

        Level instance = Instantiate(
            level,
            worldOrigin,
            Quaternion.identity,
            transform
        );
        instance.Origin = origin;

        return instance;
    }

    public void RemoveLevel(Level level)
    {
        backgroundGenerator.Remove(level);
        Destroy(level.gameObject);
    }
}