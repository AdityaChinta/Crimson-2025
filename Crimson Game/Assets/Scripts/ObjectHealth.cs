using UnityEngine;

public class ObjectHealth
{
    public int currentHealth;
    public int maxHealth;

    public ObjectHealth(int health, int maximumHealth)
    {
        currentHealth = health;
        maxHealth = maximumHealth;
    }

    public void DealDamage(int amt)
    {
        if (currentHealth - amt > 0)
            currentHealth = currentHealth - amt;
        else
        {    // Trigger death animation 
        }

    }

    public void Heal(int amt)
    {
        currentHealth = currentHealth + amt;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    /*public int Health
    {
        get
        { return currentHealth; }

        set
        { currentHealth = value; }
    }*/
}
