using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Packet;
using UnityEngine.EventSystems;

namespace Logic
{
    public class UI_JSBattle_Rank_Layout
    {
        private Button closeBtn;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        public ScrollRect scrollRect;
        public void Init(Transform transform)
        {
            scrollRect = transform.Find("Animator/ScrollView_Rank").GetComponent<ScrollRect>();
            infinityGrid = transform.Find("Animator/ScrollView_Rank").GetComponent<InfinityGrid>();
            closeBtn = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
            //Lib.Core.EventTrigger.Get(scrollRect.gameObject).onDragEnd += listener.OnDragEnd;
            Lib.Core.EventTrigger.Get(scrollRect.gameObject).onDrag = listener.OnDrag;
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
            //void OnDragEnd(GameObject go);
            void OnDrag(GameObject go, Vector2 delta);

        }
    }

    public class UI_JSBattle_RankCeil
    {
        private Image rankImage;
        private Text rankText;
        private Text nameText;
        private Text careerText;
        private Text pointText;
        private Text familyNameText;
        private Image careerImage;
        public void Init(Transform transform)
        {
            rankImage = transform.Find("Rank/Image_Icon").GetComponent<Image>();
            rankText = transform.Find("Rank/Text_Rank").GetComponent<Text>();
            nameText = transform.Find("Text_Name").GetComponent<Text>();
            careerText = transform.Find("Text_Profession").GetComponent<Text>();
            careerImage = transform.Find("Text_Profession/Image").GetComponent<Image>();
            pointText = transform.Find("Text_Rate").GetComponent<Text>();
            familyNameText = transform.Find("Text_Family").GetComponent<Text>();
        }

