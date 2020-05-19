using UnityEngine;
using UnityEngine.UI;

public class TargetHealthBar: MonoBehaviour
{
    // Properties //
    Stats stats;
    Text text;
    Slider slider;

    public CanvasGroup canvasGroup;

    // Functions //
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
            slider.maxValue = stats.MaxHealth;
            slider.value = stats.CurrentHealth;
            text.text = stats.CurrentHealth + " / " + stats.MaxHealth;
        }
    }

    void OnEnable()
    {
        PlayerController.OnPlayerFocusedTarget += ConnectStats;
        PlayerController.OnPlayerDefocusedTarget += DisconnectStats;
    }

    void OnDisable()
    {
        PlayerController.OnPlayerFocusedTarget -= ConnectStats;
        PlayerController.OnPlayerDefocusedTarget -= DisconnectStats;
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
