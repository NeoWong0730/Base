using Logic.Core;
using Lib.AssetLoader;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Framework;

namespace Logic
{
    public class UI_Menu_Convey
    {
        private Transform transform;

        private Text txtPlace;
        private Image imgSlider;

        public void Init(Transform trans)
        {
            transform = trans;

            txtPlace = transform.Find("Image_Place/Text_Place").GetComponent<Text>();
            imgSlider = transform.Find("Image_Slider").GetComponent<Image>();
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void StartTip(uint npcId)
        {
            CSVNpc.Data data = CSVNpc.Instance.GetConfData(npcId);
            if (data != null)
                txtPlace.text = LanguageHelper.GetNpcTextContent(data.name);
            imgSlider.fillAmount = 0f;
        }

        public void SetProgress(float progress)
        {
            imgSlider.fillAmount = progress;
        }
    }
}


