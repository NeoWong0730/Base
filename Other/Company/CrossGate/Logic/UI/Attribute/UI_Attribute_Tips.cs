using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using System;

namespace Logic
{

    public class AttributeTip
    {
        public uint tipLan;
        public Vector3 pos = Vector3.zero;
    }

    public class UI_Attribute_Tips : UIBase
    {
        private Text describe;
        private Button closeBtn;
        private Transform trans;
        private AttributeTip tip;

        protected override void OnLoaded()
        {
            describe = transform.Find("ImageBG/Text_Property").GetComponent<Text>();
            closeBtn = transform.Find("Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseClicked);
            trans = transform.Find("ImageBG");
        }


        protected override void OnOpen(object arg)
        {            
            tip = (AttributeTip)arg;
        }

        protected override void OnShow()
        {
            describe.text = LanguageHelper.GetTextContent(tip.tipLan);
            if (!tip.pos.Equals(Vector3.zero))
            {
                Vector2 screenPoint = new Vector2(tip.pos.x, tip.pos.y);
                RectTransformUtility.ScreenPointToWorldPointInRectangle(gameObject.GetComponent<RectTransform>(), screenPoint, CameraManager.mUICamera, out Vector3 pos);
                trans.position = pos;
            }
        }

        private void OnCloseClicked()
        {
            UIManager.CloseUI(EUIID.UI_Attribute_Tips);
        }
    }
}
