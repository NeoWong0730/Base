using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System.Collections.Generic;
using System;
using Lib.Core;
using UnityEngine.ResourceManagement.AsyncOperations;
using Framework;
using UnityEngine.Playables;
using System.Text;

namespace Logic
{
    public class UI_Pet_Domestication_Layout
    {
        private Button closeBtn;
        private Button domesticationBtn;
        public GameObject petListViewGo;
        public List<PropItem> propItems;
        public Image petIcon;
        public Animator domesticationAni;
        public GameObject rightView;
        public GameObject noneView;
        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            domesticationBtn = transform.Find("Animator/View_Right/Button_Domestication").GetComponent<Button>();
            petListViewGo = transform.Find("Animator/Scroll_View_Pets").gameObject;

            Transform itemGrid = transform.Find("Animator/View_Right/Item_Grid");
            int count = itemGrid.childCount;
            propItems = new List<PropItem>(count);
            for (int i = 0; i < itemGrid.childCount; i++)
            {
                PropItem propItem = new PropItem();
                propItem.BindGameObject(itemGrid.GetChild(i).gameObject);
                propItem.txtName.gameObject.SetActive(true);
                propItems.Add(propItem);
            }

            petIcon = transform.Find("Animator/View_Right/Image_Icon").GetComponent<Image>();

            domesticationAni = transform.Find("Animator").GetComponent<Animator>();
            rightView = transform.Find("Animator/View_Right").gameObject;
            noneView = transform.Find("Animator/View_Npc022").gameObject;
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
            domesticationBtn.onClick.AddListener(listener.OnDomesticationBtnClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
            void OnDomesticationBtnClicked();
        }
    }

    public class UI_Pet_Domestication : UIBase, UI_Pet_Domestication_Layout.IListener
    {
        public class UI_PetDomesticationInfo : UIComponent
        {
            private GameObject noneGO;
            private Transform attrTrans;
            private UI_Pet_MountSkillCeil skill = new UI_Pet_MountSkillCeil();
            protected override void Loaded()
            {
                skill.BingGameObject(transform.Find("View_Info/Skill").gameObject);
                skill.AddClickListener(SkillClicked);
                attrTrans = transform.Find("View_Info/AttrGroup");
                noneGO = transform.Find("View_Info/None").gameObject;
            }

