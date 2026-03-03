using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] IAEnemy iaEnemy;
    void Start()
    {
        iaEnemy = GetComponentInChildren<IAEnemy>();
        findWaypoints();
        iaEnemy.SetWaypoints(waypoints);


    }


    void findWaypoints()
    {
        PosWaypoint[] postWaypoints = GetComponentsInChildren<PosWaypoint>();


        waypoints = new Transform[postWaypoints.Length];

        for (int i = 0; i < postWaypoints.Length; i++)
        {
            waypoints[i] = postWaypoints[i].transform;
        }
        Debug.Log("Waypoints encontrados: " + waypoints.Length);
    }
    public Transform[] GetWaypoints()
    {
        return waypoints;
    }

}
