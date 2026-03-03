using UnityEngine;

public abstract class Factory<T> : MonoBehaviour
{
    public abstract T Create();
    public abstract void Return(T item);
}
