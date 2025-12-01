using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FountainController : MonoBehaviour
{
    public void Interact(GameObject player)
    {
        player.GetComponent<PlayerHealth>().Heal(RollHealing());
        Destroy(gameObject.GetComponent<CircleCollider2D>());
        Destroy(this);
    }

    private float RollHealing() {return Mathf.Round(Random.Range(5f, 50f) * 100f) / 100f;}
}