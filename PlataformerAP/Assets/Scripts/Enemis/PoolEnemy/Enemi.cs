using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemi : MonoBehaviour
{
    [SerializeField] Transform[] Waypoints;
    protected Factory<IAEnemy> _factory;
    protected Transform targenPost;

    Waypoints waypoints;
    private void Start()
    {
        waypoints = GetComponent<Waypoints>();
        SetWaypoints(waypoints.GetWaypoints());

    }

    public virtual void Initialize(Factory<IAEnemy> Factory, Transform targetPos)
    {
        _factory = Factory;
        targenPost = targetPos;
    }
    public void SetWaypoints(Transform[] wp)
    {
        Waypoints = new Transform[wp.Length];
        for (int i = 0; i < wp.Length; i++)
        {
            Waypoints[i] = wp[i];
        }

        Debug.Log("Waypoints asignados: " + Waypoints.Length);

    }
    public virtual void Refresh() { }
}
