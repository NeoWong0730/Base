using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using Packet;
using System;

namespace Logic
{
    public class UI_Pet_Develop_Layout
    {
        public Transform transform;
        public GameObject expItemGo;
        public GameObject loyatyItemGo;

        public GameObject viewexp;
        public GameObject viewLoyalty;
        public GameObject basicDevelopePage;
        public GameObject resistanceStrengthenPage;

        public Button onceBtn;
        public Button tenBtn;

        //Exp develope
        public Text explevel;
        public Text explevelup;
        public Slider explevelslider;
        public Text expperadd;

        //Loyalty develope
        public Slider loyaltySlider;
        public Text loyaltyNum;
        public Text loyaltyUpPer;

        //Resistance Strengthen
        public GameObject resattrGo;
        public Text leftPoint;
        public Text strengthenLv;
        public Text strengthenNum;
        public Text useBtnText;
        public Slider strengthenSlider;
        public Button useItemBtn;
        public Button resetBtn;
        public Button addPointBtn;

        public Toggle basicToggle;
        public Toggle resistanceToggle;
        public Transform planView;
        public Button useBtn;

        public void Init(Transform transform)
        {
            this.transform = transform;

            expItemGo = transform.Find("Page01/View_Train/Scroll_View/Content/Basic_Item/Item_Grid/PropItem").gameObject;
            loyatyItemGo = transform.Find("Page01/View_Train/Scroll_View/Content/Attr_Item/Item_Grid/PropItem").gameObject;
            viewexp = transform.Find("Page01/View_Train/View_Exp").gameObject;
            viewLoyalty = transform.Find("Page01/View_Train/View_Loyalty").gameObject;

            basicDevelopePage = transform.Find("Page01").gameObject;
            resistanceStrengthenPage = transform.Find("Page02").gameObject;

            onceBtn = transform.Find("Page01/View_Train/Btn_One").GetComponent<Button>();
            tenBtn = transform.Find("Page01/View_Train/Btn_Ten").GetComponent<Button>();

            explevel = transform.Find("Page01/View_Train/View_Exp/Text_Level/Text_Num").GetComponent<Text>();
            explevelup = transform.Find("Page01/View_Train/View_Exp/Text_Exp/Text_Percent").GetComponent<Text>();
            expperadd = transform.Find("Page01/View_Train/View_Exp/Text_Tips/Text_Num").GetComponent<Text>();
            explevelslider = transform.Find("Page01/View_Train/View_Exp/Text_Exp/Slider_Exp").GetComponent<Slider>();

            loyaltySlider = transform.Find("Page01/View_Train/View_Loyalty/Text_Exp/Slider_Exp").GetComponent<Slider>();
            loyaltyNum = transform.Find("Page01/View_Train/View_Loyalty/Text_Exp/Text_Percent").GetComponent<Text>();
            loyaltyUpPer = transform.Find("Page01/View_Train/View_Loyalty/Text_Tips/Text_Num").GetComponent<Text>();

            resattrGo = transform.Find("Page02/Scroll_View/Attr_Grid/Attr01 (1)").gameObject;
            leftPoint = transform.Find("Page02/Text_Reset/Text_Num").GetComponent<Text>();
            useBtnText = transform.Find("Page02/View_Btn/Button_Use/Text_01").GetComponent<Text>();

            strengthenLv = transform.Find("Page02/Strong/Text_Strong/Text_Num").GetComponent<Text>();
            strengthenNum = transform.Find("Page02/Strong/Text_Percent").GetComponent<Text>();
            strengthenSlider = transform.Find("Page02/Strong/Slider_Exp").GetComponent<Slider>();

            useItemBtn = transform.Find("Page02/Strong/Button_Add").GetComponent<Button>();
            resetBtn = transform.Find("Page02/View_Btn/Btn_01").GetComponent<Button>();
            addPointBtn = transform.Find("Page02/View_Btn/Button_OK").GetComponent<Button>();

            basicToggle = transform.Find("Menu/ListItem").GetComponent<Toggle>();
            resistanceToggle = transform.Find("Menu/ListItem (1)").GetComponent<Toggle>();
            planView = transform.Find("Page02/Label_Scroll01");
            useBtn = transform.Find("Page02/View_Btn/Button_Use").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            onceBtn.onClick.AddListener(listener.OnoncebtnClicked);
            tenBtn.onClick.AddListener(listener.OntenbtnClicked);
            useItemBtn.onClick.AddListener(listener.OnuseItemBtnClicked);
            resetBtn.onClick.AddListener(listener.OnresetBtnClicked);
            addPointBtn.onClick.AddListener(listener.OnaddPointBtnClicked);
            useBtn.onClick.AddListener(listener.OnuseBtnClicked);
            basicToggle.onValueChanged.AddListener(listener.OnbasicToggleValueChanged);
            resistanceToggle.onValueChanged.AddListener(listener.OnresistanceToggleValueChanged);
        }

