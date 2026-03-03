
using UnityEngine;
public abstract class State<T>
{
    public State()
    {
    }
    public void AddTransitionsState(Transition<T>[] transition)
    {
        transitions = transition;
    }
    public Transition<T>[] transitions;
    public abstract void OnBegin();
    public abstract void OnEnd();
    public abstract void OnUpdate();
}
