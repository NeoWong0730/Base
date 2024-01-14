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
    public class UI_Ornament_Recast_ViewMiddle : UIComponent
    {
        private ulong curItemUid;
        private ItemData itemData;
        private CSVItem.Data costItem;

        private Image imgIcon;
        private Image imgCostIcon;
        private Image imgCostQuality;
        private Text txtName;
        private Text txtCostNum;
        private Button btnRecast;
        private Button btnDetail;
        private Button btnCostItem;
        private Button btnItemIcon;
        private List<UI_Ornament_Recast_AttrCellView> listNowAttr = new List<UI_Ornament_Recast_AttrCellView>();
        private List<UI_Ornament_Recast_AttrCellView> listAfterAttr = new List<UI_Ornament_Recast_AttrCellView>();
        private Timer timerBtnStop;
        private bool isRecasting;
        /// <summary> 是否需要重铸二次确认弹窗 </summary>
        private bool needRecastConfirmHint = false;
        /// <summary> 缓存的锁定列表 记录锁定词缀的位置 </summary>
        private List<uint> cacheLockList = new List<uint>();
        /// <summary> 缓存的展示列表 记录当前词缀显示顺序 </summary>
        private List<uint> cacheShowList = new List<uint>();

        private Timer btnCDTimer;   //按钮cd 防止多次请求
        private bool isBtnCD;

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
            timerBtnStop?.Cancel();
            btnCDTimer?.Cancel();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Ornament.Instance.eventEmitter.Handle<uint, bool>(Sys_Ornament.EEvents.OnRecastLockResBack, OnRecastLockResBack, toRegister);
        }
        public override void OnDestroy()
        {
            timerBtnStop?.Cancel();
            btnCDTimer?.Cancel();
            base.OnDestroy();
        }
        #endregion

        #region func
        private void Parse()
        {
            imgIcon = transform.Find("Image_Icon").GetComponent<Image>();
            imgCostIcon = transform.Find("View_Attribute/PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
            imgCostQuality = transform.Find("View_Attribute/PropItem/Btn_Item/Image_Quality").GetComponent<Image>();
            txtName = transform.Find("Text_Name").GetComponent<Text>();
            txtCostNum = transform.Find("View_Attribute/PropItem/Text_Number").GetComponent<Text>();
            btnRecast = transform.Find("Btn_Recast").GetComponent<Button>();
            btnRecast.onClick.AddListener(OnBtnRecastClick);
            btnDetail = transform.Find("Btn_Details").GetComponent<Button>();
            btnDetail.onClick.AddListener(OnBtnDetailClick);
            btnDetail.gameObject.SetActive(false);//策划说界面说明先不显示
            btnCostItem = transform.Find("View_Attribute/PropItem/Btn_Item").GetComponent<Button>();
            btnCostItem.onClick.AddListener(OnBtnCostItemClick);
            btnItemIcon = transform.Find("Image_Icon").GetComponent<Button>();
            btnItemIcon.onClick.AddListener(OnBtnItemIconClick);

            Transform nowAttrParent = transform.Find("View_Attribute/Attr_Grid_1");
            for (int i = 0; i < nowAttrParent.childCount; i++)
            {
                GameObject go = nowAttrParent.GetChild(i).gameObject;
                UI_Ornament_Recast_AttrCellView cell = new UI_Ornament_Recast_AttrCellView();
                cell.Init(go.transform);
                cell.RegisterLockAction(OnCellBtnLockClick);
                listNowAttr.Add(cell);
            }
            Transform afterAttrParent = transform.Find("View_Attribute/Attr_Grid_2");
            for (int i = 0; i < afterAttrParent.childCount; i++)
            {
                GameObject go = afterAttrParent.GetChild(i).gameObject;
                UI_Ornament_Recast_AttrCellView cell = new UI_Ornament_Recast_AttrCellView();
                cell.Init(go.transform);
                listAfterAttr.Add(cell);
            }
        }
        public void UpdateView(ulong itemUuid)
        {
            if (curItemUid != itemUuid)
            {
                //更换饰品的时候重新生成缓存锁定列表
                ResetCacheLockList(itemUuid);
            }
            curItemUid = itemUuid;
            itemData = Sys_Bag.Instance.GetItemDataByUuid(itemUuid);
            Sys_Ornament.Instance.FixedOrnamentLockList(itemData);
            imgIcon.gameObject.SetActive(true);
            ImageHelper.SetIcon(imgIcon, itemData.cSVItemData.icon_id);
            txtName.text = LanguageHelper.GetTextContent(itemData.cSVItemData.name_id);
            CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(itemData.Id);
            uint costKey = ornament.reforge_cost[0][0];
            uint costValue = ornament.reforge_cost[0][1];
            if (itemData.ornament.LockList.Count > 0)
            {
                var index = itemData.ornament.LockList.Count - 1;
                if (index < ornament.lock_reforge_cost.Count)
                {
                    //costKey = ornament.lock_reforge_cost[index][0];
                    costValue += ornament.lock_reforge_cost[index][1];
                }
                else
                {
                    costValue += ornament.lock_reforge_cost[ornament.lock_reforge_cost.Count - 1][1];
                    //DebugUtil.LogError("饰品锁定条目超过上限 uid " + curItemUid + " LockListCount " + itemData.ornament.LockList.Count);
                }
            }
            costItem = CSVItem.Instance.GetConfData(costKey);
            imgCostIcon.gameObject.SetActive(true);
            imgCostQuality.gameObject.SetActive(true);
            ImageHelper.SetIcon(imgCostIcon, costItem.icon_id);
            ImageHelper.GetQualityColor_Frame(imgCostQuality, (int)costItem.quality);
            long hasNum = Sys_Bag.Instance.GetItemCount(costItem.id);
            uint worldStyleId = hasNum >= costValue ? (uint)83 : 84;
            TextHelper.SetText(txtCostNum, hasNum + "/" + costValue, CSVWordStyle.Instance.GetConfData(worldStyleId));
            UpdateAttrView(itemData);
        }
        private void UpdateAttrView(ItemData itemData)
        {
            List<Ornament.Types.ExtAttr> ExtAttr = new List<Ornament.Types.ExtAttr>();
            List<Ornament.Types.ExtSkill> ExtSkill = new List<Ornament.Types.ExtSkill>();
            List<uint> lockList = new List<uint>();
            needRecastConfirmHint = false;
            if (itemData != null)
            {
                ExtAttr.AddRange(itemData.ornament.ExtAttr);
                ExtSkill.AddRange(itemData.ornament.ExtSkill);
                lockList.AddRange(itemData.ornament.LockList);
            }
            for (int i = 0; i < listNowAttr.Count; i++)
            {
                var nowCell = listNowAttr[i];
                var afterCell = listAfterAttr[i];

                if (i < cacheShowList.Count)
                {
                    var curInfoId = cacheShowList[i];
                    //判断是属性id还是技能id
                    if (CSVOrnamentsAttributes.Instance.GetConfData(curInfoId) != null)
                    {
                        //属性
                        for (int j = 0; j < ExtAttr.Count; j++)
                        {
                            if (ExtAttr[j].InfoId == curInfoId)
                            {
                                var attr = ExtAttr[j];
                                bool isLock = lockList.Contains(attr.InfoId);
                                nowCell.UpdateView(attr, i);
                                nowCell.SetLockState(true, isLock);
                                if (isLock)
                                {
                                    afterCell.UpdateView(attr);
                                    afterCell.SetLockState(false);
                                }
                                else
                                {
                                    afterCell.UpdateView(0);
                                }
                                needRecastConfirmHint = needRecastConfirmHint || Sys_Ornament.Instance.CheckExtAttrIsBest(attr.InfoId, attr.AttrValue);
                                break;
                            }
                        }
                    }
                    else
                    {
                        //技能
                        for (int j = 0; j < ExtSkill.Count; j++)
                        {
                            if (ExtSkill[j].InfoId == curInfoId)
                            {
                                var skill = ExtSkill[j];
                                bool isLock = lockList.Contains(skill.InfoId) || Sys_Ornament.Instance.CheckIsSpecialInfoId(skill.InfoId);
                                nowCell.UpdateView(skill, i);
                                nowCell.SetLockState(true, isLock);
                                if (isLock)
                                {
                                    afterCell.UpdateView(skill);
                                    afterCell.SetLockState(false);
                                }
                                else
                                {
                                    afterCell.UpdateView(0);
                                }
                                needRecastConfirmHint = needRecastConfirmHint || Sys_Ornament.Instance.CheckExtSkillIsBest(skill.SkillId);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    nowCell.UpdateView(-1);
                    afterCell.UpdateView(-1);
                }
            }
        }
        private void ResetBtnState()
        {
            isRecasting = false;
        }
        /// <summary>
        /// 重新生成缓存锁定列表
        /// </summary>
        private void ResetCacheLockList(ulong uid)
        {
            cacheLockList.Clear();
            cacheShowList.Clear();
            itemData = Sys_Bag.Instance.GetItemDataByUuid(uid);
            if (itemData != null && itemData.ornament != null)
            {
                var lockList = itemData.ornament.LockList;
                var extAttr = itemData.ornament.ExtAttr;
                for (int i = 0; i < extAttr.Count; i++)
                {
                    var infoId = extAttr[i].InfoId;
                    cacheShowList.Add(infoId);
                    if (lockList.Contains(infoId))
                    {
                        cacheLockList.Add(infoId);
                    }
                    else
                    {
                        cacheLockList.Add(0);
                    }
                }
                var extSkill = itemData.ornament.ExtSkill;
                for (int i = 0; i < extSkill.Count; i++)
                {
                    var infoId = extSkill[i].InfoId;
                    cacheShowList.Add(infoId);
                    if (lockList.Contains(infoId))
                    {
                        cacheLockList.Add(infoId);
                    }
                    else
                    {
                        cacheLockList.Add(0);
                    }
                }
            }
        }
        /// <summary>
        /// 重新设置showList 保持锁定词条的位置，补齐其他位置 (在重铸后，刷新界面前调用)
        /// </summary>
        public void RefreshCacheShowlist()
        {
            itemData = Sys_Bag.Instance.GetItemDataByUuid(curItemUid);
            if (itemData != null && itemData.ornament != null)
            {
                //先检查重铸出来的新属性里是否有特殊属性，从而修正缓存锁定列表
                var specialInfoId = Sys_Ornament.Instance.SpecialAttrInfoId;
                if (!cacheLockList.Contains(specialInfoId) && itemData.ornament.LockList.Contains(specialInfoId))
                {
                    for (int i = 0; i < cacheLockList.Count; i++)
                    {
                        //塞到空位置上
                        if(cacheLockList[i] == 0)
                        {
                            cacheLockList[i] = specialInfoId;
                            break;
                        }
                    }
                }

                cacheShowList.Clear();
                //深拷贝cacheLockList
                for (int i = 0; i < cacheLockList.Count; i++)
                {
                    cacheShowList.Add(cacheLockList[i]);
                }

                List<Ornament.Types.ExtAttr> ExtAttr = new List<Ornament.Types.ExtAttr>();
                List<Ornament.Types.ExtSkill> ExtSkill = new List<Ornament.Types.ExtSkill>();
                ExtAttr.AddRange(itemData.ornament.ExtAttr);
                ExtSkill.AddRange(itemData.ornament.ExtSkill);
                var allCount = ExtAttr.Count + ExtSkill.Count;
                var lockList = itemData.ornament.LockList;
                for (int i = 0; i < allCount; i++)
                {
                    uint curInfoId;
                    if (i < ExtAttr.Count)
                    {
                        curInfoId = ExtAttr[i].InfoId;
                    }
                    else
                    {
                        curInfoId = ExtSkill[i - ExtAttr.Count].InfoId;
                    }
                    if (!lockList.Contains(curInfoId))
                    {
                        for (int j = 0; j < cacheShowList.Count; j++)
                        {
                            if(cacheShowList[j] == 0)
                            {
                                cacheShowList[j] = curInfoId;
                                break;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region event
        private void OnBtnRecastClick()
        {
            if (!isRecasting)
            {
                isRecasting = true;
                timerBtnStop?.Cancel();
                timerBtnStop = Timer.Register(0.5f, ResetBtnState);
                if (itemData != null)
                {
                    CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(itemData.Id);
                    //先判断一下属性是否都锁住了
                    var allAttrCount = itemData.ornament.ExtAttr.Count + itemData.ornament.ExtSkill.Count;
                    var lockList = itemData.ornament.LockList;
                    if (lockList.Count >= allAttrCount)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000574));//所有属性都已锁定，无法重铸
                        return;
                    }
                    uint costValue = ornament.reforge_cost[0][1];
                    long hasNum = Sys_Bag.Instance.GetItemCount(costItem.id);
                    if (hasNum >= costValue)
                    {
                        if (itemData.IsLocked)
                        {
                            PromptBoxParameter.Instance.Clear();
                            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, LanguageHelper.GetTextContent(itemData.cSVItemData.name_id), LanguageHelper.GetTextContent(itemData.cSVItemData.name_id));
                            PromptBoxParameter.Instance.SetConfirm(true, () =>
                            {
                                Sys_Bag.Instance.OnItemLockReq(itemData.Uuid, false);
                            });
                            PromptBoxParameter.Instance.SetCancel(true, null);
                            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                            return;
                        }
                        
                        if (needRecastConfirmHint && !Sys_Ornament.Instance.UnShowRecastHint)
                        {
                            Sys_Ornament.Instance.PopupOrnamentRecastConfirmHintView(() =>
                            {
                                Sys_Ornament.Instance.OrnamentRebuildReq(itemData.Uuid);
                            });
                        }
                        else
                        {
                            Sys_Ornament.Instance.OrnamentRebuildReq(itemData.Uuid);
                        }
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000558));//饰品精华不足
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000557));//请先选中要重铸的饰品
                }
            }
        }
        private void OnBtnDetailClick()
        {

        }
        private void OnBtnCostItemClick()
        {
            if (costItem != null)
            {
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(costItem.id, 0, false, false, false, false, false, false, true);
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Ornament, itemData));
            }
        }
        private void OnBtnItemIconClick()
        {
            if (itemData != null)
            {
                OrnamentTipsData tipData = new OrnamentTipsData
                {
                    equip = itemData,
                    isCompare = false,
                    isShowOpBtn = false,
                };
                UIManager.OpenUI(EUIID.UI_Tips_Ornament, false, tipData);
            }
        }
        private void OnCellBtnLockClick(uint infoId)
        {
            //防止连点
            if (isBtnCD)
                return;
            isBtnCD = true;
            btnCDTimer?.Cancel();
            btnCDTimer = null;
            btnCDTimer = Timer.Register(0.3f, () =>
            {
                isBtnCD = false;
            });
            if (itemData != null && itemData.ornament != null)
            {
                Sys_Ornament.Instance.FixedOrnamentLockList(itemData);
                var curLockList = itemData.ornament.LockList;
                bool lockState = curLockList.Contains(infoId) || Sys_Ornament.Instance.CheckIsSpecialInfoId(infoId);
                if (!lockState)
                {
                    CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(itemData.Id);
                    //从解锁变为锁定状态
                    var maxCount = ornament.lock_reforge_cost.Count;
                    if (curLockList.Count >= maxCount)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000570, maxCount.ToString()));//最多同时锁定{0}条额外属性
                        return;
                    }
                    var allAttrCount = itemData.ornament.ExtAttr.Count + itemData.ornament.ExtSkill.Count;
                    if (curLockList.Count >= allAttrCount - 1)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000571));//至少1条额外属性要处于未锁定状态
                        return;
                    }
                    //二次确认
                    Sys_Ornament.Instance.PopupOrnamentRecastLockConfirmHintView(() =>
                    {
                        Sys_Ornament.Instance.OrnamentLockReq(curItemUid, infoId, true);
                    });
                }
                else
                {
                    if (Sys_Ornament.Instance.CheckIsSpecialInfoId(infoId))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000573));//时光祝福属性无法解锁
                        return;
                    }
                    //取消不用二次确认
                    Sys_Ornament.Instance.OrnamentLockReq(curItemUid, infoId, false);
                }
            }
        }
        public void OnRecastLockResBack(uint infoId, bool isLock)
        {
            if (isLock)
            {
                for (int i = 0; i < cacheShowList.Count; i++)
                {
                    if (cacheShowList[i] == infoId)
                    {
                        //当前位置上锁
                        cacheLockList[i] = infoId;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < cacheLockList.Count; i++)
                {
                    if (cacheLockList[i] == infoId)
                    {
                        //清除锁定状态
                        cacheLockList[i] = 0;
                        break;
                    }
                }
            }
            UpdateView(curItemUid);
        }
        #endregion

        public class UI_Ornament_Recast_AttrCellView : UIComponent
        {
            private uint infoId;
            private int index;

            public Text txtAttr;
            public Text txtValue;
            private Button BtnLock;
            private GameObject goLock;
            private GameObject goUnlock;
            private Action<uint> action;

            protected override void Loaded()
            {
                txtAttr = transform.Find("Text_Property").GetComponent<Text>();
                txtValue = transform.Find("Text_Num").GetComponent<Text>();
                BtnLock = transform.Find("Btn_Lock")?.GetComponent<Button>();
                if (BtnLock != null)
                {
                    BtnLock.onClick.AddListener(OnBtnLockClick);
                    goLock = transform.Find("Btn_Lock/Lock").gameObject;
                    goUnlock = transform.Find("Btn_Lock/Unlock").gameObject;
                }
            }

            public void UpdateView(int key, uint value = 0)
            {
                if (key > 0)
                {
                    //暂时不会走到这里
                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData((uint)key);
                    txtAttr.text = LanguageHelper.GetTextContent(attrData.name);
                    txtValue.text = Sys_Attr.Instance.GetAttrValue(attrData, value);
                }
                else if (key == 0)
                {
                    txtAttr.text = LanguageHelper.GetTextContent(680000556);//属性重新随机生成
                    txtValue.text = "";
                    SetLockState(false);
                }
                else
                {
                    txtAttr.text = "";
                    txtValue.text = "";
                    SetLockState(false);
                }
            }
            public void UpdateView(Ornament.Types.ExtAttr attr, int _index = -1)
            {
                index = _index;
                if (attr != null)
                {
                    infoId = attr.InfoId;
                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attr.AttrId);
                    txtAttr.text = LanguageHelper.GetTextContent(attrData.name);
                    txtValue.text = Sys_Ornament.Instance.GetExtAttrShowString(attr.AttrValue, attr.InfoId);
                }
                else
                {
                    UpdateView(-1);
                }
            }
            public void UpdateView(Ornament.Types.ExtSkill skill, int _index = -1)
            {
                index = _index;
                if (skill != null)
                {
                    infoId = skill.InfoId;
                    CSVPassiveSkillInfo.Data skillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skill.SkillId);
                    txtAttr.text = LanguageHelper.GetTextContent(skillInfoData.name);
                    txtValue.text = Sys_Ornament.Instance.GetPassiveSkillShowString(skill.InfoId, skillInfoData);
                }
                else
                {
                    UpdateView(-1);
                }
            }
            public void SetLockState(bool showLock, bool isLock = false)
            {
                if (BtnLock != null)
                {
                    BtnLock.gameObject.SetActive(showLock);
                    goLock.SetActive(isLock);
                    goUnlock.SetActive(!isLock);
                }
            }
            private void OnBtnLockClick()
            {
                action?.Invoke(infoId);
            }
            public void RegisterLockAction(Action<uint> act)
            {
                action = act;
            }
        }
    }
}
