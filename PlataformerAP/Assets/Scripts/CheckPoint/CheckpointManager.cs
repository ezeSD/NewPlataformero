using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private Checkpoint activeCheckpoint;
    private Vector3 initialPosition;
    private bool hasCheckpoint = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetInitialPosition(Vector3 position)
    {
        initialPosition = position;
    }

    public void SetActiveCheckpoint(Checkpoint checkpoint)
    {
        if (activeCheckpoint != null)
        {
            activeCheckpoint.Deactivate();
        }

        activeCheckpoint = checkpoint;
        activeCheckpoint.Activate();
        hasCheckpoint = true;

    }

    public Vector3 GetRespawnPosition()
    {
        if (hasCheckpoint && activeCheckpoint != null)
        {
            return activeCheckpoint.GetPosition();
        }
        return initialPosition;
    }

    public bool HasActiveCheckpoint()
    {
        return hasCheckpoint;
    }

    public void ResetCheckpoint()
    {
        if (activeCheckpoint != null)
        {
            activeCheckpoint.Deactivate();
        }
        activeCheckpoint = null;
        hasCheckpoint = false;
    }
}