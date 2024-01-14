using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Table;
using Lib.Core;
using UnityEngine.EventSystems;

namespace Logic
{
    public class UI_Pet_BookList_PetCeil
    {
        public uint curPetId;
        public Transform transform;
        private Image PetCardImage;
        private Image PetCardCircleImage;
        private Text petCardLevelText;

        private Image petHeadImage;
        private Text battleLevelText;
        private Text petNameText;
        private GameObject lockGo;
        private Image petTypeImage;
        private GameObject redGo;
        private Image dataGo;
        private Button funcBtn;
        private Action<uint> action;
        private bool m_ForceHideRed;
        public void Init(Transform _transform)
        {
            transform = _transform;
            PetCardImage = transform.Find("GameObject/Image_Levelbg").GetComponent<Image>();
            PetCardCircleImage = transform.Find("GameObject/card_lv").GetComponent<Image>();
            petCardLevelText = transform.Find("GameObject/card_lv/Text").GetComponent<Text>();
            petHeadImage = transform.Find("GameObject/HeadImage").GetComponent<Image>();
            funcBtn = petHeadImage.gameObject.GetComponent<Button>();
            funcBtn.onClick.AddListener(OnPetClicked);
            lockGo = transform.Find("GameObject/Image_IconNone").gameObject;
            petNameText = transform.Find("GameObject/Image_Namebg/Text_Name").GetComponent<Text>();
            battleLevelText = transform.Find("GameObject/Text_Battle").GetComponent<Text>();
            petTypeImage = transform.Find("GameObject/Image").GetComponent<Image>();
            dataGo = transform.Find("GameObject").GetComponent<Image>();
            redGo = transform.Find("Image_Dot").gameObject;
        }

        private void OnPetClicked()
        {
            action?.Invoke(curPetId);            
        }

        public void SetData(uint id, bool forceHideRed = false)
        {
            curPetId = id;
            m_ForceHideRed = forceHideRed;
            RefreshView();
        }

        public void RefreshView()
        {
            CSVPetNew.Data curPet = CSVPetNew.Instance.GetConfData(curPetId);
            if (null != curPet)
            {
                transform.name = "Pet_" + curPetId.ToString();
                ImageHelper.SetIcon(PetCardImage, Sys_Pet.Instance.SetPetBookQuality(curPet.card_type));
                ImageHelper.SetIcon(PetCardCircleImage, Sys_Pet.Instance.SetPetBookCircleQuality(curPet.card_type));
                petCardLevelText.text = curPet.card_lv.ToString();
                redGo.SetActive(Sys_Pet.Instance.GetPetBookCanActive(curPetId) || Sys_Pet.Instance.GetPetBookLoveCanUp(curPetId));
                ImageHelper.SetIcon(petHeadImage, curPet.bust);

                petNameText.text = LanguageHelper.GetTextContent(curPet.name);
                battleLevelText.text = LanguageHelper.GetTextContent(2009405, curPet.participation_lv.ToString());
                CSVGenus.Data cSVGenusData = CSVGenus.Instance.GetConfData(curPet.race);
                if(null != cSVGenusData)
                {
                    ImageHelper.SetIcon(petTypeImage, cSVGenusData.rale_icon);
                }
                if (!dataGo.gameObject.activeSelf)
                    dataGo.gameObject.SetActive(true);
                lockGo.SetActive(!Sys_Pet.Instance.GetPetIsActive(curPetId));
            }
            else
            {
                if(dataGo.gameObject.activeSelf)
                    dataGo.gameObject.SetActive(false);
            }

            if (m_ForceHideRed)
            {
                redGo.SetActive(false);
            }
        }

        public void Register(Action<uint> action)
        {
            this.action = action;
        }
    }

}

