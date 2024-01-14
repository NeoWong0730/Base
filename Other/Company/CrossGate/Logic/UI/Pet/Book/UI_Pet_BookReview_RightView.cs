using UnityEngine;
using Logic.Core;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using Lib.Core;
using Table;

namespace Logic
{
    public class UI_PetReodel_AttrReview
    {
        private uint attrid;
        private uint petid;
        private uint remodelcount;
        private float maxgrade;
        private float maxgradeLow;
        private Text attrname;
        private GameObject stargo;
        private Text maxText;
        private Text minText;

        public UI_PetReodel_AttrReview(uint _attrid)
        {
            attrid = _attrid;
        }

        public void Init(Transform transform)
        {
            attrname = transform.GetComponent<Text>();
            stargo = transform.Find("Grid_Grade/Image_GradeBG01").gameObject;
            minText = transform.Find("Text_Total/Text_Num").GetComponent<Text>();
            maxText = transform.Find("Text_Total").GetComponent<Text>();
        }

        public void RefreshShow(uint _petId, uint _remodelcount)
        {
            attrname.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(attrid).name);

            petid = _petId;
            remodelcount = _remodelcount;
            GetMaxGrade();
            AddStar();
        }

        private void GetMaxGrade()
        {
            CSVPetNew.Data petnew = CSVPetNew.Instance.GetConfData(petid);
            if(null != petnew)
            {
                if (attrid == 5)
                {
                    maxgrade = petnew.endurance;                    
                }
                else if (attrid == 7)
                {
                    maxgrade = petnew.strength;
                }
                else if (attrid == 9)
                {
                    maxgrade = petnew.strong;
                }
                else if (attrid == 11)
                {
                    maxgrade = petnew.speed;
                }
                else if (attrid == 13)
                {
                    maxgrade = petnew.magic;
                }
                maxgradeLow = petnew.max_lost_gear;
            }
           
        }

