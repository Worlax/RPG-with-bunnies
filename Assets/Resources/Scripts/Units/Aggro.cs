using UnityEngine;
using System.Collections.Generic;

public class Aggro: MonoBehaviour
{
	// Properties //
	Enemy me;
	List<Enemy> nearEnemys;
	
    // Functions //
	void Awake()
	{
		me = GetComponentInParent<Enemy>();
		nearEnemys = new List<Enemy>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (!me.InBattle && GetComponentInParent<Stats>().Dead == false)
		{
			if (other.tag == "Player")
			{
				Player player = other.GetComponentInParent<Player>();

				if (player.InBattle)
				{
					BattleManager.instance.AddUnitsToBattle(me);
				}
				else
				{
					List<Unit> unitsToBattle = new List<Unit>(nearEnemys);
					unitsToBattle.Add(player);

					BattleManager.instance.StartBattle(me, unitsToBattle.ToArray());
				}
			}
			else if (other.tag == "Enemy")
			{
				nearEnemys.Add(other.GetComponentInParent<Enemy>());
			}
		}
		else if (me.InBattle)
		{
			if (other.tag == "Enemy")
			{
				Enemy otherEnemy = other.GetComponentInParent<Enemy>();

				if (otherEnemy.InBattle == false)
				{
					BattleManager.instance.AddUnitsToBattle(otherEnemy);
				}
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (!me.InBattle)
		{
			if (other.tag == "Enemy")
			{
				Enemy otherEnemy = other.GetComponentInParent<Enemy>();
				nearEnemys.Remove(otherEnemy);
			}
		}
	}

	public void AddNearEnemysToBattle()
	{
		BattleManager.instance.AddUnitsToBattle(nearEnemys.ToArray());
	}
}
