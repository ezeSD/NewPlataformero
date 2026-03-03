using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmunePowerUp : PowerUp
{
    public override void ApplyPowerUp()
    {
        if (IsActive == false)
            return;

        playerGet.SetInmune(true);
    }

    public override void RemovePowerUp()
    {
        playerGet.SetInmune(false);
        print("Immune PowerUp Removed");
        base.RemovePowerUp();
    }


}

