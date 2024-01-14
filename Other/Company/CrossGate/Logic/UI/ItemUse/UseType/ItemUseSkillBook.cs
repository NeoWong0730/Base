using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 技能书
    /// </summary>
    public class ItemUseSkillBook : ItemUseBase
    {
        public ItemUseSkillBook(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            uint uiid = _itemData.cSVItemData.fun_value[0];
            MessageEx practiceEx = new MessageEx();
            practiceEx.messageState = (EPetMessageViewState)uiid;
            if(_itemData.cSVItemData.type_id != 3015)
            {
                practiceEx.subPage = _itemData.cSVItemData.type_id == 3010 ? 0 : 1;
            }
            Sys_Pet.Instance.OnGetPetInfoReq(practiceEx, EPetUiType.UI_Message);
            return true;
        }
    }
}


