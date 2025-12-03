using UnityEngine;

public class NotEnoughShards : MonoBehaviour
{
    private bool on = true;
    private GameObject nes;

    void Start()
    {
        nes = GameObject.Find("Not_Enough_Shards");
        ToggleDialogue();
    }

    public void ToggleDialogue()
    {
        on = !on;
        nes.SetActive(on);
    }
}
