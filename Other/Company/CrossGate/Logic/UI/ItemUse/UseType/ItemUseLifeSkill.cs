using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 生活技能熟练度
    /// </summary>
    public class ItemUseLifeSkill : ItemUseBase
    {
        public ItemUseLifeSkill(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            EUIID eUIID = (EUIID)_itemData.cSVItemData.fun_value[0];
            uint skillId = _itemData.cSVItemData.fun_value[1];
            LifeSkillOpenParm lifeSkillOpenParm = new LifeSkillOpenParm();
            lifeSkillOpenParm.skillId = skillId;
            lifeSkillOpenParm.itemId = 0;
            UIManager.OpenUI(EUIID.UI_LifeSkill_Message, false, lifeSkillOpenParm);
            //UIManager.OpenUI(EUIID.UI_LifeSkill_Message, false, skillId);

            return true;
        }
    }
}


