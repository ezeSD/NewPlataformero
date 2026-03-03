using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace IA.PathFinding
{
    public class PathFinder : MonoBehaviour
    {
        [SerializeField] float detection_radius;
        [SerializeField] LayerMask nodemask;

        bool walk = false;
        List<Node> path = new List<Node>();

        [SerializeField]float closeDist = 0.5f;
        [SerializeField] float speed = 5f;

        [SerializeField] float simulatio_step_delay = 0.05f;


        // Start is called before the first frame update
        void Start()
        {
            Target.instance.SubscribeToEndClick(OnClick);
        }

        void OnClick()
        {
            Node initial = FindMostClosestNode(transform.position);
            Node final = FindMostClosestNode(Target.Position);

            if (initial == null) Debug.LogError(" no se encontro el inicio");
            if (final == null) Debug.LogError(" no se encontro el final");

            path.Clear();
            path = Astar(initial, final);
            FinishPath(path);


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

        int index = 0;
        private void Update()
        {
            if (walk)
            {
                Vector3 direction = path[index].transform.position - transform.position;

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
                    transform.position = transform.position + direction * Time.deltaTime * speed;
                }
            }
        }

        List<Node> BFS(Node initial, Node final)
        {
            //foreach (var node in NodeBuilder.Instance.Nodes)
            //{
            //    node.Clean();
            //}

            Queue<Node> open = new Queue<Node>();
            List<Node> visited = new List<Node>();

            open.Enqueue(initial);
            visited.Add(initial);

            while (open.Count > 0)
            {
                Node current = open.Dequeue();

                if (current == final)
                {
                    return Reconstruct(initial, final);
                }

                foreach (Node n in current.Neighbors) // Explode
                {
                    if (visited.Contains(n)) continue;

                    n.SetParent(current);
                    visited.Add(n);
                    open.Enqueue(n);
                }
            }
            return null;
        }

       
        List<Node> Dijkstra(Node initial, Node final)
        {
            foreach (var n in NodeBuilder.Instance.Nodes)
            {
                n.Clean();
            }


            List<Node> visited = new List<Node> ();
            PriorityQueue<Node> abiertos = new PriorityQueue<Node>();

            initial.costo = 0;
            abiertos.Enqueue(initial, initial.costo);

            while (abiertos.Count > 0)
            {
                Node current = abiertos.Dequeue();

                if (current == final)
                {
                    return Reconstruct(initial, final);
                }

                visited.Add(current);

                foreach (Node n in current.Neighbors)
                {
                    if (visited.Contains(n)) continue;

                    float newCost = current.costo + 
                        Vector3.Distance(current.transform.position, n.transform.position);

                    if (newCost < n.costo)
                    {
                        n.SetParent(current);
                        n.costo = newCost;
                        abiertos.Enqueue(n, newCost);
                    }
                }
            }

            return null;

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
            initial.costoFinal = initial.costo + Vector3.Distance(initial.transform.position, final.transform.position) ;
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

        float mostClose;
        Node bestNode;
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
    }

    public class PriorityQueue<T>
    {
        List<PriorityPair> list;

        public int Count { get { return list.Count; } }

        public PriorityQueue()
        {
            list = new List<PriorityPair>();
        }

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

            PriorityPair pair = new PriorityPair(data, priority);
            list.Add(pair);
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

            public void UpdatePriority(float _priority)
            {
                priority = _priority;
            }

            public PriorityPair(T _data, float prior)
            {
                this.data = _data;
                this.priority = prior;
            }
        }
    }
}
