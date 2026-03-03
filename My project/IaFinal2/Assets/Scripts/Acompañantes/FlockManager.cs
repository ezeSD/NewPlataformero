using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public static FlockManager instance;

    [SerializeField] List<Transform> followers = new List<Transform>();
    [SerializeField] Transform leader;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void SetLeader(Transform newLeader)
    {
        leader = newLeader;
    }

    public Transform GetLeader()
    {
        return leader;
    }

    public void AddFollower(Transform f)
    {
        if (!followers.Contains(f))
            followers.Add(f);
    }

    public List<Transform> GetFollowers()
    {
        return followers;
    }

    public List<Transform> GetNeighbors(Vector3 pos, float radius, Transform except = null)
    {
        List<Transform> result = new List<Transform>();

        foreach (var f in followers)
        {
            if (f == except) continue;
            Vector3 diff = f.position - pos;

            if (diff.sqrMagnitude < radius * radius)
                result.Add(f);
        }
        return result;
    }
}
