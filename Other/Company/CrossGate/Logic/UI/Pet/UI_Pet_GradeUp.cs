using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Pet_GradeUp_Layout
    {
        public Transform transform;
        public Button closeBtn;
        public Button sureBtn;
        public GameObject attrGo;

        public void Init(Transform transform)
        {
            this.transform = transform;
            closeBtn = transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            sureBtn = transform.Find("Animator/Btn_01").GetComponent<Button>();
            attrGo = transform.Find("Animator/View_Property/Attr").gameObject;
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
            sureBtn.onClick.AddListener(listener.OnsureBtnClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
            void OnsureBtnClicked();
        }
    }

    public class UI_Pet_Grade : UIComponent
    {
        private Text attrname;
        private Slider costslider;
        private Text costnumber;
        private Text maxnumber;
        private Text addnumber;
        private GameObject prepGo;
        private GameObject prepGoGroup;

        private uint attrid;
        private uint itemid;
        private PetGradeEvt gradeevt;

        private float minWidth = 40;
        private float maxWidth = 195;

        public UI_Pet_Grade( uint _attrid, PetGradeEvt _evt) : base()
        {
            attrid = _attrid;
            gradeevt = _evt;
        }

        protected override void Loaded()
        {
            attrname = transform.Find("Text_Attr").GetComponent<Text>();
            costslider = transform.Find("Text_Attr/Slider").GetComponent<Slider>();
            costnumber = transform.Find("Text_Attr/Text_Percent").GetComponent<Text>();
            if (null != transform.Find("Text_Attr/Text_Percent (1)"))
            {
                maxnumber = transform.Find("Text_Attr/Text_Percent (1)").GetComponent<Text>();
            }
            addnumber = transform.Find("Text_Attr/Text_Up").GetComponent<Text>();
            prepGo = transform.Find("Text_Attr/Grid_Grade/Image_GradeBG01").gameObject;
            prepGoGroup = transform.Find("Text_Attr/Grid_Grade").gameObject;
            maxWidth = transform.Find("Text_Attr/Slider").GetComponent<RectTransform>().rect.width;
        }

        private float GetAdvanceConfigValue(uint id)
        {
            float advanceValue = 0;
            CSVPetNew.Data currentpet = CSVPetNew.Instance.GetConfData(id);
            if (null != currentpet)
            {
                if (attrid == (uint)EBaseAttr.Vit)
                {
                    advanceValue = currentpet.endurance / 10000.0f;
                }
                else if (attrid == (uint)EBaseAttr.Snh)
                {
                    advanceValue = currentpet.strength / 10000.0f;
                }
                else if (attrid == (uint)EBaseAttr.Inten)
                {
                    advanceValue = currentpet.strong / 10000.0f;
                }
                else if (attrid == (uint)EBaseAttr.Speed)
                {
                    advanceValue = currentpet.speed / 10000.0f;
                }
                else if (attrid == (uint)EBaseAttr.Magic)
                {
                    advanceValue = currentpet.magic / 10000.0f;
                }
            }
            return advanceValue;
        }

        public void RefreshShow(PetGradeEvt evt)
        {
            ClientPet clientPet = null;
            float currentAdvanceValue = 0;
            for (int i=0;i< Sys_Pet.Instance.petsList.Count;++i)
            {
                if (Sys_Pet.Instance.petsList[i].petUnit.Uid == evt.petuid)
                {
                    currentAdvanceValue = GetAdvanceConfigValue(Sys_Pet.Instance.petsList[i].petUnit.SimpleInfo.PetId);
                    clientPet = Sys_Pet.Instance.petsList[i];
                }
            }
            if (null != maxnumber)
            {
                costnumber.text = clientPet.grades[attrid].ToString("0.#");
                maxnumber.text = currentAdvanceValue.ToString();
            }
            else
            {
                costnumber.text = string.Format("{0}/{1}", clientPet.grades[attrid].ToString("0.#"), currentAdvanceValue);
            }
            DwShow(clientPet.grades[attrid], currentAdvanceValue);
            costslider.value = 1.0f;
            attrname.text =LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(attrid).name);
            //addnumber.text = "+" + (CSVPetGroom.Instance.GetConfData(gradeevt.itemid).item_effect / 10000.0f).ToString();
        }

        private void DwShow(float currentD, float petConfigD)
        {
            DefaultPetDw();
            int prep = (int)currentD - (int)petConfigD;
            RectTransform advSliderRect = costslider.GetComponent<RectTransform>();
            Vector2 offset = advSliderRect.anchoredPosition;
            float ad_width = ((petConfigD / 50) * maxWidth);
            if (ad_width < minWidth)
            {
                ad_width = minWidth;
            }
            else if (ad_width > maxWidth)
            {
                ad_width = maxWidth;
            }

            advSliderRect.sizeDelta = new Vector2(ad_width, advSliderRect.sizeDelta.y);
            RectTransform prepGoGrouprect = prepGoGroup.GetComponent<RectTransform>();
            prepGoGrouprect.anchoredPosition = new Vector2(offset.x + advSliderRect.sizeDelta.x + prepGoGrouprect.sizeDelta.x / 2 + 5, prepGoGroup.transform.localPosition.y);
            prepGoGroup.gameObject.SetActive(true);
            if (prep > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i < 4)
                    {
                        GameObject go = GameObject.Instantiate<GameObject>(prepGo, prepGoGroup.transform);
                        Advance_DwSub dwGo = new Advance_DwSub();
                        dwGo.BingGameObject(go);
                        dwGo.SetState(EDwState.Have);
                    }
                }
                for (int i = 0; i < prep; i++)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(prepGo, prepGoGroup.transform);
                    Advance_DwSub dwGo = new Advance_DwSub();
                    dwGo.BingGameObject(go);
                    dwGo.SetState(EDwState.Full);
                }
            }
            else
            {
                for (int i = 4; i > 0; i--)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(prepGo, prepGoGroup.transform);
                    Advance_DwSub dwGo = new Advance_DwSub();
                    dwGo.BingGameObject(go);
                    if (prep + i <= 0)
                    {
                        dwGo.SetState(EDwState.Cut);
                    }
                    else
                    {
                        dwGo.SetState(EDwState.Have);
                    }
                }
            }
            prepGo.SetActive(false);
        }

        void DefaultPetDw()
        {
            prepGo.SetActive(true);
            for (int i = 0; i < prepGoGroup.transform.childCount; i++)
            {
                if (i >= 1) GameObject.Destroy(prepGoGroup.transform.GetChild(i).gameObject);
            }
        }

    }

    public class UI_Pet_GradeUp : UIBase, UI_Pet_GradeUp_Layout.IListener
    {
        private UI_Pet_GradeUp_Layout layout = new UI_Pet_GradeUp_Layout();
        private List<UI_Pet_Grade> petgradelist = new List<UI_Pet_Grade>();

        private PetGradeEvt gradeevt;


        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this); 
        }

        protected override void OnOpen(object evt)
        {
            gradeevt =(PetGradeEvt) evt;
        }

        protected override void OnShow()
        {
            DefaultAttr();
            AddAttrList();
        }


        private void AddAttrList()
        {
            petgradelist.Clear();
            foreach (var item in Sys_Pet.Instance.baseAttrs2Id)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.attrGo, layout.attrGo.transform.parent);
                UI_Pet_Grade petgrade = new UI_Pet_Grade(item.Value,gradeevt);
                petgrade.Init(go.transform);
                petgrade.RefreshShow(gradeevt);
                petgradelist.Add(petgrade);
            }
            layout.attrGo.SetActive(false);
        }

        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_GradeUp);
        }

        public void OnsureBtnClicked()
        {
            if (Sys_Bag.Instance.GetItemCount(gradeevt.itemid) > 0)
            {
                Sys_Hint.Instance.PushEffectInNextFight();
                //Sys_Pet.Instance.OnGroomUseItemReq(gradeevt.petuid, gradeevt.itemid, 1);
                UIManager.CloseUI(EUIID.UI_Pet_GradeUp);
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10643));
            }
            else
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4052));
        }

        private void DefaultAttr()
        {
            layout.attrGo.SetActive(true);
            for (int i=0;i< petgradelist.Count;++i) { petgradelist[i].OnDestroy(); }
            FrameworkTool.DestroyChildren(layout.attrGo.transform.parent.gameObject, layout.attrGo.transform.name);
        }
    }
}