        public interface IListener
        {
            void OnoncebtnClicked();
            void OntenbtnClicked();
            void OnuseItemBtnClicked();
            void OnresetBtnClicked();
            void OnaddPointBtnClicked();
            void OnbasicToggleValueChanged(bool isOn);
            void OnresistanceToggleValueChanged(bool isOn);
            void OnuseBtnClicked();
        }
    }

    public class UI_Pet_Develop_Prop : UIComponent
    {
        private Button iconbtn;
        public Text count;
        private Text name;
        private GameObject selected;
        public GameObject fxonce;
        public GameObject fxten;
        public uint itemid;
        public PropIconLoader.ShowItemData showItemData;

        public UI_Pet_Develop_Prop(uint _itemid, PropIconLoader.ShowItemData _showItemData) : base()
        {
            itemid = _itemid;
            showItemData = _showItemData;
        }

        protected override void Loaded()
        {
            base.Loaded();
            count = transform.Find("Text_Number").GetComponent<Text>();
            name = transform.Find("Text_Name").GetComponent<Text>();
            iconbtn = transform.Find("Btn_Item").GetComponent<Button>();
            iconbtn.onClick.AddListener(OnIconbtnClicked);
            selected = transform.Find("Image_Select").gameObject;
            fxonce = transform.Find("Btn_Item/Image_BG/Fx_ui_item01").gameObject;
            fxten = transform.Find("Btn_Item/Image_BG/Fx_ui_item10").gameObject;
        }

        public override void Show()
        {
            base.Show();
        }

        private void OnIconbtnClicked()
        {
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnSelectItem, itemid);
        }

