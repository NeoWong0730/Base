 using Packet;
using UnityEngine;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine.UI;
using Table;
using System.Linq;
using Lib.Core;
namespace Logic

{
    public enum BattlePetState
    {
        None,
        Battle,      
        Wait,          
        Forbid,       
    }

    public enum ForbidPetState
    {
        Canuse,
        PetLevelMax,
        RoleLevelMin,
        PetLoyatyMin,
        PetUesd,
        NoUseCount,
        IsInBattle,
    }

    public class UI_MainBattle_Pet_Layout
    {
        public Transform transform;
        public GameObject petGo;
        public GameObject nopetGo;
        public Button callBtn;
        public Button closeBtn;
        public Text lefttime;

        public void Init(Transform transform)
        {
            this.transform = transform;
            petGo = transform.Find("Animator/List/Scroll_View/Viewport/Item").gameObject;
            nopetGo = transform.Find("Animator/List/Image_NoPet").gameObject;
            closeBtn = transform.Find("Button_Close").GetComponent<Button>();
            callBtn = transform.Find("Animator/List/Btn_01").GetComponent<Button>();
            lefttime= transform.Find("Animator/List/Text_Tip").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnClose_ButtonClicked);
            callBtn.onClick.AddListener(listener.OnCall_ButtonClicked);
        }

