using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;
using System.Text;

namespace Logic
{
    public class TeleInfo
    {
        public string roleName;
        public uint needLvl;
        public uint needTask;
    }
    /// <summary>
    /// 地图进入条件提示
    /// </summary>
    public class UI_MapConditionTips : UIBase
    {
        #region 界面组件
        private Text txtTitle;
        private Button btnClose;
        private Button btnOK;
        private Transform vGroup;
        private Transform protoCell;
        private List<TeleInfo> showTeleInfos;
        #endregion

        #region 系统函数        
        protected override void OnLoaded()
        {
            OnParseComponent();
        }        
        protected override void OnOpen(object arg)
        {
            showTeleInfos = arg as List<TeleInfo>;
        }
        protected override void OnShow()
        {
            RefreshGroup(showTeleInfos);
            txtTitle.text = GetTitleTextString();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnEnterMap, OnEnterMap, toRegister);
        }
        #endregion

        #region 初始化
        private void OnParseComponent()
        {
            txtTitle = transform.Find("Animator/Title").GetComponent<Text>();

            vGroup = transform.Find("Animator/Group");
            protoCell = transform.Find("Animator/Group/Text");

            btnClose = transform.Find("Animator/View_TipsBg02_Small/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(Close);
            btnOK = transform.Find("Animator/Btn_01").GetComponent<Button>();
            btnOK.onClick.AddListener(Close);

        }
        #endregion

        #region 界面显示
        private void RefreshGroup(List<TeleInfo> teleInfos)
        {
            int len = teleInfos.Count;
            for (int i = 0; i < len; i++)
            {
                if (teleInfos[i].needLvl > 0 || teleInfos[i].needTask > 0)
                {
                    Transform clone = GameObject.Instantiate(protoCell);
                    clone.SetParent(vGroup);
                    clone.localPosition = Vector3.zero;
                    clone.localScale = Vector3.one;
                    clone.GetComponent<Text>().text = GetCellString(teleInfos[i]);
                }
            }
            protoCell.gameObject.SetActive(false);
        }
        /// <summary>
        /// 获取单行文本提示内容
        /// </summary>
        private string GetCellString(TeleInfo teleInfo)
        {
            StringBuilder str = new StringBuilder(string.Format(CSVLanguage.Instance.GetConfData(5701).words, teleInfo.roleName));
            if (teleInfo.needLvl > 0)
            {
                str.Append(string.Format(CSVLanguage.Instance.GetConfData(5702).words, teleInfo.needLvl));
            }
            if (teleInfo.needTask > 0)
            {
                CSVTask.Data taskInfo = CSVTask.Instance.GetConfData(teleInfo.needTask);
                if (taskInfo != null)
                {
                    str.Append(string.Format(CSVLanguage.Instance.GetConfData(5703).words, CSVTaskLanguage.Instance.GetConfData(CSVTask.Instance.GetConfData(teleInfo.needTask).taskName).words));
                }
                else
                {
                    Debug.Log("找不到任务名称 任务ID:" + teleInfo.needTask);
                }
            }
            return str.ToString();
        }

        private string GetTitleTextString()
        {
            CSVMapInfo.Data csvMap = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.TeleErrMapId);
            if (csvMap != null)
            {
                return string.Format(CSVLanguage.Instance.GetConfData(5704).words, LanguageHelper.GetTextContent(csvMap.name));
            }
            Debug.Log("找不到map数据，mapId:" + Sys_Map.Instance.TeleErrMapId);
            return "";
        }
        #endregion

        private void Close()
        {
            UIManager.CloseUI(EUIID.UI_MapCondition_Tips);
        }

        private void OnEnterMap()
        {
            Close();
        }
    }
}
