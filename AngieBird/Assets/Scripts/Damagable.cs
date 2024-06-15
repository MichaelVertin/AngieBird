using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 3f;
    [SerializeField] private float _fallDamageThreshold = 1f;
    [SerializeField] private float _collisionDamageMultiplier = 1f;

    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        _currentHealth -= damageAmount;

        if (_currentHealth <= 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactVelocity = collision.relativeVelocity.magnitude;
        float damage = impactVelocity * _collisionDamageMultiplier;

        if( collision.gameObject.GetComponent<AngieBird>() == null )
        {
            damage *= .25f;
        }

        if (damage > _fallDamageThreshold)
        {
            TakeDamage(damage);
        }
    }
}
