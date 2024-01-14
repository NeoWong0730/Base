using UnityEngine;
using Logic.Core;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Framework;
using Table;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Lib.Core;

namespace Logic
{
    public class UI_Pet_BookReview_LeftView : UIComponent
    {
        //private Button friendBtn;
        private Button poinBtn;
        public Image eventImage;
        // 属相属性相关
        private Transform attrParent;
        private GameObject attrGo;
        private Button attrBBtn;
        //品质相关
        private Image cardQuality;
        private Text cardName;
        private Image cardLevel;
        //宠物名称等级
        private Text petName;
        //宠物系别
        private Image cardFamily;
        private Text cardFamilyName;
        private GameObject mountTgaGo;
        private Button showFashionBtn;
        //private GameObject model;

        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        private uint currentId = 0;
        private CSVPetNew.Data CurrentPetData = null;

        bool isShowFashionPreview = false;
        protected override void Loaded()
        {
            eventImage = transform.Find("EventImage").GetComponent<Image>();
            petName = transform.Find("Image_Namebg/Text_Name").GetComponent<Text>();

            cardQuality = transform.Find("Image_Card").GetComponent<Image>();
            cardName = transform.Find("Image_Card/Text_CardName").GetComponent<Text>();
            cardLevel = transform.Find("Image_Card/Text_CardLevel").GetComponent<Image>();

            cardFamily = transform.Find("Image_Type").GetComponent<Image>();
            cardFamilyName = transform.Find("Image_Type/Text_TypeName").GetComponent<Text>();
            attrParent = transform.Find("Image_Attr");
            attrGo = transform.Find("Image_Bg").gameObject;
            mountTgaGo = transform.Find("Image_Mount").gameObject;

            attrBBtn = transform.Find("Button_Attribute").GetComponent<Button>();
            attrBBtn.onClick.AddListener(() => { UIManager.OpenUI(EUIID.UI_Element); });
            poinBtn = transform.Find("Btn_Point").GetComponent<Button>();
            poinBtn.onClick.AddListener(OnPoinBtnClicked);

            showFashionBtn = transform.Find("Btn_MagicCore").GetComponent<Button>();
            showFashionBtn.onClick.AddListener(OnShowFashionBtnClicked);

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);

            Lib.Core.EventTrigger cardClicke = Lib.Core.EventTrigger.Get(cardQuality);
            cardClicke.AddEventListener(EventTriggerType.PointerClick, OnCardClicked);
        }

        private void OnShowFashionBtnClicked()
        {
            isShowFashionPreview = !isShowFashionPreview;
            showFashionBtn.transform.Find("Image_Select").gameObject.SetActive(isShowFashionPreview);
            ResetFashion();
        }

