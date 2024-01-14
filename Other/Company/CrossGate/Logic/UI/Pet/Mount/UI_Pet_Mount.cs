using Framework;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;
public class UI_Pet_MountSkillCeil
{
    public Transform transform;
    public PetSkillBase petSkillBase;
    public PetSkillItem02 skillItem;
    private Action<UI_Pet_MountSkillCeil> onClick;
    public Button addBtn;
    public GameObject lockGo;
    public GameObject addGo;
    public bool isLock = true;
    public uint lockLevel = 0;
    public void BingGameObject(GameObject go)
    {
        transform = go.transform;

        skillItem = new PetSkillItem02();
        skillItem.Bind(transform.Find("PetSkillItem01").gameObject);
        lockGo = transform.Find("Btn_Add/Image_Lock")?.gameObject;
        addGo = transform.Find("Btn_Add/Image")?.gameObject;
        addBtn = transform.Find("Btn_Add")?.GetComponent<Button>();
        addBtn?.onClick.RemoveAllListeners();
        addBtn?.onClick.AddListener(OnClicked);
        skillItem.AddClickListener(OnClicked);
    }

    public void AddClickListener(Action<UI_Pet_MountSkillCeil> onclicked = null)
    {
        onClick = onclicked;
    }

    private void OnClicked()
    {
        onClick?.Invoke(this);
    }

    /// <summary>
    /// index 只有在是改造技能是有效
    /// </summary>
    /// <param name="skillId"></param>
    /// <param name="isUnique"></param>
    /// <param name="isBuild"></param>
    /// <param name="itemId"></param>
    /// <param name="index"></param>
    public void SetData(uint skillId, bool isMountSkill, uint lockLevel = 0)
    {
        if (petSkillBase == null)
        {
            petSkillBase = new PetSkillBase(skillId, false, false, 0, false, false, false, isMountSkill, false, false);
        }
        else
        {
            petSkillBase.Reset(skillId, false, false, 0, false, false, false, isMountSkill, false, false);
        }
        isLock = lockLevel > 0;
        this.lockLevel = lockLevel;
        SetPetSkillInfo();
    }

    private void SetPetSkillInfo()
    {
        bool hasSkill = petSkillBase.isHasSkill && lockLevel == 0;
        if (hasSkill)
        {
            skillItem.SetDate(petSkillBase.skillId);
            skillItem.skillImage.gameObject.SetActive(true);
            addGo?.gameObject.SetActive(false);
            lockGo?.gameObject.SetActive(false);
        }
        else
        {
            lockGo?.gameObject.SetActive(!Sys_Pet.Instance.MountIsOpen || isLock);
            addGo?.gameObject.SetActive(!isLock && Sys_Pet.Instance.MountIsOpen);
        }
        ImageHelper.SetImageGray(transform, petSkillBase.isHasHight, true);
        skillItem.transform.gameObject.SetActive(hasSkill);
        skillItem.mountGo?.SetActive(petSkillBase.isMountSkill);
        skillItem.uniqueGo.SetActive(petSkillBase.isUnique);
        skillItem.buildGo.SetActive(false);
    }
}

class MountPetModelShow : JsBattleModelShowBase
{
    private ClientPet clientPet;
    private CSVPetNew.Data petData;
    protected DisplayControl<EPetModelParts> m_DisplayControl;

    public override void LoadModel(uint id, uint caree = 0, uint weaponid = 0, Dictionary<uint, List<dressData>> DressValue = null, bool isPreview = true)
    {
        m_DisplayControl = DisplayControl<EPetModelParts>.Create((int)EHeroModelParts.Count);
        m_DisplayControl.eLayerMask = ELayerMask.ModelShow;
        m_DisplayControl.onLoaded = OnLoadend;

        if (isPreview)
        {
            petData = CSVPetNew.Instance.GetConfData(id);
            m_DisplayControl.LoadMainModel(EPetModelParts.Main, petData.model_show, EPetModelParts.None, "Pos");
        }
        else
        {
            clientPet = Sys_Pet.Instance.GetPetByUId(id);
            petData = CSVPetNew.Instance.GetConfData(clientPet.petData.id);
            m_DisplayControl.LoadMainModel(EPetModelParts.Main, Sys_Pet.Instance.GetPetModelPath(clientPet), EPetModelParts.None, "Pos");
        }
    }


