using Logic.Core;
using Table;


namespace Logic
{
    /// <summary>
    /// 符文附魔
    /// </summary>
    public class ItemUseSuccessRate : ItemUseBase
    {
        public ItemUseSuccessRate(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            uint lifeSkillId = _itemData.cSVItemData.fun_value[1];
            LifeSkillOpenParm lifeSkillOpenParm = new LifeSkillOpenParm();
            lifeSkillOpenParm.skillId = lifeSkillId;
            lifeSkillOpenParm.itemId = 0;
            UIManager.OpenUI(EUIID.UI_LifeSkill_Message, false, lifeSkillOpenParm);
            return true;
        }
    }
}


