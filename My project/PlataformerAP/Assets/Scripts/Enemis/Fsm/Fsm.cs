
using System;
using UnityEngine;

public class FSM<T>
{
    public State<T> current;
    public FSM(State<T> first)
    {
        this.current = first;
    }
    public void StartFSM()
    {
        ChangeState(current);
    }
    public void SendInput(T input)
    {
        for (int i = 0; i < current.transitions.Length; i++)
        {
            if (current.transitions[i].GetInput().Equals(input))
            {
                ChangeState(current.transitions[i].GetState());
            }
        }
    }

    void ChangeState(State<T> newState)
    {
        if (current != null)
        {
            current.OnEnd();
        }
        current = newState;

        current.OnBegin();
    }

    public void UpdateFSM()
    {
        if (current != null)
        {
            current.OnUpdate();
        }
    }


}
