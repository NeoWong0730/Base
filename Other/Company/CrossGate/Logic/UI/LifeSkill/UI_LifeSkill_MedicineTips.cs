using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Logic
{
    public class UI_LifeSkill_MedicineTips : UIBase
    {
        private Transform parent1;
        private Transform parent2;
        private Transform parent3;
        private GameObject title1;
        private GameObject title2;
        private GameObject title3;

        private uint formulaId;
        private uint formulasuccessItem;
        private uint forextraItems;
        private uint forfailItems;
        private List<Button> eventButtons = new List<Button>();
        private Button closeBtn;

        protected override void OnOpen(object arg)
        {
            formulaId = (uint)arg;
            formulasuccessItem = CSVFormula.Instance.GetConfData(formulaId).forge_success_item;
            forextraItems = CSVFormula.Instance.GetConfData(formulaId).extra_item;
            forfailItems = CSVFormula.Instance.GetConfData(formulaId).forge_fail_item;
        }

        protected override void OnLoaded()
        {
            title1 = transform.Find("Animator/Tips01/Title01").gameObject;
            title2 = transform.Find("Animator/Tips01/Title02").gameObject;
            title3 = transform.Find("Animator/Tips01/Title03").gameObject;
            parent1 = transform.Find("Animator/Tips01/Scroll_View01/Viewport");
            parent2 = transform.Find("Animator/Tips01/Scroll_View02/Viewport");
            parent3 = transform.Find("Animator/Tips01/Scroll_View03/Viewport");
            closeBtn = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_LifeSkill_MedicineTips);
            });
        }

        protected override void OnShow()
        {
            foreach (var item in eventButtons)
            {
                item.onClick.RemoveAllListeners();
            }
            Refresh();
        }

        private void Refresh()
        {
            if (formulasuccessItem == 0)
            {
                title1.SetActive(false);
                FrameworkTool.CreateChildList(parent1, 0);
            }
            else
            {
                title1.SetActive(true);
                List<ItemIdCount> itemIdCounts = CSVDrop.Instance.GetDropItem(formulasuccessItem);
                int needCount = itemIdCounts.Count;
                FrameworkTool.CreateChildList(parent1, needCount);
                for (int i = 0; i < needCount; i++)
                {
                    GameObject go = parent1.GetChild(i).gameObject;
                    Image image = go.transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                    Image quality = go.transform.Find("Btn_Item/Image_BG").GetComponent<Image>();
                    Text num = go.transform.Find("Text_Number").GetComponent<Text>();
                    Text name = go.transform.Find("Text_Name").GetComponent<Text>();
                    uint itemId = itemIdCounts[i].id;
                    long count = itemIdCounts[i].count;
                    CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(itemId);
                    if (cSVItemData != null)
                    {
                        TextHelper.SetText(name, cSVItemData.name_id);
                        ImageHelper.SetIcon(image, cSVItemData.icon_id);
                        ImageHelper.GetQualityColor_Frame(quality, (int)cSVItemData.quality);
                        TextHelper.SetText(num, count.ToString());
                        Button button = go.transform.Find("Btn_Item").GetComponent<Button>();
                        button.onClick.AddListener(() =>
                        {
                            UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_LifeSkill_MedicineTips,
                                new PropIconLoader.ShowItemData(itemId, count, true, false, false, false, false)));
                        });
                        eventButtons.Add(button);
                    }
                }
            }
            if (forextraItems == 0)
            {
                title2.SetActive(false);
                FrameworkTool.CreateChildList(parent2, 0);
            }
            else
            {
                title2.SetActive(true);
                List<ItemIdCount> itemIdCounts = CSVDrop.Instance.GetDropItem(forextraItems);
                int needCount = itemIdCounts.Count;
                FrameworkTool.CreateChildList(parent2, needCount);
                for (int i = 0; i < needCount; i++)
                {
                    GameObject go = parent2.GetChild(i).gameObject;
                    Image image = go.transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                    Image quality = go.transform.Find("Btn_Item/Image_BG").GetComponent<Image>();
                    Text num = go.transform.Find("Text_Number").GetComponent<Text>();
                    Text name = go.transform.Find("Text_Name").GetComponent<Text>();
                    uint itemId = itemIdCounts[i].id;
                    long count = itemIdCounts[i].count;
                    CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(itemId);
                    if (cSVItemData != null)
                    {
                        TextHelper.SetText(name, cSVItemData.name_id);
                        ImageHelper.SetIcon(image, cSVItemData.icon_id);
                        ImageHelper.GetQualityColor_Frame(quality, (int)cSVItemData.quality);
                        TextHelper.SetText(num, count.ToString());
                        Button button = go.transform.Find("Btn_Item").GetComponent<Button>();
                        button.onClick.AddListener(() =>
                        {
                            UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_LifeSkill_MedicineTips,
                                new PropIconLoader.ShowItemData(itemId, count, true, false, false, false, false)));
                        });
                        eventButtons.Add(button);
                    }
                }
            }
            if (forfailItems == 0)
            {
                title3.SetActive(false);
                FrameworkTool.CreateChildList(parent3, 0);
            }
            else
            {
                title3.SetActive(true);
                List<ItemIdCount> itemIdCounts = CSVDrop.Instance.GetDropItem(forfailItems);
                int needCount = itemIdCounts.Count;
                FrameworkTool.CreateChildList(parent3, needCount);
                for (int i = 0; i < needCount; i++)
                {
                    GameObject go = parent3.GetChild(i).gameObject;
                    Image image = go.transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                    Image quality = go.transform.Find("Btn_Item/Image_BG").GetComponent<Image>();
                    Text num = go.transform.Find("Text_Number").GetComponent<Text>();
                    Text name = go.transform.Find("Text_Name").GetComponent<Text>();
                    uint itemId = itemIdCounts[i].id;
                    long count = itemIdCounts[i].count;
                    CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(itemId);
                    if (cSVItemData != null)
                    {
                        TextHelper.SetText(name, cSVItemData.name_id);
                        ImageHelper.SetIcon(image, cSVItemData.icon_id);
                        ImageHelper.GetQualityColor_Frame(quality, (int)cSVItemData.quality);
                        TextHelper.SetText(num, count.ToString());
                        Button button = go.transform.Find("Btn_Item").GetComponent<Button>();
                        button.onClick.AddListener(() =>
                        {
                            UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_LifeSkill_MedicineTips,
                                new PropIconLoader.ShowItemData(itemId, count, true, false, false, false, false)));
                        });
                        eventButtons.Add(button);
                    }
                }
            }
        }
    }
}


