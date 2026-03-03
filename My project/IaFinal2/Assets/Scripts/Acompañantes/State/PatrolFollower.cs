using System.Collections.Generic;
using UnityEngine;

public class PatrolFollower<T> : State<T>
{
    float steeringForce = 0.15f;
    float flockSpeed = 15f;

    FSM<EnemyState> _fsm;

    float separationForce = 2f;
    float alignmentForce = 0.02f;
    float cohesionForce = 0.01f;

    float separationRadius = 10f;
    float alignmentRadius = 0.04f;
    float cohesionRadius = 0.01f;

    Transform myTransform;
    Transform leader;

    float leaderStopRadius = 2.2f;     // DISTANCIA FINAL AL LÍDER
    float leaderFollowRadius = 3.2f;   // CUÁNDO VOLVER A SEGUIR

    Vector3 velocity;

    public PatrolFollower(
        Transform me,
        Transform leaderFollow,
        float stopRadius,
        float followRadius)
    {
        myTransform = me;
        leader = leaderFollow;
        leaderStopRadius = stopRadius;
        leaderFollowRadius = followRadius;
    }

    public override void OnBegin()
    {
        velocity = myTransform.forward * 0.1f;
    }

    public override void OnUpdate()
    {
        FlockingBehaviour();
    }


    void FlockingBehaviour()
    {
        Vector3 desired = Vector3.zero;

        var sep = FlockManager.instance.GetNeighbors(myTransform.position, separationRadius, myTransform);
        var ali = FlockManager.instance.GetNeighbors(myTransform.position, alignmentRadius, myTransform);
        var coh = FlockManager.instance.GetNeighbors(myTransform.position, cohesionRadius, myTransform);

        desired += Separation(sep) * separationForce;

        desired += Alignment(ali) * alignmentForce;
        desired += Cohesion(coh) * cohesionForce;

        float distToLeader = Vector3.Distance(myTransform.position, leader.position);

        if (distToLeader > leaderFollowRadius)
        {
            desired += FollowLeader();
        }
        if (distToLeader <= leaderStopRadius)
        {
            velocity *= 0.85f; 
            desired = Vector3.ClampMagnitude(desired, flockSpeed * 0.3f);
        }

        if (desired.sqrMagnitude < 0.0001f)
            return;

        Vector3 desiredVelocity = desired.normalized * flockSpeed;
        Vector3 steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, steeringForce);

        velocity += steering;
        velocity = Vector3.ClampMagnitude(velocity, flockSpeed);

        myTransform.position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > 0.05f)
        {
            myTransform.forward = Vector3.Lerp(
                myTransform.forward,
                velocity.normalized,
                Time.deltaTime * 8f
            );
        }
    }

    Vector3 Separation(List<Transform> neighbors)
    {
        if (neighbors.Count == 0) return Vector3.zero;

        Vector3 force = Vector3.zero;

        foreach (var n in neighbors)
        {
            Vector3 diff = myTransform.position - n.position;
            float dist = diff.magnitude;
            if (dist > 0.001f)
                force += diff.normalized / dist;
        }

        return force.normalized;
    }

    Vector3 Alignment(List<Transform> neighbors)
    {
        if (neighbors.Count == 0) return Vector3.zero;

        Vector3 avg = Vector3.zero;

        foreach (var n in neighbors)
        {
            var pf = n.GetComponent<IAEnemyFollower>()?.fsm?.current as PatrolFollower<T>;
            if (pf != null)
                avg += pf.velocity;
        }

        if (avg == Vector3.zero) return Vector3.zero;
        return avg.normalized;
    }

    Vector3 Cohesion(List<Transform> neighbors)
    {
        if (neighbors.Count == 0) return Vector3.zero;

        Vector3 center = Vector3.zero;
        foreach (var n in neighbors)
            center += n.position;

        center /= neighbors.Count;
        return (center - myTransform.position).normalized;
    }

    Vector3 FollowLeader()
    {
        Vector3 behind = leader.position - leader.forward * leaderStopRadius;
        return (behind - myTransform.position).normalized;
    }

    public override void OnEnd()
    {
    }

    public void SetFSM(FSM<EnemyState> fsm)
    {
        _fsm = fsm;
    }

}

