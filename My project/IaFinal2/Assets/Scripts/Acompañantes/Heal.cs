using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface takeDamage
{
    void HealDamage(int healAmount);
}


public class Heal : MonoBehaviour, takeDamage
{
    public int healAmount = 100;
    public int minHealth = 30;
    [SerializeField] Transform Safearea;
    [SerializeField] float rangeToSafe = 5f;

    FSM<EnemyState> _fsm;

    public void HealDamage(int damage)
    {
        healAmount -= damage;

        if (healAmount <= 0)
        {
            Destroy(gameObject);
        }
    }



    private void Update()
    {
        if(Vector3.Distance(transform.position, Safearea.position) <= rangeToSafe)
        {
            healAmount = 100;
        }
    }

    public void setHeal(int amountHeal)
    {
        healAmount = amountHeal;
    }

    public void setFsm(FSM<EnemyState> fsm)
    {
        _fsm = fsm;
    }

}
