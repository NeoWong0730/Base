
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_MagicOpen
    {
        public Transform transform;        
        public Button functionBtn;
        private Text messageText;
        private Image iconImage;
        private GameObject fxGo;
        private uint chapterId;
        
        public void Init(Transform transform)
        {
            this.transform = transform;
            functionBtn = transform.Find("Image_Black").GetComponent<Button>();
            functionBtn.onClick.AddListener(FunctionBtnClicked);
            messageText = transform.Find("Text_Message").GetComponent<Text>();
            iconImage = transform.Find("Image/Image_Icon").GetComponent<Image>();
            fxGo = transform.Find("Image_Black/Fx_ui_TaskOrTeam_lg").gameObject;
        }

        private List<uint> ids = new List<uint>();
        public void SetSys()
        {
            ids = Sys_FuncPreview.Instance.GetUnReadedIds(true);
            // ListHelper.Print(this.ids);
            
            chapterId = 0;
            if (ids.Count > 0) {
                chapterId = ids[0];
                var csv = CSVFunForeshow.Instance.GetConfData(chapterId);
                bool levelValid = Sys_Role.Instance.Role.Level >= csv.FunctionLv;
                if (!(this.fxGo is null)) {
                    this.fxGo.SetActive(levelValid);
                }

                string funcName = LanguageHelper.GetTextContent(csv.words);
                string content = "";
                if (csv.FunctionLv == 0) {
                    if (levelValid) {
                        content = LanguageHelper.GetTextContent(12412, funcName);
                    }
                    else {
                        content = LanguageHelper.GetTextContent(12413, funcName);
                    } 
                }
                else {
                    if (levelValid) {
                        content = LanguageHelper.GetTextContent(12410, funcName, Sys_Role.Instance.Role.Level.ToString(), csv.FunctionLv.ToString());
                    }
                    else {
                        content = LanguageHelper.GetTextContent(12411, funcName, Sys_Role.Instance.Role.Level.ToString(), csv.FunctionLv.ToString());
                    }
                }

                TextHelper.SetText(messageText, content);
                ImageHelper.SetIcon(iconImage, csv.SysIcon);
            }
            
            transform.gameObject.SetActive(ids.Count > 0);
        }
        
        private void FunctionBtnClicked()
        {
            if (this.chapterId != 0) {
                UIManager.OpenUI(EUIID.UI_FunctionPreview, false, this.chapterId);
            }
        }
    }
}
