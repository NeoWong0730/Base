using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using System;
using Logic.Core;
using Framework;
using UnityEngine.EventSystems;

namespace Logic
{
    public class UI_PropertyList_Ceil
    {

        private Text txt_name;
        private Text txt_count;
        private Button btn_jump;
        private uint coinId;
        private uint relationId;
        private List<uint> relationLists = new List<uint>();

        public UI_PropertyList_Ceil(uint _id,uint _relationId)
        {
            coinId = _id;
            relationId = _relationId;
        }

        public void Init(Transform transform)
        {
             txt_name=transform.Find("Text_01").GetComponent<Text>();
             txt_count = transform.Find("Text_02").GetComponent<Text>();
             btn_jump = transform.Find("Btn_01_Small").GetComponent<Button>();
           
            btn_jump.onClick.AddListener(OnExChangeButtonClicked);
        }


        public void SetValue()
        {
            CSVItem.Data iData = CSVItem.Instance.GetConfData(coinId);
            txt_name.text = LanguageHelper.GetTextContent(iData.name_id);
            txt_count.text= Sys_Bag.Instance.GetItemCount(coinId).ToString();
            btn_jump.gameObject.SetActive(relationId != 0);
          
            
        }

        private void OnExChangeButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Property_Tips);
            CSVUiRelation.Data rData = CSVUiRelation.Instance.GetConfData(relationId);
            if (rData.functionNum.Count>1)
            {
                MallPrama mprama = new MallPrama();
                mprama.mallId = rData.functionNum[0];
                mprama.shopId = rData.functionNum[1];
                UIManager.OpenUI((int)rData.UIID, false, mprama);

            }else if (rData.UIID==253)
            {
                MallPrama mprama = new MallPrama();
                mprama.mallId = rData.functionNum[0];
                UIManager.OpenUI((int)rData.UIID, false, mprama);
            }else if (rData.UIID == 70)
            {
                PartnerUIParam param = new PartnerUIParam();
                param.tabIndex = 4;
                UIManager.OpenUI((int)rData.UIID, false, param);
            }
            else
            {
                if (coinId==19)
                {
                    if (!Sys_BattlePass.Instance.OpenBattlepassShop())
                    {
                        PromptBoxParameter.Instance.Clear();
                        PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2024602);
                        PromptBoxParameter.Instance.SetConfirm(true, null);
                        PromptBoxParameter.Instance.SetCancel(true, null);
                        UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
                    }
                    return;
                    
                }
                if (rData.relationUiid==0)
                {
                    UIManager.OpenUI((int)rData.UIID, false,(int)rData.functionNum[0]);
                }
                else
                {
                    GetRealUIList(rData);
                    OpenUI();
                }
            }
            

        }
        //ItemUseOpenRelationUI
        private void GetRealUIList(CSVUiRelation.Data cSVUiRelationData)
        {
            relationLists.Add(cSVUiRelationData.id);
            if (cSVUiRelationData.relationUiid == 0)
            {
                return;
            }
            uint relationId = cSVUiRelationData.relationUiid;
            CSVUiRelation.Data cSV = CSVUiRelation.Instance.GetConfData(relationId);
            GetRealUIList(cSV);

        }

        private void OpenUI()
        {
            for (int i = relationLists.Count - 1; i >= 0; --i)
            {
                CSVUiRelation.Data cSVUiRelationData = CSVUiRelation.Instance.GetConfData(relationLists[i]);
                if (!Sys_FunctionOpen.Instance.IsOpen(cSVUiRelationData.functionID, true))
                {
                    continue;
                }
                uint uiId = CSVUiRelation.Instance.GetConfData(relationLists[i]).UIID;
                int funParm = (int)CSVUiRelation.Instance.GetConfData(relationLists[i]).functionNum[0];
                if (funParm != 0)
                {
                    if (uiId == 58) //技能界面
                    {
                        UIManager.OpenUI((EUIID)uiId, true, new List<int>() { funParm });
                    }
                    else
                    {
                        UIManager.OpenUI((EUIID)uiId, true, funParm);
                    }
                }
                else
                {
                    UIManager.OpenUI((EUIID)uiId);
                }
            }
        }

    }

    public class UI_Property_Tips : UIBase
    {
        private struct TipsCeil
        {
            public uint itemId;
            public uint relationId;

            public TipsCeil(uint _item,uint _rId)
            {
                itemId = _item;
                relationId = _rId;
            }
        }

        private GameObject ceilItem;
        private Button Btn_Close;

        private List<TipsCeil> tipsList = new List<TipsCeil>();
        private uint exId;
        private List<uint> tipList = new List<uint>();

        protected override void OnOpen(object arg = null)
        {
            if (arg!=null)
            {
                tipList = (List<uint>)arg;
            }
        }
        protected override void OnLoaded()
        {
            Btn_Close = transform.Find("Animator/ClickClose").GetComponent<Button>();
            ceilItem = transform.Find("Animator/List/Content/Item01").gameObject;
            Btn_Close.onClick.AddListener(OnCloseButtonClicked);
        }

        protected override void OnShow()
        {            
            InitTipsList();
            TipsListShow();
            
        }

        protected override void OnHide()
        {            
            tipsList.Clear();
            FrameworkTool.DestroyChildren(ceilItem.transform.parent.gameObject, ceilItem.transform.name);
        }

        public void InitTipsList()
        {
            tipsList.Clear();
            for (int i=1;i<=CSVMoneyShow.Instance.Count; i++ )
            {
                CSVMoneyShow.Data mData = CSVMoneyShow.Instance.GetConfData((uint)i);
                if (CanThisCoinJoin(mData.itemId) && CheckFunctionIsOpen(mData.functionId))
                {
                    tipsList.Add(new TipsCeil(mData.itemId,mData.relationUiid));
                }
            }
        }
        private bool CanThisCoinJoin(uint _id)
        {
            if (_id==5)
            {
                return true;
            }
            for (int i=0;i<tipList.Count;i++)
            {
                if (_id==tipList[i])
                {
                    return false;
                }
            }
            return true;
        }

        public void TipsListShow()
        {
            FrameworkTool.CreateChildList(ceilItem.transform.parent, tipsList.Count);
            for (int i=0;i<tipsList.Count;i++)
            {
                UI_PropertyList_Ceil tItem = new UI_PropertyList_Ceil(tipsList[i].itemId, tipsList[i].relationId);
                tItem.Init(ceilItem.transform.parent.GetChild(i).transform);
                tItem.SetValue();
            }
        }
        private bool CheckFunctionIsOpen(uint _functionId)
        {
            if (_functionId==0)
            {
                return true;
            }
            if (_functionId ==30701)
            {
                return Sys_Family.Instance.familyData.isInFamily;
            }
            return Sys_FunctionOpen.Instance.IsOpen(_functionId);
        }

        public void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Property_Tips);
        }

    }

}
