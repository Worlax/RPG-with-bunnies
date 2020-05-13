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

    void UnitDied()
    {
		unitAnim.UnitDied();

		currentHealth = 0;
        dead = true;

        GetComponent<UnitController>().KillUnit();
        OnUnitDied?.Invoke(transform);
    }

    public void AddStats(int _maxHealth = 0, int _minDamage = 0, int _maxDamage = 0)
    {
        maxHealth += _maxHealth;
        minDamage += _minDamage;
        maxDamage += _maxDamage;
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
