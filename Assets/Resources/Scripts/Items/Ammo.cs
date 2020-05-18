using UnityEngine;

public class Ammo: Stackable
{
	// Properties //
	public enum AmmoType
	{
		A762x51mm,
		A556x45mm,
		A9mm
	}

	[SerializeField]
	AmmoType _type;
	public AmmoType Type { get => _type; private set => _type = value; }

	static int maxInStack_A762x39mm = 20;
	static int maxInStack_A545x39mm = 30;
	static int maxInStack_A9mm = 50;

	// Functions //
	protected override void Start()
	{
		base.Start();

		SetMaxInStack();
	}

	void SetMaxInStack()
	{
		switch (Type)
		{
			case AmmoType.A762x51mm:
				maxInStack = maxInStack_A762x39mm;
				break;

			case AmmoType.A556x45mm:
				maxInStack = maxInStack_A545x39mm;
				break;

			case AmmoType.A9mm:
				maxInStack = maxInStack_A9mm;
				break;
		}
	}
}