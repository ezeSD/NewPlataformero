using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerUp : PowerUp 
{
    [SerializeField] float speedMultiplier = 2.0f;
    [SerializeField] float originalSpeed;
    bool speedIncreased = false;

    public override void ApplyPowerUp()
    {
        if (IsActive == false)
            return;
        playerGet.SetSpeed(playerGet.GetSpeed() * speedMultiplier);
    }

    public override void RemovePowerUp()
    {
        if (speedIncreased)
        {
            playerGet.SetSpeed(playerGet.SetSpeed(originalSpeed));
            speedIncreased = false;
            print("Speed PowerUp Removed");
        }
        base.RemovePowerUp();
    }


}
