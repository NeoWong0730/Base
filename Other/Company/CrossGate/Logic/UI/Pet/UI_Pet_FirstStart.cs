using Lib.Core;
using Logic;
using Logic.Core;
using Packet;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System;
using Framework;
using System.Collections.Generic;


namespace Logic
{
    public enum EPetFirstStart
    {
        None=0,
        LeftChoice=1,
        RightChoice=2,
    }
    public class UI_Pet_FirstStart : UIBase, UI_PetFirstStart_Left.IListener, UI_PetFirstStart_Right.IListener
    {
        private Button btn_Close;
        private UI_PetFirstStart_Left m_Left;
        private UI_PetFirstStart_Right m_Right;
        #region 系统函数
        protected override void OnLoaded()
        {
            btn_Close = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            btn_Close.onClick.AddListener(OnCloseButtonClicked);
            m_Left = AddComponent<UI_PetFirstStart_Left>(transform.Find("Animator/View_Pet/View_Left"));
            m_Left.RegisterListener(this);
            m_Right = AddComponent<UI_PetFirstStart_Right>(transform.Find("Animator/View_Pet/View_Right"));
            m_Right.RegisterListener(this);
        }
        protected override void OnDestroy()
        {

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPetFirstChoiceUpdate, Refresh, toRegister);
        }

        private void Refresh()
        {
            m_Left.Show();
            m_Right.Show();
        }

