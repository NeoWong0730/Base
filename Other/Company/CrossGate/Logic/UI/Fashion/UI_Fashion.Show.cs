using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Framework;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;

namespace Logic
{
    public partial class UI_Fashion : UIBase
    {
        private Image eventImage;
        private GameObject Part_Slider;
        private Slider modelSlider;         //控制模型大小
        private ShowSceneControl showSceneControl;
        private HeroDisplayControl heroDisplay;
        private AssetDependencies assetDependencies;

        private Transform endPos;
        private Vector3 startPos;
        private Vector3 pointInPlane;
        private Vector3 dir;   //方向
        private float offestDelta;
        private Vector3 targetPos;
        private float InitY;
        private float MaxY;
        private float MinY;
        private Timer timer;

        private Dictionary<EHeroModelParts, uint> showParts = new Dictionary<EHeroModelParts, uint>();

        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;

        private AssetsGroupLoader assetsGroupLoader;

        private uint _curUpdateAnimationFlagId;
        private uint _curWeaponFashionModelId;

        private void ResgisterShowEvent()
        {
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnModleDrag);
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnModelClick);

            Lib.Core.EventTrigger eventListener1 = Lib.Core.EventTrigger.Get(compareFirst);
            eventListener1.AddEventListener(EventTriggerType.PointerDown, OnCompareFirstPointerDown);
            eventListener1.AddEventListener(EventTriggerType.PointerUp, OnCompareFirstPointUp);

