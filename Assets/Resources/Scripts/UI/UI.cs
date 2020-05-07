using UnityEngine;
using UnityEngine.UI;

public class UI: MonoBehaviour
{
    // Properties //
    public Button wait;
    public Button skip;

    // Functions //
	void Start()
    {
        wait.onClick.AddListener(WaitTurn);
        skip.onClick.AddListener(EndTurn);
    }

    public void WaitTurn()
    {
        if (GameManager.instance.playerMove == false)
            return;

        GameManager.instance.currentUnit.GetComponent<PlayerController>().WaitTurn();
    }

    public void EndTurn()
    {
        if (GameManager.instance.playerMove == false)
            return;

        GameManager.instance.currentUnit.GetComponent<PlayerController>().EndTurn();
    }
}
