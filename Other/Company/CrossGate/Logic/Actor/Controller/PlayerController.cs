using Logic.Core;
using UnityEngine;
using Framework;
using System.Collections.Generic;
#if false
namespace Logic
{
    public class PlayerController : Actor//, Core.IUpdateCmd
    {
        private JoystickData mJoystickData;
        private MovementComponent mMovement;
        private AnimationComponent animationComponent;
        private StateComponent stateComponent;
        private WeaponComponent mWeaponComponent;
        private Hero hero;

        GameObject MoveToFx;
        ParticleSystem[] MoveToFxPS;
        static Vector3 fxOffset = new Vector3(0f, 0.1f, 0f);        
        public void SetTarget(Hero actor)
        {
            hero = actor;

            mMovement = hero.movementComponent;
            stateComponent = hero.stateComponent;
            animationComponent = hero.animationComponent;
            mWeaponComponent = hero.weaponComponent;

            //mMovement = World.GetComponent<MovementComponent>(actor);
            //stateComponent = World.GetComponent<StateComponent>(actor);
            //animationComponent = World.GetComponent<AnimationComponent>(actor);
            //mWeaponComponent = World.GetComponent<WeaponComponent>(actor);            

            //bFollowOther = Sys_Team.Instance.isFollowed(actor.uID);
        }

        protected override void OnConstruct()
        {
            Sys_Input.Instance.onLeftJoystick += OnLeftJoystick;
            Sys_Input.Instance.onInput += OnInput;
            Sys_Input.Instance.onTouchUp += OnTouchUp;
            Sys_Input.Instance.onTouchLongPress += OnTouchLongPress;
            Sys_Input.Instance.onTouchRightUp += OnTouchRightUp;
            Sys_Input.Instance.onTouchLongMove += OnTouchLongMoveByMouse;

            MoveToFx = GameObject.Instantiate<GameObject>(GlobalPreloadAssets.GetAsset<GameObject>(GlobalAssets.sPrefab_Fx_yindao));
            GameObject.DontDestroyOnLoad(MoveToFx);
            MoveToFxPS = MoveToFx.GetComponentsInChildren<ParticleSystem>();
            MoveToFx.gameObject.SetActive(false);

            Sys_Net.Instance.eventEmitter.Handle(Sys_Net.EEvents.OnReconnectStart, OnReconnectStart, true);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnChageEquiped, OnChangeEquipment, true);
        }

        protected override void OnDispose()
        {
            Sys_Input.Instance.onLeftJoystick -= OnLeftJoystick;
            Sys_Input.Instance.onInput -= OnInput;
            Sys_Input.Instance.onTouchUp -= OnTouchUp;
            Sys_Input.Instance.onTouchUp -= OnTouchLongPress;
            Sys_Input.Instance.onTouchRightUp -= OnTouchRightUp;
            Sys_Input.Instance.onTouchLongMove -= OnTouchLongMoveByMouse;

            GameObject.Destroy(MoveToFx);
            
            Sys_Net.Instance.eventEmitter.Handle(Sys_Net.EEvents.OnReconnectStart, OnReconnectStart, false);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnChageEquiped, OnChangeEquipment, false);

            base.OnDispose();
        }

        private void OnTouchUp(Vector2 pos)
        {
            //根据需求选择 RaycastHitAll
            RaycastHit(pos);
        }

        /// <summary>
        /// PC端右键交互（NPC）
        /// </summary>
        /// <param name="pos"></param>
        private void OnTouchRightUp(Vector2 pos)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            int hitlayer = RaycastHitLayer(pos);
            if (LayerMaskUtil.ContainLayerInt(ELayerMask.NPC, hitlayer))
            {
                RaycastHit(pos);
            }
