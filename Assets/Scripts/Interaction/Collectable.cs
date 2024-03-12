using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jam.Events;

public class Collectable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Destroy(gameObject);
        GameScoreEvent.InvokePlayerScored();
    }
}
