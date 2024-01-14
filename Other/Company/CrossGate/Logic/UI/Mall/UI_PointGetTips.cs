using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using System;

namespace Logic
{
    public class UI_PointGetTips_Layout
    {
        private Transform transform;
        public GameObject itemGo;
        public Button closeBtn;

        public void Init(Transform transform)
        {
            this.transform = transform;
            itemGo = transform.Find("Animator/View_Tips/Info/Item").gameObject;
            closeBtn = transform.Find("Blank").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnCloseBtnClicked);
        }

        public interface IListener
        {
            void OnCloseBtnClicked();
        }
    }

    public class UI_PointGetTips : UIBase, UI_PointGetTips_Layout.IListener
    {

        private UI_PointGetTips_Layout layout = new UI_PointGetTips_Layout();
        private CSVAidPoint.Data csvAidPointData;
        private CSVCaptainPoint.Data csvCaptainPointData;
        private CSVAidValue.Data csvAidValueData;
        private CSVBattleType.Data csvBattleTypeData;
        private CSVReturnArdent.Data csvLovePointData;
        private EPointShopType curPointType;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg)
        {

          curPointType = (EPointShopType)arg;

        }

        protected override void OnShow()
        {
            if(curPointType== EPointShopType.Captain)
            {
                AddCaptainList();
            }
            else if(curPointType == EPointShopType.Aid)
            {
                AddAidList();
            }
            else if (curPointType == EPointShopType.LovePoint)
            {
                AddLovePointList();

            }
        }

        protected override void OnHide()
        {
            DefaultItem();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnDailyPointUpdate, UpdateDailyPointInfo, toRegister);
        }

        #region Function
        private void AddAidList()
        {
            int count = CSVAidPoint.Instance.Count;
            FrameworkTool.CreateChildList(layout.itemGo.transform.parent, count);
            for (int i = 0; i < count; i++)
            {
                csvAidPointData = CSVAidPoint.Instance.GetConfData((uint)i + 1);
                uint battleTypeId = csvAidPointData.BattleType;
                Transform child = layout.itemGo.transform.parent.GetChild(i);
                child.Find("Name").GetComponent<Text>().text =LanguageHelper.GetTextContent(csvAidPointData.Name);
                Text number = child.Find("Name/Value").GetComponent<Text>();
                if (battleTypeId == 0)
                {
                    if (Sys_Attr.Instance.aidPointDic.TryGetValue(csvAidPointData.Teambattle_type, out uint info))
                    {
                        number.text = LanguageHelper.GetTextContent(2009835, info.ToString());
                    }
                    else
                    {
                        number.text = LanguageHelper.GetTextContent(2009835, "0");
                    }
                }
                else
                {
                    csvAidValueData = CSVAidValue.Instance.GetConfData(battleTypeId);
                    if (Sys_Attr.Instance.aidPointDic.TryGetValue(csvAidPointData.Teambattle_type, out uint info))
                    {
                        number.text = LanguageHelper.GetTextContent(2009836, info.ToString(), csvAidValueData.AidUpperLimit.ToString());
                    }
                    else
                    {
                        number.text = LanguageHelper.GetTextContent(2009836, "0", csvAidValueData.AidUpperLimit.ToString());
                    }
                }            
            }
        }

        private void AddCaptainList()
        {
            int count = CSVCaptainPoint.Instance.Count;
            FrameworkTool.CreateChildList(layout.itemGo.transform.parent, count);
            for (int i = 0; i < count; i++)
            {
                csvCaptainPointData = CSVCaptainPoint.Instance.GetConfData((uint)i + 1);
                uint  teamBattleTypeId = csvCaptainPointData.BattleType;
                csvBattleTypeData = CSVBattleType.Instance.GetConfData(csvCaptainPointData.BattleType);
                Transform child = layout.itemGo.transform.parent.GetChild(i);
                child.Find("Name").GetComponent<Text>().text = LanguageHelper.GetTextContent(csvCaptainPointData.Name);
                Text number = child.Find("Name/Value").GetComponent<Text>();
                if (csvBattleTypeData.CaptainUpperLimit == 0)
                {
                    if (Sys_Attr.Instance.captainPointDic.TryGetValue(csvCaptainPointData.Teambattle_type, out uint info))
                    {
                        number.text = LanguageHelper.GetTextContent(2009835, info.ToString());
                    }
                    else
                    {
                        number.text = LanguageHelper.GetTextContent(2009835, "0");
                    }
                }
                else
                {
                    if (Sys_Attr.Instance.captainPointDic.TryGetValue(csvCaptainPointData.Teambattle_type, out uint info))
                    {
                        number.text = LanguageHelper.GetTextContent(2009836, info.ToString(), csvBattleTypeData.CaptainUpperLimit.ToString());
                    }
                    else
                    {
                        number.text = LanguageHelper.GetTextContent(2009836, "0", csvBattleTypeData.CaptainUpperLimit.ToString());
                    }
                }
            }
        }

        private void AddLovePointList()
        {
            int count = CSVReturnArdent.Instance.Count;
            FrameworkTool.CreateChildList(layout.itemGo.transform.parent, count);
            for (int i = 0; i < count; i++)
            {
                csvLovePointData = CSVReturnArdent.Instance.GetConfData((uint)i + 1);
                uint TypeId = csvLovePointData.enthusiasmLimit;
                Transform child = layout.itemGo.transform.parent.GetChild(i);
                child.Find("Name").GetComponent<Text>().text = LanguageHelper.GetTextContent(csvLovePointData.text);
                Text number = child.Find("Name/Value").GetComponent<Text>();
                if (TypeId == 0)
                {
                    if (Sys_BackAssist.Instance.LovePointDictionary.TryGetValue(csvLovePointData.id, out uint info))
                    {
                        number.text = LanguageHelper.GetTextContent(2009835, info.ToString());
                    }
                    else
                    {
                        number.text = LanguageHelper.GetTextContent(2009835, "0");
                    }
                }
                else
                {
                    if (Sys_BackAssist.Instance.LovePointDictionary.TryGetValue(csvLovePointData.id, out uint info))
                    {
                        number.text = LanguageHelper.GetTextContent(2009836, info.ToString(), csvLovePointData.enthusiasmLimit.ToString());
                    }
                    else
                    {
                        number.text = LanguageHelper.GetTextContent(2009836, "0", csvLovePointData.enthusiasmLimit.ToString());
                    }
                }
            }
        }
        private void DefaultItem()
        {
            FrameworkTool.DestroyChildren(layout.itemGo.transform.parent.gameObject, layout.itemGo.transform.name);
        }
        #endregion

        #region 响应事件

        private void UpdateDailyPointInfo()
        {
            DefaultItem();
            if (curPointType == EPointShopType.Captain)
            {
                AddCaptainList();
            }
            else if (curPointType == EPointShopType.Aid)
            {
                AddAidList();
            }
            else if(curPointType == EPointShopType.LovePoint)
            {
                AddLovePointList();
            }
        }

        public void OnCloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_PointGetTips);
        }
        #endregion
    }

}