#endif
        }

        private void OnTouchLongMoveByMouse(Vector3 pos)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (bFollowOther|| mMovement==null)
                return;
            Vector3 mouseInput = pos;
            Vector3 playerScreen = CameraManager.mCamera.WorldToScreenPoint(hero.transform.position);
            mouseInput.z = playerScreen.z;
            ActionCtrl.Instance.actionCtrlStatus = ActionCtrl.EActionCtrlStatus.PlayerCtrl;
            mMovement.MoveTo(CameraManager.mCamera.ScreenToWorldPoint(mouseInput));
#endif
        }

        private void OnTouchLongPress(Vector2 pos)
        {
            RaycastHitLongPress(pos);
        }

        void StartMoveTo(Vector3 pos)
        {
            MoveToFx.transform.position = pos + fxOffset;
            MoveToFx.gameObject.SetActive(true);
            foreach (var ps in MoveToFxPS)
            {
                ps.Play();
            }
        }

        void EndMoveTo()
        {
            foreach (var ps in MoveToFxPS)
            {
                ps.Stop();
            }
            MoveToFx.gameObject.SetActive(false);
        }

        private void OnInput(EInputType obj)
        {
            if (obj == EInputType.Jump)
            {
                stateComponent.ChangeState(EStateType.NormalAttack);
            }
        }

        private void OnLeftJoystick(Vector2 dir, float dis)
        {
            if (Sys_Team.Instance.isMainHeroFollowed())
                return;

            if (mMovement == null)
                return;            

            if (dis <= 0)
            {
                mMovement.Stop();
            }
            else
            {
                Vector3 joyDir;
                Transform cameraTransform = CameraManager.mCamera.transform;
                if (Vector3.Dot(cameraTransform.up, Vector3.forward) == 0)
                {
                    joyDir = new Vector3(dir.x, 0, dir.y);
                }
                else
                {
                    joyDir = new Vector3(dir.x, dir.y, 0);
                }

                Vector3 moveDir = cameraTransform.rotation * joyDir;
                Vector2 moveDirV2 = new Vector2(moveDir.x, moveDir.z);
                // moveDirV2.Normalize();--这样写在ILruntime模式下不能被归一

                Vector2 tempVector2 = moveDirV2.normalized;
                mMovement.MoveBy(tempVector2, dis);
            }
        }

        //public void Update()
        //{
        //    if (mMovement == null)
        //        return;
        //
        //    float dis = mJoystickData.dis;
        //
        //    if (dis <= 0)
        //        return;
        //
        //    Vector2 dir = mJoystickData.dir;
        //
        //    Vector3 joyDir;
        //    Transform cameraTransform = CameraManager.mCamera.transform;
        //    if (Vector3.Dot(cameraTransform.up, Vector3.forward) == 0)
        //    {
        //        joyDir = new Vector3(dir.x, 0, dir.y);
        //    }
        //    else
        //    {
        //        joyDir = new Vector3(dir.x, dir.y, 0);
        //    }
        //
        //    Vector3 moveDir = cameraTransform.rotation * joyDir;
        //    Vector2 moveDirV2 = new Vector2(moveDir.x, moveDir.z);
        //   // moveDirV2.Normalize();--这样写在ILruntime模式下不能被归一
        //
        //    Vector2 tempVector2 = moveDirV2.normalized;
        //    mMovement.MoveBy(tempVector2, dis);                                                             
        //}

        public void StopMove()
        {
            mMovement?.Stop();
            Sys_PathFind.Instance.eventEmitter.Trigger<bool>(Sys_PathFind.EEvents.OnPathFind, false);
        }

        private void OnReconnectStart()
        {
            //停止自动任务
            Sys_Task.Instance.StopAutoTask(true);
            Sys_Task.Instance.OnClearExcuteTasks();
        }

        private void OnChangeEquipment()
        {
            mWeaponComponent?.UpdateWeapon(Sys_Equip.Instance.GetCurWeapon(), false);
            hero?.ChangeModel();
        }

        //public void MoveToNpc(uint id)
        //{

        //}
        //public void MoveToNpc(Npc npc)
        //{
        //    Vector3 targetPos = npc.transform.position + npc.transform.forward;
        //    mMovement.MoveTo(targetPos, StartMoveTo, EndMoveTo, null, MovementComponent.defaultAllowDistance);
        //}