            Lib.Core.EventTrigger eventListener2 = Lib.Core.EventTrigger.Get(compareLast);
            eventListener2.AddEventListener(EventTriggerType.PointerDown, OnCompareLastPointDown);
            eventListener2.AddEventListener(EventTriggerType.PointerUp, OnCompareLastPointUp);
        }

        private uint GetDressMainPartShowId()
        {
            uint fashionClothesId = Sys_Fashion.Instance.dressedList[0];
            //fashionClothesId = Sys_Fashion.Instance.GetDressedClothesFashionId(Sys_Fashion.Instance.dressedList);
            return fashionClothesId;
        }

        private uint GetDressWeaponPartShowId()
        {
            uint fashionWeaponId = Sys_Fashion.Instance.dressedList[1];
            //fashionWeaponId = Sys_Fashion.Instance.GetDressedWeaponFashionId(Sys_Fashion.Instance.dressedList);
            return fashionWeaponId;
        }

        private List<uint> GetDressAccePartShowId()
        {
            List<uint> acces = new List<uint>();
            //acces = Sys_Fashion.Instance.GetDressedAcceFashionId(Sys_Fashion.Instance.dressedList);\
            for (int i = 2; i < 6; i++)
            {
                if (Sys_Fashion.Instance.dressedList[i] != 0)
                {
                    acces.Add(Sys_Fashion.Instance.dressedList[i]);
                }
            }
            return acces;
        }

        private void SetModelSliderActive(bool _active)
        {
            Part_Slider.SetActive(_active);
            modelSlider.value = 0;
        }

        #region Compare
        private void UpdateCompareLastShowOrHide()
        {
            if (curDyeParentLable == 1)
            {
                compareLast.gameObject.SetActive(curFashionClothes.SchemeCount == 2);
            }
            else if (curDyeParentLable == 2)
            {
                compareLast.gameObject.SetActive(curFashionWeapon.SchemeCount == 2);
            }
            else if (curDyeParentLable == 3)
            {
                compareLast.gameObject.SetActive(curFashionAccessory.SchemeCount == 2);
            }
        }

        private void OnCompareFirstPointerDown(BaseEventData eventData)
        {
            if (curDyeParentLable == 1)
            {
                RevertToFirstColor(EHeroModelParts.Main, curTintIndex);
            }
            if (curDyeParentLable == 2)
            {
                RevertToFirstColor(EHeroModelParts.Weapon, curTintIndex);
            }
            if (curDyeParentLable == 3)
            {
                EHeroModelParts eHeroModelParts = Sys_Fashion.Instance.parts[curFashionAccessory.Id];
                RevertToFirstColor(eHeroModelParts, curTintIndex);
            }
        }

        private void OnCompareFirstPointUp(BaseEventData eventData)
        {
            if (curDyeParentLable == 1)
            {
                Color color = curFashionClothes.GetCurUseColor(curTintIndex);
                SetColor(curTintIndex, color);
            }
            if (curDyeParentLable == 2)
            {
                Color color = curFashionWeapon.GetCurUseColor(curTintIndex);
                SetColor(curTintIndex, color);
            }
            if (curDyeParentLable == 3)
            {
                Color color = curFashionAccessory.GetCurUseColor(curTintIndex);
                SetColor(curTintIndex, color);
            }
        }

        private void OnCompareLastPointDown(BaseEventData eventData)
        {
            if (curDyeParentLable == 1)
            {
                RevertToLastColor(EHeroModelParts.Main, curTintIndex);
            }
            if (curDyeParentLable == 2)
            {
                RevertToLastColor(EHeroModelParts.Weapon, curTintIndex);
            }
            if (curDyeParentLable == 3)
            {
                EHeroModelParts eHeroModelParts = Sys_Fashion.Instance.parts[curFashionAccessory.Id];
                RevertToLastColor(eHeroModelParts, curTintIndex);
            }
        }

        private void OnCompareLastPointUp(BaseEventData eventData)
        {
            if (curDyeParentLable == 1)
            {
                Color color = curFashionClothes.GetCurUseColor(curTintIndex);
                SetColor(curTintIndex, color);
            }
            if (curDyeParentLable == 2)
            {
                Color color = curFashionWeapon.GetCurUseColor(curTintIndex);
                SetColor(curTintIndex, color);
            }
            if (curDyeParentLable == 3)
            {
                Color color = curFashionAccessory.GetCurUseColor(curTintIndex);
                SetColor(curTintIndex, color);
            }
        }

        #endregion


        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }
            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);
            //rawImage.texture = showSceneControl.GetTemporary(0, 0, 16, RenderTextureFormat.ARGB32, 1f);

            if (heroDisplay == null)
            {
                heroDisplay = HeroDisplayControl.Create(true);
                heroDisplay.onLoaded = OnShowModelLoaded;
                heroDisplay.eLayerMask = ELayerMask.ModelShow;
            }

            startPos = showSceneControl.mCamera.transform.position;
            endPos = showSceneControl.mRoot.transform.Find("_pos");
            pointInPlane = endPos.position;
            dir = pointInPlane - startPos;
            InitY = startPos.y;
        }


        private void _UnloadShowContent()
        {
            if (_uiModelShowManagerEntity != null)
            {
                _uiModelShowManagerEntity.Dispose();
                _uiModelShowManagerEntity = null;
            }

            //rawImage.texture = null;
            //heroDisplay.Dispose();
            HeroDisplayControl.Destory(ref heroDisplay);
            showSceneControl.Dispose();
        }

        private void UpdateCameraPos(float dt)
        {
            Vector3 pos = showSceneControl.mCamera.transform.position - startPos;
            float distance = Mathf.Abs(pos.z);
            offestDelta = Mathf.Tan(Mathf.Deg2Rad * showSceneControl.mCamera.fieldOfView / 2) * distance;
            MaxY = InitY + offestDelta;
            MinY = InitY - offestDelta * 0.165f;
            if (distance > 0.05f)
            {
                dir = showSceneControl.mCamera.transform.position - startPos;
            }
            else
            {
                dir = endPos.position - startPos; ;
            }
            pointInPlane = MathUtlilty.GetIntersectWithLineAndPlane(startPos, Vector3.Normalize(dir), endPos.position, -Vector3.forward);
            targetPos = startPos + modelSlider.value * (pointInPlane - startPos);
            showSceneControl.mCamera.transform.position = targetPos;
        }


        private void OnShowModelLoaded(int obj)
        {
            EHeroModelParts eHeroModelParts = (EHeroModelParts)obj;
            if (!showParts.ContainsKey(eHeroModelParts))
                return;

            uint id = showParts[eHeroModelParts];

            UIUpdateAnimation(eHeroModelParts, id);

            switch (eHeroModelParts)
            {
                case EHeroModelParts.Main:
                    DyeClothes(id);
                    break;
                case EHeroModelParts.Weapon:
                    DyeWeapon(id);
                    break;
                case EHeroModelParts.Jewelry_Head:
                case EHeroModelParts.Jewelry_Back:
                case EHeroModelParts.Jewelry_Waist:
                case EHeroModelParts.Jewelry_Face:
                    DyeAcce(id, eHeroModelParts);
                    break;
                default:
                    break;
            }
        }

        protected void UIUpdateAnimation(EHeroModelParts eHeroModelParts)
        {
            uint id;
            if (!showParts.TryGetValue(eHeroModelParts, out id))
                return;

            UIUpdateAnimation(eHeroModelParts, id);
        }

        protected void UIUpdateAnimation(EHeroModelParts eHeroModelParts, uint id)
        {
            //DebugUtil.Log(ELogType.eBag, $"<Color=yellow>_processWeaponType  :{_processWeaponType}  eHeroModelParts:  {eHeroModelParts}</Color>");
            if (b_MainChanged || b_WeaponChanged)
            {
                VirtualGameObject virtualGameObject = heroDisplay.GetPart(0);
                if (virtualGameObject != null)
                {
                    GameObject go = virtualGameObject.gameObject;
                    if (go != null)
                    {
                        uint _charId;
                        if (eHeroModelParts == EHeroModelParts.Main)
                        {
                            _charId = id * 10000u + Sys_Role.Instance.HeroId;
                        }
                        else
                        {
                            uint mainId;
                            if (showParts.TryGetValue(EHeroModelParts.Main, out mainId))
                                _charId = mainId * 10000u + Sys_Role.Instance.HeroId;
                            else
                                _charId = 0;
                        }

                        if (_charId > 0u)
                        {
                            go.SetActive(false);

                            CSVFashionModel.Data cSVFashionModelData = CSVFashionModel.Instance.GetConfData(_charId);
                            if (cSVFashionModelData != null)
                            {
                                uint isUpdateAnimtionsFlagId = cSVFashionModelData.action_show_id * 10u + cSVFashionModelData.show_label;
                                _charId = cSVFashionModelData.action_show_id;
                                uint weaponfashionModeId = 0u;
                                uint equidId;
                                if (showParts.ContainsKey(EHeroModelParts.Weapon))
                                {
                                    uint fashionWeaponId = showParts[EHeroModelParts.Weapon];
                                    CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(Sys_Equip.Instance.GetCurWeapon());
                                    weaponfashionModeId = fashionWeaponId * 10 + cSVEquipmentData.equipment_type;
                                    equidId = CSVFashionWeaponModel.Instance.GetConfData(weaponfashionModeId).equip_id;

                                    if (_uiModelShowManagerEntity != null)
                                        _uiModelShowManagerEntity.DoClearData(_curWeaponFashionModelId != weaponfashionModeId);

                                    LoadDepencyAssets(go, _charId, equidId);

                                    if (_curUpdateAnimationFlagId != isUpdateAnimtionsFlagId || _curWeaponFashionModelId != weaponfashionModeId)
                                    {
                                        DebugUtil.Log(ELogType.eUIModelShowWorkStream, $"<Color=yellow>更新动画:_curUpdateAnimationFlagId:{_curUpdateAnimationFlagId.ToString()}  _charId:{_charId.ToString()}    _curWeaponFashionModelId:{_curWeaponFashionModelId.ToString()}  weaponfashionModeId:{weaponfashionModeId.ToString()}</Color>");
                                        heroDisplay.mAnimation.UpdateHoldingAnimations(_charId, equidId, Constants.UIModelShowAnimationClipHashSet, EStateType.Idle, go);
                                    }
                                }
                                else
                                {
                                    equidId = Constants.UMARMEDID;
                                    weaponfashionModeId = equidId;

                                    if (_uiModelShowManagerEntity != null)
                                        _uiModelShowManagerEntity.DoClearData(_curWeaponFashionModelId != weaponfashionModeId);

                                    LoadDepencyAssets(go, _charId, Constants.UMARMEDID);

                                    if (_curUpdateAnimationFlagId != isUpdateAnimtionsFlagId || _curWeaponFashionModelId > 0u)
                                    {
                                        DebugUtil.Log(ELogType.eUIModelShowWorkStream, $"<Color=yellow>更新动画:_curUpdateAnimationFlagId:{_curUpdateAnimationFlagId.ToString()}  _charId:{_charId.ToString()}    _curWeaponFashionModelId:{_curWeaponFashionModelId.ToString()}  weaponfashionModeId:{weaponfashionModeId.ToString()}</Color>");
                                        heroDisplay.mAnimation.UpdateHoldingAnimations(_charId, Constants.UMARMEDID, Constants.UIModelShowAnimationClipHashSet, EStateType.Idle, go);
                                    }
                                }

                                _curUpdateAnimationFlagId = isUpdateAnimtionsFlagId;
                                _curWeaponFashionModelId = weaponfashionModeId;

                                GameObject weaponGo = null;
                                VirtualGameObject weaponVGO = heroDisplay.GetPart(1);
                                if (weaponVGO != null)
                                    weaponGo = weaponVGO.gameObject;
                                
                                VirtualGameObject weapon02VGO = heroDisplay.GetOtherWeapon();

                                SetPosEulerAngles(0f);
                                if (_uiModelShowManagerEntity == null)
                                    _uiModelShowManagerEntity = WS_UIModelShowManagerEntity.Start(cSVFashionModelData.ui_show_workID, null, heroDisplay.mAnimation, go, equidId, weaponGo, weapon02VGO);
                                else
                                    _uiModelShowManagerEntity = _uiModelShowManagerEntity.StartWork(cSVFashionModelData.ui_show_workID, null, heroDisplay.mAnimation, go, equidId, weaponGo, weapon02VGO);
                            }
                            else
                            {
                                DebugUtil.LogErrorFormat($"找不到时装模型配置{_charId}");
                            }
                        }
                    }
                }
            }
        }

        protected void LoadDepencyAssets(GameObject go, uint heroId, uint weaponId)
        {
            assetsGroupLoader = new AssetsGroupLoader();
            List<string> animationPaths;
            AnimationComponent.GetAnimationPaths(heroId, weaponId, out animationPaths, Constants.IdleAnimationClipHashSet);
            if (animationPaths != null)
            {
                for (int index = 0, len = animationPaths.Count; index < len; index++)
                {
                    assetsGroupLoader.AddLoadTask(animationPaths[index]);
                }
            }
            else
            {
                DebugUtil.LogError($"animationPaths is null heroID: {heroId}");
            }

            if (assetsGroupLoader != null)
            {
                assetsGroupLoader.StartLoad(null, () =>
                {
                }, null);
            }
        }

        private void _LoadShowModel()
        {
            if (showParts.ContainsKey(EHeroModelParts.Main))
            {
                uint mainPartId = showParts[EHeroModelParts.Main];
                LoadModelParts(mainPartId, EHeroModelParts.Main);
            }
            if (showParts.ContainsKey(EHeroModelParts.Weapon))
            {
                uint weaponPartId = showParts[EHeroModelParts.Weapon];
                LoadModelParts(weaponPartId, EHeroModelParts.Weapon);
            }
            if (showParts.ContainsKey(EHeroModelParts.Jewelry_Waist))
            {
                uint Jewelry_WaistPartId = showParts[EHeroModelParts.Jewelry_Waist];
                LoadModelParts(Jewelry_WaistPartId, EHeroModelParts.Jewelry_Waist);
            }
            if (showParts.ContainsKey(EHeroModelParts.Jewelry_Face))
            {
                uint Jewelry_FacePartId = showParts[EHeroModelParts.Jewelry_Face];
                LoadModelParts(Jewelry_FacePartId, EHeroModelParts.Jewelry_Face);
            }
        }

        private void RevertModelShow()
        {
            LoadModelParts(curMainPartShowId, EHeroModelParts.Main);
        }

        private void UnloadWearParts()
        {
            List<EHeroModelParts> parts = new List<EHeroModelParts>();
            foreach (var item in showParts)
            {
                if (!(item.Key == EHeroModelParts.Main))
                {
                    UnLoadModelParts(item.Value, item.Key);
                    parts.Add(item.Key);
                }
            }
            foreach (var item in parts)
            {
                showParts.Remove(item);

                if (item == EHeroModelParts.Weapon)
                {
                    UIUpdateAnimation(EHeroModelParts.Main);
                }
            }
        }

        private void LoadModelParts(uint id, EHeroModelParts eHeroModelParts)
        {
            if (eHeroModelParts == EHeroModelParts.Main)
            {
                uint clothesfashionModelId = id * 10000 + Sys_Role.Instance.HeroId;
                CSVFashionModel.Data cSVFashionModelData = CSVFashionModel.Instance.GetConfData(clothesfashionModelId);
                if (cSVFashionModelData != null)
                {
                    string modelPath = cSVFashionModelData.model_show;
                    heroDisplay.LoadMainModel(EHeroModelParts.Main, modelPath, EHeroModelParts.None, null);
                    heroDisplay.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
                    curMainPartShowId = id;
                }
            }
            if (eHeroModelParts == EHeroModelParts.Weapon)
            {
                var csvFashtionWeaponData = Sys_Fashion.Instance.GetWeaponModelData(id, Sys_Equip.Instance.GetCurWeapon());
                if (csvFashtionWeaponData != null)
                {
                    heroDisplay.LoadWeaponModel(csvFashtionWeaponData, true);
                }
            }
            if (eHeroModelParts == EHeroModelParts.Jewelry_Back || eHeroModelParts == EHeroModelParts.Jewelry_Face
                || eHeroModelParts == EHeroModelParts.Jewelry_Head || eHeroModelParts == EHeroModelParts.Jewelry_Waist)
            {
                CSVFashionAccessory.Data cSVFashionAccessoryData = CSVFashionAccessory.Instance.GetConfData(id);
                if (cSVFashionAccessoryData != null)
                {
                    string modelPath = cSVFashionAccessoryData.model_show;
                    string socketName = cSVFashionAccessoryData.AccSlot;
                    heroDisplay.LoadMainModel(eHeroModelParts, modelPath, EHeroModelParts.Main, socketName);
                }
            }
            showParts[eHeroModelParts] = id;
        }

        private void UnLoadModelParts(uint id, EHeroModelParts eHeroModelParts)
        {
            heroDisplay.LoadMainModel(eHeroModelParts, null, EHeroModelParts.Main, null);
        }

        private void RevertToFirstColor(EHeroModelParts eHeroModelParts, ETintIndex eTintIndex)
        {
            if (eHeroModelParts == EHeroModelParts.Main)
            {
                Color color_r = curFashionClothes.GetFirstColor(eTintIndex);
                SetColor(eHeroModelParts, eTintIndex, color_r);
            }
            if (eHeroModelParts == EHeroModelParts.Weapon)
            {
                Color color_r = curFashionWeapon.GetFirstColor(eTintIndex);
                SetColor(eHeroModelParts, eTintIndex, color_r);
            }
            if (eHeroModelParts == EHeroModelParts.Jewelry_Back || eHeroModelParts == EHeroModelParts.Jewelry_Face
                    || eHeroModelParts == EHeroModelParts.Jewelry_Head || eHeroModelParts == EHeroModelParts.Jewelry_Waist)
            {
                Color color_r = curFashionAccessory.GetFirstColor(eTintIndex);
                SetColor(eHeroModelParts, eTintIndex, color_r);
            }
        }

        //对比上次
        private void RevertToLastColor(EHeroModelParts eHeroModelParts, ETintIndex eTintIndex)
        {
            if (eHeroModelParts == EHeroModelParts.Main)
            {
                Color color_r = curFashionClothes.GetNextSchemeColor(eTintIndex);
                SetColor(eHeroModelParts, eTintIndex, color_r);
            }
            if (eHeroModelParts == EHeroModelParts.Weapon)
            {
                Color color_r = curFashionWeapon.GetNextSchemeColor(eTintIndex);
                SetColor(eHeroModelParts, eTintIndex, color_r);
            }
            if (eHeroModelParts == EHeroModelParts.Jewelry_Back || eHeroModelParts == EHeroModelParts.Jewelry_Face
                    || eHeroModelParts == EHeroModelParts.Jewelry_Head || eHeroModelParts == EHeroModelParts.Jewelry_Waist)
            {
                Color color_r = curFashionAccessory.GetNextSchemeColor(eTintIndex);
                SetColor(eHeroModelParts, eTintIndex, color_r);
            }
        }

        private void RevertToFirstColor(uint fashionId)
        {
            EHeroModelParts eHeroModelParts = EHeroModelParts.None;
            if (curFashionType == 1)
            {
                if (curFashionClothes.Id != fashionId)
                {
                    return;
                }
                Color color_r = curFashionClothes.GetFirstColor(ETintIndex.R);
                SetColor(EHeroModelParts.Main, ETintIndex.R, color_r);
                Color color_g = curFashionClothes.GetFirstColor(ETintIndex.G);
                SetColor(EHeroModelParts.Main, ETintIndex.G, color_g);
                Color color_b = curFashionClothes.GetFirstColor(ETintIndex.B);
                SetColor(EHeroModelParts.Main, ETintIndex.B, color_b);
                Color color_a = curFashionClothes.GetFirstColor(ETintIndex.A);
                SetColor(EHeroModelParts.Main, ETintIndex.A, color_a);
            }
            if (curFashionType == 2)
            {
                if (curFashionWeapon.Id != fashionId)
                {
                    return;
                }
                Color color_r = curFashionWeapon.GetFirstColor(ETintIndex.R);
                SetColor(EHeroModelParts.Weapon, ETintIndex.R, color_r);
            }
            if (curFashionType == 3)
            {
                if (curFashionAccessory.Id != fashionId)
                {
                    return;
                }
                Color color_r = curFashionAccessory.GetFirstColor(ETintIndex.R);
                eHeroModelParts = Sys_Fashion.Instance.parts[curFashionAccessory.Id];
                SetColor(eHeroModelParts, ETintIndex.R, color_r);
            }
        }

        private void SetToLastUseColor(uint part)
        {
            EHeroModelParts eHeroModelParts = (EHeroModelParts)part;
            if (eHeroModelParts == EHeroModelParts.Main && curFashionClothes != null)
            {
                Color color_r = curFashionClothes.GetLastUseColor(ETintIndex.R);
                SetColor(EHeroModelParts.Main, ETintIndex.R, color_r);
                Color color_g = curFashionClothes.GetLastUseColor(ETintIndex.G);
                SetColor(EHeroModelParts.Main, ETintIndex.G, color_g);
                Color color_b = curFashionClothes.GetLastUseColor(ETintIndex.B);
                SetColor(EHeroModelParts.Main, ETintIndex.B, color_b);
                Color color_a = curFashionClothes.GetLastUseColor(ETintIndex.A);
                SetColor(EHeroModelParts.Main, ETintIndex.A, color_a);
            }
            else if (eHeroModelParts == EHeroModelParts.Weapon && curFashionWeapon != null)
            {
                Color color_r = curFashionWeapon.GetLastUseColor(ETintIndex.R);
                SetColor(EHeroModelParts.Weapon, ETintIndex.R, color_r);
            }
            else if (curFashionAccessory != null)
            {
                Color color_r = curFashionAccessory.GetLastUseColor(ETintIndex.R);
                SetColor(eHeroModelParts, ETintIndex.R, color_r);
            }
        }

        private void RevertToLastColorAll()
        {
            if (curFashionClothes != null)
            {
                Color color_r = curFashionClothes.GetLastUseColor(ETintIndex.R);
                SetColor(EHeroModelParts.Main, ETintIndex.R, color_r);
                Color color_g = curFashionClothes.GetLastUseColor(ETintIndex.G);
                SetColor(EHeroModelParts.Main, ETintIndex.G, color_g);
                Color color_b = curFashionClothes.GetLastUseColor(ETintIndex.B);
                SetColor(EHeroModelParts.Main, ETintIndex.B, color_b);
                Color color_a = curFashionClothes.GetLastUseColor(ETintIndex.A);
                SetColor(EHeroModelParts.Main, ETintIndex.A, color_a);
            }
            if (curFashionWeapon != null)
            {
                Color color_r = curFashionWeapon.GetLastUseColor(ETintIndex.R);
                SetColor(EHeroModelParts.Weapon, ETintIndex.R, color_r);
            }
            if (curFashionAccessory != null)
            {
                EHeroModelParts eHeroModelParts = EHeroModelParts.None;
                Color color_r = curFashionAccessory.GetLastUseColor(ETintIndex.R);
                eHeroModelParts = Sys_Fashion.Instance.parts[curFashionAccessory.Id];
                SetColor(eHeroModelParts, ETintIndex.R, color_r);
            }
        }


        private void OnDyeModelParts(uint id, EHeroModelParts eHeroModelParts)
        {
            switch (eHeroModelParts)
            {
                case EHeroModelParts.None:
                    break;
                case EHeroModelParts.Main:
                    DyeClothes(id);
                    break;
                case EHeroModelParts.Weapon:
                    DyeWeapon(id);
                    break;
                case EHeroModelParts.Jewelry_Head:
                case EHeroModelParts.Jewelry_Back:
                case EHeroModelParts.Jewelry_Waist:
                case EHeroModelParts.Jewelry_Face:
                    DyeAcce(id, eHeroModelParts);
                    break;
                case EHeroModelParts.Count:
                    break;
                default:
                    break;
            }
        }

        private void DyeClothes(uint id)
        {
            FashionClothes fashionClothes = Sys_Fashion.Instance._FashionClothes.Find(x => x.Id == id);
            if (fashionClothes != null)
            {
                if (viewModel == 0)
                {
                    Color color = fashionClothes.GetLastUseColor(ETintIndex.R);
                    SetColor(EHeroModelParts.Main, ETintIndex.R, color);

                    Color color1 = fashionClothes.GetLastUseColor(ETintIndex.G);
                    SetColor(EHeroModelParts.Main, ETintIndex.G, color1);

                    Color color2 = fashionClothes.GetLastUseColor(ETintIndex.B);
                    SetColor(EHeroModelParts.Main, ETintIndex.B, color2);

                    Color color3 = fashionClothes.GetLastUseColor(ETintIndex.A);
                    SetColor(EHeroModelParts.Main, ETintIndex.A, color3);
                }
                else
                {
                    Color color = fashionClothes.GetCurUseColor(ETintIndex.R);
                    SetColor(EHeroModelParts.Main, ETintIndex.R, color);

                    Color color1 = fashionClothes.GetCurUseColor(ETintIndex.G);
                    SetColor(EHeroModelParts.Main, ETintIndex.G, color1);

                    Color color2 = fashionClothes.GetCurUseColor(ETintIndex.B);
                    SetColor(EHeroModelParts.Main, ETintIndex.B, color2);

                    Color color3 = fashionClothes.GetCurUseColor(ETintIndex.A);
                    SetColor(EHeroModelParts.Main, ETintIndex.A, color3);
                }
            }
        }

        private void DyeWeapon(uint id)
        {
            FashionWeapon fashionWeapon = Sys_Fashion.Instance._FashionWeapons.Find(x => x.Id == id);
            if (fashionWeapon != null)
            {
                if (viewModel == 0)
                {
                    Color color = fashionWeapon.GetLastUseColor(ETintIndex.R);
                    SetColor(EHeroModelParts.Weapon, ETintIndex.R, color);
                }
                else
                {
                    Color color = fashionWeapon.GetCurUseColor(ETintIndex.R);
                    SetColor(EHeroModelParts.Weapon, ETintIndex.R, color);
                }
            }
        }

        private void DyeAcce(uint id, EHeroModelParts eHeroModelParts)
        {
            FashionAccessory fashionAccessories = Sys_Fashion.Instance._FashionAccessories.Find(x => x.Id == id);
            if (fashionAccessories != null)
            {
                if (viewModel == 0)
                {
                    Color color = fashionAccessories.GetLastUseColor(ETintIndex.R);
                    SetColor(eHeroModelParts, ETintIndex.R, color);
                }
                else
                {
                    Color color = fashionAccessories.GetCurUseColor(ETintIndex.R);
                    SetColor(eHeroModelParts, ETintIndex.R, color);
                }
            }
        }


        public void OnModelClick(BaseEventData eventData)
        {
            if (_uiModelShowManagerEntity != null)
                _uiModelShowManagerEntity.TouchModelOperation();
        }

        public void OnModleDrag(BaseEventData eventData)
        {
            if (_uiModelShowManagerEntity != null &&
                !_uiModelShowManagerEntity.IsCanControlUIOperation(WS_UIModelShowManagerEntity.ControlUIOperationEnum.OnRotateModel))
                return;

            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
            if (ped.delta.y < -5)
            {
                showSceneControl.mCamera.transform.Translate(Vector3.up * deltaTime * 0.2f, Space.Self);
                float y = Mathf.Clamp(showSceneControl.mCamera.transform.position.y, MinY, MaxY);
                showSceneControl.mCamera.transform.position = new Vector3(showSceneControl.mCamera.transform.position.x, y, showSceneControl.mCamera.transform.position.z);
            }
            else if (ped.delta.y > 5)
            {
                showSceneControl.mCamera.transform.Translate(Vector3.down * deltaTime * 0.2f, Space.Self);
                float y = Mathf.Clamp(showSceneControl.mCamera.transform.position.y, MinY, MaxY);
                showSceneControl.mCamera.transform.position = new Vector3(showSceneControl.mCamera.transform.position.x, y, showSceneControl.mCamera.transform.position.z);
            }
        }

        public void AddEulerAngles(Vector3 angle)
        {
            if (showSceneControl.mModelPos.transform != null)
            {
                Vector3 localAngle = showSceneControl.mModelPos.transform.localEulerAngles;
                //localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                Vector3 newAngle = new Vector3(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                showSceneControl.mModelPos.transform.localEulerAngles = newAngle;
            }
        }

        public void SetPosEulerAngles(float eulerAnglesY)
        {
            Transform posTrans = showSceneControl.mModelPos.transform;
            if (posTrans != null)
            {
                Vector3 localAngle = posTrans.localEulerAngles;
                posTrans.localEulerAngles = new Vector3(localAngle.x, eulerAnglesY, localAngle.z);
            }
        }
    }
}
