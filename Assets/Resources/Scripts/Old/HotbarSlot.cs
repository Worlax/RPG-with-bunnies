using UnityEngine;
using UnityEngine.EventSystems;

public class HotbarSlot: InventorySlot, IPointerDownHandler
{
    // Properties //
    public GameObject hotbarItemPrefab;
    HotbarItem hotbarItem;

    // Functions //
    public override bool ConnectOrSwapItem(Item2D item, bool ignoreMoveTurn = true)
    {
        if (ignoreMoveTurn == false && GameManager.instance.playerMove == false)
        {
            return false;
        }

        ConnectItem(item);

        return true;
    }

    protected override void ConnectItem(Item2D item)
    {
        hotbarItem = Instantiate(hotbarItemPrefab).GetComponent<HotbarItem>();

        itemInSlot = item;
        itemInSlot.SetLastConnectedSlot(this);
        itemInSlot.transform.SetParent(transform);
        itemInSlot.transform.localPosition = Vector3.zero;
    }

    public override void DisconnectItem()
    {
        itemInSlot.transform.SetParent(GetComponentInParent<Canvas>().transform);
        itemInSlot = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // itemInSlot.Use();
    }
}