        private void AddStar()
        {
            maxText.text = maxgrade.ToString();
            float minValue = maxgrade;
            if (maxgradeLow != 0)
            {
                minValue = maxgrade - Math.Min(Sys_Pet.Instance.MaxGradeLo, maxgradeLow);
                
            }
            minText.text = minValue.ToString() + "/";
            stargo.SetActive(true);
            for (int i = 0; i < stargo.transform.parent.transform.childCount; i++)
            {
                if (i >= 1) GameObject.Destroy(stargo.transform.parent.transform.GetChild(i).gameObject);
            }
            float fullStar = Mathf.Floor((maxgrade + 4) / 10);
            bool haveCut = 0 < (maxgrade % 10) && (maxgrade % 10) < 6;

            for (int i = 0; i < fullStar; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(stargo, stargo.transform.parent);
                go.transform.GetChild(0).gameObject.SetActive(true);
                go.transform.GetChild(0).GetComponent<Image>().fillAmount = 1;
            }
            if (haveCut)
            {
                GameObject go = GameObject.Instantiate<GameObject>(stargo, stargo.transform.parent);
                go.transform.GetChild(0).gameObject.SetActive(true);
                go.transform.GetChild(0).GetComponent<Image>().fillAmount = 0.5f;
            }
            stargo.SetActive(false);
        }
    }

    public class UI_Pet_AttributeReview
    {
        private uint currentPetId;
        private uint remodelCount;
        private List<UI_PetReodel_AttrReview> uI_PetReodel_AttrReviews = new List<UI_PetReodel_AttrReview>();
        private Text maxText;
        private Text minText;
        public void Init(Transform transform)
        {
            uI_PetReodel_AttrReviews.Clear();

            UI_PetReodel_AttrReview vit_Sub = new UI_PetReodel_AttrReview(5);
            vit_Sub.Init(transform.Find("Text_Vit"));
            uI_PetReodel_AttrReviews.Add(vit_Sub);

            UI_PetReodel_AttrReview pow_Sub = new UI_PetReodel_AttrReview(7);
            pow_Sub.Init(transform.Find("Text_Pow"));
            uI_PetReodel_AttrReviews.Add(pow_Sub);

            UI_PetReodel_AttrReview str_Sub = new UI_PetReodel_AttrReview(9);
            str_Sub.Init(transform.Find("Text_Str"));
            uI_PetReodel_AttrReviews.Add(str_Sub);

            UI_PetReodel_AttrReview mp_Sub = new UI_PetReodel_AttrReview(13);
            mp_Sub.Init(transform.Find("Text_Mp"));
            uI_PetReodel_AttrReviews.Add(mp_Sub);

            UI_PetReodel_AttrReview spe_Sub = new UI_PetReodel_AttrReview(11);
            spe_Sub.Init(transform.Find("Text_Spe"));
            uI_PetReodel_AttrReviews.Add(spe_Sub);

            minText = transform.Find("Text_Title/Text_Total/Text_Num").GetComponent<Text>();
            maxText = transform.Find("Text_Title/Text_Total").GetComponent<Text>();
        }

        public void SetData(params object[] arg)
        {
            if (arg.Length >= 2)
            {
                currentPetId = (uint)arg[0];
                remodelCount = System.Convert.ToUInt32(arg[1]);
            }
            RefreshValue();
        }

        public void RefreshValue()
        {
            CSVPetNew.Data petnew = CSVPetNew.Instance.GetConfData(currentPetId);
            int maxgrade = 0;
            if (null != petnew)
            {
                maxgrade = petnew.endurance + petnew.strength + petnew.strong + petnew.speed + petnew.magic;
                maxText.text = maxgrade.ToString();
                minText.text = (maxgrade - petnew.max_lost_gear).ToString() + "/";
            }

            for (int i = 0; i < uI_PetReodel_AttrReviews.Count; i++)
            {
                uI_PetReodel_AttrReviews[i].RefreshShow(currentPetId, remodelCount);
            }
        }
    }

    public class UI_Pet_BookReview_RightView : UIComponent, UI_Pet_Right_Tabs.IListener
    {
        private UI_PetSealInfoView sealInfoView;
        private UI_Pet_BookReview_RightSeal sealView;
        private UI_Pet_BookReview_RightFriend friendView;
        private UI_Pet_BookReview_RightBackGround backView;

        private UI_Pet_Right_Tabs uI_Pet_Right_Tabs;
        private EPetBookPageType ePetReviewPageType;
        private uint currentPetId;
        private Dictionary<int, UIComponent> dictTabPanels = new Dictionary<int, UIComponent>();

        private GameObject loveRedGo;

        protected override void Loaded()
        {
            sealInfoView = new UI_PetSealInfoView();
            sealInfoView.Init(transform.Find("bg"));
            sealView = AddComponent<UI_Pet_BookReview_RightSeal>(transform.Find(" Page01"));
            friendView = AddComponent<UI_Pet_BookReview_RightFriend>(transform.Find(" Page02"));
            backView = AddComponent<UI_Pet_BookReview_RightBackGround>(transform.Find(" Page03"));

            uI_Pet_Right_Tabs = new UI_Pet_Right_Tabs("ListItem", Enum.GetValues(typeof(EPetBookPageType)).Length);
            uI_Pet_Right_Tabs.Init(transform.Find("Menu").gameObject.transform);
            uI_Pet_Right_Tabs.Register(this);
            loveRedGo = transform.Find("Menu/ListItem1/Image_Red").gameObject;

            dictTabPanels.Add((int)EPetBookPageType.Seal, sealView);
            dictTabPanels.Add((int)EPetBookPageType.Friend, friendView);
            dictTabPanels.Add((int)EPetBookPageType.BackGroud, backView);
        }

        public override void SetData(params object[] arg)
        {
            currentPetId = (uint)arg[0];
        }

        public void RefreshData(uint _currentPetId)
        {
            base.Show();
            currentPetId = _currentPetId;
            List<UIComponent> dataList = new List<UIComponent>(dictTabPanels.Values);
            for (int i = 0; i < dataList.Count; i++)
            {
                dataList[i].Hide();
            }
            sealInfoView.SetData(currentPetId);

            if (ePetReviewPageType == EPetBookPageType.Seal)
            {
                sealView.SetData(currentPetId);
                sealView.Show();
            }
            else if(ePetReviewPageType == EPetBookPageType.Friend)
            {
                friendView.SetData(currentPetId);
                friendView.Show();
            }
            else if(ePetReviewPageType == EPetBookPageType.BackGroud)
            {
                backView.SetData(currentPetId);
                backView.Show();
            }
            loveRedGo.SetActive(Sys_Pet.Instance.GetPetBookLoveCanUp(currentPetId));
        }

        public void RefreshStroy(uint stroyId)
        {
            backView.RefreshCeil(stroyId);
        }

        public void OnLoveUpExpUp()
        {
            friendView.RefreshData();
            loveRedGo.SetActive(Sys_Pet.Instance.GetPetBookLoveCanUp(currentPetId));
        }

        public void BackGroundStroyClose()
        {
            backView.SelectState();
        }

        public void ActiveSeal()
        {
            sealView.RefreshNetMessage();
            loveRedGo.SetActive(Sys_Pet.Instance.GetPetBookLoveCanUp(currentPetId));
        }

        public void InitShow(EPetBookPageType initType)
        {
            ePetReviewPageType = initType;
            uI_Pet_Right_Tabs.ShowEx((int)initType);
        }

        public void ShowEx()
        {
            uI_Pet_Right_Tabs.ShowEx((int)ePetReviewPageType);
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void OnClickTabType(int _type)
        {
            ePetReviewPageType = (EPetBookPageType)_type;
            List<int> keyList = new List<int>(dictTabPanels.Keys);
            for (int i = 0; i < keyList.Count; i++)
            {
                int key = keyList[i];
                UIComponent value = dictTabPanels[key];
                if (key == (int)_type)
                {
                    value.SetData(currentPetId);
                    value.Show();
                }
                else
                {
                    value.Hide();
                }
            }
        }

    }
}
