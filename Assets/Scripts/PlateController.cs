using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateController : MonoBehaviour
{
    public List<MultiplyPlate> multiplyPlate;
    public EnemySpawnerController nextEnemySpannwer, previousEnemySpannwer, currentEnenmySpawner;
    private void Awake()
    {
        currentEnenmySpawner = transform.root.GetComponentInChildren<EnemySpawnerController>();
    }
    private void Start()
    {
        foreach (MultiplyPlate obj in transform.parent.GetComponentsInChildren<MultiplyPlate>())
        {
            multiplyPlate.Add(obj);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (MultiplyPlate plate in multiplyPlate)
                if (plate.trigger)
                {
                    plate.Activate();
                    
                    LevelManager.instance.GenerateLevel();
                    LevelManager.instance.levelPassed++;

                    DeTriggerMultiplyPlate();
                    previousEnemySpannwer.DespawnEnemyEntity();
                    currentEnenmySpawner.SetTarget();
                    nextEnemySpannwer.SpawnEnemyEntity();
                    return;
                }
        }
    }
    private void DeTriggerMultiplyPlate()
    {
        foreach (MultiplyPlate plate in multiplyPlate)
            plate.trigger = false;
    }
}
