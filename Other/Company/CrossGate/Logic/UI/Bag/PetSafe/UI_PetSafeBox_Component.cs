using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using UnityEngine.UI;
using Table;
using System;
using UnityEngine.EventSystems;

namespace Logic
{
    public enum EBankPetCeilType
    {
        StorageCeil,
        CarryCeil,
    }

    public class UI_PetSafeCeilGrid
    {
        private GameObject iconGo;
        private Image petIconImage;
        private Image petQualityImage;
        private Text petNameText;
        private Text petLevelText;
        private Text petSkillNumText;
        private Text petScoreNumText;

        private Button getOrPushBtn;

        private GameObject selectBgGo;
        private GameObject selectLineGo;

        private GameObject emptyGo;
        private GameObject fightPetFlagGo;
        private GameObject rareFlagGo;
        private GameObject bingFlagGo;
        public EBankPetCeilType ceilType;
        public UI_PetSafeCeilGrid(EBankPetCeilType eBankPetCeilType)
        {
            ceilType = eBankPetCeilType;
        }
        public ClientPet curentPet;
        public int index;
        private Action<UI_PetSafeCeilGrid> onClick;
        private Action<UI_PetSafeCeilGrid> onIconClick;
        private Action<UI_PetSafeCeilGrid> onLongPressed;
        private Action<UI_PetSafeCeilGrid> onDoubleClick;
        public void Init(Transform transform)
        {
            iconGo = transform.Find("PetIconGameObject").gameObject;
            emptyGo = transform.Find("Pet_Empty").gameObject;
            petIconImage = transform.Find("PetIconGameObject/Image_Icon").GetComponent<Image>();
            petQualityImage = transform.Find("PetIconGameObject/Image_Quality").GetComponent<Image>();
            petNameText = transform.Find("PetIconGameObject/Text_Name").GetComponent<Text>();
            petLevelText = transform.Find("PetIconGameObject/Text_Level/Text").GetComponent<Text>();
            petSkillNumText = transform.Find("PetIconGameObject/Text_Skill").GetComponent<Text>();
            petScoreNumText = transform.Find("PetIconGameObject/Text_Score").GetComponent<Text>();
            
            selectBgGo = transform.Find("Image_Select").gameObject;
            selectLineGo = transform.Find("Image_Line_Selected").gameObject;
            fightPetFlagGo = transform.Find("Image_War").gameObject;
            rareFlagGo = transform.Find("PetIconGameObject/Image_Rare").gameObject;
            bingFlagGo = transform.Find("PetIconGameObject/Text_Bound").gameObject;

            getOrPushBtn = transform.Find("Button").GetComponent<Button>();
            getOrPushBtn.onClick.AddListener(OnGetOrPushBtnClicked);

            Lib.Core.DoubleClickEvent eventListener = Lib.Core.DoubleClickEvent.Get(petIconImage.gameObject);
            eventListener.onClick += (ret) => { OnIconClicked(); };
            eventListener.onDoubleClick += (ret) => { OnDoubled(); };

            Lib.Core.DoubleClickEvent eventListenerClick = Lib.Core.DoubleClickEvent.Get(transform.gameObject);
            eventListenerClick.onClick += (ret) => { OnClicked(); };
            eventListenerClick.onDoubleClick+= (ret) => { OnDoubled(); }; 

            UI_LongPressButton uI_LongPressButton = transform.gameObject.AddComponent<UI_LongPressButton>();
            uI_LongPressButton.onStartPress.AddListener(OnLongPressed);
        }

        public void AddClickListener(Action<UI_PetSafeCeilGrid> onclicked = null, Action<UI_PetSafeCeilGrid> onlongPressed = null, Action<UI_PetSafeCeilGrid> onDoubleClicked = null, Action<UI_PetSafeCeilGrid> onIconclicked = null)
        {
            onClick = onclicked;
            onLongPressed = onlongPressed;
            onDoubleClick = onDoubleClicked;
            onIconClick = onIconclicked;
        }

        private void OnClicked()
        {
            onClick?.Invoke(this);
        }

        private void OnIconClicked()
        {
            onIconClick?.Invoke(this);
        }

