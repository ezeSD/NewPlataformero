using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public Factory<Bullet> Factory;
    public abstract void Shoot();
}
