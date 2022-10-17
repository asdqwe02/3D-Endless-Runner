using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Utility;
using TMPro;
public class RotateEventArgs : EventArgs
{
    public float angle { get; set; }
    public RotateEventArgs() { }
    public RotateEventArgs(float angle)
    {
        this.angle = angle;
        // this.rotatedSideway = rotatedSideway;
    }
}
public class PlayerController : MonoBehaviour
{
    public float sidewaySpeed;
    public float rotationAngle;
    private bool entityRotatedSideway;
    public static PlayerController instance;
    public event EventHandler<RotateEventArgs> MovedSideway;
    public event EventHandler<float> SkillButtonPressed;
    public event EventHandler<int> PlayerPowerLevelChanged;
    private Vector3 _sidewayDirection;
    public Transform entityPrefab;
    public Transform formation;
    public List<EntitySpawnPosition> entitySpawnPositions;
    private int _totalPowerLevel;
    public int maxUnit;
    private TextMeshPro powerLevelText;
    public List<PlayerEntity> flyingEntity;
    public float laserSkillTime, shieldSkillTime, skillCDTime, skillCDTimeTotal;
    public bool skillCD;
    public Action skill;
    public enum TurningState
    {
        forward,
        left,
        right,
    }
    [SerializeField] private TurningState _turnState; // might become useless

    public int TotalPowerLevel
    {
        get => _totalPowerLevel; 
        set 
        {
            _totalPowerLevel = value;
            OnPlayerPowerLevelChange(_totalPowerLevel);
        } 
    }