            public void SetPetInfo(ClientPet client)
            {
                if (null != client)
                {
                    CSVPetMount.Data mountData = CSVPetMount.Instance.GetConfData(client.petData.id);
                    if (null != mountData)
                    {
                        bool hasSkill = null != mountData.mount_skills && client.GetUniqueRidingSkillsCount() > 0;
                        noneGO.SetActive(!hasSkill);
                        skill.transform.gameObject.SetActive(hasSkill);
                        if (hasSkill)//预设现在不支持技能多显示
                        {
                            skill.SetData(mountData.mount_skills[0][0], true);
                        }
                        int indentureCount = 0;
                        if (null != mountData.indenture_effect)
                        {
                            indentureCount = mountData.indenture_effect.Count;
                        }
                        int allTextCount = 2 + indentureCount;
                        bool isOpen = Sys_Pet.Instance.MountDomeIsOpen;
                        if (!isOpen)
                        {
                            allTextCount += 1;
                        }
                        FrameworkTool.CreateChildList(attrTrans, allTextCount);
                        for (int i = 0; i < allTextCount; i++)
                        {

                            if (i == 0)
                            {
                                long roleSpeed = Sys_Attr.Instance.GetRoleBaseSpeed();
                                TextHelper.SetText(attrTrans.GetChild(i).GetComponent<Text>(),
                    LanguageHelper.GetTextContent(680000401,
                    LanguageHelper.GetTextContent(2022122u, ((client.GetAttrValueByAttrId(101) + 0f) / roleSpeed * 100.0f).ToString())
                    ), CSVWordStyle.Instance.GetConfData(135u));
                            }
                            else if (i == 1)
                            {
                                TextHelper.SetText(attrTrans.GetChild(i).GetComponent<Text>(), LanguageHelper.GetTextContent(680000402u, mountData.skill_grid.ToString()), CSVWordStyle.Instance.GetConfData(isOpen ? 135u : 136u));
                            }
                            else if (!isOpen && i == allTextCount - 1)
                            {
                                TextHelper.SetText(attrTrans.GetChild(i).GetComponent<Text>(), LanguageHelper.GetTextContent(680000403u, Sys_Pet.Instance.MountDomeOpenLevel.ToString()), CSVWordStyle.Instance.GetConfData(137));
                            }
                            else
                            {
                                uint effect = mountData.indenture_effect[i - 2];
                                uint lowG = client.GetPetMaxGradeCount() - client.GetPetGradeCount();


                                CSVPetMountAttr.Data configData = Sys_Pet.Instance.GetAttrListByGreatAndId((int)effect, lowG);
                                var vs = configData.base_attr;
                                StringBuilder strBuilder = StringBuilderPool.GetTemporary();
                                if (null != vs)
                                {
                                    for (int j = 0; j < vs.Count; j++)
                                    {
                                        CSVAttr.Data attrInfo = CSVAttr.Instance.GetConfData((uint)vs[j][0]);
                                        strBuilder.Append(LanguageHelper.GetTextContent(attrInfo.name));

                                        if (vs[j][0] == 101)
                                        {
                                            long roleSpeed = Sys_Attr.Instance.GetRoleBaseSpeed();

                                            if (vs[j][1] >= 0)
                                            {
                                                strBuilder.Append(LanguageHelper.GetTextContent(2006142u, (attrInfo.show_type == 1 ? vs[j][1].ToString() : (vs[j][1] / 100.0f).ToString("") + "%")));
                                            }
                                            else
                                            {
                                                strBuilder.Append((attrInfo.show_type == 1 ? vs[j][1].ToString() : (vs[j][1] / 100.0f).ToString("") + "%"));
                                            }
                                        }
                                        else
                                        {
                                            if (vs[j][1] >= 0)
                                            {
                                                strBuilder.Append(LanguageHelper.GetTextContent(2006142u, (attrInfo.show_type == 1 ? vs[j][1].ToString() : (vs[j][1] / 100.0f).ToString("") + "%")));
                                            }
                                            else
                                            {
                                                strBuilder.Append((attrInfo.show_type == 1 ? vs[j][1].ToString() : (vs[j][1] / 100.0f).ToString() + "%"));
                                            }
                                        }
                                        if (j != vs.Count - 1)
                                        {
                                            strBuilder.Append("、");
                                        }
                                    }
                                }

                                TextHelper.SetText(attrTrans.GetChild(i).GetComponent<Text>(), LanguageHelper.GetTextContent(680000400, (i - 2 + 1).ToString(), StringBuilderPool.ReleaseTemporaryAndToString(strBuilder)), CSVWordStyle.Instance.GetConfData(isOpen ? 135u : 136u));
                            }

                        }

                    }
                    else
                    {
                        DebugUtil.LogError($"Not find Id= {client.petData.id} in CSVPetMount");
                    }

                }
            }

            private void SkillClicked(UI_Pet_MountSkillCeil skillCeil)
            {
                uint skillId = skillCeil.petSkillBase.skillId;
                if (0 != skillId)
                {
                    if (skillCeil.petSkillBase.isMountSkill)
                    {
                        UI_MountSkill_TipsParam param = new UI_MountSkill_TipsParam();
                        param.pet = null;
                        param.skillId = skillCeil.petSkillBase.skillId;
                        UIManager.OpenUI(EUIID.UI_MountSkill_Tips, false, param);
                    }
                    else
                    {
                        UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
                    }                    
                }
            }
        }

        private UI_Pet_Domestication_Layout layout;
        private UI_Pet_DomesticationList pet_ViewList;
        private UI_PetDomesticationInfo infoView;
        private ClientPet currentChoosePet;
        private List<ItemIdCount> itemIdCounts;

        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;

        //private AsyncOperationHandle<GameObject> requestRef;

        private bool isShowFx = false;

        private PlayableDirector timeLineDir;

        //公用workStream
        private WS_CommunalAIManagerEntity _communalAIManagerEntity;

