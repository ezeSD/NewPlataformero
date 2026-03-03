using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBullet : Bullet
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

        if (_counter >= 7f)
        {
            _factory.Return(this);
        }
    }


    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.GetComponent<IDamageable>() != null)
        {
            other.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
            _factory.Return(this);
        }
    }



}