    private void OnLoadend(int id)
    {
        if (m_DisplayControl == null || m_DisplayControl.bMainPartFinished == false)
            return;
        go = m_DisplayControl.GetPart(EPetModelParts.Main).gameObject;
        go.SetActive(false);
        uint petFashionId = 0;
        if (null != clientPet)
        {
            petFashionId = clientPet.GetPetSuitFashionId();
        }
        SceneActor.PetWearSet(Constants.PETWEAR_EQUIP, petFashionId, go.transform);
        var value = m_DisplayControl.GetPart(id);
        value.gameObject.transform.SetParent(Parent.transform, false);
        if(null != petData)
            m_DisplayControl.mAnimation.UpdateHoldingAnimations(petData.action_id_show, Constants.UMARMEDID, go: go);
    }

    public override void SetParent(VirtualGameObject parent)
    {
        if (Parent == parent)
            return;

        Parent = parent;

        if (m_DisplayControl == null || m_DisplayControl.bMainPartFinished == false)
            return;

        var value = m_DisplayControl.GetPart(EPetModelParts.Main);

        if (value != null)
            value.gameObject.transform.SetParent(Parent.transform, false);
    }

    public override void SetActive(bool active)
    {
        if (m_DisplayControl == null || m_DisplayControl.bMainPartFinished == false)
            return;

        var value = m_DisplayControl.GetPart(EPetModelParts.Main);

        if (value != null && value.gameObject.activeSelf != active)
        {
            value.gameObject.SetActive(active);
            m_DisplayControl?.mAnimation.Play((uint)EStateType.Idle);
        }

    }
    public override void Dispose()
    {
        Parent = null;
        go = null;
        DisplayControl<EPetModelParts>.Destory(ref m_DisplayControl);
    }
}

public class UI_Pet_Mount : UIComponent
{
    private Button energyBtn;
    private Button mountTipBtn;
    private Button screemBtn;
    private Button contractLeveBtn;
    private Slider energySlider;
    private Text energyText;
    private Transform skillTrans;
    private GameObject noneGo;
    public enum EMountPos
    {
        Pos0 = 0,
        Pos1,
        Pos2,
        Pos3,
    }
    private AssetDependencies mInfoAssetDependencies;
    private List<ShowSceneControl> showSceneControls = new List<ShowSceneControl>(4);
    private ShowSceneControl m_ShowSceneControl = null;
    private List<JsBattleShowModelItem> m_ShowPosList = new List<JsBattleShowModelItem>(4);

    private List<JsBattleModelShowBase> m_ModelShow = new List<JsBattleModelShowBase>(4);
    /// <summary> 模型ui视图 </summary>
    List<UI_Pet_MountInfo> uI_Pet_MountInfos = new List<UI_Pet_MountInfo>(4);
    /// <summary> 无限滚动 </summary>
    private InfinityGrid infinityGrid;
    private int selectIndex;
    private List<PetMountCell> cells = new List<PetMountCell>();
    private List<ClientPet> mountPets = new List<ClientPet>();
    private List<uint> mountIds = new List<uint>();
    private List<uint> showPets = new List<uint>();

    private List<uint> skillIds = new List<uint>();
    private List<uint> skillIdsState = new List<uint>();
    
    protected override void Loaded()
    {
        mInfoAssetDependencies = transform.GetComponent<AssetDependencies>();
        energyBtn = transform.Find("Energy/Btn_Charge").GetComponent<Button>();
        energyBtn.onClick.AddListener(OnEnergyBtnClicked);

        energySlider = transform.Find("Energy/Slider").GetComponent<Slider>();
        energyText = transform.Find("Energy/Text_Value").GetComponent<Text>();
        mountTipBtn =transform.Find("View_Bottom/Btn_Domestication").GetComponent<Button>();
        mountTipBtn.onClick.AddListener(MountTipBtnClicked);
        skillTrans = transform.Find("View_Bottom/Scroll View/Viewport/Content");
        noneGo = transform.Find("View_Npc022").gameObject;
        screemBtn = transform.Find("Scroll_View_Pets/Btn_Screen").GetComponent<Button>();
        screemBtn.onClick.AddListener(OnMoutScreemBtnClicked);
        infinityGrid = transform.Find("Scroll_View_Pets").GetComponent<InfinityGrid>();
        infinityGrid.onCreateCell = OnHeadCreateCell;
        infinityGrid.onCellChange = OnHeadCellChange;

        contractLeveBtn =  transform.Find("Btn_Intensify").GetComponent<Button>();
        contractLeveBtn.onClick.AddListener(OnContractLeveBtnClicked);

        UI_Pet_MountInfo temp1 = AddComponent<UI_Pet_MountInfo>(transform.Find("View_Formation/ItemMain"));
        temp1.Init(0);
        UI_Pet_MountInfo temp2 = AddComponent<UI_Pet_MountInfo>(transform.Find("View_Formation/Group/Item_1"));
        temp2.Init(1);
        UI_Pet_MountInfo temp3 = AddComponent<UI_Pet_MountInfo>(transform.Find("View_Formation/Group/Item_2"));
        temp3.Init(2);
        UI_Pet_MountInfo temp4 = AddComponent<UI_Pet_MountInfo>(transform.Find("View_Formation/Group/Item_3"));
        temp4.Init(3);
        uI_Pet_MountInfos.Add(temp1);
        uI_Pet_MountInfos.Add(temp2);
        uI_Pet_MountInfos.Add(temp3);
        uI_Pet_MountInfos.Add(temp4);
        
    }

