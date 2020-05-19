using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar: MonoBehaviour
{
    // Properties //
    Stats stats;
    Text text;
    Slider slider;

    // Functions //
    void Start()
    {
        text = GetComponentInChildren<Text>();
        slider = GetComponentInChildren<Slider>();
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
        UnitController.OnNewUnitTurn += ConnectStats;
    }

    void OnDisable()
    {
        UnitController.OnNewUnitTurn -= ConnectStats;
    }

    void ConnectStats(UnitController unit)
    {
        if (unit.tag != "Player")
            return;

        stats = GameManager.instance.currentUnit.GetComponent<Stats>();
    }
}