        public void SetData(VictoryArenaRankUnit data, bool isMy = false)
        {
            if (isMy)
            {
                uint myRank = Sys_JSBattle.Instance.GetMyCurrentRank();
                bool isHasMedel = myRank < 4;
                rankImage.gameObject.SetActive(isHasMedel);
                rankText.gameObject.SetActive(!isHasMedel);
                if (isHasMedel)
                {
                    uint iconId = Sys_Rank.Instance.GetRankIcon((int)myRank);
                    ImageHelper.SetIcon(rankImage, iconId, true);
                }
                else
                {
                    TextHelper.SetText(rankText, myRank.ToString());
                }
                TextHelper.SetText(pointText, Sys_Attr.Instance.rolePower.ToString());
                TextHelper.SetText(nameText, Sys_Role.Instance.Role.Name.ToStringUtf8());
                CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career);
                if (null != cSVCareerData)
                {
                    TextHelper.SetText(careerText, cSVCareerData.name);

                    ImageHelper.SetIcon(careerImage, cSVCareerData.logo_icon);
                }
                else
                {
                    DebugUtil.LogError($"Not Find {data.Career} In CSVCareer");
                }
                familyNameText.text = Sys_Family.Instance.GetFamilyName();
            }
            else if (null != data)
            {
                bool isHasMedel = data.Rank < 4;
                rankImage.gameObject.SetActive(isHasMedel);
                rankText.gameObject.SetActive(!isHasMedel);
                if (isHasMedel)
                {
                    uint iconId = Sys_Rank.Instance.GetRankIcon((int)data.Rank);
                    ImageHelper.SetIcon(rankImage, iconId, true);
                }
                else
                {
                    TextHelper.SetText(rankText, data.Rank.ToString());
                }

                TextHelper.SetText(pointText, data.Score.ToString());
                TextHelper.SetText(nameText, data.Name.ToStringUtf8());
                CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(data.Career);
                if (null != cSVCareerData)
                {
                    TextHelper.SetText(careerText, cSVCareerData.name);

                    ImageHelper.SetIcon(careerImage, cSVCareerData.logo_icon);
                }
                else
                {
                    DebugUtil.LogError($"Not Find {data.Career} In CSVCareer");
                }

                if (string.IsNullOrEmpty(data.FamilyName.ToStringUtf8()))
                {
                    familyNameText.text = LanguageHelper.GetTextContent(540000028);
                }
                else
                {
                    familyNameText.text = data.FamilyName.ToStringUtf8();
                }
            }

        }
    }

    public class UI_JSBattle_Rank : UIBase, UI_JSBattle_Rank_Layout.IListener
    {
        private UI_JSBattle_Rank_Layout layout = new UI_JSBattle_Rank_Layout();
        private List<VictoryArenaRankUnit> victoryArenaRankUnits = new List<VictoryArenaRankUnit>();
        private UI_JSBattle_RankCeil myRank = new UI_JSBattle_RankCeil();
        private bool isQuerying = false;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            myRank.Init(transform.Find("Animator/MyRank/HaveInfo"));
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_JSBattle.Instance.eventEmitter.Handle(Sys_JSBattle.EEvents.GetRankListEnd, RefreshInfo, toRegister);
        }

        protected override void OnShow()
        {
            Sys_JSBattle.Instance.ClearVictoryArenaRankUnits();
            isQuerying = true;
            Sys_JSBattle.Instance.VictoryArenaRankListReq(0);
        }


        protected override void OnClose()
        {
            Sys_JSBattle.Instance.ClearVictoryArenaRankUnits();
        }

        private void RefreshInfo()
        {
            isQuerying = false;
            myRank.SetData(null, true);
            victoryArenaRankUnits = Sys_JSBattle.Instance.GetVictoryArenaRankUnits();
            layout.SetInfinityGridCell(victoryArenaRankUnits.Count);
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_JSBattle_RankCeil entry = new UI_JSBattle_RankCeil();
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
            if (index < 0 || index >= victoryArenaRankUnits.Count)
                return;
            UI_JSBattle_RankCeil entry = cell.mUserData as UI_JSBattle_RankCeil;

            entry.SetData(victoryArenaRankUnits[index]);
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_JSBattle_Rank);
        }


        /// <summary>
        /// 拖拽刷新 每个页码只请求一次
        /// </summary>
        /// <param name="go"></param>
        /// <param name="delta"> delta.y 手指向上拖是正的 向下拖是负</param>
        /*public void OnDrag(GameObject go, Vector2 delta)
        {
            
            if (isQuerying) return;

            int index = victoryArenaRankUnits.Count / Sys_JSBattle.Instance.OnePageDatasNum;//页数也是index
            var vertical = layout.scrollRect.verticalNormalizedPosition;//当前所在比例。越接近0越底下
            if (vertical > 1)
            {
                vertical = 0.99f;
            }
            else if (vertical < 0)
            {
                vertical = 0.01f;
            }
            
            int currentPage = (int)Math.Floor(index *  (1 - vertical)); //所在页数     
            
            var up = delta.y > 0;

            if (up)
            {
                if (currentPage >= Sys_JSBattle.Instance.MaxPage - 1)
                {
                    return;
                }
                if((vertical) <= 0.2 / index)
                {
                    Debug.Log("-----ppppppppppp-----------" + " !-------! " + currentPage.ToString() + " ______" + vertical.ToString());
                    isQuerying = true;
                    Sys_JSBattle.Instance.VictoryArenaRankListReq((uint)index);
                }
            }

        }*/

        int page = 0;//上次刷新的页数

        /// <summary>
        /// 滚动动态请求 -页码连接处可能出现相同数据-页码可重复请求
        /// </summary>
        /// <param name="go"></param>
        /// <param name="delta"></param>
        public void OnDrag(GameObject go, Vector2 delta)
        {
            if (isQuerying) return;

            int index = victoryArenaRankUnits.Count / Sys_JSBattle.Instance.OnePageDatasNum;//页数也是index
            var vertical = layout.scrollRect.verticalNormalizedPosition;//当前所在比例。越接近0越底下
            if (vertical > 1)
            {
                vertical = 0.99f;
            }
            else if (vertical < 0)
            {
                vertical = 0.01f;
            }

            int currentPage = (int)Math.Floor(index * (1 - vertical)); //所在页数     

            var up = delta.y > 0;

            if (up)
            {
                if (currentPage >= Sys_JSBattle.Instance.MaxPage - 1)
                {
                    return;
                }
                if (vertical <= 0.2 * (index - currentPage))
                {
                    if (page == currentPage + 1)
                    {
                        return;
                    }
                    isQuerying = true;
                    page = currentPage + 1;

                    Sys_JSBattle.Instance.VictoryArenaRankListReq((uint)page);
                }
            }
            else
            {
                if (currentPage <= 0)
                {
                    return;
                }

                if (vertical >= 0.2 * (index - currentPage))
                {
                    if (page == currentPage - 1)
                    {
                        return;
                    }
                    isQuerying = true;
                    page = currentPage - 1;
                    Sys_JSBattle.Instance.VictoryArenaRankListReq((uint)page);
                }
            }
        }

    }
}