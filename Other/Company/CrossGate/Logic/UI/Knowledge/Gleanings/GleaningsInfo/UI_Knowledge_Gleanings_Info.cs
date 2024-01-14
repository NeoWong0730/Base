using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Knowledge_Gleanings_Info : UIBase
    {
        //ui
        private Button btnFront;
        private Button btnNext;
        private Text textName;
        private Text textDes;

        private GameObject goImgBottom;

        private GameObject goImgTwo;
        private Image imgOld;
        private Image imgNew;

        private uint _gleaningsId = 0;
        private CSVGleanings.Data _gleaningsData;

        private List<uint> activeList = new List<uint>();

        protected override void OnLoaded()
        {            
            btnFront = transform.Find("Animator/Image_mask/Image_messagebg/Arrow_Left/Button_Left").GetComponent<Button>();
            btnFront.onClick.AddListener(OnClickFront);
            btnFront.gameObject.SetActive(false);

            btnNext = transform.Find("Animator/Image_mask/Image_messagebg/Arrow_Right/Button_Right").GetComponent<Button>();
            btnNext.onClick.AddListener(OnClickNext);
            btnNext.gameObject.SetActive(false);

            Button btnOff = transform.Find("Animator/Image_mask/Image_messagebg/Button_off").GetComponent<Button>();
            btnOff.onClick.AddListener(() =>{ this.CloseSelf(); });

            textName = transform.Find("Animator/Image_mask/Image_messagebg/Info_Image/Image_label/Text_label").GetComponent<Text>();
            textDes = transform.Find("Animator/Image_mask/Image_messagebg/Info_Image/Text").GetComponent<Text>();

            //goImgBottom = transform.Find("Animator/Image_mask/Image_messagebg/Image_messagebg1/Image_Bottom").gameObject;

            //goImgTwo = transform.Find("Animator/Image_mask/Image_messagebg/Info_Image/Image_Left").gameObject;
            imgOld = transform.Find("Animator/Image_mask/Image_messagebg/Info_Image/Image_Left").GetComponent<Image>();
            imgNew = transform.Find("Animator/Image_mask/Image_messagebg/Info_Image/Image_Right").GetComponent<Image>();
            //Lib.Core.EventTrigger.Get(imgOld).AddEventListener(EventTriggerType.PointerClick, ClickImgTwo);
            //Lib.Core.EventTrigger.Get(imgNew).AddEventListener(EventTriggerType.PointerClick, ClickImgTwo);
        }

        protected override void OnOpen(object arg)
        {            
            _gleaningsId = 0;
            if (arg != null)
                _gleaningsId = (uint)arg;

            CSVGleanings.Data data = CSVGleanings.Instance.GetConfData(_gleaningsId);

            activeList = Sys_Knowledge.Instance.GetGeoActiveList(data.type2_id);
        }        

        protected override void OnShow()
        {            
            bool onlyOne = activeList.Count == 1;
            btnFront.gameObject.SetActive(!onlyOne);
            btnNext.gameObject.SetActive(!onlyOne);

            UpdateInfo();
        }       

        private void OnClickFront()
        {
            int index = activeList.IndexOf(_gleaningsId);
            index--;
            if (index < 0)
            {
                index = activeList.Count - 1;
            }

            _gleaningsId = activeList[index];
            UpdateInfo();
        }

        private void OnClickNext()
        {
            int index = activeList.IndexOf(_gleaningsId);
            index++;
            if (index >= activeList.Count)
            {
                index = 0;
            }

            _gleaningsId = activeList[index];
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            _gleaningsData = CSVGleanings.Instance.GetConfData(_gleaningsId);
            if (_gleaningsData == null)
            {
                Debug.LogErrorFormat("CSVGleanings Id = {0} is Null", _gleaningsId);
                return;
            }

            textName.text = LanguageHelper.GetTextContent(_gleaningsData.name_id);
            textDes.text = LanguageHelper.GetTextContent(_gleaningsData.describe_id);

            ImageHelper.SetIcon(imgNew, _gleaningsData.show_image2);
            ImageHelper.SetIcon(imgOld, _gleaningsData.show_image);
        }
    }
}


