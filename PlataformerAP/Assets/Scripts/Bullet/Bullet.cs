using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [SerializeField] protected float _speed;
    public int damage;
    protected Factory<Bullet> _factory;

    protected Vector3 targetPosition;

    public virtual void Initialize(Factory<Bullet> Factory, Vector3 targetPos)
    {
        _factory = Factory;
        targetPosition = targetPos;
    }

    public virtual void Refresh() { }

}
