using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility;
public class EnemyEntity : Entity
{
    // Start is called before the first frame update
    public Transform target;
    public float speed;
    private Animator animator;
    protected void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        if (target != null && target.gameObject.activeSelf)
        {
            Vector3 pos = target.position;
            pos.y = 0;
            transform.LookAt(pos);
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }
  
    public override void Kill()
    {
        if (target != null)
            target = null;
        GameManager.instance.AddScore(10);
        ParticleExplode();
        base.Kill();
    }
    public override void Kill(Color color) 
    {
        if (target != null)
            target = null;
        GameManager.instance.AddScore(10);
        base.Kill(color);
    }
    private void OnEnable()
    {
        target = null;
        animator.SetBool("Running", false);

    }
    public void SetTarget()
    {
        if (PlayerController.instance.TotalPowerLevel >= 1)
        {
            List<EntitySpawnPosition> playerEntityPos = GetSpawnPositionWithEntity(PlayerController.instance.entitySpawnPositions);
            target = playerEntityPos[Random.Range(0, playerEntityPos.Count - 1)].entity;
            animator.SetBool("Running", true);
        }
    }
    public override void ChangeAppearance()
    {
        base.ChangeAppearance();
        Renderer.material.EnableKeyword("_EMISSION");
        Renderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
        Renderer.material.SetColor("_EmissionColor", Renderer.material.color * .5f);
    }
}