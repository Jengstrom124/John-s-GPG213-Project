using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;

    public event Action<GameObject> DeathEvent;
    
    public void Awake()
    {
        currentHealth = maxHealth;
    }

    public void ChangeHealth(float amount)
    {
        currentHealth += amount;

        if (currentHealth <= 0)
        {
            DeathEvent?.Invoke(gameObject);
        }
    }
}
