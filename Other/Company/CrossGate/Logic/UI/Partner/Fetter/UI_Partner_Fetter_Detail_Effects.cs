using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Table;
using Packet;
using Logic.Core;
using Framework;

namespace Logic
{
    //羁绊组合激活条件基类
    public class FetterEffectActiveBase
    {
        protected uint mbondId;
        protected CSVBond.Data mBondData;
        public virtual uint GetNum() { return 0; }
        public void Init(uint bondId)
        {
            this.mbondId = bondId;
            this.mBondData = CSVBond.Instance.GetConfData(this.mbondId);
        }
    }

    //羁绊组合激活条件-解锁伙伴
    public class FetterEffectActiveUnlock : FetterEffectActiveBase
    {
        public override uint GetNum()
        {
            uint count = 0;
            for (int i = 0; i < mBondData.group_partnerID.Count; ++i)
            {
                if (Sys_Partner.Instance.IsUnLock(mBondData.group_partnerID[i]))
                    count++;
            }

            return count;
        }
    }

    //羁绊组合激活条件-符文等级
    public class FetterEffectActiveRuneLevel : FetterEffectActiveBase
    {
        public override uint GetNum()
        {
            uint count = 0;
            for (int i = 0; i < mBondData.group_partnerID.Count; ++i)
            {
                Partner paInfo = Sys_Partner.Instance.GetPartnerInfo(mBondData.group_partnerID[i]);
                if (paInfo != null)
                {
                    count += paInfo.TotalRuneLv;
                }
            }

            return count;
        }
    }

    //羁绊组合激活条件-金符文数量
    public class FetterEffectActiveRuneGoldNum : FetterEffectActiveBase
    {
        public override uint GetNum()
        {
            uint count = 0;
            for (int i = 0; i < mBondData.group_partnerID.Count; ++i)
            {
                Partner paInfo = Sys_Partner.Instance.GetPartnerInfo(mBondData.group_partnerID[i]);
                if (paInfo != null)
                {
                    count += paInfo.TotalGoldRune;
                }
            }

            return count;
        }
    }
    
    //羁绊组合激活条件-投入属性强化点数
    public class FetterEffectActiveUsePointNum : FetterEffectActiveBase
    {
        public override uint GetNum()
        {
            uint count = 0;
            for (int i = 0; i < mBondData.group_partnerID.Count; ++i)
            {
                Partner paInfo = Sys_Partner.Instance.GetPartnerInfo(mBondData.group_partnerID[i]);
                if (paInfo != null && paInfo.ImproveData != null)
                {
                    count += paInfo.ImproveData.UsePoint;
                }
            }

            return count;
        }
    }

    public class FetterEffectCondition
    {
        public FetterEffectActiveBase condBase;
        public void Init(uint typeId, uint bondId)
        {
            switch(typeId)
            {
                case 1:
                    condBase = new FetterEffectActiveUnlock();
                    condBase.Init(bondId);
                    break;
                case 2:
                    condBase = new FetterEffectActiveRuneLevel();
                    condBase.Init(bondId);
                    break;
                case 3:
                    condBase = new FetterEffectActiveRuneGoldNum();
                    condBase.Init(bondId);
                    break;
                case 4:
                    condBase = new FetterEffectActiveUsePointNum();
                    condBase.Init(bondId);
                    break;
            }
        }

        public uint ActiveNum 
        {
            get
            {
                if (condBase != null)
                   return condBase.GetNum();
                else
                    return 0;
            }
        }
    }

    public class UI_Partner_Fetter_Detail_Effects
    {
        public class EffectCell
        {
            public class EffectCondition
            {
                private Transform transform;
                private GameObject goTemplate;

                public void Init(Transform trans)
                {
                    transform = trans;

                    goTemplate = transform.Find("Item").gameObject;
                    goTemplate.SetActive(false);
                }

                public void UpdateInfo(uint bondId, uint effectId, bool isActive)
                {
                    CSVBond.Data bondData = CSVBond.Instance.GetConfData(bondId);
                    CSVBondEffect.Data effectData = CSVBondEffect.Instance.GetConfData(effectId);

                    Lib.Core.FrameworkTool.DestroyChildren(goTemplate.transform.parent.gameObject, goTemplate.name);

                    int count = effectData.group_activeinfo.Count;
                    for(int i = 0; i < count; ++i)
                    {
                        GameObject go = Lib.Core.FrameworkTool.CreateGameObject(goTemplate, goTemplate.transform.parent.gameObject);
                        go.name = string.Format("{0}{1}", go.name, "(Clone)");
                        go.gameObject.SetActive(true);

                        uint typeValue = effectData.group_active[i][0];
                        uint paraValue = effectData.group_active[i][1];
                        uint condName = effectData.group_activeinfo[i];

                        uint achiveValue = 0;
                        FetterEffectCondition cond = new FetterEffectCondition();
                        cond.Init(typeValue, bondId);
                        achiveValue = cond.ActiveNum;

                        go.GetComponent<Text>().text = LanguageHelper.GetTextContent(condName, achiveValue.ToString(), paraValue.ToString());

                        ImageHelper.SetImageGray(go.transform, !isActive, true);
                    }
                }
            }

