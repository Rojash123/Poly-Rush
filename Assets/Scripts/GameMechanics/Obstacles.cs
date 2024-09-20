using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour , IConsumables
{
    public void ConsumedEvent()
    {
        if(!PlayerController.Instance.isInvincible)
            UIManager.Instance.GameOver();
    }
}
public interface IConsumables
{
    public void ConsumedEvent();
}
