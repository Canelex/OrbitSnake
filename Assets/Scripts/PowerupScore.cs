using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupScore : Powerup
{
    public override void Effect() 
    {
        GameManager.Instance.score += 30;
    }
}