        private void OnPoinBtnClicked()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(10544, true))//宠物功能开启条件
                return;
        }

        public override void Show()
        {
        }

        public override void Hide()
        {
            UnloadModel();
            CurrentPetData = null;
        }

        public override void SetData(params object[] arg)
        {
            currentId = (uint)arg[0];
            CurrentPetData = CSVPetNew.Instance.GetConfData(currentId);
            if (CurrentPetData != null)
            {
                isShowFashionPreview = false;
                showFashionBtn.transform.Find("Image_Select").gameObject.SetActive(isShowFashionPreview);
                UnloadModel();
                _LoadShowScene();
                _LoadShowModel();
                UpdateView();
            }
        }

        public void UnloadModel()
        {
            _UnloadShowContent();
        }

        private void _UnloadShowContent()
        {
            //petDisplay?.Dispose();
            if(null != petDisplay && null != petDisplay.mAnimation)
            {
                petDisplay.mAnimation.StopAll();
            }
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl?.Dispose();
            modelGo = null;
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            sceneModel.transform.localPosition = new Vector3(1000, 0, 0);

            showSceneControl.Parse(sceneModel);            
            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }
        }

        private void _LoadShowModel()
        {
            string _modelPath = CurrentPetData.model_show;
            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            showSceneControl.mModelPos.transform.Rotate(new Vector3(CurrentPetData.angle1, CurrentPetData.angle2, CurrentPetData.angle3));
            showSceneControl.mModelPos.transform.localScale = new Vector3(CurrentPetData.size, CurrentPetData.size, CurrentPetData.size);
            showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x + CurrentPetData.translation, CurrentPetData.height, showSceneControl.mModelPos.transform.localPosition.z);
        }
        public GameObject modelGo;
        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                modelGo = petDisplay.GetPart(EPetModelParts.Main).gameObject;
                modelGo.SetActive(false);
                ResetFashion();
                petDisplay.mAnimation.UpdateHoldingAnimations(CurrentPetData.action_id_show, CurrentPetData.weapon, Constants.PetShowAnimationClipHashSet, go: modelGo);
                ani_index = 0;
                checkAni = true;
            }
        }

        public void ResetFashion()
        {
            uint fashionId = 0;
            if(isShowFashionPreview)
            {
                var configs = CSVPetEquipSuitAppearance.Instance.GetAll();
                for (int i = 0,len = configs.Count; i < len; i++)
                {
                    var config = configs[i];
                    if(config.pet_id.Contains(CurrentPetData.id))
                    {
                        fashionId = config.show_id;
                        break;
                    }
                }
            }
            SceneActor.PetWearSet(Constants.PETWEAR_EQUIP, fashionId, modelGo.transform);
        }


        bool checkAni = false;
        int ani_index = 0;
        protected override void Update()
        {
            if (null != petDisplay && null != petDisplay.mAnimation && checkAni && null != modelGo && modelGo.activeSelf)
            {
                if (petDisplay?.mAnimation.GetClipCount() == Constants.PetShowAnimationClip.Count)
                {
                    checkAni = false;
                    PlayAnimator();
                }
            }
        }

        private void PlayAnimator()
        {
            if (null != petDisplay && Constants.PetShowAnimationClip.Count > ani_index)
            {
                petDisplay.mAnimation?.CrossFade(Constants.PetShowAnimationClip[ani_index], Constants.CORSSFADETIME, CrossFadeEnd);
            }
            else
            {
                petDisplay.mAnimation?.CrossFade((uint)EStateType.Idle, Constants.CORSSFADETIME);
            }
        }

        private void CrossFadeEnd()
        {
            ani_index += 1;
            PlayAnimator();
        }

        void UpdateView()
        {
            petName.text = LanguageHelper.GetTextContent(CurrentPetData.name);
            CSVGenus.Data cSVGenusData = CSVGenus.Instance.GetConfData(CurrentPetData.race);

            if (null != cSVGenusData)
            {
                ImageHelper.SetIcon(cardFamily, cSVGenusData.rale_icon);
                cardFamilyName.text = LanguageHelper.GetTextContent(cSVGenusData.rale_name);      
            }
            showFashionBtn.gameObject.SetActive(CurrentPetData.show_appearance);
            SetMount(CurrentPetData.mount);
            ImageHelper.GetPetCardLevel(cardLevel, CurrentPetData.card_lv);
            cardName.text = Sys_Pet.Instance.CardName(CurrentPetData.card_type);
            ImageHelper.SetIcon(cardQuality, Sys_Pet.Instance.SetPetQuality(CurrentPetData.card_type));
            SetPetPkAttr();
        }

        private void SetMount(bool show)
        {
            mountTgaGo.SetActive(show);
        }

        private void SetPetPkAttr()
        {
            FrameworkTool.DestroyChildren(attrParent.gameObject);
            if(null != CurrentPetData.init_attr)
            {

                for (int i = 0; i < CurrentPetData.init_attr.Count; i++)
                {
                    if(CurrentPetData.init_attr[i].Count >= 2)
                    {
                        CSVAttr.Data cSVAttrData = CSVAttr.Instance.GetConfData(CurrentPetData.init_attr[i][0]);
                        if (null != cSVAttrData && cSVAttrData.attr_type == 3)
                        {
                            GameObject go = GameObject.Instantiate<GameObject>(attrGo, attrParent);
                            ImageHelper.SetIcon(go.transform.Find("Image_Attr").GetComponent<Image>(), cSVAttrData.attr_icon);
                            TextHelper.SetText(go.transform.Find("Image_Attr/Text").GetComponent<Text>(), CurrentPetData.init_attr[i][1].ToString());
                            go.SetActive(true);
                        }
                    }
                }
            }
            
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

        public void OnCardClicked(BaseEventData eventData)
        {
            if(null != CurrentPetData)
                UIManager.OpenUI(EUIID.UI_Card_Tips, false, LanguageHelper.GetTextContent(10999, LanguageHelper.GetTextContent(10999u + CurrentPetData.card_type), CurrentPetData.card_lv.ToString()));
        }
    }
}