        private void OnLongPressed()
        {
            onLongPressed?.Invoke(this);
        }

        private void OnDoubled()
        {
            onDoubleClick?.Invoke(this);
        }

        private void OnGetOrPushBtnClicked()
        {
            onDoubleClick?.Invoke(this);
        }

        public void RefreshData(int index, ClientPet clientPet, int Select)
        {
            this.index = index;
            curentPet = clientPet;
            if (null != curentPet)
            {
                CSVPetNew.Data currentPetData = CSVPetNew.Instance.GetConfData(curentPet.petUnit.SimpleInfo.PetId);
                if (null != currentPetData)
                {
                    ImageHelper.SetIcon(petIconImage, currentPetData.icon_id);
                }
                TextHelper.SetText(petNameText, LanguageHelper.GetTextContent(1000979, Sys_Pet.Instance.GetPetName(curentPet)));
                TextHelper.SetText(petLevelText, LanguageHelper.GetTextContent(1000979, curentPet.petUnit.SimpleInfo.Level.ToString()));
                TextHelper.SetText(petSkillNumText, LanguageHelper.GetTextContent(1000978, Sys_Pet.Instance.GetPetSkillNum(curentPet).ToString()));
                TextHelper.SetText(petScoreNumText, LanguageHelper.GetTextContent(1000977, curentPet.petUnit.SimpleInfo.Score.ToString()));
                rareFlagGo.SetActive(curentPet.petUnit.SimpleInfo.Rare);
                fightPetFlagGo.SetActive(Sys_Pet.Instance.fightPet!=null&&curentPet.petUnit.Uid==Sys_Pet.Instance.fightPet.GetUid());
                bingFlagGo.SetActive(curentPet.petUnit.SimpleInfo.Bind);
            }
            else
            {
                rareFlagGo.SetActive(false);
                fightPetFlagGo.SetActive(false);
                bingFlagGo.SetActive(false);
            }
            bool hasPet = null != curentPet;
            iconGo.SetActive(hasPet);
            emptyGo.SetActive(!hasPet);
            selectBgGo.SetActive(Select == this.index);
            selectLineGo.SetActive(null != curentPet && Select == this.index);
            getOrPushBtn.gameObject.SetActive(null != curentPet && Select == this.index);
        }

        public void SetSelect(bool Select)
        {
            selectBgGo.SetActive(Select);
            selectLineGo.SetActive(null != curentPet && Select);
            getOrPushBtn.gameObject.SetActive(null != curentPet && Select);
        }
    }

    public class UI_PetSafeBox_Right : UIComponent
    {
        private Text numText;
        private Dictionary<GameObject, UI_PetSafeCeilGrid> petCeilGrids = new Dictionary<GameObject, UI_PetSafeCeilGrid>();
        private List<UI_PetSafeCeilGrid> petGrids = new List<UI_PetSafeCeilGrid>();
        private int visualableGridCount;                                
        private InfinityGridLayoutGroup infinity;
        private Transform parent;
        private List<ClientPet> petList;

        public int selectIndex;

        private IListener listener;              

        protected override void Loaded()
        {
            numText = transform.Find("Image_Title/Text_Number").GetComponent<Text>();
            parent = transform.Find("Scroll_Carry/Grid").transform;
            infinity = parent.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 7;
            infinity.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject go = parent.GetChild(i).gameObject;
                UI_PetSafeCeilGrid petSafeCeilGrid = new UI_PetSafeCeilGrid(EBankPetCeilType.CarryCeil);
                petSafeCeilGrid.Init(go.transform);
                petSafeCeilGrid.AddClickListener(OnCeilClick, OnLongClick, OnDoubleClick, OnIconClick);
                petCeilGrids.Add(go, petSafeCeilGrid);                
                petGrids.Add(petSafeCeilGrid);
            }
        }

        private void OnIconClick(UI_PetSafeCeilGrid uI_PetSafeCeilGrid)
        {
            selectIndex = uI_PetSafeCeilGrid.index;
            SetItemSelect();
            if (null != uI_PetSafeCeilGrid.curentPet)
            {
                listener?.OnRightSelectCell();
                Sys_Pet.Instance.ShowPetTip(uI_PetSafeCeilGrid.curentPet, 0);
            }
            listener?.OnRightSelectCell();
        }

