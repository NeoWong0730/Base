using Logic.Core;
using Table;


namespace Logic
{
    /// <summary>
    /// 宠物改造道具使用
    /// </summary>
    public class ItemUsePassiveSkill : ItemUseBase
    {
        public ItemUsePassiveSkill(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            if (Sys_Cooking.Instance.usingCookings.ContainsKey(_itemData.cSVItemData.type_id))
            {
                Sys_Cooking.AttrCooking temp = Sys_Cooking.Instance.usingCookings[_itemData.cSVItemData.type_id];
                if (temp.b_Valid)
                {
                    PromptBoxParameter.Instance.OpenPromptBox(20003, 0, () => { Sys_Bag.Instance.UsetItem(_itemData.Id, _itemData.Uuid, 1); }, null);
                }
                else
                {
                    Sys_Bag.Instance.UsetItem(_itemData.Id, _itemData.Uuid, 1);
                }
            }
            else
            {
                Sys_Bag.Instance.UsetItem(_itemData.Id, _itemData.Uuid, 1);
            }
            return true;
        }
    }
}


