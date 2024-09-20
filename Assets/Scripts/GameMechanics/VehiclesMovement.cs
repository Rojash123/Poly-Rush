using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclesMovement : MonoBehaviour
{
    private Vector3 initialPos;
    private bool canMove;

    [SerializeField] private float speed;


    public void MoveVehicles()
    {
        canMove = true;
        GetComponent<AudioSource>().Play();
    }
    private void OnEnable()
    {
        initialPos = this.transform.localPosition;
        movePos = this.transform.localPosition;
    }
    private Vector3 movePos;
    private void FixedUpdate()
    {
        if (canMove)
        {
            movePos += new Vector3(0, 0, -speed) * Time.fixedDeltaTime;
            this.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, movePos.z);
        }
    }
    private void OnDisable()
    {
        canMove = false;
        this.transform.localPosition = initialPos;
        initialPos = Vector3.zero;
    }
}
