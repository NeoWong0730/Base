using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Framework;
using UnityEngine.EventSystems;
using Lib.Core;
using System;
using Packet;

namespace Logic
{
    /*public class UI_Pet_MagicDeedsPreview_Layout 
    {
        public Transform transform;
        public Button closeBtn;
        public GameObject oldskillGo;
        public GameObject newskillGo;
        public Button replaceBtn;
        public Text itemname;
        public GameObject itemGo;
        public Image coinicon;
        public Text coinnum;
        public Button washBtn;
        public Button washRuleBtn;
        public GameObject noskilltips;
        public GameObject newskillsrollview;

        public void Init(Transform transform)
        {
            this.transform = transform;
            closeBtn = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            replaceBtn = transform.Find("Animator/Btn_02").GetComponent<Button>();
            washBtn = transform.Find("Animator/Btn_01").GetComponent<Button>();
            washRuleBtn = transform.Find("Animator/Btn_Rule").GetComponent<Button>();
            oldskillGo = transform.Find("Animator/View_Skill01/Scroll_View_Skill/Grid/Item").gameObject;
            newskillGo = transform.Find("Animator/View_Skill02/Scroll_View_Skill/Grid/Item").gameObject;
            newskillsrollview = transform.Find("Animator/View_Skill02/Scroll_View_Skill").gameObject;
            itemname = transform.Find("Animator/View_Cost01/Text_Name").GetComponent<Text>();
            itemGo = transform.Find("Animator/View_Cost01/PropItem").gameObject;
            coinicon = transform.Find("Animator/View_Cost02/Image_Icon").GetComponent<Image>();
            coinnum = transform.Find("Animator/View_Cost02/Text_Num").GetComponent<Text>();
            noskilltips = transform.Find("Animator/View_Skill02/Text_Tips").gameObject;
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
            replaceBtn.onClick.AddListener(listener.OnreplaceBtnClicked);
            washBtn.onClick.AddListener(listener.OnwashBtnClicked);
            washRuleBtn.onClick.AddListener(listener.OnWashRuleClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
            void OnreplaceBtnClicked();
            void OnwashBtnClicked();
            void OnWashRuleClicked();
        }
    }
    public class UI_Pet_MagicDeedsPreview : UIBase, UI_Pet_MagicDeedsPreview_Layout.IListener
    {
        private UI_Pet_MagicDeedsPreview_Layout layout = new UI_Pet_MagicDeedsPreview_Layout();
        private List<UI_Pet_MagicDeeds_Skill> skillsList = new List<UI_Pet_MagicDeeds_Skill>();
        private List<UI_Pet_MagicDeeds_Skill> newskillsList = new List<UI_Pet_MagicDeeds_Skill>();

        private PropItem propItem;
        private uint deviceid;
        private Animator washanimator;
        private Timer washtimer;

        uint baptizeId;
        uint currencyId;
        uint currencyMaxCount;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            washanimator = transform.Find("Animator").GetComponent<Animator>();
            baptizeId =CSVPetDevices.Instance.GetConfData(deviceid).device_baptize[0];
            currencyId = CSVPetDevices.Instance.GetConfData(deviceid).device_currency[0];
            currencyMaxCount = CSVPetDevices.Instance.GetConfData(deviceid).device_currency[1];
        }

        protected override void OnOpen(object id)
        {
            deviceid = (uint)id;
        }

        protected override void OnShow()
        {
            if (Sys_Pet.Instance.devicesdic[deviceid].SkillBuff[0]==0)
            {
                layout.newskillsrollview.SetActive(false);
                layout.noskilltips.SetActive(true);
            }
            else
            {
                layout.newskillsrollview.SetActive(true);
                layout.noskilltips.SetActive(false);
                DefaultItem(layout.newskillGo, newskillsList);
                AddBuffSkilltem();
            }
            DefaultItem(layout.oldskillGo, skillsList);
            AddSkillItem();
            ItemShow();
        }

        protected override void OnHide()
        {
            washtimer?.Cancel();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnDevicePracticeSkill, OnDevicePracticeSkill, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnDeviceReplaceSkill, OnDeviceReplaceSkill, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnChangeItemCount, OnChangeItemCount, toRegister);
        }

        #region CallBack

        private void OnChangeItemCount()
        {
            int baptizeCountInBag = (int)Sys_Bag.Instance.GetItemCount(baptizeId);
            int currencyCountInBag = (int)Sys_Bag.Instance.GetItemCount(currencyId);
            if (baptizeCountInBag == 0)
                propItem.txtNumber.text = "<color=red>" + baptizeCountInBag.ToString() + "</color>/ 1";
            else
                propItem.txtNumber.text = LanguageHelper.GetTextContent(2009377, baptizeCountInBag,1);
            if (currencyCountInBag == 0)
                layout.coinnum.text = "<color=red>" + currencyCountInBag.ToString() + "</color>/" + currencyMaxCount.ToString();
            else
                layout.coinnum.text = LanguageHelper.GetTextContent(2009377, currencyCountInBag, currencyMaxCount) ;
        }

        private void OnDeviceReplaceSkill()
        {
            DefaultItem(layout.oldskillGo, skillsList);
            layout.newskillsrollview.SetActive(false);
            layout.noskilltips.SetActive(true);
            AddReplaceSkillItem();
        }

        private void OnDevicePracticeSkill()
        {
            DefaultItem(layout.newskillGo, newskillsList);
            layout.newskillsrollview.SetActive(true);
            layout.noskilltips.SetActive(false);
            AddNewSkilltem();
            washanimator.enabled = true;
            washanimator.Play("Wash&Practice_Open", -1,0);
            washtimer?.Cancel();
            washtimer = Timer.Register(1.664f, () =>
            {
                washanimator.enabled = false;
                foreach (var skill in newskillsList)
                {
                    if (skill.Fx_ui_Select03.activeInHierarchy)
                    {
                        skill.Fx_ui_Select03.SetActive(false);
                    }
                    skill.Fx_ui_Select03.SetActive(true);
                }
            }, null, false, false);

        }
        #endregion

        #region Function
        private void AddSkillItem()
        {
            skillsList.Clear();
            for (int i=0;i< Sys_Pet.Instance.devicesdic[deviceid].SkillList.Count;++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.oldskillGo, layout.oldskillGo.transform.parent);
                UI_Pet_MagicDeeds_Skill skill = new UI_Pet_MagicDeeds_Skill(Sys_Pet.Instance.devicesdic[deviceid].SkillList[i]);
                skill.Init(go.transform);
                skill.RefreshItem(Sys_Pet.Instance.devicesdic[deviceid].SkillList[i]);
                skillsList.Add(skill);
            }
            layout.oldskillGo.SetActive(false);
        }

        private void AddReplaceSkillItem()
        {
            skillsList.Clear();
            for (int i = 0; i < Sys_Pet.Instance.replaceSkillList.Count; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.oldskillGo, layout.oldskillGo.transform.parent);
                UI_Pet_MagicDeeds_Skill skill = new UI_Pet_MagicDeeds_Skill( Sys_Pet.Instance.replaceSkillList[i]);
                skill.Init(go.transform);
                skill.RefreshItem(Sys_Pet.Instance.replaceSkillList[i]);
                skillsList.Add(skill);
            }
            layout.oldskillGo.SetActive(false);
        }

        private void AddNewSkilltem()
        {
            newskillsList.Clear();
            for (int i = 0; i < Sys_Pet.Instance.newSkillList.Count; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.newskillGo, layout.newskillGo.transform.parent);
                UI_Pet_MagicDeeds_Skill skill = new UI_Pet_MagicDeeds_Skill(Sys_Pet.Instance.newSkillList[i]);
                skill.Init(go.transform);
                skill.RefreshItem(Sys_Pet.Instance.newSkillList[i]);
                newskillsList.Add(skill);
            }
            layout.newskillGo.SetActive(false);
        }

        private void AddBuffSkilltem()
        {
            newskillsList.Clear();
            for (int i=0;i< Sys_Pet.Instance.devicesdic[deviceid].SkillBuff.Count;++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.newskillGo, layout.newskillGo.transform.parent);
                UI_Pet_MagicDeeds_Skill skill = new UI_Pet_MagicDeeds_Skill( Sys_Pet.Instance.devicesdic[deviceid].SkillBuff[i]);
                skill.Init(go.transform);
                skill.RefreshItem(Sys_Pet.Instance.devicesdic[deviceid].SkillBuff[i]);
                newskillsList.Add(skill);
            }
            layout.newskillGo.SetActive(false);
        }

        private void ItemShow()
        {
            propItem = new PropItem();
            propItem.BindGameObject(layout.itemGo);
            propItem.SetData(new MessageBoxEvt( EUIID.UI_Pet_MagicDeedsPreview, new PropIconLoader.ShowItemData(baptizeId, 
                1, true, false, false, false, false, _bShowCount: true, _bUseClick: true, _bShowBagCount: true)));
            layout.itemname.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(baptizeId).name_id);
            ImageHelper.SetIcon(layout.coinicon, CSVItem.Instance.GetConfData(currencyId).icon_id);
            int count = (int)Sys_Bag.Instance.GetItemCount(currencyId);
            if (count == 0)
                layout.coinnum.text = "<color=red>" + count.ToString() + "</color>/" + currencyMaxCount.ToString();
            else
                layout.coinnum.text =LanguageHelper.GetTextContent(2009377,  count, currencyMaxCount);
        }

        private void DefaultItem(GameObject go,List<UI_Pet_MagicDeeds_Skill>list)
        {
            go.SetActive(true);
            for (int i=0;i< list.Count;++i) { list[i].OnDestroy(); }
            FrameworkTool.DestroyChildren(go.transform.parent.gameObject, go.transform.name);
        }
        #endregion

        #region ButtonClick
        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_MagicDeedsPreview);
        }

        public void OnreplaceBtnClicked()
        {
            if (Sys_Pet.Instance.devicesdic[deviceid].SkillBuff[0] != 0||Sys_Pet.Instance.newSkillList.Count!=0)
            {
                Sys_Hint.Instance.PushEffectInNextFight();
                Sys_Pet.Instance.OnDeviceSkillReplaceReq(deviceid);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009458));
            }
        }

        public void OnwashBtnClicked()
        {
            if (Sys_Bag.Instance.GetItemCount(baptizeId) > 0 )            
            {
                if (Sys_Bag.Instance.GetItemCount(currencyId) >=currencyMaxCount)
                {
                    Sys_Hint.Instance.PushEffectInNextFight();
                    Sys_Pet.Instance.OnDeviceSkillTrainingReq(deviceid);    
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009454));
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4052));
            }
        }

        public void OnWashRuleClicked()
        {
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2009497) });
        }
        #endregion
    }
    */
}
