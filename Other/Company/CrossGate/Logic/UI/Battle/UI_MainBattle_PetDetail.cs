using Packet;
using UnityEngine;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine.UI;
using Table;
using Framework;
using Lib.Core;
using static Packet.PetPkAttr.Types;
using System;

namespace Logic
{
    public class UI_MainBattle_PetDetail_Layout
    {
        public Transform transform;
        public RawImage rawImage;
        public Text petName;
        public Text level;
        public Image cardLevel;
        public Image cardQuality;
        public GameObject rareGo;
        public GameObject attrGo;
        public GameObject skillRoot;
        public Button closeBtn;
        public Toggle basicToggle;
        public Toggle remouldToggle;

        public void Init(Transform transform)
        {
            this.transform = transform;
            rawImage = transform.Find("Animator/Detail/Charapter").GetComponent<RawImage>();
            petName = transform.Find("Animator/Detail/Image_Namebg/Text_Name").GetComponent<Text>();
            level = transform.Find("Animator/Detail/Image_Namebg/Text_Level").GetComponent<Text>();
            cardLevel = transform.Find("Animator/Detail/Image_Quality/Text_CardLevel").GetComponent<Image>();
            cardQuality= transform.Find("Animator/Detail/Image_Quality").GetComponent<Image>();
            rareGo = transform.Find("Animator/Detail/Image_Quality/Image_Rare").gameObject;
            attrGo = transform.Find("Animator/Detail/Grid_Attr/Text_Attr01").gameObject;
            skillRoot = transform.Find("Animator/Scroll_View_Skill/Viewport/Content").gameObject;
            closeBtn = transform.Find("Button_Close").GetComponent<Button>();

            basicToggle = transform.Find("Animator/Menu/ListItem").GetComponent<Toggle>();
            remouldToggle = transform.Find("Animator/Menu/ListItem (1)").GetComponent<Toggle>();

        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
            basicToggle.onValueChanged.AddListener(listener.OnbasicToggleValueChanged);
            remouldToggle.onValueChanged.AddListener(listener.OnremouldToggleValueChanged);
        }

        public interface IListener
        {
            void OnbasicToggleValueChanged(bool arg0);
            void OncloseBtnClicked();
            void OnremouldToggleValueChanged(bool arg0);
        }
    }

    public class UI_MainBattle_PetDetail : UIBase, UI_MainBattle_PetDetail_Layout.IListener
    {
        private UI_MainBattle_PetDetail_Layout layout = new UI_MainBattle_PetDetail_Layout();
        private PetUnit petUnit;
        private ClientPet clientPet;
        
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        public AssetDependencies assetDependencies;

        private List<PetSkillCeil> skillGrids = new List<PetSkillCeil>();
        private List<uint> skillIdList = new List<uint>();
        private Dictionary<GameObject, PetSkillCeil> skillCeilGrids = new Dictionary<GameObject, PetSkillCeil>();
        private InfinityGridLayoutGroup infinity;
        private int infinityCount;

        protected override void OnLoaded()
        { 
            layout.Init(transform);
            layout.RegisterEvents(this);
            assetDependencies = transform.GetComponent<AssetDependencies>();
            infinity = layout.skillRoot.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 8;
            infinity.updateChildrenCallback = UpdateChildrenCallback;
            SetSkillItem();
        }

        protected override void OnOpen(object arg)
        {
            clientPet = arg as ClientPet;
            petUnit = clientPet.petUnit;
        }

        protected override void OnShow()
        {
            SetValue();
            SetBasicSkillItem();
        }


        protected override void OnHide()
        {            
            OnDestroyModel();
            DefaultItem();
        }

        private void SetValue()
        {
            layout.petName.text = Sys_Pet.Instance.GetPetName(clientPet);
            layout.level.text = LanguageHelper.GetTextContent(2009441, petUnit.SimpleInfo.Level.ToString());
            ImageHelper.GetPetCardLevel(layout.cardLevel, (int)clientPet.petData.card_lv);
            ImageHelper.SetIcon(layout.cardQuality, Sys_Pet.Instance.SetPetQuality(clientPet.petData.card_type));
            ImageHelper.SetIcon(layout.rareGo.GetComponent<Image>(), Sys_Pet.Instance.GetQuality_ScoreImage(clientPet));
            ImageHelper.SetImageGray(layout.remouldToggle, petUnit.BuildInfo.BuildCount == 0, true);
            layout.remouldToggle.enabled = petUnit.BuildInfo.BuildCount != 0;
            OnCreateModel();
            AddAttr();
        }

