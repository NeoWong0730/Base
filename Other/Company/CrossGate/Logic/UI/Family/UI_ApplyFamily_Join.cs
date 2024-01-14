using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using System.Text;

namespace Logic
{
    public class FamilyLiveValue
    {
        public static string GetString(ulong livevalue)
        {
            string livestring;
            if (livevalue >= 100000000ul)
            {
                livestring = LanguageHelper.GetTextContent(15228, ((livevalue / 1000000) / 100f).ToString());
            }
            else if (livevalue >= 10000ul)
            {
                livestring = LanguageHelper.GetTextContent(15227, ((livevalue / 100) / 100f).ToString());
            }
            else
            {
                livestring = livevalue.ToString();
            }

            return livestring;
        }
    }
    /// <summary> 加入家族子界面 </summary>
    public class UI_ApplyFamily_Join : UIComponent
    {
        #region 界面组件
        /// <summary> 滚动区域 </summary>
        private ScrollRect scrollRect_Family;
        /// <summary> 输入查询家族 </summary>
        private InputField inputField_Query;
        /// <summary> 清理查询内容 </summary>
        private Button button_ClearQuery;
        /// <summary> 家族名称 </summary>
        private Text text_Name;
        /// <summary> 家族宣言 </summary>
        private Text text_Declaration;
        /// <summary> 家族族长 </summary>
        private Text text_Patriarch;
        /// <summary> 一键加入 </summary>
        private Button button_OneKey;
        /// <summary> 申请加入 </summary>
        private Button button_Join;
        /// <summary> 取消加入 </summary>
        private Button button_Cannel;
        /// <summary> 无限滚动 </summary>
        private ScrollGridVertical scrollGridVertical;
        /// <summary> 家族模版列表 </summary>
        private List<Toggle> list_FamilyItem = new List<Toggle>();

        /// <summary> 排序选项 </summary>
        private Toggle toggle_Sort;
        /// <summary> 菜单选项列表 </summary>
        private List<Toggle> list_Menu = new List<Toggle>();

        #endregion
        #region 数据定义
        /// <summary> 筛选类型 </summary>
        public enum EFamilySortType
        {
            Id = 0,                 //编号
            Level = 1,              //等级
            MenberCount = 2,             //成员数量
            Live = 3,
        }

        /// <summary> 功能类型 </summary>
        public enum FunctionType
        {
            CheckAndApply,//查找以及申请
            Check,        //查找
        }
        /// <summary> 查询列表 </summary>
        private List<BriefInfo> list_Query = new List<BriefInfo>();
        /// <summary> 是否查询中 </summary>
        private bool isQuerying = false;
        /// <summary> 选中数据 </summary>
        public BriefInfo curBriefInfo
        {
            get
            {
                if (selectIndex < 0 || selectIndex >= list_Query.Count) return null;

                return list_Query[selectIndex];
            }
        }
        /// <summary> 选中下标 </summary>
        private int selectIndex = 0;
        /// <summary> 功能类型 </summary>
        private FunctionType functionType = FunctionType.CheckAndApply;
        /// <summary> 名称最大字数 </summary>
        private const int maxLimit_Name = 12;
        /// <summary> 是否第一次打开界面 </summary>
        private bool isFirstOpen = false;

        private ulong selectID = 0;
        #endregion
        #region 系统函数        
        protected override void Loaded()
        {
            OnParseComponent();
        }
        public override void SetData(params object[] arg)
        {
            functionType = FunctionType.Check;
            if (arg.Length > 0)
            {
                inputField_Query.text = System.Convert.ToUInt64(arg[0]).ToString();
                OnClick_QueryFamily();
            }
        }
        public override void Show()
        {
            base.Show();
            if (isFirstOpen)
            {
                if (list_Menu.Count > 3 && list_Menu[3].isOn == false)
                    list_Menu[3].isOn = true;

                if (list_Menu.Count > 3 && list_Menu[3].isOn)
                    toggle_Sort.transform.position = list_Menu[3].transform.position;

                RefreshView();
                ApplyFamilyList();
                isFirstOpen = false;
            }
        }