        protected override void OnLoaded()
        {
            layout = new UI_Pet_Domestication_Layout();
            layout.Init(transform);
            layout.RegisterEvents(this);
            pet_ViewList = AddComponent<UI_Pet_DomesticationList>(layout.petListViewGo.transform);
            infoView = AddComponent<UI_PetDomesticationInfo>(layout.rightView.transform);
            assetDependencies = transform.GetComponent<AssetDependencies>();
            CSVPetNewParam.Data demesticationParam = CSVPetNewParam.Instance.GetConfData(36u);
            if(null != demesticationParam)
            {
                string[] strs = demesticationParam.str_value.Split('|');
                itemIdCounts = new List<ItemIdCount>(strs.Length);
                for (int i = 0; i < strs.Length; i++)
                {
                    string[] strs2 = strs[i].Split('&');
                    if(strs2.Length >= 2)
                    {
                        ItemIdCount item = new ItemIdCount(Convert.ToUInt32(strs2[0]), Convert.ToInt64(strs2[1]));
                        itemIdCounts.Add(item);
                    }
                }
            }
            
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<UI_Pet_Cell>(Sys_Pet.EEvents.OnChoosePetCell, ChoosePetCell, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnPetMountDomestication, OnPetMountDomestication, toRegister);
            
            pet_ViewList.ProcessEvents(toRegister);
        }

        private bool needClose = false;
        private void OnPetMountDomestication(uint uid)
        {
            needClose = true;
            if (!isShowFx)
            {
                isShowFx = true;
                currentChoosePet = Sys_Pet.Instance.GetPetByUId(uid);
                _LoadShowScene();
                _LoadShowModel(currentChoosePet);
            }
        }

