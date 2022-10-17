using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public struct Level
    {
        public Transform ground;
        public List<GameObject> obstacleList;
    }
    public List<Level> levelList;

    public int levelPassed; // use this to calculate point and maybe spawn obstacle
    public int nextLevelIndex;
    public int lastLevelIndex;
    public static LevelManager instance;
    public float boundaryValue;
    public float speed;
    public int levelToSpawnObstacle;

    [Header("Offsets values")]
    [SerializeField] private float _offsetZWorld;
    [SerializeField] private float _offsetX;
    [SerializeField] private float _offsetZ;
    [SerializeField] private float _offsetFromPlate;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        boundaryValue = levelList[0].ground.GetComponent<Renderer>().bounds.size.x / 2;
        var mesh = levelList[0].ground.GetComponent<MeshFilter>().mesh;
        _offsetZWorld = levelList[0].ground.GetComponent<Renderer>().bounds.size.z;

        _offsetX = mesh.bounds.size.x;
        _offsetZ = mesh.bounds.size.z;
        _offsetFromPlate = _offsetZ / 5f;
        nextLevelIndex = 0;
        lastLevelIndex = levelList.Count - 1;
    }
    private void Start()
    {
        SetupSpawnerReference();
    }
    private void FixedUpdate()
    {
        foreach (Level level in levelList)
        {
            level.ground.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
        }
    }
    public void SetupSpawnerReference()
    {
        for (int i = 0; i < levelList.Count - 1; i++) // ... WTF
        {
            levelList[i].ground.GetComponentInChildren<PlateController>().nextEnemySpannwer = levelList[i + 1].ground.GetComponentInChildren<EnemySpawnerController>();
            levelList[i + 1].ground.GetComponentInChildren<PlateController>().previousEnemySpannwer = levelList[i].ground.GetComponentInChildren<EnemySpawnerController>();
        }

        levelList[levelList.Count - 1].ground.GetComponentInChildren<PlateController>().nextEnemySpannwer = levelList[0].ground.GetComponentInChildren<EnemySpawnerController>();
        levelList[0].ground.GetComponentInChildren<PlateController>().previousEnemySpannwer = levelList[levelList.Count - 1].ground.GetComponentInChildren<EnemySpawnerController>();

    }
    public void GenerateLevel()
    {
        if (levelPassed >= 1)
        {
            DespawnObstacle(levelList[nextLevelIndex]);
            levelList[nextLevelIndex].ground.position = new Vector3(0, 0, _offsetZWorld + levelList[lastLevelIndex].ground.position.z);
            foreach (MultiplyPlate mp in levelList[nextLevelIndex].ground.GetComponentsInChildren<MultiplyPlate>())
            {
                mp.RollMultiplyPlate();
            }
            // calculate next level to generate
            lastLevelIndex = nextLevelIndex;
            nextLevelIndex++;
            if (nextLevelIndex > levelList.Count - 1)
                nextLevelIndex = 0;
            if (levelPassed >= levelToSpawnObstacle)
            {
                if (nextLevelIndex + 1 > levelList.Count - 1)
                {
                    // Debug.Log("spawn obstacle in level index: " + 0);
                    SpawnObstacles(levelList[0], amount: 2, true);
                    SpawnObstacles(levelList[0], amount: 2, false);
                }
                else
                {
                    // Debug.Log("spawn obstacle in level index: " + (nextLevelToGenerate + 1));
                    SpawnObstacles(levelList[nextLevelIndex + 1], 2, true);
                    SpawnObstacles(levelList[nextLevelIndex + 1], 2, false);
                }
            }
        }
    }

    public void CheckBoundary(Transform tObject)
    {
        Vector3 current_pos = tObject.position;
        if (tObject.position.x <= -boundaryValue)
        {
            current_pos.x = -boundaryValue;
            tObject.position = current_pos;
        }
        if (tObject.position.x >= boundaryValue)
        {
            current_pos.x = boundaryValue;
            tObject.position = current_pos;
        }
    }
    public Transform GetCurrentLevel()
    {
        return levelList[nextLevelIndex].ground;
    }
    public void SpawnObstacles(Level level, int amount, bool beforePlate)
    {
        List<Vector3> spawnPosList = new List<Vector3>();
        float minOffsetZ, maxOffsetZ;
        if (beforePlate)
        {
            minOffsetZ = (-_offsetZ / 2) - _offsetFromPlate;
            maxOffsetZ = 0;
        }
        else
        {
            minOffsetZ = 0;
            maxOffsetZ = (_offsetZ / 2) + _offsetFromPlate;
        }
        for (int i = 0; i < amount; i++)
        {
            GameObject obstacle = ObjectPooler.instance.GetPooledObject("Obstacle");
            if (obstacle)
            {
                Vector3 spawnPos;
                spawnPos = GetObstacleSpawnPosition(_offsetX / 2, minOffsetZ, maxOffsetZ);
                while (spawnPosList.Contains(spawnPos))
                {
                    spawnPos = GetObstacleSpawnPosition(_offsetX / 2, minOffsetZ, maxOffsetZ);
                }
                // base scale and position on ground
                // Vector3 scale = obstacle.transform.localScale;
                spawnPos.y = obstacle.transform.localPosition.y;

                obstacle.transform.parent = level.ground;
                // obstacle.transform.localScale = scale;
                obstacle.transform.localPosition = spawnPos; // using global position because Renderer.bound.size get the global size instead of local
                obstacle.SetActive(true);
                level.obstacleList.Add(obstacle);
            }
            else return;
        }

    }
    public void DespawnObstacle(Level level)
    {
        foreach (var obstacle in level.obstacleList)
        {
            float yPos = obstacle.transform.localPosition.y;
            obstacle.transform.parent = ObjectPooler.instance.transform;
            obstacle.transform.localPosition = new Vector3(0, yPos, 0);
            obstacle.SetActive(false);

        }
    }
    public Vector3 GetObstacleSpawnPosition(float offsetX, float minOffsetZ, float maxOffsetZ)
    {
        return new Vector3(Random.Range(-offsetX, offsetX), 0, Random.Range(minOffsetZ, maxOffsetZ));
    }
}
