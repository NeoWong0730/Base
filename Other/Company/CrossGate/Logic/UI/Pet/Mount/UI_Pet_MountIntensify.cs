using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using System.Text;

namespace Logic
{
    public class UI_Pet_MountIntensify_Layout
    {
        private Button closeBtn;
        private Button upgradeBtn;
        public Text descText;
        public Image currentImage;
        public Image nextcurrentImage;
        public GameObject arrowGo;
        public GameObject fullGo;
        public GameObject itemInfoGo;
        public Text costText;
        public List<PropItem> propItems = new List<PropItem>();
        public Animator ani;

        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Animator/View_TipsBgNew03/Btn_Close").GetComponent<Button>();
            upgradeBtn = transform.Find("Animator/View_Info/Des2/Btn_01").GetComponent<Button>();
            descText = transform.Find("Animator/View_Info/Des1/Text_Des").GetComponent<Text>();
            currentImage = transform.Find("Animator/View_Info/Image_bg/Level/Image_Icon").GetComponent<Image>();
            nextcurrentImage = transform.Find("Animator/View_Info/Image_bg/Level (1)/Image_Icon").GetComponent<Image>();
            fullGo = transform.Find("Animator/View_Info/Text_Full").gameObject;
            arrowGo = transform.Find("Animator/View_Info/Image_bg/Image_Arrow").gameObject;
            itemInfoGo = transform.Find("Animator/View_Info/Des2").gameObject;
            costText = transform.Find("Animator/View_Info/Des2/Text_Tips").GetComponent<Text>();
            ani = transform.Find("Animator/View_Info").GetComponent<Animator>();

