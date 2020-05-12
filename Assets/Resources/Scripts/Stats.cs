using UnityEngine;
using System;

public class Stats: MonoBehaviour
{
	// Properties //
	UnitAnim unitAnim;

    public int level = 1;
    public int exp = 0;
    public int expForLevelUp = 50;
    public int currentHealth = 70;
    public int maxHealth = 70;
    public int minDamage = 3;
    public int maxDamage = 5;

    public bool dead = false;

    // Events //
    public static event Action<Transform> OnUnitDied;
    public static event Action StatsUpdated;

    // Functions //
	void Start()
	{
		unitAnim = GetComponent<UnitAnim>();
	}

    public void DealDamage(UnitController source, int damage, bool randomLocation = false)
    {
        if (dead)
            return;

        if (currentHealth > damage)
        {
            currentHealth -= damage;
			unitAnim.Damaged(source, damage, randomLocation);
        }
        else
        {
            if (GetComponent<EnemyController>() == true)
            {
                Stats killerStats = source.GetComponent<Stats>();

                killerStats.AddExp(exp);
			}

            UnitDied();
        }
    }

    public bool Heal(int _health)
    {
        if (dead || _health < 1 || currentHealth == maxHealth)
            return false;

		int missingHealth = maxHealth - currentHealth;

		if (_health <= missingHealth)
		{
			currentHealth += _health;
			unitAnim.Heal(_health);
		}
		else
		{
			currentHealth += missingHealth;
			unitAnim.Heal(missingHealth);
		}

        return true;
    }

    void UnitDied()
    {
		unitAnim.UnitDied();

		currentHealth = 0;
        dead = true;
        UnitController unit = GetComponent<UnitController>();

        unit.weapon.GetComponent<Animator>().enabled = false;
        unit.weapon.transform.parent = null;

        unit.KillUnit();

        OnUnitDied?.Invoke(transform);
    }

    public void GetStats(ref String _level, out String _exp, out String _expForLevelUp, out String _health, out String _maxHealth, out String _minDamage, out String _maxDamage)
    {
        _level = level.ToString();
        _exp = exp.ToString();
        _expForLevelUp = expForLevelUp.ToString();
        _health = currentHealth.ToString();
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
        currentHealth = 0;
    }

    public void SetStatsForUnit(int _level = 1, int _exp = 0, int _expForLevelUp = 100, int _health = 50, int _maxHealth = 100, int _minDamage = 2, int _maxDamage = 5)
    {
        level = _level;
        exp = _exp;
        expForLevelUp = _expForLevelUp;
        currentHealth = _health;
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

    void AddExp(int amount)
    {
        exp += amount;
		unitAnim.AddExp(exp);

        if (exp >= expForLevelUp)
        {
            LevelUp();
        }
        else
        {
            StatsUpdated?.Invoke();
        }
    }

    void LevelUp()
    {
        ++level;
		unitAnim.LevelUp(level);

        exp = exp - expForLevelUp;
        expForLevelUp = (int)(1.2 * expForLevelUp);

        if (exp >= expForLevelUp)
        {
            LevelUp();
        }
        else
        {
            StatsUpdated?.Invoke();
        }
    }
}