    private void OnContractLeveBtnClicked()
    {
        CSVPetNewParam.Data levelParam = CSVPetNewParam.Instance.GetConfData(92u);
        if(null != levelParam)
        {
            if (Sys_Role.Instance.Role.Level < levelParam.value)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1018623, levelParam.value.ToString()));
                return;
            }
            if (selectIndex >= 0 && selectIndex < mountPets.Count)
            {
                ClientPet clientPet = mountPets[selectIndex];
                UIManager.OpenUI(EUIID.UI_Pet_MountIntensify, false, clientPet.GetPetUid());
            }
        }
    }

    protected override void ProcessEventsForEnable(bool toRegister)
    {

    }

    public override void Show()
    {
        RefreshView();
        SetEnergyInfo();
        SetMountPetSkillInfo();
    }

    public override void Hide()
    {
        UnLoadShowScene();
    }

    public void Close()
    {
        selectIndex = 0;
    }

    public void OnMountScreemChange()
    {
        selectIndex = 0;
        RefreshView();
        SetEnergyInfo();
        SetMountPetSkillInfo();
    }

    public void RefreshView()
    {
        mountPets = Sys_Pet.Instance.GetMountClientPets();
        mountIds = Sys_Pet.Instance.GetMountPetByConfigs();

        int petCount = mountPets.Count + mountIds.Count;
        if (petCount == 0)
        {
            selectIndex = -1;
        }
        /*else if(selectIndex >= petCount)
        {
            selectIndex = 0;
        }*/

        infinityGrid.CellCount = petCount;
        infinityGrid.ForceRefreshActiveCell();

        SelectState();
        UnLoadShowScene();
        SetShowPetInfo();
        SetButtonState();
        SetNoneState();
        LoadShowScene();
    }

    #region 模型加载

    public int currentNum = 0;
    public void LoadShowScene()
    {
        
        int needCount = showPets.Count;
        if (currentNum > needCount)
        {
            UnLoadShowScene(needCount);
        }
        else
        {
            for (int i = currentNum; i < needCount; i++)
            {
                GameObject scene = GameObject.Instantiate<GameObject>(mInfoAssetDependencies.mCustomDependencies[0] as GameObject);

                ShowSceneControl _ShowSceneControl = new ShowSceneControl();

                scene.transform.SetParent(GameCenter.sceneShowRoot.transform);
                scene.transform.localPosition = new Vector3(i * 10, 0, 0);
                _ShowSceneControl.Parse(scene);
                Transform objTrans = _ShowSceneControl.mRoot.transform.Find("Pos");

                VirtualGameObject vobj = new VirtualGameObject();
                vobj.SetGameObject(objTrans.gameObject, true);

                m_ShowPosList.Add(new JsBattleShowModelItem() { VGO = vobj });
                showSceneControls.Add(_ShowSceneControl);
                currentNum++;
            }
        }
       
        for (int i = 0; i < showPets.Count; i++)
        {
            if(showPets[i] > 0)
            {
                SetMemberModel((EMountPos)i, (uint)i, showPets[i], isPreview: selectIndex >= mountPets.Count);
            }
        }

        for (int i = 0; i < uI_Pet_MountInfos.Count; i++)
        {
            if(showPets.Count <= i)
            {
                uI_Pet_MountInfos[i].ClearRawImage();
                uI_Pet_MountInfos[i].Hide();
            }
            else
            {
                uI_Pet_MountInfos[i].SetRawImage(showSceneControls[i].GetTemporary(0, 0, 16, RenderTextureFormat.ARGB32, 1f), showPets[i]);
                uI_Pet_MountInfos[i].Show();
            }
        }
    }

    public void UnLoadShowScene(int needCount)
    {
        if (showSceneControls.Count == 0)
            return;
        for (int i = showSceneControls.Count - 1 ; i >= needCount; i--)
        {
            showSceneControls[i].Dispose();

            showSceneControls[i] = null;
            uI_Pet_MountInfos[i].ClearRawImage();
            //uI_Pet_MountInfos[i].Hide();
            showSceneControls.RemoveAt(i);
            m_ShowPosList.RemoveAt(i);
            if(i < m_ModelShow.Count)
            {
                m_ModelShow[i].Dispose();
                m_ModelShow[i] = null;
                m_ModelShow.RemoveAt(i);
            }
            
            currentNum--;
        }

    }

    public void UnLoadShowScene()
    {
        if (showSceneControls.Count == 0)
            return;
        for (int i = 0; i < showSceneControls.Count; i++)
        {
            showSceneControls[i].Dispose();

            showSceneControls[i] = null;
            uI_Pet_MountInfos[i].ClearRawImage();
        }

        showSceneControls.Clear();

        m_ShowPosList.Clear();

        int count = m_ModelShow.Count;
        for (int i = 0; i < count; i++)
        {
            m_ModelShow[i].Dispose();
            m_ModelShow[i] = null;
        }
        m_ModelShow.Clear();
        currentNum = 0;
    }

    public void RestShowScene(int index)
    {
        if(index < m_ShowPosList.Count)
        {
            var value = m_ShowPosList[index];

            if (value.ModelShow != null)
            {
                value.SetModelShow(null);
            }
        }
    }

    public void UpdateShowScene(int index)
    {
        if (index < m_ModelShow.Count)
        {
            var value = m_ModelShow[index];

            if (value != null)
            {
                value.Dispose();

                m_ModelShow.RemoveAt(index);
            }
        }
    }

    public void SetMemberModel(EMountPos ePos, uint index, uint id,  uint caree = 0,uint WeaponID = 0, uint dressId = 0, Dictionary<uint, List<dressData>> DressValue = null, bool isPreview = false)
    {
        JsBattleModelShowBase modelShow = null;

        modelShow = m_ModelShow.Find(o => o.index == index);

        if (modelShow != null)
        {
            modelShow.ChangeWeapon(WeaponID);
        }
        
        if (modelShow == null)
        {
            modelShow = CreateModeShow(ePos, id, caree, WeaponID, dressId, DressValue, isPreview);

            modelShow.index = index;

            m_ModelShow.Add(modelShow);

        }

        var value = GetModelPosition(ePos);

        if (value.ModelShow != null)
        {
            value.ModelShow.SetActive(false);
        }
        value.SetModelShow(modelShow);

    }

    private JsBattleShowModelItem GetModelPosition(EMountPos ePos)
    {
        JsBattleShowModelItem posObj = null;

        posObj = m_ShowPosList[(int)ePos];

        return posObj;
    }

    private JsBattleModelShowBase CreateModeShow(EMountPos ePos, uint id, uint caree = 0,uint WeaponID = 0, uint dressId = 0, Dictionary<uint, List<dressData>> DressValue = null, bool isPreview = true)
    {
        MountPetModelShow modeShow = new MountPetModelShow();

        var parent = GetModelPosition(ePos);

        modeShow.Parent = parent.VGO;
        CSVPetNew.Data curCsvData = null;
        if(isPreview)
        {
            curCsvData = CSVPetNew.Instance.GetConfData(id);
        }
        else
        {
            var clientPet = Sys_Pet.Instance.GetPetByUId(id);
            if(null != clientPet)
            {
                curCsvData = CSVPetNew.Instance.GetConfData(clientPet.petData.id);
            }
        }

        modeShow.Parent.transform.Rotate(new Vector3(curCsvData.angle1, curCsvData.angle2, curCsvData.angle3));
        modeShow.Parent.transform.localScale = new Vector3(curCsvData.size, curCsvData.size, curCsvData.size);
        modeShow.Parent.transform.localPosition = new Vector3(modeShow.Parent.transform.localPosition.x + curCsvData.translation, curCsvData.height, modeShow.Parent.transform.localPosition.z);


        modeShow.LoadModel(id, caree,WeaponID, DressValue, isPreview);

        return modeShow;
    }
    #endregion

    /// <summary>
    /// 滚动列表创建回调
    /// </summary>
    /// <param name="cell"></param>
    public void OnHeadCreateCell(InfinityGridCell cell)
    {
        PetMountCell entry = new PetMountCell();
        GameObject go = cell.mRootTransform.gameObject;
        entry.Init(go.transform);
        entry.AddListen(OnCellClicked);
        cell.BindUserData(entry);
        cells.Add(entry);
    }

    /// <summary>
    /// 滚动列表滚动回调
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="index"></param>
    public void OnHeadCellChange(InfinityGridCell cell, int index)
    {
        if (index < 0 || index >= mountPets.Count + mountIds.Count)
            return;
        PetMountCell entry = cell.mUserData as PetMountCell; 
        if(index < mountPets.Count)
        {
            entry.ReSetData(mountPets[index],0, index);
            entry.SetSelect(selectIndex == index);
        }
        else if(index >= mountPets.Count)
        {
            entry.ReSetData(null, mountIds[index - mountPets.Count], index);
            entry.SetSelect(selectIndex == index);
        }
    }

    private void SelectState()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].SetSelect(cells[i].index == selectIndex);
        }
    }

    private void SetShowPetInfo()
    {
        showPets.Clear();
        if (selectIndex >= 0)
        {
            uint petId = 0;
            if (selectIndex >= mountPets.Count)
            {
                var indexer = selectIndex - mountPets.Count;
                if (indexer < 0 || indexer >= mountIds.Count)
                {
                    if(mountPets.Count > 0)
                    {
                        selectIndex = 0;
                        SetShowPetInfo();
                        return;
                    }
                    else
                    {
                        selectIndex = -1;
                        return;
                    }
                }
                petId = mountIds[indexer];
                CSVPetMount.Data mountData = CSVPetMount.Instance.GetConfData(petId);
                if (null != mountData)
                {
                    showPets.Add(petId);
                    if (null != mountData.indenture_effect && mountData.indenture_effect.Count >= 2)
                    {
                        for (int i = 1; i < mountData.indenture_effect.Count; i++)
                        {
                            showPets.Add(0);
                        }

                        for (int i = 0; i < showPets.Count; i++)
                        {
                            uI_Pet_MountInfos[i].SetAttrValueRand(Sys_Pet.Instance.GetAttrListByGreatAndId((int)mountData.indenture_effect[i], 0));
                        }
                    }
                }
            }
            else
            {
                if (selectIndex < 0 || selectIndex >= mountPets.Count)
                {
                    selectIndex = -1;
                    return;
                }
                    
                petId = mountPets[selectIndex].petData.id;
                CSVPetMount.Data mountData = CSVPetMount.Instance.GetConfData(petId);
                if (null != mountData)
                {
                    showPets.Add(mountPets[selectIndex].GetPetUid());
                    if (null != mountData.indenture_effect && mountData.indenture_effect.Count >= 2)
                    {
                        for (int i = 1; i < mountData.indenture_effect.Count; i++)
                        {
                            ClientPet pet = Sys_Pet.Instance.GetPetByUId(mountPets[selectIndex].GetSubByIndex((i - 1)));
                            if(null != pet)
                            {
                                showPets.Add(pet.GetPetUid());
                            }
                            else
                            {
                                showPets.Add(0);
                            }
                        }
                        uint partnerUid = 0;
                        if (selectIndex >= 0)
                        {
                            if (selectIndex < mountPets.Count)
                            {
                                partnerUid = mountPets[selectIndex].GetPetUid();
                            }
                        }
                        for (int i = 0; i < showPets.Count; i++)
                        {
                            var clientPet = mountPets[selectIndex];
                            uint lowG = clientPet.GetPetMaxGradeCount() - clientPet.GetPetGradeCount();
                            uI_Pet_MountInfos[i].SetAttrValue(Sys_Pet.Instance.GetAttrListByGreatAndId((int)mountData.indenture_effect[i], lowG), partnerUid);
                        }
                    }

                    
                }
            }
        }

    }

    public void SetEnergyInfo()
    {
        var currentValue = Sys_Pet.Instance.RidingEnergy;
        var currentMax = CSVPetNewParam.Instance.GetConfData(61).value;
        energySlider.value = (currentValue + 0f) / currentMax;
        energyText.text = currentValue.ToString();
    }

    private void SetMountPetSkillInfo()
    {
        skillIds.Clear();
        skillIdsState.Clear();
        if (selectIndex >= 0)
        {
            uint petId = 0;
            if (selectIndex >= mountPets.Count)
            {
                petId = mountIds[selectIndex - mountPets.Count];
                CSVPetMount.Data mountData = CSVPetMount.Instance.GetConfData(petId);
                if (null != mountData)
                {
                    if (null != mountData.mount_skills)
                    {
                        for (int i = 0; i < mountData.mount_skills.Count; i++)
                        {
                            skillIds.Add(mountData.mount_skills[i][0]);
                            skillIdsState.Add(0);
                        }
                    }

                    for (int i = 0; i < mountData.skill_grid; i++)
                    {
                        skillIds.Add(0);
                        skillIdsState.Add(0);
                    }
                }
            }
            else
            {
                ClientPet pet = mountPets[selectIndex];
                petId = pet.petData.id;
                CSVPetMount.Data mountData = CSVPetMount.Instance.GetConfData(petId);
                skillIds = pet.GetMountSkill();
                for (int i = 0; i < skillIds.Count; i++)
                {
                    skillIdsState.Add(0);
                }
                int noralSkillCount = pet.GetRidingSkillCount();
                if (null != mountData)
                {
                    int maxSKillCount = (int)mountData.skill_grid;
                    for (int i = noralSkillCount; i < maxSKillCount; i++)
                    {
                        skillIds.Add(0);
                        skillIdsState.Add(0);
                    }

                    if (pet.GetPetIsDomestication())
                    {
                        var unlockLevel = pet.ContractLevel;
                        List<uint> exSkillsLevel = new List<uint>();
                        uint startLevel = 0;
                        for (int i = 0; i < CSVPetMountStrengthen.Instance.Count; i++)
                        {
                            var tempData = CSVPetMountStrengthen.Instance.GetByIndex(i);
                            if(null != tempData && pet.mountData.strengthen_type == tempData.type)
                            {
                                if (startLevel < tempData.extra_skill_grid)
                                {
                                    startLevel = tempData.level;
                                    exSkillsLevel.Add(startLevel);
                                }
                            }
                        }
                        maxSKillCount += exSkillsLevel.Count;
                        if (noralSkillCount > mountData.skill_grid)
                        {
                            int disCount = maxSKillCount - noralSkillCount;
                            int starIndex = exSkillsLevel.Count - disCount;
                            for (int i = starIndex; i < disCount; i++)
                            {
                                skillIds.Add(0);
                                skillIdsState.Add(unlockLevel >= exSkillsLevel[i]? 0: exSkillsLevel[i]);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < exSkillsLevel.Count; i++)
                            {
                                skillIds.Add(0);
                                skillIdsState.Add(unlockLevel >= exSkillsLevel[i] ? 0 : exSkillsLevel[i]);
                            }
                        }
                    }
                }
            }
        }

        int count = skillIds.Count;
        FrameworkTool.CreateChildList(skillTrans, count);
        for (int i = 0; i < count; i++)
        {
            Transform trans = skillTrans.GetChild(i);
            UI_Pet_MountSkillCeil ceil = new UI_Pet_MountSkillCeil();
            ceil.BingGameObject(trans.gameObject);
            ceil.AddClickListener(SkillClicked);
            uint skillId = skillIds[i];
            ceil.SetData(skillId, true, skillIdsState[i]);
        }
    }

    private void SetButtonState()
    {
        if (selectIndex >= 0)
        {
            if (selectIndex >= mountPets.Count)
            {
                contractLeveBtn.gameObject.SetActive(false);
                mountTipBtn.transform.Find("Text_01").GetComponent<Text>().text = LanguageHelper.GetTextContent(60513u);
                mountTipBtn.gameObject.SetActive(true);
            }
            else
            {

                ClientPet clientPet = mountPets[selectIndex];
                bool isDomestication = clientPet.GetPetIsDomestication();
                contractLeveBtn.gameObject.SetActive(isDomestication && Sys_FunctionOpen.Instance.IsOpen(10592));
                if (!isDomestication)
                {
                    mountTipBtn.transform.Find("Text_01").GetComponent<Text>().text = LanguageHelper.GetTextContent(60512u);
                }
                else
                {
                    var levelText = contractLeveBtn.transform.Find("Text_Num").GetComponent<Text>();
                    bool showLevelText = clientPet.ContractLevel > 0;
                    if (showLevelText)
                        levelText.text = clientPet.ContractLevel.ToString();
                    levelText.gameObject.SetActive(showLevelText);
                }
                
                mountTipBtn.gameObject.SetActive(!isDomestication);
            }
        }
        else
        {
            contractLeveBtn.gameObject.SetActive(false);
        }
    }

    private void SetNoneState()
    {
        noneGo.SetActive(selectIndex < 0);
    }

    #region ClickEvent
    private void OnEnergyBtnClicked()
    {
        UIManager.OpenUI(EUIID.UI_Pet_MountCharge);
    }

    private void MountTipBtnClicked()
    {
        if (selectIndex >= 0)
        {
            if (selectIndex >= mountPets.Count)
            {
                uint petId = mountIds[selectIndex - mountPets.Count];

                UIManager.OpenUI(EUIID.UI_Pet_BookReview, false, petId);
            }
            else
            {
                CSVPetNewParam.Data cSVPetParameterData = CSVPetNewParam.Instance.GetConfData(37u);
                if (null != cSVPetParameterData)
                {
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVPetParameterData.value);
                    UIManager.CloseUI(EUIID.UI_Pet_Message, needDestroy: false);
                }
            }
        }
        
    }

    private void OnMoutScreemBtnClicked()
    {
        UIManager.OpenUI(EUIID.UI_Pet_MountScreem);
    }

    private void OnCellClicked(int index)
    {
        if(selectIndex  != index)
        {
            selectIndex = index;
            SelectState();
            UnLoadShowScene();
            SetShowPetInfo();
            SetMountPetSkillInfo();
            SetButtonState();
            LoadShowScene();
        }
    }

    private void SkillClicked(UI_Pet_MountSkillCeil skillCeil)
    {
        uint skillId = skillCeil.petSkillBase.skillId;
        if(0 != skillId)
        {
            ClientPet clientPet = null;
            if (selectIndex >= 0)
            {
                if (selectIndex < mountPets.Count)
                {
                    clientPet = mountPets[selectIndex];
                }
            }
            Sys_Pet.Instance.CheckGetSkillCostReset();
            UI_MountSkill_TipsParam param = new UI_MountSkill_TipsParam();
            param.pet = clientPet;
            param.skillId = skillId;
            UIManager.OpenUI(EUIID.UI_MountSkill_Tips, false, param);
        }
        else
        {
            if(Sys_Pet.Instance.MountIsOpen)
            {
                ClientPet clientPet = null;
                if (selectIndex >= 0)
                {
                    if (selectIndex < mountPets.Count)
                    {
                        clientPet = mountPets[selectIndex];
                    }
                }
                if(clientPet == null)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000208));
                }
                else
                {

                    if (clientPet.petUnit.SimpleInfo.ExpiredTick > 0)//限时
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000207));
                        return;
                    }
                    else if (!clientPet.GetPetIsDomestication())
                    {
                        PromptBoxParameter.Instance.Clear();
                        PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680000209);
                        PromptBoxParameter.Instance.SetConfirm(true, () =>
                        {
                            if (clientPet != null)
                            {
                                CSVPetNewParam.Data cSVPetParameterData = CSVPetNewParam.Instance.GetConfData(37u);
                                if (null != cSVPetParameterData)
                                {
                                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVPetParameterData.value);
                                    UIManager.CloseUI(EUIID.UI_Pet_Message, needDestroy: false);
                                }
                            }
                        });
                        PromptBoxParameter.Instance.SetCancel(true, null);
                        UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                        return;
                    }
                    if(skillCeil.isLock)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1018624, skillCeil.lockLevel.ToString()));
                        return;
                    }

                    UIManager.OpenUI(EUIID.UI_Pet_MountSkillSelectItem, false, new UI_SelectMountSkillParam
                    {
                        tittle_langId = 60520,
                        petUid = clientPet.GetPetUid()
                    }, EUIID.UI_Pet_Message);
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000222, Sys_Pet.Instance.MountSkillOpenLevel.ToString()));
            }
        }
    }

    #endregion
}

