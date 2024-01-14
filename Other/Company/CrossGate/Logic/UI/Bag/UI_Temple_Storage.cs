using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class  UI_Temple_PetIcon: UIComponent
    {
        public Image petIconImage;
        public Text petNameText;
        //private Text rareText;
        public Image rareImage;
        public GameObject isBindGo;
        public Button clickBtn;
        public Text petLevelText;
        public uint petId;
        public ClientPet pet;
        protected override void Loaded()
        {
            petIconImage = transform.Find("Image_Icon").GetComponent<Image>();
            petNameText = transform.Find("Text_Name").GetComponent<Text>();
            //rareText = transform.Find("Image_Rare/Text").GetComponent<Text>();
            rareImage = transform.Find("Image_Rare").GetComponent<Image>();
            isBindGo = transform.Find("Text_Bound").gameObject;
            clickBtn = transform.Find("Button_Normal").GetComponent<Button>();
            petLevelText = transform.Find("Text_Level/Text").GetComponent<Text>();

            if (!clickBtn.enabled)
                clickBtn.enabled = true;
            clickBtn.onClick.AddListener(OnClicked);
        }

        public void SetTempIcon(ClientPet petTempPackUnit)
        {
            if(null != petTempPackUnit)
            {
                pet = petTempPackUnit;
                uint iconId = 990800 + (uint)Sys_Pet.Instance.GetPetQuality(pet.petUnit);
                //TextHelper.SetText(rareText, langId);
                ImageHelper.SetIcon(rareImage, iconId);
                TextHelper.SetText(petLevelText, pet.petUnit.SimpleInfo.Level.ToString());
                petId = pet.petUnit.SimpleInfo.PetId;
                bool isBind = pet.petUnit.SimpleInfo.Bind;
                isBindGo.SetActive(isBind);
                CSVPetNew.Data curPet = CSVPetNew.Instance.GetConfData(petId);
                if (null != curPet)
                {
                    TextHelper.SetText(petNameText, LanguageHelper.GetTextContent(curPet.name));
                    ImageHelper.SetIcon(petIconImage, curPet.icon_id);
                }
            }
        }

        private void OnClicked()
        {
            if (null != pet)
            {
                Sys_Pet.Instance.ShowPetTip(pet, 1);
            }
        }

    }

    public class UI_Temple_Storage : UIBase
    {
        private Dictionary<GameObject, UI_Temple_PetIcon> ceilGrids = new Dictionary<GameObject, UI_Temple_PetIcon>();
        private List<UI_Temple_PetIcon> uuidGrids = new List<UI_Temple_PetIcon>();
        List<ClientPet> tempBagData = new List<ClientPet>();
        private InfinityGrid infinity;

        private Button closeBtn;
        private Button fetchBtn;
        private Button batchReleaseBtn;
        protected override void OnLoaded()
        {
            closeBtn = transform.Find("Animator/View_TipsBg02_Smallest/Btn_Close").GetComponent<Button>();
            fetchBtn = transform.Find("Animator/View_List/Button_Tackout").GetComponent<Button>();
            batchReleaseBtn = transform.Find("Animator/View_List/Btn_BatchRelease").GetComponent<Button>();
            closeBtn.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_Temple_Storage);
            });
            fetchBtn.onClick.AddListener(OnFetchPets);
            batchReleaseBtn.onClick.AddListener(OnBatchReleaseBtnClicked);

            infinity = transform.Find("Animator/View_List/Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            
            GameObject go = cell.mRootTransform.gameObject;
            UI_Temple_PetIcon entry = AddComponent<UI_Temple_PetIcon>(go.transform);
            ceilGrids.Add(go, entry);
            uuidGrids.Add(entry);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_Temple_PetIcon entry = cell.mUserData as UI_Temple_PetIcon;
            if (index < tempBagData.Count)
            {
                entry.SetTempIcon(tempBagData[index]);
            }
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<int>(Sys_Pet.EEvents.OnTemplePetBagChange, OnTemplePetBagChange, toRegister);
        }


        protected override void OnShow()
        {
            RefreshUI();
        }

        private void OnTemplePetBagChange(int count)
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            tempBagData = Sys_Pet.Instance.GetTemplePetBagData();
            infinity.CellCount = tempBagData.Count;
            infinity.ForceRefreshActiveCell();
        }

        private void OnFetchPets()
        {
            if (tempBagData.Count == 0)
                return;

            if (Sys_Pet.Instance.PetIsFull())
            {

                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000942));
                return;
            }
            Sys_Pet.Instance.OnPetOutFromPetTempPackReq();
        }

        private void OnBatchReleaseBtnClicked()
        {
            if (tempBagData.Count == 0)
                return;

            UIManager.OpenUI(EUIID.UI_BatchRelease);
        }
    }
}

