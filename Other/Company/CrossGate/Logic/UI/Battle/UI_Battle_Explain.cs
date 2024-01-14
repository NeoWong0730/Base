using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Framework;

namespace Logic
{
    public class UI_Battle_Explain_Layout
    {
        public Transform transform;
        public GameObject levelTipView;
        public GameObject playerMessageView;
        public Button closeBtn;
        public RawImage rawImage;

        public Text title;
        public Text content;
        public Text page;
        public Button lastBtn;
        public Button nextBtn;

        public void Init(Transform transform)
        {
            this.transform = transform;
            levelTipView = transform.Find("Animator/View_StageHint").gameObject;
            playerMessageView = transform.Find("Animator/View_Message").gameObject;
            closeBtn = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            rawImage = transform.Find("Animator/View_StageHint/Content/View_None/Texture").GetComponent<RawImage>();

            title = transform.Find("Animator/View_StageHint/Content/Title/Text").GetComponent<Text>();
            content = transform.Find("Animator/View_StageHint/Content/Scroll_View/Viewport/Content").GetComponent<Text>();
            page = transform.Find("Animator/View_StageHint/Cut_Page/Text").GetComponent<Text>();
            lastBtn = transform.Find("Animator/View_StageHint/Cut_Page/Btn_Left").GetComponent<Button>();
            nextBtn = transform.Find("Animator/View_StageHint/Cut_Page/Btn_Right").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnClose_ButtonClicked);
            lastBtn.onClick.AddListener(listener.OnLast_ButtonClicked);
            nextBtn.onClick.AddListener(listener.OnNext_ButtonClicked);
        }

        public interface IListener
        {
            void OnClose_ButtonClicked();
            void OnLast_ButtonClicked();
            void OnNext_ButtonClicked();
        }
    }

    public class UI_Battle_Explain : UIBase, UI_Battle_Explain_Layout.IListener
    {
        private UI_Battle_Explain_Layout layout = new UI_Battle_Explain_Layout();
        private CSVLevelName.Data csvLevelNameData;
        private CSVMonster.Data csvMonsterData;

        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        private AssetDependencies assetDependencies;
        private GameObject model;
        private int curPage;
        private int totalPage;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            assetDependencies = transform.GetComponent<AssetDependencies>();
        }

        protected override void OnShow()
        {
            curPage = 1;
            uint groupId = Sys_Fight.curMonsterGroupId;          
            if (CSVLevelName.Instance.TryGetValue(groupId, out csvLevelNameData))
            {
                layout.title.text = LanguageHelper.GetTextContent(csvLevelNameData.prompt_title[curPage - 1]);
                layout.content.text = LanguageHelper.GetTextContent(csvLevelNameData.prompt_info[curPage - 1]);
                uint monsterId = csvLevelNameData.prompt_model[curPage - 1];
                CSVMonster.Instance.TryGetValue(monsterId, out csvMonsterData);
                totalPage = csvLevelNameData.prompt_title.Count;
            }

            OnCreateModel();
            layout.page.text = curPage.ToString() + "/" + totalPage.ToString();
        }        

        protected override void OnHide()
        {
            _UnloadShowContent();
        }

        protected override void OnDestroy()
        {
            UIManager.CloseUI(EUIID.UI_Battle_Explain);
        }

        #region 模型展示代码
        private void OnCreateModel()
        {
            if (csvLevelNameData == null)
            {
                return;
            }
            _LoadShowScene();
            _LoadShowModel(curPage-1);
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

        private void _LoadShowModel(int index)
        {
            if (csvMonsterData == null)
            {
                return;
            }
            string _modelPath = csvMonsterData.model;

            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x, showSceneControl.mModelPos.transform.localPosition.y, showSceneControl.mModelPos.transform.localPosition.z);
            showSceneControl.mModelPos.transform.localRotation = Quaternion.Euler(0, csvLevelNameData.rotation_y[curPage - 1], 0);

            float size = csvLevelNameData.model_zoom[index] / 10000.0f;
            showSceneControl.mModelPos.transform.localScale = new Vector3(size, size, size);

        }

        public GameObject modelGo;

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                uint weaponId = Constants.UMARMEDID;
                modelGo = petDisplay.GetPart(EPetModelParts.Main).gameObject;
                modelGo.SetActive(false);
                CSVMonster.Data data = CSVMonster.Instance.GetConfData(csvLevelNameData.prompt_model[curPage-1]);
                petDisplay.mAnimation.UpdateHoldingAnimations(csvMonsterData.monster_id, weaponId, go: modelGo);
            }
        }

        private void _UnloadShowContent()
        {
            //设置RenderTexture纹理到RawImage
            layout.rawImage.texture = null;
            //petDisplay.Dispose();
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl.Dispose();
            modelGo = null;
            petDisplay = null;
        }

        #endregion

        private void SetContent()
        {

        }

        public void OnClose_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Battle_Explain);
        }

        public void OnLast_ButtonClicked()
        {
            if (csvLevelNameData == null)
                return;
            if (curPage == 1)
            {
                curPage = totalPage;
            }
            else
            {
                curPage --;
            }  
            CSVMonster.Instance.TryGetValue(csvLevelNameData.prompt_model[curPage-1], out csvMonsterData);
            layout.page.text = curPage.ToString() + "/" + csvLevelNameData.prompt_title.Count.ToString();
            layout.title.text = LanguageHelper.GetTextContent(csvLevelNameData.prompt_title[curPage - 1]);
            layout.content.text = LanguageHelper.GetTextContent(csvLevelNameData.prompt_info[curPage - 1]);
            _UnloadShowContent();
            OnCreateModel();
        }

        public void OnNext_ButtonClicked()
        {
            if (csvLevelNameData == null)
                return;          
            if (curPage == totalPage)
            {
                curPage = 1;
            }
            else
            {
                curPage++;
            }
            CSVMonster.Instance.TryGetValue(csvLevelNameData.prompt_model[curPage - 1], out csvMonsterData);
            layout.page.text = curPage.ToString() + "/" + csvLevelNameData.prompt_title.Count.ToString();
            layout.title.text = LanguageHelper.GetTextContent(csvLevelNameData.prompt_title[curPage - 1]);
            layout.content.text = LanguageHelper.GetTextContent(csvLevelNameData.prompt_info[curPage - 1]);
            _UnloadShowContent();
            OnCreateModel();
        }
    }
}
