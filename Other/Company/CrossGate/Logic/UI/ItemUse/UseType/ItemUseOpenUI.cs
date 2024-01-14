using Logic.Core;
using Table;

namespace Logic
{
    /// <summary>
    /// 打开UI
    /// </summary>
    public class ItemUseOpenUI : ItemUseBase
    {
        public ItemUseOpenUI(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            uint uiid = _itemData.cSVItemData.fun_value[0];
            switch (uiid)
            {
                case (uint)EUIID.UI_Equipment:
                    {
                        Sys_Equip.EquipmentOperations opType = Sys_Equip.EquipmentOperations.Inlay;
                        if (_itemData.cSVItemData.fun_value.Count > 1)
                            opType = (Sys_Equip.EquipmentOperations)_itemData.cSVItemData.fun_value[1];

                        if (!Sys_FunctionOpen.Instance.IsOpen(10300 + (uint)opType, true))
                            return false;

                        Sys_Equip.UIEquipPrama data = new Sys_Equip.UIEquipPrama();
                        data.opType = opType;
                        UIManager.OpenUI(EUIID.UI_Equipment, false, data);
                    }
                    break;
                case (uint)EUIID.UI_Horn:
                    {
                        Sys_Chat.Instance.SelectedHorn(_itemData.cSVItemData);
                        UIManager.OpenUI((EUIID)uiid);
                    }
                    break;
                case (uint)EUIID.UI_Lotto:
                    {
                        if (Sys_OperationalActivity.Instance.CheckLotteryActivityIsOpen())
                        {
                            UIManager.OpenUI(EUIID.UI_Lotto);
                        }
                        else
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021134));
                        }
                    }
                    break;
                case (uint)EUIID.UI_Fashion:
                    {
                        if (Sys_FunctionOpen.Instance.IsOpen(10400))
                        {
                            UIManager.OpenUI(EUIID.UI_Fashion);
                        }
                        else
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002125));
                        }
                    }
                    break;
                case (uint)EUIID.UI_Transfiguration_Study:
                    {
                        if (Sys_FunctionOpen.Instance.IsOpen(50110))
                        {
                            UIManager.OpenUI(EUIID.UI_Transfiguration_Study, false, _itemData.cSVItemData.fun_value[1]);
                        }
                        else
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002125));
                        }
                    }
                    break;
                default:
                    UIManager.OpenUI((EUIID)uiid);
                    break;

            }
            return true;
        }
    }
}