public class UI_Pet_MountInfo : UIComponent
{
    private RawImage rawImage;
    private Button addBtn;
    private Button changeBtn;
    private Transform attrGroup;
    private Transform attrRangeGroup;
    private int index;
    private uint partnerPetUid;
    private ClientPet PartnerPet
    {
        get
        {
            return Sys_Pet.Instance.GetPetByUId(partnerPetUid);
        }
    }
    protected override void Loaded()
    {
        rawImage = transform.Find("Show").GetComponent<RawImage>();
        attrGroup = transform.Find("AttrGroup");
        attrRangeGroup = transform.Find("AttrGroup_Range");
        changeBtn = rawImage.GetComponent<Button>();
        changeBtn.onClick.AddListener(ChangeBtnClicked);
        addBtn = transform.Find("Btn_Add").GetComponent<Button>();
        addBtn.onClick.AddListener(AddBtnClicked);
    }

    public void SetRawImage(RenderTexture texture, uint petId)
    {
        
        rawImage.texture = texture;
        bool hasPet = petId > 0;
        rawImage.gameObject.SetActive(hasPet);

        addBtn.gameObject.SetActive(!hasPet);
    }

    public void ClearRawImage()
    {
        this.partnerPetUid = 0;
        rawImage.texture = null;
        rawImage.gameObject.SetActive(false);
        addBtn.gameObject.SetActive(true);
    }

