using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using System.Text;
using System;


namespace Logic
{
    public class RewardPanelParam
    {
        public List<ItemIdCount> propList;
        public Vector3 Pos = Vector3.zero;
        public bool isLocalPos = false;
    }
    public class UI_RewardPanel : UIBase
    {
        private Button btn_Close;
        private GameObject go_Content;
        private bool isGet = false;
        RewardPanelParam _panelParam;
        private Transform transPanel;

        protected override void OnOpen(object arg = null)
        {
            if (arg != null)
            {
                _panelParam = (RewardPanelParam)arg;
            }
        }

        protected override void OnLoaded()
        {
            transPanel= transform.Find("Animator");
            btn_Close = transform.Find("Image_off").GetComponent<Button>();
            go_Content = transform.Find("Animator/Image_BG/Scroll View/Viewport/Content").gameObject;
            btn_Close.onClick.AddListener(OnCloseButtonClicked);
        }
        protected override void OnShow()
        {
            if (_panelParam != null)
            {
                if (!_panelParam.Pos.Equals(Vector3.zero))
                {
                    if (!_panelParam.isLocalPos)
                    {
                        Vector3 _lVec = TransformCaculate(_panelParam.Pos);
                        Vector2 screenPoint = new Vector2(_lVec.x, _lVec.y);
                        RectTransformUtility.ScreenPointToWorldPointInRectangle(gameObject.GetComponent<RectTransform>(), screenPoint, CameraManager.mUICamera, out Vector3 pos);
                        transPanel.position = pos;
                    }
                    else
                    {
                        transPanel.localPosition = _panelParam.Pos;
                    }
                }

            }
            if (_panelParam.propList!= null)
            {
                FrameworkTool.CreateChildList(go_Content.transform, _panelParam.propList.Count);
                for (int i = 0; i < _panelParam.propList.Count; i++)
                {
                    PropItem propItem = new PropItem();
                    propItem.BindGameObject(go_Content.transform.GetChild(i).gameObject);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_RewardPanel, new PropIconLoader.ShowItemData(_panelParam.propList[i].id, _panelParam.propList[i].count, true, false, false, false, false,
                            _bShowCount: true, _bUseClick: true, _bShowBagCount: false)));
                }

            }
        }  
        
        private Vector3 TransformCaculate(Vector3 _vec)
        {
            Vector3 _NewVect = _vec;
            RectTransform thisTrans = transPanel.GetComponent<RectTransform>();
            _NewVect += new Vector3(0, thisTrans.rect.height*1.30f,0);
            return _NewVect;
        }
        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_RewardPanel);
        }
    }


}
