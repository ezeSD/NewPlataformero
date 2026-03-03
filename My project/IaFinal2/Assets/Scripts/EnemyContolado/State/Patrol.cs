using IA.PathFinding;
using System.Collections.Generic;
using UnityEngine;
public class Patrol<T> : State<T>
{
    private FSM<EnemyState> _fsm; // Referencia a la FSM
    float detection_radius;
    LayerMask nodemask;
    bool walk = false;
    List<Node> path = new List<Node>();
    float closeDist = 0.5f;
    float speed = 15f;
    float mostClose;
    int index = 0;
    Transform Mytransform;


    public Patrol(Transform myTr, float detection_radius, LayerMask nodemask)
    {
        this.detection_radius = detection_radius;
        this.nodemask = nodemask;
        Mytransform = myTr;
    }

    public override void OnBegin()
    {
        Target.instance.SubscribeToEndClick(OnClick);
    }

    public override void OnEnd()
    {
        Target.instance.UnsubscribeToEndClick(OnClick);
    }

    public override void OnUpdate()
    {
        if (walk)
        {
            Vector3 direction = path[index].transform.position - Mytransform.position;

            if (direction.magnitude < closeDist)
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
                Mytransform.position = Mytransform.position + direction * Time.deltaTime * speed;
            }
        }

        Vector3 TrPost = Target.Position;
        if(Vector3.Distance(Mytransform.position,TrPost) < 20f)
        {
            speed = 0;
        }

    }
    Node bestNode;

    void OnClick()
    {
        speed = 15f;
        Node initial = FindMostClosestNode(Mytransform.position);
        Node final = FindMostClosestNode(Target.Position);

        if (initial == null || final == null)
        {
            Debug.LogError("No se pudo calcular el path");
            return;
        }

        List<Node> newPath = Astar(initial, final);

        if (newPath != null && newPath.Count > 0)
        {
            FinishPath(newPath);
        }

    }

    void FinishPath(List<Node> _path)
    {
        this.path = _path;

        if (path != null)
        {
            index = 0;
            walk = true;
        }
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

        return list;
    }

    Node FindMostClosestNode(Vector3 point)
    {
        Collider[] cols = Physics.OverlapSphere(point, detection_radius, nodemask);

        bestNode = null;
        mostClose = detection_radius + 1;

        for (int i = 0; i < cols.Length; i++)
        {
            Node node = cols[i].GetComponent<Node>();

            if (node != null)
            {
                Vector3 dir = point - node.transform.position;

                if (dir.magnitude < mostClose)
                {
                    mostClose = dir.magnitude;
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