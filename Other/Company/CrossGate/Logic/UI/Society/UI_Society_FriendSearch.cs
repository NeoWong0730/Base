using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Logic
{
    public class UI_Society_FriendSearch_Layout
    {
        public class RoleInfoItem
        {
            GameObject root;

            public Image roleIcon;
            public Image roleIconFrame;
            public Text roleName;
            public Text roleLv;
            public Text roleCareer;
            public Button addButton;

            Sys_Society.RoleInfo roleInfo;

            public void BindGameObject(GameObject go)
            {
                root = go;

                roleIcon = root.FindChildByName("Head").GetComponent<Image>();
                roleIconFrame = root.FindChildByName("Image_Before_Frame").GetComponent<Image>();
                roleName = root.FindChildByName("Text_Name").GetComponent<Text>();
                roleLv = root.FindChildByName("Text_Lv").GetComponent<Text>();
                roleCareer = root.FindChildByName("Text_Job").GetComponent<Text>();
                addButton = root.FindChildByName("Btn_01_Small").GetComponent<Button>();
                addButton.onClick.AddListener(OnClickAddButton);
            }

            public void UpdateItem(Sys_Society.RoleInfo _roleInfo)
            {
                roleInfo = _roleInfo;

                ImageHelper.SetIcon(roleIcon, Sys_Head.Instance.GetHeadImageId(roleInfo.heroID, roleInfo.iconId));
                ImageHelper.SetIcon(roleIconFrame, CSVHeadframe.Instance.GetConfData(roleInfo.iconFrameId).HeadframeIcon, true);
                TextHelper.SetText(roleName, roleInfo.roleName);
                //HZCTODO
                TextHelper.SetText(roleLv, LanguageHelper.GetTextContent(2029408, roleInfo.level.ToString()));
                TextHelper.SetText(roleCareer, CSVCareer.Instance.GetConfData(roleInfo.occ).name);
            }

            void OnClickAddButton()
            {
                ///是自己///
                if (roleInfo.roleID == Sys_Role.Instance.RoleId)
                {
                    //HZCTODO
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13036));
                    return;
                }

                ///已经是好友了///
                if (Sys_Society.Instance.socialFriendsInfo.friendsIdsDic.ContainsKey(roleInfo.roleID))
                {
                    //HZCTODO
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13035));
                    return;
                }

                ///在我的黑名单中///
                if (Sys_Society.Instance.socialBlacksInfo.blacksIdsDic.ContainsKey(roleInfo.roleID))
                {
                    //HZCTODO
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13034));
                    return;
                }

                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(2002093).words;
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_Society.Instance.ReqAddFriend(roleInfo.roleID);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }

        public GameObject root;

        public Button closeButton;

        public GameObject searchTableRoot;
        public CP_Toggle idSearchToggle;
        public CP_Toggle detailSearchToggle;
        public CP_Toggle relationSearchToggle;

        public GameObject idSearchRoot;
        public Button idSearchSubmitButton;
        public Button idSearchClearButton;
        public InputField idSearchInput;
        public InfinityGrid idSearchInfinityGrid;

        public GameObject detailSearchRoot;

        public GameObject detailPersonSetRoot;
        public Dropdown detailPersonSetAgeDropdown;
        public Dropdown detailPersonSetSexDropdown;
        public Dropdown detailPersonSetLocationDropdown;
        public GameObject detailPersonSetInterstingRoot;
        public CP_Toggle detailPersonSetIntersting_Film;
        public CP_Toggle detailPersonSetIntersting_Travel;
        public CP_Toggle detailPersonSetIntersting_Read;
        public CP_Toggle detailPersonSetIntersting_Comic;
        public CP_Toggle detailPersonSetIntersting_Friend;
        public CP_Toggle detailPersonSetIntersting_Sport;
        public CP_Toggle detailPersonSetIntersting_Music;
        public CP_Toggle detailPersonSetIntersting_Pet;
        public CP_Toggle detailPersonSetIntersting_Food;
        public List<CP_Toggle> detailPersonSetInterstingToggles = new List<CP_Toggle>();
        public Button detailPersonSetSubmitButton;

        public GameObject detailFriendSearchRoot;
        public Button detailFriendSearchSetInfoButton;
        public Dropdown detailFriendSearchAgeDropdown;
        public Dropdown detailFriendSearchSexDropdown;
        public Dropdown detailFriendSearchLocationDropdown;
        public GameObject detailFriendSearchInterstingRoot;
        public CP_Toggle detailFriendSearchIntersting_Film;
        public CP_Toggle detailFriendSearchIntersting_Travel;
        public CP_Toggle detailFriendSearchIntersting_Read;
        public CP_Toggle detailFriendSearchIntersting_Comic;
        public CP_Toggle detailFriendSearchIntersting_Friend;
        public CP_Toggle detailFriendSearchIntersting_Sport;
        public CP_Toggle detailFriendSearchIntersting_Music;
        public CP_Toggle detailFriendSearchIntersting_Pet;
        public CP_Toggle detailFriendSearchIntersting_Food;
        public Button detailFriendSearchAgeButton;
        public Button detailFriendSearchSexButton;
        public Button detailFriendSearchLocationButton;
        public Button detailFriendSearchInterstingButton;

        public GameObject detailResultRoot;
        public Text detailResultSearchText;
        public InfinityGrid detailSearchInfinityGrid;

        public GameObject relationSearchRoot;
        public Button relationSearchSubmitButton;
        public InfinityGrid relationSearchInfinityGrid;

        public void Init(GameObject obj)
        {
            root = obj;

            closeButton = root.FindChildByName("Btn_Close").GetComponent<Button>();
            searchTableRoot = root.FindChildByName("SearchTable");
            idSearchToggle = searchTableRoot.FindChildByName("Toggle0").GetComponent<CP_Toggle>();
            detailSearchToggle = searchTableRoot.FindChildByName("Toggle1").GetComponent<CP_Toggle>();
            relationSearchToggle = searchTableRoot.FindChildByName("Toggle2").GetComponent<CP_Toggle>();

            idSearchRoot = root.FindChildByName("IDSearch");
            idSearchSubmitButton = idSearchRoot.FindChildByName("Button_Sreach").GetComponent<Button>();
            idSearchClearButton = idSearchRoot.FindChildByName("Btn_Clear").GetComponent<Button>();
            idSearchInput = idSearchRoot.FindChildByName("InputField").GetComponent<InputField>();
            idSearchInfinityGrid = idSearchRoot.FindChildByName("Scroll View").GetComponent<InfinityGrid>();

            detailSearchRoot = root.FindChildByName("DetailSearch");

            detailPersonSetRoot = detailSearchRoot.FindChildByName("Personal_Inf");
            detailPersonSetAgeDropdown = detailPersonSetRoot.FindChildByName("AgeDropdown").GetComponent<Dropdown>();
            detailPersonSetSexDropdown = detailPersonSetRoot.FindChildByName("SexDropdown").GetComponent<Dropdown>();
            detailPersonSetLocationDropdown = detailPersonSetRoot.FindChildByName("LocationDropdown").GetComponent<Dropdown>();
            detailPersonSetInterstingRoot = detailPersonSetRoot.FindChildByName("Toggle");
            detailPersonSetIntersting_Film = detailPersonSetInterstingRoot.FindChildByName("tg0").GetComponent<CP_Toggle>();
            detailPersonSetIntersting_Travel = detailPersonSetInterstingRoot.FindChildByName("tg1").GetComponent<CP_Toggle>();
            detailPersonSetIntersting_Read = detailPersonSetInterstingRoot.FindChildByName("tg2").GetComponent<CP_Toggle>();
            detailPersonSetIntersting_Comic = detailPersonSetInterstingRoot.FindChildByName("tg3").GetComponent<CP_Toggle>();
            detailPersonSetIntersting_Friend = detailPersonSetInterstingRoot.FindChildByName("tg4").GetComponent<CP_Toggle>();
            detailPersonSetIntersting_Sport = detailPersonSetInterstingRoot.FindChildByName("tg5").GetComponent<CP_Toggle>();
            detailPersonSetIntersting_Music = detailPersonSetInterstingRoot.FindChildByName("tg6").GetComponent<CP_Toggle>();
            detailPersonSetIntersting_Pet = detailPersonSetInterstingRoot.FindChildByName("tg7").GetComponent<CP_Toggle>();
            detailPersonSetIntersting_Food = detailPersonSetInterstingRoot.FindChildByName("tg8").GetComponent<CP_Toggle>();
            detailPersonSetInterstingToggles.Clear();
            detailPersonSetInterstingToggles.Add(detailPersonSetIntersting_Film);
            detailPersonSetInterstingToggles.Add(detailPersonSetIntersting_Travel);
            detailPersonSetInterstingToggles.Add(detailPersonSetIntersting_Read);
            detailPersonSetInterstingToggles.Add(detailPersonSetIntersting_Comic);
            detailPersonSetInterstingToggles.Add(detailPersonSetIntersting_Friend);
            detailPersonSetInterstingToggles.Add(detailPersonSetIntersting_Sport);
            detailPersonSetInterstingToggles.Add(detailPersonSetIntersting_Music);
            detailPersonSetInterstingToggles.Add(detailPersonSetIntersting_Pet);
            detailPersonSetInterstingToggles.Add(detailPersonSetIntersting_Food);
            detailPersonSetSubmitButton = detailPersonSetRoot.FindChildByName("Btn_01").GetComponent<Button>();

            detailFriendSearchRoot = detailSearchRoot.FindChildByName("Search");
            detailFriendSearchSetInfoButton = detailFriendSearchRoot.FindChildByName("Btn_01").GetComponent<Button>();
            detailFriendSearchAgeDropdown = detailFriendSearchRoot.FindChildByName("AgeDropdown").GetComponent<Dropdown>();
            detailFriendSearchSexDropdown = detailFriendSearchRoot.FindChildByName("SexDropdown").GetComponent<Dropdown>();
            detailFriendSearchLocationDropdown = detailFriendSearchRoot.FindChildByName("LocationDropdown").GetComponent<Dropdown>();
            detailFriendSearchInterstingRoot = detailFriendSearchRoot.FindChildByName("Toggle");
            detailFriendSearchIntersting_Film = detailFriendSearchInterstingRoot.FindChildByName("tg0").GetComponent<CP_Toggle>();
            detailFriendSearchIntersting_Travel = detailFriendSearchInterstingRoot.FindChildByName("tg1").GetComponent<CP_Toggle>();
            detailFriendSearchIntersting_Read = detailFriendSearchInterstingRoot.FindChildByName("tg2").GetComponent<CP_Toggle>();
            detailFriendSearchIntersting_Comic = detailFriendSearchInterstingRoot.FindChildByName("tg3").GetComponent<CP_Toggle>();
            detailFriendSearchIntersting_Friend = detailFriendSearchInterstingRoot.FindChildByName("tg4").GetComponent<CP_Toggle>();
            detailFriendSearchIntersting_Sport = detailFriendSearchInterstingRoot.FindChildByName("tg5").GetComponent<CP_Toggle>();
            detailFriendSearchIntersting_Music = detailFriendSearchInterstingRoot.FindChildByName("tg6").GetComponent<CP_Toggle>();
            detailFriendSearchIntersting_Pet = detailFriendSearchInterstingRoot.FindChildByName("tg7").GetComponent<CP_Toggle>();
            detailFriendSearchIntersting_Food = detailFriendSearchInterstingRoot.FindChildByName("tg8").GetComponent<CP_Toggle>();           
            detailFriendSearchAgeButton = detailFriendSearchRoot.FindChildByName("AgeButton").GetComponent<Button>();
            detailFriendSearchSexButton = detailFriendSearchRoot.FindChildByName("SexButton").GetComponent<Button>();
            detailFriendSearchLocationButton = detailFriendSearchRoot.FindChildByName("LocationButton").GetComponent<Button>();
            detailFriendSearchInterstingButton = detailFriendSearchRoot.FindChildByName("InterstingButton").GetComponent<Button>();

            detailResultRoot = detailSearchRoot.FindChildByName("Search_Result");
            detailResultSearchText = detailResultRoot.FindChildByName("ResultText").GetComponent<Text>();
            detailSearchInfinityGrid = detailResultRoot.FindChildByName("Scroll View").GetComponent<InfinityGrid>();

            relationSearchRoot = root.FindChildByName("RelationSearch");
            relationSearchSubmitButton = relationSearchRoot.FindChildByName("Btn_01").GetComponent<Button>();
            relationSearchInfinityGrid = relationSearchRoot.FindChildByName("Scroll View").GetComponent<InfinityGrid>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            idSearchToggle.onValueChanged.AddListener(listener.OnClickCloseIdSearchToggle);
            detailSearchToggle.onValueChanged.AddListener(listener.OnClickDetailSearchToggle);
            relationSearchToggle.onValueChanged.AddListener(listener.OnClickRelationSearchToggle);

            idSearchSubmitButton.onClick.AddListener(listener.OnClickIdSearchSubmitButton);
            idSearchClearButton.onClick.AddListener(listener.OnClickIdSearchClearButton);
            idSearchInput.onValueChanged.AddListener(listener.OnIdSearchInputValueChanged);           

            detailPersonSetSubmitButton.onClick.AddListener(listener.OnClickDetailPersonSetSubmitButton);
            detailPersonSetAgeDropdown.onValueChanged.AddListener(listener.OnDetailPersonSetAgeDropdownValueChange);
            detailPersonSetSexDropdown.onValueChanged.AddListener(listener.OnDetailPersonSetSexDropdownValueChange);
            detailPersonSetLocationDropdown.onValueChanged.AddListener(listener.OnDetailPersonSetLocationDropdownValueChange);

            detailFriendSearchSetInfoButton.onClick.AddListener(listener.OnClickDetailFriendSearchSetInfoButton);
            detailFriendSearchAgeDropdown.onValueChanged.AddListener(listener.OnDetailFriendSearchAgeDropdownValueChange);
            detailFriendSearchSexDropdown.onValueChanged.AddListener(listener.OnDetailFriendSearchSexDropdownValueChange);
            detailFriendSearchLocationDropdown.onValueChanged.AddListener(listener.OnDetailFriendSearchLocationDropdownValueChange);
            detailFriendSearchAgeButton.onClick.AddListener(listener.OnClickDetailFriendSearchAgeButton);
            detailFriendSearchSexButton.onClick.AddListener(listener.OnClickDetailFriendSearchSexButton);
            detailFriendSearchLocationButton.onClick.AddListener(listener.OnClickDetailFriendSearchLocationButton);
            detailFriendSearchInterstingButton.onClick.AddListener(listener.OnClickDetailFriendSearchInterstingButton);
           
            relationSearchSubmitButton.onClick.AddListener(listener.OnClickRelationSearchSubmitButton);          
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnClickCloseIdSearchToggle(bool isOn);

            void OnClickDetailSearchToggle(bool isOn);

            void OnClickRelationSearchToggle(bool isOn);

            void OnClickIdSearchSubmitButton();

            void OnClickIdSearchClearButton();

            void OnIdSearchInputValueChanged(string str);

            void OnClickDetailPersonSetSubmitButton();

            void OnDetailPersonSetAgeDropdownValueChange(int index);

            void OnDetailPersonSetSexDropdownValueChange(int index);

            void OnDetailPersonSetLocationDropdownValueChange(int index);

            void OnClickDetailFriendSearchSetInfoButton();

            void OnDetailFriendSearchAgeDropdownValueChange(int index);

            void OnDetailFriendSearchSexDropdownValueChange(int index);

            void OnDetailFriendSearchLocationDropdownValueChange(int index);

            void OnClickDetailFriendSearchAgeButton();

            void OnClickDetailFriendSearchSexButton();

            void OnClickDetailFriendSearchLocationButton();

            void OnClickDetailFriendSearchInterstingButton();

            void OnClickRelationSearchSubmitButton();
        }
    }


    public class UI_Society_FriendSearch : UIBase, UI_Society_FriendSearch_Layout.IListener
    {
        private UI_Society_FriendSearch_Layout layout = new UI_Society_FriendSearch_Layout();

        List<Sys_Society.RoleInfo> idSearchRoleInfos = new List<Sys_Society.RoleInfo>();
        List<Sys_Society.RoleInfo> detailSearchRoleInfos = new List<Sys_Society.RoleInfo>();
        List<Sys_Society.RoleInfo> relationSearchRoleInfos = new List<Sys_Society.RoleInfo>();

        int curDetailPersonSetAgeIndex;
        int curDetailPersonSetSexIndex;
        int curDetailPersonSetLocationIndex;
        int curDetailFriendSearchAgeIndex;
        int curDetailFriendSearchSexIndex;
        int curDetailFriendSearchLocationIndex;

        protected override void OnLoaded()
        {
            layout.Init(gameObject);
            layout.RegisterEvents(this);

            layout.idSearchInfinityGrid.onCreateCell += IdSearchOnCreateCell;
            layout.idSearchInfinityGrid.onCellChange += IdSearchOnCellChange;

            layout.detailSearchInfinityGrid.onCreateCell += DetailSearchOnCreateCell;
            layout.detailSearchInfinityGrid.onCellChange += DetailSearchOnCellChange;

            layout.relationSearchInfinityGrid.onCreateCell += RelationSearchOnCreateCell;
            layout.relationSearchInfinityGrid.onCellChange += RelationSearchOnCellChange;
        }

        protected override void OnShow()
        {
            layout.idSearchToggle.SetSelected(true, false);
            layout.idSearchToggle.onValueChanged.Invoke(true);
            layout.idSearchInput.text = string.Empty;
            idSearchRoleInfos.Clear();
            detailSearchRoleInfos.Clear();
            relationSearchRoleInfos.Clear();

            LoadDetailPersonSetAgeDropdown();
            LoadDetailPersonSetSexDropdown();
            LoadDetailPersonSetLocationDropDown();
            InitDetailPersonInfos();

            LoadAndInitDetailFriendSearchAgeDropdown();
            LoadAndInitDetailFriendSearchSexDropdown();
            LoadAndInitDetailFriendSearchLocationDropdown();
            LoadAndInitDetailFriendSearchInterstings();

            curDetailPersonSetAgeIndex = 0;
            curDetailPersonSetSexIndex = 0;
            curDetailPersonSetLocationIndex = 0;
            curDetailFriendSearchAgeIndex = 0;
            curDetailFriendSearchSexIndex = 0;
            curDetailFriendSearchLocationIndex = 0;
        }

        void LoadDetailPersonSetAgeDropdown()
        {
            layout.detailPersonSetAgeDropdown.ClearOptions();
            List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
            for (int index = 0, len = CSVFriendSearch.Instance.AgeDatas.Count; index < len; index++)
            {
                optionDatas.Add(new Dropdown.OptionData()
                {
                    text = LanguageHelper.GetTextContent(CSVFriendSearch.Instance.AgeDatas[index].LanguageID),
                });
            }
            layout.detailPersonSetAgeDropdown.AddOptions(optionDatas);
        }

        void LoadDetailPersonSetSexDropdown()
        {
            layout.detailPersonSetSexDropdown.ClearOptions();
            List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
            for (int index = 0, len = CSVFriendSearch.Instance.SexDatas.Count; index < len; index++)
            {
                optionDatas.Add(new Dropdown.OptionData()
                {
                    text = LanguageHelper.GetTextContent(CSVFriendSearch.Instance.SexDatas[index].LanguageID),
                });
            }
            layout.detailPersonSetSexDropdown.AddOptions(optionDatas);
        }

        void LoadDetailPersonSetLocationDropDown()
        {
            layout.detailPersonSetLocationDropdown.ClearOptions();
            List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
            for (int index = 0, len = CSVFriendSearch.Instance.LocationDatas.Count; index < len; index++)
            {
                optionDatas.Add(new Dropdown.OptionData()
                {
                    text = LanguageHelper.GetTextContent(CSVFriendSearch.Instance.LocationDatas[index].LanguageID),
                });
            }
            layout.detailPersonSetLocationDropdown.AddOptions(optionDatas);
        }

        void InitDetailPersonInfos()
        {
            layout.detailPersonSetAgeDropdown.value = CSVFriendSearch.Instance.GetAgeDataIndex(Sys_Society.Instance.Age);
            layout.detailPersonSetAgeDropdown.onValueChanged.Invoke(layout.detailPersonSetAgeDropdown.value);

            layout.detailPersonSetSexDropdown.value = CSVFriendSearch.Instance.GetSexDataIndex(Sys_Society.Instance.Sex);
            layout.detailPersonSetSexDropdown.onValueChanged.Invoke(layout.detailPersonSetSexDropdown.value);

            layout.detailPersonSetLocationDropdown.value = CSVFriendSearch.Instance.GetLocationDataIndex(Sys_Society.Instance.Area);
            layout.detailPersonSetLocationDropdown.onValueChanged.Invoke(layout.detailPersonSetLocationDropdown.value);

            for (int index = 0, len = layout.detailPersonSetInterstingToggles.Count; index < len; index++)
            {
                if (Sys_Society.Instance.Hobbys.Contains(CSVFriendSearch.Instance.InterestDatas[index].id))
                {
                    layout.detailPersonSetInterstingToggles[index].SetSelected(true, false);
                }
                else
                {
                    layout.detailPersonSetInterstingToggles[index].SetSelected(false, false);
                }
            }

            if (Sys_Society.Instance.Hobbys.Count == 0)
            {
                layout.detailPersonSetInterstingToggles[0].SetSelected(true, false);
            }
        }

        void LoadAndInitDetailFriendSearchAgeDropdown()
        {
            layout.detailFriendSearchAgeDropdown.ClearOptions();
            List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
            for (int index = 0, len = CSVFriendSearch.Instance.AgeDatas.Count; index < len; index++)
            {
                optionDatas.Add(new Dropdown.OptionData()
                {
                    text = LanguageHelper.GetTextContent(CSVFriendSearch.Instance.AgeDatas[index].LanguageID),
                });
            }
            layout.detailFriendSearchAgeDropdown.AddOptions(optionDatas);
        }

        void LoadAndInitDetailFriendSearchSexDropdown()
        {
            layout.detailFriendSearchSexDropdown.ClearOptions();
            List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
            for (int index = 0, len = CSVFriendSearch.Instance.SexDatas.Count; index < len; index++)
            {
                optionDatas.Add(new Dropdown.OptionData()
                {
                    text = LanguageHelper.GetTextContent(CSVFriendSearch.Instance.SexDatas[index].LanguageID),
                });
            }
            layout.detailFriendSearchSexDropdown.AddOptions(optionDatas);
        }

        void LoadAndInitDetailFriendSearchLocationDropdown()
        {
            layout.detailFriendSearchLocationDropdown.ClearOptions();
            List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
            for (int index = 0, len = CSVFriendSearch.Instance.LocationDatas.Count; index < len; index++)
            {
                optionDatas.Add(new Dropdown.OptionData()
                {
                    text = LanguageHelper.GetTextContent(CSVFriendSearch.Instance.LocationDatas[index].LanguageID),
                });
            }
            layout.detailFriendSearchLocationDropdown.AddOptions(optionDatas);
        }

        void LoadAndInitDetailFriendSearchInterstings()
        {
            layout.detailFriendSearchIntersting_Film.SetSelected(false, false);
            layout.detailFriendSearchIntersting_Travel.SetSelected(false, false);
            layout.detailFriendSearchIntersting_Read.SetSelected(false, false);
            layout.detailFriendSearchIntersting_Comic.SetSelected(false, false);
            layout.detailFriendSearchIntersting_Friend.SetSelected(false, false);
            layout.detailFriendSearchIntersting_Sport.SetSelected(false, false);
            layout.detailFriendSearchIntersting_Music.SetSelected(false, false);
            layout.detailFriendSearchIntersting_Pet.SetSelected(false, false);
            layout.detailFriendSearchIntersting_Food.SetSelected(false, false);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.RoleInfo>(Sys_Society.EEvents.OnGetBriefInfoSuccess, OnGetBriefInfoSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<List<Sys_Society.RoleInfo>>(Sys_Society.EEvents.OnGetDetailSearchFriendSuccess, OnGetDetailSearchFriendSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<List<Sys_Society.RoleInfo>>(Sys_Society.EEvents.OnGetRelationSearchFriendSuccess, OnGetRelationSearchFriendSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle(Sys_Society.EEvents.OnSetRoleInfoSuccess, OnSetRoleInfoSuccess, toRegister);
        }

        void OnSetRoleInfoSuccess()
        {
            layout.detailSearchToggle.SetSelected(true, false);
            layout.detailSearchToggle.onValueChanged.Invoke(true);
        }

        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_Society_FriendSearch);
        }

        public void OnClickCloseIdSearchToggle(bool isOn)
        {
            if (isOn)
            {
                layout.idSearchRoot.SetActive(true);
                layout.detailSearchRoot.SetActive(false);
                layout.relationSearchRoot.SetActive(false);
            }
        }

        public void OnClickDetailSearchToggle(bool isOn)
        {
            if (isOn)
            {
                layout.idSearchRoot.SetActive(false);
                layout.detailSearchRoot.SetActive(true);

                if (Sys_Society.Instance.Sex == 0)
                {
                    layout.detailFriendSearchRoot.SetActive(false);
                    layout.detailPersonSetRoot.SetActive(true);
                    layout.detailResultRoot.SetActive(false);
                }
                else
                {
                    layout.detailFriendSearchRoot.SetActive(true);
                    layout.detailPersonSetRoot.SetActive(false);
                    layout.detailResultRoot.SetActive(false);
                }

                layout.relationSearchRoot.SetActive(false);
            }
        }
        public void OnClickRelationSearchToggle(bool isOn)
        {
            if (isOn)
            {
                layout.idSearchRoot.SetActive(false);
                layout.detailSearchRoot.SetActive(false);
                layout.relationSearchRoot.SetActive(true);
            }
        }

        #region IDSearch

        public void OnClickIdSearchSubmitButton()
        {
            if (string.IsNullOrWhiteSpace(layout.idSearchInput.text))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13033));
                return;
            }
            Sys_Society.Instance.ReqGetBriefInfo(layout.idSearchInput.text);
        }

        public void OnClickIdSearchClearButton()
        {
            layout.idSearchInput.text = string.Empty;
        }

        public void OnIdSearchInputValueChanged(string str)
        {
            layout.idSearchInput.text = Sys_WordInput.Instance.LimitLengthAndFilter(str);
        }

        void OnGetBriefInfoSuccess(Sys_Society.RoleInfo roleInfo)
        {
            idSearchRoleInfos.Clear();
            idSearchRoleInfos.Add(roleInfo);
            layout.idSearchInfinityGrid.CellCount = idSearchRoleInfos.Count;
            layout.idSearchInfinityGrid.ForceRefreshActiveCell();
        }

        public void IdSearchOnCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_Society_FriendSearch_Layout.RoleInfoItem itemCell = new UI_Society_FriendSearch_Layout.RoleInfoItem();
            itemCell.BindGameObject(go);
            cell.BindUserData(itemCell);
        }

        public void IdSearchOnCellChange(InfinityGridCell cell, int index)
        {
            UI_Society_FriendSearch_Layout.RoleInfoItem mCell = cell.mUserData as UI_Society_FriendSearch_Layout.RoleInfoItem;
            if (index < idSearchRoleInfos.Count)
            {
                var item = idSearchRoleInfos[index];
                mCell.UpdateItem(item);
            }
        }

        #endregion

        #region DetailSearch

        public void OnDetailPersonSetAgeDropdownValueChange(int index)
        {
            curDetailPersonSetAgeIndex = index;
        }

        public void OnDetailPersonSetSexDropdownValueChange(int index)
        {
            curDetailPersonSetSexIndex = index;
        }

        public void OnDetailPersonSetLocationDropdownValueChange(int index)
        {
            curDetailPersonSetLocationIndex = index;
        }

        public void OnClickDetailFriendSearchSetInfoButton()
        {
            layout.detailFriendSearchRoot.SetActive(false);
            layout.detailPersonSetRoot.SetActive(true);
            layout.detailResultRoot.SetActive(false);
        }

        public void OnDetailFriendSearchAgeDropdownValueChange(int index)
        {
            curDetailFriendSearchAgeIndex = index;
        }

        public void OnDetailFriendSearchSexDropdownValueChange(int index)
        {
            curDetailFriendSearchSexIndex = index;
        }

        public void OnDetailFriendSearchLocationDropdownValueChange(int index)
        {
            curDetailFriendSearchLocationIndex = index;
        }

        public void OnClickDetailPersonSetSubmitButton()
        {
            List<uint> interstings = new List<uint>();

            for (int index = 0, len = layout.detailPersonSetInterstingToggles.Count; index < len; index++)
            {
                if (layout.detailPersonSetInterstingToggles[index].IsOn)
                {
                    interstings.Add(CSVFriendSearch.Instance.InterestDatas[index].id);
                }
            }          

            Sys_Society.Instance.SetSocialInfoReq(CSVFriendSearch.Instance.AgeDatas[curDetailPersonSetAgeIndex].id,
                CSVFriendSearch.Instance.SexDatas[curDetailPersonSetSexIndex].id,
                CSVFriendSearch.Instance.LocationDatas[curDetailPersonSetLocationIndex].id,
                interstings);           
        }

        public void OnClickDetailFriendSearchAgeButton()
        {
            Sys_Society.Instance.ReqSearchFriendByType(CSVFriendSearch.Instance.AgeDatas[curDetailFriendSearchAgeIndex].id);
            layout.detailFriendSearchRoot.SetActive(false);
            layout.detailResultRoot.SetActive(true);
            TextHelper.SetText(layout.detailResultSearchText, CSVFriendSearch.Instance.AgeDatas[curDetailFriendSearchAgeIndex].LanguageID);
        }

        public void OnClickDetailFriendSearchSexButton()
        {
            Sys_Society.Instance.ReqSearchFriendByType(CSVFriendSearch.Instance.SexDatas[curDetailFriendSearchSexIndex].id);
            layout.detailFriendSearchRoot.SetActive(false);
            layout.detailResultRoot.SetActive(true);
            TextHelper.SetText(layout.detailResultSearchText, CSVFriendSearch.Instance.SexDatas[curDetailFriendSearchSexIndex].LanguageID);
        }

        public void OnClickDetailFriendSearchLocationButton()
        {
            Sys_Society.Instance.ReqSearchFriendByType(CSVFriendSearch.Instance.LocationDatas[curDetailFriendSearchLocationIndex].id);
            layout.detailFriendSearchRoot.SetActive(false);
            layout.detailResultRoot.SetActive(true);
            TextHelper.SetText(layout.detailResultSearchText, CSVFriendSearch.Instance.LocationDatas[curDetailFriendSearchLocationIndex].LanguageID);
        }

        public void OnClickDetailFriendSearchInterstingButton()
        {
            if (layout.detailFriendSearchIntersting_Film.IsOn)
            {
                Sys_Society.Instance.ReqSearchFriendByType(CSVFriendSearch.Instance.InterestDatas[0].id);
                TextHelper.SetText(layout.detailResultSearchText, CSVFriendSearch.Instance.InterestDatas[0].LanguageID);
            }
            else if (layout.detailFriendSearchIntersting_Travel.IsOn)
            {
                Sys_Society.Instance.ReqSearchFriendByType(CSVFriendSearch.Instance.InterestDatas[1].id);
                TextHelper.SetText(layout.detailResultSearchText, CSVFriendSearch.Instance.InterestDatas[1].LanguageID);
            }
            else if (layout.detailFriendSearchIntersting_Read.IsOn)
            {
                Sys_Society.Instance.ReqSearchFriendByType(CSVFriendSearch.Instance.InterestDatas[2].id);
                TextHelper.SetText(layout.detailResultSearchText, CSVFriendSearch.Instance.InterestDatas[2].LanguageID);
            }
            else if (layout.detailFriendSearchIntersting_Comic.IsOn)
            {
                Sys_Society.Instance.ReqSearchFriendByType(CSVFriendSearch.Instance.InterestDatas[3].id);
                TextHelper.SetText(layout.detailResultSearchText, CSVFriendSearch.Instance.InterestDatas[3].LanguageID);
            }
            else if (layout.detailFriendSearchIntersting_Friend.IsOn)
            {
                Sys_Society.Instance.ReqSearchFriendByType(CSVFriendSearch.Instance.InterestDatas[4].id);
                TextHelper.SetText(layout.detailResultSearchText, CSVFriendSearch.Instance.InterestDatas[4].LanguageID);
            }
            else if (layout.detailFriendSearchIntersting_Sport.IsOn)
            {
                Sys_Society.Instance.ReqSearchFriendByType(CSVFriendSearch.Instance.InterestDatas[5].id);
                TextHelper.SetText(layout.detailResultSearchText, CSVFriendSearch.Instance.InterestDatas[5].LanguageID);
            }
            else if (layout.detailFriendSearchIntersting_Music.IsOn)
            {
                Sys_Society.Instance.ReqSearchFriendByType(CSVFriendSearch.Instance.InterestDatas[6].id);
                TextHelper.SetText(layout.detailResultSearchText, CSVFriendSearch.Instance.InterestDatas[6].LanguageID);
            }
            else if (layout.detailFriendSearchIntersting_Pet.IsOn)
            {
                Sys_Society.Instance.ReqSearchFriendByType(CSVFriendSearch.Instance.InterestDatas[7].id);
                TextHelper.SetText(layout.detailResultSearchText, CSVFriendSearch.Instance.InterestDatas[7].LanguageID);
            }
            else if (layout.detailFriendSearchIntersting_Food.IsOn)
            {
                Sys_Society.Instance.ReqSearchFriendByType(CSVFriendSearch.Instance.InterestDatas[8].id);
                TextHelper.SetText(layout.detailResultSearchText, CSVFriendSearch.Instance.InterestDatas[8].LanguageID);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13007));
                return;
            }
            layout.detailFriendSearchRoot.SetActive(false);
            layout.detailResultRoot.SetActive(true);
        }

        void OnGetDetailSearchFriendSuccess(List<Sys_Society.RoleInfo> roleInfos)
        {
            detailSearchRoleInfos.Clear();
            for (int index = 0, len = roleInfos.Count; index < len; index++)
            {
                detailSearchRoleInfos.Add(roleInfos[index]);
            }
            layout.detailSearchInfinityGrid.CellCount = detailSearchRoleInfos.Count;
            layout.detailSearchInfinityGrid.ForceRefreshActiveCell();
        }

        public void DetailSearchOnCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_Society_FriendSearch_Layout.RoleInfoItem itemCell = new UI_Society_FriendSearch_Layout.RoleInfoItem();
            itemCell.BindGameObject(go);
            cell.BindUserData(itemCell);
        }

        public void DetailSearchOnCellChange(InfinityGridCell cell, int index)
        {
            UI_Society_FriendSearch_Layout.RoleInfoItem mCell = cell.mUserData as UI_Society_FriendSearch_Layout.RoleInfoItem;
            if (index < detailSearchRoleInfos.Count)
            {
                var item = detailSearchRoleInfos[index];
                mCell.UpdateItem(item);
            }
        }

        #endregion

        #region RelationSearch

        public void OnClickRelationSearchSubmitButton()
        {
            Sys_Society.Instance.ReqSearchFriendByRelation();
        }

        void OnGetRelationSearchFriendSuccess(List<Sys_Society.RoleInfo> roleInfos)
        {
            relationSearchRoleInfos.Clear();
            for (int index = 0, len = roleInfos.Count; index < len; index++)
            {
                relationSearchRoleInfos.Add(roleInfos[index]);
            }
            layout.relationSearchInfinityGrid.CellCount = relationSearchRoleInfos.Count;
            layout.relationSearchInfinityGrid.ForceRefreshActiveCell();
        }

        public void RelationSearchOnCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_Society_FriendSearch_Layout.RoleInfoItem itemCell = new UI_Society_FriendSearch_Layout.RoleInfoItem();
            itemCell.BindGameObject(go);
            cell.BindUserData(itemCell);
        }

        public void RelationSearchOnCellChange(InfinityGridCell cell, int index)
        {
            UI_Society_FriendSearch_Layout.RoleInfoItem mCell = cell.mUserData as UI_Society_FriendSearch_Layout.RoleInfoItem;
            if (index < relationSearchRoleInfos.Count)
            {
                var item = relationSearchRoleInfos[index];
                mCell.UpdateItem(item);
            }
        }

        #endregion
    }
}