using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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

    public class PlayerSoundEvent : MonoBehaviour
    {
        public delegate void OnFootStep(float amountMove, bool running);
        public static event OnFootStep onFootStep;

        public static void InvokeFootStepEvent(float amountMove, bool running)
        {
            if (onFootStep != null)
                onFootStep(amountMove, running);
        }
    }

    public class EnemyFocusEvent : MonoBehaviour
    {
        public delegate void OnEnemyFocus(float amount);
        public static event OnEnemyFocus EnemyFocus;

        public static void InvokeEnemyFocus(float amount)
        {
            if (EnemyFocus != null)
                EnemyFocus(amount);
        }

        public delegate void OnEnemyExitFocus();
        public static event OnEnemyExitFocus EnemyExitFocus;

        public static void InvokeEnemyExitFocus()
        {
            if (EnemyExitFocus != null)
                EnemyExitFocus();
        }
    }

    public class GameScoreEvent : MonoBehaviour
    {
        public delegate void OnPlayerScore();
        public static event OnPlayerScore PlayerScored;

        public static void InvokePlayerScored()
        {
            if (PlayerScored != null)
                PlayerScored();
        }
    }

    public class EnemyAttackEvent : MonoBehaviour
    {
        //OnEnemyAttack
        public delegate void OnEnemyAttack(Transform enemy);
        public static event OnEnemyAttack EnemyAttack;

        public static void InvokeEnemyAttack(Transform enemy)
        {
            if (EnemyAttack != null)
                EnemyAttack(enemy);
        }

        //OnGameOver
        public delegate void OnGameOver();
        public static event OnGameOver GameOver;

        public static void InvokeGameOver()
        {
            if (GameOver != null)
                GameOver();
        }
    }

}
