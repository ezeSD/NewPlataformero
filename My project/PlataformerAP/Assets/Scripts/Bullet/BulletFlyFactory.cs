
using UnityEngine;


public class BulletFlyFactory : Factory<Bullet>
{
    public Bullet prefab;

    ObjectPool<Bullet> _pool;
    int bulletAmount = 20;


    private void Awake()
    {
        _pool = new ObjectPool<Bullet>(CreatePrefab, Desactive, Active, bulletAmount);
    }
    public override Bullet Create()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 targetPos = player != null ? player.transform.position : Vector3.zero;

        var x = _pool.Get();
        x.Initialize(this, targetPos);

        return x;
    }

    public override void Return(Bullet bullet)
    {
        _pool.Return(bullet);
    }

    Bullet CreatePrefab()
    {
        return Instantiate(prefab);
    }

    void Active(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    void Desactive(Bullet bullet)
    {
        bullet.Refresh();
        bullet.gameObject.SetActive(false);
    }
}