        private void FxPlayEnd()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Domestication);
        }

        protected override void OnShow()
        {
            Sys_Pet.Instance.ChangeFightPetIndex();
            if(currentChoosePet == null)
            {
                currentChoosePet = Sys_Pet.Instance.GetFirstDomesticationPet();
            }
            else
            {
                ClientPet tempPet = Sys_Pet.Instance.GetPetByUId(currentChoosePet.GetPetUid());
                if(null == tempPet)
                {
                    currentChoosePet = Sys_Pet.Instance.GetFirstDomesticationPet();
                }
            }
            pet_ViewList.Show();
            UpdateInfo();
            SetItems();
        }

        public void ChoosePetCell(UI_Pet_Cell uI_Pet_Cell)
        {
            if (!uI_Pet_Cell.longState)
            {
                if (uI_Pet_Cell.gridState == EPetGridState.Normal)
                {
                    currentChoosePet = uI_Pet_Cell.pet;
                    UpdateInfo();
                }
            }
        }

        private void UpdateInfo()
        {
            pet_ViewList.UpdateAllgrid();
            if (null != currentChoosePet)
            {
                int index = Sys_Pet.Instance.GetPetListIndexByUid(currentChoosePet.petUnit.Uid);
                if (index != -1)
                    pet_ViewList.SetSelect((uint)index);
            }
            else
            {
                pet_ViewList.SetSelect(0);
            }
            SetPetIcon();
        }

        private void SetItems()
        {
            if(null != itemIdCounts)
            {
                for (int i = 0; i < itemIdCounts.Count; i++)
                {
                    if(i < layout.propItems.Count)
                    {
                        ItemIdCount itemC = itemIdCounts[i];
                        layout.propItems[i].SetData(new PropIconLoader.ShowItemData(itemC.id, itemC.count, 
                            true, false, false, false, false, true, true, true), EUIID.UI_Pet_Domestication);
                        layout.propItems[i].txtName.text = LanguageHelper.GetTextContent(itemC.CSV.name_id);
                    }
                }
            }
        }

        private void SetPetIcon()
        {
            bool hasPet = null != currentChoosePet;
            if (hasPet)
            {
                ImageHelper.SetIcon(layout.petIcon, currentChoosePet.petData.icon_id);
                infoView.SetPetInfo(currentChoosePet);
                /*long roleSpeed = Sys_Attr.Instance.GetRoleBaseSpeed();
                TextHelper.SetText(layout.addText, 
                    LanguageHelper.GetTextContent(680000662, 
                    LanguageHelper.GetTextContent(2022122, ((currentChoosePet.GetAttrValueByAttrId(101) + 0f) / roleSpeed * 100.0f).ToString())
                    ));*/
            }
            layout.noneView.SetActive(!hasPet);
            layout.rightView.SetActive(hasPet);
        }

        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Domestication);
        }

        protected override void OnHide()
        {
            //currentChoosePet = null;
            pet_ViewList.Hide();
            UnloadModel();
            if(needClose)
            {
                UIManager.CloseUI(EUIID.UI_Pet_Domestication);
            }
        }

        private bool CheckItemsIsEnough()
        {
            if (null != itemIdCounts)
            {
                for (int i = 0; i < itemIdCounts.Count; i++)
                {
                    ItemIdCount itemC = itemIdCounts[i];
                    if(!itemC.Enough)
                    {
                        //道具不足
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000653, LanguageHelper.GetTextContent(itemC.CSV.name_id)));
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void OnDomesticationBtnClicked()
        {
            if(null != currentChoosePet)
            {
                bool isDomestication = currentChoosePet.petUnit.SimpleInfo.MountDomestication == 1;
                if (isDomestication)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000651));
                    return;
                }

                if (!currentChoosePet.petData.mount)
                {
                    //只有骑宠才能驯化
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000652));
                    return;
                }

                if(!CheckItemsIsEnough())
                {
                    return;
                }

                Sys_Pet.Instance.OnPetMountdomesticationReq(currentChoosePet.GetPetUid());
            }
        }

        public void UnloadModel()
        {
            _UnloadShowContent();
        }

        private void _UnloadShowContent()
        {
            if (_communalAIManagerEntity != null)
            {
                _communalAIManagerEntity.Dispose();
                _communalAIManagerEntity = null;
            }

            //petDisplay?.Dispose();
            if (null != petDisplay && null != petDisplay.mAnimation)
            {
                petDisplay.mAnimation.StopAll();
            }
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl?.Dispose();
            modelGo = null;
        }

        private void _LoadShowScene()
        {
            transform.Find("Blank").GetComponent<Image>().color = new Color(0, 0, 0, 0);
            transform.Find("Animator").gameObject.SetActive(false);
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            sceneModel.transform.localPosition = new Vector3((int)(EUIID.UI_Pet_Message), 0, 0);

            showSceneControl.Parse(sceneModel);
            timeLineDir = sceneModel.transform.Find("Timeline/Card").GetComponent<PlayableDirector>();
            timeLineDir.stopped += OnOneAnimationEnd;
            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }
        }

        private void OnOneAnimationEnd(PlayableDirector playableDirector)
        {
            FxPlayEnd();
        }

        private void _LoadShowModel(ClientPet clientPet)
        {
            CSVPetNew.Data curCsvData = CSVPetNew.Instance.GetConfData(currentChoosePet.petUnit.SimpleInfo.PetId);
            string _modelPath = curCsvData.model_show;
            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);

            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            showSceneControl.mModelPos.transform.Rotate(new Vector3(curCsvData.angle1, curCsvData.angle2, curCsvData.angle3));
            showSceneControl.mModelPos.transform.localScale = new Vector3(curCsvData.size, curCsvData.size, curCsvData.size);
            showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x + curCsvData.translation, curCsvData.height, showSceneControl.mModelPos.transform.localPosition.z);
        }

        public GameObject modelGo;
        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                CSVPetNew.Data curCsvData = CSVPetNew.Instance.GetConfData(currentChoosePet.petUnit.SimpleInfo.PetId);
                uint highId = currentChoosePet.petUnit.SimpleInfo.PetId;
                modelGo = petDisplay.GetPart(EPetModelParts.Main).gameObject;
                modelGo.SetActive(false);
                SceneActor.PetWearSet(Constants.PETWEAR_EQUIP, currentChoosePet.GetPetSuitFashionId(), modelGo.transform);
                if (_communalAIManagerEntity != null)
                    _communalAIManagerEntity.Dispose();
                _communalAIManagerEntity = WS_CommunalAIManagerEntity.Start(2u, modelGo);

                petDisplay.mAnimation.UpdateHoldingAnimations(CSVPetNew.Instance.GetConfData(highId).action_id_show, curCsvData.weapon, go: modelGo);
                if (null != timeLineDir)
                {
                    timeLineDir.Play();
                }
            }
        }
    }
}