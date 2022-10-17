using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility;
public class Bomb : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _target;
    public float time;
    public float speed;
    public int damage;
    float startTime;
    Vector3 centerPoint, startRelCenter, endRelCenter; // start and end point relative center
    public Transform eplodeEffect;
    [SerializeField] private List<EnemyEntity> enemyEntities;
    ParticleSystemRenderer psr;
    TrailRenderer tr;
    Color color;
    private void Awake()
    {
        enemyEntities = new List<EnemyEntity>();
        psr = GetComponent<ParticleSystemRenderer>();
        tr = GetComponentInChildren<TrailRenderer>();
    }
    private void Update()
    {
        if (Vector3.Distance(transform.position, _target.position) <= 1 || _target.gameObject.activeSelf == false)
        {
            Explode();
        }
        GetCenter(Vector3.up);
        float fracComplete = (Time.time - startTime) / time * speed;
        transform.position = Vector3.Slerp(startRelCenter, endRelCenter, fracComplete * speed);
        transform.position += centerPoint;
    }
    public void SetUp(Transform target, Transform firepoint, int damage, Color color)
    {
        _target = target;
        _firePoint = firepoint;
        this.damage = damage;
        this.color = color;

        // bomb color
        psr.material.color = color;
        psr.material.EnableKeyword("_EMISSION");
        psr.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
        psr.material.SetColor("_EmissionColor", color * 1.4f); // hdr intensity should be 1.5f or 1.43...

        // trail color
        // tr.material.EnableKeyword("_EMISSION");
        tr.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
        tr.material.SetColor("_Color1", color * 2.347384f);

        startTime = Time.time;
        AudioManager.instance.PlaySound(AudioManager.Sound.BombSizzle);
    }

    public void GetCenter(Vector3 direction)
    {
        centerPoint = (_firePoint.position + _target.position) * .5f;
        centerPoint -= direction;
        startRelCenter = _firePoint.position - centerPoint;
        endRelCenter = _target.position - centerPoint;
    }
    private void Explode()
    {
        // Deal damage
        foreach (EnemyEntity enemyEntity in enemyEntities)
        {
            if (enemyEntity.gameObject.activeSelf)
            {
                Color color = enemyEntity.tiers[enemyEntity.GetTier()].color;
                enemyEntity.TakeDamage(damage);
                if (enemyEntity.powerLevel <= 0)
                {
                    enemyEntity.Kill(color);
    
                }
                // else enemyEntity.ChangeAppearance();
            }
        }

        // spawn Explosion VFX
        GameObject explodeVFX = ObjectPooler.instance.GetPooledObject("ExplosionVFX");
        if (explodeVFX != null)
        {
            explodeVFX.transform.parent = null;
            explodeVFX.transform.position = transform.position;
            explodeVFX.GetComponent<ExplodeVFXController>().SetUp(color);

            explodeVFX.SetActive(true);
            AudioManager.instance.PlaySound(AudioManager.Sound.BombExplode, transform.position);
        }


        // disable the bomb
        transform.parent = ObjectPooler.instance.transform;
        enemyEntities.Clear();
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyEntity"))
        {
            enemyEntities.Add(other.GetComponent<EnemyEntity>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyEntity"))
        {
            EnemyEntity ee = other.GetComponent<EnemyEntity>();
            if (enemyEntities.Contains(ee))
            {
                enemyEntities.Remove(ee);
            }
        }
    }
}
