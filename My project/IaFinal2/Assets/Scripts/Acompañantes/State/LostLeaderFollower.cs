using IA.PathFinding;
using System.Collections.Generic;
using UnityEngine;

public class LostLeaderFollower<T> : State<T>
{
    FSM<EnemyState> _fsm;

    Transform me;
    Transform leader;

    List<Node> path = new List<Node>();
    int index = 0;

    float closeDist = 0.5f;
    float speed = 15f;
    float steeringForce = 0.15f;

    Vector3 velocity = Vector3.zero;
    Vector3 desired = Vector3.zero;
    Vector3 steering = Vector3.zero;

    LayerMask thetaMask;
    float sphereRadius = 0.2f;

    Vector3 lastLeaderPos;
    float repathDistance = 10f;
    public LostLeaderFollower(Transform me, Transform leader, LayerMask thetaMask)
    {
        this.me = me;
        this.leader = leader;
        this.thetaMask = thetaMask;
    }

    public override void OnBegin()
    {
        lastLeaderPos = leader.position;
        ComputePath();
    }

    public override void OnEnd() { Debug.Log("Paso a patrullar"); }

    public override void OnUpdate()
    {

        if ((leader.position - lastLeaderPos).sqrMagnitude > repathDistance * repathDistance)
        {
            lastLeaderPos = leader.position;
            ComputePath();
        }

        FollowPath();
    }

    void ComputePath()
    {
        Node initial = FindMostClosestNode(me.position);
        if (initial == null) Debug.Log("no hay inicial");

        Node final = FindMostClosestNode(leader.position);
        if (final == null) Debug.Log("no hay final");



        if (initial == null || final == null)
        {
            Debug.LogError("Theta* error: nodos invalidos");
            return;
        }

        path = Astar(initial, final);
        index = 0;
    }

    void FollowPath()
    {
        Debug.Log("FOLLOW PATH");
        if (path == null || path.Count == 0)
            return;

        Vector3 target = path[index].transform.position;
        Vector3 toTarget = target - me.position;
        toTarget.y = 0f;

        if (toTarget.magnitude < closeDist)
        {
            index++;
            if (index >= path.Count)
            {
                velocity = Vector3.zero;
                return;
            }

            target = path[index].transform.position;
            toTarget = target - me.position;
            toTarget.y = 0f;
        }

        Vector3 desiredVelocity = toTarget.normalized * speed;

        velocity = Vector3.Lerp(velocity, desiredVelocity, steeringForce);
        me.position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > 0.001f)
            me.forward = velocity.normalized;
    }



    Node FindMostClosestNode(Vector3 point)
    {
        float best = Mathf.Infinity;
        Node bestNode = null;

        foreach (var n in NodeBuilder.Instance.Nodes)
        {
            float d = (point - n.transform.position).sqrMagnitude;
            if (d < best)
            {
                best = d;
                bestNode = n;
            }
        }

        return bestNode;
    }

    List<Node> Astar(Node initial, Node final)
    {
        foreach (var n in NodeBuilder.Instance.Nodes)
            n.Clean();

        List<Node> visited = new List<Node>();
        PriorityQueue<Node> open = new PriorityQueue<Node>();

        initial.costo = 0;
        initial.costoFinal = initial.costo + Vector3.Distance(initial.transform.position, final.transform.position);
        open.Enqueue(initial, initial.costoFinal);
        initial.open = true;

        while (open.Count > 0)
        {
            Node current = open.Dequeue();
            current.open = false;

            if (current == final)
                return ReconstructTheta(initial, final);

            visited.Add(current);
            current.visited = true;

            foreach (Node n in current.Neighbors)
            {
                if (visited.Contains(n)) continue;

                float newCost = current.costo + Vector3.Distance(current.transform.position, n.transform.position);

                if (newCost < n.costo)
                {
                    n.SetParent(current);
                    n.costo = newCost;

                    float H = Vector3.Distance(n.transform.position, final.transform.position);

                    n.costoFinal = n.costo + H;
                    open.Enqueue(n, n.costoFinal);
                    n.open = true;
                }
            }
        }
        return null;
    }

    List<Node> ReconstructTheta(Node initial, Node final)
    {
        List<Node> list = new List<Node>();
        Node current = final;

        while (current != null && current != initial)
        {
            list.Add(current);

            Node prev = current.Parent;
            Node best = prev;

            while (prev != null && OnSight(current, prev))
            {
                best = prev;
                prev = prev.Parent;
            }

            current = best;
        }

        list.Add(initial);
        list.Reverse();
        return list;
    }

    bool OnSight(Node a, Node b)
    {
        Vector3 offset = Vector3.up * sphereRadius;
        Vector3 dir = (b.transform.position + offset) - (a.transform.position + offset);

        float distance = dir.magnitude;

        RaycastHit hit;
        return !Physics.SphereCast(a.transform.position, sphereRadius, dir.normalized, out hit, distance, thetaMask);
    }

    public void SetFSM(FSM<EnemyState> fsm)
    {
        _fsm = fsm;
    }
}