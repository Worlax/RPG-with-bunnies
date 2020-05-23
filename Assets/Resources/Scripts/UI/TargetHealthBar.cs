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
        Player.OnPlayerFocusedTarget += ConnectStats;
        Player.OnPlayerDefocusedTarget += DisconnectStats;
    }

    void OnDisable()
    {
        Player.OnPlayerFocusedTarget -= ConnectStats;
        Player.OnPlayerDefocusedTarget -= DisconnectStats;
    }

    void ConnectStats(Unit target)
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
