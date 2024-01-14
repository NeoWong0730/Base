using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{
    public class UI_Pet_MountScreem : UIBase
    {
        private Button closeBtn;
        /// <summary> 删选列表 </summary>
        private List<Toggle> screemList = new List<Toggle>();
        private ulong screemBit;
        protected override void OnLoaded()
        {
            closeBtn = transform.Find("Blank").GetComponent<Button>();
            closeBtn.onClick.AddListener(CloseBtnClicked);
            var _toggles = transform.Find("Animator/Image_BG/ToogleGroup").GetComponentsInChildren<Toggle>(true);
            screemList.AddRange(_toggles);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }

        protected override void OnShow()
        {
            CreateItemList();
        }

        /// <summary>
        /// 创建模版列表
        /// </summary>
        private void CreateItemList()
        {
            var petParam = CSVPetNewParam.Instance.GetConfData(62);
            if(null != petParam)
            {
                List<List<uint>> data = ReadHelper.ReadArray2_ReadUInt(petParam.str_value, '|', '&');
                for (int i = 0; i < data.Count; i++)
                {
                    screemList[i].SetIsOnWithoutNotify(Sys_Pet.Instance.GetScreemTypeState((uint)i));
                }
            }
        }

        private void OnCheckBitIsChange()
        {
            ulong state = 0;
            for (int i = 0; i < screemList.Count; i++)
            {
                var bit = Convert.ToUInt32(screemList[i].name);
                if (screemList[i].isOn)
                {
                    Sys_Pet.Instance.SetBitvalue(bit, ref state);
                }
                else
                {
                    Sys_Pet.Instance.SetBitValueZeo(bit, ref state);
                }
            }
            if (state != Sys_Pet.Instance.GetScreemBit())
            {
                Sys_Pet.Instance.SaveMountScreeDB(state);
                Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnMountScreemChange);
            }
        }

        public void CloseBtnClicked()
        {
            OnCheckBitIsChange();
            UIManager.CloseUI(EUIID.UI_Pet_MountScreem);
        }
    }
}