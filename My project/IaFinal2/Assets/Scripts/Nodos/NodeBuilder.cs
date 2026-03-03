using IA.PathFinding;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IA.PathFinding
{
    [ExecuteInEditMode]
    public class NodeBuilder : MonoBehaviour
    {
        public static NodeBuilder Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this.gameObject);
        }

        public Node[] Nodes
        {
            get
            {
                return nodes;
            }
        }

        [SerializeField] Node[] nodes;

        [SerializeField] bool update = false;
        [SerializeField] bool bake = false;

        private void OnEnable() => EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

        private void OnDisable() => EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode) this.enabled = false;
        }


        void Update()
        {
            if (update || bake)
            {
                bake = false;
                nodes = GetComponentsInChildren<Node>();

                foreach (Node node in nodes)
                {
                    node.BakeNeighbors();
                }
            }
        }
    }
}