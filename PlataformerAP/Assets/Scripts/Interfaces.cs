using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount);
}

public interface Finish
{
    void ChangeScene(string scene);
}
public interface IEnemy
{
    void Initialize(Transform playerTarget);
    void ReturnToPool();
    void OnSpawn();
    void OnDespawn();
}