        public interface IListener
        {
            void OnCall_ButtonClicked();
            void OnClose_ButtonClicked();
        }
    }

    public class UI_Select_Pet : UIComponent
    {
        private Image petIcon;
        private Button iconBtn;
        private Button waitBtn;
        private Button battleBtn;
        private Button forbidBtn;
        private Text name;
        private Text level;
        private Text loyatyNum;
        private Text bloodNum;
        private Text magicNum;
        private Slider blood;
        private Slider magic;
        private GameObject attrGo;
        private GameObject resttipGo;

        private GameObject waitSelected;
        private GameObject battleSelected;
        private GameObject forbidSelected;

        private uint petHlevel;
        public PetUnit petuint;
        private ClientPet clientPet;
        private CSVPetNew.Data csvPet;
        private BattlePetState petstate;
        public ForbidPetState forbidState;

        protected override void Loaded()
        {
            petIcon = transform.Find("Image_Icon_BG/Image_Icon").GetComponent<Image>();
            iconBtn = transform.Find("Image_Icon_BG/Image_Icon").GetComponent<Button>();
            iconBtn.onClick.AddListener(OniconBtnClick);
            waitBtn = transform.Find("Button_Wait").GetComponent<Button>();
            waitBtn.onClick.AddListener(OnwaitBtnClick);
            battleBtn = transform.Find("Button_Battle").GetComponent<Button>();
            battleBtn.onClick.AddListener(OnbattleBtnClick);
            forbidBtn = transform.Find("Button_Dead").GetComponent<Button>();
            forbidBtn.onClick.AddListener(OndeadBtnClick);
            name = transform.Find("Text_Name").GetComponent<Text>();
            level = transform.Find("Text_Level").GetComponent<Text>();
            loyatyNum = transform.Find("Image_Loyal/Text").GetComponent<Text>();
            blood = transform.Find("Slider_Hp").GetComponent<Slider>();
            magic = transform.Find("Slider_Mp").GetComponent<Slider>();
            bloodNum = transform.Find("Slider_Hp/Text_Percent").GetComponent<Text>();
            magicNum = transform.Find("Slider_Mp/Text_Percent").GetComponent<Text>();
            attrGo = transform.Find("Attr_Grid/Image_Attr01").gameObject;
            resttipGo = transform.Find("Text_Tip").gameObject;

            waitSelected = transform.Find("Button_Wait/Image_Selected").gameObject;
            battleSelected = transform.Find("Button_Battle/Image_Selected").gameObject;
            forbidSelected = transform.Find("Button_Dead/Image_Selected").gameObject;

            petHlevel = CSVPetNewParam.Instance.GetConfData(7).value;

        }

        #region ButtonClick
        private void OnwaitBtnClick()
        {
            if (GameCenter.fightControl.CanUseSkill)
            {
                Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnCallPet, petuint.Uid, forbidState);
            }
        }

        private void OndeadBtnClick()
        {
            Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnCallPet, petuint.Uid, forbidState);
        }

        private void OnbattleBtnClick()
        {
            Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnCallPet, petuint.Uid, forbidState);
        }

        private void OniconBtnClick()
        {
            UIManager.OpenUI(EUIID.UI_Pet_Details,false, clientPet);
        }
        #endregion

        #region Function
        public void Refresh(PetUnit petunit)
        {
            this.petuint = petunit;
            this.csvPet = CSVPetNew.Instance.GetConfData(petuint.SimpleInfo.PetId);
            for (int i = 0; i < Sys_Pet.Instance.petsList.Count; ++i)
            {
                if (Sys_Pet.Instance.petsList[i].petUnit.Uid == petuint.Uid)
                {
                    clientPet = Sys_Pet.Instance.petsList[i];
                }
            }
            if (csvPet != null)
            {
                ImageHelper.SetIcon(petIcon, csvPet.icon_id);
                if (petuint.SimpleInfo.Name.IsEmpty)
                {
                    name.text = CSVLanguage.Instance.GetConfData(csvPet.name).words;
                }
                else
                {
                    name.text = petuint.SimpleInfo.Name.ToStringUtf8();
                }
                level.text = LanguageHelper.GetTextContent(2011127, petunit.SimpleInfo.Level.ToString());
                loyatyNum.text = petuint.SimpleInfo.Loyalty + "/" + Sys_Pet.Instance.MaxLoyalty;
                if (GameCenter.mainFightPet!= null&&petunit.Uid == GameCenter.mainFightPet.battleUnit.PetId) //出战宠物
                {
                    petstate = BattlePetState.Battle;
                    forbidState = ForbidPetState.IsInBattle;
                }
                else
                {
                    if (GameCenter.fightControl.forbidpetsList.Contains(petunit.Uid))
                    {
                        petstate = BattlePetState.Forbid;
                        if (Sys_Role.Instance.Role.Level + petHlevel < petuint.SimpleInfo.Level)
                        {
                            forbidState = ForbidPetState.PetLevelMax;
                        }
                        else if (petuint.SimpleInfo.Loyalty == 0)
                        {
                            forbidState = ForbidPetState.PetLoyatyMin;
                        }
                        else if (Sys_Role.Instance.Role.Level < csvPet.participation_lv)
                        {
                            forbidState = ForbidPetState.RoleLevelMin;
                        }
                        else if (GameCenter.fightControl.currentUsePetTime == 0)
                        {
                            forbidState = ForbidPetState.NoUseCount;
                        }
                        else
                        {
                            forbidState = ForbidPetState.PetUesd;
                        }
                    }
                    else
                    {
                        petstate = BattlePetState.Wait;
                        forbidState = ForbidPetState.Canuse;
                    }
                }
                FrameworkTool.DestroyChildren(attrGo.transform.parent.gameObject, attrGo.transform.name);
                for (int i=0;i< csvPet.init_attr.Count;++i)
                {
                    uint id = csvPet.init_attr[i][0];
                    if (CSVAttr.Instance.TryGetValue(id, out CSVAttr.Data data)&&id!=101)
                    {
                        GameObject go = i == 0 ? attrGo : GameObject.Instantiate<GameObject>(attrGo, attrGo.transform.parent);
                        ImageHelper.SetIcon(go.transform.Find("Image_Attr_Icon").GetComponent<Image>(), data.attr_icon);
                        go.transform.Find("Text_Number").GetComponent<Text>().text = csvPet.init_attr[i][1].ToString();
                    }
                }
                SetPetState(petstate);
            }
        }

        private void SetPetState(BattlePetState state)
        {
            if(state == BattlePetState.Battle)
            {
                waitBtn.gameObject.SetActive(false);
                battleBtn.gameObject.SetActive(true);
                forbidBtn.gameObject.SetActive(false);
                blood.gameObject.SetActive(true);
                magic.gameObject.SetActive(true);
                resttipGo.SetActive(false);
                SetHpMp(state);
            }
            else if(state == BattlePetState.Wait)
            {
                waitBtn.gameObject.SetActive(true);
                battleBtn.gameObject.SetActive(false);
                forbidBtn.gameObject.SetActive(false);
                blood.gameObject.SetActive(true);
                magic.gameObject.SetActive(true);
                resttipGo.SetActive(false);
                SetHpMp(state);
            }
            else if(state == BattlePetState.Forbid)
            {
                waitBtn.gameObject.SetActive(false);
                battleBtn.gameObject.SetActive(false);
                forbidBtn.gameObject.SetActive(true);
                blood.gameObject.SetActive(false);
                magic.gameObject.SetActive(false);
                resttipGo.SetActive(true);
                ImageHelper.SetImageGray(forbidBtn.GetComponent<Image>(), true,true);
            }           
        }

        private void SetHpMp(BattlePetState type)
        {
            if (type == BattlePetState.Battle)
            {
                if (GameCenter.mainFightPet == null)
                {
                    DebugUtil.Log(ELogType.eCombat, "GameCenter.mainFightPet is null");
                    return;
                }
                BattleUnit unit = GameCenter.mainFightPet.battleUnit;
                blood.value = (float)unit.CurHp / unit.MaxHp;
                bloodNum.text = LanguageHelper.GetTextContent(2009377, unit.CurHp.ToString(), unit.MaxHp.ToString());
                magic.value = (float)unit.CurMp / unit.MaxMp;
                magicNum.text = LanguageHelper.GetTextContent(2009377, unit.CurMp.ToString(), unit.MaxMp.ToString());
            }
            else
            {
                long curHp = clientPet.GetAttrValueByAttrId((int)EPkAttr.CurHp);
                long maxHp = clientPet.GetAttrValueByAttrId((int)EPkAttr.MaxHp);
                long curMp = clientPet.GetAttrValueByAttrId((int)EPkAttr.CurMp);
                long maxMp = clientPet.GetAttrValueByAttrId((int)EPkAttr.MaxMp);
                blood.value = (float)curHp / maxHp;
                bloodNum.text = LanguageHelper.GetTextContent(2009377, curHp.ToString(), maxHp.ToString());
                magic.value = (float)curMp / maxMp;
                magicNum.text = LanguageHelper.GetTextContent(2009377, curMp.ToString(), maxMp.ToString());
            }
        }

        public void SetSelected(uint selectedUid)
        {
            bool isSelected = petuint.Uid == selectedUid;
            waitSelected.SetActive(isSelected);
            forbidSelected.SetActive(isSelected);
            battleSelected.SetActive(isSelected);
        }
        #endregion
    }

    public class UI_MainBattle_Pet : UIBase,UI_MainBattle_Pet_Layout.IListener
    {
        private UI_MainBattle_Pet_Layout layout = new UI_MainBattle_Pet_Layout();
        private uint selectedPetUid;
        private ForbidPetState selectedPetState;
        private List<UI_Select_Pet> pets;
        private List<PetUnit> waitPetsList;
        private List<PetUnit> forbidPetsList;
        private PetUnit fightPet;
        private PetUnit petUnitTemp;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            pets = new List<UI_Select_Pet>();
            waitPetsList = new List<PetUnit>();
            forbidPetsList = new List<PetUnit>();
        } 

        protected override void OnShow()
        {
            CheckPetFightLevelAndLoyaty();
            AddList();
            layout.lefttime.text = LanguageHelper.GetTextContent(2009710,GameCenter.fightControl.currentUsePetTime.ToString());
            ImageHelper.SetImageGray(layout.callBtn.GetComponent<Image>(), GameCenter.fightControl.currentUsePetTime == 0);
            layout.callBtn.enabled = GameCenter.fightControl.currentUsePetTime != 0;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Net_Combat.Instance.eventEmitter.Handle<uint, ForbidPetState>(Net_Combat.EEvents.OnCallPet, OnCallPet, toRegister);
        }

        protected override void OnDestroy()
        {
            UIManager.CloseUI(EUIID.UI_MainBattle_Pet);
        }

        private void OnCallPet(uint petUid, ForbidPetState petState)
        {
            selectedPetUid = petUid;
            selectedPetState = petState;
            for(int i=0;i< pets.Count; ++i)
            {
                pets[i].SetSelected(petUid);
            }
        }

        #region Fuction
        private void AddList()
        {
            pets.Clear();
            DefaultState();
            int count =0;
            if (Net_Combat.Instance.petsInBattle.Count == 0)
            {
                layout.petGo.SetActive(false);
                layout.nopetGo.SetActive(true);
            }
            else
            {
                layout.petGo.SetActive(true);
                layout.nopetGo.SetActive(false);
                OrderPetList();
                if (fightPet != null)
                {
                    ClonePetItem(count, fightPet);
                    ++count;
                }
                for (int i=0;i<waitPetsList.Count;++i)
                {
                    ClonePetItem(count, waitPetsList[i]);
                    ++count;
                }
                for (int i = 0; i < forbidPetsList.Count; ++i)
                {
                    ClonePetItem(count, forbidPetsList[i]);
                    ++count;
                }
                if (pets.Count > 0)
                {
                    OnCallPet(pets[0].petuint.Uid, pets[0].forbidState);
                }
            }
        }

        private void CheckPetFightLevelAndLoyaty()
        {
            uint petHlevel = CSVPetNewParam.Instance.GetConfData(7).value;
            for (int i = 0; i < Net_Combat.Instance.petsInBattle.Count; ++i)
            {
                petUnitTemp = Net_Combat.Instance.petsInBattle[i].Pet;
                CSVPetNew.Data sVPetData = CSVPetNew.Instance.GetConfData(petUnitTemp.SimpleInfo.PetId);
                if (GameCenter.mainFightPet!=null && GameCenter.mainFightPet.battleUnit.PetId != petUnitTemp.Uid && !GameCenter.fightControl.forbidpetsList.Contains(petUnitTemp.Uid))
                {
                    if (GameCenter.fightControl.currentUsePetTime == 0)
                    {
                        GameCenter.fightControl.forbidpetsList.Add(petUnitTemp.Uid);
                    }
                    else if (Sys_Role.Instance.Role.Level < sVPetData.participation_lv)
                    {
                        GameCenter.fightControl.forbidpetsList.Add(petUnitTemp.Uid);
                    }
                    else if (Sys_Role.Instance.Role.Level + petHlevel < petUnitTemp.SimpleInfo.Level)
                    {
                        GameCenter.fightControl.forbidpetsList.Add(petUnitTemp.Uid);
                    }
                    else if(petUnitTemp.SimpleInfo.Loyalty == 0)
                    {
                        GameCenter.fightControl.forbidpetsList.Add(petUnitTemp.Uid);
                    }
                    else if (Net_Combat.Instance.petsInBattle[i].Firenum>=1)
                    {
                        GameCenter.fightControl.forbidpetsList.Add(petUnitTemp.Uid);
                    }
                }
            }
        }

        private void OrderPetList()
        {
            List<PetUnit> waitPetsListTemp = new List<PetUnit>();
            List<PetUnit> forbidPetsListTemp = new List<PetUnit>();
            forbidPetsList.Clear();
            waitPetsList.Clear();
            for (int i = 0; i < Net_Combat.Instance.petsInBattle.Count;++i)
            {
                if (GameCenter.mainFightPet != null && Net_Combat.Instance.petsInBattle[i].Pet.Uid == GameCenter.mainFightPet.battleUnit.PetId)
                {
                    fightPet = Net_Combat.Instance.petsInBattle[i].Pet;
                }
                else if (GameCenter.fightControl.forbidpetsList.Contains(Net_Combat.Instance.petsInBattle[i].Pet.Uid))
                {
                    forbidPetsListTemp.Add(Net_Combat.Instance.petsInBattle[i].Pet);
                }
                else
                {
                    waitPetsListTemp.Add(Net_Combat.Instance.petsInBattle[i].Pet);
                }
            }
            waitPetsList = waitPetsListTemp.OrderBy(u => u.SimpleInfo.Score).ToList();
            waitPetsList.Reverse();
            forbidPetsList = forbidPetsListTemp.OrderBy(u => u.SimpleInfo.Score).ToList();
            forbidPetsList.Reverse();
        }

        private void ClonePetItem(int i,PetUnit petUint)
        {
            GameObject go = i == 0 ? layout.petGo : GameObject.Instantiate<GameObject>(layout.petGo, layout.petGo.transform.parent);
            UI_Select_Pet pet = AddComponent<UI_Select_Pet>(go.transform);
            pet.Refresh(petUint);
            pets.Add(pet);
        }

        private void DefaultState()
        {
            FrameworkTool.DestroyChildren(layout.petGo.transform.parent.gameObject, layout.petGo.transform.name);
        }
        #endregion

        public void OnClose_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_MainBattle_Pet);
        }

        public void OnCall_ButtonClicked()
        {
            switch (selectedPetState)
            {
                case ForbidPetState.Canuse:
                    GameCenter.fightControl.currentOperationTpye = FightControl.EOperationType.Pet;
                    GameCenter.fightControl.currentOperationID = selectedPetUid;
                    if (GameCenter.fightControl.CanUseSkill)
                    {
                        GameCenter.fightControl.DoSelectPet();
                    }
                    UIManager.CloseUI(EUIID.UI_MainBattle_Pet);
                    break;
                case ForbidPetState.PetLevelMax:
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10772));
                    break;
                case ForbidPetState.PetLoyatyMin:
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10773));
                    break;
                case ForbidPetState.PetUesd:
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10774));
                    break;
                case ForbidPetState.RoleLevelMin:
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10775));
                    break;
                case ForbidPetState.NoUseCount:
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10776));
                    break;
                case ForbidPetState.IsInBattle:
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10888));
                    break;
            }
        }
    }
}
