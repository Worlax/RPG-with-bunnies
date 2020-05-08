using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class Item: MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // Properties //
    CanvasGroup canvasGroup;

    public InventorySlot lastConnectedSlot;

    public GameObject itemIn3DPrefab;
    protected GameObject itemIn3D;

    public enum Type
    {
        None,
        Usable,
        Stackable,
        Equippable
    }

    public Type type;

    // usable
    static int maxUses;
    public static int MaxUses
    {
        get => maxUses;

        set
        {
            if (maxUses == 0) { maxUses = value; }
            else { print("Trying to set property twice"); }
        }
    }

    public int usesLeft;
    //

    // stackable
    Text inStackText;

    static int maxInStack;
    public static int MaxInStack
    {
        get => maxUses;

        set
        {
            if (maxInStack == 0) { maxInStack = value; }
            else { print("Trying to set property twice"); }
        }
    }

    public int inStack = 1;
    //

    // equippable
    bool isEquipped = false;

    public enum EquipType
    {
        None,
        Helmet,
        Body,
        Weapon
    }

    public EquipType equipType;
    //

    // Events //
    public static event Action<Item, Item3D> OnItemEquiped;
    public static event Action<Item, Item3D> OnItemUnequiped;

    // Functions //
    protected virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        GetComponentInParent<InventorySlot>().ConnectOrSwapItem(this, true);

        usesLeft = MaxUses;

        inStackText = GetComponentInChildren<Text>();
        inStackText.text = inStack.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
        transform.SetParent(GetComponentInParent<Canvas>().transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lastConnectedSlot.DisconnectItem();

        InventorySlot slot = IsMouseOverSlot();

        if (slot != null)
        {
            if (slot.ConnectOrSwapItem(this) == false)
            {
                lastConnectedSlot.ConnectOrSwapItem(this, true);
            }
        }
        else
        {
            lastConnectedSlot.ConnectOrSwapItem(this, true);
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    InventorySlot IsMouseOverSlot()
    {
        GraphicRaycaster graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
        EventSystem eventSystem = GetComponentInParent<EventSystem>();
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        graphicRaycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();

            if (slot != null)
            {
                return slot;
            }
        }

        return null;
    }

    public void Use()
    {
        if (type != Type.Usable)
            return;

        if (UseEffect() == true)
        {
            --usesLeft;

            if (usesLeft == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    protected virtual bool UseEffect()
    {
        return true;
    }

    void AddToStack(int amount)
    {
        if (type != Type.Stackable)
            return;

        inStack += amount;
        if (inStack > maxInStack)
        {
            inStack = maxInStack;
        }

        inStackText.text = inStack.ToString();
    }
    
    public void SubtractFromStack(int amount)
    {
        if (type != Type.Stackable)
            return;

        inStack -= amount;
        if (inStack <= 0)
        {
            Destroy(gameObject);
        }

        inStackText.text = inStack.ToString();
    }

    public void EquipItem()
    {
        if (type != Type.Equippable || equipType == EquipType.None || isEquipped == true)
            return;

        itemIn3D = Instantiate(itemIn3DPrefab);
        itemIn3D.transform.SetParent(GameManager.instance.currentUnit.transform, false);

        EquipItemEffect();
        OnItemEquiped(this, itemIn3D.GetComponent<Item3D>());
    }

    protected virtual void EquipItemEffect()
    {
        return;
    }

    public void UnequipItem()
    {
        if (type != Type.Equippable || equipType == EquipType.None || isEquipped == false)
            return;

        UnequipItemEffect();
        OnItemUnequiped(this, itemIn3D.GetComponent<Item3D>());

        Destroy(itemIn3D);
    }

    protected virtual void UnequipItemEffect()
    {
        return;
    }
}
