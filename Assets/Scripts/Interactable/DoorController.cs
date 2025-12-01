using System;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public void Interact(GameObject player)
    {
        //if (player.GetComponent<PlayerController>().IsFacingRight()) transform.Rotate(0, 90, 0);
        //else transform.Rotate(0, -90, 0);
        transform.Rotate(0, player.GetComponent<PlayerController>().IsFacingRight() ? 90 : -90, 0);
        Destroy(gameObject.GetComponent<BoxCollider2D>());
        Destroy(this);
    }
}