using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupHealth : Powerup
{
    public override void Effect() 
    {
        GameManager.Instance.snake.IncreaseNodes(3);
    }
}
