using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{
    public enum EFamilyCreatureGet
    {
        Get,
        PreView,
    }

    public class UI_FamilyCreatures_Get_Layout
    {
        private Button closeBtn;
        private Button getBtn;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        private GameObject tabProto;
        public GameObject TabProto { get => tabProto;}
        public Text titleText;

        private GameObject textTips;
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Animator/View_Right/Scroll View").GetComponent<InfinityGrid>();
            closeBtn = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            getBtn = transform.Find("Animator/View_Right/Btn_Select").GetComponent<Button>();
            textTips = transform.Find("Animator/View_Right/Text_Tips").gameObject;
            tabProto = transform.Find("Animator/View_LeftList/Toggle_Group/Toggle").gameObject;
            titleText =transform.Find("Animator/View_TipsBgNew01/Text_Title").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            getBtn.onClick.AddListener(listener.GetBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
        }

        public void SetGetButtonState(bool state)
        {
            getBtn.gameObject.SetActive(state);
        }

        public void SetTitleTextState(bool state)
        {
            textTips.SetActive(state);
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
        }

        public void InfinityGridMoveTo(int index)
        {
            infinityGrid.MoveToIndex(index);
        }

        public void SetTitle(uint langId)
        {
            TextHelper.SetText(titleText, langId);
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void GetBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    /// <summary> UI_FamilyCreaturesHead </summary>
    public class UI_FamilyCreaturesHead : UIComponent
    {
        //private Text nameText;
        private RawImage headImage;
        private Text levelText;
        private Text typeText;
        private GameObject currentStateObj;
        private GameObject nextTypeObj;
        /// <summary> 预设节点加载 </summary>
        protected override void Loaded()
        {
            //nameText = transform.Find("Title/Text").GetComponent<Text>();
            headImage = transform.Find("Texture").GetComponent<RawImage>();
            levelText = transform.Find("Image_Form/Text_Value").GetComponent<Text>();
            typeText = transform.Find("Text_Name").GetComponent<Text>();
            nextTypeObj = transform.Find("Image_Arrows").gameObject;
            currentStateObj = transform.Find("Image_Currene").gameObject;
        }

        public void SetData(CSVFamilyPet.Data data)
        {
            ImageHelper.SetTexture(headImage, data.icon2_id);
            levelText.text = LanguageHelper.GetTextContent(2023502, data.stage.ToString());
            typeText.text = LanguageHelper.GetTextContent(Sys_Family.Instance.CreatureState(data.stage));
            nextTypeObj.SetActive(data.familyPet_id != 0);
            currentStateObj.SetActive(Sys_Family.Instance.IsHasCreatureState(data.id));
        }
    }

    public class UI_FamilyCreatures_Get : UIBase, UI_FamilyCreatures_Get_Layout.IListener
    {
        private UI_FamilyCreatures_Get_Layout layout = new UI_FamilyCreatures_Get_Layout();

        public UIElementCollector<GetTab> tabVds = new UIElementCollector<GetTab>();

        List<CSVFamilyPet.Data>  cSVFamilyPetDatas = new List<CSVFamilyPet.Data>();
        public class GetTab : UISelectableElement
        {
            public CP_Toggle toggle;
            public Text tabNameLight;
            public Text tabNameDark;
            public GameObject getObj;
            public int tabId = 0;

            protected override void Loaded()
            {
                this.toggle = this.transform.GetComponent<CP_Toggle>();
                this.toggle.onValueChanged.AddListener(this.Switch);

                this.tabNameLight = this.transform.Find("Background/Text").GetComponent<Text>();
                this.tabNameDark = this.transform.Find("Image_Select/Text").GetComponent<Text>();
                this.getObj = this.transform.Find("Image_Get").gameObject;
            }

            public void Refresh(int typeid, bool btnType)
            {
                this.tabId = typeid;
                uint langId = 2010605;
               switch(typeid)
                {
                    case 1:
                        langId = 2023512;
                        break;
                    case 2:
                        langId = 2023513;
                        break;
                    case 3:
                        langId = 2023511;
                        break;
                    case 4:
                        langId = 2023514;
                        break;
                }
                TextHelper.SetText(this.tabNameLight, langId);
                TextHelper.SetText(this.tabNameDark, langId);

                if(btnType)
                {
                    bool hasGet = Sys_Family.Instance.GetTypeIsGet((uint)typeid);
                    SetGetState(hasGet);
                    ImageHelper.SetImageGray(toggle, hasGet, true);
                    toggle.interactable = !hasGet;
                }
            }

            public void SetGetState(bool state)
            {
                this.getObj.SetActive(state);
            }

            public void Switch(bool arg)
            {
                if (arg)
                {
                    this.onSelected?.Invoke(this.tabId, true);
                }
            }

            public override void SetSelected(bool toSelected, bool force)
            {
                this.toggle.SetSelected(toSelected, true);
            }
        }

        private EFamilyCreatureGet eFamilyCreatureGet = EFamilyCreatureGet.Get;

        public int currentTabId = -1;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }

        protected override void OnOpen(object arg = null)
        {
            if (arg is Tuple<uint, object>)
            {
                Tuple<uint, object> tuple = arg as Tuple<uint, object>;
                eFamilyCreatureGet = (EFamilyCreatureGet)System.Convert.ToUInt32(tuple.Item1);
                if(eFamilyCreatureGet != EFamilyCreatureGet.Get)
                {
                    currentTabId = Convert.ToInt32(tuple.Item2);
                }
                else
                {
                    currentTabId = -1;
                }
            }
        }

        protected override void OnShow()
        {
            RefreshAll();
        }

        List<int> tabIds = new List<int>() { 1, 2 ,3, 4};

        public void RefreshAll()
        {
            layout.SetTitle(eFamilyCreatureGet == EFamilyCreatureGet.Get ? 2023508u : 2023500u);
            var ids = this.tabIds;
            this.tabVds.BuildOrRefresh<int>(this.layout.TabProto, this.layout.TabProto.transform.parent, ids, (vd, id, indexOfVdList) => {
                vd.SetUniqueId(id);
                vd.SetSelectedAction((innerId, force) => {
                    this.currentTabId = innerId;

                    cSVFamilyPetDatas.Clear();

                    var dataList = CSVFamilyPet.Instance.GetAll();
                    for (int i = 0, len = dataList.Count; i < len; i++)
                    {
                        if(currentTabId == dataList[i].food_Type)
                        {
                            cSVFamilyPetDatas.Add(dataList[i]);
                        }
                    }
                    if(cSVFamilyPetDatas.Count > 1)
                    {
                        cSVFamilyPetDatas.Sort((a, b) => {
                            return (int)a.stage - (int)b.stage;
                        });
                    }
                    layout.SetInfinityGridCell(cSVFamilyPetDatas.Count);
                    this.layout.InfinityGridMoveTo(0);
                });
                vd.Refresh(id, eFamilyCreatureGet == EFamilyCreatureGet.Get);
            });

            // 默认选中Tab
            if (ids.Count > 0)
            {
                if (this.currentTabId <= 0 || !ids.Contains(this.currentTabId))
                {
                    for (int i = 0; i < ids.Count; i++)
                    {
                        //如果是获取界面默认选中未领取，如果是预览默认选中预览的兽
                        if(eFamilyCreatureGet != EFamilyCreatureGet.Get || !Sys_Family.Instance.GetTypeIsGet((uint)ids[i]))
                        {
                            this.currentTabId = ids[i];
                            break;
                        }
                    }
                    
                }
                this.tabVds[this.currentTabId - 1].SetSelected(true, true);
            }

            SetBtnState();
        }

        private void  SetBtnState()
        {
            layout.SetGetButtonState(eFamilyCreatureGet == EFamilyCreatureGet.Get && Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.FamilyPetEgg));
            layout.SetTitleTextState(eFamilyCreatureGet == EFamilyCreatureGet.Get && !Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.FamilyPetEgg));
        }

        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_FamilyCreaturesHead entry = new UI_FamilyCreaturesHead();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= cSVFamilyPetDatas.Count)
                return;
            UI_FamilyCreaturesHead entry = cell.mUserData as UI_FamilyCreaturesHead;

            entry.SetData(cSVFamilyPetDatas[index]);
        }

        public void CloseBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Get, "CloseBtnClicked");
            UIManager.CloseUI(EUIID.UI_FamilyCreatures_Get);
        }

        public void GetBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Get, "GetBtnClicked");
            if (cSVFamilyPetDatas.Count > 0)
            { 
                string str = LanguageHelper.GetTextContent(2023520, LanguageHelper.GetTextContent(2023366, LanguageHelper.GetTextContent(Sys_Family.Instance.GetTypeName(cSVFamilyPetDatas[0].food_Type)), LanguageHelper.GetTextContent(Sys_Family.Instance.CreatureState(cSVFamilyPetDatas[0].stage))));
                PromptBoxParameter.Instance.OpenPromptBox(str, 0, () => {
                    Sys_Family.Instance.GuildPetGetGuildPetReq(cSVFamilyPetDatas[0].id / 10);
                });
                
            }
        }
    }
}