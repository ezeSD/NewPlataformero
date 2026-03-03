using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IA.PathFinding
{
    public class Node : MonoBehaviour
    {

        [SerializeField] float detectionRadius = 2f;

        [SerializeField] List<Node> neighbors;

        public List<Node> Neighbors { get { return neighbors; } }

        public float costo = Mathf.Infinity; // con el que comparamos
        public float costoFinal = Mathf.Infinity; // se va a encolar

        public bool visited;
        public bool open;

        [Range(0,3)] public float environmental;

        // para reconectar el camino de regreso
        Node parent = null;
        public Node Parent { get { return parent; } }
        public void SetParent(Node _p)
        {
            parent = _p;
        }

        public void Clean()
        {
            parent = null;
            costo = float.MaxValue;
            costoFinal = float.MaxValue;
            visited = false;
            open = false;
        }


        [SerializeField] LayerMask nodeMask;

        [SerializeField] LayerMask maskview;
        [SerializeField] LayerMask floorAndObstacles;

        [SerializeField] float maxSlope = 0.5f;

        [Header("Gizmos")]
        [SerializeField] bool drawRadius = false;
        [SerializeField] bool drawsphere = false;
        [SerializeField] bool drawConnections = false;

        [SerializeField] float floorUPOffset = 0.1f;

        void Start()
        {

        }

        public void BakeNeighbors()
        {
            Adjust();
            Detect();
        }

        void Adjust()
        {

            if (Physics.Raycast(transform.position + Vector3.up * 10, Vector3.down, out RaycastHit hit, 20, floorAndObstacles))
            {
               transform.position = hit.point + Vector3.up * floorUPOffset;
            }
        }

        void Detect()
        {
            neighbors = new List<Node>();
            Collider[] colls = Physics.OverlapSphere(transform.position, detectionRadius, nodeMask);

            for (int i = 0; i < colls.Length; i++)
            {
                Node node = colls[i].GetComponent<Node>();
                if (node != null && node != this)
                {
                    Vector3 dir = node.transform.position - transform.position;

                    Ray ray = new Ray();
                    ray.origin = transform.position;
                    ray.direction = dir;
  
                    if (Physics.Raycast(ray, out RaycastHit hit, dir.magnitude, maskview))
                    {
          
                        Node hitNode = hit.collider.GetComponent<Node>();
                        if (hitNode != null && hitNode == node)
                        {
                            float h = node.transform.position.y - transform.position.y;

                            if (Mathf.Abs(h) < maxSlope)
                            {
                                neighbors.Add(node);
                            }
                        }
                    }
                }
            }
        }


        private void OnDrawGizmos()
        {
            if (drawRadius)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, detectionRadius);
            }

            if (drawConnections)
            {
                Gizmos.color = Color.white;

                if (neighbors != null)
                {
                    for (int i = 0; i < neighbors.Count; i++)
                    {
                        Vector3 dir = neighbors[i].transform.position - transform.position;

                        dir /= 3;

                        Gizmos.DrawLine(transform.position, transform.position + dir);

                    }
                }
            }

            

            if (drawsphere)
            {
                Gizmos.color = open ? Color.yellow : (visited ? Color.magenta : Color.black);
                Gizmos.DrawSphere(transform.position, 0.1f);
            }

            if (environmental > 0)
            {
                if (environmental >= 1)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(transform.position + Vector3.up * 0.2f, Vector3.one * 0.15f);
                }
                if (environmental >= 2)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawCube(transform.position + Vector3.up * 0.3f, Vector3.one * 0.15f);
                }
                if (environmental >= 3)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(transform.position + Vector3.up * 0.4f, Vector3.one * 0.15f);
                }
            }
        }
    }
}