        private void OnCeilClick(UI_PetSafeCeilGrid uI_PetSafeCeilGrid)
        {
            selectIndex = uI_PetSafeCeilGrid.index;
            SetItemSelect();
            listener?.OnRightSelectCell();
        }

        private void OnLongClick(UI_PetSafeCeilGrid uI_PetSafeCeilGrid)
        {
            selectIndex = uI_PetSafeCeilGrid.index;
            SetItemSelect();
            if (null != uI_PetSafeCeilGrid.curentPet)
            {
                listener?.OnRightSelectCell();
                Sys_Pet.Instance.ShowPetTip(uI_PetSafeCeilGrid.curentPet, 0);
            }
        }

        private void OnDoubleClick(UI_PetSafeCeilGrid uI_PetSafeCeilGrid)
        {
            selectIndex = uI_PetSafeCeilGrid.index;
            SetItemSelect();
            listener?.OnRightDouble();
        }

        private void SetItemSelect()
        {
            for (int i = 0; i < petGrids.Count; i++)
            {
                UI_PetSafeCeilGrid item = petGrids[i];
                item.SetSelect(selectIndex == item.index);
            }
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index >= visualableGridCount)
                return;
            UI_PetSafeCeilGrid petSafeCeilGrid = petCeilGrids[trans.gameObject];
            petSafeCeilGrid.RefreshData(index, index >= petList.Count? null : petList[index], selectIndex);
        }

        public override void Show()
        {
            RefreshPetList();
        }

        public void RefreshPetList()
        {
            selectIndex = -1;
            petList = Sys_Pet.Instance.GetCarryPetList();
            visualableGridCount = (int)Sys_Pet.Instance.devicesCount;
            infinity.SetAmount(visualableGridCount);
            //标记语言
            TextHelper.SetText(numText, LanguageHelper.GetTextContent(1000974, petList.Count.ToString(), visualableGridCount.ToString()));
        }

        public override void Hide()
        {
            selectIndex = -1;
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnRightDouble();
            void OnRightSelectCell();
        }
    }

    public class UI_PetSafeBox_Left_Drop_Sub : UIComponent
    {
        private Button subBtn;
        private GameObject selectGo;
        private GameObject lockGo;
        private Text subText;
        private int subIndex;

        private Action<int> action;        

        protected override void Loaded()
        {
            subBtn = gameObject.GetComponent<Button>();
            subBtn.onClick.AddListener(() => {
                action?.Invoke(subIndex);
            });
            subText = transform.Find("Text").GetComponent<Text>();
            selectGo = transform.Find("Image_Select").gameObject;
            lockGo = transform.Find("Image_Lock").gameObject;
        }

        public void SetState(int index, bool isSelect, bool isLock = false)
        {
            subIndex = index;
            selectGo.SetActive(isSelect && !isLock);
            lockGo.SetActive(isLock);
            subText.gameObject.SetActive(!isLock);
            CSVPetBank.Data configData = CSVPetBank.Instance.GetConfData(1);
            if(null != configData)
            {
                TextHelper.SetText(subText, LanguageHelper.GetTextContent(configData.tab_name, subIndex.ToString()));
            }            
        }

        public void AddEventAciton(Action<int> action)
        {
            this.action = action;
        }

        public void ResetSelect(uint index)
        {
            bool isSelect = subIndex == index;
            if (selectGo.activeSelf && !isSelect)
                selectGo.SetActive(isSelect);
            else if(!selectGo.activeSelf && isSelect)
                selectGo.SetActive(isSelect);
        }
        
    }

    public class UI_PetSafeBox_Left_Drop : UIComponent
    {
        private GameObject dropSubGo;
        private Transform parent;
        Dictionary<GameObject, UI_PetSafeBox_Left_Drop_Sub> dicSub = new Dictionary<GameObject, UI_PetSafeBox_Left_Drop_Sub>();
        List<UI_PetSafeBox_Left_Drop_Sub> mUI_PetSafeBox_Left_Drop_Subs = new List<UI_PetSafeBox_Left_Drop_Sub>();
        private IListener listener;
        
        protected override void Loaded()
        {
            parent = transform.Find("Lab_Select");
        }

        public void InitData(int select)
        {           
            int count = (int)Sys_Pet.Instance.PetBankCount;
            if(count < Sys_Pet.Instance.GetBankLabelNum())
            {
                count += 1;
            }
            FrameworkTool.CreateChildList(parent, count);
            for (int i = 0; i < count; i++)
            {
                int index = i + 1;
                Transform tran = parent.GetChild(i);
                UI_PetSafeBox_Left_Drop_Sub sub = null;
                if (dicSub.ContainsKey(tran.gameObject))
                {
                    sub = dicSub[tran.gameObject];
                }
                else
                {
                    sub = AddComponent<UI_PetSafeBox_Left_Drop_Sub>(tran);
                    sub.AddEventAciton(DropSubSelect);
                    mUI_PetSafeBox_Left_Drop_Subs.Add(sub);
                    dicSub.Add(tran.gameObject, sub);
                }

                if(i < (int)Sys_Pet.Instance.PetBankCount)
                {
                    sub.SetState(index, select == index);
                }
                else
                {
                    sub.SetState(0, false, true);
                }
                

            }

            gameObject.SetActive(true);
        }

        public void RefreshSelect(uint select)
        {
            for (int i = 0; i < mUI_PetSafeBox_Left_Drop_Subs.Count; i++)
            {
                mUI_PetSafeBox_Left_Drop_Subs[i].ResetSelect(select);
            }
        }

        private void DropSubSelect(int index)
        {
            if(index == 0)
            {
                CSVPetBank.Data petBankData = CSVPetBank.Instance.GetConfData(1);
                if (null != petBankData)
                {
                    uint lockCount = 0;
                    CSVItem.Data resource = CSVItem.Instance.GetConfData(petBankData.fund_type);
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
                    if (petBankData.fund_type == 0)
                    {
                    }
                    else
                    {
                        if (null != resource)
                        {
                            int pageNum = (int)Sys_Pet.Instance.PetBankCount;

                            if (0 < pageNum && pageNum < petBankData.uarray2_value.Count)
                            {
                                if (petBankData.uarray2_value[pageNum].Count >= 2)
                                {
                                    lockCount = petBankData.uarray2_value[pageNum][1];
                                }
                            }
                            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(1000966, lockCount.ToString(), LanguageHelper.GetTextContent(resource.name_id));
                        }
                    }
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        if (null != resource && (lockCount > Sys_Bag.Instance.GetItemCount(resource.id)))
                        {
                            Sys_Bag.Instance.TryOpenExchangeCoinUI((ECurrencyType)resource.id, lockCount);
                            //string content = string.Format(CSVLanguage.Instance.GetConfData(1000934).words, LanguageHelper.GetTextContent(resource.name_id));
                            //Sys_Hint.Instance.PushContent_Normal(content);
                            return;
                        }
                        Sys_Pet.Instance.OnPetBankUnlockReq();
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
            }
            else
            {
                Sys_Pet.Instance.OnPetGetBankInfoReq((uint)index);
            }
            CloseDrop();
        }

        private void CloseDrop()
        {
            listener?.CloseDrop();
            gameObject.SetActive(false);
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void CloseDrop();
        }
    }

    public class UI_PetSafeBox_Left : UIComponent, UI_PetSafeBox_Left_Drop.IListener
    {
        private UI_PetSafeBox_Left_Drop dropGo;
        private Text numText;
        private Text storageName;
        private Dictionary<GameObject, UI_PetSafeCeilGrid> petCeilGrids = new Dictionary<GameObject, UI_PetSafeCeilGrid>();
        private List<UI_PetSafeCeilGrid> petGrids = new List<UI_PetSafeCeilGrid>();
        private int visualableGridCount;
        private InfinityGridLayoutGroup infinity;
        private Transform parent;
        private ScrollRect scroll;
        private List<ClientPet> petList;

        private Button leftBtn;
        private Button rightBtn;
        private Button downBtn;
        private Image downBtnImage;
        public int selectPageIndex;
        public int selectIndex;
        private IListener listener;   
        
        protected override void Loaded()
        {

            dropGo = AddComponent<UI_PetSafeBox_Left_Drop>(transform.Find("Drop_Down"));
            dropGo.RegisterListener(this);
            leftBtn = transform.Find("Image_Title/Button_Left").GetComponent<Button>();
            leftBtn.onClick.AddListener(()=>{ ChangePage(-1); });
            rightBtn = transform.Find("Image_Title/Button_Right").GetComponent<Button>();
            rightBtn.onClick.AddListener(() => { ChangePage(1); });
            downBtn = transform.Find("Image_Title/Button_Down").GetComponent<Button>();
            downBtnImage = transform.Find("Image_Title/Button_Down").GetComponent<Image>();
            downBtn.onClick.AddListener(() => 
            {
                if (!dropGo.gameObject.activeSelf)
                {                    
                    dropGo.InitData(selectPageIndex);
                }                    
                else
                {
                    dropGo.gameObject.SetActive(false);
                }

                DropBtnState();
            });
            storageName = transform.Find("Image_Title/Text_Title").GetComponent<Text>();
            numText = transform.Find("Image_Title/Text_Number").GetComponent<Text>();
            scroll = transform.Find("Scroll_Storage").GetComponent<ScrollRect>();
            parent = transform.Find("Scroll_Storage/Grid").transform;

            infinity = parent.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 7;
            infinity.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject go = parent.GetChild(i).gameObject;
                UI_PetSafeCeilGrid petSafeCeilGrid = new UI_PetSafeCeilGrid(EBankPetCeilType.StorageCeil);
                petSafeCeilGrid.Init(go.transform);
                petSafeCeilGrid.AddClickListener(OnCeilClick, OnLongClick, OnDoubleClick, OnIconClick);
                petCeilGrids.Add(go, petSafeCeilGrid);
                petGrids.Add(petSafeCeilGrid);
            }
            //parent.gameObject.SetActive(false);
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index >= visualableGridCount)
                return;
            UI_PetSafeCeilGrid petSafeCeilGrid = petCeilGrids[trans.gameObject];
            petSafeCeilGrid.RefreshData(index, index >= petList.Count ? null : petList[index], selectIndex);
        }

        private void OnIconClick(UI_PetSafeCeilGrid uI_PetSafeCeilGrid)
        {
            selectIndex = uI_PetSafeCeilGrid.index;
            for (int i = 0; i < petGrids.Count; i++)
            {
                UI_PetSafeCeilGrid item = petGrids[i];
                item.SetSelect(selectIndex == item.index);
            }
            if (null != uI_PetSafeCeilGrid.curentPet)
            {
                Sys_Pet.Instance.ShowPetTip(uI_PetSafeCeilGrid.curentPet, 2, (uint)selectPageIndex);
            }
            listener?.OnLeftSelectCell();
        }

        private void OnCeilClick(UI_PetSafeCeilGrid uI_PetSafeCeilGrid)
        {
            selectIndex = uI_PetSafeCeilGrid.index;
            SetItemSelect();
            listener?.OnLeftSelectCell();
        }

        private void OnLongClick(UI_PetSafeCeilGrid uI_PetSafeCeilGrid)
        {
            selectIndex = uI_PetSafeCeilGrid.index;
            SetItemSelect();
            if (null != uI_PetSafeCeilGrid.curentPet)
            {
                listener?.OnLeftSelectCell();
                Sys_Pet.Instance.ShowPetTip(uI_PetSafeCeilGrid.curentPet, 2, (uint)selectPageIndex);
            }
        }

        private void OnDoubleClick(UI_PetSafeCeilGrid uI_PetSafeCeilGrid)
        {
            selectIndex = uI_PetSafeCeilGrid.index;
            SetItemSelect();
            if (null != uI_PetSafeCeilGrid.curentPet)
                listener?.OnLeftDouble();
        }

        private void SetItemSelect()
        {
            for (int i = 0; i < petGrids.Count; i++)
            {
                UI_PetSafeCeilGrid item = petGrids[i];
                item.SetSelect(selectIndex == item.index);
            }
        }


        public override void Show()
        {
            RefreshBankLabelData(1, false);
            Sys_Pet.Instance.OnPetGetBankInfoReq(1);
        }

        public void GetLabelInfo(uint label)
        {            
            Sys_Pet.Instance.OnPetGetBankInfoReq(label);
        }

        private void SetViewData()
        {
            int dataNum = petList.Count;
            for (int i = 0; i < dataNum; i++)
            {
                UI_PetSafeCeilGrid bankceil = petGrids[i];
                bankceil.RefreshData(i, petList[i], selectIndex);
            }

            int allnum = parent.childCount;
            for (int i = dataNum; i < allnum; i++)
            {
                UI_PetSafeCeilGrid bankceil = petGrids[i];
                bankceil.RefreshData(i, null, selectIndex);
            }            
        }

        public void ButtonStateCheck()
        {
            int maxPage = (int)Sys_Pet.Instance.PetBankCount;
            rightBtn.gameObject.SetActive(!(selectPageIndex == maxPage));
            leftBtn.gameObject.SetActive(!(selectPageIndex == 1));
            downBtnImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 0);
        }

        public void RefreshBankLabelData(uint label, bool init)
        {
            if(init)
                scroll.normalizedPosition = new Vector2(0, 1);
            selectIndex = -1;
            selectPageIndex = (int)label;
            petList = Sys_Pet.Instance.GetBankPetByLabel(label);
            int maxLabelNum = (int)Sys_Pet.Instance.GetBankLabelStorageNumByLabel(label);
            if(null != petList)
            {
                visualableGridCount = petList.Count > maxLabelNum ? petList.Count : maxLabelNum;
            }
            else
            {
                visualableGridCount = 0;
            }

            int petListCount = petList != null ? petList.Count : 0;
            infinity.SetAmount(visualableGridCount);
            parent.gameObject.SetActive(true);

            TextHelper.SetText(numText, LanguageHelper.GetTextContent(1000974, petListCount.ToString(), maxLabelNum.ToString()));
            CSVPetBank.Data bankData = CSVPetBank.Instance.GetConfData(1);
            if (null != bankData)
            {
                TextHelper.SetText(storageName, LanguageHelper.GetTextContent(bankData.tab_name, label.ToString()));
            }
            ButtonStateCheck();
            if (dropGo.gameObject.activeSelf)
                dropGo.RefreshSelect((uint)selectPageIndex);
        }

        private void ChangePage(int addnum)
        {
            selectPageIndex += addnum;
            GetLabelInfo((uint)selectPageIndex);
        }

        public override void Hide()
        {
            selectIndex = -1;
            if (dropGo.gameObject.activeSelf)
                dropGo.gameObject.SetActive(false);
            DropBtnState();
        }

        private void DropBtnState()
        {
            bool isExpand = !dropGo.gameObject.activeSelf;
            float rotateZ = isExpand ? 0f : 180f;
            downBtnImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotateZ);
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public void CloseDrop()
        {
            downBtnImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 0);
        }

        public interface IListener
        {
            void OnLeftDouble();
            void OnLeftSelectCell();
        }
    }

    public class UI_PetSafeBox_Component : UIComponent, UI_PetSafeBox_Right.IListener, UI_PetSafeBox_Left.IListener
    {
        public UI_PetSafeBox_Right mUI_PetSafeBox_Right;
        public UI_PetSafeBox_Left mUI_PetSafeBox_Left;

        private Button storageBtn;
        private Button getBtn;        
        protected override void Loaded()
        {
            mUI_PetSafeBox_Right = AddComponent<UI_PetSafeBox_Right>(transform.Find("View_Right"));
            mUI_PetSafeBox_Right.RegisterListener(this);
            mUI_PetSafeBox_Left = AddComponent<UI_PetSafeBox_Left>(transform.Find("View_Left"));
            mUI_PetSafeBox_Left.RegisterListener(this);
            storageBtn = transform.Find("View_Middle/Image_arrow_L/Image4").GetComponent<Button>();
            storageBtn.onClick.AddListener(OnStorageBtnClicked);
            getBtn = transform.Find("View_Middle/Image_arrow_R/Image4").GetComponent<Button>();
            getBtn.onClick.AddListener(OnGetBtnClicked);
        }

        private void OnStorageBtnClicked()
        {
            int index = mUI_PetSafeBox_Right.selectIndex;
            if (index == -1)
            {
                return;
            }
            uint page = (uint)mUI_PetSafeBox_Left.selectPageIndex;
            if (Sys_Pet.Instance.IsBankLabelIsFull(page))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000937));
                return;
            }
            
            List<ClientPet> clientPets = Sys_Pet.Instance.GetCarryPetList();
           
            if (index >= 0 && index < clientPets.Count)
            {
                ClientPet clientPet = clientPets[index];
                if (Sys_Pet.Instance.fightPet.IsSamePet(clientPet.GetPetUid()))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12205));
                    return;
                }
                else if (clientPet.petUnit.SimpleInfo.ExpiredTick > 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000661));
                    return;
                }
                else 
                if (Sys_Pet.Instance.IsLastPetEntExpiredTick(clientPet.petUnit.SimpleInfo.ExpiredTick > 0))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12206));
                    return;
                }
                else if (clientPet.HasEquipDemonSpiritSphere())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002044));
                    return;
                }
                Sys_Pet.Instance.PetBankMoveReq(1, page, clientPet.petUnit.Uid);
            }
            storageBtn.transform.parent.gameObject.SetActive(false);
        }

        private void OnGetBtnClicked()
        {
            uint page = (uint)mUI_PetSafeBox_Left.selectPageIndex;
            List<ClientPet> clientPets = Sys_Pet.Instance.GetBankPetByLabel(page);
            int index = mUI_PetSafeBox_Left.selectIndex;
            if (index == -1 && index < clientPets.Count)
            {
                return;
            }
            if (Sys_Pet.Instance.PetIsFull())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000939));
                return;
            }           
            
            if (index >= 0 && index < clientPets.Count)
            {
                Sys_Pet.Instance.PetBankMoveReq(2, page, clientPets[index].petUnit.Uid);
            }
            getBtn.transform.parent.gameObject.SetActive(false);
        }

        public override void Show()
        {
            base.Show();
            storageBtn.transform.parent.gameObject.SetActive(false);
            getBtn.transform.parent.gameObject.SetActive(false);
            mUI_PetSafeBox_Right.Show();
            mUI_PetSafeBox_Left.Show();
        }

        public void LeftViewRefresh(uint label)
        {
            mUI_PetSafeBox_Left.RefreshBankLabelData(label, true);
            getBtn.transform.parent.gameObject.SetActive(false);
            storageBtn.transform.parent.gameObject.SetActive(false);
        }

        public void OnStorageChange(uint label)
        {
            mUI_PetSafeBox_Right.RefreshPetList();
            mUI_PetSafeBox_Left.RefreshBankLabelData(label, false);
            getBtn.transform.parent.gameObject.SetActive(false);
            storageBtn.transform.parent.gameObject.SetActive(false);
        }

        public override void Hide()
        {
            base.Hide();
            mUI_PetSafeBox_Left.Hide();
            mUI_PetSafeBox_Right.Hide();
            storageBtn.transform.parent.gameObject.SetActive(false);
            getBtn.transform.parent.gameObject.SetActive(false);
        }

        public void OnRightDouble()
        {
            OnStorageBtnClicked();
            storageBtn.transform.parent.gameObject.SetActive(false);
        }

        public void OnLeftDouble()
        {
            OnGetBtnClicked();
            getBtn.transform.parent.gameObject.SetActive(false);
        }

        public void OnRightSelectCell()
        {
            int index = mUI_PetSafeBox_Right.selectIndex;
            List<ClientPet> clientPets = Sys_Pet.Instance.GetCarryPetList();           
            storageBtn.transform.parent.gameObject.SetActive(index >= 0 && index < clientPets.Count);
        }

        public void OnLeftSelectCell()
        {
            uint page = (uint)mUI_PetSafeBox_Left.selectPageIndex;
            List<ClientPet> clientPets = Sys_Pet.Instance.GetBankPetByLabel(page);
            int index = mUI_PetSafeBox_Left.selectIndex;
            getBtn.transform.parent.gameObject.SetActive(index >= 0 && index < clientPets.Count);
        }
    }
}
