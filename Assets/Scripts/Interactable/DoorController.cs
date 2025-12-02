using System;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public void Interact(GameObject player)
    {
        transform.Rotate(0, player.GetComponent<PlayerController>().IsFacingRight() ? 90 : -90, 0);
        Destroy(gameObject.GetComponent<BoxCollider2D>());
        Destroy(this);
    }
}