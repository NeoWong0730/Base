using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_HangupFightOption : UIBase {
        private Text text_title;
        private Text text_level;

        private Transform tr_MonsterNode;
        private Transform tr_MonsterItem;

        private Transform tr_ItemNode;
        private Transform tr_Item;

        protected override void OnLoaded() {
            text_title = transform.Find("Animator/Tip/Image_Title/Text_Title").GetComponent<Text>();
            text_level = transform.Find("Animator/Tip/Image_Lv/Text (1)").GetComponent<Text>();
            tr_MonsterNode = transform.Find("Animator/Tip/Scroll_Pet/Viewport/Content");
            tr_MonsterItem = transform.Find("Animator/Tip/Scroll_Pet/Viewport/Content/Image_Bottom");
            tr_ItemNode = transform.Find("Animator/Tip/Scroll_Reward/Viewport/Content");
            tr_Item = transform.Find("Animator/Tip/Scroll_Reward/Viewport/Content/PropItem");
        }

        private uint id;

        protected override void OnOpen(object arg) {
            this.id = Convert.ToUInt32(arg);
        }

        protected override void OnOpened() {
            CSVHangupLayerStage.Data cSVHangupLayerStageData = CSVHangupLayerStage.Instance.GetConfData(id);
            if (null == cSVHangupLayerStageData) return;
            CSVHangup.Data cSVHangupData = CSVHangup.Instance.GetConfData(cSVHangupLayerStageData.Hangupid);
            if (null == cSVHangupData) return;
            /// <summary> 标题 </summary>
            text_title.text = LanguageHelper.GetTextContent(cSVHangupData.HangupName) + LanguageHelper.GetTextContent(cSVHangupLayerStageData.Name);
            /// <summary> 等级 </summary>
            uint lowerLv = 0, upperLv = 0;
            if (null != cSVHangupLayerStageData.RecommendLv && cSVHangupLayerStageData.RecommendLv.Count >= 2) {
                lowerLv = cSVHangupLayerStageData.RecommendLv[0];
                upperLv = cSVHangupLayerStageData.RecommendLv[1];
            }

            text_level.text = LanguageHelper.GetTextContent(2104014, lowerLv.ToString(), upperLv.ToString());
            /// <summary> 怪物列表 </summary>
            List<uint> list_monseterIcon = cSVHangupLayerStageData.MonseterIcon;
            UI_HangupFight.CreateItemList(tr_MonsterNode, tr_MonsterItem, list_monseterIcon.Count);
            for (int i = 0, count = tr_MonsterNode.childCount; i < count; i++) {
                if (i < list_monseterIcon.Count) {
                    SetMonsterItem(tr_MonsterNode.GetChild(i), list_monseterIcon[i]);
                }
                else {
                    SetMonsterItem(tr_MonsterNode.GetChild(i), 0);
                }
            }

            /// <summary> 道具列表 </summary>
            List<ItemIdCount> list_drop = CSVDrop.Instance.GetDropItem(cSVHangupLayerStageData.Dropid);
            UI_HangupFight.CreateItemList(tr_ItemNode, tr_Item, list_drop.Count);
            List<PropItem> list_PropItem = new List<PropItem>();
            for (int i = 0, count = tr_ItemNode.childCount; i < count; i++) {
                PropItem propItem = new PropItem();
                propItem.BindGameObject(tr_ItemNode.GetChild(i).gameObject);
                list_PropItem.Add(propItem);
            }

            for (int i = 0, count = list_PropItem.Count; i < count; i++) {
                if (i < list_drop.Count) {
                    SetRewardItem(list_PropItem[i], list_drop[i]);
                }
                else {
                    SetRewardItem(list_PropItem[i], null);
                }
            }
        }

        private void SetMonsterItem(Transform tr, uint iconId) {
            if (0 == iconId) {
                tr.gameObject.SetActive(false);
                return;
            }

            tr.gameObject.SetActive(true);
            /// <summary> 图标 </summary>
            Image image_Icon = tr.Find("Image_Icon").GetComponent<Image>();
            ImageHelper.SetIcon(image_Icon, iconId);
        }

        private void SetRewardItem(PropItem propItem, ItemIdCount itemIdCount) {
            if (null == itemIdCount) {
                propItem.SetActive(false);
                return;
            }

            propItem.SetActive(true);
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(itemIdCount.id, itemIdCount.count, true, false, false, false, false, false, false, true, null, true, true);
            propItem.SetData(new MessageBoxEvt(EUIID.UI_HangupFight, itemData));
        }
    }
}