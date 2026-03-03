using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPowerUp : PowerUp
{
    [SerializeField] float jumpMultiplier = 2.0f;
    bool isJumpEnhanced = false;
    public override void ApplyPowerUp()
    {

         if (IsActive == false)
             return;

         if (!isJumpEnhanced)
         {
            playerGet.SetJumpForce(playerGet.GetJumpForce() * jumpMultiplier);
             isJumpEnhanced = true;
         }
    }


    public override void RemovePowerUp()
    {
        if (isJumpEnhanced)
        {
            playerGet.SetJumpForce(playerGet.GetJumpForce() / jumpMultiplier);
            isJumpEnhanced = false;
            print("Jump PowerUp Removed");

        }
        base.RemovePowerUp();
    }



}
