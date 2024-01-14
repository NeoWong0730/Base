using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Lib.Core;
using System;
using Packet;

namespace Logic
{
    public class UI_Ornament_Upgrade_ViewMiddle : UIComponent
    {
        readonly int ATTR_MAX = 9;
        private uint ornamentId;
        private bool needSelect;
        private bool resultState = false;

        private Image imgIcon;
        private Text txtName;
        private Text txtRate;
        private Text txtItemName1;
        private Text txtItemLv1;
        private Text txtItemName2;
        private Text txtItemLv2;
        private Transform contentAttr;
        private GameObject goAttr;
        private Button btnUpgrade;
        private Button btnUpgradeAll;
        private Button btnDetail;
        private GameObject goParticleItem;
        private GameObject goParticleSuccess;
        private GameObject goParticleFail;
        private GameObject goItemRedPoint;
        private GameObject goUpgradeBtnRedPoint;
        private Timer timerParticle;
        private Timer timerBtnStop;
        private bool isUpgrading;
        /// <summary> 升级是否需要二次确认 </summary>
        private bool needConfirmHint = false;

        private UI_Ornament_Upgrade_Item itemLeft;
        private UI_Ornament_Upgrade_Item itemRight;

        private List<uint> listCellIndex = new List<uint>();
        private Dictionary<uint, UI_Ornament_Upgrade_AttrCell> dictCell = new Dictionary<uint, UI_Ornament_Upgrade_AttrCell>();

        #region 系统函数
        protected override void Loaded()
        {
            Parse();
        }

        public override void Show()
        {
            base.Show();
            ResetBtnState();
        }
        public override void Hide()
        {
            base.Hide();
            ResetParticleState();
            timerParticle?.Cancel();
            timerBtnStop?.Cancel();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Ornament.Instance.eventEmitter.Handle(Sys_Ornament.EEvents.OnUpgradeTargetSeleted, OnUpgradeTargetSeleted, toRegister);
        }
        public override void OnDestroy()
        {
            timerParticle?.Cancel();
            timerBtnStop?.Cancel();
            base.OnDestroy();
        }
        #endregion

