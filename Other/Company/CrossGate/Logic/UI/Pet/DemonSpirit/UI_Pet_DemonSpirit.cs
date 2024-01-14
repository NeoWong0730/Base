using Framework;
using Lib.Core;
using Logic;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
/// <summary>
/// 魔魂-魂珠类型
/// </summary>
public enum EDemonSpiritSphere
{
    Di = 1,
    Tian = 2,
    Long = 3,
    Shen= 4,
}

/// <summary> UI_Pet_DemonSpirit </summary>
public class UI_Pet_DemonSpirit : UIComponent
{
    private class UI_Pet_OwnDemonSpirit
    {
        /// <summary> 专属魔魂按钮 </summary>
        private Button ownDemonSpiritBtn;
        private Button activeBtn;
        private GameObject unLockGo;
        private GameObject emptyGo;
        private GameObject remake1Go;
        private GameObject remake1TextGo;
        private GameObject canActiveFx1Go;
        private GameObject Lock1Go;
        private Button remake1Btn;
        private GameObject remakeUnLock1Go;
        private GameObject remake2Go;
        private GameObject remake2TextGo;
        private GameObject canActiveFx2Go;
        private GameObject Lock2Go;
        private Button remake2Btn;
        private GameObject remakeUnLock2Go;
        /// <summary> 专属魔魂技能图片 </summary>
        private Image ownDemonSpiritImage;
        /// <summary> 专属魔魂名称 </summary>
        private Text ownDemonSpiritNameText;
        /// <summary> 专属魔魂等级 </summary>
        private Text ownDemonSpiritLevelText;

        private ClientPet clientPet;
        public void Init(Transform transform)
        {
            ownDemonSpiritBtn = transform.Find("Button").GetComponent<Button>();
            ownDemonSpiritBtn.onClick.AddListener(OnOwnDemonSpiritBtnBeClicked);
            activeBtn = transform.Find("Lock/Button").GetComponent<Button>();
            activeBtn.onClick.AddListener(OnOwnDemonSpiritBtnBeClicked);
            unLockGo = transform.Find("Lock").gameObject;
            emptyGo = transform.Find("Empty").gameObject;
            remake1Go = transform.Find("Other1").gameObject;
            remake1Btn = transform.Find("Other1/Image").GetComponent<Button>();
            remake1Btn.onClick.AddListener(OnRemake1BtnBeClicked);
            remake1TextGo = transform.Find("Other1/Image/Text").gameObject;
            remakeUnLock1Go = transform.Find("Other1/Lock").gameObject;
            canActiveFx1Go = transform.Find("Other1/Lock/Image/Fx").gameObject;
            Lock1Go = transform.Find("Other1/Lock/Image_Lock").gameObject;

            remake2Go = transform.Find("Other2").gameObject;
            remake2Btn = transform.Find("Other2/Image").GetComponent<Button>();
            remake2Btn.onClick.AddListener(OnRemake2BtnBeClicked);
            remake2TextGo = transform.Find("Other2/Image/Text").gameObject;
            remakeUnLock2Go = transform.Find("Other2/Lock").gameObject;
            canActiveFx2Go = transform.Find("Other2/Lock/Image/Fx").gameObject;
            Lock2Go = transform.Find("Other2/Lock/Image_Lock").gameObject;

            ownDemonSpiritImage = transform.Find("Image_Icon").GetComponent<Image>();
            ownDemonSpiritNameText = transform.Find("Image_Name/Text").GetComponent<Text>();
            ownDemonSpiritLevelText =transform.Find("Image_Name/Text/Text_Lv").GetComponent<Text>();
        }

        private void OnRemake2BtnBeClicked()
        {
            if(null != clientPet)
            {
                ClickedRemakeTimesBtn(2);
            }
        }

        private void OnRemake1BtnBeClicked()
        {
            if (null != clientPet)
            {
                ClickedRemakeTimesBtn(1);
            }
        }

        private void ClickedRemakeTimesBtn(int index)
        {
            UIManager.OpenUI(EUIID.UI_Pet_DemonReform, false, new UI_Pet_DemonParam() { type= (uint)index, tuple = clientPet.GetPetUid()});
        }

