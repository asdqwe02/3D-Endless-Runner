using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using static Utility;
using TMPro;
public class MultiplyPlate : MonoBehaviour
{
    public int value;
    public TextMeshPro textMesh;
    public enum ExpressionType
    {
        PLUS,
        MULTIPLY,
        SUBTRACT,
        DIVIDE,
        EQUAL,
        SQRT,
    }
    public ExpressionType expresionType;
    Expression expression;
    public bool trigger;
    void Start()
    {
        textMesh = GetComponentInChildren<TextMeshPro>();
        RollMultiplyPlate();
        trigger = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            trigger = true;
        }
    }
    public void RollMultiplyPlate()
    {
        value = Random.Range(2, 6);
        expression = (Expression)expressionWeightPool.RollPoolObjects().Clone();
        expression.ChangeValue(value);
        textMesh.text = expression.ToString();

    }
    public int CalculatePowerLevel()
    {
        int current_pl = PlayerController.instance.TotalPowerLevel;
        return expression.Perform(current_pl); // -> if get 0 mean bug

    }
    public int SpawnPlayerEntity(int amount)
    {
        // int currentPowerLevel = PlayerController.instance.totalPowerLevel;
        int EntitySpawned = 0;
        for (int i = 0; i < amount; i++)
        {
            GameObject playerEntity = ObjectPooler.instance.GetPooledObject("PlayerEntity");
            EntitySpawnPosition entitySpawnPos = PlayerController.instance.GetEntitySpawnPosition();
            if (playerEntity != null && entitySpawnPos != null)
            {
                Vector3 pos = entitySpawnPos.position;
                pos.y = playerEntity.transform.localPosition.y;

                playerEntity.transform.parent = PlayerController.instance.formation;
                playerEntity.transform.localPosition = pos; // keep local pos y 

                entitySpawnPos.entity = playerEntity.transform;
                playerEntity.SetActive(true);
                playerEntity.GetComponent<PlayerEntity>().ChangeAppearance();
                EntitySpawned++;
            }
            else return EntitySpawned;
        }
        return EntitySpawned;
    }
    public void Activate()
    {
        int calculatedPowerLevel = CalculatePowerLevel();
        int EntitySpawned = 0;
        if (PlayerController.instance.TotalPowerLevel < calculatedPowerLevel)
        {
            EntitySpawned = SpawnPlayerEntity(calculatedPowerLevel - PlayerController.instance.TotalPowerLevel);
        }
        CalculatePowerLevelForEntity(calculatedPowerLevel, PlayerController.instance.TotalPowerLevel, EntitySpawned);

        PlayerController.instance.TotalPowerLevel = calculatedPowerLevel;
        PlayerController.instance.UpdatePowerLevel();
    }
    private void CalculatePowerLevelForEntity(int calculatedPowerLevel, int currentPowerLevel, int newSpawn) // extremely bad practice here
    {
        List<GameObject> PlayerEntity = ObjectPooler.instance.GetActivePoolObjects("PlayerEntity");
        int activePlayerEntity = ObjectPooler.instance.ActivePooledObjectCount("PlayerEntity");
        int offset = Mathf.Abs(calculatedPowerLevel - currentPowerLevel - newSpawn);
        int[] powerLevelArr = PartitionPowerLevel(offset, activePlayerEntity);
        int index = 0;
        // increase
        if (calculatedPowerLevel > currentPowerLevel)
        {
            if (offset > activePlayerEntity)
            {
                foreach (var entity in PlayerEntity) // get position with player entity
                {
                    PlayerEntity playerEntity = entity.GetComponent<PlayerEntity>();
                    playerEntity.powerLevel += powerLevelArr[index];
                    playerEntity.ChangeAppearance();
                    index++;
                }
            }
            else if (offset <= activePlayerEntity && activePlayerEntity == PlayerController.instance.maxUnit)
            {
                PlayerEntity playerEntity = PlayerEntity[Random.Range(0, PlayerEntity.Count)].GetComponent<PlayerEntity>();
                playerEntity.powerLevel += (offset);
                playerEntity.ChangeAppearance();
            }
            // grouping
            if (activePlayerEntity >= 10)
            {
                List<PlayerEntity> groupList =
                    Utility.GetRandomItemsFromList<PlayerEntity>(
                        Utility.ConvertGameObjsToSCript<PlayerEntity>(PlayerEntity),
                        Random.Range(3, 6)); //... wtf
                bool grouped = GroupPlayerEntity(groupList);
                // Debug.Log("group player entity: " + grouped); // debug
            }
        }
        // decrease
        if (calculatedPowerLevel < currentPowerLevel)
        {
            if (calculatedPowerLevel < PlayerController.instance.maxUnit) // split the player entity
            {
                PlayerController.instance.RemoveAllEntity();
                SpawnPlayerEntity(calculatedPowerLevel);
                return;
            }
            if (offset > activePlayerEntity)
            {

                List<PlayerEntity> killList = new List<PlayerEntity>();
                foreach (var entity in PlayerEntity)
                {
                    PlayerEntity pe = entity.GetComponent<PlayerEntity>();
                    pe.powerLevel -= powerLevelArr[index];
                    pe.ChangeAppearance(); // NOTE: move this somewhere else
                    index++;
                    if (pe.powerLevel <= 0)
                    {
                        killList.Add(pe);
                        int powerLevelDecrease = Mathf.Abs(pe.powerLevel);
                        if (index != powerLevelArr.Length)
                        {
                            powerLevelArr[index] += powerLevelDecrease;
                        }
                        else
                        {
                            while (powerLevelDecrease > 0)
                            {
                                // Debug.Log("loop offset > active");
                                PlayerEntity rdEntity = PlayerEntity[Random.Range(0, PlayerEntity.Count)].GetComponent<PlayerEntity>();
                                if (killList.Contains(rdEntity))
                                    continue;
                                rdEntity.powerLevel -= powerLevelDecrease;
                                rdEntity.ChangeAppearance();// NOTE: move this somewhere else
                                if (rdEntity.powerLevel <= 0)
                                {
                                    powerLevelDecrease = Mathf.Abs(rdEntity.powerLevel);
                                    killList.Add(rdEntity);
                                }
                                else break;
                            }

                        }
                    }
                }
                if (killList.Count > 0)
                {
                    foreach (var entity in killList)
                        entity.Kill();
                }
            }
            if (offset < activePlayerEntity && calculatedPowerLevel >= PlayerController.instance.maxUnit)
            {
                int powerLevelDecrease = offset;
                List<PlayerEntity> killList = new List<PlayerEntity>();

                /* 3 outcomes: 
                * power level - pdl > 0 -> break
                * power level - pdl < 0 -> pld = Mathf.Abs(power level - pdl)
                * power level - pdl = 0 -> break
                *
                */
                while (powerLevelDecrease > 0)
                {
                    // Debug.Log("loop offset < active");
                    PlayerEntity randomEntity = PlayerEntity[Random.Range(0, PlayerEntity.Count)].GetComponent<PlayerEntity>();
                    if (killList.Contains(randomEntity))
                        continue;
                    randomEntity.powerLevel -= powerLevelDecrease;
                    randomEntity.ChangeAppearance(); // NOTE: move this somewhere else
                    if (randomEntity.powerLevel <= 0)
                    {
                        powerLevelDecrease = Mathf.Abs(randomEntity.powerLevel);
                        killList.Add(randomEntity);
                    }
                    else break;
                }
                if (killList.Count > 0)
                {
                    foreach (var e in killList)
                        e.Kill();
                }
            }

        }
    }

    private bool GroupPlayerEntity(List<PlayerEntity> entities)
    {
        int sumPower = 0;
        int maxTier = 0;
        foreach (var entity in entities)
        {
            sumPower += entity.powerLevel;
            if (entity.GetTier() > maxTier)
                maxTier = entity.GetTier();
        }
        // PlayerEntity temp = ObjectPooler.instance.GetPooledObject("PlayerEntity").GetComponent<PlayerEntity>();
        PlayerEntity randomEntity = entities[Random.Range(0, entities.Count())];
        int tempPowerlevel = randomEntity.powerLevel;
        randomEntity.powerLevel = sumPower;
        if (randomEntity.GetTier() > maxTier)
        {
            // Debug.Log("sum power of 3 random entity: " + sumPower);
            randomEntity.ChangeAppearance();
            entities.Remove(randomEntity);
            foreach (var entity in entities)
            {
                entity.powerLevel = 1;
                PlayerController.instance.RemoveEntityFromFormation(entity.transform);
                ObjectPooler.instance.DeactivatePooledObject(entity.gameObject);

            }
            return true;
        }
        else randomEntity.powerLevel = tempPowerlevel; //return to the old power level
        return false;

    }
}
