using UnityEngine;
using UnityEngine.EventSystems;

public class ConsumableItem: Item2D, IPointerDownHandler
{
    // Properties //
    float lastTimeClicked = 0;
    float doubleClickMaxSpread = 0.25f;

    public static int maxUses = 3;
    public int usesLeft;

    // Functions //
    protected override void Start()
    {
        base.Start();

        usesLeft = maxUses;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (lastTimeClicked == 0)
        {
            lastTimeClicked = Time.time;
            return;
        }

        if (Time.time - lastTimeClicked < doubleClickMaxSpread)
        {
            Use();
        }

        lastTimeClicked = Time.time;
    }

    public virtual bool Use()
    {
        if (GameManager.instance.playerMove == false)
            return false;

        if (UseEffect() == true)
        {
            --usesLeft;

            Equipment equipment = GameManager.instance.equipment;
            equipment.UpdateStatsDisplay();
        }

        if (usesLeft == 0)
        {
            GetComponentInParent<InventorySlot>().DisconnectItem();
            Destroy(gameObject);
        }

        return true;
    }

    protected virtual bool UseEffect()
    {
        Stats stats = GameManager.instance.currentUnit.GetComponent<Stats>();
        return stats.Heal(10);
    }
}