            PropItem item1 = new PropItem();
            item1.BindGameObject(transform.Find("Animator/View_Info/Des2/Group/PropItem").gameObject);
            propItems.Add(item1);
            PropItem item2 = new PropItem();
            item2.BindGameObject(transform.Find("Animator/View_Info/Des2/Group/PropItem (1)").gameObject);
            propItems.Add(item2);
            PropItem item3 = new PropItem();
            item3.BindGameObject(transform.Find("Animator/View_Info/Des2/Group/PropItem (2)").gameObject);
            propItems.Add(item3);
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            upgradeBtn.onClick.AddListener(listener.UpgradeBtnClicked);

        }
        public interface IListener
        {
            void CloseBtnClicked();
            void UpgradeBtnClicked();
        }
    }

    public class UI_Pet_MountIntensify : UIBase, UI_Pet_MountIntensify_Layout.IListener
    {
        private UI_Pet_MountIntensify_Layout layout = new UI_Pet_MountIntensify_Layout();
        private uint currentPetUid;
        private List<ulong> selectItems = new List<ulong>();
        uint needQuality = 0;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<int, ulong>(Sys_Pet.EEvents.OnMountSkillItemSelect, OnSelectSkillItem, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnContractLevelUpRes, OnContractLevelUpRes, toRegister);
        }

        private void OnContractLevelUpRes()
        {
            layout.ani.Play("Strengthen", -1, 0f);
            selectItems.Clear();
            RefreshView();
        }

        protected override void OnOpen(object arg = null)
        {
            currentPetUid = Convert.ToUInt32(arg);
        }

        protected override void OnShow()
        {
            RefreshView();
        }
        
        private void RefreshView()
        {
            var pet = Sys_Pet.Instance.GetPetByUId(currentPetUid);
            if(null == pet)
            {
                UIManager.CloseUI(EUIID.UI_Pet_MountIntensify);
            }
            else
            {
                var currentLevel = pet.ContractLevel;
                if (null != pet.mountData)
                {
                    ImageHelper.SetIcon(layout.currentImage, 330000 + currentLevel);
                    StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
                    CSVPetMountStrengthen.Data currentMountPetIntensifyData = CSVPetMountStrengthen.Instance.GetMountPetIntensifyData(currentLevel, pet.mountData.strengthen_type);
                    if (null != currentMountPetIntensifyData)
                    {

                        stringBuilder.Append(LanguageHelper.GetTextContent(1018620, (currentMountPetIntensifyData.attribute_bonus / 100).ToString())); //万分比-百分比显示
                        CSVPetMountStrengthen.Data nextMountPetIntensifyData = CSVPetMountStrengthen.Instance.GetMountPetIntensifyData(currentLevel + 1, pet.mountData.strengthen_type);
                        bool hasNext = null != nextMountPetIntensifyData;
                        if (hasNext)
                        {
                            uint needCount = 0;
                            if (null != currentMountPetIntensifyData.strengthen_cost)
                            {
                                layout.costText.gameObject.SetActive(false);
                                needCount = (uint)currentMountPetIntensifyData.strengthen_cost.Count;
                                for (int i = 0; i < currentMountPetIntensifyData.strengthen_cost.Count; i++)
                                {
                                    if (currentMountPetIntensifyData.strengthen_cost[i].Count >= 2)
                                    {
                                        var itemId = currentMountPetIntensifyData.strengthen_cost[i][0];
                                        var itemNum = currentMountPetIntensifyData.strengthen_cost[i][1];
                                        if (i < layout.propItems.Count)
                                        {
                                            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(itemId, itemNum, true, false, false, false, false, true, true, true);
                                            layout.propItems[i].SetData(itemN, EUIID.UI_Pet_MountIntensify);
                                            layout.propItems[i].Layout.imgIcon.enabled = true;
                                            layout.propItems[i].transform.gameObject.SetActive(true);
                                        }
                                        else
                                        {
                                            PropItem propItem = new PropItem();
                                            GameObject go = GameObject.Instantiate(layout.propItems[0].transform.gameObject, layout.propItems[0].transform.parent, false);
                                            propItem.BindGameObject(go);
                                            layout.propItems.Add(propItem);
                                            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(itemId, itemNum, true, false, false, false, false, true, true, true);
                                            propItem.SetData(itemN, EUIID.UI_Pet_MountIntensify);
                                            propItem.Layout.imgIcon.enabled = true;
                                            propItem.transform.gameObject.SetActive(true);
                                        }

                                    }
                                }
                            }
                            else
                            {
                                if (null != currentMountPetIntensifyData.advanced_cost && currentMountPetIntensifyData.advanced_cost.Count >= 2)
                                {
                                    needCount = currentMountPetIntensifyData.advanced_cost[1];
                                    needQuality = currentMountPetIntensifyData.advanced_cost[0];
                                    TextHelper.SetText(layout.costText, 1018604, needCount.ToString(), LanguageHelper.GetTextContent(1018610 + needQuality));
                                    layout.costText.gameObject.SetActive(true);
                                    for (int i = 0; i < needCount; i++)
                                    {
                                        if (i >= selectItems.Count)
                                        {
                                            selectItems.Add(0);
                                        }
                                        uint itemId = 0;
                                        var itemuUId = selectItems[i];
                                        var itemData = Sys_Bag.Instance.GetItemDataByUuid(itemuUId);
                                        if (null != itemData)
                                        {
                                            itemId = itemData.Id;
                                        }
                                        PropItem showItem = null;
                                        PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(itemId, 0, true, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
                                        itemN.ceilIndex = i;
                                        if (i < layout.propItems.Count)
                                        {
                                            showItem = layout.propItems[i];
                                        }
                                        else
                                        {
                                            PropItem propItem = new PropItem();
                                            GameObject go = GameObject.Instantiate(layout.propItems[0].transform.gameObject, layout.propItems[0].transform.parent, false);
                                            propItem.BindGameObject(go);
                                            layout.propItems.Add(propItem);
                                            showItem = propItem;
                                        }
                                        showItem.SetData(itemN, EUIID.UI_Pet_MountIntensify);

                                        if (itemId == 0)
                                        {
                                            showItem.txtNumber.gameObject.SetActive(false);
                                            showItem.btnNone.gameObject.SetActive(true);
                                            showItem.Layout.imgIcon.enabled = false;
                                            showItem.Layout.imgQuality.gameObject.SetActive(false);
                                            showItem.transform.gameObject.SetActive(true);
                                        }
                                        else
                                        {
                                            showItem.btnNone.gameObject.SetActive(false);
                                            showItem.Layout.imgIcon.enabled = true;
                                            showItem.transform.gameObject.SetActive(true);
                                        }
                                    }


                                }
                            }

                            for (int i = (int)needCount; i < layout.propItems.Count; i++)
                            {
                                layout.propItems[i].transform.gameObject.SetActive(false);
                            }

                            ImageHelper.SetIcon(layout.nextcurrentImage, 330000 + currentLevel + 1);
                            stringBuilder.Append(LanguageHelper.GetTextContent(1018621, ((nextMountPetIntensifyData.attribute_bonus - currentMountPetIntensifyData.attribute_bonus) / 100).ToString())); //万分比-百分比显示

                            List<uint> exSkillsLevel = new List<uint>();
                            uint showLevel = 0;
                            for (int i = 0; i < CSVPetMountStrengthen.Instance.Count; i++)
                            {
                                var tempData = CSVPetMountStrengthen.Instance.GetByIndex(i);
                                if (null != tempData && pet.mountData.strengthen_type == tempData.type)
                                {
                                    if (currentMountPetIntensifyData.extra_skill_grid < tempData.extra_skill_grid)
                                    {
                                        showLevel = tempData.level;
                                        break;
                                    }
                                }
                            }
                            if (showLevel > 0)
                            {
                                stringBuilder.Append("\n");
                                stringBuilder.Append(LanguageHelper.GetTextContent(1018622, showLevel.ToString()));
                            }
                        }
                        layout.fullGo.SetActive(!hasNext);
                        layout.arrowGo.SetActive(hasNext);
                        layout.itemInfoGo.SetActive(hasNext);
                        layout.nextcurrentImage.transform.parent.gameObject.SetActive(hasNext);
                    }

                    layout.descText.text = StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder);
                }
            }
            
        }

        private void ItemGridBeClicked(PropItem propItem)
        {
            UIManager.OpenUI(EUIID.UI_Pet_MountSkillSelectItem, false, new UI_SelectMountSkillParam
            {
                tittle_langId = 1018625,
                useType = 1,
                fiters = selectItems,
                selectIndex = propItem.ItemData.ceilIndex,
                itemQuailty = needQuality,
            }, EUIID.UI_Pet_MountIntensify);
        }

        private void OnSelectSkillItem(int index, ulong uuid)
        {
            if (0 <= index && index < selectItems.Count)
            {
                selectItems[index] = uuid;
                SetProItemInfo(index);
            }
        }

        private void SetProItemInfo(int index)
        {
            uint itemId = 0;
            var itemuUId = selectItems[index];
            var itemData = Sys_Bag.Instance.GetItemDataByUuid(itemuUId);
            if (null != itemData)
            {
                itemId = itemData.Id;
            }
            PropItem showItem = null;
            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(itemId, 0, true, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
            if (index < layout.propItems.Count)
            {
                showItem = layout.propItems[index];
            }
            else
            {
                PropItem propItem = new PropItem();
                GameObject go = GameObject.Instantiate(layout.propItems[0].transform.gameObject, layout.propItems[0].transform.parent, false);
                propItem.BindGameObject(go);
                layout.propItems.Add(propItem);
                showItem = propItem;
            }
            showItem.SetData(itemN, EUIID.UI_Pet_MountIntensify);

            if (itemId == 0)
            {
                showItem.txtNumber.gameObject.SetActive(false);
                showItem.btnNone.gameObject.SetActive(true);
                showItem.Layout.imgIcon.enabled = false;
                showItem.Layout.imgQuality.gameObject.SetActive(false);
                showItem.transform.gameObject.SetActive(true);
            }
            else
            {
                showItem.btnNone.gameObject.SetActive(false);
                showItem.Layout.imgIcon.enabled = true;
                showItem.transform.gameObject.SetActive(true);
            }
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_MountIntensify);
        }

        public void UpgradeBtnClicked()
        {
            var pet = Sys_Pet.Instance.GetPetByUId(currentPetUid);
            var currentLevel = pet.ContractLevel;
            if (null != pet.mountData)
            {
                CSVPetMountStrengthen.Data currentMountPetIntensifyData = CSVPetMountStrengthen.Instance.GetMountPetIntensifyData(currentLevel, pet.mountData.strengthen_type);
                if (null != currentMountPetIntensifyData)
                {
                    if (null != currentMountPetIntensifyData.strengthen_cost)
                    {
                        for (int i = 0; i < currentMountPetIntensifyData.strengthen_cost.Count; i++)
                        {
                            if (currentMountPetIntensifyData.strengthen_cost[i].Count >= 2)
                            {
                                var itemId = currentMountPetIntensifyData.strengthen_cost[i][0];
                                var itemNum = currentMountPetIntensifyData.strengthen_cost[i][1];
                                ItemIdCount itemIdCount = new ItemIdCount(itemId, itemNum);
                                if (!itemIdCount.Enough)
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1018608));
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (selectItems.Contains(0))
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1018609));
                            return;
                        }
                    }
                }

                Sys_Pet.Instance.OnPetContractLevelUpReq(currentPetUid, selectItems);
            }
        }
    }
}