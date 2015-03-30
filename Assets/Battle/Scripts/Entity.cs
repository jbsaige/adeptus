using UnityEngine;
using System.Collections;
using System;

public class Entity : MonoBehaviour 
{
    public float health;

    public virtual void TakeDamage(float dmg)
    {
        Debug.Log("Taking damage: " + dmg.ToString());
        health -= dmg;

        Debug.Log(health);

        if (health <= 0)
        {
            Debug.Log("About to call Die");
            System.Threading.Thread.Sleep(50);
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log("Dead");
        Destroy(gameObject);
    }
}
