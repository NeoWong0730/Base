using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Pet_TypeToggle
    {
        private Transform transform;
        private CP_Toggle toggle;
        private Text textShow;
        private Text textLight;
        private Image typeImageShow;
        private Image typeImageLight;
        private GameObject selectObj;
        private Action<UI_Pet_TypeToggle> action;
        public uint currentTypeId;
        public int index;
        private EPetBookType currentType;

        private GameObject redGo;
        public void Init(Transform _transform)
        {
            transform = _transform;
            toggle = transform.gameObject.GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener(OnToggleClick);

            textShow = toggle.transform.Find("Text_Dark").GetComponent<Text>();
            typeImageShow = toggle.transform.Find("Image_Frame/Image_Profession").GetComponent<Image>();

            textLight = toggle.transform.Find("Image_Select/Text_Light").GetComponent<Text>();
            typeImageLight = toggle.transform.Find("Image_Select/Image_Profession").GetComponent<Image>();
            redGo = toggle.transform.Find("Image_Dot").gameObject;
            selectObj = toggle.transform.Find("Image_Select").gameObject;
        }

        private void OnToggleClick(bool _select)
        {
            if (_select)
            {
                action?.Invoke(this);
            }
        }

        public void AddListener(Action<UI_Pet_TypeToggle> _action)
        {
            action = _action;
        }

        public void OnSelect(bool _select)
        {
            toggle.SetSelected(_select, false);
        }

        public void ToggleOff()
        {
            toggle.SetSelected(false, false);
        }

        public void RefreshRedState()
        {
            bool redState = false;
            if (currentType == EPetBookType.Genus)
            {
                redState = Sys_Pet.Instance.GetBookRedState(currentTypeId, 0);
            }
            else if (currentType == EPetBookType.Area)
            {
                redState = Sys_Pet.Instance.GetBookRedState(0, currentTypeId);
            }
            else if (currentType == EPetBookType.Race)
            {
                redState = Sys_Pet.Instance.GetRarytyRedState(currentTypeId);
            }
            else if (currentType == EPetBookType.Mount)
            {
                redState = Sys_Pet.Instance.GetMountRedState(currentTypeId - 1 == 1);
            }
            redGo.SetActive(redState);
        }

        public void SetInfo(uint id, EPetBookType type, int index, bool isSealSetting)
        {
            transform.name = "PetBookList" + id.ToString();
            this.index = index;            
            currentType = type;
            bool redState = false;
            uint imageId = 999; // 无意义id,报错即为配置问题
            if(type == EPetBookType.Genus)
            {
                currentTypeId = id;
                if (id == 0) //all
                {
                    textShow.text = LanguageHelper.GetTextContent(2006003);
                    textLight.text = LanguageHelper.GetTextContent(2006003);
                }else
                {
                    CSVGenus.Data cSVGenusData = CSVGenus.Instance.GetConfData(id);
                    if (null != cSVGenusData)
                    {
                        textShow.text = LanguageHelper.GetTextContent(cSVGenusData.rale_name);
                        textLight.text = LanguageHelper.GetTextContent(cSVGenusData.rale_name);
                        imageId = cSVGenusData.rale_icon;
                    }
                }
                redState = Sys_Pet.Instance.GetBookRedState(currentTypeId, 0);
            }
            else if(type == EPetBookType.Area)
            {
                if (id == 0) //all
                {
                    textShow.text = LanguageHelper.GetTextContent(2006003);
                    textLight.text = LanguageHelper.GetTextContent(2006003);
                }
                else
                {
                    CSVPetNewShowFilter.Data showFilterData = CSVPetNewShowFilter.Instance.GetConfData(id);
                    textShow.text = LanguageHelper.GetTextContent(showFilterData.pet_list);
                    textLight.text = LanguageHelper.GetTextContent(showFilterData.pet_list);
                    currentTypeId = showFilterData.pet_list;
                    imageId = showFilterData.list_pic;
                }
                redState = Sys_Pet.Instance.GetBookRedState(0, currentTypeId);
            }
            else if (type == EPetBookType.Race)
            {
                currentTypeId = id;
                if (id == 0) //all
                {
                    textShow.text = LanguageHelper.GetTextContent(2006003);
                    textLight.text = LanguageHelper.GetTextContent(2006003);
                }
                else
                {
                    CSVPetNewMapRarity.Data rarityData = CSVPetNewMapRarity.Instance.GetConfData(id);
                    if (null != rarityData)
                    {
                        textShow.text = LanguageHelper.GetTextContent(rarityData.BooksRarity);
                        textLight.text = LanguageHelper.GetTextContent(rarityData.BooksRarity);
                        imageId = rarityData.type_pic;
                    }                    
                }
                redState = Sys_Pet.Instance.GetRarytyRedState(currentTypeId); 
            }
            else if (type == EPetBookType.Mount)
            {
                currentTypeId = id;
                if (id == 0) //all
                {
                    textShow.text = LanguageHelper.GetTextContent(2006003);
                    textLight.text = LanguageHelper.GetTextContent(2006003);
                }
                else
                {
                    CSVPetNewIsMount.Data isMountData = CSVPetNewIsMount.Instance.GetConfData(id);
                    if (null != isMountData)
                    {
                        textShow.text = LanguageHelper.GetTextContent(isMountData.BooksRarity);
                        textLight.text = LanguageHelper.GetTextContent(isMountData.BooksRarity);
                        imageId = isMountData.type_pic;
                    }
                }
                redState = Sys_Pet.Instance.GetMountRedState(currentTypeId - 1 == 1);
            }
            ImageHelper.SetIcon(typeImageShow, imageId, true);
            ImageHelper.SetIcon(typeImageLight, imageId, true);
            if (isSealSetting)
            {
                redGo.SetActive(false);
            }
            else
            {
                redGo.SetActive(redState);
            }
        }
    }
}

