using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArmor : Weapon
{
    Transform shootDirectionTransform;

    public float shootInterval = 2.0f;
    private float lastShootTime = 0f;

    private void Start()
    {
        shootDirectionTransform = GameObject.FindGameObjectWithTag("PostShootEnemy").transform;
        Factory = FindObjectOfType<BulletFactory>();
    }

    public override void Shoot()
    {
        var b = Factory.Create();
        b.transform.position = shootDirectionTransform.position;
        b.transform.forward = shootDirectionTransform.forward;
    }

    public void ColdownShoot()
    {
        if (Time.time - lastShootTime >= shootInterval)
        {
            Shoot();

            lastShootTime = Time.time;
        }
    }


}
