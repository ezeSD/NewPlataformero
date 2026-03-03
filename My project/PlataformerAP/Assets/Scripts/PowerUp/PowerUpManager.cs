using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void CollectPowerUp(PowerUp powerUp, PlayerModel player)
    {
        powerUp.playerGet = player;
        powerUp.IsActive = true;
        powerUp.ApplyPowerUp();

        StartCoroutine(PowerUpTimer(powerUp));
    }

    IEnumerator PowerUpTimer(PowerUp powerUp)
    {
        yield return new WaitForSeconds(powerUp.Duration);
        powerUp.RemovePowerUp();
        powerUp.gameObject.SetActive(false);
    }
}