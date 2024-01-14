using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Packet;

namespace Logic
{
    public class UI_Family_Funds_Layout
    {
        private Button closeBtn;
        private Button sureBtn;
        private Text tipsText;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Animator/View_Content/View_Merge/Scroll_View").GetComponent<InfinityGrid>();
            tipsText = transform.Find("Animator/View_Content/View_Merge/Text_01").GetComponent<Text>();
            closeBtn = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            sureBtn = transform.Find("Animator/View_Content/View_Below/Btn_Ok").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            sureBtn.onClick.AddListener(listener.CloseBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
        }

        public void SetNextFundsData()
        {
            var data = CSVFamilyBonus.Instance.GetByIndex(0);

            int index = Sys_Family.Instance.GetFundsDataIndex(data);

            if(index != -1)
            {
                index = index + 1;
            }
            else
            {
                index = 0;
            }
            if (null != data.RewardTrigger && data.RewardTrigger.Count > 0)
            {
                if(index >= data.RewardTrigger.Count)
                {
                    tipsText.text = LanguageHelper.GetTextContent(15175);
                }
                else
                {
                    if(index == 0)
                    {
                        tipsText.text = LanguageHelper.GetTextContent(15172, (data.RewardTrigger[index] - Sys_Family.Instance.GetFundsCoinGainWeekly()).ToString());
                    }
                    else if (index > 0 && index < data.RewardTrigger.Count - 1)
                    {
                        tipsText.text = LanguageHelper.GetTextContent(15173, (data.RewardTrigger[index] - Sys_Family.Instance.GetFundsCoinGainWeekly()).ToString());
                    }
                    else
                    {
                        tipsText.text = LanguageHelper.GetTextContent(15174, (data.RewardTrigger[index] - Sys_Family.Instance.GetFundsCoinGainWeekly()).ToString());
                    }
                }
            }
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class UI_Family_FundsCeil
    {
        private Text positionText;
        private Image careerImage;
        private Text careerText;
        private Text nameText;
        private Image itemImage;
        private Text itemText;
        private Image item2Image;
        private Text item2Text;
        public void Init(Transform transform)
        {
            positionText = transform.Find("Text_Level").GetComponent<Text>();
            careerImage = transform.Find("Image_Prop").GetComponent<Image>();
            careerText = transform.Find("Image_Prop/Text_Profession").GetComponent<Text>();
            nameText = transform.Find("Text_Name").GetComponent<Text>();
            itemImage = transform.Find("Text_Amount/Image_Coin").GetComponent<Image>();
            itemText = transform.Find("Text_Amount").GetComponent<Text>();
            item2Image = transform.Find("Text_Range/Image_Coin").GetComponent<Image>();
            item2Text = transform.Find("Text_Range").GetComponent<Text>();
        }

        public void SetView(UI_Family_FundsEntry fundsEntry)
        {
            uint branchId = fundsEntry.pos / 10000;
            uint position = fundsEntry.pos % 10000;
            if (null != fundsEntry.memberInfo)
            {
                branchId = fundsEntry.memberInfo.Position / 10000;
                position = fundsEntry.memberInfo.Position % 10000;
                nameText.text = fundsEntry.memberInfo.Name.ToStringUtf8();
                CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(fundsEntry.memberInfo.Occ);
                if (null != cSVCareerData)
                {
                    TextHelper.SetText(careerText, cSVCareerData.name);

                    ImageHelper.SetIcon(careerImage, cSVCareerData.logo_icon);

                    careerImage.gameObject.SetActive(true);
                }
                else
                {
                    DebugUtil.LogError($"Not Find {fundsEntry.memberInfo.Occ} In CSVCareer");
                }
            }
            else
            {
                nameText.text = LanguageHelper.GetTextContent(15171);
                careerImage.gameObject.SetActive(false);
            }


            CSVFamilyPostAuthority.Data cSVFamilyPostAuthorityData = CSVFamilyPostAuthority.Instance.GetConfData(position);
            if (null == cSVFamilyPostAuthorityData)
            {
                positionText.text = string.Empty;
            }
            /*else if (BranchId > 0)
            {
                var BranchInfo = Sys_Family.Instance.familyData.CheckBranchInfo(BranchId);
                string BranchName = BranchInfo == null ? string.Empty : BranchInfo.Name.ToStringUtf8();
                positionText.text = string.Concat(BranchName, LanguageHelper.GetTextContent(cSVFamilyPostAuthorityData.PostName));
                
            }*/
            else
            {
                positionText.text = LanguageHelper.GetTextContent(cSVFamilyPostAuthorityData.PostName);
            }

            CSVItem.Data fundsItem = CSVItem.Instance.GetConfData(Sys_Family.Instance.FundsItemId);
            if (null != fundsItem)
            {
                ImageHelper.SetIcon(item2Image, fundsItem.small_icon_id);
                ImageHelper.SetIcon(itemImage, fundsItem.small_icon_id);
            }
            CSVFamilyBonus.Data cSVFamilyBonus = CSVFamilyBonus.Instance.GetConfData(position);
            if (null != cSVFamilyBonus && null != cSVFamilyBonus.Reward)
            {
                if (cSVFamilyBonus.Reward.Count >= 2)
                {
                    item2Text.text = string.Format($"{cSVFamilyBonus.Reward[0].ToString()}-{cSVFamilyBonus.Reward[cSVFamilyBonus.Reward.Count - 1].ToString()}");
                }
                else if (cSVFamilyBonus.Reward.Count > 0)
                {
                    item2Text.text = cSVFamilyBonus.Reward[0].ToString();
                }
                else
                {
                    DebugUtil.LogError($"CSVFamilyBonus.Reward figures Wrong, Please Check Check");
                }
            }
            var fundsCount = Sys_Family.Instance.GetFundsByPositionAndWeeklyCoin(position);
            if(fundsCount == 0)
            {
                itemText.text = LanguageHelper.GetTextContent(15171);
                itemImage.gameObject.SetActive(false);
            }
            else
            {
                itemImage.gameObject.SetActive(true);
                itemText.text = fundsCount.ToString();
            }
            
        }
    }

    public class UI_Family_FundsEntry
    {
        public uint pos;
        public CmdGuildGetMemberInfoAck.Types.MemberInfo memberInfo;
    }

    public class UI_Family_Funds : UIBase, UI_Family_Funds_Layout.IListener
    {
        private UI_Family_Funds_Layout layout = new UI_Family_Funds_Layout();
        /// <summary> 成员信息 </summary>
        private List<UI_Family_FundsEntry> fundsEntrys = new List<UI_Family_FundsEntry>(16);
        private List<uint> fundsPosition;

        public List<uint> FundsPosition
        {
            get
            {
                if (null == fundsPosition)
                {
                    fundsPosition = new List<uint>(4);
                    var count = CSVFamilyBonus.Instance.Count;
                    for (int i = 0; i < count; i++)
                    {
                        fundsPosition.Add(CSVFamilyBonus.Instance.GetByIndex(i).id);
                    }
                    fundsPosition.Reverse();
                }
                return fundsPosition;
            }
        }

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }

        protected override void OnOpen(object arg = null)
        {
            
        }

        protected override void OnShow()
        {
            SetData();
            RefreshView();
        }

        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnClose()
        {

        }

        private void SetData()
        {
            fundsEntrys.Clear();
            var members = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetMemberInfoAck.Member;
            var pos = FundsPosition;

            for (int i = 0; i < pos.Count; i++)
            {
                var position = pos[i];
                uint count = Sys_Family.Instance.familyData.GetPostNum((Sys_Family.FamilyData.EFamilyStatus)position);
                if(position == (uint)Sys_Family.FamilyData.EFamilyStatus.EBranchLeader)
                {
                    if (null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info)
                        count = (uint)Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.BranchMemberInfo.Count;
                    //count = Sys_Family.Instance.familyData.familyBuildInfo.buildBranchNum;
                }
                uint addCount = 0;
                for (int j = 0, membersCount = members.Count; j < membersCount; j++)
                {
                    var member = members[j];
                    if (position == member.Position % 10000)
                    {
                        UI_Family_FundsEntry myEntry = new UI_Family_FundsEntry();
                        myEntry.pos = position;
                        myEntry.memberInfo = member;
                        fundsEntrys.Add(myEntry);
                        addCount++;
                    }
                        
                }
                count = count - addCount;
                for (int k = 0; k < count; k++)
                {
                    UI_Family_FundsEntry myEntry = new UI_Family_FundsEntry();
                    myEntry.pos = position;
                    myEntry.memberInfo = null;
                    fundsEntrys.Add(myEntry);
                }
            }
        }

        private void RefreshView()
        {
            layout.SetInfinityGridCell(fundsEntrys.Count);
            layout.SetNextFundsData();
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_Family_FundsCeil entry = new UI_Family_FundsCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            cell.BindUserData(entry);
        }
        
        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_Family_FundsCeil entry = cell.mUserData as UI_Family_FundsCeil;
            if (index < 0 || index >= fundsEntrys.Count)
                return;
            entry.SetView(fundsEntrys[index]);
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Family_Funds);
        }
    }
}