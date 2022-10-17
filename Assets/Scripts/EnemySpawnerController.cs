using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility;
using System.Linq;
public class EnemySpawnerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float offsetZ, offsetX;
    public List<MultiplyPlate> multiplyPlate;
    public int maxPowerLevel = 0;
    List<Vector3> _enemyPos; // enemy positions
    public bool render;
    public List<EnemyEntity> enemyEntityList;
    void Awake()
    {
        _enemyPos = new List<Vector3>();
        offsetX = GetComponent<Renderer>().bounds.size.x / 2;
        offsetZ = GetComponent<Renderer>().bounds.size.z / 2;
        multiplyPlate = transform.parent.GetComponentsInChildren<MultiplyPlate>().ToList<MultiplyPlate>();
        GetComponent<Renderer>().enabled = render;
        enemyEntityList = new List<EnemyEntity>();

    }
    public void CalculatePowerLevel()
    {
        maxPowerLevel = 0;
        foreach (var plate in multiplyPlate)
        {
            int pl = plate.GetComponent<MultiplyPlate>().CalculatePowerLevel();
            if (pl > maxPowerLevel)
                maxPowerLevel = pl;
        }
    }
    public void SpawnEnemyEntity()
    {
        CalculatePowerLevel();
        if (maxPowerLevel <= PlayerController.instance.maxUnit)
        {
            for (int i = 0; i < maxPowerLevel; i++)
            {
                // Debug.Log("spawn enemy dummy");
                GameObject enemyEntity = ObjectPooler.instance.GetPooledObject("EnemyEntity");
                if (enemyEntity != null)
                {
                    Vector3 randomPos = transform.position + new Vector3(Random.Range(-offsetX, offsetX), 0, Random.Range(-offsetZ, offsetZ));
                    while (_enemyPos.Contains(randomPos))
                    {
                        randomPos = new Vector3(Random.Range(-offsetX, offsetX), 0, Random.Range(-offsetZ, offsetZ));
                    }
                    _enemyPos.Add(randomPos);
                    
                    enemyEntity.transform.position = randomPos;
                    enemyEntity.GetComponent<EnemyEntity>().ChangeAppearance();
                    enemyEntity.transform.parent = transform;
                    enemyEntity.SetActive(true);
                    enemyEntityList.Add(enemyEntity.GetComponent<EnemyEntity>());
                }
            }
        }
        else
        {
            for (int i = 0; i < PlayerController.instance.maxUnit; i++)
            {
                Vector3 randomPos = transform.position + new Vector3(Random.Range(-offsetX, offsetX), 0, Random.Range(-offsetZ, offsetZ));
                while (_enemyPos.Contains(randomPos))
                {
                    randomPos = new Vector3(Random.Range(-offsetX, offsetX), 0, Random.Range(-offsetZ, offsetZ));
                }
                GameObject enemyEntity = ObjectPooler.instance.GetPooledObject("EnemyEntity");
                if (enemyEntity != null)
                {
                    enemyEntity.transform.position = randomPos;
                    enemyEntity.SetActive(true);
                    enemyEntityList.Add(enemyEntity.GetComponent<EnemyEntity>());
                }
            }
            DistributePowerLevel(); 
        }
    }
    public void DespawnEnemyEntity()
    {
        foreach (var entity in enemyEntityList)
        {
            ObjectPooler.instance.DeactivatePooledObject(entity.gameObject);
        }
        enemyEntityList.Clear();
        _enemyPos.Clear();
    }
    // distribute power level is used when the amount of enemy enity to spawn is equal to player entity maximum unit count (default is 16)
    public void DistributePowerLevel()
    {
        int[] pArr = PartitionPowerLevel(maxPowerLevel, PlayerController.instance.maxUnit);
        int index = 0;

        foreach (var enemyEntity in enemyEntityList)
        {
            enemyEntity.powerLevel = pArr[index];
            enemyEntity.GetComponent<EnemyEntity>().ChangeAppearance();
            enemyEntity.transform.parent = transform;
            index++;
        }
    }
    public void SetTarget()
    {
        foreach (var entity in enemyEntityList)
        {
            entity.SetTarget();
        }
    }
}
