using UnityEngine;


public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool isActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerLife>() != null)
        {
            CheckpointManager.Instance.SetActiveCheckpoint(this);
        }
    }

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
