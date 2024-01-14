using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Framework;
using Lib.Core;
using UnityEngine.EventSystems;
using static Packet.PetPkAttr.Types;
using System;

namespace Logic
{
    public class UI_Pet_GetCatch : UIBase, UI_Pet_GetMix_Layout.IListener
    {
        private UI_Pet_GetMix_Layout layout = new UI_Pet_GetMix_Layout();
        private List<UI_Pet_Attr> attrlist = new List<UI_Pet_Attr>();
        private List<PetSkillCeil> skillGrids = new List<PetSkillCeil>();
        private List<uint> skillIdList = new List<uint>();
        private List<uint> mountSkillIdList = new List<uint>();
        private InfinityGridLayoutGroup infinity;
        private Dictionary<GameObject, PetSkillCeil> skillCeilGrids = new Dictionary<GameObject, PetSkillCeil>();

        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        public AssetDependencies assetDependencies;

        private ClientPet clientpet;
        private Timer timeclose;

        protected override void OnOpen(object pet)
        {            
            clientpet = (ClientPet)pet;
        }

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            assetDependencies = transform.GetComponent<AssetDependencies>();
            infinity = layout.skillGrid.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 12;
            infinity.updateChildrenCallback = UpdateChildrenCallback;
            SetSkillItem();
        }

        protected override void OnShow()
        {
            OnCreateModel();
            Init();
            DefaultItem();
            AddAttrList();
            skillIdList.Clear();
            skillIdList = clientpet.GetPetSkillList();
            mountSkillIdList = clientpet.GetMountSkill();
            infinity.SetAmount(skillIdList.Count + mountSkillIdList.Count);
            layout.grownum.text = LanguageHelper.GetTextContent(10884, clientpet.GetPetCurrentGradeCount().ToString(), clientpet.GetPetBuildMaxGradeCount().ToString());
            layout.closeBtn.enabled = false;
            timeclose?.Cancel();
            var closeTime = float.Parse(CSVParam.Instance.GetConfData(554).str_value) / 1000;
            timeclose = Timer.Register(closeTime, () =>
            {
                layout.countDownText.gameObject.SetActive(false);
                layout.closeBtn.enabled = true;
            },
            (b) =>
            {
                layout.countDownText.gameObject.SetActive(true);
                layout.countDownText.text = LanguageHelper.GetTextContent(12091, Math.Ceiling(closeTime - b).ToString());
            }, false, true);

            //trigger 宠物融合结束修改
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnNumberChangePet);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(layout.eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
        }


        protected override void OnHide()
        {
            timeclose.Cancel();
            OnDestroyModel();
        }

