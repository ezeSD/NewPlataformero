using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnemyArmor : Weapon
{
     Transform shootDirectionTransform;

    public float shootInterval = 2.0f;
    private float lastShootTime = 0f;


    private void Start()
    {
        shootDirectionTransform = GameObject.FindGameObjectWithTag("PostShootFly").transform;
        Factory = FindObjectOfType<BulletFlyFactory>();
    }

    public override void Shoot()
    {
        var b = Factory.Create();
        b.transform.position = shootDirectionTransform.position;
        Vector3 DownDirection = -shootDirectionTransform.up;

        b.transform.position += DownDirection * 0.5f;
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