    public void Init(int index)
    {
        this.index = index;
        ClearRawImage();
    }

    public void SetAttrValue(CSVPetMountAttr.Data configData, uint partnerPetUid)
    {
        this.partnerPetUid = partnerPetUid;
        var vs = configData.base_attr;
        if (null != vs)
        {
            FrameworkTool.CreateChildList(attrGroup, vs.Count);
            float strengthenBonuP = 0f;
            var partnerPet = PartnerPet;
            if (null != partnerPet && null != partnerPet.mountData)
            {
                var strengtgenData = CSVPetMountStrengthen.Instance.GetMountPetIntensifyData(partnerPet.ContractLevel, partnerPet.mountData.strengthen_type);
                if (null != strengtgenData)
                    strengthenBonuP = strengtgenData.attribute_bonus / 10000f;
            }
            
            for (int i = 0; i < vs.Count; i++)
            {
                Transform trans = attrGroup.GetChild(i);
                CSVAttr.Data attrInfo = CSVAttr.Instance.GetConfData((uint)vs[i][0]);
                TextHelper.SetText(trans.Find("Text_Name").GetComponent<Text>(), attrInfo.name);
                float attr_value = vs[i][1] * (1 + strengthenBonuP);
                if (attr_value >= 0)
                {
                    TextHelper.SetText(trans.Find("Text_Value").GetComponent<Text>(), LanguageHelper.GetTextContent(2006142u, (attrInfo.show_type == 1 ? attr_value.ToString("0.##") : (attr_value / 100.0f).ToString("0.##") + "%")));
                }
                else
                {
                    TextHelper.SetText(trans.Find("Text_Value").GetComponent<Text>(), (attrInfo.show_type == 1 ? attr_value.ToString("0.##") : (attr_value / 100.0f).ToString("0.##") + "%"));
                }
            }
        }
        else
        {
            FrameworkTool.CreateChildList(attrGroup, 0);
        }
        attrRangeGroup.gameObject.SetActive(false);
        attrGroup.gameObject.SetActive(true);
    }

