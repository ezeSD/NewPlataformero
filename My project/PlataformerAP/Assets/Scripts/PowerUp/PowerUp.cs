using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public float Duration;
    public float Cooldown;
    public bool IsActive;

    float MaxDuration;
    PlayerModel PlayerModel;
    public PlayerModel playerGet
    {
        get { return PlayerModel; }
        set { PlayerModel = value; }
    }

    #region Getters
    public float maxDuration
    {
        get { return MaxDuration; }
    }
    #endregion

    void Start()
    {
        MaxDuration = Duration;
        PlayerModel = FindObjectOfType<PlayerModel>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerModel player = other.GetComponent<PlayerModel>();

            if (player != null)
            {
                player = playerGet;
                PowerUpManager.Instance.CollectPowerUp(this, player);
            }
        }
    }

    public virtual void ApplyPowerUp()
    {
        // To be overridden in derived classes
    }

    public virtual void RemovePowerUp()
    {
        IsActive = false;
        Duration = MaxDuration;
        gameObject.SetActive(false);
    }

}
