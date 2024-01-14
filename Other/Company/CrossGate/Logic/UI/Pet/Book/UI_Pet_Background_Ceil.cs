using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using static Packet.CmdPetGetHandbookRes.Types;
using Lib.Core;

namespace Logic
{
    public enum EPetStoryCeilState
    {
        Null,
        Lock,
        CanUnlock,
        Unlocking,
        Unlock,
    }
    public class UI_Pet_Background_Ceil
    {
        private Transform transform;
        private Button stroyBtn;
        private Text tomelText;
        private Text storyText;
        private GameObject isLockGo;
        private GameObject isActiveGo;
        private GameObject selectGo;
        private EPetStoryCeilState ePetStoryCeilState = EPetStoryCeilState.Null;
        private bool isAni = false;
        private Animator unlockAnimator;
        private Timer unlockTime;
        public uint petId;
        public uint storyId;
        public uint index;
        private CSVPetNewLoveUp.Data cSVBackgroundStoryData;        

        public void Init(Transform _transform)
        {
            transform = _transform;
            stroyBtn = transform.gameObject.GetComponent<Button>();
            stroyBtn.onClick.AddListener(OnStroyBtnClicked);
            tomelText = transform.Find("Image_BG/Text01").GetComponent<Text>();
            storyText = transform.Find("Image_BG/Text02").GetComponent<Text>();
            isLockGo = transform.Find("Image_Lock").gameObject;
            isActiveGo = transform.Find("Image_Active").gameObject;
            unlockAnimator = isActiveGo.GetComponent<Animator>();
            selectGo = transform.Find("Image_BG/Image_Select").gameObject;
        }

        private void OnStroyBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Pet_Story, false, storyId);
            selectGo.SetActive(true);
        }

        public void SetCeilData(uint _storyId, uint _petId, uint _index)
        {
            index = _index;
            storyId = _storyId;
            cSVBackgroundStoryData = CSVPetNewLoveUp.Instance.GetConfData(storyId);
            petId = _petId;
            ResetValue();
        }

        public void ResetActiveState()
        {
            HandbookData bookData = Sys_Pet.Instance.GetPetBookData(petId);
            if(null != bookData)
            {
                uint level = storyId - (uint)(storyId / 1000) * 1000;//目标等级
                bool isLevelActive = level <= bookData.LoveLevel;
                bool isActive = bookData.ClickStory.Contains(level);
                if (isActive)
                {
                    ePetStoryCeilState = EPetStoryCeilState.Unlock;
                }
                else
                {
                    if(isLevelActive)
                    {
                        ePetStoryCeilState = EPetStoryCeilState.CanUnlock;
                    }
                    else
                    {
                        ePetStoryCeilState = EPetStoryCeilState.Lock;
                    }
                }
            }
            else
            {
                ePetStoryCeilState = EPetStoryCeilState.Lock;
            }
            isLockGo.SetActive(ePetStoryCeilState == EPetStoryCeilState.Lock || ePetStoryCeilState == EPetStoryCeilState.CanUnlock);
            isActiveGo.SetActive(ePetStoryCeilState == EPetStoryCeilState.CanUnlock);
            ImageHelper.SetImageGray(stroyBtn, ePetStoryCeilState == EPetStoryCeilState.Lock, true);
        }

        public void ResetValue()
        {
            ResetActiveState();
            isAni = false;
            CSVPetNewLoveUp.Data csvPetStroy = CSVPetNewLoveUp.Instance.GetConfData(storyId);
            if(null != csvPetStroy)
            {
                tomelText.text = (index + 1).ToString();
                SetTextWithE(storyText, LanguageHelper.GetTextContent(csvPetStroy.contests));                
            }
            storyText.gameObject.SetActive(ePetStoryCeilState == EPetStoryCeilState.Unlock);
        }

        public void SelectState()
        {
            selectGo.SetActive(false);
        }


        private void SetTextWithE(Text text, string value)
        {
            TextGenerator generator = new TextGenerator();
            RectTransform rect = text.GetComponent<RectTransform>();
            TextGenerationSettings settings = text.GetGenerationSettings(rect.rect.size);
            generator.Populate(value, settings);
            var charcter = generator.characterCountVisible;
            string s = value;
            if(value.Length > charcter)
            {
                s = value.Substring(0, charcter - 2);
                s += "……";
            }
            text.text = s;
        }
    }
}
