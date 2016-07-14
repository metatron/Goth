using UnityEngine;
using System.Collections;
using Soomla.Store;

public class ShopItemAssets : IStoreAssets {

	//Virtual Currency
	public const string SPIRIT_CURRENCY_ITEMID = "spirit_item";

	//Market Items
	public const string SPIRIT100PACK_ITEMID = "spirit_item_100";
	public const string SPIRIT_PRODUCTID_100 = "jp.arrow.angelforest.alptraum.spirit100";

	public const string SPIRIT500PACK_ITEMID = "spirit_item_500";
	public const string SPIRIT_PRODUCTID_500 = "jp.arrow.angelforest.alptraum.spirit500";

	public const string SPIRIT1000PACK_ITEMID = "spirit_item_1000";
	public const string SPIRIT_PRODUCTID_1000 = "jp.arrow.angelforest.alptraum.spirit1000";

	public const string SPIRIT2000PACK_ITEMID = "spirit_item_2000";
	public const string SPIRIT_PRODUCTID_2000 = "jp.arrow.angelforest.alptraum.spirit2000";

	public const string SPIRIT5000PACK_ITEMID = "spirit_item_5000";
	public const string SPIRIT_PRODUCTID_5000 = "jp.arrow.angelforest.alptraum.spirit5000";

	public const string SPIRIT10000PACK_ITEMID = "spirit_item_10000";
	public const string SPIRIT_PRODUCTID_10000 = "jp.arrow.angelforest.alptraum.spirit10000";


	//Virtual Goods (Usages)
	public const string NO_ADS_LIFETIME_PRODUCT_ID	= "NOADS_PRODUCT";
	public const string GHOSTUPGRADE				= "GHOSTUPGRADE";
	public const string GHOSTUPGRADE_RUMTIME		= "GHOSTUPGRADE_RUNTIME";

//	public const string SPIRIT_ITEMID_100	= "SPIRIT_100";
//	public const string SPIRIT_ITEMID_500	= "SPIRIT_500";
//	public const string SPIRIT_ITEMID_1000	= "SPIRIT_1000";
//	public const string SPIRIT_ITEMID_2000	= "SPIRIT_2000";
//	public const string SPIRIT_ITEMID_5000	= "SPIRIT_5000";


	public int GetVersion() {
		return 1;
	}

	public VirtualCurrency[] GetCurrencies() {
		return new VirtualCurrency[] {SpiritCurrency};
	}

	public VirtualCurrencyPack[] GetCurrencyPacks() {
		return new VirtualCurrencyPack[] {SpiritPack_100, SpiritPack_500, SpiritPack_1000, SpiritPack_2000, SpiritPack_5000, SpiritPack_10000};
	}

	public VirtualGood[] GetGoods() {
		return new VirtualGood[] { GhostUpgradeGood }; //Spirits_100, Spirits_500, Spirits_1000, Spirits_2000, Spirits_5000};
	}


	public VirtualCategory[] GetCategories() {
		return new VirtualCategory[] {};
	}

	/** Virtual Currencies **/

	public static VirtualCurrency SpiritCurrency = new VirtualCurrency(
		"Spirits",										// name
		"",											// description
		SPIRIT_CURRENCY_ITEMID						// item id
	);


	/** Virtual Currency Packs **/

	public static VirtualCurrencyPack SpiritPack_100 = new VirtualCurrencyPack(
		"100 Spirits",                                  	// name
		"100 Spirits",                     					// description
		SPIRIT100PACK_ITEMID,                           	// item id
		100,												// number of currencies in the pack
		SPIRIT_CURRENCY_ITEMID,                         	// the currency associated with this pack
		new PurchaseWithMarket(SPIRIT_PRODUCTID_100, 99)
	);

	public static VirtualCurrencyPack SpiritPack_500 = new VirtualCurrencyPack(
		"500 Spirits",                                  	// name
		"500 Spirits",                     					// description
		SPIRIT500PACK_ITEMID,                          	 	// item id
		500,												// number of currencies in the pack
		SPIRIT_CURRENCY_ITEMID,                         	// the currency associated with this pack
		new PurchaseWithMarket(SPIRIT_PRODUCTID_500, 450)
	);

