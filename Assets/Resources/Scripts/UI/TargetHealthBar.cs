using UnityEngine;
using UnityEngine.UI;

public class TargetHealthBar: MonoBehaviour
{
    //============//
    // Properties //
    //============//

    Stats stats;
    Text text;
    Slider slider;

    public CanvasGroup canvasGroup;

    //===========//
    // Functions //
    //===========//

    void Start()
    {
        text = GetComponentInChildren<Text>();
        slider = GetComponentInChildren<Slider>();
        DisconnectStats();
    }

    void Update()
    {
        if (stats != null)
        {
            slider.maxValue = stats.maxHealth;
            slider.value = stats.health;
            text.text = stats.health + " / " + stats.maxHealth;
        }
    }

    void OnEnable()
    {
        PlayerController.OnFocusTarget += ConnectStats;
        PlayerController.OnDefocusTarget += DisconnectStats;
    }

    void OnDisable()
    {
        PlayerController.OnFocusTarget -= ConnectStats;
        PlayerController.OnDefocusTarget -= DisconnectStats;
    }

    void ConnectStats(UnitController target)
    {
        stats = target.GetComponent<Stats>();
        canvasGroup.alpha = 1;
    }

    void DisconnectStats()
    {
        canvasGroup.alpha = 0;
        stats = null;
    }
}
