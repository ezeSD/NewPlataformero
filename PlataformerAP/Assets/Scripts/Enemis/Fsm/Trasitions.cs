
using UnityEngine;

public class Transition<T>
{
    T input;
    State<T> state;

    public Transition(T input, State<T> state)
    {
        this.input = input;
        this.state = state;
    }

    public T GetInput()
    {
        return input;
    }

    public State<T> GetState()
    {
        return state;
    }
}