        #region func
        private void Parse()
        {
            imgIcon = transform.Find("Image_Icon").GetComponent<Image>();
            txtName = transform.Find("Text_Name").GetComponent<Text>();
            txtRate = transform.Find("Text_Success Rate").GetComponent<Text>();
            txtItemName1 = transform.Find("Text_Name_1").GetComponent<Text>();
            txtItemLv1 = transform.Find("Text_Level_1").GetComponent<Text>();
            txtItemName2 = transform.Find("Text_Name_2").GetComponent<Text>();
            txtItemLv2 = transform.Find("Text_Level_2").GetComponent<Text>();
            contentAttr = transform.Find("View_Attribute/Attr_Grid");
            goAttr = contentAttr.Find("Attr").gameObject;
            btnUpgrade = transform.Find("Btn_Upgrade").GetComponent<Button>();
            btnUpgrade.onClick.AddListener(OnBtnUpgradeClick);
            btnUpgradeAll = transform.Find("Btn_Upgrade_All").GetComponent<Button>();
            btnUpgradeAll.onClick.AddListener(OnBtnUpgradeAllClick);
            btnDetail = transform.Find("Btn_Details").GetComponent<Button>();
            btnDetail.onClick.AddListener(OnBtnDetailClick);
            btnDetail.gameObject.SetActive(false);//策划说界面说明先不显示
            goParticleItem = transform.Find("Image_Icon/Fx_UI_Jewelry_01").gameObject;
            goParticleSuccess = transform.Find("Image_Icon/Fx_UI_Jewelry_chenggong").gameObject;
            goParticleFail = transform.Find("Image_Icon/Fx_UI_Jewelry_shibai").gameObject;
            goItemRedPoint = transform.Find("PropItem_1/Image_Dot").gameObject;
            goUpgradeBtnRedPoint = transform.Find("Btn_Upgrade/Image_Dot").gameObject;

            itemLeft = AddComponent<UI_Ornament_Upgrade_Item>(transform.Find("PropItem_1"));
            itemLeft.isLeft = true;
            itemRight = AddComponent<UI_Ornament_Upgrade_Item>(transform.Find("PropItem_2"));

            listCellIndex.Clear();
            dictCell.Clear();
            FrameworkTool.CreateChildList(contentAttr, ATTR_MAX);
            for (int i = 0; i < ATTR_MAX; i++)
            {
                GameObject go = contentAttr.GetChild(i).gameObject;
                UI_Ornament_Upgrade_AttrCell cell = new UI_Ornament_Upgrade_AttrCell();
                cell.Init(go.transform);
                listCellIndex.Add((uint)i);
                dictCell.Add((uint)i, cell);
            }
        }
        public void UpdateView(uint type, uint lv)
        {
            ornamentId = Sys_Ornament.Instance.GetOrnamentIdByTypeAndLv(type, lv);
            CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(ornamentId);
            CSVOrnamentsUpgrade.Data nextOrnament = CSVOrnamentsUpgrade.Instance.GetConfData(ornament.nextlevelid);
            if (ornament != null && nextOrnament != null)
            {
                CSVItem.Data item = CSVItem.Instance.GetConfData(ornament.nextlevelid);
                imgIcon.gameObject.SetActive(true);
                ImageHelper.SetIcon(imgIcon, item.icon_id);
                txtName.text = LanguageHelper.GetTextContent(item.name_id);
                if (ornament.upgrade_rate > 0)
                {
                    float rate = (float)(ornament.upgrade_rate) / 100;
                    txtRate.text = LanguageHelper.GetTextContent(4812, rate.ToString());
                }
                else
                {
                    txtRate.text = "";
                }
                UpdateAttrView(ornament, nextOrnament);
                bool showAllBtn = ornament.upgrade_cost_equip != null;
                needSelect = ornament.extra_attr_num > 0;
                btnUpgradeAll.gameObject.SetActive(showAllBtn);
                UpdateItemLeftViewBySelect(needSelect);
                UpdateItemRightView(ornament.upgrade_cost_item);
                needConfirmHint = nextOrnament.equipment_level > Sys_Role.Instance.Role.Level;
            }
        }
        private void UpdateAttrView(CSVOrnamentsUpgrade.Data ornament, CSVOrnamentsUpgrade.Data nextOrnament, ulong selectedUuid = 0)
        {
            ItemData selectedItem = Sys_Bag.Instance.GetItemDataByUuid(selectedUuid);
            int baseCount = 0;
            int count = 0;
            if (nextOrnament != null)
            {
                baseCount = nextOrnament.base_attr.Count;
                count = baseCount + (int)nextOrnament.extra_attr_num;
            }
            for (int i = 0; i < listCellIndex.Count; i++)
            {
                var cell = dictCell[listCellIndex[i]];
                if (i < count)
                {
                    if (i < baseCount)
                    {
                        cell.UpdateView((int)nextOrnament.base_attr[i][0], nextOrnament.base_attr[i][1], ornament.base_attr[i][1]);
                    }
                    else
                    {
                        if (selectedItem != null && selectedItem.ornament != null)
                        {
                            var extAttrCount = selectedItem.ornament.ExtAttr.Count;
                            if (i < baseCount + extAttrCount)
                            {
                                var attr = selectedItem.ornament.ExtAttr[i - baseCount];
                                cell.UpdateView((int)attr.AttrId, 0, attr.AttrValue, false);
                            }
                            else if (i < baseCount + extAttrCount + selectedItem.ornament.ExtSkill.Count)
                            {
                                var skill = selectedItem.ornament.ExtSkill[i - baseCount - extAttrCount];
                                cell.UpdateSkillView(skill.InfoId, skill.SkillId);
                            }
                            else
                            {
                                cell.UpdateView(0);
                            }
                        }
                        else
                        {
                            cell.UpdateView(0);
                        }
                    }
                }
                else
                {
                    cell.UpdateView(-1);
                }
            }
        }
        private void UpdateItemLeftViewBySelect(bool _needSelect)
        {
            uint key = 0;
            uint value = 0;
            CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(ornamentId);
            Sys_Ornament.Instance.UpgradeTargetUuid = 0;
            if (_needSelect)
            {
                itemLeft.SetOrnamentId(ornamentId);
                UpdateItemLeftView(key, value);
                txtItemName1.text = LanguageHelper.GetTextContent(680000551);//选择添加饰品
                //红点刷新需要在UpdateItemLeftView之后
                goItemRedPoint.SetActive(Sys_Ornament.Instance.GetSelectRedPoint(ornamentId));
            }
            else
            {
                itemLeft.SetOrnamentId(0);
                key = ornament.id;
                value = 1;
                var costOrnament = ornament.upgrade_cost_equip;
                if (costOrnament != null)
                {
                    value += costOrnament[1];
                }
                UpdateItemLeftView(key, value);
            }
        }
        private void UpdateItemLeftView(uint key, uint value)
        {
            goItemRedPoint.SetActive(false);
            goUpgradeBtnRedPoint.SetActive(Sys_Ornament.Instance.GetUpgradeBtnRedPoint(key));

            itemLeft.UpdateView(key, value);
            CSVItem.Data itemData = CSVItem.Instance.GetConfData(key);
            CSVOrnamentsUpgrade.Data ornamentData = CSVOrnamentsUpgrade.Instance.GetConfData(key);
            string name = "";
            string lv = "";
            if (itemData != null)
            {
                name = LanguageHelper.GetTextContent(itemData.name_id);
            }
            if (ornamentData != null)
            {
                lv = LanguageHelper.GetTextContent(4811, ornamentData.lv.ToString());
            }
            txtItemName1.text = name;
            txtItemLv1.text = lv;
        }
        private void UpdateItemRightView(List<List<uint>> costItems)
        {
            uint key = 0;
            uint value = 0;
            string name = "";
            string lv = "";
            if (costItems != null && costItems[0] != null)
            {
                key = costItems[0][0];
                value = costItems[0][1];
                CSVItem.Data item = CSVItem.Instance.GetConfData(key);
                name = LanguageHelper.GetTextContent(item.name_id);
            }
            itemRight.UpdateView(key, value);
            txtItemName2.text = name;
            txtItemLv2.text = lv;
        }
        private void OnUpgradeTargetSeleted()
        {
            CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(ornamentId);
            if (ornament != null)
            {
                uint key = ornamentId;
                uint value = 1;
                UpdateItemLeftView(key, value);
                CSVOrnamentsUpgrade.Data nextOrnament = CSVOrnamentsUpgrade.Instance.GetConfData(ornament.nextlevelid);
                if (nextOrnament != null)
                {
                    UpdateAttrView(ornament, nextOrnament, Sys_Ornament.Instance.UpgradeTargetUuid);
                }
            }
        }
        public void OnFailUpdate()
        {
            OnUpgradeTargetSeleted();
            CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(ornamentId);
            UpdateItemRightView(ornament.upgrade_cost_item);
        }
        public void PlayParticle(bool success)
        {
            resultState = success;
            itemLeft.PlayParticle();
            itemRight.PlayParticle();
            timerParticle?.Cancel();
            timerParticle = Timer.Register(0.5f, PlayItemParticle);
        }
        private void PlayItemParticle()
        {
            goParticleItem.SetActive(false);
            goParticleItem.SetActive(true);
            timerParticle?.Cancel();
            timerParticle = Timer.Register(0.3f, PlayResultParticle);
        }
        private void PlayResultParticle()
        {
            if (resultState)
            {
                goParticleSuccess.SetActive(false);
                goParticleSuccess.SetActive(true);
            }
            else
            {
                goParticleFail.SetActive(false);
                goParticleFail.SetActive(true);
            }
        }
        private void ResetParticleState()
        {
            itemLeft.PlayParticle(true);
            itemRight.PlayParticle(true);
            goParticleItem.SetActive(false);
            goParticleSuccess.SetActive(false);
            goParticleFail.SetActive(false);
        }
        private void ResetBtnState()
        {
            isUpgrading = false;
        }
        #endregion

