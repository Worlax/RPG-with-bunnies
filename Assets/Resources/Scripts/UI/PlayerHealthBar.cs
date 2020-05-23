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
        Unit.OnNewUnitTurn += ConnectStats;
    }

    void OnDisable()
    {
        Unit.OnNewUnitTurn -= ConnectStats;
    }

    void ConnectStats(Unit unit)
    {
        if (unit.tag != "Player")
            return;

        stats = BattleManager.instance.CurrentUnit.GetComponent<Stats>();
    }
}
