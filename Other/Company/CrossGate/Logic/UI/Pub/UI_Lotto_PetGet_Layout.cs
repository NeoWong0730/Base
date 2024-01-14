using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Framework;
using Lib.Core;
using UnityEngine.EventSystems;

namespace Logic
{
    public partial class UI_Lotto_PetGet_Layout
    {
        public Transform transform;
        public Image card;
        public Text level;
        public Image rare;
        public Text petname;
        public Text grade;
        public GameObject eleicon;
        public RawImage rawImage;
        public Image eventImage;
        public Text grownum;
        public GameObject skillGo;
        public GameObject skillGrid;
        public Button closeBtn;
        public GameObject attrGo;

        private ClickItemGroup<UI_Lotto_Pet_Attr> m_AttrGroup = new ClickItemGroup<UI_Lotto_Pet_Attr>();

       // private Dictionary<GameObject, PetSkillCeil> skillCeilGrids = new Dictionary<GameObject, PetSkillCeil>();
       // private InfinityGridLayoutGroup infinity;

        private IListener m_Listener;

        private Transform mTransPoint;


        private ClickItemGroup<UI_Lotto_Pet_Skill> m_SkillGroup = new ClickItemGroup<UI_Lotto_Pet_Skill>();
        public void Init(Transform transform)
        {
            this.transform = transform;

            card = transform.Find("Animator/Image/Image_Card").GetComponent<Image>();
            level = transform.Find("Animator/Image/Image_Namebg/Text_Level").GetComponent<Text>();

            rare = transform.Find("Animator/Image/Image_Rare").GetComponent<Image>();
            petname = transform.Find("Animator/Image/Image_Namebg/Text_Name").GetComponent<Text>();

            grade = transform.Find("Animator/Image/Image_Point/Text_Comprehensive_Grade/Text_Num").GetComponent<Text>();
            mTransPoint = transform.Find("Animator/Image/Image_Point");


            eleicon = transform.Find("Animator/Image/Image_Attr/Image_Bg").gameObject;
            rawImage = transform.Find("Animator/Texture1").GetComponent<RawImage>();

            eventImage = transform.Find("Animator/EventImage").GetComponent<Image>();
            grownum = transform.Find("Animator/View_Property/Text_Grow/Text_Number").GetComponent<Text>();

            skillGo = transform.Find("Animator/Scroll_View_Skill/Grid/PetSkillItem01").gameObject;
            skillGrid = transform.Find("Animator/Scroll_View_Skill/Grid").gameObject;

            int skillcount = skillGrid.transform.childCount;

            for (int i = 0; i < skillcount; i++)
            {
                m_SkillGroup.AddChild(skillGrid.transform.GetChild(i));
            }


            closeBtn = transform.Find("Black").GetComponent<Button>();

            attrGo = transform.Find("Animator/View_Property/Attr_Grid/Text_Attr").gameObject;
            m_AttrGroup.AddChild(attrGo.transform);

            assetDependencies = transform.GetComponent<AssetDependencies>();


            level.gameObject.SetActive(false);

            var GrowTransObj = transform.Find("Animator/View_Property/Text_Grow").gameObject;

            GrowTransObj.SetActive(false);

            var PointTransObj = transform.Find("Animator/Image/Image_Point").gameObject;
            PointTransObj.SetActive(false);

            var AttrTransObj = transform.Find("Animator/Image/Image_Attr").gameObject;
            AttrTransObj.SetActive(false);

            var CardTransObj = transform.Find("Animator/Image/Image_Card").gameObject;
            CardTransObj.SetActive(false);

            var ImageTransObj = transform.Find("Animator/Image/Image").gameObject;
            ImageTransObj.SetActive(false);
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
            m_Listener = listener;

        }


        public interface IListener
        {
            void OncloseBtnClicked();

            void OnLoadPetModelOver(int part);

            void OnSkillItemUpdate(int index);
        }
    }

    public partial class UI_Lotto_PetGet_Layout
    {
        public void SetPetInfo(CSVPetNew.Data data)
        {
            petname.text = LanguageHelper.GetTextContent(data.name);


            //grade.text = clientpet.petUnit.Power.ToString();
            //level.text = LanguageHelper.GetTextContent(2009330, clientpet.petUnit.Level);

           // rare.gameObject.SetActive(clientpet.petUnit.Rare);
          //  grownum.text = ((float)clientpet.petUnit.Growth / 1000).ToString("0.000");
        }
    }

    public partial class UI_Lotto_PetGet_Layout
    {
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        public AssetDependencies assetDependencies;
        private GameObject model;



