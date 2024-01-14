using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

//#if USE_ADDRESSABLE_ASSET
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Framework;
using Lib.Core;
//#else
//using Lib.AssetLoader;
//#endif


namespace Logic
{
    public class UI_SkillPlay : UIBase
    {
        private Text m_SkillName;
        private Text m_MinSpirit;    //zuidijingchen
        private Text m_Mana_cost;  //haomo
        private Text m_Des;
        private Image m_SkillIcon;
        private RawImage m_RawImage;
        private CSVActiveSkillInfo.Data m_CSVActiveSkillInfoData;
        private Button m_CloseButton;
        private ShowSceneControl _showSceneControl;
        private uint m_SkillInfoId;
        //#if USE_ADDRESSABLE_ASSET
        AsyncOperationHandle<GameObject> mHandle;
        //#else
        //        private AssetRequest _assetRequest;
        //#endif        
        private AttitudeAngleTransform mAttitudeAngleTransform;
        private CanvasScaler m_CanvasScaler;

        protected override void OnLoaded()
        {
            m_SkillName = transform.Find("Animator/View_Left/Text_SkillName").GetComponent<Text>();
            m_SkillIcon = transform.Find("Animator/View_Left/Image_Frame/Image_Skill").GetComponent<Image>();
            m_Mana_cost = transform.Find("Animator/View_Left/Text_Mp/Text_Num").GetComponent<Text>();
            m_MinSpirit = transform.Find("Animator/View_Left/Text_jingshen/Text_Num").GetComponent<Text>();
            m_Des = transform.Find("Animator/View_Left/Text_Tips").GetComponent<Text>();
            m_RawImage = transform.Find("Animator/Texture").GetComponent<RawImage>();
            m_CloseButton = transform.Find("Animator/View_Bg/Btn_Close").GetComponent<Button>();
            m_CanvasScaler = transform.GetComponent<CanvasScaler>();
            m_CloseButton.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_SkillPlay);
            });
            // m_CanvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
        }

        protected override void OnOpen(object arg)
        {
            m_SkillInfoId = (uint)arg;
        }

        protected override void OnShow()
        {
            //UIManager.OpenUI(EUIID.HUD);
            Sys_HUD.Instance.OpenHud();
            UpdateUI();
            _LoadShowScene();
            Sys_HUD.Instance.eventEmitter.Trigger<int, bool>(Sys_HUD.EEvents.OnUpdateHUDMoudles, (int)(EHudMoudle.e_Actor | EHudMoudle.e_Bubble | EHudMoudle.e_Blood), false);
            Sys_HUD.Instance.eventEmitter.Trigger<int>(Sys_HUD.EEvents.OnSetLayer, 10000);
        }

        protected override void OnShowEnd()
        {
            CameraManager.relativePos = m_RawImage.rectTransform.localPosition;
            CanvasScreenMatch canvasScreenMatch = transform.GetComponent<CanvasScreenMatch>();
            if (canvasScreenMatch)
            {
                CanvasScaler canvasScaler = transform.GetComponent<CanvasScaler>();
                float a = canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y;
                float b = (float)Screen.width / (float)Screen.height;
                //如果宽高比 比设计宽高比小 则根据宽度适配
                if (b < a)
                {
                    CameraManager.b_MatchWitchHeight = false;
                }
                else
                {
                    CameraManager.b_MatchWitchHeight = true;
                }
            }
            else
            {
                CameraManager.b_MatchWitchHeight = true;
            }
        }

        private void UpdateUI()
        {
            CSVActiveSkill.Data cSVActiveSkillData = CSVActiveSkill.Instance.GetConfData(CSVActiveSkillInfo.Instance.GetConfData(m_SkillInfoId).skill_show_id);
            TextHelper.SetText(m_SkillName, CSVActiveSkillInfo.Instance.GetConfData(m_SkillInfoId).name);
            TextHelper.SetText(m_Des, Sys_Skill.Instance.GetSkillDesc(m_SkillInfoId));
            TextHelper.SetText(m_Mana_cost, cSVActiveSkillData.mana_cost.ToString());
            TextHelper.SetText(m_MinSpirit, cSVActiveSkillData.min_spirit.ToString());

            CSVActiveSkillInfo.Data cSVActiveSkillBehaviorData = CSVActiveSkillInfo.Instance.GetConfData(m_SkillInfoId);
            if (cSVActiveSkillBehaviorData == null)
            {
                DebugUtil.LogErrorFormat($"没有配置对应技能");
                return;
            }
            uint icon = cSVActiveSkillBehaviorData.icon;
            if (icon == 0)
            {
                DebugUtil.LogErrorFormat($"没有配置对应icon");
                return;
            }
            ImageHelper.SetIcon(m_SkillIcon, icon);
        }

        private void _LoadShowScene()
        {
            if (_showSceneControl == null)
            {
                _showSceneControl = new ShowSceneControl();
            }

            AddressablesUtil.InstantiateAsync(ref mHandle, Constants.SceneModelPath, MHandle_Completed, true, GameCenter.sceneShowRoot.transform);
        }

        private void MHandle_Completed(AsyncOperationHandle<GameObject> handle)
        {
            GameObject sceneModel = handle.Result;
            _showSceneControl.Parse(sceneModel);
            m_RawImage.texture = _showSceneControl.GetTemporary(0, 0, 16, RenderTextureFormat.ARGB32, 1f);

            //mSkillPreViewActor = GameCenter.SkillPreViewWorld.CreateActor<SkillPreViewActor>(_skillInfoId);
            // mSkillPreViewActor.SetRootGameObject(sceneModel);
            Camera skillplayCamera = sceneModel.transform.Find("Camera").GetComponent<Camera>();
            CameraManager.SetSkillPlayCamera(skillplayCamera);
            HandleCamera(skillplayCamera);
        }

        private void HandleCamera(Camera camera)
        {
            mAttitudeAngleTransform = camera.gameObject.GetNeedComponent<AttitudeAngleTransform>();

            camera.gameObject.transform.SetParent(_showSceneControl.mRoot.transform);
            CSVBattleScene.Data cSVBattleSceneData = CSVBattleScene.Instance.GetConfData(999);
            if (cSVBattleSceneData != null)
            {
                mAttitudeAngleTransform.fov = cSVBattleSceneData.fov / 10000f;

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                if (AspectRotioController.Instance.curRatio == Enum_Ratio.Type_1)
                {                  
                    mAttitudeAngleTransform.distance = cSVBattleSceneData.distance[1] / 10000f;
                    mAttitudeAngleTransform.clipFar = cSVBattleSceneData.far[1] / 10000f;
                }
                else
                {
                    mAttitudeAngleTransform.distance = cSVBattleSceneData.distance[0] / 10000f;
                    mAttitudeAngleTransform.clipFar = cSVBattleSceneData.far[0] / 10000f;
                }
#else
                if (Screen.width == 2480 && Screen.height == 2200)
                {
                    mAttitudeAngleTransform.distance = cSVBattleSceneData.distance[2] / 10000f;
                    mAttitudeAngleTransform.clipFar = cSVBattleSceneData.far[2] / 10000f;
                }
                else if (Screen.width == 2208 && Screen.height == 1768)
                {
                    mAttitudeAngleTransform.distance = cSVBattleSceneData.distance[3] / 10000f;
                    mAttitudeAngleTransform.clipFar = cSVBattleSceneData.far[3] / 10000f;
                }
                else
                {
                    mAttitudeAngleTransform.distance = cSVBattleSceneData.distance[0] / 10000f;
                    mAttitudeAngleTransform.clipFar = cSVBattleSceneData.far[0] / 10000f;
                }
#endif
                mAttitudeAngleTransform.pith = cSVBattleSceneData.pith / 10000f;
                mAttitudeAngleTransform.yaw = cSVBattleSceneData.yaw / 10000f;
                mAttitudeAngleTransform.fixedLookPoint = new Vector3(cSVBattleSceneData.battle_scene_pointx / 10000f, cSVBattleSceneData.battle_scene_pointy / 10000f,
                    cSVBattleSceneData.battle_scene_pointz / 10000f);;
                Vector3 offset = new Vector3(cSVBattleSceneData.x_offset / 10000f, cSVBattleSceneData.y_offset / 10000f, cSVBattleSceneData.z_offset / 10000f);
                mAttitudeAngleTransform.lookPointOffset = offset;
            }
            else
            {
                mAttitudeAngleTransform.distance = System.Convert.ToSingle(CSVParam.Instance.GetConfData(173).str_value) / 10000f;
                mAttitudeAngleTransform.fov = System.Convert.ToSingle(CSVParam.Instance.GetConfData(174).str_value) / 10000f;
                mAttitudeAngleTransform.pith = System.Convert.ToSingle(CSVParam.Instance.GetConfData(170).str_value) / 10000f;
                mAttitudeAngleTransform.yaw = System.Convert.ToSingle(CSVParam.Instance.GetConfData(171).str_value) / 10000f;
                mAttitudeAngleTransform.fixedLookPoint = new Vector3(cSVBattleSceneData.battle_scene_pointx / 10000f, cSVBattleSceneData.battle_scene_pointy / 10000f,
                    cSVBattleSceneData.battle_scene_pointz / 10000f);;
                Vector3 offset = new Vector3(System.Convert.ToSingle(CSVParam.Instance.GetConfData(175).str_value) / 10000f, System.Convert.ToSingle(CSVParam.Instance.GetConfData(176).str_value) / 10000f, System.Convert.ToSingle(CSVParam.Instance.GetConfData(177).str_value) / 10000f);
                mAttitudeAngleTransform.lookPointOffset = offset;
            }


            //if (Screen.width / Screen.height > (4 / 3))
            //    {
            //        mAttitudeAngleTransform.distance = cSVBattleSceneData.distance[0] / 10000f;
            //        mAttitudeAngleTransform.clipFar = cSVBattleSceneData.far[0] / 10000f;
            //    }
            //    else
            //    {
            //        mAttitudeAngleTransform.distance = cSVBattleSceneData.distance[1] / 10000f;
            //        mAttitudeAngleTransform.clipFar = cSVBattleSceneData.far[1] / 10000f;
            //    }
            //    mAttitudeAngleTransform.fov = cSVBattleSceneData.fov / 10000f;
            //    mAttitudeAngleTransform.pith = cSVBattleSceneData.pith / 10000f;
            //    mAttitudeAngleTransform.yaw = cSVBattleSceneData.yaw / 10000f;
            //    mAttitudeAngleTransform.fixedLookPoint = new Vector3(cSVBattleSceneData.battle_scene_pointx / 10000f, cSVBattleSceneData.battle_scene_pointy / 10000f,
            //    cSVBattleSceneData.battle_scene_pointz / 10000f);
            //    Vector3 offset = new Vector3(cSVBattleSceneData.x_offset / 10000f, cSVBattleSceneData.y_offset / 10000f, cSVBattleSceneData.z_offset / 10000f);
            //    mAttitudeAngleTransform.lookPointOffset = offset;
            //    mAttitudeAngleTransform.SetLookPoint(mAttitudeAngleTransform.fixedLookPoint);
            //}

            mAttitudeAngleTransform.Recalculation();

            CombatManager.Instance.SetCombatSceneCameraData(mAttitudeAngleTransform.TargetCamera);
        }

        private void _UnloadShowContent()
        {
            m_RawImage.texture = null;
            _showSceneControl.Dispose();
            AddressablesUtil.ReleaseInstance(ref mHandle, MHandle_Completed);
            SkillPreView.Instance.Dispose();
        }

        protected override void OnHide()
        {
            _UnloadShowContent();
            mAttitudeAngleTransform = null;
            CameraManager.SetSkillPlayCamera(null);
            //UIManager.CloseUI(EUIID.HUD);
            Sys_HUD.Instance.CloseHud();
            Sys_HUD.Instance.eventEmitter.Trigger<int, bool>(Sys_HUD.EEvents.OnUpdateHUDMoudles, (int)(EHudMoudle.e_Actor | EHudMoudle.e_Bubble | EHudMoudle.e_Blood), true);
            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnRevertLayer);
        }
    }
}


