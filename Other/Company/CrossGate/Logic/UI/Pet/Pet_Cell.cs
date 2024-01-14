using Logic.Core;
using System;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{
    public enum EPetGridState
    {
        None = 0,
        Normal = 1,
        Emyty = 2,
        Unlock = 3,
    }

    public class UI_Pet_Cell
    {
        private Transform transform;
        public ClientPet pet;
        private CSVPetNew.Data petData;
        public uint currentPos;
        private Button petCellBtn;
        //宠物信息相关
        private GameObject petGo;
        private Image qualuty;
        private Image icon;
        private RectTransform battleTab;
        private GameObject lockTab;
        private Text level;
        private GameObject addRedPointGo;
        //格子状态相关
        private GameObject noneGo;
        private Button lockGoButton;
        private GameObject redGo;
        
        //选择状态相关
        private GameObject select02;
        private GameObject select01;
        private GameObject unSelect;
        private Button changeTagButton;
        private Action<UI_Pet_Cell> onLongPressed;

        private RectTransform mountTagGo;
        private GameObject domesticationTagGo;
        private GameObject unDomesticationTagGo;
        public EPetGridState gridState;

        public UI_Pet_Cell(uint _currentPos)
        {
            currentPos = _currentPos;
        }

        private bool canClikck = true;
        public bool longState = false;
        public void Init(Transform _transform)
        {
            transform = _transform;
            petCellBtn = transform.Find("ItemBg").GetComponent<Button>();
            petGo = transform.Find("Pet01").gameObject;
            qualuty = transform.Find("Pet01/Item_Quality").GetComponent<Image>();
            icon = transform.Find("Pet01/Image_Icon").GetComponent<Image>();
            battleTab = transform.Find("Pet01/Imag_Fight").GetComponent<RectTransform>();
            lockTab = transform.Find("Pet01/Text_Bound").gameObject;
            level = transform.Find("Pet01/Image_Level/Text_Level").GetComponent<Text>();
            mountTagGo = transform.Find("Pet01/Image_Ride").GetComponent<RectTransform>();
            domesticationTagGo = transform.Find("Pet01/Image_Domestication_1").gameObject; 
            unDomesticationTagGo = transform.Find("Pet01/Image_Domestication_0").gameObject; 
            noneGo = transform.Find("PetNone").gameObject;
            lockGoButton = transform.Find("PetNone/Image_Lock").GetComponent<Button>();
            redGo = transform.Find("PetNone/Image_Dot").gameObject;
            changeTagButton = transform.Find("Image_Change").GetComponent<Button>();
            addRedPointGo = transform.Find("Image_Dot")?.gameObject;
            select01 = transform.Find("Image_Select01").gameObject;
            select02 = transform.Find("Pet01/Image_Select02").gameObject;
            unSelect = transform.Find("ItemBg/Image").gameObject;

            petCellBtn.onClick.AddListener(OnClicked);
            changeTagButton.onClick.AddListener(OnChangeTagClicked);
            lockGoButton.onClick.AddListener(OnClicked);
        }


        private void OnClicked()
        {
            if (canClikck)
            {
                Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnChoosePetCell, this);
            }
        }
        
        private void OnChangeTagClicked()
        {
            longState = false;
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnGotoChangelPostion, this);
        }
        
        void OnLongPressed()
        {
            if(!longState)
            {
                longState = true;
                onLongPressed?.Invoke(this);
            }
        }

        public void ReSetData()
        {
            if (currentPos + 1 > Sys_Pet.Instance.devicesCount)
            {
                gridState = EPetGridState.Unlock;
                addRedPointGo?.SetActive(false);
            }
            else
            {
                gridState = EPetGridState.Emyty;
                addRedPointGo?.SetActive(false);
            }

            pet = Sys_Pet.Instance.GetClientPet2Postion((int)currentPos);
            if(null != pet)
            {
                gridState = EPetGridState.Normal;
            }

            ImageHelper.SetImageGray(icon, false);
            bool has_data = gridState == EPetGridState.Normal;
            noneGo.SetActive(!has_data);
            petGo.SetActive(has_data);
            changeTagButton.gameObject.SetActive(false);
            redGo.SetActive(false);
            longState = false;
            if (gridState == EPetGridState.Normal)
            {
                petData = CSVPetNew.Instance.GetConfData(pet.petUnit.SimpleInfo.PetId);
                transform.name = "PetItem_" + petData.id.ToString();
                ImageHelper.SetIcon(icon, petData.icon_id);
                uint petHlevel = CSVPetNewParam.Instance.GetConfData(7).value;
                uint styleId = 151;
                if (Sys_Role.Instance.Role.Level + petHlevel < pet.petUnit.SimpleInfo.Level)
                {
                    styleId = 22;
                }
                CSVWordStyle.Data worldStyleData = CSVWordStyle.Instance.GetConfData(styleId);
                TextHelper.SetText(level, LanguageHelper.GetTextContent(12462, pet.petUnit.SimpleInfo.Level.ToString()), worldStyleData);
                bool isFightPet = Sys_Pet.Instance.fightPet.IsSamePet(pet.petUnit);
                bool isMout = pet.GetPetUid() == Sys_Pet.Instance.mountPetUid;
                addRedPointGo?.SetActive((isFightPet && Sys_Pet.Instance.IsHasFightPetPointNotUse()) || Sys_Pet.Instance.PetCanAdvanced(pet));
                bool show2Icon = isFightPet && isMout;
                if(show2Icon)
                {
                    battleTab.anchoredPosition = new Vector3(14,-22, 0);
                    mountTagGo.anchoredPosition = new Vector3(40, -22, 0);
                }
                else
                {
                    if(isFightPet)
                    {
                        battleTab.anchoredPosition = new Vector3(14, -22, 0);
                    }
                    else
                    {
                        mountTagGo.anchoredPosition = new Vector3(14, -22, 0);
                    }
                }
                battleTab.gameObject.SetActive(isFightPet);
                mountTagGo.gameObject.SetActive(isMout);
                bool isMount = petData.mount;
                bool isDomes = pet.GetPetIsDomestication();
                domesticationTagGo.SetActive(isMount && isDomes);
                unDomesticationTagGo.SetActive(isMount && !isDomes);
                lockTab.gameObject.SetActive(pet.petUnit.SimpleInfo.Bind);
                return;
            }
            petGo.SetActive(false);
            if (gridState == EPetGridState.Emyty)
            {
                lockGoButton.gameObject.SetActive(false);
            }
            else if (gridState == EPetGridState.Unlock)
            {
                lockGoButton.gameObject.SetActive(true);
            }
        }

        public void RefreshSelect(uint currentSelect)
        {
            if (currentPos == currentSelect && !select02.activeSelf)
            {
                select02.SetActive(true);
                unSelect.SetActive(false);
                select01.SetActive(true);
            }
            else if(currentPos != currentSelect && select02.activeSelf)
            {
                select02.SetActive(false);
                unSelect.SetActive(true);
                select01.SetActive(false);
            }
        }

        public void SetRedState(bool state)
        {
            redGo.SetActive(state);
        }

        public void SetActive(bool state)
        {
            transform.gameObject.SetActive(state);
        }
    }
}
