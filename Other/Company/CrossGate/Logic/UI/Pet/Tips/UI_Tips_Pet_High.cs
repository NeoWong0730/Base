using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Packet;
using Table;
using Logic.Core;
using Lib.Core;

namespace Logic
{
    public class UI_Tips_Pet_High
    {
        private class HighProp
        {
            private Transform transform;

            private Slider _slider;
            private Text _text;
            private GameObject _prepGo;
            private GameObject _prepGoGroup;

            private uint _attrId;
            private EBaseAttr _attrType;

            private float minWidth = 40;
            private float maxWidth = 195;
            public HighProp(uint id, EBaseAttr attrType)
            {
                _attrId = id;
                _attrType = attrType;
            }

            public void Init(Transform trans)
            {
                transform = trans;

                _slider = transform.Find("Slider").GetComponent<Slider>();
                _text = transform.Find("Text_Percent").GetComponent<Text>();

                _prepGo = transform.Find("Grid_Grade/Image_GradeBG").gameObject;
                _prepGoGroup = transform.Find("Grid_Grade").gameObject;
                transform.gameObject.GetComponent<Text>().raycastTarget = true;
                UI_LongPressButton advance_LongPressButton = transform.gameObject.AddComponent<UI_LongPressButton>();
                advance_LongPressButton.onStartPress.AddListener(OnLongPressed);
                advance_LongPressButton.onRelease.AddListener(OnPointerUp);
            }

            private void OnLongPressed()
            {
                uint lanId = 0u;
                switch(_attrType)
                {
                    case EBaseAttr.Vit:
                        lanId = 2001108u;
                        break;
                    case EBaseAttr.Snh:
                        lanId = 2001109u;
                        break;
                    case EBaseAttr.Inten:
                        lanId = 2001110u;
                        break;
                    case EBaseAttr.Speed:
                        lanId = 2001112u;
                        break;
                    case EBaseAttr.Magic:
                        lanId = 2001111u;
                        break;
                }
                
                if (lanId != 0u)
                    UIManager.OpenUI(EUIID.UI_Pet_Tips01, false, lanId);
            }

            private void OnPointerUp()
            {
                UIManager.CloseUI(EUIID.UI_Pet_Tips01);
            }

            private float GetAdvanceConfigValue(uint id)
            {
                float advanceValue = 0;
                CSVPetNew.Data currentpet = CSVPetNew.Instance.GetConfData(id);
                if (null != currentpet)
                {
                    if (_attrType == EBaseAttr.Vit)
                    {
                        advanceValue = currentpet.endurance;
                    }
                    else if (_attrType == EBaseAttr.Snh)
                    {
                        advanceValue = currentpet.strength;
                    }
                    else if (_attrType == EBaseAttr.Inten)
                    {
                        advanceValue = currentpet.strong;
                    }
                    else if (_attrType == EBaseAttr.Speed)
                    {
                        advanceValue = currentpet.speed;
                    }
                    else if (_attrType == EBaseAttr.Magic)
                    {
                        advanceValue = currentpet.magic;
                    }
                }
                return advanceValue;
            }

            public void SetData(ClientPet clientPet)
            {
                if(null != clientPet)
                {
                    float currentAdvanceValue = GetAdvanceConfigValue(clientPet.petUnit.SimpleInfo.PetId);
                    _text.text = string.Format("{0}/{1}", clientPet.grades[Sys_Pet.Instance.baseAttrs2Id[_attrType]].ToString("0.#"), currentAdvanceValue);
                    DwShow(clientPet.grades[Sys_Pet.Instance.baseAttrs2Id[_attrType]], currentAdvanceValue);
                    _slider.value = 1.0f;
                }
            }

            private void DwShow(float currentD, float petConfigD)
            {
                DefaultPetDw();
                int prep = (int)currentD - (int)petConfigD;
                RectTransform advSliderRect = _slider.GetComponent<RectTransform>();
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
                RectTransform prepGoGrouprect = _prepGoGroup.GetComponent<RectTransform>();
                prepGoGrouprect.anchoredPosition = new Vector2(offset.x + advSliderRect.sizeDelta.x + prepGoGrouprect.sizeDelta.x / 2 + 5, _prepGoGroup.transform.localPosition.y);
                _prepGoGroup.gameObject.SetActive(true);


                if (currentD > 0)
                {
                    if (prep > 0)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 4)
                            {
                                GameObject go = GameObject.Instantiate<GameObject>(_prepGo, _prepGoGroup.transform);
                                Advance_DwSub dwGo = new Advance_DwSub();
                                dwGo.BingGameObject(go);
                                dwGo.SetState(EDwState.Have);
                            }

                        }

                        for (int i = 0; i < prep; i++)
                        {
                            GameObject go = GameObject.Instantiate<GameObject>(_prepGo, _prepGoGroup.transform);
                            Advance_DwSub dwGo = new Advance_DwSub();
                            dwGo.BingGameObject(go);
                            dwGo.SetState(EDwState.Full);
                        }
                    }
                    else
                    {
                        for (int i = 4; i > 0; i--)
                        {
                            GameObject go = GameObject.Instantiate<GameObject>(_prepGo, _prepGoGroup.transform);
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
                }

                _prepGo.SetActive(false);
            }

            void DefaultPetDw()
            {
                _prepGo.SetActive(true);
                for (int i = 0; i < _prepGoGroup.transform.childCount; i++)
                {
                    if (i >= 1) GameObject.Destroy(_prepGoGroup.transform.GetChild(i).gameObject);
                }
            }
        }

        private Transform transform;

        private List<HighProp> _highPropList = new List<HighProp>();
        private Text _textGrowth;
        public void Init(Transform trans)
        {
            transform = trans;
            HighProp vit_Sub = new HighProp(5, EBaseAttr.Vit);
            vit_Sub.Init(transform.Find("Text_Title").gameObject.transform);
            _highPropList.Add(vit_Sub);

            HighProp pow_Sub = new HighProp(7, EBaseAttr.Snh);
            pow_Sub.Init(transform.Find("Text_Title (1)").gameObject.transform);
            _highPropList.Add(pow_Sub);

            HighProp str_Sub = new HighProp(9, EBaseAttr.Inten);
            str_Sub.Init(transform.Find("Text_Title (2)").gameObject.transform);
            _highPropList.Add(str_Sub);

            HighProp mp_Sub = new HighProp(11, EBaseAttr.Magic);
            mp_Sub.Init(transform.Find("Text_Title (3)").gameObject.transform);
            _highPropList.Add(mp_Sub);

            HighProp spe_Sub = new HighProp(13, EBaseAttr.Speed);
            spe_Sub.Init(transform.Find("Text_Title (4)").gameObject.transform);
            _highPropList.Add(spe_Sub);

            _textGrowth = transform.Find("Text_Title (5)/Text_Percent").GetComponent<Text>();
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void SetData(PetUnit pet)
        {
            ClientPet clientPet = new ClientPet(pet);
            for (int i = 0; i < _highPropList.Count; i++)
            {
                _highPropList[i].SetData(clientPet);
            }

            _textGrowth.text = 1.ToString("0.000");   //宠物成长系数取消了  临时处理
        }
    }
}
