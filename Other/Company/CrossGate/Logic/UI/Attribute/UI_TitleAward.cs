using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Framework;
using Table;
using Lib.Core;
using System;
using System.Text;

namespace Logic
{
    public class UI_TitleAward : UIBase
    {
        public class TitleReward
        {
            public int index;
            public bool get;

            public TitleReward(int _index,bool _get)
            {
                index = _index;
                get = _get;
            }
        }

        private TitleSeries titleSeries;
        private Transform parent;
        private List<TitleReward> titleRewards = new List<TitleReward>();
        private Image eventBg;
        private GameObject root;

        protected override void OnOpen(object arg)
        {
            titleSeries = arg as TitleSeries;
        }


        protected override void OnLoaded()
        {
            root = transform.Find("Animator/Image_Bg").gameObject;
            parent = transform.Find("Animator/Image_Bg/Scroll_View01/Viewport");
            eventBg = transform.Find("Animator/eventBg").GetComponent<Image>();
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventBg);
            eventListener.AddEventListener(EventTriggerType.PointerClick, (_) => { UIManager.CloseUI(EUIID.UI_TitleAward); });
        }

        protected override void OnShow()
        {
            Refresh();
        }

        private void Refresh()
        {
            titleRewards.Clear();
            int _index0 = 0;
            int _index2 = 0;
            foreach (var item in titleSeries.rewardState)
            {
                if (item == 0) 
                {
                    TitleReward titleReward = new TitleReward(_index0,false);
                    titleRewards.Add(titleReward);
                }
                _index0++;
            }
            foreach (var item in titleSeries.rewardState)
            {
                if (item == 2)
                {
                    TitleReward titleReward = new TitleReward(_index2,true);
                    titleRewards.Add(titleReward);
                }
                _index2++;
            }
            int childCount = titleRewards.Count;
            FrameworkTool.CreateChildList(parent, childCount);
            for (int i = 0; i < childCount; i++)
            {
                Transform child = parent.GetChild(i);
                Text num = child.Find("Text_Num").GetComponent<Text>();
                num.text = titleSeries.cSVTitleSeriesData.seriesCollect[titleRewards[i].index][0].ToString();

                uint dropId = titleSeries.cSVTitleSeriesData.seriesCollect[titleRewards[i].index][1];
                List<ItemIdCount> itemIdCounts= CSVDrop.Instance.GetDropItem(dropId);

                //CSVDrop.Data cSVDropData = CSVDrop.Instance.GetConfData(dropId);
                int dropCount = itemIdCounts.Count;
                Transform dropParent = child.Find("Scroll_View/Viewport");
                FrameworkTool.CreateChildList(dropParent, dropCount);
                for (int j = 0; j < dropCount; j++)
                {
                    Transform _child = dropParent.GetChild(j);
                    Image icon = _child.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                    GameObject get = _child.Find("Image_Get").gameObject;
                    icon.enabled = true;
                    Text _num = _child.Find("Text_Number").GetComponent<Text>();
                    _num.gameObject.SetActive(true);
                    ImageHelper.SetIcon(icon, itemIdCounts[j].CSV.icon_id);
                    _num.text = itemIdCounts[j].count.ToString();
                    get.SetActive(titleRewards[i].get);
                }
            }
            FrameworkTool.ForceRebuildLayout(root);
        }
    }
}
 

