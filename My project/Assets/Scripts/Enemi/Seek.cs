using UnityEngine;

public class Seek : MonoBehaviour
{
    [SerializeField] float stopDistance = 2f; // Distancia para frenar y disparar
    public Transform Target;
    bool FollowPlayer;
    Vector3 desired = Vector3.zero; 
    Vector3 velocity = Vector3.zero; 
    Vector3 steering = Vector3.zero; 
    Vector3 dir = Vector3.zero;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float steeringForce = 0.1f;


    private void Start()
    {
  
    }
    void Update()
    {
        if (FollowPlayer)
        {
            actualizedSeek();
        }
      
    }

    void actualizedSeek()
    {
        dir = Target.transform.position - transform.position;
        float distance = dir.magnitude;
        if (distance <= stopDistance)
        {
            velocity = Vector3.zero;
           
            transform.LookAt(Target);


        }
        else
        {
        
            desired = dir.normalized * moveSpeed;
            steering = desired - velocity;
            steering = Vector3.ClampMagnitude(steering, steeringForce);
            velocity = Vector3.ClampMagnitude(velocity + steering, moveSpeed);
            transform.position += velocity * Time.deltaTime;
            transform.LookAt(Target);
            Vector3 moveDir = velocity.normalized;

        }
    }

  
    public void changeFollow(bool value)
    {
        FollowPlayer = value;
    }
 
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; //desired
        Gizmos.DrawLine(transform.position, transform.position + desired);

        Gizmos.color = Color.red; //velocity
        Gizmos.DrawLine(transform.position, transform.position + velocity);

        Gizmos.color = Color.blue; //steering
        Gizmos.DrawLine(transform.position, transform.position + steering * 50);
    }
}
