using System.Collections.Generic;
using UnityEngine;

public class IcanSeePlayer : MonoBehaviour
{
    private FSM<EnemyState> _fsm;

    [Header("References")]
    [SerializeField] Transform root;
    [SerializeField] Transform eyes;

    [Header("Target Detection")]
    [SerializeField] float visionDistance = 15f;
    [SerializeField] float angleMin = 90f;
    [SerializeField] LayerMask toDetect;

    [SerializeField] IAEnemy enemyScript;


    [SerializeField] Transform tr;
    public Transform FirstEnemyDetected { get; private set; }

    private Vector3 lastSeenPosition;

    private IAEnemy[] allEnemies;

    private void Start()
    {
        allEnemies = GameObject.FindObjectsOfType<IAEnemy>();
    }

    private void Update()
    {
        DetectClosestVisibleEnemy();
    }

    void DetectClosestVisibleEnemy()
    {
        Vector3 dirToForward = eyes.forward;

        Ray ray = new Ray(eyes.position, dirToForward);
        RaycastHit[] hits = Physics.RaycastAll(ray, visionDistance, toDetect);

        if (hits.Length == 0)
        {
            FirstEnemyDetected = null;
            if (_fsm != null)
                _fsm.SendInput(EnemyState.Patrol);
            return;
        }

        List<RaycastHit> enemyHits = new List<RaycastHit>();

        foreach (var hit in hits)
        {
            Vector3 dirToEnemy = hit.collider.transform.position - eyes.position;
            float angle = Vector3.Angle(eyes.forward, dirToEnemy);

            if (angle < angleMin / 2f)
            {
                enemyHits.Add(hit);
            }
        }

        if (enemyHits.Count == 0)
        {
            FirstEnemyDetected = null;
            if (_fsm != null)
                _fsm.SendInput(EnemyState.Patrol);
            return;
        }

        enemyHits.Sort((a, b) => a.distance.CompareTo(b.distance));

        FirstEnemyDetected = enemyHits[0].collider.transform;
        tr = FirstEnemyDetected;

        if (enemyScript != null && FirstEnemyDetected != null)
        {
            enemyScript.setTransform(FirstEnemyDetected);
            lastSeenPosition = FirstEnemyDetected.position;
        }

        foreach (var e in allEnemies)
        {
            e.SetLastSeenPosition(lastSeenPosition);
            e.inputs(true, EnemyState.InRange);
        }

        if (_fsm != null)
            _fsm.SendInput(EnemyState.InRange);
    }

    public void SetFSM(FSM<EnemyState> fsmInstance)
    {
        _fsm = fsmInstance;
    }

    private void OnDrawGizmos()
    {
        if (eyes != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(eyes.position, 0.1f);
            Gizmos.DrawWireSphere(eyes.position, visionDistance);
        }

        if (FirstEnemyDetected != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(eyes.position, FirstEnemyDetected.position);
            Gizmos.DrawWireSphere(FirstEnemyDetected.position, 0.3f);
        }

        Vector3 left = Quaternion.Euler(0, -angleMin / 2f, 0) * root.forward;
        Vector3 right = Quaternion.Euler(0, angleMin / 2f, 0) * root.forward;
        Gizmos.color = Color.red;

        Gizmos.DrawRay(root.position, left * visionDistance);
        Gizmos.DrawRay(root.position, right * visionDistance);
    }
}



