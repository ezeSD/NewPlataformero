using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyBullet : Bullet
{
    float _counter;
    void Update()
    {
        BulletDirection();
    }

    public override void Refresh()
    {
        _counter = 0;
    }

    void BulletDirection()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;

        transform.position += direction * _speed * Time.deltaTime;

        _counter += Time.deltaTime;

        if (_counter >= 3f)
        {
            _factory.Return(this);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        IDamageable takeDamage = other.gameObject.GetComponent<IDamageable>();
        if (takeDamage != null)
        {
            _factory.Return(this);
            takeDamage.TakeDamage(damage);
        }
    }

}
