using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum levelTypes
{
    Easy,
    Medium,
}

public class PlatformProperty : MonoBehaviour
{
    public levelTypes type;

    public List<VehiclesMovement> movingObstacles;

    public List<Coin> coinsInPlatform;

    public void MoveAllVehicles()
    {
        foreach (var item in movingObstacles)
        {
            item.MoveVehicles();
        }
    }
    public void ActivateCoin()
    {
        foreach (var item in coinsInPlatform)
        {
            item.gameObject.SetActive(true);
        }
    }

    public void DeactivateCoin()
    {
        foreach (var item in coinsInPlatform)
        {
            item.gameObject.SetActive(false);
        }
    }

}
