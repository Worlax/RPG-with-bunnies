using UnityEngine;
using System;

public class Stats: MonoBehaviour
{
    // Properties //
    public int level = 1;
    public int exp = 0;
    public int expForLevelUp = 0;
    public int health = 70;
    public int maxHealth = 70;
    public int minDamage = 3;
    public int maxDamage = 5;

    public bool dead = false;

    Vector3 popupOffset = new Vector3(0f, 0f, 0.8f);

    // Events //
    public static event Action<Transform> OnUnitDied;

    // Functions //
    public void DealDamage(int damage, Transform source, bool randomLocation = false)
    {
        if (dead)
            return;

        if (health > damage)
        {
            health -= damage;

            Animator animator = GetComponent<Animator>();

            if (animator != null)
            {
                transform.LookAt(source);
                animator.Play("Damaged", 0, 0);
            }
        }
        else
        {
            UnitDied();
        }

        GameObject damagePopup = Instantiate(GameManager.instance.popupDamagePrefab);
        TextMesh textMesh = damagePopup.GetComponent<TextMesh>();

        textMesh.text = "-" + damage.ToString();

        float rand = UnityEngine.Random.Range(-0.5f, 0.5f);
        float rand2 = UnityEngine.Random.Range(-0.2f, 0.2f);
        Vector3 randomOffset = popupOffset + new Vector3(rand, 0, rand2);

        if (randomLocation == true)
        {
            damagePopup.transform.position = transform.position + randomOffset;
        }
        else
        {
            damagePopup.transform.position = transform.position + popupOffset;
        }
    }

    public bool Heal(int _health)
    {
        if (dead || _health < 1 || health == maxHealth)
            return false;

        health += _health;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        return true;
    }

    void UnitDied()
    {
        health = 0;
        dead = true;
        UnitController unit = GetComponent<UnitController>();

        if (unit is EnemyController)
        {
            ExpPopup();
        }

        unit.weapon.GetComponent<Animator>().enabled = false;
        unit.weapon.transform.parent = null;

        unit.KillUnit();

        OnUnitDied?.Invoke(transform);
    }

    void ExpPopup()
    {
        GameObject expPopup = Instantiate(GameManager.instance.popupExpPrefab);
        TextMesh textMesh = expPopup.GetComponentInChildren<TextMesh>();

        textMesh.text = "+" + exp.ToString() + " exp";

        expPopup.transform.position = transform.position + popupOffset;
    }

    public void GetStats(ref String _level, out String _exp, out String _expForLevelUp, out String _health, out String _maxHealth, out String _minDamage, out String _maxDamage)
    {
        _level = level.ToString();
        _exp = exp.ToString();
        _expForLevelUp = expForLevelUp.ToString();
        _health = health.ToString();
        _maxHealth = maxHealth.ToString();
        _minDamage = minDamage.ToString();
        _maxDamage = maxDamage.ToString();
    }

    public void SetStatsForItem(int _level = 1, int _maxHealth = 5, int _minDamage = 2, int _maxDamage = 3)
    {
        level = _level;
        maxHealth = _maxHealth;
        minDamage = _minDamage;
        maxDamage = _maxDamage;

        exp = 0;
        expForLevelUp = 0;
        health = 0;
    }

    public void SetStatsForUnit(int _level = 1, int _exp = 0, int _expForLevelUp = 100, int _health = 50, int _maxHealth = 100, int _minDamage = 2, int _maxDamage = 5)
    {
        level = _level;
        exp = _exp;
        expForLevelUp = _expForLevelUp;
        health = _health;
        maxHealth = _maxHealth;
        minDamage = _minDamage;
        maxDamage = _maxDamage;
    }

    public void AddStats(Stats a)
    {
        maxHealth += a.maxHealth;
        minDamage += a.minDamage;
        maxDamage += a.maxDamage;
    }

    public void AddStats(int _maxHealth = 0, int _minDamage = 0, int _maxDamage = 0)
    {
        maxHealth += _maxHealth;
        minDamage += _minDamage;
        maxDamage += _maxDamage;
    }

    public void SubtractStats(Stats a)
    {
        maxHealth -= a.maxHealth;
        minDamage -= a.minDamage;
        maxDamage -= a.maxDamage;
    }

    public void SubtractStats(int _maxHealth = 0, int _minDamage = 0, int _maxDamage = 0)
    {
        maxHealth -= _maxHealth;
        minDamage -= _minDamage;
        maxDamage -= _maxDamage;
    }
}
