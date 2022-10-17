using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility;

public class PlayerEntity : Entity
{
    [SerializeField] private float _offsetZ;
    public float catchUpSpeed;
    private Animator animator;
    [SerializeField] private Vector3 _default_pos;
    // public int currentTier; // for debugging only
    public Transform laserTarget;
    public Transform laserPoint;

    [Range(3.5f, 5.5f)]
    public float flyUpYPosition;
    [Range(-65f, -90f)]
    public float flyUpRotationX;
    public bool flying;
    [SerializeField] private int _shield;
    bool enableShield;

    private bool _hit;
    protected void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        _hit = false;

    }
    void Start()
    {
        PlayerController.instance.MovedSideway += RotateEntity;
        _default_pos = transform.localPosition;
        foreach (Transform child in transform)
        {
            laserTarget = child.Find("Target");
            if (laserTarget != null)
                break;
        }
    }
    private void OnEnable()
    {
        _default_pos = transform.localPosition;
        _hit = false;
        BackToGround();
    }
    void Update()
    {
        // currentTier = GetTier(); // debug only
        LevelManager.instance.CheckBoundary(transform);
    }
    private void OnCollisionEnter(Collision other)
    {
        if (!_hit)
        {
            if (other.gameObject.CompareTag("Obstacle") && !enableShield)
            {
                PlayerController.instance.TotalPowerLevel -= powerLevel;
                PlayerController.instance.UpdatePowerLevel();
                ParticleExplode();
                Kill();
                return;
            }
            if (other.gameObject.CompareTag("EnemyEntity"))
            {
                _hit = true;
                var enemyEntity = other.gameObject.GetComponent<EnemyEntity>();
                Color color = tiers[GetTier()].color;
                TakeDamage(enemyEntity.powerLevel);
                enemyEntity.Kill();
                if (powerLevel <= 0)
                {
                    ParticleExplode(color);
                    Kill();
                }
                else
                    ChangeAppearance();
                PlayerController.instance.UpdatePowerLevel();
            }
        }

    }
    private void OnCollisionExit(Collision other)
    {
        _hit = false;
    }
    private void RotateEntity(object sender, RotateEventArgs rotateInfo)
    {
        if (rotateInfo.angle == 0)
        {
            transform.rotation = Quaternion.identity;
            return;
        }
        transform.Rotate(new Vector3(0, rotateInfo.angle, 0), Space.World);
    }
    private void FixedUpdate()
    {
        if (transform.localPosition != _default_pos)
            CatchUp();
    }
    private void CatchUp()
    {
        Vector3 target_pos = _default_pos - transform.localPosition;
        transform.Translate(target_pos * catchUpSpeed * Time.deltaTime);
    }

    public override void Kill()
    {
        powerLevel = 1;
        ChangeAppearance();
        PlayerController.instance.RemoveEntityFromFormation(transform);
        ObjectPooler.instance.DeactivatePooledObject(gameObject);
    }
    public void FlyUp()
    {
        flying = true;
        flyUpYPosition = UnityEngine.Random.Range(3.5f, 5.5f); // arbitrary offset just to make thing look cool
        _default_pos.y = flyUpYPosition;
        animator.SetBool("IsFlying", true);
        animator.SetBool("Running", false);
    }
    public void BackToGround()
    {
        flying = false;
        // transform.rotation = Quaternion.identity;
        if (PlayerController.instance != null)
            _default_pos.y = -PlayerController.instance.transform.position.y;
        animator.SetBool("IsFlying", false);
        animator.SetBool("Running", true);
    }
    public void SetUpShield()
    {
        enableShield = true;
        _shield = (powerLevel * 25) / 100;
        Renderer.material.EnableKeyword("_EMISSION");
        Renderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
        Renderer.material.SetColor("_EmissionColor", Renderer.material.color * 1.5f);
    }
    public void DisableShield()
    {
        _shield = 0;
        enableShield = false;
        Renderer.material.DisableKeyword("_EMISSION");
        Renderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
    }
    public override void TakeDamage(int Damage)
    {
        int finalDamage = Damage;
        int oldPowerLevel = powerLevel; // save old player powerlevel
        finalDamage -= _shield;

        powerLevel -= finalDamage;
        _shield -= Damage;
        if (powerLevel <= 0)
            PlayerController.instance.TotalPowerLevel -= oldPowerLevel;
        else
        {
            PlayerController.instance.TotalPowerLevel -= finalDamage;
        }
        if (_shield <= 0 && enableShield)
        {
            // ParticleExplode();
            DisableShield();
        }
    }
    public void ThrowBomb(Transform target)
    {
        GameObject bomb = ObjectPooler.instance.GetPooledObject("Explosion");
        if (bomb != null)
        {
            bomb.GetComponent<Bomb>().SetUp(target, transform, powerLevel, tiers[GetTier()].color);
            bomb.SetActive(true);
        }
    }
}
