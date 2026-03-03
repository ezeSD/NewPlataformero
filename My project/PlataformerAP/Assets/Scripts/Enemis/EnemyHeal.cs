using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeal : MonoBehaviour, IDamageable
{
    [SerializeField] int healAmount = 10;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        die();

    }
    public void TakeDamage(int amount)
    {
       if(healAmount > 0)
       {
            healAmount -= amount;
       }

      

    }

    

    void die()
    {
        if (healAmount <= 0)
        {
            Destroy(gameObject);
        }
    }

}
