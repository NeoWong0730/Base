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
    public class UI_WorldBossRewardList : UIBase
    {
        private RectTransform transAni;
        private Button btn_Close;
        private GameObject go_Content;
        private RectTransform Image_BG;
        private List<ItemIdCount> propList;
        private bool isGet = false;
        private Button clickBtn;

        protected override void OnOpen(object arg = null)
        {
            propList = null;
            if (arg is Tuple<List<ItemIdCount>, bool,Button>){
                Tuple<List<ItemIdCount>, bool,Button> tuple = arg as Tuple<List<ItemIdCount>, bool, Button>;
                propList = tuple.Item1;
                isGet = tuple.Item2;
                clickBtn = tuple.Item3;
            }        
        }

        protected override void OnLoaded()
        {
            transAni = transform.Find("Animator").GetComponent<RectTransform>();
            Image_BG = transform.Find("Animator/Image_BG").GetComponent<RectTransform>();
            btn_Close = transform.Find("Image_off").GetComponent<Button>();
            go_Content = transform.Find("Animator/Image_BG/Scroll View/Viewport/Content").gameObject;
            btn_Close.onClick.AddListener(OnCloseButtonClicked);
        }
        protected override void OnShow()
        {
            SetTipPos();
            if (propList!=null)
            {
                FrameworkTool.CreateChildList(go_Content.transform, propList.Count);
                for (int i = 0; i < propList.Count; i++)
                {
                    PropItem propItem = new PropItem();
                    propItem.BindGameObject(go_Content.transform.GetChild(i).gameObject);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_RewardPanel, new PropIconLoader.ShowItemData(propList[i].id, propList[i].count, true, false, false, false, false,
                            _bShowCount: true, _bUseClick: true, _bShowBagCount: false)));
                    propItem.SetGot(isGet);
                }

            }
        }

        private void SetTipPos()
        {
            var clickBtnPos = CameraManager.mUICamera.WorldToScreenPoint(clickBtn.GetComponent<RectTransform>().position);
            CanvasScaler canvasScaler = transform.GetComponent<CanvasScaler>();
            
            float factorx = Screen.height / canvasScaler.referenceResolution.y;
            float factory = Screen.width / canvasScaler.referenceResolution.x;
            Vector3 tempVec = Vector3.zero;
            if (clickBtnPos.x >= Screen.width * 0.5f){
                tempVec.x = clickBtnPos.x - Image_BG.rect.width * factory;
            }
            else{
                tempVec.x = clickBtnPos.x + 40 * factory;
            }

            if (clickBtnPos.y >= Screen.height * 0.5f){
                tempVec.y = clickBtnPos.y - Image_BG.rect.height*0.5f * factorx;
            }
            else{
                tempVec.y = clickBtnPos.y + Image_BG.rect.height*0.5f * factorx;
            } 
            if(clickBtnPos.y + Image_BG.rect.height * factorx > Screen.height){
                tempVec.y = Screen.height - Image_BG.rect.height * factorx;
            }
            Vector2 screenPoint = new Vector2(tempVec.x, tempVec.y);
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transAni, screenPoint, CameraManager.mUICamera, out Vector3 pos)){
                transAni.position = pos;
            }

        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_WorldBossRewardList);
        }
    }


}
