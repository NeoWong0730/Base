using UnityEngine;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine.UI;
using Table;
using Framework;
using Lib.Core;
using System;

namespace Logic
{
    public class UI_ChoosePet_Layout 
    {
        public Transform transform;   
        public GameObject pet_Go01;
        public GameObject pet_Go02;

        public void Init(Transform transform) 
        {
            this.transform = transform;
            pet_Go01 = transform.Find("Animator/Image_BG/Grid_Pet/Item01").gameObject;
            pet_Go02 = transform.Find("Animator/Image_BG/Grid_Pet/Item02").gameObject;
        }
    }

    public class UI_Pet : UIComponent
    {

        private Button chooseButton;
        private GameObject skillGo;
        private GameObject fxselect; 
        public RawImage rawImage_Model;

        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;

        public uint petId;
        public Timer fxtimer;
        private CSVPetNewTemplate.Data csvPet;
        private List<uint> skillList = new List<uint>();
        private List<PetSkillCeil> skillGrids = new List<PetSkillCeil>();

        
        protected override void Loaded()
        {
            chooseButton = transform.Find("Button_Choose").GetComponent<Button>();
            chooseButton.onClick.AddListener(ChooseClick);
            skillGo = transform.Find("Grid_Skill/Image_Bottom").gameObject;
            rawImage_Model = transform.Find("RawImage").GetComponent<RawImage>();
        }

        public void Initialize(uint petId,uint petmodelid,int num)
        {
            this.petId = petId;
            this.csvPet = CSVPetNewTemplate.Instance.GetConfData(petmodelid);
            if (csvPet != null)
            {
                AddSkills();
                _LoadShowScene(num);
                _LoadShowModel(petId);
            }
        }

        private void _LoadShowScene(int num)
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }
            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            if(num==1)
                sceneModel.transform.localPosition = new Vector3(100, 0, 0);
            else
                sceneModel.transform.localPosition = new Vector3(-100, 0, 0);
            showSceneControl.Parse(sceneModel);
            fxselect = sceneModel.transform.Find("bg/Fx_ui_Pet_Select").gameObject;
            rawImage_Model.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }
        }

        private void _LoadShowModel(uint petid)
        {
            CSVPetNew.Data data = CSVPetNew.Instance.GetConfData(petid);
            string _modelPath = data.model_show;
            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x + data.translation, showSceneControl.mModelPos.transform.localPosition.y + data.height, showSceneControl.mModelPos.transform.localPosition.z);
            showSceneControl.mModelPos.transform.localRotation = Quaternion.Euler(data.angle1, data.angle2, data.angle3);
            showSceneControl.mModelPos.transform.localScale = new Vector3((float)data.size, (float)data.size, (float)data.size);
        }

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                uint weaponId = Constants.UMARMEDID;
                uint highId = petId;
                petDisplay.mAnimation.UpdateHoldingAnimations(CSVPetNew .Instance.GetConfData(highId).action_id_show, weaponId);
            }
        }

        public void UnloadModel()
        {
            _UnloadShowContent();
            chooseButton.onClick.RemoveListener(ChooseClick);
        }

        private void _UnloadShowContent()
        {
            rawImage_Model.texture = null;
            // petDisplay.Dispose();
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl.Dispose();
        }

        private void SetSkills()
        {
            skillList.Clear();
            if (csvPet.required_skills != null)
            {
                for (int i = 0; i < csvPet.required_skills.Count; ++i)
                {
                    skillList.Add(csvPet.required_skills[i][0]);
                }
            }
            if (csvPet.unique_skills != null)
            {
                for (int i = 0; i < csvPet.unique_skills.Count; ++i)
                {
                    skillList.Add(csvPet.unique_skills[i][0]);
                }
            }
            if (csvPet.remake_skills != null)
            {
                for (int i = 0; i < csvPet.remake_skills.Count; ++i)
                {
                    skillList.Add(csvPet.remake_skills[i]);
                }
            }
        }

        private void AddSkills()
        {
            SetSkills();
           for(int i=0;i<skillList.Count;++i)
            {
                GameObject go =GameObject.Instantiate<GameObject>(skillGo, skillGo.transform.parent);
                PetSkillCeil petSkillCeil = new PetSkillCeil();
                petSkillCeil.BingGameObject(skillGo);
                petSkillCeil.AddClickListener(OnSkillSelect);

                petSkillCeil.SetData(skillList[i], false,false);
                skillGrids.Add(petSkillCeil);
            }
            for(int i = skillList.Count+1; i <= 4; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(skillGo, skillGo.transform.parent);
                go.transform.Find("PetSkillItem01").gameObject.SetActive(false);
                go.transform.Find("Text_Lv").gameObject.SetActive(false);
            }
            skillGo.SetActive(false);
        }

        private void OnSkillSelect(PetSkillCeil petSkillCeil)
        {
            UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(petSkillCeil.petSkillBase.skillId, 0));
        }

        private void ChooseClick()
        {
            if (fxselect.activeInHierarchy)
            {
                fxselect.SetActive(false);
            }
            fxselect.SetActive(true);
            fxtimer?.Cancel();
            fxtimer = Timer.Register(0.7f, () =>
            {
                Sys_Pet.Instance.OnInitSelectReq(petId);       
            }, null, false, true);
    
        }
    }

    public class UI_ChoosePet : UIBase
    {
        private uint petId;
        private CSVPetNew  csvPet;
        private List<UI_Pet> pets;
        private UI_ChoosePet_Layout layout = new UI_ChoosePet_Layout();

        protected override void OnLoaded()
        {
            pets = new List<UI_Pet>();
            layout.Init(transform);
        }

        protected override void OnShow()
        {
            AddList();
            ClickComponent.GlobalEnableFlag = false;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnChangeStatePet, OnChangeStatePet, toRegister);
        }

        protected override void OnHide()
        {
            ClickComponent.GlobalEnableFlag = true;            
            DefaultPetList();
        }

        private void OnChangeStatePet()
        {
            UIManager.CloseUI(EUIID.UI_ChoosePet);
            CameraManager.mCamera.gameObject.SetActive(true);
        }

        private void AddList()
        {
            string str = CSVParam.Instance.GetConfData(530).str_value;
            string[] strs = str.Split('|');
            
            SetPetMessage(layout.pet_Go01, strs[0],1);
            SetPetMessage(layout.pet_Go02, strs[1],2);
        }

        public void SetPetMessage(GameObject go,string strs,int num)
        {
            UI_Pet pet = AddComponent<UI_Pet>(go.transform);
            pet.assetDependencies = transform.GetComponent<AssetDependencies>();
            string[] strspet = strs.Split('&');
            uint petid = 0;
            uint petmodelid = 0;
            uint.TryParse(strspet[0], out petid);
            uint.TryParse(strspet[1], out petmodelid);
            pet.Initialize(petid, petmodelid,num);
            pets.Add(pet);
        }

        public void DefaultPetList()
        {
            foreach (var pet in pets)
            {
                if (pet.rawImage_Model != null)
                {
                    pet.UnloadModel();
                    pet.fxtimer?.Cancel();
                }
            }
            pets.Clear();
        }
    }
}