    private void Awake()
    {
        SetUpLaserSkill();
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        TotalPowerLevel = 1;
        _turnState = TurningState.forward;
        entityRotatedSideway = false;
        entitySpawnPositions = new List<EntitySpawnPosition>();
        flyingEntity = new List<PlayerEntity>();
        powerLevelText = GetComponentInChildren<TextMeshPro>();
    }
    private void Start()
    {
        UpdatePowerLevel();
        foreach (var pos in formation.GetComponent<PlayerEntityFomrationSetup>().formation)
            entitySpawnPositions.Add(pos);
        maxUnit = GetComponentInChildren<PlayerEntityFomrationSetup>().FormationUnitCount();
    }
    private void Update()
    {
        ProcessInput();
        LevelManager.instance.CheckBoundary(transform);
    }
    private void FixedUpdate()
    {
        // transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime,Space.World);
        MoveEntitySideWay();

    }
    private void ProcessInput()
    {
        // BAD
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && _turnState != TurningState.right)
        {
            _turnState = TurningState.left;
            _sidewayDirection = Vector3.left;
        }
        else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && _turnState != TurningState.left)
        {
            _turnState = TurningState.right;
            _sidewayDirection = Vector3.right;

        }
        else if (_turnState != TurningState.forward) //reset rotation
        {
            entityRotatedSideway = false;
            _turnState = TurningState.forward;
            _sidewayDirection = Vector3.zero;
            OnMovingSideway(new RotateEventArgs(0f));

        }

        if (Input.GetKeyDown(KeyCode.Space) && !skillCD && !GameManager.instance.pause)
        {
            skill();
            skillCD = true;

        }
    }
    public void MoveEntitySideWay()
    {
        transform.Translate(_sidewayDirection * sidewaySpeed * Time.deltaTime, Space.World);


        if (_turnState != TurningState.forward && entityRotatedSideway == false)
        {
            OnMovingSideway(new RotateEventArgs(_sidewayDirection.x * rotationAngle));
            entityRotatedSideway = true;
        }
    }
    protected virtual void OnMovingSideway(RotateEventArgs e)
    {
        // Debug.Log("entity turning event called");
        MovedSideway?.Invoke(this, e);
    }
    protected virtual void OnSkillPress(float e)
    {
        SkillButtonPressed?.Invoke(this, e);
    }
    protected virtual void OnPlayerPowerLevelChange(int e)
    {
        PlayerPowerLevelChanged?.Invoke(this, e);
    }
    public EntitySpawnPosition GetEntitySpawnPosition()
    {
        foreach (var pos in entitySpawnPositions)
            if (pos.entity == null)
                return pos;
        return null;
    }
    public EntitySpawnPosition GetEntityPosition(Transform entity)
    {
        foreach (var pos in entitySpawnPositions)
            if (pos.entity == entity)
                return pos;
        return null;
    }
    public void UpdatePowerLevel()
    {
        powerLevelText.text = TotalPowerLevel.ToString();

        // debugging
        // int countpl = 0;
        // foreach (var pe in ObjectPooler.instance.GetActivePoolObjects("PlayerEntity"))
        // {
        //     countpl += pe.GetComponent<PlayerEntity>().powerLevel;
        // }
        // Debug.Log("total power level from player entity: " + countpl);
        // Debug.Log("total power level from player controller: " + PlayerController.instance.totalPowerLevel);

    }
    public void RemoveEntityFromFormation(Transform entity)
    {
        EntitySpawnPosition esp = GetEntityPosition(entity);
        if (esp != null)
        {
            esp.entity = null;
        }
    }
    public void ResetAllEntityPowerLevel()
    {
        foreach (var pos in entitySpawnPositions)
        {
            if (pos.entity != null)
            {
                PlayerEntity entity = pos.entity.GetComponent<PlayerEntity>();
                entity.powerLevel = 1;
                entity.ChangeAppearance();
            }
        }
    }
    public void RemoveAllEntity()
    {
        ResetAllEntityPowerLevel();
        foreach (var pos in entitySpawnPositions)
            if (pos.entity != null)
            {
                pos.entity = null;
            }
        ObjectPooler.instance.RemoveAllObjectWithTag("PlayerEntity");
    }

    public IEnumerator LaserSkill(int amount) // basic function to test thing out
    {
        if (flyingEntity.Count == 0)
        {
            // abomination code
            List<EntitySpawnPosition> entityPos = GetRandomItemsFromList(GetSpawnPositionWithEntity(entitySpawnPositions), amount);
            List<LaserController> entityLaser = new List<LaserController>();
            foreach (var pos in entityPos)
            {
                PlayerEntity entity = pos.entity.GetComponent<PlayerEntity>();
                LaserController laser = ObjectPooler.instance.GetPooledObject("Laser").GetComponent<LaserController>();
                entity.FlyUp();
                if (laser != null)
                {
                    laser.SetUp(entity, laserSkillTime, entity.powerLevel);
                    laser.gameObject.SetActive(true);
                    entityLaser.Add(laser);
                }
                flyingEntity.Add(pos.entity.GetComponent<PlayerEntity>());
            }
            GameObject laserSound = AudioManager.instance.PlaySound(AudioManager.Sound.LaserBeam);
            yield return new WaitForSeconds(laserSkillTime);
            StartCoroutine(AudioManager.instance.DisableSoundObject(laserSound, 0f));
            // return flying entity to normal running state
            foreach (var entity in flyingEntity)
            {
                entity.BackToGround();
            }
            // disable laser
            foreach (var laser in entityLaser)
            {
                laser.transform.parent = ObjectPooler.instance.transform;
                laser.gameObject.SetActive(false);
            }
            flyingEntity.Clear(); // clear the list
            StartCoroutine(PlayerSkillCD(skillCDTime, () => skillCD = false)); // start cooldown
        }
    }
    public IEnumerator ShieldSkill()
    {
        // apply shield effect
        List<PlayerEntity> peList = GetPlayerEntityInSpawnPosition(entitySpawnPositions);
        foreach (var entity in peList)
        {
            entity.SetUpShield();
        }
        AudioManager.instance.PlaySound(AudioManager.Sound.Shield);
        yield return new WaitForSeconds(shieldSkillTime);
        // remove shield effect
        foreach (var entity in peList)
        {
            entity.DisableShield();
        }
        StartCoroutine(PlayerSkillCD(skillCDTime, () => skillCD = false)); // start cooldown

    }
    public void BombSkill(int amount)
    {
        List<PlayerEntity> playerEntityList = GetRandomItemsFromList<PlayerEntity>(GetPlayerEntityInSpawnPosition(entitySpawnPositions), amount);
        Transform currentLevel = LevelManager.instance.GetCurrentLevel();
        List<EnemyEntity> targetedEnemy = new List<EnemyEntity>(
            GetChildGameObjectWithScript<EnemyEntity>(currentLevel.GetComponentInChildren<EnemySpawnerController>().transform)); // WTF... AGAIN
        // Debug.Log(targetedEnemy);

        foreach (var entity in playerEntityList)
        {
            if (targetedEnemy.Count >= 1)
            {
                int index = UnityEngine.Random.Range(0, targetedEnemy.Count);
                entity.ThrowBomb(targetedEnemy[index].transform);
                targetedEnemy.RemoveAt(index);
            }
        }

        StartCoroutine(PlayerSkillCD(skillCDTime, () => skillCD = false)); // start cooldown
    }
    public void SetUpShieldSkill()
    {
        skillCDTimeTotal = skillCDTime + shieldSkillTime;
        skill = () =>
        {
            OnSkillPress(skillCDTimeTotal);
            StartCoroutine(ShieldSkill());
        };
    }
    public void SetUpLaserSkill()
    {
        skillCDTimeTotal = skillCDTime + laserSkillTime;
        skill = () =>
        {
            OnSkillPress(skillCDTimeTotal);
            StartCoroutine(LaserSkill(amount: 3)); // litearlly magic number
        };
    }
    public void SetUpBombSkill()
    {
        skillCDTimeTotal = skillCDTime;
        skill = () =>
        {
            OnSkillPress(skillCDTimeTotal);
            BombSkill(4);
        };
    }
    public void TriggerSkill()
    {
        if (skill != null && !skillCD)
        {
            skill();
            skillCD = true;
        }
    }

}