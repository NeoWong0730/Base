using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;
using Packet;

namespace Logic
{
    public class UI_Partner_Property : UIParseCommon, UI_Partner_Property_HeadList.IListener
    {
        public class AttrCell
        {
            private Transform transform;

            private Text txtAttrName;
            private Button btnMinus;
            private Button btnPlus;

            private Image imgFillPre;
            private Image imgFillCur;
            private Text txtSlider;
            private Text txtState;

            private CSVPartner.Data _infoData;
            private Partner paInfo;
            private PartnerImproveCount proveInfo;
            private int _index;
            private CmdPartnerDistributePointReq.Types.DistributePointInfo distriInfo;
            private CSVAttrBoost.Data boostInfo;
            private uint lvMaxRate;
            private uint totalRate;
            private uint nextLevel;

            private bool singleMaxLimit; //单条分配最大上限
            private bool lvMaxLimit; //单条等级分配上限
            private bool nextCostLmit; //已达分配上限

            public void Init(Transform trans)
            {
                transform = trans;

                txtAttrName = transform.Find("Text_Name").GetComponent<Text>();
                btnMinus = transform.Find("Btn_Minus").GetComponent<Button>();
                btnMinus.onClick.AddListener(OnClickMinus);
                btnPlus = transform.Find("Btn_Plus").GetComponent<Button>();
                btnPlus.onClick.AddListener(OnClickAdd);
                imgFillPre = transform.Find("Slider/Fill_2").GetComponent<Image>();
                imgFillCur = transform.Find("Slider/Fill_1").GetComponent<Image>();
                txtSlider = transform.Find("Slider/Text").GetComponent<Text>();
                txtState = transform.Find("Image_State/Text").GetComponent<Text>();
            }

            public void UpdateInfo(uint paId, PartnerImproveCount improveCount, int index)
            {
                _infoData = CSVPartner.Instance.GetConfData(paId);
                paInfo = Sys_Partner.Instance.GetPartnerInfo(paId);
                proveInfo = improveCount;
                _index = index;
                distriInfo = Sys_Partner.Instance.GenDsitributes(index, improveCount);

                boostInfo = CSVAttrBoost.Instance.GetConfData(improveCount.KeyId);
                CSVAttr.Data attrInfo = CSVAttr.Instance.GetConfData(boostInfo.attr_id);
                txtAttrName.text = LanguageHelper.GetTextContent(attrInfo.name);

                totalRate = 0;
                lvMaxRate = 0;
                nextLevel = 0;
                singleMaxLimit = false;
                lvMaxLimit = false;
                nextCostLmit = false;
                CalLevelMaxRate();
                RefreshRate();
                RefreshState();
                imgFillPre.fillAmount = imgFillCur.fillAmount;
            }

            private void OnClickAdd()
            {

                if (singleMaxLimit)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2014207));
                    return;
                }