#region 射线检测并保留所有结果
        //例：当多个角色重叠时 玩家需要点击后获取 角色列表在UI上 进行精确的选择
        //缓存点中的玩家列表
        List<SceneActor> sceneActors = new List<SceneActor>(8);
        RaycastHit[] rayArray = new RaycastHit[16];
        public void RaycastHitAll(Vector2 pos)
        {
            int layerMask = 0;
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal)
            {
                layerMask = (int)(ELayerMask.Default | ELayerMask.NPC | ELayerMask.OtherActor);
            }
            else if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                layerMask = (int)(ELayerMask.Monster | ELayerMask.Partner | ELayerMask.Player);
            }
            else
            {
                return;
            }

            ELayerMask currentLayerMask = ELayerMask.None;

            Ray ray = CameraManager.mCamera.ScreenPointToRay(pos);
            int count = Physics.RaycastNonAlloc(ray, rayArray, 500f, layerMask);

            //未验证 射线检测是否根据距离排序
            //如果没有排序 则先排序
            for (int i = 0; i < count; ++i)
            {
                RaycastHit hit = rayArray[i];

                //如果有限点击地面则行走
                if ( LayerMaskUtil.ContainLayerInt(ELayerMask.Terrain, hit.collider.gameObject.layer) && currentLayerMask == ELayerMask.None)
                {
                    ActionCtrl.Instance.actionCtrlStatus = ActionCtrl.EActionCtrlStatus.PlayerCtrl;
                    mMovement.MoveTo(hit.point, StartMoveTo, EndMoveTo, null, MovementComponent.defaultAllowDistance);
                    break;
                }

                //点击重叠人群后是否张开列表
                //需要展开 收集所有需要显示的角色
                //不需要展开 直接处理最近点击的角色 则不需要支持射线返回多结果
                if (LayerMaskUtil.ContainLayerInt(ELayerMask.NPC | ELayerMask.OtherActor, hit.collider.gameObject.layer))
                {
                    currentLayerMask = ELayerMask.NPC | ELayerMask.OtherActor;

                    SceneActorWrap actorWrap = hit.collider.gameObject.GetComponent<SceneActorWrap>();
                    if (actorWrap.sceneActor != null)
                    {
                        sceneActors.Add((SceneActor)actorWrap.sceneActor);
                        if (sceneActors.Count >= sceneActors.Capacity)
                        {
                            //最大缓存数量后不再缓存
                            break;
                        }
                    }
                }
            }

            //例：OpenUI(CharLiat) 并传递参数

            for (int i = 0; i < sceneActors.Count; ++i)
            {
                //是否需要额外的点击事件
                //不需要可直接处理点击 后触发事件
                //ClickComponent clickCom = World.GetComponent<ClickComponent>(sceneActors[i]);
                //if (clickCom != null)
                //{
                //    clickCom.OnClick();
                //}

                sceneActors[i].OnClick();
            }

            //数据缓存传递完成后清空
            sceneActors.Clear();
        }
