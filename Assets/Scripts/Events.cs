using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Jam.Events
{
    public class StaminaChangeEvent : MonoBehaviour
    {
        public delegate void OnStaminaChange(float newStamina, bool isCooldown);
        public static event OnStaminaChange StaminaChanged;

        public static void InvokeStaminaChange(float newStamina, bool isCooldown)
        {
            if (StaminaChanged != null)
                StaminaChanged(newStamina, isCooldown);
        }
    }

}
