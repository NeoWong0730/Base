using Logic.Core;
using Table;
using System.Collections.Generic;

namespace Logic
{
    public class ItemUseOpenRelationUI : ItemUseBase
    {
        private List<uint> relationLists = new List<uint>();

        public ItemUseOpenRelationUI(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            relationLists.Clear();
            uint relationId = _itemData.cSVItemData.fun_value[0];
            CSVUiRelation.Data cSVUiRelationData = CSVUiRelation.Instance.GetConfData(relationId);
            GetRelations(cSVUiRelationData);
            OpenUI();
            return true;
        }

        private void GetRelations(CSVUiRelation.Data cSVUiRelationData)
        {
            relationLists.Add(cSVUiRelationData.id);
            if (cSVUiRelationData.relationUiid == 0)
            {
                return;
            }
            uint relationId = cSVUiRelationData.relationUiid;
            CSVUiRelation.Data cSV = CSVUiRelation.Instance.GetConfData(relationId);
            GetRelations(cSV);
        }

        private void OpenUI()
        {
            for (int i = relationLists.Count - 1; i >= 0; --i)
            {
                CSVUiRelation.Data cSVUiRelationData = CSVUiRelation.Instance.GetConfData(relationLists[i]);
                if (!Sys_FunctionOpen.Instance.IsOpen(cSVUiRelationData.functionID, true))
                {
                    continue;
                }
                uint uiId = CSVUiRelation.Instance.GetConfData(relationLists[i]).UIID;
                int funParm = (int)CSVUiRelation.Instance.GetConfData(relationLists[i]).functionNum[0];
                if (funParm != 0)
                {
                    if (uiId == 58) //技能界面
                    {
                        UIManager.OpenUI((EUIID)uiId, true, new List<int>() { funParm });
                    }
                    else
                    {
                        UIManager.OpenUI((EUIID)uiId, true, funParm);
                    }
                }
                else
                {
                    UIManager.OpenUI((EUIID)uiId);
                }
            }
        }
    }
}


