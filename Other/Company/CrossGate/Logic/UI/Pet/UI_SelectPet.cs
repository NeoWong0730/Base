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
    public class SelectPetParam
    {
        public List<PetUnit> PetList;
        public uint commonId;
        public Action<uint> action;
    }

    public class UI_SelectPet_Layout 
    {
        public Transform transform;
        public Button closeBtn;
        public GameObject petGo;

        public void Init(Transform transform)
        {
            this.transform = transform;
            closeBtn = transform.Find("Blank (1)").GetComponent<Button>();
            petGo = transform.Find("Animator/Scroll_View/Grid/Item").gameObject;
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
        }
    }

    public class UI_SelectPet_Item : UIComponent
    {
        public uint petid;
        private Button petIconBtn;
        private Image icon;
        private Text petname;
        private Text level;
        private GameObject selectedGo;
        private GameObject fightGo;
        private GameObject mountTagGo;
        private GameObject domesticationTagGo;
        private GameObject unDomesticationTagGo;

        private Button selectBtn;
        Action<UI_SelectPet_Item> action;
        public UI_SelectPet_Item(uint _petid) : base()
        {
            petid = _petid;
        }

        protected override void Loaded()
        {
            icon = transform.Find("PetItem01/Pet01/Image_Icon").GetComponent<Image>();
            petname = transform.Find("Text_Name").GetComponent<Text>();
            level = transform.Find("Text_MagicDeeds").GetComponent<Text>();
            selectedGo = transform.Find("PetItem01/Image_Select01").gameObject;
            fightGo = transform.Find("PetItem01/Pet01/Imag_Fight").gameObject;

            mountTagGo = transform.Find("PetItem01/Pet01/Image_Ride")?.gameObject;
            domesticationTagGo = transform.Find("Image_Ride01").gameObject;
            unDomesticationTagGo = transform.Find("Image_Ride02").gameObject;
            selectBtn = transform.GetComponent<Button>();
            selectBtn.onClick.AddListener(OnselectBtnClicked);
            petIconBtn = transform.Find("PetItem01/ItemBg").GetComponent<Button>();
            petIconBtn.onClick.AddListener(OnPetIconBtnClicked);
        }

        public void AddListenSelect(Action<UI_SelectPet_Item> action)
        {
            this.action = action;
        }

        private void OnselectBtnClicked()
        {
            action?.Invoke(this);
        }

        private void OnPetIconBtnClicked()
        {
            MessageEx practiceEx = new MessageEx
            {
                messageState = EPetMessageViewState.Attribute,
                petUid = petid
            };
            Sys_Pet.Instance.OnGetPetInfoReq(practiceEx, EPetUiType.UI_Message);
        }

        public void RefreshItem(PetUnit petunit)
        {
            CSVPetNew.Data petData = CSVPetNew.Instance.GetConfData(petunit.SimpleInfo.PetId);
            ImageHelper.SetIcon(icon, petData.icon_id);
            if (petunit.SimpleInfo.Name.IsEmpty)
            {
                petname.text = LanguageHelper.GetTextContent(CSVPetNew.Instance.GetConfData(petunit.SimpleInfo.PetId).name);
            }
            else
            {
                petname.text = petunit.SimpleInfo.Name.ToStringUtf8();
            }
            
            mountTagGo?.SetActive(petunit.Uid == Sys_Pet.Instance.mountPetUid);
            bool isMount = petData.mount;
            bool isDomes = petunit.SimpleInfo.MountDomestication == 1;
            domesticationTagGo.SetActive(isMount && isDomes);
            unDomesticationTagGo.SetActive(isMount && !isDomes);

            fightGo.SetActive(Sys_Pet.Instance.fightPet.IsSamePet(petunit));

            level.text = LanguageHelper.GetTextContent(2009305, petunit.SimpleInfo.Level.ToString());
        }
    }

    public class UI_SelectPet : UIBase, UI_SelectPet_Layout.IListener
    {
        private UI_SelectPet_Layout layout = new UI_SelectPet_Layout();
        private List<UI_SelectPet_Item> itemlist = new List<UI_SelectPet_Item>();
        private SelectPetParam param;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg)
        {
            param = arg as SelectPetParam;
        }

        protected override void OnShow()
        {
            SetValue();
        }

        protected override void OnHide()
        {
            DefaultItem();
        }

        public void OnSelectPet(UI_SelectPet_Item petItem)
        {
            param.action?.Invoke(petItem.petid);
        }

        private void SetValue()
        {
            itemlist.Clear();
            for (int i = 0; i < param.PetList.Count; i++)
            {
                if(null != Sys_Pet.Instance.GetPetByUId(param.PetList[i].Uid))
                {
                    GameObject go = GameObject.Instantiate<GameObject>(layout.petGo, layout.petGo.transform.parent);
                    UI_SelectPet_Item item = new UI_SelectPet_Item(param.PetList[i].Uid);
                    item.Init(go.transform);
                    item.AddListenSelect(OnSelectPet);
                    item.RefreshItem(param.PetList[i]);
                    itemlist.Add(item);
                }
            }

            layout.petGo.SetActive(false);
        }

        private void DefaultItem()
        {
            layout.petGo.SetActive(true);
            for (int i=0;i< itemlist.Count;++i) { itemlist[i].OnDestroy(); }
            FrameworkTool.DestroyChildren(layout.petGo.transform.parent.gameObject, layout.petGo.transform.name);
        }

        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_SelectPet);
        }
    }
}
