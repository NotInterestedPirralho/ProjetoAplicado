using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    public int dano = 10;


    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
