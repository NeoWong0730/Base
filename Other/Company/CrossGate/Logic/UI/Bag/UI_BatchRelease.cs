using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public struct BatchRelesaseParam
    {
        public List<uint> selectUids;
        public int[] typeNum;
        public long silverNum;
    }
    public class UI_BatchReleaseIcon : UI_Temple_PetIcon
    {
        private Button selectBtn;
        private GameObject selectGo;
        private Text gear;
        public Action<UI_BatchReleaseIcon> action;
        public bool isSelect;
        protected override void Loaded()
        {
            base.Loaded();
            selectBtn = transform.Find("Btn_Check").GetComponent<Button>();
            selectBtn.onClick.AddListener(OnSelectClicked);
            selectGo = transform.Find("Btn_Check/Checkmark").gameObject;
            gear = transform.Find("Gear/Text").GetComponent<Text>();
        }

        public void SetTempIcon(ClientPet petTempPackUnit, bool select)
        {
            base.SetTempIcon(petTempPackUnit);
            if(null != petTempPackUnit)
            {
                gear.text = (petTempPackUnit.GetPetMaxGradeCount() - petTempPackUnit.GetPetGradeCount()).ToString();
            }
            isSelect = select;
            selectGo.SetActive(isSelect);
        }


        public void SetSelectState(bool select)
        {
            isSelect = select;
            selectGo.SetActive(isSelect);
        }

        public void AddListener(Action<UI_BatchReleaseIcon> action)
        {
           
            this.action = action;
        }

        public void OnSelectClicked()
        {
            if (Sys_Pet.Instance.IsUniquePet(pet.petData.id))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12467));
                return;
            }
            if (!Sys_Pet.Instance.IsPetBeEffectWithSecureLock(pet.petUnit))
            {
                SetSelectState(!isSelect);
                action?.Invoke(this);
            }
        }
    }

    public class UI_BatchRelease : UIBase
    {
        
        private InfinityGrid infinity;
        private Button closeBtn;
        private Button batchReleaseBtn;
        private Dropdown dropdown;
        List<ClientPet> tempBagData = new List<ClientPet>();
        private BatchRelesaseParam batchRelesaseParam;
        private int selectIndex;
        protected override void OnLoaded()
        {
            batchRelesaseParam = new BatchRelesaseParam();
            batchRelesaseParam.selectUids = new List<uint>();
            batchRelesaseParam.typeNum = new int[3];
            closeBtn = transform.Find("Animator/View_TipsBgNew03/Btn_Close").GetComponent<Button>();
            batchReleaseBtn = transform.Find("Animator/View_List/Btn_Confirm").GetComponent<Button>();
            closeBtn.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_BatchRelease);
            });
            batchReleaseBtn.onClick.AddListener(OnBatchReleaseBtnClicked);
            dropdown = transform.Find("Animator/View_List/Dropdown").GetComponent<Dropdown>();
            PopdownListBuild();
            infinity = transform.Find("Animator/View_List/Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;
        }

        private void PopdownListBuild()
        {
            dropdown.ClearOptions();
            dropdown.options.Clear();
            //全部 index 0
            Dropdown.OptionData op = new Dropdown.OptionData
            {
                text = LanguageHelper.GetTextContent(1011004)
            };
            dropdown.options.Add(op);
            //满档 index 1
            Dropdown.OptionData op1 = new Dropdown.OptionData
            {
                text = LanguageHelper.GetTextContent(1011005)
            };
            dropdown.options.Add(op1);
            for (int j = 1; j <= Constants.Max_DeficiencyGear; j++)
            {
                // index 2 - 19
                Dropdown.OptionData op2 = new Dropdown.OptionData
                {
                    text = LanguageHelper.GetTextContent(1011006, j.ToString())
                };
                dropdown.options.Add(op2);
            }
            dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(int index)
        {
            selectIndex = index;
            SendParamClear();
            if (index == 0) // all
            {
                for (int i = 0; i < tempBagData.Count; i++)
                {
                    ClientPet clientPet = tempBagData[i];
                    if (!Sys_Pet.Instance.IsPetBeEffectWithSecureLock(clientPet.petUnit, false) && !Sys_Pet.Instance.IsUniquePet(clientPet.petData.id))
                    {
                        SetParam(clientPet, 1);
                    }
                }

            }
            else
            {
                for (int i = 0; i < tempBagData.Count; i++)
                {
                    ClientPet clientPet = tempBagData[i];
                    uint lowG = clientPet.GetPetMaxGradeCount() - clientPet.GetPetGradeCount();
                    if ((int)lowG > (index - 1) && !Sys_Pet.Instance.IsPetBeEffectWithSecureLock(clientPet.petUnit, false) && !Sys_Pet.Instance.IsUniquePet(clientPet.petData.id))
                    {
                        SetParam(clientPet, 1);
                    }
                }
            }
            infinity.ForceRefreshActiveCell();
        }

        private void SetParam(ClientPet clientPet, int num)
        {
            if(num > 0)
            {
                batchRelesaseParam.selectUids.Add(clientPet.GetPetUid());
                batchRelesaseParam.silverNum += clientPet.abandonCoin;
            }
            else
            {
                batchRelesaseParam.selectUids.Remove(clientPet.GetPetUid());
                batchRelesaseParam.silverNum -= clientPet.abandonCoin;
            }
            batchRelesaseParam.typeNum[(int)(clientPet.petData.card_type - 1)] += num;
            
        }

        private void OnSelectClicked(UI_BatchReleaseIcon uI_BatchReleaseIcon)
        {
            uint uid = uI_BatchReleaseIcon.pet.GetPetUid();
            if (batchRelesaseParam.selectUids.Contains(uid))
            {
                SetParam(uI_BatchReleaseIcon.pet, -1);
            }
            else
            {
                SetParam(uI_BatchReleaseIcon.pet, 1);
            }
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_BatchReleaseIcon entry = AddComponent<UI_BatchReleaseIcon>(go.transform);
            entry.AddListener(OnSelectClicked);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_BatchReleaseIcon entry = cell.mUserData as UI_BatchReleaseIcon;
            if (index < tempBagData.Count)
            {
                ClientPet pet = tempBagData[index];
                entry.SetTempIcon(pet, batchRelesaseParam.selectUids.Contains(pet.GetPetUid()));
            }
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<int>(Sys_Pet.EEvents.OnTemplePetBagChange, OnTemplePetBagChange, toRegister);
        }

        protected override void OnShow()
        {
            RefreshUI();
        }

        protected override void OnShowEnd()
        {
            CSVPetNewParam.Data csv = CSVPetNewParam.Instance.GetConfData(45);
            if (null != csv)
            {
                dropdown.value = Convert.ToInt32(csv.str_value) + 1; // 与实际值index 偏差1
            }
            else
            {
                dropdown.value = 11;
            }
            
        }

        protected override void OnClose()
        {
            selectIndex = 0;
            SendParamClear();
        }

        private void SendParamClear()
        {
            batchRelesaseParam.selectUids.Clear();
            batchRelesaseParam.silverNum = 0;
            for (int i = 0; i < batchRelesaseParam.typeNum.Length; i++)
            {
                batchRelesaseParam.typeNum[i] = 0;
            }
        }

        private void OnTemplePetBagChange(int count)
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            tempBagData = Sys_Pet.Instance.GetTemplePetBagData();
            infinity.CellCount = tempBagData.Count;
            infinity.ForceRefreshActiveCell();
        }

        private void OnBatchReleaseBtnClicked()
        {
            if (batchRelesaseParam.selectUids.Count == 0)
                return;
            StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
            stringBuilder.Append(LanguageHelper.GetTextContent(1011008, batchRelesaseParam.selectUids.Count.ToString()));
            stringBuilder.Append("\n");
            stringBuilder.Append(LanguageHelper.GetTextContent(1011009, batchRelesaseParam.typeNum[0].ToString(), batchRelesaseParam.typeNum[1].ToString(), batchRelesaseParam.typeNum[2].ToString()));
            stringBuilder.Append("\n");
            stringBuilder.Append(LanguageHelper.GetTextContent(1011010, batchRelesaseParam.silverNum.ToString()));
            PromptBoxParameter.Instance.content = StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Pet.Instance.PetTempBagBatchAbandonPetReq(batchRelesaseParam.selectUids);
                CloseSelf();
                UIManager.CloseUI(EUIID.UI_Temple_Storage);
            }, 1011012);
            PromptBoxParameter.Instance.SetCancel(true, null);
            PromptBoxParameter.Instance.SetCountdown(3f, PromptBoxParameter.ECountdown.Confirm, PromptBoxParameter.ECountDownType.SetEnable);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance, EUIID.UI_BatchRelease);
            
        }
    }
}

