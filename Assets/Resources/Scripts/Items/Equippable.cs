using System;

public class Equippable: Item
{
    // Properties //
    bool isEquipped = false;

    public enum Type
    {
        None,
        Helmet,
        Body,
        Weapon
    }

    public Type type;

    // Events //
    public static event Action<Item, Item3D> OnItemEquiped;
    public static event Action<Item, Item3D> OnItemUnequiped;

    // Functions //
    public void EquipItem()
    {
        if (isEquipped == true)
            return;

        itemIn3D = Instantiate(itemIn3DPrefab);
        itemIn3D.transform.SetParent(GameManager.instance.currentUnit.transform, false);

        EquipItemEffect();
        isEquipped = false;

        OnItemEquiped(this, itemIn3D.GetComponent<Item3D>());
    }

    protected virtual void EquipItemEffect()
    {
        return;
    }

    public void UnequipItem()
    {
        if (isEquipped == false)
            return;

        UnequipItemEffect();
        isEquipped = true;

        OnItemUnequiped(this, itemIn3D.GetComponent<Item3D>());

        Destroy(itemIn3D);
    }

    protected virtual void UnequipItemEffect()
    {
        return;
    }
}
