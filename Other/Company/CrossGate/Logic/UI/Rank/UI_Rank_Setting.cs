using Lib.Core;
using Logic.Core;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Rank_Setting_Layout : UILayoutBase
    {
        public Button btnExit;
        public Button btnSure;

        public Transform itemGroupProto;

        public void Parse(GameObject root)
        {
            this.Init(root);
            this.btnExit = this.transform.Find("Animator/View_Title/Btn_Close").GetComponent<Button>();
            this.itemGroupProto = this.transform.Find("Animator/Scroll_View/View_Choice/Proto");
        }

        public void RegisterEvents(IListener listener)
        {
            this.btnExit.onClick.AddListener(listener.OnBtnExitClicked);
        }

        public interface IListener
        {
            void OnBtnExitClicked();
        }
    }

    public class UI_Rank_Setting : UIBase, UI_Rank_Setting_Layout.IListener
    {
        public class Item : UIComponent
        {
            public Text itemName;
            public CP_Toggle toggle;

            public uint rankMainId;
            public UI_Rank_Setting ui;

            protected override void Loaded()
            {
                this.itemName = this.transform.Find("Text").GetComponent<Text>();
                this.toggle = this.transform.Find("itemProto").GetComponent<CP_Toggle>();

                this.toggle.onValueChanged.AddListener(this.OnValueChanged);
            }

            public void Refresh(UI_Rank_Setting ui, uint rankMainId)
            {
                this.ui = ui;
                this.rankMainId = rankMainId;

                CSVRanklistmain.Data rankMainData = CSVRanklistmain.Instance.GetConfData(rankMainId);
                if (rankMainData != null)
                {
                    TextHelper.SetText(this.itemName, rankMainData.Name);
                }
                bool show = Sys_Rank.Instance.GetRankSetting().GetSubTypeState(rankMainId);
                this.toggle.SetSelected(show, false);
            }

            private void OnValueChanged(bool selected)
            {
                this.ui.SetSelected(this.rankMainId, selected);
            }

            public void SetSelected(bool toSelected)
            {
                this.toggle.SetSelected(toSelected, true);
            }
        }
        public class ItemGroup : UIComponent
        {
            public Text itemTypeName;
            public Transform itemProto;

            public COWVd<Item> items = new COWVd<Item>();

            public UI_Rank_Setting ui;
            public uint rankGroupType;

            protected override void Loaded()
            {
                itemTypeName = transform.Find("Image_Title/Text_Title").GetComponent<Text>();
                itemProto = transform.Find("SrollItem/Image");
            }

            public void SetSelected(bool toSelected)
            {
                for (int i = 0, length = this.items.RealCount; i < length; i++)
                {
                    this.items[i].SetSelected(toSelected);
                }
            }

            public void Refresh(UI_Rank_Setting ui, uint rankGroupType)
            {
                this.ui = ui;
                this.rankGroupType = rankGroupType;

                var ls = Sys_Rank.Instance.RankGroup[rankGroupType];
                items.TryBuildOrRefresh(itemProto.gameObject, itemProto.parent, ls.Count, (item, index) => {
                    item.Refresh(ui, ls[index]);
                });

                CSVRanklistsort.Data rankTypeData = CSVRanklistsort.Instance.GetConfData(rankGroupType);
                
                
                if (null != rankTypeData)
                {
                    StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
                    stringBuilder.Append(LanguageHelper.GetTextContent(rankTypeData.langid));
                    stringBuilder.Append(LanguageHelper.GetTextContent(2022105));
                    TextHelper.SetText(itemTypeName, StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder));
                }
                else
                {
                    DebugUtil.Log(ELogType.eNone, $"CSVRanklistsort.Data not find id = {rankGroupType}");
                }
            }
        }

        public UI_Rank_Setting_Layout Layout = new UI_Rank_Setting_Layout();
        public COWVd<ItemGroup> groups = new COWVd<ItemGroup>();

        private RankSetting tempRank = new RankSetting();
        protected override void OnLoaded()
        {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);
        }

        protected override void OnShow()
        {
            RankSetting ranSt = Sys_Rank.Instance.GetRankSetting();
            if (null != ranSt)
            {
                tempRank.SetSettingData(ranSt.GetSettingData(), ranSt.GetnextSetTime());
                this.groups.TryBuildOrRefresh(this.Layout.itemGroupProto.gameObject, this.Layout.itemGroupProto.parent, Sys_Rank.Instance.RankGroup.Count, (itemGroup, index) => {
                    uint key = Sys_Rank.Instance.rankGroupList[index];
                    itemGroup.Refresh(this, key);
                });

                Lib.Core.FrameworkTool.ForceRebuildLayout(gameObject);
            }
        }

        public void SetAllSelected(bool toSelected)
        {
            for (int i = 0, length = this.groups.RealCount; i < length; i++)
            {
                this.groups[i].SetSelected(toSelected);
            }
        }

        public void SetSelected(uint rankType, bool toSelected)
        {
            if(toSelected)
            {
                tempRank.SetBitvalue(rankType);
            }
            else
            {
                tempRank.SetBitValueZeo(rankType);
            }
        }

        public void OnBtnExitClicked()
        {
            if(tempRank.GetnextSetTime() <= Sys_Time.Instance.GetServerTime())
            {
                Sys_Rank.Instance.RankSetStateReq(tempRank.GetSettingData());
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(560000003));
            }
            this.CloseSelf();
        }
    }
}