        #region event
        /// <summary> 升级 </summary>
        private void OnBtnUpgradeClick()
        {
            if (!isUpgrading)
            {
                isUpgrading = true;
                timerBtnStop?.Cancel();
                timerBtnStop = Timer.Register(0.5f, ResetBtnState);
                if (Sys_Ornament.Instance.CheckOrnamentCanUpgrade(ornamentId, needSelect, true))
                {
                    List<ItemData> itemList = Sys_Ornament.Instance.GetItemListByOrnamentId(ornamentId);
                    ulong targetUid;
                    if (needSelect)
                    {
                        targetUid = Sys_Ornament.Instance.UpgradeTargetUuid;
                    }
                    else
                    {
                        targetUid = itemList[0].Uuid;
                    }
                    if (needConfirmHint && !Sys_Ornament.Instance.UnShowUpgradeHint)
                    {

                        Sys_Ornament.Instance.PopupOrnamentUpgradeConfirmHintView(() =>
                        {
                            Sys_Ornament.Instance.OrnamentLvUpReq(targetUid);
                        });
                    }
                    else
                    {
                        Sys_Ornament.Instance.OrnamentLvUpReq(targetUid);
                    }
                }
            }
        }
        /// <summary> 全部升级 </summary>
        private void OnBtnUpgradeAllClick()
        {
            if (!isUpgrading)
            {
                isUpgrading = true;
                timerBtnStop?.Cancel();
                timerBtnStop = Timer.Register(0.5f, ResetBtnState);
                if (Sys_Ornament.Instance.CheckOrnamentCanUpgrade(ornamentId, needSelect, true))
                {
                    if (needConfirmHint && !Sys_Ornament.Instance.UnShowUpgradeHint)
                    {
                        Sys_Ornament.Instance.PopupOrnamentUpgradeConfirmHintView(() =>
                        {
                            Sys_Ornament.Instance.OrnamentAllLvUpReq(ornamentId);
                        });
                    }
                    else
                    {
                        Sys_Ornament.Instance.OrnamentAllLvUpReq(ornamentId);
                    }
                }
            }
        }
        private void OnBtnDetailClick()
        {
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { TitlelanId = 2022222, StrContent = LanguageHelper.GetTextContent(600000190) });
        }
        #endregion