#endregion

        float lastTime = 0;
        //仅需要获取最近点击目标
        public void RaycastHit(Vector2 pos)
        {
            int layerMask = 0;
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal)
            {
                layerMask = (int)(ELayerMask.Default | ELayerMask.Terrain | ELayerMask.NPC | ELayerMask.OtherActor);
            }
            else if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                layerMask = (int)(ELayerMask.Monster | ELayerMask.Partner | ELayerMask.Player);
            }
            else
            {
                return;
            }

            Ray ray = CameraManager.mCamera.ScreenPointToRay(pos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500f, layerMask))
            {
                if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal)
                {
                    if (!Sys_Team.Instance.isMainHeroFollowed() && LayerMaskUtil.ContainLayerInt(ELayerMask.Terrain | ELayerMask.Default, hit.collider.gameObject.layer))
                    {
                        ActionCtrl.Instance.actionCtrlStatus = ActionCtrl.EActionCtrlStatus.PlayerCtrl;
                        mMovement.MoveTo(hit.point, StartMoveTo, EndMoveTo, null, MovementComponent.defaultAllowDistance);

                        Sys_Input.Instance.onTouchTerrain?.Invoke(layerMask);
                    }
                    else if (LayerMaskUtil.ContainLayerInt(ELayerMask.NPC, hit.collider.gameObject.layer))
                    {
                        SceneActorWrap sceneActorWrap = hit.collider.gameObject.GetComponent<SceneActorWrap>();
                        if (sceneActorWrap != null)
                        {
                            //可以尝试直接发送
                            //Sys_Interactive.Instance.eventEmitter.Trigger<InteractiveEvtData>(EInteractiveType.Click, new InteractiveEvtData());

                            //ClickComponent clickCom = World.GetComponent<ClickComponent>(sceneActorWrap.sceneActor);
                            //if (clickCom != null)
                            //{
                            //    clickCom.OnClick();
                            //}

                            sceneActorWrap.sceneActor.OnClick();
                        }
                    }
                    else if (LayerMaskUtil.EqualsLayerInt(ELayerMask.OtherActor, hit.collider.gameObject.layer))
                    {

                    }
                }
                else 
                {
                    SceneActorWrap sceneActorWrap = hit.collider.gameObject.GetComponent<SceneActorWrap>();
                    if (sceneActorWrap != null)
                    {
                        //ClickComponent clickCom = World.GetComponent<ClickComponent>(sceneActorWrap.sceneActor);
                        //DoubleClickComponent douleClickCom = World.GetComponent<DoubleClickComponent>(sceneActorWrap.sceneActor);
                        //if (douleClickCom!=null&&Time.realtimeSinceStartup - lastTime < GameCenter.fightControl.doubleTime)
                        //{
                        //    douleClickCom.OnDouleClick();
                        //}
                        //lastTime = Time.realtimeSinceStartup;
                        //if (clickCom != null)
                        //{
                        //    clickCom.OnClick();
                        //}

                                                
                        if (Time.realtimeSinceStartup - lastTime < GameCenter.fightControl.doubleTime)
                        {
                            sceneActorWrap.sceneActor.OnDoubleClick();
                        }
                        lastTime = Time.realtimeSinceStartup;
                        sceneActorWrap.sceneActor.OnClick();
                    }
                }
            }
        }

        private int RaycastHitLayer(Vector2 pos)
        {
            int layerMask = 0;
            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
                return 0;

            layerMask = (int)(ELayerMask.Default | ELayerMask.Terrain | ELayerMask.NPC | ELayerMask.OtherActor);

            Ray ray = CameraManager.mCamera.ScreenPointToRay(pos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500f, layerMask))
            {
                return hit.collider.gameObject.layer;
            }
            return 0;
        }

        public void RaycastHitLongPress(Vector2 pos)
        {
            int layerMask = 0;
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                layerMask = (int)(ELayerMask.Monster | ELayerMask.Partner | ELayerMask.Player);
            }
            else
            {
                return;
            }

            Ray ray = CameraManager.mCamera.ScreenPointToRay(pos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500f, layerMask))
            {
                SceneActorWrap sceneActorWrap = hit.collider.gameObject.GetComponent<SceneActorWrap>();
                if (sceneActorWrap != null)
                {
                    //LongPressComponent clickCom = World.GetComponent<LongPressComponent>(sceneActorWrap.sceneActor);
                    //if (clickCom != null)
                    //{
                    //    clickCom.OnClick();
                    //}
                    sceneActorWrap.sceneActor.OnClick();
                }
            }
        }*/
    }
}
#endif