using UnityEngine;

public class SaveManager: MonoBehaviour
{
	// Singleton //
	public static SaveManager instance = null;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Properties //


	// Functions //
	void Start()
	{
		SaveSystem.Load();
		print("loaded!");
	}
}