using Logic.Core;
using System;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{
    public class PetMountCell
    {
        private Transform transform;
        public ClientPet pet;
        private CSVPetNew.Data petData;
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
        
        //选择状态相关
        private GameObject select02;
        private GameObject select01;
        private GameObject unSelect;
        private RectTransform mountTagGo;
        private GameObject domesticationTagGo;
        private GameObject unDomesticationTagGo;
        private GameObject recommendGo;

        public int index;
        private Action<int> action;
        public void Init(Transform _transform)
        {
            transform = _transform;
            petCellBtn = transform.Find("ItemBg").GetComponent<Button>();
            unSelect = transform.Find("ItemBg/Image").gameObject;

            petGo = transform.Find("Pet01").gameObject;
            qualuty = transform.Find("Pet01/Item_Quality").GetComponent<Image>();
            icon = transform.Find("Pet01/Image_Icon").GetComponent<Image>();
            battleTab = transform.Find("Pet01/Imag_Fight").GetComponent<RectTransform>();
            lockTab = transform.Find("Pet01/Text_Bound").gameObject;
            level = transform.Find("Pet01/Image_Level/Text_Level/Text_Num").GetComponent<Text>();
            mountTagGo = transform.Find("Pet01/Image_Ride").GetComponent<RectTransform>();
            domesticationTagGo = transform.Find("Pet01/Image_Domestication_1").gameObject; 
            unDomesticationTagGo = transform.Find("Pet01/Image_Domestication_0").gameObject;
            recommendGo = transform.Find("Pet01/Image_Recommend")?.gameObject;
            lockGoButton = transform.Find("PetNone/Image_Lock").GetComponent<Button>();
            select01 = transform.Find("Image_Select01").gameObject;
            select02 = transform.Find("Pet01/Image_Select02").gameObject;
            unSelect = transform.Find("ItemBg/Image").gameObject;
            petCellBtn.onClick.AddListener(OnClicked);
            lockGoButton.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            action?.Invoke(index);
        }

        public void AddListen(Action<int> action)
        {
            this.action = action;
        }

        public void SetSelect(bool state)
        {
            unSelect.SetActive(!state);
            select01.SetActive(state);
            select02.SetActive(state);
        }

        public void ReSetData(ClientPet pet, uint petId, int index)
        {
            this.index = index;
            this.pet = pet;
            ImageHelper.SetImageGray(icon, pet == null, true);
            recommendGo?.SetActive(pet == null);
            if (pet != null)
            {
                petData = CSVPetNew.Instance.GetConfData(pet.petUnit.SimpleInfo.PetId);
                ImageHelper.SetIcon(icon, petData.icon_id);
                level.text = pet.petUnit.SimpleInfo.Level.ToString();
                bool isFightPet = Sys_Pet.Instance.fightPet.IsSamePet(pet.petUnit);
                bool isMout = pet.GetPetUid() == Sys_Pet.Instance.mountPetUid;
                bool show2Icon = isFightPet && isMout;
                if (show2Icon)
                {
                    battleTab.anchoredPosition = new Vector3(14, -22, 0);
                    mountTagGo.anchoredPosition = new Vector3(40, -22, 0);
                }
                else
                {
                    if (isFightPet)
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
            }
            else
            {
                petData = CSVPetNew.Instance.GetConfData(petId);
                level.text = "1";
                ImageHelper.SetIcon(icon, petData.icon_id);
                battleTab.gameObject.SetActive(false);
                mountTagGo.gameObject.SetActive(false);
                domesticationTagGo.SetActive(false);
                unDomesticationTagGo.SetActive(false);
                lockTab.gameObject.SetActive(false);
            }
        }
    }
}
