using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;

namespace Logic
{
    public class UI_PetGodReviewCeil
    {
        public uint curPetId;
        private Image PetCardImage;
        private Image PetCardCircleImage;
        private Text petCardLevelText;

        private Image petHeadImage;
        private Text battleLevelText;
        private Text petNameText;

        private Image petTypeImage;

        private Image dataGo;

        private Button headBtn;
        
        public void Init(Transform transform)
        {
            PetCardImage = transform.Find("GameObject/Image_Levelbg").GetComponent<Image>();
            PetCardCircleImage = transform.Find("GameObject/card_lv").GetComponent<Image>();
            petCardLevelText = transform.Find("GameObject/card_lv/Text").GetComponent<Text>();

            petHeadImage = transform.Find("GameObject/HeadImage").GetComponent<Image>();
            headBtn = petHeadImage.GetComponent<Button>();

            petNameText = transform.Find("GameObject/Image_Namebg/Text_Name").GetComponent<Text>();
            battleLevelText = transform.Find("GameObject/Text_Battle").GetComponent<Text>();
            petTypeImage = transform.Find("GameObject/Image").GetComponent<Image>();
            dataGo = transform.Find("GameObject").GetComponent<Image>();
            headBtn.onClick.AddListener(OnPetClicked);
        }

        private void OnPetClicked()
        {
            PetBookListPar petBookListPar = new PetBookListPar();
            petBookListPar.petId = curPetId;
            petBookListPar.showChangeBtn = false;
            petBookListPar.ePetReviewPageType = EPetBookPageType.Seal;
            UIManager.OpenUI(EUIID.UI_Pet_BookReview, false, petBookListPar);
        }

        public void SetData(uint id)
        {
            curPetId = id;
            RefreshView();
        }

        private void RefreshView()
        {
            CSVPetNew.Data curPet = CSVPetNew.Instance.GetConfData(curPetId);
            if (null != curPet)
            {
                ImageHelper.SetIcon(PetCardImage, Sys_Pet.Instance.SetPetBookQuality(curPet.card_type));
                petCardLevelText.text = curPet.card_lv.ToString();
                ImageHelper.SetIcon(PetCardCircleImage, Sys_Pet.Instance.SetPetBookCircleQuality(curPet.card_type));
                ImageHelper.SetIcon(petHeadImage, curPet.bust);

                petNameText.text = LanguageHelper.GetTextContent(curPet.name);
                battleLevelText.text = LanguageHelper.GetTextContent(2009405, curPet.participation_lv.ToString());
                CSVGenus.Data cSVGenusData = CSVGenus.Instance.GetConfData(curPet.race);
                if (null != cSVGenusData)
                {
                    ImageHelper.SetIcon(petTypeImage, cSVGenusData.rale_icon);
                }
                if (!dataGo.gameObject.activeSelf)
                    dataGo.gameObject.SetActive(true);
                //ImageHelper.SetImageGray(dataGo, !Sys_Pet.Instance.GetPetIsActive(curPetId), true);
            }
            else
            {
                if (dataGo.gameObject.activeSelf)
                    dataGo.gameObject.SetActive(false);
            }
        }
    }
    public class UI_GodPetReview : UIBase
    {
        private InfinityGridLayoutGroup infinity;
        private Dictionary<GameObject, UI_PetGodReviewCeil> reviewGridsGrids = new Dictionary<GameObject, UI_PetGodReviewCeil>();
        private List<UI_PetGodReviewCeil> reviewGrids = new List<UI_PetGodReviewCeil>();
        private List<uint> showGodPetList = new List<uint>();

        private Button closeBtn;
        protected override void OnLoaded()
        {
            closeBtn = transform.Find("Animator/View_TipsBg01_Big/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(() =>
            {
                CloseSelf();
            });
            infinity = transform.Find("Animator/Scroll_View/Grid").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 16;
            infinity.updateChildrenCallback = UpdateChildrenCallback;
            SetSkillItem();
        }

        protected override void OnShow()
        {
            showGodPetList = Sys_Pet.Instance.GetGodReviewPetid();
            infinity.SetAmount(showGodPetList.Count);
        }

        private void SetSkillItem()
        {
            for (int i = 0; i < infinity.transform.childCount; i++)
            {
                Transform tran = infinity.transform.GetChild(i);
                UI_PetGodReviewCeil petGodReviewCeil = new UI_PetGodReviewCeil();
                petGodReviewCeil.Init(tran);
                reviewGrids.Add(petGodReviewCeil);
                reviewGridsGrids.Add(tran.gameObject, petGodReviewCeil);
            }
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= reviewGrids.Count)
                return;
            if (reviewGridsGrids.ContainsKey(trans.gameObject))
            {
                UI_PetGodReviewCeil petGodReviewCeil = reviewGridsGrids[trans.gameObject];
                petGodReviewCeil.SetData(showGodPetList[index]);
            }
        }
    }
}