        protected override void OnShow()
        {
            Sys_Pet.Instance.OnFightPetSetListReq();
            if (Sys_Pet.Instance.PetFirstSetField.Count!=0)
            {
                Refresh();
            }
        }
        protected override void OnUpdate()
        {

        }
        protected override void OnHide()
        {

        }
        #endregion
        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_FirstStart);
        }

        public void OnLeftSelectCeil(bool isShow)
        {
            m_Right.RefreshButton(isShow);
        }

        public void OnRightSelectCeil(uint petuid)
        {
            m_Left.ReqFightSet(petuid);
        }

        public void OnRefreshRightButton(bool isShow)
        {
            m_Right.RefreshButton(isShow);
        }
    }

    public class UI_PetFirstStart_Left : UIComponent
    {
        private List<PetCharryCeil> petLeftList = new List<PetCharryCeil>();
        private Dictionary<GameObject, PetCharryCeil> petCeilGrids = new Dictionary<GameObject, PetCharryCeil>();
        private List<ClientPet> petList=new List<ClientPet>();
        private int visualableGridCount;
        private InfinityGridLayoutGroup infinity;
        private Transform parent;
        private ScrollRect scroll;
        private IListener listener;
        public int selectIndex;
        protected override void Loaded()
        {
            scroll = transform.Find("Scroll_Storage").GetComponent<ScrollRect>();
            parent = transform.Find("Scroll_Storage/Grid").transform;

            infinity = parent.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 7;
            infinity.updateChildrenCallback = UpdateChildrenCallback;
            petCeilGrids.Clear();
            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject go = parent.GetChild(i).gameObject;
                PetCharryCeil petSafeCeilGrid = new PetCharryCeil(EPetFirstStart.LeftChoice);
                petSafeCeilGrid.Init(go.transform);
                petSafeCeilGrid.AddRefreshListener(OnCeilClick);
                petCeilGrids.Add(go, petSafeCeilGrid);
                petLeftList.Add(petSafeCeilGrid);
            }

        }
        private void OnCeilClick(int index,bool isShow)
        {
            selectIndex = index;
            listener?.OnLeftSelectCeil(isShow);
        }
        public override void Show()
        {
            RefreshPetList();
        }
        public override void Hide()
        {

        }
        public void ReqFightSet(uint petid)
        {
            if (selectIndex<=0) return;
            Sys_Pet.Instance.OnFightPetSetReq((uint)selectIndex, petid, 1);
        }
        private void InitPetList()
        {
            petList.Clear();
            foreach (var item in Sys_Pet.Instance.PetFirstSetField)
            {
                if (item.Value>=0)
                {
                    var pet = Sys_Pet.Instance.GetPetByUId((uint)item.Value);
                    petList.Add(pet);
                }
                else
                {
                    petList.Add(null);
                }
            }
        }
        public void RefreshPetList()
        {
            InitPetList();
            selectIndex = -1;
            visualableGridCount = petList.Count;
            infinity.SetAmount(visualableGridCount);
        }
        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index >= visualableGridCount)
                return;
            PetCharryCeil petSafeCeilGrid = petCeilGrids[trans.gameObject];
            petSafeCeilGrid.RefreshData(index+1, index >= petList.Count ? null : petList[index]);
        }
        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnLeftSelectCeil(bool isShow);
        }
    }
    public class UI_PetFirstStart_Right : UIComponent
    {
        private Dictionary<GameObject, PetCharryCeil> petCeilGrids = new Dictionary<GameObject, PetCharryCeil>();
        private List<PetCharryCeil> petGrids = new List<PetCharryCeil>();
        private int visualableGridCount;
        private InfinityGridLayoutGroup infinity;
        private Transform parent;
        private List<ClientPet> petList;


        private IListener listener;

        protected override void Loaded()
        {
            parent = transform.Find("Scroll_Storage/Grid").transform;
            infinity = parent.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 7;
            infinity.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject go = parent.GetChild(i).gameObject;
                PetCharryCeil petSafeCeilGrid = new PetCharryCeil(EPetFirstStart.RightChoice);
                petSafeCeilGrid.Init(go.transform);
                petSafeCeilGrid.AddRefreshListener(onClick,OnSetClick);
                petCeilGrids.Add(go, petSafeCeilGrid);
                petGrids.Add(petSafeCeilGrid);
            }
        }

        private void onClick(int arg1, bool arg2)
        {
            listener?.OnRefreshRightButton(arg2);
        }

        private void OnSetClick(uint obj)
        {
            listener?.OnRightSelectCeil(obj);
        }

        public override void Show()
        {
            RefreshPetList();
        }
        public override void Hide()
        {
        }
        public void RefreshButton(bool isShow)
        {
            for (int i = 0; i < petGrids.Count; i++)
            {
                petGrids[i].SetButtonUpdate(isShow);
            }
        }
        public void RefreshPetList()
        {
            petList = Sys_Pet.Instance.GetCarryPetList();
            visualableGridCount = (int)Sys_Pet.Instance.devicesCount;
            infinity.SetAmount(visualableGridCount);
        }
        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index >= visualableGridCount)
                return;
            PetCharryCeil petCeil = petCeilGrids[trans.gameObject];
            petCeil.RefreshData(index,index >= petList.Count? null : petList[index]);
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnRefreshRightButton(bool isShow);
            void OnRightSelectCeil(uint uid);
        }

    }
    public class PetCharryCeil
    {
        private GameObject goEmpty;
        private Button btnAdd;
        private Text txtAddType;
        private GameObject goPetSet;
        private GameObject goPetContent;
        private Button btnIcon;
        private Image petIconImage;
        private Image petQualityImage;
        private Text petNameText;
        private Text petLevelText;
        private GameObject petType;
        private Text petTypeText;
        private Text petScoreNumText;
        private Button btnRest;
        private GameObject warFlagGo;

        private Action<int,bool> onClick;
        private Action<uint> onSetClick;

        public ClientPet curentPet;
        public EPetFirstStart eType;
        private int m_Index=-1;
        private uint count = 0;
        private bool isButtonShow=false;
        
        public PetCharryCeil(EPetFirstStart eType)
        {
            this.eType = eType;
        }

        public void Init(Transform transform)
        {
            goEmpty = transform.Find("Pet_Empty").gameObject;
            txtAddType = transform.Find("Pet_Empty/Text").GetComponent<Text>();
            btnAdd = transform.Find("Pet_Empty/Btn_Add").GetComponent<Button>();

            goPetSet = transform.Find("Pet_Set").gameObject;
            goPetContent = transform.Find("Pet_Set/Content").gameObject;
            btnIcon = transform.Find("Pet_Set/Content/Image_Head").GetComponent<Button>();
            btnIcon.onClick.AddListener(OnIconClicked);
            petIconImage = transform.Find("Pet_Set/Content/Image_Head/Image_Icon").GetComponent<Image>();
            petNameText = transform.Find("Pet_Set/Content/Text_Name").GetComponent<Text>();
            petLevelText = transform.Find("Pet_Set/Content/Text_Level").GetComponent<Text>();
            petType= transform.Find("Pet_Set/Content/Type").gameObject;
            petTypeText = transform.Find("Pet_Set/Content/Type/Text").GetComponent<Text>();
            warFlagGo = transform.Find("Pet_Set/Content/Image_War").gameObject;
            petScoreNumText = transform.Find("Pet_Set/Content/Text_Score/Text_Value").GetComponent<Text>();
            btnRest = transform.Find("Pet_Set/Content/Button").GetComponent<Button>();
            btnAdd.onClick.RemoveAllListeners();
            btnAdd.onClick.AddListener(OnClicked);
            btnRest.onClick.RemoveAllListeners();
            btnRest.onClick.AddListener(OnRest);

        }
        public void SetData()
        {
            if (null != curentPet)
            {
                goPetSet.SetActive(true);
                goPetContent.SetActive(true);

                var currentPetData = curentPet.petData;
                if (null != currentPetData)
                {
                    ImageHelper.SetIcon(petIconImage, currentPetData.icon_id);
                }
                TextHelper.SetText(petNameText, LanguageHelper.GetTextContent(1000979, Sys_Pet.Instance.GetPetName(curentPet)));
                TextHelper.SetText(petLevelText, LanguageHelper.GetTextContent(6005, curentPet.petUnit.SimpleInfo.Level.ToString()));
                petScoreNumText.text=curentPet.petUnit.SimpleInfo.Score.ToString();

                if (eType == EPetFirstStart.RightChoice)
                {
                    warFlagGo.SetActive(Sys_Pet.Instance.fightPet != null && curentPet.petUnit.Uid == Sys_Pet.Instance.fightPet.GetUid());
                    petType.gameObject.SetActive(false);
                    btnRest.gameObject.SetActive(isButtonShow);
                }
                else
                {
                    warFlagGo.SetActive(false);
                    petType.gameObject.SetActive(true);
                    LeftSet();
                    btnRest.gameObject.SetActive(true);
                }
            }
            else
            {
                if (eType == EPetFirstStart.RightChoice)
                {
                    goPetContent.SetActive(false);
                }
                else
                {
                    goPetSet.SetActive(false);
                    LeftSet();
                }
            }
        }
        private void LeftSet()
        {
            if (m_Index>0)
            {
                var data = CSVBattlePet.Instance.GetConfData((uint)m_Index);
                if (null!=data)
                {
                    petTypeText.text= txtAddType.text = LanguageHelper.GetTextContent(data.groupName);
                }
            }
        }
        public void SetButtonUpdate(bool isShow)
        {
            if (eType == EPetFirstStart.RightChoice)
            {
                isButtonShow = isShow;
                btnRest.gameObject.SetActive(isShow);
            }
        }
        private void OnClicked()
        {
            
            if (eType == EPetFirstStart.RightChoice)
                return;

            count++;
            if (IsOdd(count))
            {
                onClick?.Invoke(m_Index,true);
            }
            else
            {
                onClick?.Invoke(-1,false);
            }
            
        }
        private bool IsOdd(uint num)
        {
            return Convert.ToBoolean(num % 2);
        }
        private void OnIconClicked()
        {
            if (null != curentPet)
                Sys_Pet.Instance.ShowPetTip(curentPet, 0);
        }
        private void OnRest()
        {
            if (null == curentPet)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10695));
                return;
            }
            if (eType == EPetFirstStart.RightChoice)
            {
                CSVPetNew.Data sVPetData = CSVPetNew.Instance.GetConfData(curentPet.petUnit.SimpleInfo.PetId);
                uint petHlevel = CSVPetNewParam.Instance.GetConfData(7).value;

                if (Sys_Role.Instance.Role.Level < sVPetData.participation_lv)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009445));
                    return;
                }
                else if (Sys_Role.Instance.Role.Level + petHlevel < curentPet.petUnit.SimpleInfo.Level)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009446, petHlevel.ToString()));
                    return;
                }
                else if (curentPet.petUnit.SimpleInfo.ExpiredTick != 0)
                {
                    //时效坐骑，无法出战
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000650));
                    return;
                }
                onClick(-1,false);
                onSetClick?.Invoke(curentPet.petUnit.Uid);
            }
            else
            {
                Sys_Pet.Instance.OnFightPetSetReq((uint)m_Index, curentPet.petUnit.Uid, 2);
            }
        }
        public void RefreshData(int index,ClientPet clientPet)
        {
            m_Index = index;
            count = 0;
            curentPet = clientPet;
            SetData();
        }
        public void AddRefreshListener(Action<int,bool> onclicked = null, Action<uint> onsetclicked = null)
        {
            onClick = onclicked;
            onSetClick = onsetclicked;
        }
    }
}