        private void OnOwnDemonSpiritBtnBeClicked()
        {
            if (null != clientPet)
            {
                if(clientPet.petData.soul_skill_id == 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002036u));
                }
                else
                {
                    var petParam = CSVPetNewParam.Instance.GetConfData(83u);
                    var limitLevel = 0u;
                    if (null != petParam)
                    {
                        limitLevel = petParam.value;
                    }

                    if(clientPet.petUnit.SimpleInfo.Level < limitLevel)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002026u));
                    }
                    else if (clientPet.petUnit.SimpleInfo.ExpiredTick > 0)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002027u));
                    }
                    else
                    {
                        UIManager.OpenUI(EUIID.UI_Pet_Demon, false, clientPet.GetPetUid());
                    }
                }
            }
        }

        public void SetOwnDemonSpiritView(ClientPet clientPet)
        {
            this.clientPet = clientPet;
            ResetView();
        }

        private void ResetView()
        {
            if(null != clientPet)
            {
                bool hasDemonSpiritSkill = clientPet.petData.soul_skill_id != 0;
                emptyGo.SetActive(!hasDemonSpiritSkill);
                bool isActiveDemonSpiritSkill = clientPet.GetDemonSpiritIsActive();
                unLockGo.SetActive(hasDemonSpiritSkill && !isActiveDemonSpiritSkill);
                remake1Go.SetActive(hasDemonSpiritSkill && isActiveDemonSpiritSkill);
                remake2Go.SetActive(hasDemonSpiritSkill && isActiveDemonSpiritSkill);
                ownDemonSpiritImage.gameObject.SetActive(hasDemonSpiritSkill);
                ownDemonSpiritNameText.transform.parent.gameObject.SetActive(hasDemonSpiritSkill && isActiveDemonSpiritSkill);
                ownDemonSpiritLevelText.transform.parent.gameObject.SetActive(hasDemonSpiritSkill && isActiveDemonSpiritSkill);
                if (hasDemonSpiritSkill)
                {
                    uint skillId = clientPet.petData.soul_skill_id;
                    if (isActiveDemonSpiritSkill)
                    {
                        //魔魂技能Id = 取服务器技能id
                        skillId = clientPet.petUnit.PetSoulUnit.SkillId;
                        //装配等级
                        uint equipSphereLevel = clientPet.GetEquipSphereTotalLevel();
                        var equipLevelLimits = Sys_Pet.Instance.DemonSpiritRemakeTimesByEquipLevel;
                        if(null != equipLevelLimits && equipLevelLimits.Count >= 2)
                        {
                            var rCount = clientPet.GetPetDemonSoulRemakeTimes();
                            //是否激活额外改造
                            bool isActive = rCount >= 1;

                            var currentLimitLevel = equipLevelLimits[0];
                            var allLevel = clientPet.GetEquipSphereTotalLevel();
                            bool canActive = currentLimitLevel <= allLevel;
                            Lock1Go.SetActive(!isActive && !canActive);
                            canActiveFx1Go.SetActive(!isActive && canActive);
                            remakeUnLock1Go.SetActive(!isActive);
                            remake1TextGo.SetActive(isActive);
                            currentLimitLevel = equipLevelLimits[1];
                            canActive = currentLimitLevel <= allLevel;
                            isActive = rCount >= 2;
                            remakeUnLock2Go.SetActive(!isActive);
                            remake2TextGo.SetActive(isActive);
                            Lock2Go.SetActive(!isActive && !canActive);
                            canActiveFx2Go.SetActive(!isActive && canActive);
                        }

                        if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
                        {
                            CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                            if (skillInfo != null)
                            {
                                TextHelper.SetText(ownDemonSpiritLevelText, 680003020, skillInfo.level.ToString());
                                ownDemonSpiritNameText.text = LanguageHelper.GetTextContent(skillInfo.name);
                            }
                            else
                            {
                                Debug.LogErrorFormat("not found skillId={0} in skillInfo", skillId);
                            }
                        }
                        else
                        {
                            CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                            if (skillInfo != null)
                            {
                                TextHelper.SetText(ownDemonSpiritLevelText, 680003020, skillInfo.level.ToString());
                                ownDemonSpiritNameText.text = LanguageHelper.GetTextContent(skillInfo.name);
                            }
                            else
                            {
                                Debug.LogErrorFormat("not found skillId={0}", skillId);
                            }
                        }
                    }

                    bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(skillId);
                    if (isActiveSkill) //主动技能
                    {
                        CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                        if (skillInfo != null)
                        {
                            ImageHelper.SetIcon(ownDemonSpiritImage, skillInfo.icon);
                        }
                        else
                        {
                            Debug.LogErrorFormat("not found skillId={0} in skillInfo", skillId);
                        }
                    }
                    else
                    {

                        CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                        if (skillInfo != null)
                        {
                            ImageHelper.SetIcon(ownDemonSpiritImage, skillInfo.icon);
                        }
                        else
                        {
                            Debug.LogErrorFormat("not found skillId={0}", skillId);
                        }
                    }

                    
                }
            }
        }
    }
    private UI_Pet_OwnDemonSpirit ownDemonSpirit = new UI_Pet_OwnDemonSpirit();
    private Button demonSpiritBagBtn;

    public ClientPet clientPet;
    private CSVPetNew.Data curCsvData;

    public AssetDependencies assetDependencies;
    private ShowSceneControl showSceneControl;
    private DisplayControl<EPetModelParts> petDisplay;

    private AsyncOperationHandle<GameObject> requestRef;
    private AsyncOperationHandle<GameObject> perfectFxrequestRef;
    private AsyncOperationHandle<GameObject> demonSpiritFxrequestRef;
    private GameObject demonSpiritGo;
    private GameObject petFxGo;
    private PlayableDirector timeLineOwnDir;
    private PlayableDirector timeLineRemakeDir;
    //模型内有表现球体 需要对位置进行设置
    List<Transform> sphereTrans = new List<Transform>(4);
    List<Transform> items = new List<Transform>(4);
    List<UI_Pet_DemonSpiritSphere> petDemonSpiritSpheres = new List<UI_Pet_DemonSpiritSphere>(4);

    /// <summary> 预设节点加载 </summary>
    protected override void Loaded()
    {
        assetDependencies = transform.GetComponent<AssetDependencies>();
        items.Add(transform.Find("View_Demon/Di/Model"));
        items.Add(transform.Find("View_Demon/Tian/Model"));
        items.Add(transform.Find("View_Demon/Long/Model"));
        items.Add(transform.Find("View_Demon/Shen/Model"));
        UI_Pet_DemonSpiritSphere tian = new UI_Pet_DemonSpiritSphere();
        tian.Init(transform.Find("View_Demon/Tian"), EDemonSpiritSphere.Tian);
        UI_Pet_DemonSpiritSphere di = new UI_Pet_DemonSpiritSphere();
        di.Init(transform.Find("View_Demon/Di"), EDemonSpiritSphere.Di);
        UI_Pet_DemonSpiritSphere l = new UI_Pet_DemonSpiritSphere();
        l.Init(transform.Find("View_Demon/Long"), EDemonSpiritSphere.Long);
        UI_Pet_DemonSpiritSphere shen = new UI_Pet_DemonSpiritSphere();
        shen.Init(transform.Find("View_Demon/Shen"), EDemonSpiritSphere.Shen);
        petDemonSpiritSpheres.Add(tian);
        petDemonSpiritSpheres.Add(di);
        petDemonSpiritSpheres.Add(l);
        petDemonSpiritSpheres.Add(shen);

        ownDemonSpirit.Init(transform.Find("Special_Skill"));

        demonSpiritBagBtn = transform.Find("Button").GetComponent<Button>();
        demonSpiritBagBtn.onClick.AddListener(OnDemonSpiritBagBtnClicked);
    }

    private void OnDemonSpiritBagBtnClicked()
    {
        UIManager.OpenUI(EUIID.UI_Pet_Demon_Bag);
    }

    /// <summary> 事件注册与反注册 </summary>
    protected override void ProcessEventsForEnable(bool toRegister)
    {
        Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnEquipDemonSpiritSphere, OnEquipChange, toRegister);
        //Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnActiveDemonSpirit, RefreshView, toRegister);
        Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnActiveDemonSpiritRemake, RefreshView, toRegister);
        Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnSaveOrDelectDemonSpiritSphereSkill, RefreshView, toRegister);
        Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnActiveOwnDemonSpiritOrRemakeAniUiAnimatorEnd, OnActiveOwnDemonSpiritOrRemakeAniUiAnimatorEnd, toRegister);
        Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnDemonSpiritUpgrade, OnRefreshDemonSpiritSkill, toRegister);
    }

    private void OnActiveOwnDemonSpiritOrRemakeAniUiAnimatorEnd(uint type)
    {
        if (type == 1)
        {
            if (null != timeLineOwnDir)
            {
                timeLineOwnDir.Play();
            }
        }
        else
        {
            if (null != timeLineRemakeDir)
            {
                timeLineRemakeDir.Play();
            }
        }
        if (null != petFxGo)
        {
            petFxGo.SetActive(false);
        }
        if (null != modelGo)
        {
            modelGo.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    private void OnRefreshDemonSpiritSkill(uint isCrit)
    {
        if (null != clientPet)
        {
            ownDemonSpirit.SetOwnDemonSpiritView(clientPet);
            SetDemonSpiritSphereCeilView();
            SetModelSphere();
        }
    }

    /// <summary> 界面参数设定 </summary>
    public override void SetData(params object[] arg)
    {

    }

    /// <summary> 界面开启 </summary>
    public override void Show()
    {
        base.Show();
        CheckPetFashion();
    }

    /// <summary> 界面隐藏或关闭 </summary>
    public override void Hide()
    {
        base.Hide();
        sphereTrans.Clear();
        UnloadModel();
        clientPet = null;
    }

    /// <summary> 界面删除 </summary>
    public override void OnDestroy()
    {
    }

    public void SetValue(ClientPet _clientPet)
    {
        if (null != _clientPet)
        {
            curCsvData = CSVPetNew.Instance.GetConfData(_clientPet.petUnit.SimpleInfo.PetId);
            if (clientPet == null)
            {
                clientPet = _clientPet;
                _LoadShowScene();
                _LoadShowModel(_clientPet);
            }
            else
            {
                uint curUid = clientPet.petUnit.Uid;
                clientPet = _clientPet;
                if (curUid != _clientPet.petUnit.Uid)
                {
                    UnloadModel();
                    _LoadShowScene();
                    _LoadShowModel(_clientPet);
                }
            }
        }
        else
        {
            clientPet = null;
            UnloadModel();
        }

        RefreshView();
    }

    private void OnEquipChange()
    {
        if (Sys_Pet.Instance.IsNeedShowDemonSpiritFx(clientPet))
        {
            if (null == demonSpiritGo)
            {
                LoadPetDemonSpiritAssetAsyn(Sys_Pet.Instance.DemonSpiritFxString);
            }
            else
            {
                demonSpiritGo.SetActive(true);
            }
        }
        else
        {
            if (null != demonSpiritGo)
            {
                demonSpiritGo.SetActive(false);
            }
        }
        RefreshView();
    }

    private void RefreshView()
    {
        if(null != clientPet)
        {
            ownDemonSpirit.SetOwnDemonSpiritView(clientPet);
            SetDemonSpiritSphereCeilView();
            SetModelSphere();
        }
    }

    private void SetDemonSpiritSphereCeilView()
    {
        uint petUid = clientPet.GetPetUid();
        for (int i = 0; i < petDemonSpiritSpheres.Count; i++)
        {
            petDemonSpiritSpheres[i].Set(clientPet);
        }
    }

    private void SetModelSphere()
    {
        uint petUid = clientPet.GetPetUid();
        for (int i = 0; i < sphereTrans.Count; i++)
        {
            var index = clientPet.GetSoulSphereByIndex(i);
            bool hasSphere = index > 0;
            sphereTrans[i].gameObject.SetActive(hasSphere);
        }
    }

    private void ResetDemonSpiritSphereCeilView()
    {
        for (int i = 0; i < petDemonSpiritSpheres.Count; i++)
        {
            petDemonSpiritSpheres[i].Reset();
        }
    }

    #region Model
    public void UnloadModel()
    {
        if(null != timeLineRemakeDir)
            timeLineRemakeDir.Stop();
        if (null != timeLineOwnDir)
            timeLineOwnDir.Stop();
        timeLineRemakeDir = null;
        timeLineOwnDir = null;
        _UnloadShowContent();
    }

    private void _UnloadShowContent()
    {
        //petDisplay?.Dispose();
        //petDisplay = null;
        if (null != petDisplay && null != petDisplay.mAnimation)
        {
            petDisplay.mAnimation.StopAll();
        }
        DisplayControl<EPetModelParts>.Destory(ref petDisplay);
        showSceneControl?.Dispose();
        showSceneControl = null;
        petDisplay = null;
        modelGo = null;
        petFxGo = null;
        demonSpiritGo = null;
        if (requestRef.IsValid())
        {
            AddressablesUtil.Release<GameObject>(ref requestRef, MHandle_Completed);
        }
        if (perfectFxrequestRef.IsValid())
        {
            AddressablesUtil.Release<GameObject>(ref perfectFxrequestRef, PerfectFxMHandle_Completed);
        }
        if(demonSpiritFxrequestRef.IsValid())
        {
            AddressablesUtil.Release<GameObject>(ref demonSpiritFxrequestRef, DemonSpiritFxMHandle_Completed); 
        }
    }

    private void _LoadShowScene()
    {
        if (showSceneControl == null)
        {
            showSceneControl = new ShowSceneControl();
        }

        GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
        sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
        sceneModel.transform.localPosition = new Vector3((int)(EUIID.UI_Pet_Message), 0, 0);
        sphereTrans.Clear();
        Transform sphereFxPartner = sceneModel.transform.Find("Fx_DemonS/Fx_DemonS_Ball");
        for (int i = 0; i < sphereFxPartner.childCount; i++)
        {
            sphereTrans.Add(sphereFxPartner.Find($"Fx_DemonS_Ball{i + 1}"));
        }
        petFxGo = sceneModel.transform.Find("Fx").gameObject;
        showSceneControl.Parse(sceneModel);
        UpdataShowScenePosPosition();
        timeLineOwnDir = sceneModel.transform.Find("Timeline/Activate_ZhuanShu").GetComponent<PlayableDirector>();
        timeLineRemakeDir = sceneModel.transform.Find("Timeline/Activate_EWaiGaiZao").GetComponent<PlayableDirector>();
        timeLineOwnDir.stopped += OnOneAnimationEnd;
        timeLineRemakeDir.stopped += OnOneAnimationEnd;
        //eventImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
        if (petDisplay == null)
        {
            petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
            petDisplay.onLoaded = OnShowModelLoaded;
        }
    }

    private void OnOneAnimationEnd(PlayableDirector playableDirector)
    {
        if (null == timeLineRemakeDir || null == timeLineOwnDir)
            return;
        if (null != modelGo)
        {
            modelGo.SetActive(true);
        }
        if (null != petDisplay && null != petDisplay.mAnimation)
            petDisplay.mAnimation.Play((uint)EStateType.Idle);

        
        if(null != petFxGo)
        {
            petFxGo.SetActive(true);
        }
        gameObject.SetActive(true);
        RefreshView();
    }

    public void UpdataShowScenePosPosition()
    {
        if(null != showSceneControl)
        {
            int count = sphereTrans.Count;
            for (int i = 0; i < count; i++)
            {
                Transform objTrans = sphereTrans[i].transform;
                //将模型的位置转为视图坐标
                Vector3 posshow = showSceneControl.mCamera.WorldToViewportPoint(objTrans.position);

                var itemposition = items[i].position;
                //将ui的坐标也转换成视图坐标
                Vector3 uiitem = UIManager.mUICamera.WorldToViewportPoint(itemposition);
                // 将模型的视图坐标用模型的x y 替换
                posshow.x = uiitem.x;
                posshow.y = uiitem.y;
                //将转换后的模型视图坐标再次转换回模型坐标
                Vector3 position = showSceneControl.mCamera.ViewportToWorldPoint(posshow);
                //position.z = objTrans.position.z;
                objTrans.position = position;
            }
        }
        
    }

    private void _LoadShowModel(ClientPet clientPet)
    {
        
        string _modelPath = Sys_Pet.Instance.GetPetModelPath(clientPet);
        //player = GameCenter.modelShowWorld.CreateActor<ModelShowActor>(clientPet.petUnit.PetId);
        petDisplay.eLayerMask = ELayerMask.ModelShow;
        petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);

        petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
        showSceneControl.mModelPos.transform.Rotate(new Vector3(curCsvData.angle1, curCsvData.angle2, curCsvData.angle3));
        showSceneControl.mModelPos.transform.localScale = new Vector3(curCsvData.size, curCsvData.size, curCsvData.size);
        showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x + curCsvData.translation, curCsvData.height, showSceneControl.mModelPos.transform.localPosition.z);
    }

    public GameObject modelGo;
    private void OnShowModelLoaded(int obj)
    {
        if (obj == 0)
        {
            uint highId = clientPet.petUnit.SimpleInfo.PetId;
            modelGo = petDisplay.GetPart(EPetModelParts.Main).gameObject;
            CheckPetFashion();
            modelGo.SetActive(false);
            petDisplay.mAnimation.UpdateHoldingAnimations(CSVPetNew.Instance.GetConfData(highId).action_id_show, curCsvData.weapon, Constants.PetShowAnimationClipHashSet, go: modelGo);
            ani_index = 0;
            checkAni = true;
            uint lowG = clientPet.GetPetMaxGradeCount() - clientPet.GetPetGradeCount();
            if (lowG == 0)
            {
                LoadPetGeerAssetAsyn(Sys_Pet.Instance.GetPetGearFxPath(clientPet.petData, true));
                var perfectPath = Sys_Pet.Instance.GetPetRemakePerfectFxPath(clientPet);
                if (null != perfectPath)
                {
                    LoadPetPerfectAssetAsyn(perfectPath);
                }
                if (Sys_Pet.Instance.IsNeedShowDemonSpiritFx(clientPet))
                {
                    if (null == demonSpiritGo)
                    {
                        LoadPetDemonSpiritAssetAsyn(Sys_Pet.Instance.DemonSpiritFxString);
                    }
                    else
                    {
                        demonSpiritGo.SetActive(true);
                    }
                }
                else
                {
                    if (null != demonSpiritGo)
                    {
                        demonSpiritGo.SetActive(false);
                    }
                }
            }
        }
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

    private void CheckPetFashion()
    {
        if (null != modelGo)
        {
            SceneActor.PetWearSet(Constants.PETWEAR_EQUIP, clientPet.GetPetSuitFashionId(), modelGo.transform);
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

    private void SetFx(GameObject fxGo)
    {
        if (null != modelGo)
        {
            Transform fxParent = showSceneControl.mRoot.transform.Find("Fx");
            if (null != fxParent)
            {
                if (fxGo != null)
                {
                    LayerMaskUtil.Setlayer(fxGo.transform, ELayerMask.ModelShow);
                }
            }

            fxGo.transform.SetParent(fxParent, false);
        }
    }

    private void PetFxSetLayer(Transform _transform, int layer)
    {
        if (_transform != null)
        {
            transform.gameObject.layer = layer;
            int count = _transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                PetFxSetLayer(_transform.GetChild(i), layer);
            }
        }
    }

    private void LoadPetGeerAssetAsyn(string path)
    {
        if (null != path)
        {
            AddressablesUtil.InstantiateAsync(ref requestRef, path, MHandle_Completed);
        }
    }

    private void LoadPetPerfectAssetAsyn(string path)
    {
        if (null != path)
        {
            AddressablesUtil.InstantiateAsync(ref perfectFxrequestRef, path, PerfectFxMHandle_Completed);
        }
    }

    private void LoadPetDemonSpiritAssetAsyn(string path)
    {
        if (null != path)
        {
            AddressablesUtil.InstantiateAsync(ref demonSpiritFxrequestRef, path, DemonSpiritFxMHandle_Completed);
        }
    }

    

    private void MHandle_Completed(AsyncOperationHandle<GameObject> handle)
    {
        SetFx(handle.Result);
    }

    private void PerfectFxMHandle_Completed(AsyncOperationHandle<GameObject> handle)
    {
        SetFx(handle.Result);
    }

    private void DemonSpiritFxMHandle_Completed(AsyncOperationHandle<GameObject> handle)
    {
        demonSpiritGo = handle.Result;
        SetFx(handle.Result);
    }


    public void OnDrag(BaseEventData eventData)
    {
        if (null != clientPet)
        {
            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }
    }

    public void AddEulerAngles(Vector3 angle)
    {
        Vector3 ilrTemoVector3 = angle;
        if (showSceneControl.mModelPos.transform != null)
        {
            showSceneControl.mModelPos.transform.Rotate(ilrTemoVector3.x, ilrTemoVector3.y, ilrTemoVector3.z);
        }
    }

}

/// <summary> 魔魂-魂珠格 </summary>
public class UI_Pet_DemonSpiritSphere
{
    //魂珠格类型
    private EDemonSpiritSphere sphereType;
    //无珠
    private GameObject unModelGo;
    //锁定
    private GameObject unLockGo;
    //有珠
    private GameObject modelGo;
    //魂珠等级文本
    private Text sphereLevel;

    //默认技能
    private GameObject skill1Go;

    //解锁技能
    private GameObject skill2Go;

    private Button unlockBtn;
    private Button sphereBtn;

    private Button skill1Btn;
    private Button skill2Btn;

    private ClientPet pet;
    /// <summary> 预设节点加载 </summary>
    public void Init(Transform transform, EDemonSpiritSphere _sphereType)
    {
        unModelGo = transform.Find("Unmodel").gameObject;
        unLockGo = unModelGo.transform.Find("Lock").gameObject;
        unlockBtn = unModelGo.transform.Find("Image_BG").GetComponent<Button>();
        unlockBtn.onClick.AddListener(UnLockBtnClicked);

        modelGo = transform.Find("Model").gameObject;
        sphereLevel = modelGo.transform.Find("Text/Text_Lv").GetComponent<Text>();
        skill1Go = modelGo.transform.Find("Skill1").gameObject;
        skill1Btn = modelGo.transform.Find("Skill1/Button").GetComponent<Button>();
        skill1Btn.onClick.AddListener(Skill1BtnClicked);
        skill2Go = modelGo.transform.Find("Skill2").gameObject;

        
        skill2Btn = modelGo.transform.Find("Skill2/Button").GetComponent<Button>();
        skill2Btn.onClick.AddListener(Skill2BtnClicked);
        sphereBtn = modelGo.transform.Find("Button").GetComponent<Button>();
        sphereBtn.onClick.AddListener(SphereBtnClicked);

        sphereType = _sphereType;
    }

    public void Set(ClientPet _pet)
    {
        pet = _pet;
        Reset();
    }

    private void SetSphereView()
    {
        if(null != pet)
        {
            var sphereData = Sys_Pet.Instance.GetPetDemonSpiritSphereInfo((uint)sphereType, pet.GetSoulSphereByIndex((int)sphereType - 1));
            if(null != sphereData)
            {
                uint level = sphereData.Level;
                uint skill1 = sphereData.SkillIds[0];
                uint skill2 = sphereData.SkillIds[1];
                SetSkill(skill1Go, skill1);
                SetSkill(skill2Go, skill2);
                TextHelper.SetText(sphereLevel, 680003020, level.ToString());
            }
        }
    }

    private void SetSkill(GameObject skillGo, uint skillId)
    {
        bool hasSkill = skillId > 0;
        if (hasSkill)
        {
            if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
            {
                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    TextHelper.SetText(skillGo.transform.Find("Text_Level").GetComponent<Text>(), 680003026, skillInfo.level.ToString());
                    ImageHelper.SetIcon(skillGo.transform.Find("PetSkillItem01/Image_Skill").GetComponent<Image>(), skillInfo.icon);
                    ImageHelper.GetPetSkillQuality_Frame(skillGo.transform.Find("PetSkillItem01/Image_Quality").GetComponent<Image>(), (int)skillInfo.quality);
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", skillId);
                }
            }
            else
            {
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    TextHelper.SetText(skillGo.transform.Find("Text_Level").GetComponent<Text>(), 680003026, skillInfo.level.ToString());
                    ImageHelper.SetIcon(skillGo.transform.Find("PetSkillItem01/Image_Skill").GetComponent<Image>(), skillInfo.icon);
                    ImageHelper.GetPetSkillQuality_Frame(skillGo.transform.Find("PetSkillItem01/Image_Quality").GetComponent<Image>(), (int)skillInfo.quality);
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                }
            }
            var unlockGo = skillGo.transform.Find("Lock").gameObject;
            var levelGroundImageGo = skillGo.transform.Find("Image_Level").gameObject;
            levelGroundImageGo?.SetActive(true);
            unlockGo?.SetActive(false);
        }
        else
        {
            var lockData = CSVSoulBead.Instance.GetSkillLockLevelData((uint)sphereType);
            if (null != lockData)
            {
                TextHelper.SetText(skillGo.transform.Find("Text_Level").GetComponent<Text>(), 680003031, lockData.level.ToString());//解锁等级
                var unlockGo = skillGo.transform.Find("Lock").gameObject;
                var levelGroundImageGo = skillGo.transform.Find("Image_Level").gameObject;
                levelGroundImageGo?.SetActive(false);
                unlockGo?.SetActive(true);
            }
        }
        skillGo.transform.Find("PetSkillItem01").gameObject.SetActive(hasSkill);
    }

    public void Reset()
    {
        if(null != pet)
        {
            //宠物是否激活主要魔魂
            bool isActiveDemonSpirit = pet.GetDemonSpiritIsActive();
            //取出宠物对应的格子位置是否有装备魔珠
            bool hasSphere = pet.GetSoulSphereByIndex((int)sphereType - 1) > 0;
            if (isActiveDemonSpirit)
            {
                if(hasSphere)
                {
                    SetSphereView();
                }
            }
            unModelGo.SetActive(!isActiveDemonSpirit || !hasSphere);
            unLockGo.SetActive(!isActiveDemonSpirit);
            modelGo.SetActive(isActiveDemonSpirit && hasSphere);
        }
    }

    private void SphereBtnClicked()
    {
        if (null != pet)
        {
            //获取宠物的魂珠
            PetSoulBeadInfo sphereTemp = Sys_Pet.Instance.GetPetDemonSpiritSphereInfo((uint)sphereType, pet.GetSoulSphereByIndex((int)sphereType - 1));
            if(null != sphereTemp)
            {
                UIManager.OpenUI(EUIID.UI_Pet_Demon_Detail, false, sphereTemp);
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_Pet_DemonSelect, false, new Tuple<uint,uint> ((uint)sphereType, pet.GetPetUid()));
            }
        }
    }

    private void Skill1BtnClicked()
    {
        if (null != pet)
        {
            //获取宠物的魂珠
            PetSoulBeadInfo sphereTemp = Sys_Pet.Instance.GetPetDemonSpiritSphereInfo((uint)sphereType, pet.GetSoulSphereByIndex((int)sphereType - 1));
            uint skill = sphereTemp.SkillIds[0];
            if(skill > 0)
            {
                UIManager.OpenUI(EUIID.UI_Pet_DemonSkill_Tips, false, new Tuple<uint, uint, uint>(sphereTemp.Type, sphereTemp.Index, 1));
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002055));
            }
        }
    }

    private void Skill2BtnClicked()
    {
        if (null != pet)
        {
            //获取宠物的魂珠
            PetSoulBeadInfo sphereTemp = Sys_Pet.Instance.GetPetDemonSpiritSphereInfo((uint)sphereType, pet.GetSoulSphereByIndex((int)sphereType - 1));
            uint skill = sphereTemp.SkillIds[1];
            if (skill > 0)
            {
                UIManager.OpenUI(EUIID.UI_Pet_DemonSkill_Tips, false, new Tuple<uint, uint, uint>(sphereTemp.Type, sphereTemp.Index,2));
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002055));
            }
        }
    }

    private void UnLockBtnClicked()
    {
        if (null != pet)
        {
            if(pet.GetDemonSpiritIsActive())
            {
                UIManager.OpenUI(EUIID.UI_Pet_DemonSelect, false, new Tuple<uint, uint>((uint)sphereType, pet.GetPetUid()));
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002028)); //激活专属魔魂后开启
            }
        }
    }
    #endregion
}

