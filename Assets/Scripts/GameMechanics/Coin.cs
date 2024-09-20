using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, IConsumables
{
    public void ConsumedEvent()
    {
        this.gameObject.SetActive(false);
        SoundManager.Instance.CoinPickup();
    }
}