        private void OnCreateModel()
        {
            _LoadShowScene();
            _LoadShowModel(clientpet.petUnit.SimpleInfo.PetId);
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            sceneModel.transform.localPosition = new Vector3(nID, 0, 0);
            showSceneControl.Parse(sceneModel);

            //设置RenderTexture纹理到RawImage
            layout.rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);

            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }
        }

        private void _LoadShowModel(uint petid)
        {
            CSVPetNew.Data csvPet = CSVPetNew.Instance.GetConfData(petid);
            string _modelPath = csvPet.model_show;

            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x + csvPet.translation, showSceneControl.mModelPos.transform.localPosition.y + csvPet.height, showSceneControl.mModelPos.transform.localPosition.z);
            showSceneControl.mModelPos.transform.localRotation = Quaternion.Euler(csvPet.angle1, csvPet.angle2, csvPet.angle3);
            showSceneControl.mModelPos.transform.localScale = new Vector3((float)csvPet.size, (float)csvPet.size, (float)csvPet.size);

        }
        public GameObject modelGo;
        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                uint weaponId = Constants.UMARMEDID;
                uint highId = clientpet.petUnit.SimpleInfo.PetId;
                modelGo = petDisplay.GetPart(EPetModelParts.Main).gameObject;
                modelGo.SetActive(false);
                petDisplay.mAnimation.UpdateHoldingAnimations(CSVPetNew.Instance.GetConfData(highId).action_id_show, weaponId, go: modelGo);
            }
        }

        private void OnDestroyModel()
        {
            _UnloadShowContent();
        }

        private void _UnloadShowContent()
        {
            //设置RenderTexture纹理到RawImage
            layout.rawImage.texture = null;
            //petDisplay.Dispose();
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl.Dispose();
            modelGo = null;
        }

        public void OnDrag(BaseEventData eventData)
        {
            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }

        public void AddEulerAngles(Vector3 angle)
        {
            Vector3 ilrTemoVector3 = angle;
            if (showSceneControl.mModelPos.transform != null)
            {
                showSceneControl.mModelPos.transform.Rotate(ilrTemoVector3.x, ilrTemoVector3.y, ilrTemoVector3.z);
            }
        }

        private void Init()
        {
            CSVPetNew.Data curCsvData = CSVPetNew.Instance.GetConfData(clientpet.petUnit.SimpleInfo.PetId);
            if (null != curCsvData)
            {
                layout.petname.text = LanguageHelper.GetTextContent(curCsvData.name);
                ImageHelper.SetIcon(layout.card, Sys_Pet.Instance.SetPetQuality(curCsvData.card_type));
                ImageHelper.GetPetCardLevel(layout.cardLevel, (int)curCsvData.card_lv);
                layout.mountGo.SetActive(curCsvData.mount);
                bool isExTimemount = curCsvData.mount && clientpet.petUnit.SimpleInfo.ExpiredTick > 0;
                layout.petTitleGo.SetActive(!isExTimemount);
                layout.mountTitleGo.SetActive(isExTimemount);
            }
            else
            {
                DebugUtil.LogError($"CSVPetNew no find id {clientpet.petUnit.SimpleInfo.PetId.ToString()}");
            }
            layout.grade.text = clientpet.petUnit.SimpleInfo.Score.ToString();
            layout.level.text = LanguageHelper.GetTextContent(2009330, clientpet.petUnit.SimpleInfo.Level.ToString());
            ImageHelper.SetIcon(layout.rare, Sys_Pet.Instance.GetQuality_ScoreImage(clientpet));
            List<AttrPair> attrPairs = clientpet.GetPetEleAttrList();
            int needCount = attrPairs.Count;
            FrameworkTool.CreateChildList(layout.eleicon.transform.parent, needCount);
            for (int i = 0; i < needCount; i++)
            {
                GameObject go = layout.eleicon.transform.parent.transform.GetChild(i).gameObject;
                uint id = attrPairs[i].AttrId;
                ImageHelper.SetIcon(go.transform.Find("Image_Attr").GetComponent<Image>(), CSVAttr.Instance.GetConfData(id).attr_icon);
                go.transform.Find("Image_Attr/Text").GetComponent<Text>().text = attrPairs[i].AttrValue.ToString();
            }
        }

        private void AddAttrList()
        {
            attrlist.Clear();
            List<uint> keyList = new List<uint>(clientpet.grades.Keys);
            for (int i = 0; i < keyList.Count; i++)
            {
                uint key = keyList[i];
                float value = clientpet.grades[key];
                GameObject go = GameObject.Instantiate<GameObject>(layout.attrGo, layout.attrGo.transform.parent);
                UI_Pet_Attr attr = new UI_Pet_Attr(key, clientpet.petUnit.SimpleInfo.PetId, value);
                attr.Init(go.transform);
                attr.RefreshShow();
                attrlist.Add(attr);
            }
            layout.attrGo.SetActive(false);
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= skillIdList.Count + mountSkillIdList.Count)
                return;
            if (skillCeilGrids.ContainsKey(trans.gameObject))
            {
                bool isMountSkill = index >= skillIdList.Count;
                int _index = isMountSkill ? (index - skillIdList.Count) : index;
                PetSkillCeil petSkillCeil = skillCeilGrids[trans.gameObject];
                uint skillid = isMountSkill ? mountSkillIdList[_index] : skillIdList[_index];
                petSkillCeil.SetData(skillid, clientpet.IsUniqueSkill(skillid), clientpet.IsBuildSkill(skillid), isMountSkill: isMountSkill);
            }
        }

        private void SetSkillItem()
        {
            for (int i = 0; i < layout.skillGo.transform.parent.childCount; i++)
            {
                GameObject go = layout.skillGo.transform.parent.GetChild(i).gameObject;
                PetSkillCeil petSkillCeil = new PetSkillCeil();

                petSkillCeil.BingGameObject(go);
                petSkillCeil.AddClickListener(OnSkillSelect);
                skillCeilGrids.Add(go, petSkillCeil);
                skillGrids.Add(petSkillCeil);
            }
        }

        private void OnSkillSelect(PetSkillCeil petSkillCeil)
        {
            if (petSkillCeil.petSkillBase.isMountSkill)
            {
                UI_MountSkill_TipsParam param = new UI_MountSkill_TipsParam();
                param.pet = null;
                param.skillId = petSkillCeil.petSkillBase.skillId;
                UIManager.OpenUI(EUIID.UI_MountSkill_Tips, false, param);
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(petSkillCeil.petSkillBase.skillId, 0));
            }

        }

        private void DefaultItem()
        {
            layout.attrGo.SetActive(true);
            for (int i = 0; i < attrlist.Count; i++)
            {
                attrlist[i].OnDestroy();
            }
            for (int i = 0; i < layout.attrGo.transform.parent.childCount; i++)
            {
                if (i >= 1) GameObject.Destroy(layout.attrGo.transform.parent.GetChild(i).gameObject);
            }
        }

        public void OncloseBtnClicked()
        {
            CloseSelf();
        }
    }
}
