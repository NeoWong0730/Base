using Logic.Core;
using System;
using Table;

namespace Logic
{
    /// <summary>
    /// Open PetMessage And AddMountEnergy UI
    /// </summary>
    public class ChangeNameLimit : ItemUseBase
    {
        public ChangeNameLimit(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            //没配置(没有任何限制 直接走改名)
            if (_itemData.cSVItemData.fun_value == null)
            {
                UIManager.OpenUI(EUIID.UI_ReName);
                return true;
            }
            //只配置了改名等级
            if (_itemData.cSVItemData.fun_value.Count == 1)
            {
                uint reNameLv = _itemData.cSVItemData.fun_value[0];
                if (reNameLv == 0)
                {
                    UIManager.OpenUI(EUIID.UI_ReName);
                    return true;
                }
                else
                {
                    if (Sys_Role.Instance.Role.Level <= reNameLv)
                    {
                        UIManager.OpenUI(EUIID.UI_ReName);
                        return true;
                    }
                    else
                    {
                        string content = LanguageHelper.GetTextContent(2025130, reNameLv.ToString());
                        Sys_Hint.Instance.PushContent_Normal(content);
                        Sys_Bag.Instance.UseItemByUuid(_itemData.Uuid, 1);
                        Sys_Bag.Instance.useItemReq = true;
                        return true;
                    }
                }
            }
            else
            {
                uint itemLv = _itemData.cSVItemData.fun_value[1];
                if (itemLv == 0)
                {
                    UIManager.OpenUI(EUIID.UI_ReName);
                    return true;
                }
                if (Sys_Role.Instance.Role.Level > itemLv)
                {
                    string content = LanguageHelper.GetTextContent(2025130, itemLv.ToString());
                    Sys_Hint.Instance.PushContent_Normal(content);
                    Sys_Bag.Instance.UseItemByUuid(_itemData.Uuid, 1);
                    Sys_Bag.Instance.useItemReq = true;
                    return true;
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_ReName);
                    return true;
                }
            }
        }
    }
}


