using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{
	// Properties //
#pragma warning disable 0649

	[SerializeField] Object scene;

#pragma warning restore 0649

	// Functions //
	void OnTriggerEnter()
	{
		SaveSystem.Save();
		SceneManager.LoadScene(scene.name);
	}
}
