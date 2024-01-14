using Logic;
using System.Collections.Generic;

namespace Table
{
    public partial class CSVShopItem
    {
        public List<uint> GetShopItemList(uint shopId)
        {
            List<uint> itemList = new List<uint>();

            int count = Count;
            for (int index = 0; index < count; index++)
            {
                var shopData = GetByIndex(index);

                if (shopData.shop_id == shopId)
                {
                    itemList.Add(shopData.id);
                }
            }

            return itemList;
        }
    }
}