    public void SetAttrValueRand(CSVPetMountAttr.Data configData)
    {
        var vs = configData.base_attr;
        var vs2 = configData.base_attr_min;
        if (null != vs && null != vs2)
        {
            FrameworkTool.CreateChildList(attrRangeGroup, vs.Count);

            for (int i = 0; i < vs.Count; i++)
            {
                Transform trans = attrRangeGroup.GetChild(i);
                CSVAttr.Data attrInfo = CSVAttr.Instance.GetConfData((uint)vs[i][0]);
                TextHelper.SetText(trans.Find("Text_Name").GetComponent<Text>(), attrInfo.name);

                TextHelper.SetText(trans.Find("Text_Value").GetComponent<Text>(),
                            LanguageHelper.GetTextContent(680000300u, ((vs2[i][1] >= 0 ? "+" : "") + (attrInfo.show_type == 1 ? vs2[i][1].ToString("0.##") : (vs2[i][1] / 100.0f).ToString("0.##") + "%")), ((vs[i][1] >= 0 ? "+" : "") + (attrInfo.show_type == 1 ? vs[i][1].ToString("0.##") : (vs[i][1] / 100.0f).ToString("0.##") + "%"))));

            }
        }
        else
        {
            FrameworkTool.CreateChildList(attrGroup, 0);
        }
        attrRangeGroup.gameObject.SetActive(true);
        attrGroup.gameObject.SetActive(false);
    }