	public static VirtualCurrencyPack SpiritPack_1000 = new VirtualCurrencyPack(
		"1000 Spirits",                                   	// name
		"1000 Spirits",                     				// description
		SPIRIT1000PACK_ITEMID,                           	// item id
		1000,												// number of currencies in the pack
		SPIRIT_CURRENCY_ITEMID,                         	// the currency associated with this pack
		new PurchaseWithMarket(SPIRIT_PRODUCTID_1000, 800)
	);

	public static VirtualCurrencyPack SpiritPack_2000 = new VirtualCurrencyPack(
		"2000 Spirits",                                   	// name
		"2000 Spirits",                     				// description
		SPIRIT2000PACK_ITEMID,                           	// item id
		2000,												// number of currencies in the pack
		SPIRIT_CURRENCY_ITEMID,                         	// the currency associated with this pack
		new PurchaseWithMarket(SPIRIT_PRODUCTID_2000, 1400)
	);

	public static VirtualCurrencyPack SpiritPack_5000 = new VirtualCurrencyPack(
		"5000 Spirits",                                   	// name
		"5000 Spirits",                     				// description
		SPIRIT5000PACK_ITEMID,                           	// item id
		5000,												// number of currencies in the pack
		SPIRIT_CURRENCY_ITEMID,                         	// the currency associated with this pack
		new PurchaseWithMarket(SPIRIT_PRODUCTID_5000, 3000)
	);

	public static VirtualCurrencyPack SpiritPack_10000 = new VirtualCurrencyPack(
		"10000 Spirits",                                   	// name
		"10000 Spirits",                     				// description
		SPIRIT10000PACK_ITEMID,                           	// item id
		10000,												// number of currencies in the pack
		SPIRIT_CURRENCY_ITEMID,                         	// the currency associated with this pack
		new PurchaseWithMarket(SPIRIT_PRODUCTID_10000, 5000)
	);

	/** LifeTimeVGs **/
	// Note: create non-consumable items using LifeTimeVG with PuchaseType of PurchaseWithMarket
	public static VirtualGood NO_ADS_LTVG = new LifetimeVG(
		"No Ads", 														// name
		"No More Ads!",				 									// description
		"no_ads",														// item id
		new PurchaseWithMarket(NO_ADS_LIFETIME_PRODUCT_ID, 200)			// the way this virtual good is purchased
	);


	/** Virtual Goods **/

//	public static VirtualGood GhostUpgradeGood = new SingleUseVG(
//		"Upgrading Ghosts", 	                                 		// name
//		"",																// description
//		GHOSTUPGRADE,	 	                                   		   	// item id
//		new PurchaseWithVirtualItem(SPIRIT_CURRENCY_ITEMID, 100)        // the way this virtual good is purchased (the price will be changed)
//	);

//	public static VirtualGood Spirits_100 = new SingleUseVG(
//		"Getting 100 Spirits",                                  	// name
//		"",															// description
//		SPIRIT_ITEMID_100,                                       	// item id
//		new PurchaseWithVirtualItem(SPIRIT_CURRENCY_ITEMID, 100)        // the way this virtual good is purchased
//	);
//
//	public static VirtualGood Spirits_500 = new SingleUseVG(
//		"Getting 500 Spirits",                                  	// name
//		"",															// description
//		SPIRIT_ITEMID_500,                                       	// item id
//		new PurchaseWithVirtualItem(SPIRIT_CURRENCY_ITEMID, 500)       // the way this virtual good is purchased
//	);
//
//	public static VirtualGood Spirits_1000 = new SingleUseVG(
//		"Getting 1000 Spirits",                                  	// name
//		"",															// description
//		SPIRIT_ITEMID_1000,                                       	// item id
//		new PurchaseWithVirtualItem(SPIRIT_CURRENCY_ITEMID, 1000)       // the way this virtual good is purchased
//	);
//
//	public static VirtualGood Spirits_2000 = new SingleUseVG(
//		"Getting 2000 Spirits",                                  	// name
//		"",															// description
//		SPIRIT_ITEMID_2000,                                       	// item id
//		new PurchaseWithVirtualItem(SPIRIT_CURRENCY_ITEMID, 2000)       // the way this virtual good is purchased
//	);
//
//	public static VirtualGood Spirits_5000 = new SingleUseVG(
//		"Getting 5000 Spirits",                                  	// name
//		"",															// description
//		SPIRIT_ITEMID_5000,                                       	// item id
//		new PurchaseWithVirtualItem(SPIRIT_CURRENCY_ITEMID, 5000)       // the way this virtual good is purchased
//	);
}