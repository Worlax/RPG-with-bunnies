using UnityEngine;
using System;

public class Stats: MonoBehaviour
{
	// Properties //
	UnitAnim unitAnim;

	[SerializeField] int _currentHealth = 70;
	[SerializeField] int _maxHealth = 70;
	[SerializeField] int _minDamage = 3;
	[SerializeField] int _maxDamage = 5;
	[SerializeField] int _playingTurnSpeed = 2;
	[SerializeField] int _level = 1;
	[SerializeField] int _exp = 0;
	[SerializeField] int _expForLevelUp = 50;

	public int Level { get => _level; private set => _level = value; }
	public int CurrentHealth { get => _currentHealth; private set => _currentHealth = value; }
	public int MaxHealth { get => _maxHealth; private set => _maxHealth = value; }
	public int MinDamage { get => _minDamage; private set => _minDamage = value; }
	public int MaxDamage { get => _maxDamage; private set => _maxDamage = value; }
	public int PlayingTurnSpeed { get => _playingTurnSpeed; private set => _playingTurnSpeed = value; }
	public int Exp { get => _exp; private set => _exp = value; }
	public int ExpForLevelUp { get => _expForLevelUp; private set => _expForLevelUp = value; }

	[ReadOnly][SerializeField] bool _dead = false;
	public bool Dead { get => _dead; private set => _dead = value; }

    // Events //
    public static event Action<Transform> OnUnitDied;
    public static event Action StatsUpdated;

    // Functions //
	void Awake()
	{
		unitAnim = GetComponent<UnitAnim>();
	}

	public bool Heal(int _health)
	{
		if (Dead || _health < 1 || CurrentHealth == MaxHealth)
			return false;

		int missingHealth = MaxHealth - CurrentHealth;

		if (_health <= missingHealth)
		{
			CurrentHealth += _health;
			unitAnim.Heal(_health);
		}
		else
		{
			CurrentHealth += missingHealth;
			unitAnim.Heal(missingHealth);
		}

		return true;
	}

	public void DealDamage(UnitController source, int damage, bool randomLocation = false)
    {
        if (Dead)
            return;

		unitAnim.Damaged(source, damage, randomLocation);

		if (CurrentHealth > damage)
        {
            CurrentHealth -= damage;
        }
        else
        {
            if (GetComponent<EnemyController>() == true)
            {
                Stats killerStats = source.GetComponent<Stats>();

                killerStats.AddExp(Exp);
			}

            UnitDied();
        }
    }

    void UnitDied()
    {
		unitAnim.UnitDied();

		CurrentHealth = 0;
        Dead = true;

        GetComponent<UnitController>().KillUnit();
        OnUnitDied?.Invoke(transform);
    }

    public void AddStats(int _maxHealth = 0, int _minDamage = 0, int _maxDamage = 0)
    {
        MaxHealth += _maxHealth;
        MinDamage += _minDamage;
        MaxDamage += _maxDamage;
    }

    public void SubtractStats(int _maxHealth = 0, int _minDamage = 0, int _maxDamage = 0)
    {
        MaxHealth -= _maxHealth;
        MinDamage -= _minDamage;
        MaxDamage -= _maxDamage;
    }

    void AddExp(int amount)
    {
        Exp += amount;
		unitAnim.AddExp(amount);

        if (Exp >= ExpForLevelUp)
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
        ++Level;
		unitAnim.LevelUp(Level);

        Exp = Exp - ExpForLevelUp;
        ExpForLevelUp = (int)(1.2 * ExpForLevelUp);

        if (Exp >= ExpForLevelUp)
        {
            LevelUp();
        }
        else
        {
            StatsUpdated?.Invoke();
        }
    }
}
