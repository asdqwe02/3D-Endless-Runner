using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntitySpawnPosition
{
    public Vector3 position;
    public Transform entity; // reference to the occupied entity
    public EntitySpawnPosition() { }
    public EntitySpawnPosition(Vector3 Pos, Transform Entity)
    {
        position = Pos;
        entity = Entity;
    }

}
public class PlayerEntityFomrationSetup : MonoBehaviour
{
    // Start is called before the first frame update
    public int column, row;
    public float offsetX, offsetZ;
    public Transform EntitySpawnPositionPrefab;
    Transform[,] _debugFormation;
    public EntitySpawnPosition[,] formation;
    public bool spawnDebuggingBalls;

    private void Awake()
    {
        _debugFormation = new Transform[row, column];
        formation = new EntitySpawnPosition[row, column];
        // Instantiate(PlayerEntityPrefab,formation[0,column/2]);


        float start_pos;
        if (column % 2 == 0)
            start_pos = -(column / 2) * offsetX + offsetX / 2;
        else start_pos = -(column / 2) * offsetX;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                formation[i, j] = new EntitySpawnPosition(new Vector3(start_pos + offsetX * j, 0, -offsetZ * i), null);
                // Debug
                // Instantiate Debug balls
                if (spawnDebuggingBalls)
                {
                    _debugFormation[i, j] = Instantiate(EntitySpawnPositionPrefab, transform);
                    _debugFormation[i, j].localPosition = formation[i, j].position;
                }

            }
        }
    }
    private void Start()
    {
        GameObject initialPlayerEnntity = ObjectPooler.instance.GetPooledObject("PlayerEntity");
        formation[0, column / 2].entity = initialPlayerEnntity.transform;

        Vector3 initial_entity_pos = formation[0, column / 2].position;
        initial_entity_pos.y = initialPlayerEnntity.transform.localPosition.y;

        initialPlayerEnntity.transform.parent = transform;
        initialPlayerEnntity.transform.localPosition = initial_entity_pos;
        initialPlayerEnntity.SetActive(true);

    }
    public int FormationUnitCount()
    {
        return row * column;
    }
}
