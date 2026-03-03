using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death<t> : State<t>
{
    private FSM<EnemyState> _fsm; // Referencia a la FSM
    GameObject MyGameObject;
    public Death(GameObject Propio) 
    {
        MyGameObject = Propio;
    }

    public override void OnBegin()
    {
      GameObject.Destroy(MyGameObject);
    }

    public override void OnEnd()
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {

        GameObject.Destroy(MyGameObject);
    }
}
