
using IA.PathFinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Follow<T> : State<T>
{
    private FSM<EnemyState> _fsm;

    [SerializeField] float detection_radius;
    [SerializeField] LayerMask nodemask;
    [SerializeField] float closeDist = 0.5f;
    [SerializeField] float speed = 5f;
    [SerializeField] float recalcInterval = 0.5f;

    public Vector3 LastWapeon;



    Transform root;
    int index = 0;
    bool walk = true;
    Transform target;
    List<Node> path = new List<Node>();


    public Follow(List<Node> PathNode, Transform MyTransform, LayerMask Lay, float detection, Transform transformTarget, Vector3 Wapeon)
    {
        path = PathNode;
        root = MyTransform;
        nodemask = Lay;
        detection_radius = detection;
        target = transformTarget;
        LastWapeon = Wapeon;

    }

    public override void OnBegin()
    {
        Debug.Log("Follow OnBegin");

        if (target == null)
        {
            Debug.LogWarning("Follow sin target asignado.");
            return;
        }

    }

    public override void OnEnd()
    {


    }
    Vector3 desired = Vector3.zero;
    Vector3 steering = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    [SerializeField] float steeringforce = 0.1f;
    public override void OnUpdate()
    {
        OnClick();
        if (walk)
        {
            desired = path[index].transform.position - root.position;
            
            root.LookAt(path[index].transform.position);

            if (desired.magnitude < closeDist)
            {
                index = index + 1;
                if (index >= path.Count)
                {
                    index = 0;
                    walk = false;
                }
            }
            else
            {

                desired = desired.normalized * speed;

                desired += Avoid() * avoidForce;

                steering = desired - velocity;
                steering = Vector3.ClampMagnitude(steering, steeringforce);

                velocity += steering;

                velocity = Vector3.ClampMagnitude(velocity, speed);

                root.position += velocity * Time.deltaTime;
            }
        }


    }

    void OnClick()
    {
        Node initial = FindMostClosestNode(root.position);
        IAEnemy enemy = root.GetComponent<IAEnemy>();
        Node final;
        if (enemy != null)
        {
             final = FindMostClosestNode(enemy.GetLastSeenPosition());
        }
        else
        {
             final = FindMostClosestNode(target.position); 
        }

        if (initial == null) Debug.LogError("<color=yellow> no se encontro el inicial</color>");
        if (final == null) Debug.LogError("<color=yellow> no se encontro el final</color>");

        path.Clear();
        path = Astar(initial, final);
        walk = true;

    }

   

    public void SetFSM(FSM<EnemyState> fsmInstance)
    {
        _fsm = fsmInstance;
    }

    List<Node> Astar(Node initial, Node final)
    {
        foreach (var n in NodeBuilder.Instance.Nodes)
        {
            n.Clean();
        }

        List<Node> visited = new List<Node>();
        PriorityQueue<Node> abiertos = new PriorityQueue<Node>();

        initial.costo = 0;
        initial.costoFinal = initial.costo + Vector3.Distance(initial.transform.position, final.transform.position);
        abiertos.Enqueue(initial, initial.costoFinal);
        initial.open = true;

        while (abiertos.Count > 0)
        {
            Node current = abiertos.Dequeue();
            current.open = false;

            if (current == final)
            {
                return Reconstruct(initial, final);
            }

            visited.Add(current);
            current.visited = true;

            foreach (Node n in current.Neighbors)
            {
                if (visited.Contains(n)) continue;

                float newCost = current.costo +
                    Vector3.Distance(current.transform.position, n.transform.position);

                if (newCost < n.costo)
                {
                    n.SetParent(current);
                    n.costo = newCost;
                    float H = Vector3.Distance(n.transform.position, final.transform.position);
                    n.costoFinal = n.costo + H;
                    abiertos.Enqueue(n, n.costoFinal);
                    n.open = true;
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
        Debug.Log("recontruido");

        return list;
    }

    Node FindMostClosestNode(Vector3 point)
    {
        Collider[] cols = Physics.OverlapSphere(point, detection_radius, nodemask);
        Node bestNode = null;
        float mostClose = detection_radius + 1;

        for (int i = 0; i < cols.Length; i++)
        {
            Node node = cols[i].GetComponent<Node>();
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


    [SerializeField] float avoidForce = 1.5f;

    [SerializeField] float avoidRadius = 3f;
    [SerializeField] float avoidCastRadius = 1f;
    [SerializeField] LayerMask avoidObstacles;
    Vector3 avoidDir = Vector3.zero;
    Vector3 Avoid()
    {
        if (Physics.SphereCast(root.position, avoidCastRadius, velocity, out RaycastHit hit, avoidRadius, avoidObstacles))
        {
            avoidDir = Vector3.Reflect(velocity.normalized, hit.normal);

            float magnitude = Math.Max(avoidDir.magnitude, 0.01f);

            return avoidDir.normalized * (avoidRadius - magnitude);
        }

        return avoidDir;
    }
}

public class PriorityQueue<T>
{
    List<PriorityPair> list = new List<PriorityPair>();
    public int Count { get { return list.Count; } }

    public void Enqueue(T data, float priority)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].data.Equals(data))
            {
                list[i].UpdatePriority(priority);
                list = list.OrderBy(a => a.priority).ToList();
                return;
            }
        }

        list.Add(new PriorityPair(data, priority));
        list = list.OrderBy(a => a.priority).ToList();
    }


    public T Dequeue()
    {
        T element = list[0].data;
        list.RemoveAt(0);
        return element;
    }

    struct PriorityPair
    {
        public T data;
        public float priority;
        public void UpdatePriority(float _priority) { priority = _priority; }
        public PriorityPair(T _data, float prior) { data = _data; priority = prior; }
    }
}

