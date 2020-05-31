using UnityEngine;
using UnityEngine.UI;

public class Shop: Inventory
{
	// Properties //
#pragma warning disable 0649

	[SerializeField] Button trade;
	[SerializeField] Text tradeMoneyText;
	[SerializeField] RectTransform buySlotsRoot;
	[SerializeField] RectTransform sellSlotsRoot;

#pragma warning restore 0649

	int moneyForTrade = 0;

	// Functions //
	protected override void Update()
	{
		base.Update();

		if (Closed == false)
		{
			SetPriceForAllItems();
		}
	}

	protected override void Start()
	{
		base.Start();

		trade.onClick.AddListener(Trade);
	}

	void Trade()
	{
		Player player = GameManager.instance.CurrenPlayer;

		if (moneyForTrade < 0)
		{
			int money = moneyForTrade * -1;

			if (player.Money >= money)
			{
				player.TakeMoney(money);
				Owner.AddMoney(money);
			}
			else
			{
				return;
			}
		}
		else if (moneyForTrade > 0)
		{
			if (Owner.Money >= moneyForTrade)
			{
				Owner.TakeMoney(moneyForTrade);
				player.AddMoney(moneyForTrade);
			}
			else
			{
				return;
			}
		}

		Inventory playerInventory = player.Inventory;
		Item[] itemsToBuy = GetAllItems(buySlotsRoot);
		Item[] itemsToSell = GetAllItems(sellSlotsRoot);

		if (playerInventory.TryToConnectItems(itemsToBuy) == true)
		{
			TryToConnectItems(itemsToSell);
		}
	}

	void SetPriceForAllItems()
	{
		moneyForTrade = 0;

		foreach (Item item in GetAllItems(buySlotsRoot))
		{
			moneyForTrade -= item.Price;
		}
		foreach (Item item in GetAllItems(sellSlotsRoot))
		{
			moneyForTrade += item.Price;
		}

		UpdateMoneyForTradeDisplay();
	}

	void UpdateMoneyForTradeDisplay()
	{
		string display = moneyForTrade.ToString();

		for (int i = display.Length, j = 0; i > 0; --i, ++j)
		{
			if (j != 0 && j % 3 == 0)
			{
				display = display.Insert(i, " ");
			}
		}

		tradeMoneyText.text = display;
	}
}