        public class UI_Ornament_Upgrade_AttrCell : UIComponent
        {
            private Text txtName;
            private Text txtValue;

            protected override void Loaded()
            {
                txtName = transform.Find("Text_Property").GetComponent<Text>();
                txtValue = transform.Find("Text_Num").GetComponent<Text>();
            }

            public void UpdateView(int key, uint value = 0, uint priorValue = 0, bool showAdd = true)
            {
                if (key > 0)
                {
                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData((uint)key);
                    txtName.text = LanguageHelper.GetTextContent(680000560, LanguageHelper.GetTextContent(attrData.name));
                    var priorValueStr = LanguageHelper.GetTextContent(680000560, Sys_Attr.Instance.GetAttrValue(attrData, priorValue));
                    var addStr = "";
                    if (showAdd)
                    {
                        addStr = LanguageHelper.GetTextContent(680000561, "+" + Sys_Attr.Instance.GetAttrValue(attrData, value - priorValue));
                    }
                    txtValue.text = priorValueStr + addStr;
                }
                else if (key == 0)
                {
                    txtName.text = LanguageHelper.GetTextContent(680000562, LanguageHelper.GetTextContent(680000552));
                    txtValue.text = LanguageHelper.GetTextContent(680000562, LanguageHelper.GetTextContent(680000553));
                }
                else
                {
                    txtName.text = "";
                    txtValue.text = "";
                }
            }
            public void UpdateSkillView(uint InfoId, uint skillId)
            {
                CSVPassiveSkillInfo.Data skillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                txtName.text = LanguageHelper.GetTextContent(680000560, LanguageHelper.GetTextContent(skillInfoData.name));
                txtValue.text = LanguageHelper.GetTextContent(680000560, Sys_Ornament.Instance.GetPassiveSkillShowString(InfoId, skillInfoData));
            }
        }

