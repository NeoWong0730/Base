using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 技能经验道具
    /// </summary>
    public class ItemUsePetSkillExp : ItemUseBase
    {
        public ItemUsePetSkillExp(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            uint uiid = _itemData.cSVItemData.fun_value[0];
            MessageEx practiceEx = new MessageEx
            {
                messageState = (EPetMessageViewState)uiid
            };
            Sys_Pet.Instance.OnGetPetInfoReq(practiceEx, EPetUiType.UI_Message);
            return true;
        }
    }
}