            public class EffectResult
            {
                private Transform transform;
                private GameObject goTemplate;

                public void Init(Transform trans)
                {
                    transform = trans;

                    goTemplate = transform.Find("Item").gameObject;
                    goTemplate.gameObject.SetActive(false);
                }

                public void UpdateInfo(uint bondId, uint effectId, bool isActive)
                {
                    //CSVBondData bondData = CSVBond.Instance.GetConfData(bondId);
                    CSVBondEffect.Data effectData = CSVBondEffect.Instance.GetConfData(effectId);

                    Lib.Core.FrameworkTool.DestroyChildren(goTemplate.transform.parent.gameObject, goTemplate.name);

                    int count = effectData.effective.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        GameObject go = Lib.Core.FrameworkTool.CreateGameObject(goTemplate, goTemplate.transform.parent.gameObject);
                        go.name = string.Format("{0}{1}", go.name, "(Clone)");
                        go.gameObject.SetActive(true);

                        Text txtAttrName = go.transform.Find("Text_Name").GetComponent<Text>();
                        Text txtArrValue = go.transform.Find("Text_Name/Text_Value").GetComponent<Text>();

                        uint attrId = effectData.effective[i][0];
                        CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrId);

                        txtAttrName.text = LanguageHelper.GetTextContent(attrData.name);
                        txtArrValue.text = string.Format("+{0}", Sys_Attr.Instance.GetAttrValue(attrData, effectData.effective[i][1]));

                        ImageHelper.SetImageGray(go.transform, !isActive, true);
                    }
                }
            }

            private Transform transform;

            private Text txtTitleName;
            private EffectCondition effectCondition;
            private EffectResult effectResult;
            private Text txtState;

            //private uint mbondId;
            //private uint mEffectId;
            //private int mIndex;

            public void Init(Transform trans)
            {
                transform = trans;

                txtTitleName = transform.Find("Title/Text_Name").GetComponent<Text>();

                effectCondition = new EffectCondition();
                effectCondition.Init(transform.Find("Condition"));

                effectResult = new EffectResult();
                effectResult.Init(transform.Find("Result"));

                txtState = transform.Find("State/Text").GetComponent<Text>();
            }

            public void UpdateInfo(uint bondId, uint effectId, int index)
            {
                PartnerBond parBond = Sys_Partner.Instance.GetBondData(bondId);
                bool active = parBond.Index >= index + 1;
                bool preActive = parBond.Index >= index;

                CSVBond.Data bondData = CSVBond.Instance.GetConfData(bondId);
                CSVBondEffect.Data effectData = CSVBondEffect.Instance.GetConfData(effectId);

                txtTitleName.text = LanguageHelper.GetTextContent(2006081, LanguageHelper.GetTextContent(effectData.name), (index + 1).ToString());
                ImageHelper.SetImageGray(txtTitleName.transform, !active, true);

                effectCondition.UpdateInfo(bondId, effectId, active);
                effectResult.UpdateInfo(bondId, effectId, active);

                uint lanId = 2006084u; //已激活
                if (!active)
                {
                    if (preActive)
                        lanId = 2006085u; //激活上一级解锁
                    else
                        lanId = 2006086u; //未激活
                }

                if (lanId == 2006086u) //红色
                {
                    TextHelper.SetText(txtState, LanguageHelper.GetTextContent(lanId), LanguageHelper.GetTextStyle(134));
                }
                else
                {
                    ImageHelper.SetImageGray(txtState.transform, !active, true);
                    uint styleId = active ? 132u : 133u;
                    TextHelper.SetText(txtState, LanguageHelper.GetTextContent(lanId), LanguageHelper.GetTextStyle(styleId));
                }
            }
        }

        private Transform transform;

        private GameObject goTemplate;

        private List<uint> listIds = new List<uint>();
        private List<EffectCell> listEffects = new List<EffectCell>();

        public void Init(Transform trans)
        {
            transform = trans;

            goTemplate = transform.Find("View_Result/Scroll View/Viewport/Content/Item").gameObject;
            goTemplate.gameObject.SetActive(false);
        }

        public void UpdateInfo(uint bondId)
        {
            listEffects.Clear();
            listIds.Clear();
            listIds.AddRange(CSVBond.Instance.GetConfData(bondId).group_effect);

            Lib.Core.FrameworkTool.DestroyChildren(goTemplate.transform.parent.gameObject, goTemplate.name);
            for (int i = 0; i < listIds.Count; ++i)
            {
                GameObject go = Lib.Core.FrameworkTool.CreateGameObject(goTemplate, goTemplate.transform.parent.gameObject);
                go.name = string.Format("{0}{1}", go.name, "(Clone)");
                go.gameObject.SetActive(true);
                EffectCell cell = new EffectCell();
                cell.Init(go.transform);
                cell.UpdateInfo(bondId, listIds[i], i);

                listEffects.Add(cell);
            }
        }
    }
}