        private void AddAttr()
        {
            foreach(var item in clientPet.pkAttrs)
            {
                if(CSVAttr.Instance.TryGetValue((uint)item.Key,out CSVAttr.Data data)&&(data.pet_show_type==2|| data.pet_show_type == 7))
                {
                    long value = item.Value;
                    GameObject go = GameObject.Instantiate<GameObject>(layout.attrGo, layout.attrGo.transform.parent);
                    go.transform.GetComponent<Text>().text = LanguageHelper.GetTextContent(data.name);
                    go.transform.Find("Text_Number").GetComponent<Text>().text = value.ToString();
                }
           
            }
            layout.attrGo.SetActive(false);
        }

        private void SetSkillItem()
        {
            for (int i = 0; i < layout.skillRoot.transform.childCount; i++)
            {
                GameObject go = layout.skillRoot.transform.GetChild(i).gameObject;
                PetSkillCeil petSkillCeil = new PetSkillCeil();

                petSkillCeil.BingGameObject(go);
                petSkillCeil.AddClickListener(OnSkillSelect);
                skillCeilGrids.Add(go, petSkillCeil);
                skillGrids.Add(petSkillCeil);
            }
        }

        private void SetBasicSkillItem()
        {
            skillIdList.Clear();
            infinityCount = 0;
            if (clientPet != null)
            {
                skillIdList = GameCenter.fightControl.GetFightPetSkills(petUnit.Uid);
                infinityCount = clientPet.GetPetSkillGridsCount();
                infinity.SetAmount(infinityCount);
            }
            else
            {
                infinity.SetAmount(infinityCount);
            }
        }

        private void SetRemouldSkillItem()
        {
            skillIdList.Clear();
            infinityCount = 0;
            if (clientPet != null)
            {
                for (int i = 0; i < petUnit.BuildInfo.BuildSkills.Count; i++)
                {
                    skillIdList.Add(petUnit.BuildInfo.BuildSkills[i]);
                }
                infinityCount = clientPet.GetPeBuildtSkillCount();
                infinity.SetAmount(infinityCount);
            }
            else
            {
                infinity.SetAmount(infinityCount);
            }
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= infinityCount)
                return;
            if (skillCeilGrids.ContainsKey(trans.gameObject))
            {
                PetSkillCeil petSkillCeil = skillCeilGrids[trans.gameObject];
                if (index < skillIdList.Count)
                {
                    uint skillId = skillIdList[index];
                    petSkillCeil.SetData(skillId, clientPet.IsUniqueSkill(skillId), clientPet.IsBuildSkill(skillId));
                }
                else
                {
                    petSkillCeil.SetData(0, false, false);
                    trans.gameObject.SetActive(false);
                }
            }
        }

        private void OnSkillSelect(PetSkillCeil petSkillCeil)
        {
            UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(petSkillCeil.petSkillBase.skillId, 0));
        }



        private void DefaultItem()
        {
            FrameworkTool.DestroyChildren(layout.attrGo.transform.parent.gameObject, layout.attrGo.transform.name);
            layout.attrGo.SetActive(true);
        }

        #region Model

        private void OnCreateModel()
        {
            _LoadShowScene();
            _LoadShowModel(petUnit.SimpleInfo.PetId);
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

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                uint weaponId = Constants.UMARMEDID;
                uint highId = petUnit.SimpleInfo.PetId;
                petDisplay.mAnimation.UpdateHoldingAnimations(CSVPetNew.Instance.GetConfData(highId).action_id_show, weaponId);
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
        }
        #endregion

        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_MainBattle_PetDetail);
        }

        public void OnbasicToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                SetBasicSkillItem();
            }
        }

        public void OnremouldToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                SetRemouldSkillItem();
            }
        }
    }
}
