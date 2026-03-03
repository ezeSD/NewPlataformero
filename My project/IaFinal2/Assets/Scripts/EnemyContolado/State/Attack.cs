using UnityEngine;

public class Attack<T> : State<T>
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    Transform root;
    FSM<EnemyState> _fSM;

    [SerializeField] float shootCooldown = 1f;
    float maxAttackDistance = 6f;
    float timer;
    Transform target;

    // Referencia al script IAEnemy para obtener el target dinámicamente
    private IAEnemy enemyScript;

    public Attack(GameObject bullet, float Shoot, Transform rot, IAEnemy enemyRef)
    {
        bulletPrefab = bullet;
        shootCooldown = Shoot;
        root = rot;
        enemyScript = enemyRef;
    }

    public override void OnBegin()
    {
        Debug.Log("Attack state");
        timer = 0f;
    }

    public override void OnUpdate()
    {
        target = enemyScript.TargetPlayer;

        if (target == null)
        {
            _fSM.SendInput(EnemyState.Patrol);
            return;
        }

        float dist = Vector3.Distance(root.position, target.position);

        if (dist > maxAttackDistance)
        {
            _fSM.SendInput(EnemyState.OutRange);
            return;
        }

        timer += Time.deltaTime;

        if (timer >= shootCooldown)
        {
            Shoot();
            timer = 0f;
        }
    }

    public void SetFSM(FSM<EnemyState> fsm)
    {
        _fSM = fsm;
    }

    void Shoot()
    {
        GameObject bullet = GameObject.Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetTarget(target);
    }

    public override void OnEnd()
    {
    }
}