    private void AddBtnClicked()
    {
        if (index == 0)
            return;
        var partnerPet = PartnerPet;
        if (partnerPet == null)//图鉴宠物
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000200));
            return;
        }
        else if(!partnerPet.GetPetIsDomestication())//前往驯化
        {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680000201);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                CSVPetNewParam.Data cSVPetParameterData = CSVPetNewParam.Instance.GetConfData(37u);
                if (null != cSVPetParameterData)
                {
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVPetParameterData.value);
                    UIManager.CloseUI(EUIID.UI_Pet_Message, needDestroy: false);
                }
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            return;
        }
        else if(partnerPet.HasPartnerPet()) // 自己是其他宠物的副宠
        {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680000202);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                ClientPet contractPet = Sys_Pet.Instance.GetPetByUId(partnerPet.PartnerUid);
                if(null != contractPet)
                {
                    Sys_Pet.Instance.OnPetContractCancleReq(partnerPet.PartnerUid, new List<uint>() { (uint)contractPet.GetIndexByPetUid(partnerPet.GetPetUid())});
                }
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            return;
        }
        UI_Pet_MountSelectItemParam param = new UI_Pet_MountSelectItemParam();
        param.index = index - 1;
        param.petUid = partnerPet.GetPetUid();
        UIManager.OpenUI(EUIID.UI_Pet_MountSelectItem, false, param);
    }

    private void ChangeBtnClicked()
    {
        if (index == 0)
            return;
        UI_Pet_MountContractParam param = new UI_Pet_MountContractParam();
        param.index = index - 1;
        param.currentpet = Sys_Pet.Instance.GetPetByUId(PartnerPet.GetSubByIndex(index - 1));
        UIManager.OpenUI(EUIID.UI_Pet_MountContract, false, param);
    }
}


