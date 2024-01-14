using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
namespace Logic
{

    public class RuneCostItem : UI_CostItem
    {
        public void Refresh(ItemIdCount idCount, ItemCostLackType lackType = ItemCostLackType.LeftLackRG)
        {
            this.idCount = idCount;

            if (idCount != null)
            {
                if (idCount.CSV != null)
                {
                    ImageHelper.SetIcon(icon, idCount.CSV.icon_id);
                }

                uint styleid = 65u;
                if (lackType == ItemCostLackType.Normal)
                {
                    TextHelper.SetText(content, idCount.count.ToString(), LanguageHelper.GetTextStyle(styleid));
                }
                else if (lackType == ItemCostLackType.LeftLackRG)
                {
                    styleid = idCount.Enough ? 65u : 22u;
                    TextHelper.SetText(content, idCount.count.ToString(), LanguageHelper.GetTextStyle(styleid));
                }
            }
        }
    }

    public class UI_RuneComeposeCard
    {
        private AsyncOperationHandle<GameObject> requestRef;
        public GameObject gameObject;
        public CSVRuneSynthetise.Data composeData;
        private GameObject resGo;
        private Transform resParent;
        private Text composeCardNameText;
        private Button funcBtn;
        private RuneCostItem costItem = new RuneCostItem();
        private int partnerSort = 0;

        public void Init(Transform transform)
        {
            gameObject = transform.gameObject;
            resParent = transform.Find("Image_Icon");
            composeCardNameText = transform.Find("Image_Tips/Text").GetComponent<Text>();
            funcBtn = transform.Find("Btn_Change").GetComponent<Button>();
            funcBtn.onClick.RemoveAllListeners();
            funcBtn.onClick.AddListener(FunctionBtnClicked);
            costItem.SetGameObject(transform.Find("Btn_Change/Image_Icon").gameObject);
        }

        private void FunctionBtnClicked()
        {
            if (null != costItem && costItem.idCount.Enough)
            {
                if(Sys_Partner.Instance.composeTimeCd + 1 < Sys_Time.Instance.GetServerTime())
                    UIManager.OpenUI(EUIID.UI_Exchange_Rune, false, composeData);
            }                
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2006141));
            }                
        }

        public void SetData(params object[] arg)
        {
            if(null != arg && arg.Length >=2)
            {
                composeData = arg[0] as CSVRuneSynthetise.Data;
                partnerSort = Convert.ToInt32(arg[1]);
            }

            if (null != composeData)
            {
                TextHelper.SetText(composeCardNameText, LanguageHelper.GetTextContent(2006140, composeData.id.ToString()));
                if (null != composeData.synthetise_expend && composeData.synthetise_expend.Count >= 2)
                {
                    costItem?.Refresh(new ItemIdCount(composeData.synthetise_expend[0], (long)composeData.synthetise_expend[1]));
                    ButtonState(costItem.idCount.Enough);
                }
                LoadNpcIconAssetAsyn(composeData.bg_path);
            }
            gameObject.SetActive(true);
        }

        public void Reset(int sort)
        {
            partnerSort = sort;
            if (null != composeData)
            {
                if (null != composeData.synthetise_expend && composeData.synthetise_expend.Count >= 2)
                {
                    costItem?.Refresh(new ItemIdCount(composeData.synthetise_expend[0], (long)composeData.synthetise_expend[1]));
                    ButtonState(costItem.idCount.Enough);
                }
            }
            ResetSort();
        }

        private void ResetSort()
        {
            if (null != resGo)
            {
                UILocalSorting[] layoutControlls = resGo.GetComponentsInChildren<UILocalSorting>();
                for (int i = 0; i < layoutControlls.Length; i++)
                {
                    layoutControlls[i].SetRootSorting(partnerSort);
                }
            }
        }

        private void ButtonState(bool isEnough)
        {
            ImageHelper.SetImageGray(funcBtn.GetComponent<Image>(), !isEnough, false);
        }

        private void LoadNpcIconAssetAsyn(string path)
        {
            AddressablesUtil.InstantiateAsync(ref requestRef, path, MHandle_Completed);
        }

        private void MHandle_Completed(AsyncOperationHandle<GameObject> handle)
        {
            resGo = handle.Result;
            if (null != resGo)
            {
                ResetSort();
                resGo.transform.SetParent(resParent);
                RectTransform rectTransform = resGo.transform as RectTransform;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }
        }

    }

    public class UI_PartnerRuneChange: UIParseCommon
    {
        bool init = false;
        private GameObject composeCardGo;
        private Transform cardParent;
        List<UI_RuneComeposeCard> cardList = new List<UI_RuneComeposeCard>();
        private int partnerSort = 0;

        protected override void Parse()
        {
            cardParent = transform.Find("ScrollView/TabList");
            composeCardGo = transform.Find("Item01").gameObject;
        }

        public override void Show()
        {
            base.Show();

            ProcessEventsForEnable(false);
            ProcessEventsForEnable(true);
            Init();
        }

        public override void Hide()
        {
            base.Hide();
            ProcessEventsForEnable(false);
            init = false;
            cardList.Clear();
        }

        private void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnRuneComposeCallBack, OnRuneComposeCallBack, toRegister);
        }

        private void OnRuneComposeCallBack()
        {
            Init();
        }

        public void ShowData(int sort)
        {
            partnerSort = sort;
            Init();
        }

        private void Init()
        {
            if (!init)
            {
                init = true;
                FrameworkTool.DestroyChildren(cardParent.gameObject);

                var dataList = CSVRuneSynthetise.Instance.GetAll();
                for (int i = 0, len = dataList.Count; i < len; i++)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(composeCardGo, cardParent);
                    UI_RuneComeposeCard cell = new UI_RuneComeposeCard();
                    cell.Init(go.transform);
                    cell.SetData(dataList[i], partnerSort);
                    cardList.Add(cell);
                }
            }
            else
            {
                for (int i = 0; i < cardList.Count; i++)
                {
                    cardList[i].Reset(partnerSort);
                }
            }
        }
    }
}
