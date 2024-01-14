using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;
using System;
using UnityEngine;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Energyspar_Advanced_Layout
    {
        public Transform transform;        
        public Button closeButton; 
        public Button advancedBtn;
        public Transform leftStarTran;
        public Transform rightStarTran;
        public Text lightDescText;
        public Text darkDescText;
        public UI_Energyspar_LevelInfo levelInfo;
        public GameObject starGo;
        public GameObject itemParentGo;

        public ItemCost onceCoseItem = new ItemCost();
        public void Init(Transform transform)
        {
            this.transform = transform;
            closeButton = transform.Find("View_TipsBg01_Largest/Btn_Close").GetComponent<Button>();
            advancedBtn = transform.Find("Animator/Btn_Advanced").GetComponent<Button>();
            leftStarTran = transform.Find("Animator/StarGroup_Left");
            rightStarTran = transform.Find("Animator/StarGroup_Right");
            lightDescText = transform.Find("Animator/Attribute_Light/Value").GetComponent<Text>();
            darkDescText = transform.Find("Animator/Attribute_Dark/Value").GetComponent<Text>();
            starGo = transform.Find("Animator/Star_Dark").gameObject;
            itemParentGo = transform.Find("Animator/Grid").gameObject;
            onceCoseItem.SetGameObject(transform.Find("Animator/Text_Cost").gameObject);
            levelInfo = new UI_Energyspar_LevelInfo();
            levelInfo.Init(transform.Find("Animator/View_Skill"));
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OncloseBtnClicked);
            advancedBtn.onClick.AddListener(listener.OnAdvancedBtnClicked);            
        }

        public interface IListener
        {
            void OncloseBtnClicked();
            void OnAdvancedBtnClicked();
        }
    }

    public class UI_Energyspar_Advanced : UIBase, UI_Energyspar_Advanced_Layout.IListener
    {
        private UI_Energyspar_Advanced_Layout layout = new UI_Energyspar_Advanced_Layout();
        private CSVStone.Data configData;
        private bool isEnough;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg)
        {
            configData = arg as CSVStone.Data;
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_StoneSkill.Instance.eventEmitter.Handle<uint>(Sys_StoneSkill.EEvents.EnergysparSpecilEvent, EnergysparSpecilEvent, toRegister);

        }

        private void EnergysparSpecilEvent(uint id)
        {
            CloseSelf();
        }

        private void UpdateInfo()
        {          
            if (null != configData)
            {
                isEnough = true;
                layout.levelInfo.UpdateInfo(configData);
                FrameworkTool.DestroyChildren(layout.leftStarTran.gameObject);
                FrameworkTool.DestroyChildren(layout.rightStarTran.gameObject);
                StoneSkillData severData = Sys_StoneSkill.Instance.GetServerDataById(configData.id);
                if(null != severData)
                {                    
                    uint currentState = severData.powerStoneUnit.Stage;
                    for (int i = 0; i < configData.max_stage; i++)
                    {
                        bool isLight = i < currentState;             
                        GameObject go = GameObject.Instantiate<GameObject>(layout.starGo, layout.leftStarTran);
                        Small_Star small_Star = new Small_Star();
                        small_Star.BindGameobject(go.transform);
                        small_Star.SetState(isLight);
                    }

                    for (int i = 0; i < configData.max_stage; i++)
                    {
                        bool isLight = i <= currentState;
                        GameObject go = GameObject.Instantiate<GameObject>(layout.starGo, layout.rightStarTran);
                        Small_Star small_Star = new Small_Star();
                        small_Star.BindGameobject(go.transform);
                        small_Star.SetState(isLight);
                    }

                    CSVStoneStage.Data stageData = CSVStoneStage.Instance.GetConfData(configData.id * 1000 + severData.powerStoneUnit.Stage + 1);
                    if(null != stageData)
                    {
                        TextHelper.SetText(layout.lightDescText, LanguageHelper.GetTextContent(stageData.desc_light));
                        TextHelper.SetText(layout.darkDescText, LanguageHelper.GetTextContent(stageData.desc_dark));
                        FrameworkTool.DestroyChildren(layout.itemParentGo);
                        bool showCost = false;
                        for (int i = 0; i < stageData.cost_item.Count; i++)
                        {
                            if (null != stageData.cost_item[i] && stageData.cost_item[i].Count >= 2)
                            {
                                if (Sys_Bag.Instance.GetItemCount(stageData.cost_item[i][0]) < stageData.cost_item[i][1] && isEnough)
                                {
                                    isEnough = false;
                                }                                
                                if (stageData.cost_item[i][0] < 500)
                                {
                                    showCost = true;
                                    layout.onceCoseItem?.Refresh(new ItemIdCount(stageData.cost_item[i][0], stageData.cost_item[i][1]));                                    
                                }
                                else
                                {                                    
                                    PropIconLoader.GetAsset(new PropIconLoader.ShowItemData(stageData.cost_item[i][0], stageData.cost_item[i][1],
                                      true, false, false, false, false, _bShowCount: true, _bShowBagCount: true), layout.itemParentGo.transform, EUIID.UI_Energyspar_Advanced);
                                }
                               
                            }
                        }
                        layout.onceCoseItem?.gameObject.SetActive(showCost);
                    }

                }
                else
                {
                    DebugUtil.LogErrorFormat("StoneSkillData 服务器数据 无法获取 Id = {0}", configData.id);
                }
               
            }            
        }

        protected override void OnHide()
        {

        }

        public void OncloseBtnClicked()
        {
            CloseSelf();
        }

        public void OnAdvancedBtnClicked()
        {
            if(!isEnough)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021063));
                return;
            }                
            if(null != configData)
            {
                Sys_StoneSkill.Instance.OnPowerStoneStageUpReq(configData.id);
                OncloseBtnClicked();
            }
            else
            {
                DebugUtil.LogError("configData 为空");
            }
            
        }
    }
}

