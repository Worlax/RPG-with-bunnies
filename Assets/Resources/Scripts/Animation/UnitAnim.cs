using UnityEngine;

public class UnitAnim: Animated
{
	// Properties //
	public GameObject popupDamagePrefab;
	public GameObject popupExpPrefab;

	Vector3 popupOffset = new Vector3(0f, 0f, 0.8f);

	// Functions //
	void Start()
	{
		animator.SetBool("idle", true);
	}

	public void Idle(bool isIdling)
	{
		animator.SetBool("idle", isIdling);
	}

	public void Heal(int amount)
	{

	}

	public void Damaged(UnitController source, int damage, bool randomLocation = false)
	{
		transform.LookAt(source.transform);
		animator.Play("Damaged", 0, 0);

		GameObject damagePopup = Instantiate(popupDamagePrefab);
		TextMesh textMesh = damagePopup.GetComponent<TextMesh>();

		textMesh.text = "-" + damage.ToString();

		if (randomLocation == false)
		{
			damagePopup.transform.position = transform.position + popupOffset;
		}
		else
		{
			float rand = UnityEngine.Random.Range(-0.5f, 0.5f);
			float rand2 = UnityEngine.Random.Range(-0.2f, 0.2f);
			Vector3 randomOffset = popupOffset + new Vector3(rand, 0, rand2);

			damagePopup.transform.position = transform.position + randomOffset;
		}
	}

	public void AddExp(int amount)
	{
		GameObject expPopup = Instantiate(popupExpPrefab);
		TextMesh textMesh = expPopup.GetComponentInChildren<TextMesh>();

		textMesh.text = "+" + amount.ToString() + " exp";

		expPopup.transform.position = transform.position + popupOffset;
	}

	public void LevelUp(int newLevel)
	{

	}

	public void UnitDied()
	{
		animator.SetBool("idle", false);

		animator.Play("Death", 0, 0);
	}
}