        public void SetSelectItem(uint selectitemId)
        {
            if (selectitemId == itemid)
            {
                selected.SetActive(true);
            }
            else
            {
                selected.SetActive(false);
            }
        }
    }

    public class UI_Pet_Resistance_Attr 
    {
        private Transform transform;
        private uint id;
        private uint perAttrNum;
        private uint addPointMax;
        private uint resistanceAddPoin;
        private uint totalAddPoint;
        private uint usedAdd;
        private uint freeAddPoint;
        private int index;
        private ClientPet clientPet;
        private CSVAttr.Data csvAttrData;
        private CSVPetNewEnhance.Data csvPetNewEnhanceData;

        private Text attrname;
        private Text addPoint;
        private Text attrNum;
        private Text attrAddNum;
        private Button addbtn;
        private Button subbtn;

        public UI_Pet_Resistance_Attr(uint _id, ClientPet _clientPet) : base()
        {
            id = _id;
            clientPet = _clientPet;

            csvAttrData = CSVAttr.Instance.GetConfData(id);
            CSVPetNewEnhance.Instance.TryGetValue(clientPet.petUnit.EnhancePlansData.Enhancelvl,out csvPetNewEnhanceData);
            addPointMax = GetAttrMaxPoint(id, out perAttrNum);
        }

        public  void Init(Transform transform)
        {
           this.transform = transform;
            attrname = transform.Find("Text_Attr").GetComponent<Text>();
            addPoint = transform.Find("Text_Addpoint").GetComponent<Text>();
            attrNum = transform.Find("Text_Num").GetComponent<Text>();
            attrAddNum = transform.Find("Text_Num/Text_AddNum").GetComponent<Text>();
            addbtn = transform.Find("Btn_Add").GetComponent<Button>();
            addbtn.onClick.AddListener(OnaddbtnClick);
            subbtn = transform.Find("Btn_Min").GetComponent<Button>();
            subbtn.onClick.AddListener(OnsubbtnClick);
        }

        private void OnsubbtnClick()
        {
            if (clientPet.petUnit.SimpleInfo.ExpiredTick > 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000654));
                return;
            }
            if (resistanceAddPoin == 0)
            {
              //  Sys_Hint.Instance.PushContent_Normal("不可减少");
            }
            else
            {
                Sys_Pet.Instance.resistanceAddPointDic[id]--;
                resistanceAddPoin = Sys_Pet.Instance.resistanceAddPointDic[id];
                Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnChangeResPoint);
            }
        }

        private void OnaddbtnClick()
        {
            if (clientPet.petUnit.SimpleInfo.ExpiredTick > 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000654));
                return;
            }
            if (freeAddPoint > totalAddPoint)
            {
                if (resistanceAddPoin + usedAdd < addPointMax)
                {
                    Sys_Pet.Instance.resistanceAddPointDic[id]++;
                    resistanceAddPoin = Sys_Pet.Instance.resistanceAddPointDic[id];
                    Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnChangeResPoint);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10839));
                }

            }
            else
            {
                //Sys_Hint.Instance.PushContent_Normal("已达到最大加点");
            }
        }

        public void RefreshItem(uint id,uint used, int _index,uint totalAdd=0)
        {
            index = _index;
            uint total = clientPet.petUnit.EnhancePlansData.TotalPoint;
            uint use = clientPet.petUnit.EnhancePlansData.Plans[index].UsePoint;
            freeAddPoint = total - use;
            resistanceAddPoin = Sys_Pet.Instance.resistanceAddPointDic[id];
            attrname.text = LanguageHelper.GetTextContent(csvAttrData.name);
            addPoint.text = resistanceAddPoin.ToString();
            totalAddPoint = totalAdd;
            usedAdd = used;
            if (resistanceAddPoin == 0)
            {
                attrAddNum.text = string.Empty;
            }
            else
            {
                attrAddNum.text = LanguageHelper.GetTextContent(10881, (resistanceAddPoin * perAttrNum / 100f).ToString()); 
            }
            subbtn.enabled = resistanceAddPoin != 0;
            addbtn.enabled = freeAddPoint > totalAddPoint;
            attrNum.text = LanguageHelper.GetTextContent(10882, "0");
            for (int i =0;i< clientPet.petUnit.EnhancePlansData.Plans[index].AllocPointId.Count; ++i)
            {
                if (clientPet.petUnit.EnhancePlansData.Plans[index].AllocPointId[i] == id)
                {
                    attrNum.text = LanguageHelper.GetTextContent(10882, (clientPet.petUnit.EnhancePlansData.Plans[index].AllocPointValue[i] * perAttrNum / 100f).ToString()); 
                }
            }
        }

        private uint GetAttrMaxPoint(uint attrId, out uint perAttrNum)
        {
            perAttrNum = 0;
            if (csvPetNewEnhanceData == null)
                return 0;
            for (int i = 0; i < csvPetNewEnhanceData.attr.Count; ++i)
            {
                if (csvPetNewEnhanceData.attr[i][0] == attrId)
                {
                    perAttrNum = csvPetNewEnhanceData.attr[i][1];
                    return csvPetNewEnhanceData.attr[i][2];
                }
            }
            return 0;
        }
    }

    public class UI_Pet_Develop : UI_Pet_Develop_Layout.IListener
    {
        private UI_Pet_Develop_Layout layout = new UI_Pet_Develop_Layout();
        private Dictionary<uint, UI_Pet_Develop_Prop> expDic = new Dictionary<uint, UI_Pet_Develop_Prop>();
        private Dictionary<uint, UI_Pet_Develop_Prop> loyatyDic = new Dictionary<uint, UI_Pet_Develop_Prop>();
        private Dictionary<uint, UI_Pet_Resistance_Attr> resistanceAttrDic = new Dictionary<uint, UI_Pet_Resistance_Attr>();
        private UI_PetPoint_Plan petPointPlan = new UI_PetPoint_Plan();
        private Transform transform;
        private PropItem propItem;
        private ClientPet currentChoosePet;
        private PetUnit petUnit;
        private GameObject go;

        public uint currentItemId;
        private uint defaultItemid;
        private float maxExp;
        private uint totalAddPoint;
        private int curIndex=-1;

        public void Init(Transform transform)
        {
            this.transform = transform;
            layout.Init(transform);
            petPointPlan.Init(layout.planView);
            layout.RegisterEvents(this);
            maxExp = float.Parse(CSVParam.Instance.GetConfData(561).str_value);
        }

        uint pageShowTime;
        public void Show()
        {
            transform.gameObject.SetActive(true);
            layout.basicDevelopePage.SetActive(true);
            layout.resistanceStrengthenPage.SetActive(false);
            layout.basicToggle.isOn = true;
            UIManager.HitPointShow(EUIID.UI_Pet_Message, EPetMessageViewState.Foster.ToString());
            pageShowTime = Sys_Time.Instance.GetServerTime();
            if (UIManager.IsOpen(EUIID.UI_Pet_Develope_SelecItem))
            {
                layout.resistanceToggle.isOn = true;
                layout.basicDevelopePage.SetActive(false);
                layout.resistanceStrengthenPage.SetActive(true);
            }
        }

        public void Hide()
        {
            currentItemId = 0;
            curIndex = -1;
            transform.gameObject.SetActive(false);
            DefaultItem();
            Sys_Pet.Instance.resistanceAddPointDic.Clear();
            UIManager.HitPointHide(EUIID.UI_Pet_Message, pageShowTime, EPetMessageViewState.Foster.ToString());
        }

        public  void ProcessEvents(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnSelectItem, OnSelectItem, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdateExp, OnUpdateExp, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int,int>(Sys_Bag.EEvents.OnRefreshChangeData, OnChangeItemCount, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnChangeResPoint, OnChangeResPoint, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdatePetInfo, OnUpdatePetInfo, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdateLoyalty, OnUpdateLoyalty, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<int, int>(Sys_Pet.EEvents.OnSelectAddPointPlan, OnSelectAddPointPlan, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnAllocEnhancePlanUse, OnAllocEnhancePlanUse, toRegister);
            petPointPlan.PocessEvent(toRegister);
        }

        #region CallBack

        private void OnSelectItem(uint itemid)
        {
            SelectShow(itemid);
        }

        private void OnChangeItemCount(int changeType, int curBoxId)
        {
            if (!transform.gameObject.activeInHierarchy|| changeType==0)
            {
                return;
            }
            foreach (var v in expDic)
            {
                if (v.Key == currentItemId)
                {
                    if (Sys_Bag.Instance.GetItemCount(currentItemId) == 0)
                    {
                        v.Value.count.text = LanguageHelper.GetTextContent(10883, Sys_Bag.Instance.GetItemCount(currentItemId).ToString());
                        v.Value.showItemData.bUseTips = true;
                    }
                    else
                    {
                        v.Value.count.text = LanguageHelper.GetTextContent(2009377, Sys_Bag.Instance.GetItemCount(currentItemId).ToString(), "1");
                        v.Value.showItemData.bUseTips = false;
                    }
                    return;
                }
            }
            foreach (var v in loyatyDic)
            {
                if (v.Key == currentItemId)
                {
                    if (Sys_Bag.Instance.GetItemCount(currentItemId) == 0)
                    {
                        v.Value.count.text = LanguageHelper.GetTextContent(10883, Sys_Bag.Instance.GetItemCount(currentItemId).ToString()); 
                    }
                    else
                    {
                        v.Value.count.text = LanguageHelper.GetTextContent(2009377, Sys_Bag.Instance.GetItemCount(currentItemId).ToString(), "1");
                    }
                }
            }
        }

        private void OnUpdateExp()
        {
            if (!transform.gameObject.activeInHierarchy)
            {
                return;
            }
            for (int i = 0; i < Sys_Pet.Instance.petsList.Count; ++i)
            {
                ClientPet v = Sys_Pet.Instance.petsList[i];
                if (v.petUnit.Uid == currentChoosePet.petUnit.Uid)
                {
                    //if (layout.explevel.text != v.petUnit.Level.ToString())
                    //{
                    //    if (leftview.Fx_pet_level_up.activeInHierarchy)
                    //    {
                    //        leftview.Fx_pet_level_up.SetActive(false);
                    //    }
                    //    leftview.Fx_pet_level_up.SetActive(true);
                    //}
                    layout.explevel.text = v.petUnit.SimpleInfo.Level.ToString();
                    if (CSVPetNewlv.Instance.TryGetValue(v.petUnit.SimpleInfo.Level + 1,out CSVPetNewlv.Data data))
                    {
                        layout.explevelslider.value = v.petUnit.SimpleInfo.Exp / (float)data.exp;
                        layout.explevelup.text = LanguageHelper.GetTextContent(2009377, v.petUnit.SimpleInfo.Exp.ToString(), data.exp.ToString());
                    }
                    else
                    {
                        layout.explevelslider.value = v.petUnit.SimpleInfo.Exp / maxExp;
                        layout.explevelup.text = LanguageHelper.GetTextContent(2009377, v.petUnit.SimpleInfo.Exp.ToString(), ((decimal)maxExp).ToString());
                    }
                }
            }
            //leftview.SetValue(currentChoosePet);
            //petviewList.updateAllgrid();
            //petviewList.SetSelect(currentChoosePet.petUnit.Position);
        }

        private void OnChangeResPoint()
        {
            totalAddPoint = 0;
            foreach (var data in Sys_Pet.Instance.resistanceAddPointDic)
            {
                totalAddPoint += data.Value;
            }
            foreach (var data in resistanceAttrDic)
            {
                uint usePoint = GetUsedAddPointByAttrId(data.Key, curIndex);
                data.Value.RefreshItem(data.Key, usePoint, curIndex, totalAddPoint);
            }
            uint total = petUnit.EnhancePlansData.TotalPoint;
            uint use = petUnit.EnhancePlansData.Plans[curIndex].UsePoint;
            layout.leftPoint.text = (total- use - totalAddPoint).ToString();
        }

        private void OnUpdatePetInfo()
        {
            if (!transform.gameObject.activeInHierarchy)
            {
                return;
            }
            totalAddPoint = 0;
            for(uint i=0;i< Sys_Pet.Instance.resistanceAddPointDic.Keys.Count;++i)
            {
                Sys_Pet.Instance.resistanceAddPointDic[i] = 0;
            }
            foreach (var data in resistanceAttrDic)
            {
                uint usePoint = GetUsedAddPointByAttrId(data.Key, curIndex);
                data.Value.RefreshItem(data.Key, usePoint ,curIndex, totalAddPoint);
            }
            layout.strengthenLv.text = petUnit.EnhancePlansData.Enhancelvl.ToString();
            if (CSVPetNewEnhance.Instance.TryGetValue(petUnit.EnhancePlansData.Enhancelvl + 1, out CSVPetNewEnhance.Data csvdata))
            {
                layout.strengthenSlider.value = petUnit.EnhancePlansData.Enhanceexp / (float)csvdata.exp;
                layout.strengthenNum.text = LanguageHelper.GetTextContent(10884, petUnit.EnhancePlansData.Enhanceexp.ToString(), csvdata.exp.ToString()); 
            }
            else
            {
                layout.strengthenSlider.value = 1;
                layout.strengthenNum.text = LanguageHelper.GetTextContent(10884, petUnit.EnhancePlansData.Enhanceexp.ToString(), CSVPetNewEnhance.Instance.GetConfData(petUnit.EnhancePlansData.Enhancelvl).exp.ToString());
            }
            uint total = petUnit.EnhancePlansData.TotalPoint;
            uint use = petUnit.EnhancePlansData.Plans[curIndex].UsePoint;
            layout.leftPoint.text = (total-use).ToString();
        }

        private void OnUpdateLoyalty()
        {
            if (!transform.gameObject.activeInHierarchy)
            {
                return;
            }
            layout.loyaltyNum.text = LanguageHelper.GetTextContent(2009377, petUnit.SimpleInfo.Loyalty.ToString(), "100");
            layout.loyaltySlider.value = petUnit.SimpleInfo.Loyalty / 100f;
        }

        private void OnSelectAddPointPlan(int index, int type)
        {
            if (type == (int)Sys_Plan.EPlanType.PetAttributeCorrect)
            {
                DefaultResAttrItem();
                curIndex = index;
                AddResistanceStrengthenAttrs(curIndex);
                if (curIndex == currentChoosePet.petUnit.EnhancePlansData.CurrentPlanIndex)
                {
                    ImageHelper.SetImageGray(layout.useBtn.GetComponent<Image>(), true);
                    TextHelper.SetText(layout.useBtnText, 8311);
                    layout.useBtn.enabled = false;
                }
                else
                {
                    ImageHelper.SetImageGray(layout.useBtn.GetComponent<Image>(), false);
                    TextHelper.SetText(layout.useBtnText, 5127);
                    layout.useBtn.enabled = true;
                }
            }
        }

        private void OnAllocEnhancePlanUse(uint petId)
        {
            if (petId == petUnit.Uid)
            {
                curIndex = curIndex == 0 ? (int)petUnit.EnhancePlansData.CurrentPlanIndex : curIndex;
                petPointPlan.Refresh(currentChoosePet.petUnit, Sys_Plan.EPlanType.PetAttributeCorrect);
            }
        }
        #endregion

        #region Function

        private void SelectShow(uint itemid, bool isFirst = false)
        {
            if (Sys_Pet.Instance.expIdData.Contains(itemid))
            {
                layout.viewexp.SetActive(true);
                layout.viewLoyalty.SetActive(false);
                layout.expperadd.text = Sys_Pet.Instance.expData[itemid].ToString();
                layout.explevel.text = petUnit.SimpleInfo.Level.ToString();
                if (CSVPetNewlv.Instance.TryGetValue(petUnit.SimpleInfo.Level + 1, out CSVPetNewlv.Data data) && data != null)
                {
                    layout.explevelslider.value = (float)petUnit.SimpleInfo.Exp / data.exp;
                    layout.explevelup.text = LanguageHelper.GetTextContent(2009377, petUnit.SimpleInfo.Exp.ToString(), data.exp.ToString());
                }
                else
                {
                    layout.explevelslider.value = petUnit.SimpleInfo.Exp / maxExp;
                    layout.explevelup.text = LanguageHelper.GetTextContent(2009377, petUnit.SimpleInfo.Exp.ToString(), CSVParam.Instance.GetConfData(561).str_value);
                }
            }
            else if (Sys_Pet.Instance.loyatyIdData.Contains(itemid))
            {
                layout.viewexp.SetActive(false);
                layout.viewLoyalty.SetActive(true);
                layout.loyaltyNum.text = LanguageHelper.GetTextContent(2009377, petUnit.SimpleInfo.Loyalty.ToString(), "100");
                layout.loyaltySlider.value = petUnit.SimpleInfo.Loyalty / 100f;
                layout.loyaltyUpPer.text = Sys_Pet.Instance.loyatyData[itemid].ToString();
            }
            currentItemId = itemid;
            foreach (var v in expDic) { v.Value.SetSelectItem(currentItemId); }
            foreach (var v in loyatyDic) { v.Value.SetSelectItem(currentItemId); }
        }

        public void SetPetData(ClientPet client,int subPage,uint itemId)
        {
            if (client == null)
                return;
            currentChoosePet = client;
            if (currentItemId == 0)
            {
                currentItemId = itemId;
            }
            petUnit = currentChoosePet.petUnit;

            curIndex = curIndex == 0 ? (int)petUnit.EnhancePlansData.CurrentPlanIndex : curIndex;
            petPointPlan.Refresh(currentChoosePet.petUnit, Sys_Plan.EPlanType.PetAttributeCorrect);


            DefaultItem();
            AddExpItemList();
            AddLoyatyItemList();
            AddResistanceStrengthenAttrs(curIndex);
            defaultItemid = Sys_Pet.Instance.expIdData[0];
            if (currentItemId == 0)
            {
                SelectShow(defaultItemid,true);
            }
            else
            {
                SelectShow(currentItemId,true);
            }
            if(subPage==1)
                layout.resistanceToggle.isOn = true;
        }

        private void AddExpItemList()
        {
            expDic.Clear();
            foreach (var data in Sys_Pet.Instance.expData)
            {
                uint id = data.Key;
                GameObject go = GameObject.Instantiate<GameObject>(layout.expItemGo, layout.expItemGo.transform.parent);

                propItem = new PropItem();
                propItem.BindGameObject(go);
                PropIconLoader.ShowItemData showItemData = new PropIconLoader.ShowItemData(id, 1, true, false, false, false, false,
                    _bShowCount: true, _bShowBagCount: true, _bUseClick: true, (propItem) => { SelectShow(id); }, true, true);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_Pet_Develop, showItemData));

                UI_Pet_Develop_Prop expItem = new UI_Pet_Develop_Prop(id, showItemData);
                expItem.Init(go.transform);
                expDic.Add(id, expItem);

                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(id);
                if (null != cSVItemData)
                    TextHelper.SetText(propItem.txtName, cSVItemData.name_id);
            }
            layout.expItemGo.SetActive(false);
        }

        private void AddLoyatyItemList()
        {
            loyatyDic.Clear();
            foreach (var data in Sys_Pet.Instance.loyatyData)
            {
                uint id = data.Key;
                GameObject go = GameObject.Instantiate<GameObject>(layout.loyatyItemGo, layout.loyatyItemGo.transform.parent);

                propItem = new PropItem();
                propItem.BindGameObject(go);
                PropIconLoader.ShowItemData showItemData= new PropIconLoader.ShowItemData(id, 1, true, false, false, false, false,
                    _bShowCount: true, _bShowBagCount: true, _bUseClick: true, (propItem) =>
                    {
                        SelectShow(id);
                    }, true, true);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_Pet_Develop,showItemData));

                UI_Pet_Develop_Prop loyatyItem = new UI_Pet_Develop_Prop(id, showItemData);
                loyatyItem.Init(go.transform);
                loyatyDic.Add(id, loyatyItem);
                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(id);
                if (null != cSVItemData)
                    TextHelper.SetText(propItem.txtName, cSVItemData.name_id);
            }
            layout.loyatyItemGo.SetActive(false);
        }

        private void DefaultItem()
        {
            layout.expItemGo.SetActive(true);
            layout.loyatyItemGo.SetActive(true);
            foreach (var item in expDic)
            {
                item.Value.OnDestroy();
            }
            foreach (var item in loyatyDic)
            {
                item.Value.OnDestroy();
            }
            expDic.Clear();
            loyatyDic.Clear();
            FrameworkTool.DestroyChildren(layout.expItemGo.transform.parent.gameObject, layout.expItemGo.transform.name);
            FrameworkTool.DestroyChildren(layout.loyatyItemGo.transform.parent.gameObject, layout.loyatyItemGo.transform.name);
            DefaultResAttrItem();
        }

        private void DefaultResAttrItem()
        {
            layout.resattrGo.SetActive(true);
            resistanceAttrDic.Clear();
            FrameworkTool.DestroyChildren(layout.resattrGo.transform.parent.gameObject, layout.resattrGo.transform.name);
        }

        private void PlayUseItemFx(uint count, uint itemid)
        {
            //if (count == 1)
            //{
            //    if (basicitemDic.TryGetValue (itemid,out UI_Pet_Develop_Prop value)&& value!=null)
            //    {
            //        if (value.fxonce.activeInHierarchy)
            //        {
            //            value.fxonce.SetActive(false);
            //        }
            //        value.fxonce.SetActive(true);
            //    }
            //    else
            //    {
            //        if (attritemDic[itemid].fxonce.activeInHierarchy)
            //        {
            //            attritemDic[itemid].fxonce.SetActive(false);
            //        }
            //        attritemDic[itemid].fxonce.SetActive(true);
            //    }
            //}
            //else
            //{
            //    if (basicitemDic[itemid].fxten.activeInHierarchy)
            //    {
            //        basicitemDic[itemid].fxten.SetActive(false);
            //    }
            //    basicitemDic[itemid].fxten.SetActive(true);
            //}
        }

        private  uint GetMaxLv(out uint Exp)
        {
            uint lv = Sys_Role.Instance.Role.Level + CSVPetNewParam.Instance.GetConfData(7).value;
            if (CSVPetNewlv.Instance.TryGetValue( lv+1, out CSVPetNewlv.Data value) && value != null)
            {
                Exp = value.exp;
            }
            else
            {
                Exp =0;
            }
            return lv;
        }

        private void AddResistanceStrengthenAttrs(int index)
        {
            uint enhancelvl = petUnit.EnhancePlansData.Enhancelvl;
            layout.strengthenLv.text = enhancelvl.ToString();
            uint use = petUnit.EnhancePlansData.Plans[index].UsePoint;
            uint total = petUnit.EnhancePlansData.TotalPoint;
            layout.leftPoint.text =(total- use).ToString();
            if (CSVPetNewEnhance.Instance.TryGetValue(enhancelvl + 1, out CSVPetNewEnhance.Data nextdata))
            {
                layout.strengthenSlider.value = petUnit.EnhancePlansData.Enhanceexp / (float)nextdata.exp;
                layout.strengthenNum.text = LanguageHelper.GetTextContent(10884, petUnit.EnhancePlansData.Enhanceexp.ToString(), nextdata.exp.ToString()); 
            }
            else
            {
                layout.strengthenSlider.value = 1;
                layout.strengthenNum.text = LanguageHelper.GetTextContent(10884, petUnit.EnhancePlansData.Enhanceexp.ToString(), CSVPetNewEnhance.Instance.GetConfData(enhancelvl).exp.ToString()); 
            }
            resistanceAttrDic.Clear();
            Sys_Pet.Instance.resistanceAddPointDic.Clear();
          
            if (enhancelvl == 0)
            {
                enhancelvl++;
            }
            List<List<uint>> list = CSVPetNewEnhance.Instance.GetConfData(enhancelvl).attr;
            for (int i = 0; i < list.Count; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.resattrGo, layout.resattrGo.transform.parent);
                UI_Pet_Resistance_Attr attr = new UI_Pet_Resistance_Attr(list[i][0], currentChoosePet);
                attr.Init(go.transform);
                Sys_Pet.Instance.resistanceAddPointDic.Add(list[i][0], 0);
                uint usePoint = GetUsedAddPointByAttrId(list[i][0], index);

                attr.RefreshItem(list[i][0], usePoint, curIndex);
                resistanceAttrDic.Add(list[i][0], attr);
            }
            layout.resattrGo.SetActive(false);
        }

        private uint GetUsedAddPointByAttrId(uint id, int index)
        {
            for(int i = 0; i < petUnit.EnhancePlansData.Plans[index].AllocPointId.Count; ++i)
            {
                if (petUnit.EnhancePlansData.Plans[index].AllocPointId[i] == id)
                {
                    return petUnit.EnhancePlansData.Plans[index].AllocPointValue[i];
                }
            }
            return 0; 
        }
        #endregion

        #region ButtonClick
        public void OnDevelopRuleBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2009495) });
        }

        public void OnoncebtnClicked()
        {
            if (petUnit.SimpleInfo.ExpiredTick > 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000654));
                return;
            }
            if (Sys_Pet.Instance.expIdData.Contains(currentItemId))   //一次经验
            {
                if (currentChoosePet.petUnit.SimpleInfo.Exp < maxExp)
                {
                    if (Sys_Bag.Instance.GetItemCount(currentItemId) > 0)
                    {
                        //检测经验是否满级   
                        uint maxLv = GetMaxLv(out uint maxFightExp);
                        if (petUnit.SimpleInfo.Level> maxLv || (petUnit.SimpleInfo.Level == maxLv&& petUnit.SimpleInfo.Exp>= maxFightExp))
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10885));
                        }
                        else
                        {
                            Sys_Hint.Instance.PushEffectInNextFight();
                            Sys_Pet.Instance.OnGroomUseItemReq(petUnit.Uid, currentItemId, 1, (uint)GroomType.Exp);
                            PlayUseItemFx(1, currentItemId);
                        }
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4052));
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101514));
                }
            }
            else if (Sys_Pet.Instance.loyatyIdData.Contains(currentItemId))   //一次忠诚度
            {
                if (petUnit.SimpleInfo.Loyalty < 100)
                {
                    Sys_Pet.Instance.OnGroomUseItemReq(petUnit.Uid, currentItemId, 1, (uint)GroomType.Loaylty);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10840));
                }
            }
        }

        public void OntenbtnClicked()
        {
            if (petUnit.SimpleInfo.ExpiredTick > 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000654));
                return;
            }
            if (Sys_Pet.Instance.expIdData.Contains(currentItemId))   //十次经验
            {
                if (petUnit.SimpleInfo.Exp < maxExp)
                {
                    if (Sys_Bag.Instance.GetItemCount(currentItemId) > 0)
                    {
                        //检测经验是否满级   
                        uint maxLv = GetMaxLv(out uint maxFightExp);
                        if (petUnit.SimpleInfo.Level > maxLv || (petUnit.SimpleInfo.Level == maxLv && petUnit.SimpleInfo.Exp >= maxFightExp))
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10885));
                        }
                        else
                        {
                            Sys_Hint.Instance.PushEffectInNextFight();
                            Sys_Pet.Instance.OnGroomUseItemReq(petUnit.Uid, currentItemId, 10, (uint)GroomType.Exp);
                            PlayUseItemFx(10, currentItemId);
                        }
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4052));
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101514));
                }
            }
            else if (Sys_Pet.Instance.loyatyIdData.Contains(currentItemId))   //十次忠诚度
            {
                if (petUnit.SimpleInfo.Loyalty < 100)
                {
                    Sys_Pet.Instance.OnGroomUseItemReq(petUnit.Uid, currentItemId, 10, (uint)GroomType.Loaylty);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10840));
                }
            }
        }

        public void OnuseItemBtnClicked()
        {
            if (petUnit.SimpleInfo.ExpiredTick > 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000654));
                return;
            }
            Sys_Pet.Instance.selectItemItem.Clear();
            string[] str = CSVPetNewParam.Instance.GetConfData(13).str_value.Split('|');
            for (int i = 0; i < str.Length; ++i)
            {
                Sys_Pet.Instance.selectItemItem.Add(uint.Parse(str[i].Split('&')[0]));
            }
            Sys_Pet.Instance.selectItemItem.Add(0);
            UIManager.OpenUI(EUIID.UI_Pet_Develope_SelecItem, false, petUnit);
        }

        public void OnresetBtnClicked()
        {
            if (petUnit.SimpleInfo.ExpiredTick > 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000654));
                return;
            }
            string contentItem = null;
            string[] str = CSVPetNewParam.Instance.GetConfData(19).str_value.Split('|');
            for(int i=0;i< str.Length; ++i)
            {
                string[] strReset = str[i].Split('&');
                contentItem+=LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(uint.Parse(strReset[0])).name_id)+strReset[1];
            }
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(10841, contentItem);  //文字表id 强化属性点重置
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                if (currentChoosePet.petUnit.EnhancePlansData.Plans[curIndex].AllocPointId.Count==0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009464));
                }
                else
                {
                    Sys_Hint.Instance.PushEffectInNextFight();
                    Sys_Pet.Instance.OnEnhancePointResetReq(petUnit.Uid, (uint)curIndex);
                }
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);

        }

        public void OnaddPointBtnClicked()
        {
            if (petUnit.SimpleInfo.ExpiredTick > 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000654));
                return;
            }
            if (totalAddPoint == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10842));
            }
            else
            {
                Sys_Pet.Instance.OnAllocEnhancePointReq(Sys_Pet.Instance.resistanceAddPointDic, petUnit.Uid, (uint)curIndex);
            }
        }

        public void OnbasicToggleValueChanged(bool isOn)
        {
            layout.basicDevelopePage.SetActive(isOn);
            layout.resistanceStrengthenPage.SetActive(!isOn);
        }

        public void OnresistanceToggleValueChanged(bool isOn)
        {
            layout.basicDevelopePage.SetActive(!isOn);
            layout.resistanceStrengthenPage.SetActive(isOn);
        }

        public void OnuseBtnClicked()
        {
            Sys_Pet.Instance.AllocEnhancePlanUseReq(petUnit.Uid, (uint)curIndex);
        }
        #endregion
    }
}