        public void SetOpenParam(uint id)
        {
            inputField_Query.text = id.ToString();
        }
        public override void Hide()
        {
            base.Hide();
        }
        protected override void Update()
        {
        }
        protected override void Refresh()
        {
        }
        public override void Reset()
        {
            Sys_Family.Instance.familyData.queryFamilyInfo.Clear();
            isQuerying = false;
            selectIndex = -1;
            scrollGridVertical.FixedPosition(0F);
            isFirstOpen = true;
            selectID = 0;
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件
        /// </summary>
        private void OnParseComponent()
        {
            scrollRect_Family = transform.Find("View_Left/ScrollView_Rank").GetComponent<ScrollRect>();
            inputField_Query = transform.Find("View_Left/InputField_Describe").GetComponent<InputField>();
            button_ClearQuery = transform.Find("View_Left/InputField_Describe/Button_Delete").GetComponent<Button>();
            text_Name = transform.Find("View_Right/Text_Name/Text").GetComponent<Text>();
            text_Declaration = transform.Find("View_Right/Text_Talk/Scroll_View01/Viewport/Text").GetComponent<Text>();
            text_Patriarch = transform.Find("View_Right/Text_Head/Text").GetComponent<Text>();
            button_OneKey = transform.Find("View_Right/Button_Quick_Apply").GetComponent<Button>();
            button_Join = transform.Find("View_Right/Button_Apply").GetComponent<Button>();
            button_Cannel = transform.Find("View_Right/Button_Cancel").GetComponent<Button>();
            scrollGridVertical = transform.Find("View_Left/ScrollView_Rank").GetComponent<ScrollGridVertical>();
            scrollGridVertical.AddCellListener(OnCellUpdateCallback);
            scrollGridVertical.AddCreateCellListener(OnCreateCellCallback);
            Lib.Core.EventTrigger.Get(scrollRect_Family.gameObject).onDragEnd += OnDragEnd;
            button_OneKey.onClick.AddListener(OnClick_OneKeyJoinFamily);
            button_Join.onClick.AddListener(OnClick_JoinFamily);
            button_Cannel.onClick.AddListener(OnClick_CannelFamily);
            button_ClearQuery.onClick.AddListener(OnClick_ClearQuery);
            transform.Find("View_Left/Button_Seek").GetComponent<Button>().onClick.AddListener(OnClick_QueryFamily);
            transform.Find("View_Right/Text_Head/Button").GetComponent<Button>().onClick.AddListener(OnClick_PrivateChat);

            inputField_Query.characterLimit = 0;
            inputField_Query.onValidateInput = OnValidateInput_Query;
            button_ClearQuery.gameObject.SetActive(false);

            toggle_Sort = transform.Find("View_Left/ArrowList/Toggle").GetComponent<Toggle>();
            toggle_Sort.onValueChanged.AddListener((bool value) => { OnClick_Sort(value); });
            var values = System.Enum.GetValues(typeof(EFamilySortType));
            for (int i = 0, count = values.Length; i < count; i++)
            {
                Toggle toggle = transform.Find(string.Format("View_Left/ArrowList/Menu/Arrow ({0})", i + 1)).GetComponent<Toggle>();
                toggle.onValueChanged.AddListener((bool value) => { OnClick_Menu(toggle, value); });
                list_Menu.Add(toggle);
            }
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateFamilyList, OnUpdateFamilyList, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.GetQueryFamilyListRes, OnGetQueryFamilyListRes, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateApplyFamilyList, OnUpdateApplyFamilyList, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.RoleInfo>(Sys_Society.EEvents.OnGetBriefInfoSuccess, OnGetBriefInfoSuccess, toRegister);
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            Sort();
            scrollGridVertical.SetCellCount(list_Query.Count);
            button_ClearQuery.gameObject.SetActive(isQuerying);
            SetDefaultToggle();
            if (functionType == FunctionType.Check)
            {
                button_OneKey.gameObject.SetActive(false);
                button_Join.gameObject.SetActive(false);
                button_Cannel.gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 设置默认
        /// </summary>
        private void SetDefaultToggle()
        {
            scrollGridVertical.RefreshAllCells();
            //SetSelectToggle(selectIndex);
        }
        /// <summary>
        /// 设置选中家族
        /// </summary>
        /// <param name="tr"></param>
        private void SetSelectFamily(BriefInfo briefInfo)
        {
            bool isShow = false;
            if (null == briefInfo)
            {
                text_Name.text = string.Empty;
                text_Declaration.text = string.Empty;
                text_Patriarch.text = string.Empty;
            }
            else
            {
                text_Name.text = briefInfo.GuildName.ToStringUtf8();
                text_Declaration.text = briefInfo.Notice.ToStringUtf8();
                text_Patriarch.text = briefInfo.LeaderName.ToStringUtf8();
                isShow = Sys_Family.Instance.familyData.familyPlayerInfo.applyList.Contains(briefInfo.GuildId);
            }

            if (functionType == FunctionType.CheckAndApply)
            {
                button_Join.gameObject.SetActive(!isShow);
                button_Cannel.gameObject.SetActive(isShow);
            }
        }
        /// <summary>
        /// 设置家族模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="briefInfo"></param>
        /// <param name="isSelect"></param>
        private void SetFamilyItem(Transform tr, BriefInfo briefInfo, bool isSelect)
        {
            tr.name = briefInfo.GuildId.ToString();
            /// <summary> 选项脚本 </summary>
            Toggle toggle = tr.GetComponent<Toggle>();
            toggle.SetIsOnWithoutNotify(isSelect);
            /// <summary> 家族编号 </summary>
            Text text_ID = tr.Find("Text_Number").GetComponent<Text>();
            text_ID.text = (briefInfo.GuildId % 100000000UL).ToString();
            /// <summary> 家族名称 </summary>
            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            text_Name.text = briefInfo.GuildName.ToStringUtf8();
            /// <summary> 家族等级 </summary>
            Text text_Lv = tr.Find("Text_Level").GetComponent<Text>();
            text_Lv.text = briefInfo.GuildLvl.ToString();
            /// <summary> 家族人数 </summary>
            Text text_Member = tr.Find("Text_Amount").GetComponent<Text>();
            text_Member.text = string.Format("{0}/{1}", briefInfo.MemberCount.ToString(), briefInfo.MemberMax.ToString());
            /// <summary> 家族族长 </summary>
            Text text_Patriarch = tr.Find("Text_Head").GetComponent<Text>();
            text_Patriarch.text = briefInfo.LeaderName.ToStringUtf8();
            /// <summary> 申请标记 </summary>
            GameObject go_ApplyMark = tr.Find("Image_Apply").gameObject;
            bool isShow = !Sys_Family.Instance.familyData.isInFamily && Sys_Family.Instance.familyData.familyPlayerInfo.applyList.Contains(briefInfo.GuildId);
            go_ApplyMark.SetActive(isShow);

            Text text_Live = tr.Find("Text_Live").GetComponent<Text>();

            text_Live.text = FamilyLiveValue.GetString(briefInfo.HistoryContribution);
        }
        #endregion
        #region 响应事件

        /// <summary>
        /// 点击菜单
        /// </summary>
        /// <param name="toggle"></param>
        private void OnClick_Menu(Toggle toggle, bool value)
        {
            if (value)
            {
                toggle_Sort.transform.position = toggle.transform.position;
                OnClick_Sort(true);
            }
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="value"></param>
        private void OnClick_Sort(bool value)
        {
            RefreshView();
        }


        /// <summary>
        /// 输入字数
        /// </summary>
        /// <param name="text"></param>
        /// <param name="charIndex"></param>
        /// <param name="addedChar"></param>
        /// <returns></returns>
        private char OnValidateInput_Query(string text, int charIndex, char addedChar)
        {
            if (TextHelper.GetCharNum(text) + TextHelper.GetCharNum(addedChar.ToString()) > maxLimit_Name)
            {
                return '\0';
            }
            return addedChar;
        }
        /// <summary>
        /// 滚动拖拽结束事件
        /// </summary>
        /// <param name="vector2"></param>
        private void OnDragEnd(GameObject go)
        {
            if (isQuerying) return;

            if (scrollRect_Family.velocity.y > 1f)
            {
                ApplyFamilyList();
            }
        }
        /// <summary>
        /// 查询家族
        /// </summary>
        private void OnClick_QueryFamily()
        {
            UIManager.HitButton(EUIID.UI_ApplyFamily, "OnClick_QueryFamily");
            Sys_Family.Instance.SendGuildFindGuildReq(inputField_Query.text);
        }
        /// <summary>
        /// 清除查询结果
        /// </summary>
        private void OnClick_ClearQuery()
        {
            UIManager.HitButton(EUIID.UI_ApplyFamily, "OnClick_ClearQuery");
            isQuerying = false;
            OnUpdateFamilyList();
        }
        /// <summary>
        /// 一键加入
        /// </summary>
        private void OnClick_OneKeyJoinFamily()
        {
            UIManager.HitButton(EUIID.UI_ApplyFamily, "OnClick_OneKeyJoinFamily");
            if (!Sys_Family.Instance.CanJoinFamily(true))
            {
                return;
            }
            else if (list_Query.Count == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10661));
                return;
            }
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(10029);
            PromptBoxParameter.Instance.SetConfirm(true, () => { Sys_Family.Instance.SendGuildOneKeyApplyReq(); });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
        }
        /// <summary>
        /// 申请加入
        /// </summary>
        private void OnClick_JoinFamily()
        {
            UIManager.HitButton(EUIID.UI_ApplyFamily, "OnClick_JoinFamily");
            if (!Sys_Family.Instance.CanJoinFamily(true))
            {
                return;
            }
            else if (list_Query.Count == 0 || null == curBriefInfo)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10661));
                return;
            }
            Sys_Family.Instance.SendGuildApplyReq(curBriefInfo.GuildId, true);
        }
        /// <summary>
        /// 取消加入
        /// </summary>
        private void OnClick_CannelFamily()
        {
            UIManager.HitButton(EUIID.UI_ApplyFamily, "OnClick_CannelFamily");
            if (null == curBriefInfo) return;

            Sys_Family.Instance.SendGuildApplyReq(curBriefInfo.GuildId, false);
        }
        /// <summary>
        /// 私聊
        /// </summary>
        private void OnClick_PrivateChat()
        {
            UIManager.HitButton(EUIID.UI_ApplyFamily, "OnClick_PrivateChat");
            if (null == curBriefInfo) return;
            if (curBriefInfo.LeaderId == Sys_Role.Instance.RoleId)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10676));
                return;
            }
            Sys_Society.Instance.ReqGetBriefInfo(curBriefInfo.LeaderId.ToString());
        }
        /// <summary>
        /// 点击家族模版
        /// </summary>
        /// <param name="toggle"></param>
        private void OnClick_FamilyItem(ScrollGridCell cell, bool value)
        {
            UIManager.HitButton(EUIID.UI_ApplyFamily, "OnClick_FamilyItem");

            var curinfo = curBriefInfo;

            if (value)
            {
                selectIndex = cell.index;

                selectID = curBriefInfo.GuildId;
            }
            else if (curinfo != null && selectID == curinfo.GuildId)
            {
                selectIndex = -1;
                selectID = 0;
            }

         
            SetSelectFamily(curBriefInfo);
        }
        /// <summary>
        /// 更新滚动子节点
        /// </summary>
        /// <param name="cell"></param>
        private void OnCellUpdateCallback(ScrollGridCell cell)
        {
            BriefInfo briefInfo = list_Query[cell.index];
            SetFamilyItem(cell.gameObject.transform, briefInfo, briefInfo.GuildId == selectID);
        }
        /// <summary>
        /// 创建模版回调
        /// </summary>
        /// <param name="cell"></param>
        private void OnCreateCellCallback(ScrollGridCell cell)
        {
            GameObject go = cell.gameObject;
            Toggle toggle = go.GetComponent<Toggle>();
            toggle.isOn = false;
            toggle.onValueChanged.AddListener((bool value) => { OnClick_FamilyItem(cell, value); });
            list_FamilyItem.Add(toggle);
        }
        /// <summary>
        /// 更新家族列表
        /// </summary>
        private void OnUpdateFamilyList()
        {
            var list = Sys_Family.Instance.familyData.queryFamilyInfo.familyList;
            inputField_Query.text = "";
            list_Query.Clear();
            list_Query.AddRange(list);

            selectIndex = list_Query.FindIndex(o => o.GuildId == selectID);
            RefreshView();
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        private void OnGetQueryFamilyListRes()
        {
            isQuerying = true;
            var list = Sys_Family.Instance.familyData.queryFamilyInfo.queryList;
            list_Query.Clear();
            list_Query.AddRange(list);
            selectIndex = list_Query.FindIndex(o => o.GuildId == selectID);
            RefreshView();
        }
        /// <summary>
        /// 更新申请列表
        /// </summary>
        private void OnUpdateApplyFamilyList()
        {
            selectIndex = list_Query.FindIndex(o => o.GuildId == selectID);
            RefreshView();
        }
        /// <summary>
        /// 获取玩家详细信息回调
        /// </summary>
        /// <param name="roleInfo"></param>
        private void OnGetBriefInfoSuccess(Sys_Society.RoleInfo roleInfo)
        {
            Sys_Society.Instance.OpenPrivateChat(roleInfo);
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 设置单选模版
        /// </summary>
        /// <param name="index">控件下标</param>
        private void SetSelectToggle(int index)
        {
            if (index < 0 || index >= list_FamilyItem.Count)
            {
                SetSelectFamily(null);
                return;
            }

            Toggle toggle = list_FamilyItem[index];
            if (!toggle.isOn)
            {
                toggle.isOn = true;
            }
            else
            {
                toggle.onValueChanged.Invoke(true);
            }
        }
        /// <summary>
        /// 申请家族列表
        /// </summary>
        private void ApplyFamilyList()
        {
            ulong targetId = Sys_Family.Instance.familyData.queryFamilyInfo.GetFamilyListLastId() + 1;
            Sys_Family.Instance.SendGuildGetGuildListReq(targetId);
        }

        /// <summary>
        /// 排序
        /// </summary>
        private void Sort()
        {
            int index = list_Menu.FindIndex(x => x.isOn == true);
            if (index < 0) return;
            bool isDescendingOrder = toggle_Sort.isOn;
            switch ((EFamilySortType)index)
            {
                case EFamilySortType.Level:
                    {
                        list_Query.Sort((x1, x2) =>
                        {
                            if (isDescendingOrder)
                                return x2.GuildLvl.CompareTo(x1.GuildLvl);
                            else
                                return x1.GuildLvl.CompareTo(x2.GuildLvl);
                        });
                    }
                    break;
                case EFamilySortType.Id:
                    {
                        list_Query.Sort((x1, x2) =>
                        {
                            if (isDescendingOrder)
                                return x1.GuildId.CompareTo(x2.GuildId);
                            else
                                return x2.GuildId.CompareTo(x1.GuildId);
                        });
                    }
                    break;
                case EFamilySortType.MenberCount:
                    {
                        list_Query.Sort((x1, x2) =>
                        {
                            if (isDescendingOrder)
                                return x2.MemberCount.CompareTo(x1.MemberCount);
                            else
                                return x1.MemberCount.CompareTo(x2.MemberCount);
                        });
                    }
                    break;
                case EFamilySortType.Live:
                    {
                        list_Query.Sort((x1, x2) =>
                        {
                            if (isDescendingOrder)
                                return x2.HistoryContribution.CompareTo(x1.HistoryContribution);
                            else
                                return x1.HistoryContribution.CompareTo(x2.HistoryContribution);
                        });
                    }
                    break;
            }
        }
        #endregion
    }
}