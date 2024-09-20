using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMovingVehicles : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<LevelBackToQueue>() != null)
        {
            var nice = other.GetComponentInParent<PlatformProperty>();
            nice.MoveAllVehicles();
            
        }
    }
}