        public void ShowPetModel(uint petID)
        {
            _LoadShowScene();
            _LoadShowModel(petID);
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            sceneModel.transform.localPosition = new Vector3(0, 0, 0);
            showSceneControl.Parse(sceneModel);

            //设置RenderTexture纹理到RawImage
            rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);

            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = m_Listener.OnLoadPetModelOver;
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
            showSceneControl.mModelPos.transform.localScale = new Vector3(csvPet.size, csvPet.size, csvPet.size);

        }

        public void OnShowModelLoaded(int part, uint petID)
        {
            if (part == 0)
            {
                uint weaponId = Constants.UMARMEDID;
                uint highId = petID;
                petDisplay.mAnimation.UpdateHoldingAnimations(CSVPetNew.Instance.GetConfData(highId).action_id_show, weaponId);
            }
        }

        public void _UnloadShowContent()
        {
            //设置RenderTexture纹理到RawImage
            rawImage.texture = null;

            //if (petDisplay != null)
            //    petDisplay.Dispose();
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);

            if (showSceneControl != null)
                showSceneControl.Dispose();
        }
    }
    public partial class UI_Lotto_PetGet_Layout
    {
        public void SetPetAttr(uint petID)
        {
            var data = CSVPetNew.Instance.GetConfData(petID);

            m_AttrGroup.SetChildSize(5);

            SetPetAttr(0, data.endurance / 10000.0f,5);
            SetPetAttr(1, data.strength / 10000.0f,7);
            SetPetAttr(2, data.strong / 10000.0f,9);
            SetPetAttr(3, data.speed / 10000.0f,11);
            SetPetAttr(4, data.magic / 10000.0f,13);
        }

        private void SetPetAttr(int index, float value, uint attrID)
        {
            var item = m_AttrGroup.getAt(index);

            if (item == null)
                return;

            item.SetStar(value);

            var data = CSVAttr.Instance.GetConfData(attrID);

            string name = data == null ? string.Empty : LanguageHelper.GetTextContent(data.name);

            item.SetTex(name);
        }
        public class UI_Lotto_Pet_Attr : ClickItem
        {
            private uint attrid;
            private uint petid;
            private float maxgrade;
            private Text attrname;
            private GameObject stargo;

            public override void Load(Transform root)
            {
                base.Load(root);

                attrname = root.Find("Text_Attr").GetComponent<Text>();
                stargo = root.Find("Grid_Grade/Image_GradeBG01").gameObject;
            }

            public override ClickItem Clone()
            {
                return Clone<UI_Lotto_Pet_Attr>(this);
            }

            public void SetStar(float value)
            {
                float count = Mathf.Floor((value + 4) / 10);
                float halfcount = Mathf.Floor((value + 4) / 10 - 1);
                if (value % 10 < 6)
                {
                    //有半星
                    for (int i = 0; i < count; ++i)
                    {
                        GameObject go = GameObject.Instantiate<GameObject>(stargo, stargo.transform.parent);
                        go.transform.GetChild(0).gameObject.SetActive(true);
                        if (i == halfcount)
                        {
                            go.transform.GetChild(0).GetComponent<Image>().fillAmount = 0.5f;
                        }
                        else
                        {
                            go.transform.GetChild(0).GetComponent<Image>().fillAmount = 1;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < halfcount; ++i)
                    {
                        GameObject go = GameObject.Instantiate<GameObject>(stargo, stargo.transform.parent);
                        go.transform.GetChild(0).gameObject.SetActive(true);
                        go.transform.GetChild(0).GetComponent<Image>().fillAmount = 1;
                    }
                }
            }

            public void SetTex(string tex)
            {
                attrname.text = tex;
            }

        }
    }

    public partial class UI_Lotto_PetGet_Layout
    {
        public void SetPetSkill(uint petID)
        {
            var data = CSVPetNew.Instance.GetConfData(petID);

            var skillCount = data.required_skills.Count;

            m_SkillGroup.SetChildSize(skillCount);

            for (int i = 0; i < skillCount; i++)
            {
                SetPetSkillItem(i, data.required_skills[i][0]);
            }
        }

        private void SetPetSkillItem(int index, uint skillID)
        {
             var item = m_SkillGroup.getAt(index);

            if (item == null)
                return;

            item.PetSkill.SetData(skillID, false, false);
        }
        class UI_Lotto_Pet_Skill : ClickItem
        {
            PetSkillCeil petSkillCeil = new PetSkillCeil();

            public PetSkillCeil PetSkill { get { return petSkillCeil; } }
            public override void Load(Transform root)
            {
                base.Load(root);

                petSkillCeil.BingGameObject(root.gameObject);

                petSkillCeil.AddClickListener(OnSkillSelect);
            }

            private void OnSkillSelect(PetSkillCeil petSkillCeil)
            {

            }
            public override ClickItem Clone()
            {
                return Clone<UI_Lotto_Pet_Skill>(this);
            }
        }
    }
}