                if (nextCostLmit)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2014208));
                    return;
                }

                if (lvMaxLimit)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2014209));
                    return;
                }
                
                int tempIndex = (int)distriInfo.Count;
                List<uint> data = boostInfo.boost_value_cost[tempIndex];
                uint tempRate = data[0];
                uint tempCost = data[1];

                if (!Sys_Partner.Instance.IsPlusDistributePoint(tempCost))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2014210)); //点数不足
                    return;
                }

                distriInfo.Count++;
                Sys_Partner.Instance.PlusDistributePoint(tempCost);

                RefreshRate();
                RefreshState();
            }

            private void OnClickMinus()
            {
                if (distriInfo.Count <= proveInfo.ImproveCount)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2014211)); //没有预分配点数
                    return;
                }

                distriInfo.Count--;
                List<uint> data = boostInfo.boost_value_cost[(int)distriInfo.Count];
                uint tempRate = data[0];
                uint tempCost = data[1];
                Sys_Partner.Instance.MinusDistributePoint(tempCost);
                
                RefreshRate();
                RefreshState();
            }

            private void CalLevelMaxRate()
            {
                nextLevel = boostInfo.limit_boost_lv[0][0];
                for (int i = boostInfo.limit_boost_lv.Count - 1; i >= 0; --i)
                {
                    uint tempLv = boostInfo.limit_boost_lv[i][0];
                    if (Sys_Role.Instance.Role.Level >= tempLv)
                    {
                        lvMaxRate = boostInfo.limit_boost_lv[i][1];
                        nextLevel = boostInfo.limit_boost_lv[i][0] + 10;
                        break;
                    }
                }
            }

            private void RefreshRate()
            {
                //计算总百分比
                totalRate = 0;
                for (int i = 0; i < boostInfo.boost_value_cost.Count; ++i)
                {
                    if (i < distriInfo.Count)
                    {
                        totalRate += boostInfo.boost_value_cost[i][0];
                    }
                }

                uint temp = totalRate * 100 / 10000;
                txtSlider.text = string.Format("{0}%", temp);
                
                float fValue = totalRate * 1.0f / boostInfo.limit_boost_max;
                imgFillCur.fillAmount = fValue;
            }

            public void RefreshState()
            {
                //是否达到单条最大值
                singleMaxLimit = totalRate >= boostInfo.limit_boost_max;
                if (singleMaxLimit)
                {
                    txtState.text = LanguageHelper.GetTextContent(2014008);
                    return;
                }
                
                //是否达到等级上限
                lvMaxLimit = totalRate >= lvMaxRate;
                if (lvMaxLimit)
                {
                    txtState.text = LanguageHelper.GetTextContent(2014010, nextLevel.ToString());
                    return;
                }
                
                uint leftCost = _infoData.attr_boost_limit -  paInfo.ImproveData.UsePoint - Sys_Partner.Instance.tempPoint.costPoint;
                int tempIndex = (int)distriInfo.Count; //下一次分配索引
                List<uint> data = boostInfo.boost_value_cost[tempIndex];
                uint tempRate = data[0];
                uint tempCost = data[1];
                //剩余点数 是否 够下次分配
                nextCostLmit = tempCost > leftCost;
                if (nextCostLmit)
                {
                    txtState.text = LanguageHelper.GetTextContent(2014009);
                    return;
                }

                uint leftPoint = Sys_Partner.Instance.tempPoint.totalPoint - Sys_Partner.Instance.tempPoint.costPoint;
                txtState.text = LanguageHelper.GetTextContent(2014011, tempCost.ToString(), leftPoint.ToString());
            }
        }

        public class DetailCell
        {
            private Transform transform;
            private Text txtName;
            private Text txtValue;
            private Transform tansBg;
            
            private Partner paInfo;
            private CSVPartner.Data csvData;
            private PartnerImproveCount improveCount;
            private int _index;
            private CSVAttr.Data attrData;
            private int value = 0;
            public void Init(Transform trans)
            {
                transform = trans;
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                txtValue = transform.Find("Text_Value").GetComponent<Text>();
                tansBg = transform.Find("Image_bg");
            }

            public void UpdateInfo(uint paId, PartnerImproveCount proveCount, int index)
            {
                improveCount = proveCount;
                _index = index;
                paInfo = Sys_Partner.Instance.GetPartnerInfo(paId);
                csvData = CSVPartner.Instance.GetConfData(paId);
                
                tansBg.gameObject.SetActive(index%2 == 0);
                
                List<AttributeRow> listbasic = Sys_Partner.Instance.GetPartnerAttr(paId, paInfo.Level, EAttryType.Basic, true);
                List<AttributeRow> listHigh = Sys_Partner.Instance.GetPartnerAttr(paId, paInfo.Level, EAttryType.High, true);
                
                List<AttributeRow> temp = new List<AttributeRow>();
                temp.AddRange(listbasic);
                temp.AddRange(listHigh);

                CSVAttrBoost.Data boostInfo = CSVAttrBoost.Instance.GetConfData(proveCount.KeyId);
                attrData = CSVAttr.Instance.GetConfData(boostInfo.attr_id);
                txtName.text = LanguageHelper.GetTextContent(attrData.name);

                value = 0;
                for (int i = 0; i < temp.Count; ++i)
                {
                    if (temp[i].Id == boostInfo.attr_id)
                    {
                        value = temp[i].Value;
                        break;
                    }
                }

                uint changeRate = 0;
                if (improveCount.ImproveCount > 0)
                {
                    int start = 0;
                    int end = (int)improveCount.ImproveCount;
                    CSVAttrBoost.Data boosInfo = CSVAttrBoost.Instance.GetConfData(improveCount.KeyId);
                    for (int i = start; i < end; ++i)
                    {
                        changeRate += boosInfo.boost_value_cost[i][0];
                    }
                }

                if (attrData.show_type == 2)
                {
                    uint rateValue = changeRate;
                    value += (int)rateValue;
                }
                else
                {
                    float fValue = value * changeRate * 1.0f / 10000;
                    int tempValue = Mathf.CeilToInt(fValue);
                    value += tempValue;
                }
                
                txtValue.text = Sys_Attr.Instance.GetAttrValue(attrData, value);
            }

            public void RefreshChange()
            {
                uint changeRate = Sys_Partner.Instance.GetChangeRate(improveCount, _index);
                if (changeRate == 0)
                {
                    txtValue.text = Sys_Attr.Instance.GetAttrValue(attrData, value);
                    return;
                }

                int tempValue = 0;
                if (attrData.show_type == 2)
                {
                    uint rateValue = changeRate;
                    tempValue = (int)rateValue;
                }
                else
                {
                    float fValue = value * changeRate * 1.0f / 10000;
                    tempValue = Mathf.CeilToInt(fValue);
                }
                
                txtValue.text = string.Format("{0} + {1}", Sys_Attr.Instance.GetAttrValue(attrData, value), 
                    Sys_Attr.Instance.GetAttrValue(attrData, tempValue));
            }
        }
        
        public class SkillChange
        {
            private Transform transform;
            private SkillItem02 skillItem;
            private Image imgLock;
            private Text txtLock;
            private Transform transCounsume;
            private Image imgConsume;
            private Text txtConsume;
            private Button btnRandom;

            private uint _paId;
            private uint _slot;
            private PartnerImproveData.Types.ImproveSkillInfo skillInfo;
            private CSVPartnerSkill.Data skillData;
            private Partner _paterInfo;

            public void Init(Transform trans)
            {
                transform = trans;

                skillItem = new SkillItem02();
                skillItem.Bind(transform.Find("SkillItem02").gameObject);
                skillItem.RegisterAction(OnClickSill);

                imgLock = transform.Find("SkillItem02/Image_Lock").GetComponent<Image>();
                txtLock = transform.Find("Text_Lock").GetComponent<Text>();
                transCounsume = transform.Find("Text_Consume");
                imgConsume = transform.Find("Text_Consume/Image_Icon").GetComponent<Image>();
                txtConsume = transform.Find("Text_Consume/Text_Value").GetComponent<Text>();
                btnRandom = transform.Find("Btn_Random").GetComponent<Button>();
                btnRandom.onClick.AddListener(OnClickRandom);
            }

            private void OnClickRandom()
            {
                uint costId = skillData.extra_skill_random_cost[0];
                uint costNum = skillData.extra_skill_random_cost[1];
                uint totalNum = (uint)Sys_Bag.Instance.GetItemCount(costId);
                if (costNum > totalNum)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2014214));
                    return;
                }
                
                Sys_Partner.Instance.OnReRandomSkillReq(_paId, _slot);
            }

            private void OnClickSill(uint skillId)
            {
                if (skillId != 0)
                {
                    UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new System.Tuple<uint, uint>(skillId, 0));
                }
            }

            public void UpdateInfo(uint paId, uint slot, PartnerImproveData.Types.ImproveSkillInfo info)
            {
                _paId = paId;
                _slot = slot;
                skillInfo = info;
                _paterInfo = Sys_Partner.Instance.GetPartnerInfo(paId);
                skillData = CSVPartnerSkill.Instance.GetConfData(paId);

                uint needPoint = skillData.extra_skill_unlock[(int) _slot];
                bool isCanRandom = _paterInfo.ImproveData.UsePoint >= needPoint;
                if (isCanRandom)
                {
                    txtLock.gameObject.SetActive(false);
                    
                    transCounsume.gameObject.SetActive(true);
                    uint costId = skillData.extra_skill_random_cost[0];
                    uint costNum = skillData.extra_skill_random_cost[1];
                    CSVItem.Data costData = CSVItem.Instance.GetConfData(costId);
                    ImageHelper.SetIcon(imgConsume, costData.small_icon_id);
                    txtConsume.text = costNum.ToString();
                    
                    btnRandom.gameObject.SetActive(true);
                    
                    if (skillInfo.SkillId != 0)
                    {
                        imgLock.gameObject.SetActive(false);
                        skillItem.imgIcon.gameObject.SetActive(true);
                        skillItem.SetData(skillInfo.SkillId);
                        skillItem.toggle.enabled = true;

                    }
                    else
                    {
                        skillItem.imgIcon.gameObject.SetActive(false);
                        skillItem.toggle.enabled = false;
                        imgLock.gameObject.SetActive(true);
                    }
                }
                else
                {
                    txtLock.gameObject.SetActive(true);
                    txtLock.text = LanguageHelper.GetTextContent(2014020, needPoint.ToString());
                    
                    transCounsume.gameObject.SetActive(false);
                    btnRandom.gameObject.SetActive(false);
                    
                    skillItem.imgIcon.gameObject.SetActive(false);
                    skillItem.toggle.enabled = false;
                    imgLock.gameObject.SetActive(true);
                }
            }

            public void ResponseNtf(uint skillId)
            {
                // imgLock.gameObject.SetActive(false);
                // imgIcon.gameObject.SetActive(true);
                skillItem.SetData(skillId);
            }
        }

        private UI_Partner_Property_HeadList _headList;
        
        private Text txtName;
        private Text txtCarrer;
        private Text txtLevel;
        private Text txtAllocted;
        private Text txtAvaliable;
        private Button btnPlus;
        private Button btnReset;
        private Text txtAddPoint;
        private Button btnAddPoint;
        
        private InfinityGrid _infinityGridAttrs;
        private Dictionary<GameObject, AttrCell> dictAttrCells = new Dictionary<GameObject,AttrCell>();
        private InfinityGrid _infinityGridDetail;
        private  Dictionary<GameObject, DetailCell> dictDetailCells = new Dictionary<GameObject,DetailCell>();
        private SkillChange skill1;
        private SkillChange skill2;

        private uint infoId;
        private CSVPartner.Data infoData;
        private Partner paInfo;

        protected override void Parse()
        {
            _headList = new UI_Partner_Property_HeadList();
            _headList.Init(transform.Find("Scroll_View_Partner"));
            _headList.Register(this);

            txtName = transform.Find("View_Right/Top/Text_Name").GetComponent<Text>();
            txtCarrer = transform.Find("View_Right/Top/Text_Profession").GetComponent<Text>();
            txtLevel = transform.Find("View_Right/Top/Text_Level").GetComponent<Text>();
            txtAllocted = transform.Find("View_Right/Top/Text_Allocated/Text_Value").GetComponent<Text>();
            txtAvaliable = transform.Find("View_Right/Top/Text_Available/Text_Value").GetComponent<Text>();
            Text temp = transform.Find("View_Right/Top/Text_Available/Text_Value/Text_Value").GetComponent<Text>();
            temp.text = "";
            btnPlus = transform.Find("View_Right/Top/Text_Available/Btn_Plus").GetComponent<Button>();
            btnPlus.onClick.AddListener(OnClickAddPoint);
            btnReset = transform.Find("View_Right/Top/Text_Reset/Btn_Plus").GetComponent<Button>();
            btnReset.onClick.AddListener(OnClickResetPoint);
            
            txtAddPoint = transform.Find("View_Right/View_Add/Text_Number/Text_Number").GetComponent<Text>();
            btnAddPoint = transform.Find("View_Right/View_Add/Btn_01").GetComponent<Button>();
            btnAddPoint.onClick.AddListener(OnClickTransPoint);

            _infinityGridAttrs = transform.Find("View_Right/View_Add/Scroll View").GetComponent<InfinityGrid>();
            _infinityGridAttrs.onCreateCell += OnCreateCell;
            _infinityGridAttrs.onCellChange += OnCellChange;
            
            _infinityGridDetail = transform.Find("View_Right/View_Details/Scroll View").GetComponent<InfinityGrid>();
            _infinityGridDetail.onCreateCell += OnCreateDetail;
            _infinityGridDetail.onCellChange += OnDetailChange;
            
            skill1 = new SkillChange();
            skill1.Init(transform.Find("View_Right/View_Details/Skill/Skill_1"));
            
            skill2 = new SkillChange();
            skill2.Init(transform.Find("View_Right/View_Details/Skill/Skill_2"));
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            ProcessEvents(false);
            ProcessEvents(true);
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
            ProcessEvents(false);
        }

        public override void OnDestroy()
        {
            dictAttrCells.Clear();
            dictDetailCells.Clear();
            base.OnDestroy();
        }

        private void ProcessEvents(bool toRegister)
        {
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnPartnerPointUpdateNtf, OnPartnerPointUpdateNtf,  toRegister);
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnPartnerDistrutePointNtf, OnPartnerDistrutePointNtf,  toRegister);
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnPartnerPointChangeNtf, OnPartnerPointChangeNtf,  toRegister);
            Sys_Partner.Instance.eventEmitter.Handle<Sys_Partner.SkillResetInfo>(Sys_Partner.EEvents.OnPartnerPointSkillNtf, OnPartnerPointSkillNtf,  toRegister);
        }

        private void OnClickAddPoint()
        {
            uint addPoint = paInfo.ImproveData.UsePoint + Sys_Partner.Instance.tempPoint.costPoint;
            if (addPoint > infoData.attr_boost_limit)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2014212));
                return;
            }
            
            UIManager.OpenUI(EUIID.UI_ExchangePointChange);
        }

        private void OnClickResetPoint()
        {
            UIManager.OpenUI(EUIID.UI_ExchangePointReset, false, infoId);
        }

        private void OnClickTransPoint()
        {
            Sys_Partner.Instance.OnDistributePointReq(infoId);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            AttrCell attrCell = new AttrCell();
            attrCell.Init(cell.mRootTransform);
            cell.BindUserData(attrCell);
            
            dictAttrCells.Add(cell.mRootTransform.gameObject, attrCell);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            AttrCell attrCell = cell.mUserData as AttrCell;
            attrCell.UpdateInfo(infoId, paInfo.ImproveData.ImproveAttrs[index], index);
        }
        
        private void OnCreateDetail(InfinityGridCell cell)
        {
            DetailCell dCell = new DetailCell();
            dCell.Init(cell.mRootTransform);
            cell.BindUserData(dCell);
            
            dictDetailCells.Add(cell.mRootTransform.gameObject, dCell);
        }

        private void OnDetailChange(InfinityGridCell cell, int index)
        {
            DetailCell attrCell = cell.mUserData as DetailCell;
            attrCell.UpdateInfo(infoId, paInfo.ImproveData.ImproveAttrs[index], index);
        }

        private void OnPartnerPointUpdateNtf()
        {
            RefreshTopInfo();
            RefreshAttrList();
            RefreshAttrs();
        }

        private void OnPartnerDistrutePointNtf()
        {
            RefreshTopInfo();
            RefreshAttrList();
            RefreshAttrs();
            RefreshSkills();
        }

        private void OnPartnerPointChangeNtf()
        {
            //已分配点数
            txtAllocted.text = string.Format("{0}+{1}/{2}", paInfo.ImproveData.UsePoint.ToString(),
                Sys_Partner.Instance.tempPoint.costPoint.ToString(),
                infoData.attr_boost_limit.ToString());
            
            txtAvaliable.text = string.Format("{0}-{1}", Sys_Partner.Instance.tempPoint.totalPoint.ToString(),
                Sys_Partner.Instance.tempPoint.costPoint);
            
            txtAddPoint.text = LanguageHelper.GetTextContent(2014206, Sys_Partner.Instance.tempPoint.costPoint.ToString(),
                Sys_Partner.Instance.tempPoint.totalPoint.ToString());
            
            foreach (var data in dictDetailCells)
            {
                data.Value.RefreshChange();
            }

            foreach (var data in dictAttrCells)
            {
                data.Value.RefreshState();
            }
        }

        private void OnPartnerPointSkillNtf(Sys_Partner.SkillResetInfo resetInfo)
        {
            if (infoId == resetInfo.paId)
            {
                if(resetInfo.index == 0)
                    skill1.ResponseNtf(resetInfo.skillId);
                else if (resetInfo.index == 1)
                    skill2.ResponseNtf(resetInfo.skillId);
            }
        }

        public void UpdateInfo(uint selectId)
        {
            _headList.UpdateInfo(selectId);
        }

        public void OnSelectPartner(uint id)
        {
            infoId = id;
            infoData = CSVPartner.Instance.GetConfData(infoId);
            paInfo = Sys_Partner.Instance.GetPartnerInfo(infoId);
            Sys_Partner.Instance.tempPoint.ResetCost();
            RefreshTopInfo();
            RefreshAttrList();
            RefreshAttrs();
            RefreshSkills();
        }

        private void RefreshTopInfo()
        {
            txtName.text = LanguageHelper.GetTextContent(infoData.name);
            txtLevel.text = LanguageHelper.GetTextContent(6005, paInfo.Level.ToString());
            txtCarrer.text = LanguageHelper.GetTextContent(OccupationHelper.GetTextID(infoData.profession));

            txtAllocted.text = string.Format("{0}/{1}", paInfo.ImproveData.UsePoint.ToString(),
                infoData.attr_boost_limit.ToString());
            
            txtAvaliable.text = Sys_Partner.Instance.tempPoint.totalPoint.ToString();

            txtAddPoint.text = LanguageHelper.GetTextContent(2014206, Sys_Partner.Instance.tempPoint.costPoint.ToString(),
                Sys_Partner.Instance.tempPoint.totalPoint.ToString());
        }

        private void RefreshAttrList()
        {
            Sys_Partner.Instance.ClearDistributes();
            _infinityGridAttrs.CellCount = paInfo.ImproveData.ImproveAttrs.Count;
            _infinityGridAttrs.ForceRefreshActiveCell();
        }

        private void RefreshAttrs()
        {
            _infinityGridDetail.CellCount = paInfo.ImproveData.ImproveAttrs.Count;
            _infinityGridDetail.ForceRefreshActiveCell();
        }

        private void RefreshSkills()
        {
            PartnerImproveData.Types.ImproveSkillInfo skillInfo1 = null;
            PartnerImproveData.Types.ImproveSkillInfo skillInfo2 = null;
            if (paInfo.ImproveData.ImproveSkills.Count >= 1)
                skillInfo1 = paInfo.ImproveData.ImproveSkills[0];
            if (paInfo.ImproveData.ImproveSkills.Count >= 2)
                skillInfo2 = paInfo.ImproveData.ImproveSkills[1];
            skill1.UpdateInfo(infoId, 0, skillInfo1);
            skill2.UpdateInfo(infoId, 1, skillInfo2);
        }
    }
}