        public class UI_Ornament_Upgrade_Item : UIComponent
        {
            public bool isLeft;
            private uint OrnamentId;
            private CSVItem.Data itemData;
            private GameObject goParticle;
            private GameObject goPlus;
            private GameObject goLock;
            private Text txtNum;
            private Button btnItem;
            private Image imgItemIcon;
            private Image imgItemQuality;

            protected override void Loaded()
            {
                goParticle = transform.Find("Fx_UI_Jewelry_02").gameObject;
                goPlus = transform.Find("Image_Add").gameObject;
                goLock = transform.Find("Image_Lock").gameObject;
                txtNum = transform.Find("Text_Number").GetComponent<Text>();
                txtNum.gameObject.SetActive(true);
                txtNum.text = "";
                btnItem = transform.Find("Btn_Item").GetComponent<Button>();
                btnItem.onClick.AddListener(OnItemClick);
                imgItemIcon = transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                imgItemQuality = transform.Find("Btn_Item/Image_Quality").GetComponent<Image>();
            }

            public void UpdateView(uint key, uint value)
            {
                itemData = CSVItem.Instance.GetConfData(key);
                if (itemData != null)
                {
                    var item = Sys_Bag.Instance.GetItemDataByUuid(Sys_Ornament.Instance.UpgradeTargetUuid);
                    imgItemIcon.gameObject.SetActive(true);
                    imgItemQuality.gameObject.SetActive(true);
                    ImageHelper.SetIcon(imgItemIcon, itemData.icon_id);
                    uint quality = itemData.quality;
                    if (item != null)
                    {
                        quality = item.Quality;
                    }
                    ImageHelper.GetQualityColor_Frame(imgItemQuality, (int)quality);
                    goPlus.SetActive(false);
                    goLock.SetActive(false);
                    long hasNum = Sys_Bag.Instance.GetItemCount(itemData.id);
                    uint worldStyleId = hasNum >= value ? (uint)83 : 84;
                    TextHelper.SetText(txtNum, hasNum + "/" + value, CSVWordStyle.Instance.GetConfData(worldStyleId));
                }
                else
                {
                    imgItemIcon.gameObject.SetActive(false);
                    imgItemQuality.gameObject.SetActive(false);
                    goPlus.SetActive(isLeft);
                    goLock.SetActive(!isLeft);
                    txtNum.text = "";
                }
            }
            public void SetOrnamentId(uint _id)
            {
                OrnamentId = _id;
            }
            public void PlayParticle(bool hide = false)
            {
                goParticle.SetActive(false);
                if (!goPlus.activeInHierarchy && !goLock.activeInHierarchy && !hide)
                {
                    goParticle.SetActive(true);
                }
            }
            private void OnItemClick()
            {
                if (OrnamentId > 0)
                {
                    UIManager.OpenUI(EUIID.UI_Ornament_Select, false, OrnamentId);
                }
                else
                {
                    if (itemData != null)
                    {
                        if (itemData.type_id == (uint)EItemType.Ornament)
                        {
                            OrnamentTipsData tipData = new OrnamentTipsData
                            {
                                equip = new ItemData(0, 0, itemData.id, 1, 0, false, false, null, null, 0),
                                isShowOpBtn = false,
                                isShowSourceBtn = true
                            };
                            UIManager.OpenUI(EUIID.UI_Tips_Ornament, false, tipData);
                        }
                        else
                        {
                            PropIconLoader.ShowItemData showItemData = new PropIconLoader.ShowItemData(itemData.id, 0, false, false, false, false, false, false, true);
                            UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Ornament, showItemData));
                        }
                    }
                }
            }
        }
    }
}
