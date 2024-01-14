using Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    sealed public class LvPlay : LevelBase
    {
        //private int nLevelState = 0;
        private int nLoadStage = 0;        
        private float fStartExitMapTime = 0;
        //TODO 写到LvPlay 数据类 GameCenter EPlayState
        //private bool bLoading = true;
        public LvPlayData mLvPlayData = new LvPlayData();

        private void CreateSystems()
        {
            mLvPlayData.OnCreate();

            mLevelSystems = new List<LevelSystemBase>(24);
            mLevelSystemDic = new Dictionary<Type, LevelSystemBase>(24);
            GetOrCreateSystem<ActorSpawnSystem>();
            GetOrCreateSystem<ModelModifySystem>();

            GetOrCreateSystem<PlayerSelfControlSystem>();
            GetOrCreateSystem<PlayerAIControlSystem>();
            GetOrCreateSystem<AnimatorSystem>();
            GetOrCreateSystem<NavigationSystem>();
            GetOrCreateSystem<MovementSystem>();
            GetOrCreateSystem<CameraControlSystem>();
        }

        private void DestroySystems()
        {
            mLvPlayData.OnDestroy();
        }

        public override void OnEnter(LevelParams param, Type fromLevelType)
        {
            base.OnEnter(param, fromLevelType);

            mLevelPreload.PreOpenScene("scene_world", UnityEngine.SceneManagement.LoadSceneMode.Single);
            mLevelPreload.StartLoad();

            CreateSystems();

            Framework.SceneManager.LoadSceneAsync("scene_00101", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }

        private void DistinguishSwitchMap()
        {

        }

        private void OnEnterMap()
        {

        }

        public override float OnLoading()
        {
            int loaded = 0;
            Framework.ESceneState sceneState = Framework.SceneManager.GetSceneState("scene_00101");
            foreach (var scene in Framework.SceneManager.mScenes.Values)
            {
                if (scene.IsDone)
                {
                    ++loaded;
                }
            }
            if(sceneState == Framework.ESceneState.eSuccess)
            {
                return base.OnLoading() * 0.5f + 0.5f;
            }
            else
            {
                return base.OnLoading() * 0.5f;
            }
            

            //return 1f;
        }

        public override void OnLoaded()
        {
            base.OnLoaded();
            
            //UIManager.OpenUI(EUIID.UI_Test);

            mLvPlayData.nPlayerActorID = 1;
            

            for (uint i = 1; i <= 1; i++)
            {
                GetOrCreateSystem<ActorSpawnSystem>().mWaitSpawnActor.Enqueue(new ActorDataFromServer() { uid = i, eActorType = EActorType.Player, pos = new Unity.Mathematics.float3(55, 6, -40) });
            }
        }

        private void OnExcuteCutScene()
        {
        
        }

        private void ShowSceneActors(bool isShow)
        {
            int cullingMask = CameraManager.GetMainCameraCullingMask();

            if (isShow)
            {
                cullingMask |= (int)ELayerMask.Player;
                cullingMask &= ~(int)ELayerMask.Monster;
            }
            else
            {
                cullingMask &= ~(int)ELayerMask.Player;
                cullingMask |= (int)ELayerMask.Monster;
            }

            CameraManager.SetMainCameraCullingMask(cullingMask);
        }

        public override void OnUpdate()
        {
          
        }

        public override void OnExit(Type toLevelType)
        {
            nLoadStage = 0;
            DestroySystems();
        }

#if DEBUG_MODE
/*
        Vector3 templookPoint = new Vector3(14f, -1.1f, -8.52f);

        float duration;
        Vector3 strength = Vector3.zero;
        int vibrato = 10;
        float randomness = 90;
        bool fadeOut = true;

        public override void OnGUI()
        {
            float scale = Screen.height / 720f;
            Rect safeArea = Screen.safeArea;
            Rect HierarchyRect = new Rect(safeArea.x + 390 * scale, safeArea.y, safeArea.width - 780 * scale, safeArea.height);

            //GUILayout.BeginArea(HierarchyRect, GUI.skin.box);
            //mJoystickInput.enabled = GUILayout.Toggle(mJoystickInput.enabled, "虚拟摇杆");
            //Sys_Input.Instance.bForbidTouch = GUILayout.Toggle(Sys_Input.Instance.bForbidTouch, "禁用点击");

            GUILayout.Label(string.Format("Pith(x) = {0}", GameCenter.mCameraController.virtualCamera.pith.ToString()));
            GameCenter.mCameraController.virtualCamera.pith = Mathf.Floor(GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.pith, 0, 360));

            GUILayout.Label(string.Format("Yaw(y) = {0}", GameCenter.mCameraController.virtualCamera.yaw.ToString()));
            GameCenter.mCameraController.virtualCamera.yaw = Mathf.Floor(GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.yaw, 0, 360));

            GUILayout.Label(string.Format("Roll(z) = {0}", GameCenter.mCameraController.virtualCamera.roll.ToString()));
            GameCenter.mCameraController.virtualCamera.roll = Mathf.Floor(GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.roll, 0, 360));

            GUILayout.Label(string.Format("Distance = {0}", GameCenter.mCameraController.virtualCamera.distance.ToString()));
            GameCenter.mCameraController.virtualCamera.distance = GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.distance, 0, 100);

            GUILayout.Label(string.Format("Fov = {0}", GameCenter.mCameraController.virtualCamera.fov.ToString()));
            GameCenter.mCameraController.virtualCamera.fov = GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.fov, 4, 179);

            if (GameCenter.mCameraController.virtualCamera.TargetCamera != null)
            {
                GUILayout.Label(string.Format("Near = {0}", GameCenter.mCameraController.virtualCamera.TargetCamera.nearClipPlane.ToString()));
                GameCenter.mCameraController.virtualCamera.TargetCamera.nearClipPlane = GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.TargetCamera.nearClipPlane, 0.001f, 100f);
            }

            GUILayout.Label(string.Format("Look Target Offset = {0}", GameCenter.mCameraController.virtualCamera.lookPointOffset.ToString()));
            float x = GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.lookPointOffset.x, 0, 10);
            float y = GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.lookPointOffset.y, 0, 10);
            float z = GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.lookPointOffset.z, 0, 10);
            GameCenter.mCameraController.virtualCamera.lookPointOffset = new Vector3(x, y, z);

            templookPoint = GUIExtension.Vector3Field("templookPoint", templookPoint);

            GameCenter.mCameraController.virtualCamera.fixedLookPoint = templookPoint;
            GameCenter.mCameraController.virtualCamera.Recalculation();

            GUILayout.Label(string.Format("Look Target = {0}", GameCenter.mCameraController.virtualCamera.fixedLookPoint.ToString()));

            duration = GUIExtension.FloatField("duration", duration);
            strength = GUIExtension.Vector3Field("strength", strength);
            vibrato = GUIExtension.IntField("vibrato", vibrato);
            randomness = GUIExtension.FloatField("randomness", randomness);
            fadeOut = GUILayout.Toggle(fadeOut, "fadeOut");

            if (GUILayout.Button("震动"))
            {
                GameCenter.mCameraController.DoShake(duration, strength, vibrato, randomness, fadeOut);
            }

            //GUILayout.EndArea();
        }
*/
#endif

    }
}