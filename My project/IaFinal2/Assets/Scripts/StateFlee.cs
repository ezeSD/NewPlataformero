using IA.PathFinding;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StateFlee<T> : State<T>
{
    Transform casa;
    float speed = 10f;
    float steeringforce = 0.1f;
    float closeDist = 0.5f;

    float detection_radius;
    LayerMask nodemask;

    float recalcInterval = 0.5f;
    float nextRecalc = 0f;



    Transform root;
    int index = 0;
    bool walk = true;

    List<Node> path = new List<Node>();

    Heal heal;
    private FSM<EnemyState> _fsm;
    float rangeToHeal = 5f;
    Node lastInitial;
    Node lastFinal;
    Node finalNode;   

    Vector3 desired = Vector3.zero;
    Vector3 steering = Vector3.zero;
    Vector3 velocity = Vector3.zero;

    [SerializeField] float avoidForce = 1f;
    [SerializeField] float avoidRadius = 1.5f;
    [SerializeField] float avoidCastRadius = 1f;
    [SerializeField] LayerMask avoidObstacles;
    Vector3 avoidDir = Vector3.zero;




    public StateFlee(
        Transform home,
        List<Node> PathNode,
        Transform MyTransform,
        LayerMask Lay,
        float detection,
        Heal healEnemy,
        float range)
    {
        casa = home;
        path = PathNode;
        root = MyTransform;
        nodemask = Lay;
        detection_radius = detection;
        heal = healEnemy;
        rangeToHeal = range;

    }


    public override void OnBegin()
    {
        Debug.Log("FLEE STATE");
        nextRecalc = 0f;
    }

    public override void OnEnd()
    {

    }


    public override void OnUpdate()
    {
        ActualizarMovimiento();

    }


    void ActualizarMovimiento()
    {
        // 🔥 Solo recalcula el path cuando es necesario
        if (Time.time >= nextRecalc)
        {
            RecalculatePath();
            nextRecalc = Time.time + recalcInterval;
        }

        if (walk)
            Mover();
    }


    void RecalculatePath()
    {
        Node initial = FindMostClosestNode(root.position);
        Node final = FindMostClosestNode(casa.position);

        if (initial == null || final == null)
            return;

        if (initial == lastInitial && final == lastFinal)
            return;

        lastInitial = initial;
        lastFinal = final;

        path = Astar(initial, final);

        if (path == null || path.Count < 2)
        {
            walk = false;
            return;
        }

        index = 0;
        walk = true;

        finalNode = final;   
    }


    void Mover()
    {
        if (path == null || path.Count < 2)
        {
            velocity = Vector3.zero;
            return;
        }

        if (index >= path.Count)
        {
            walk = false;
            return;
        }

        Vector3 targetPos = path[index].transform.position;
        Vector3 lookTarget = new Vector3(targetPos.x,root.position.y,targetPos.z);
        root.LookAt(lookTarget);

        Vector3 dir = targetPos - root.position;
        dir.y = 0f;

        if (dir.magnitude < closeDist)
        {
            index++;
            return;
        }

        desired = dir.normalized * speed;
        desired += Avoid() * avoidForce;

        steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, steeringforce);

        velocity += steering;
        velocity = Vector3.ClampMagnitude(velocity, speed);
        velocity.y = 0f;
        desired.y = 0f;
        steering.y = 0f;
        avoidDir.y = 0f;

        root.position += velocity * Time.deltaTime;
        Vector3 newPos = root.position + velocity * Time.deltaTime;
        newPos.y = 4.3f;
        root.position = newPos;

        if (Vector3.Distance(root.position, casa.position) <= closeDist)
        {
            _fsm.SendInput(EnemyState.LeaderLost);
        }
    }

    Vector3 Avoid()
    {
        if (Physics.SphereCast(root.position, avoidCastRadius, velocity,
            out RaycastHit hit, avoidRadius, avoidObstacles))
        {
            avoidDir = Vector3.Reflect(velocity.normalized, hit.normal);
            float magnitude = Mathf.Max(avoidDir.magnitude, 0.01f);
            return avoidDir.normalized * (avoidRadius - magnitude);
        }

        return avoidDir;
    }


    List<Node> Astar(Node initial, Node final)
    {
        foreach (var n in NodeBuilder.Instance.Nodes)
            n.Clean();

        List<Node> visited = new List<Node>();
        PriorityQueue<Node> abiertos = new PriorityQueue<Node>();

        initial.costo = 0;
        initial.costoFinal =
            initial.costo + Vector3.Distance(initial.transform.position, final.transform.position);
        abiertos.Enqueue(initial, initial.costoFinal);

        while (abiertos.Count > 0)
        {
            Node current = abiertos.Dequeue();

            if (current == final)
                return Reconstruct(initial, final);

            visited.Add(current);

            foreach (Node n in current.Neighbors)
            {
                if (visited.Contains(n)) continue;

                float newCost =
                    current.costo + Vector3.Distance(current.transform.position, n.transform.position);

                if (newCost < n.costo)
                {
                    n.SetParent(current);
                    n.costo = newCost;

                    float H =
                        Vector3.Distance(n.transform.position, final.transform.position);
                    n.costoFinal = n.costo + H;

                    abiertos.Enqueue(n, n.costoFinal);
                }
            }
        }

        return null;
    }


    List<Node> Reconstruct(Node initial, Node final)
    {
        List<Node> list = new List<Node>();
        Node current = final;

        while (current != null && current != initial)
        {
            list.Add(current);
            current = current.Parent;
        }

        list.Add(initial);
        list.Reverse();
        return list;
    }


    Node FindMostClosestNode(Vector3 point)
    {
        Collider[] cols = Physics.OverlapSphere(point, detection_radius, nodemask);
        Node bestNode = null;
        float mostClose = detection_radius + 1;

        foreach (var col in cols)
        {
            Node node = col.GetComponent<Node>();
            if (node != null)
            {
                float dist = Vector3.Distance(point, node.transform.position);
                if (dist < mostClose)
                {
                    mostClose = dist;
                    bestNode = node;
                }
            }
        }

        return bestNode;
    }


    public void SetFSM(FSM<EnemyState> fsmInstance)
    {
        _fsm = fsmInstance;
    }
}
