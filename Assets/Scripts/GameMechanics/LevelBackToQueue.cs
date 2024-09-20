using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBackToQueue : MonoBehaviour
{
    private LevelGenerator _generator;

    private void Start()
    {
        _generator = GetComponentInParent<LevelGenerator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Regeneration"))
        {
            _generator.AddToPool(other.transform.parent.gameObject, other.GetComponentInParent<PlatformProperty>().type);
        }
    }
}
