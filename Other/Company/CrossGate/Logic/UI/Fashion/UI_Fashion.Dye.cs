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
        private RectTransform paintCanvas; //自定义画板
        private RectTransform brush;
        private Slider vSlider;             //明暗滑动条
        private Button saveDye;
        private Image compareFirst;
        private Image compareLast;
        private Button dyeBtn;              //点击染色按钮
        private GameObject colorRoot;       //染色节点
        private GameObject colorLeftRoot;   //染色左节点
        private GameObject colorSelect1;    //色板1
        private GameObject colorSelect2;    //定制
        private GameObject acceColorRoot;
        private Image csColor0;
        private Image csColor1;
        private Image csColor2;
        private Image csColor3;
        private Transform tintParent;
        private Transform dyePropRoot;

        private CP_ToggleRegistry CP_ToggleRegistry_DyeParent;//主页签
        private CP_ToggleRegistry CP_ToggleRegistry_DyeChild;//挂饰子页签
        private CP_ToggleRegistry CP_ToggleRegistry_Cs1;  //色板1
        private CP_ToggleRegistry CP_ToggleRegistry_Tint; //染色区域
        private CP_ToggleRegistry CP_ToggleRegistry_DyeMoudleSwitch;//色板 定制切换
        private CP_ToggleRegistry CP_ToggleRegistry_Scheme;//染色方案1 2
        private CP_Toggle m_CPToggle1;
        private CP_Toggle m_CPToggle2;
        private GameObject m_SchemeGo;
        private float H; //色相
        private float S; //饱和度
        private float V; //明暗
        float x;
        float y;
        private Color color;
        private ETintIndex curTintIndex = ETintIndex.None;   //记录当前界面时装部件的具体着色部位
        private ETintIndex CurTintIndex
        {
            get { return curTintIndex; }
            set
            {
                if (curTintIndex != value)
                {
                    curTintIndex = value;
                    if (curdyeMoudle == 0)
                    {
                        SetColorSelect1Root();
                    }
                }
            }
        }
        private Dictionary<EHeroModelParts, Dictionary<ETintIndex, int>> colorCaches = new Dictionary<EHeroModelParts, Dictionary<ETintIndex, int>>();//用来存储色板 选中块具体部位某个通道对应哪个颜色块
        private EHeroModelParts curSelectModelParts;

        private int curDyeParentLable = -1;
        private int CurDyeParentLable
        {
            get { return curDyeParentLable; }
            set
            {
                if (curDyeParentLable != value)
                {
                    curDyeParentLable = value;
                    OnParentColorLableChanged();
                }
            }
        }

        private bool bacceColorVaild = false;                        //控制挂饰子页签的显示隐藏
        private bool bAcceColorVaild
        {
            get { return bacceColorVaild; }
            set
            {
                if (bacceColorVaild != value)
                {
                    bacceColorVaild = value;
                    SetAcceColorRoot(bacceColorVaild);
                }
            }
        }
        private bool bdressMode = false;
        private bool bDressMode
        {
            get { return bdressMode; }
            set
            {
                if (bdressMode != value)
                {
                    bdressMode = value;
                    if (bdressMode)
                    {
                        SwitchToDye();
                    }
                    else
                    {
                        SwitchToFashion();
                        RevertToLastColorAll();
                    }
                }
            }
        }

        private int curdyeMoudle = 0;//染色模式( 定制,色板)

        private int m_CurrentScheme = -1;//方案
        private int currentScheme
        {
            get
            {
                return m_CurrentScheme;
            }
            set
            {
                if (m_CurrentScheme != value)
                {
                    m_CurrentScheme = value;
                    OnCurrentSchemeChanged();
                }
            }
        }

        private bool dyePropEnough = false;

        private void ParseDyeComponent()
        {
            colorRoot = transform.Find("Animator/View_Color").gameObject;
            Part_Slider = transform.Find("Animator/View_Left/Part_Slider").gameObject;
            saveDye = colorRoot.transform.Find("Btn_Save01").GetComponent<Button>();
            dyePropRoot = colorRoot.transform.Find("View_Cost");
            colorSelect1 = colorRoot.transform.Find("SelectColor01").gameObject;
            colorSelect2 = colorRoot.transform.Find("SelectColor02").gameObject;
            csColor0 = colorSelect1.transform.Find("Grid1/ToggleColor/Image_Color").GetComponent<Image>();
            csColor1 = colorSelect1.transform.Find("Grid1/ToggleColor (1)/Image_Color").GetComponent<Image>();
            csColor2 = colorSelect1.transform.Find("Grid1/ToggleColor (2)/Image_Color").GetComponent<Image>();
            csColor3 = colorSelect1.transform.Find("Grid1/ToggleColor (3)/Image_Color").GetComponent<Image>();
            paintCanvas = colorSelect2.transform.Find("Btn_Color") as RectTransform;
            brush = colorSelect2.transform.Find("Btn_Color/Image_Select") as RectTransform;
            vSlider = colorSelect2.transform.Find("Image_Light/_Slider").GetComponent<Slider>();
            acceColorRoot = transform.Find("Animator/View_Color/List_Menu/TabList02").gameObject;
            colorLeftRoot = transform.Find("Animator/View_Left/Color").gameObject;
            compareFirst = colorLeftRoot.transform.Find("Part_Comtrast/Btn_ComtrastFirst").GetComponent<Image>();
            compareLast = colorLeftRoot.transform.Find("Part_Comtrast/Btn_ComtrastLast").GetComponent<Image>();
            modelSlider = transform.Find("Animator/View_Left/Part_Slider/Slider").GetComponent<Slider>();
            CP_ToggleRegistry_DyeParent = transform.Find("Animator/View_Color/List_Menu/TabList01").GetComponent<CP_ToggleRegistry>();
            CP_ToggleRegistry_DyeChild = acceColorRoot.GetComponent<CP_ToggleRegistry>();
            CP_ToggleRegistry_Cs1 = colorSelect1.transform.Find("Grid1").GetComponent<CP_ToggleRegistry>();
            CP_ToggleRegistry_DyeMoudleSwitch = transform.Find("Animator/View_Left/Color/SelectList01").GetComponent<CP_ToggleRegistry>();
            CP_ToggleRegistry_Scheme = transform.Find("Animator/View_Left/Color/Plan").GetComponent<CP_ToggleRegistry>();
            m_CPToggle1 = transform.Find("Animator/View_Left/Color/Plan/Toggle_1").GetComponent<CP_Toggle>();
            m_CPToggle2 = transform.Find("Animator/View_Left/Color/Plan/Toggle_2").GetComponent<CP_Toggle>();
            m_SchemeGo = transform.Find("Animator/View_Left/Color/Plan").gameObject;
            tintParent = transform.Find("Animator/View_Color/Select_Part/Grid1");
            CP_ToggleRegistry_Tint = tintParent.transform.GetComponent<CP_ToggleRegistry>();
            dyeBtn = transform.Find("Animator/View_Left/Fashion/Btn_Color").GetComponent<Button>();
            backBtn = transform.Find("Animator/View_Left/Color/Btn_Back").GetComponent<Button>();
            assetDependencies = transform.GetComponent<AssetDependencies>();
            SetModelSliderActive(false);
        }

        private void RegisterDyeEvent()
        {
            dyeBtn.onClick.AddListener(() => bDressMode = true);
            backBtn.onClick.AddListener(() => bDressMode = false);
            saveDye.onClick.AddListener(OnSaveDye);

            CP_ToggleRegistry_DyeParent.onToggleChange = onParentColorToggleChanged;
            CP_ToggleRegistry_DyeChild.onToggleChange = OnChildColorToggleChanged;
            CP_ToggleRegistry_Cs1.onToggleChange = OnColorSelect1ToggleChanged;
            CP_ToggleRegistry_Tint.onToggleChange = OnTintChanged;
            CP_ToggleRegistry_DyeMoudleSwitch.onToggleChange = OnColorMoudleSwitch;
            CP_ToggleRegistry_Scheme.onToggleChange = OnSchemeToggleChanged;

            vSlider.onValueChanged.AddListener(OnSliderValueChanged);
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(paintCanvas);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnPointClick);
        }

        private void SwitchToFashion()
        {
            curDyeParentLable = -1;
            m_CurrentScheme = -1;
            viewModel = 0;
            colorRoot.SetActive(false);
            colorLeftRoot.SetActive(false);
            fashionRoot.SetActive(true);
            backBtn.gameObject.SetActive(false);
            SetModelSliderActive(false);
            compareFirst.gameObject.SetActive(false);
            compareLast.gameObject.SetActive(false);
            revertToFirst.gameObject.SetActive(true);
            SetColorBtnActive();
            bAcceColorVaild = false;
            colorCaches.Clear();
            curTintIndex = ETintIndex.None;

            if (curFashionClothes != null)
            {
                curFashionClothes.ResetCurUseColor();
            }
            if (curFashionWeapon != null)
            {
                curFashionWeapon.ResetCurUseColor();
            }
            if (curFashionAccessory != null)
            {
                curFashionAccessory.ResetCurUseColor();
            }
            UpdateDyeButtonState(false);
        }

        private void SwitchToDye()
        {
            viewModel = 1;
            curdyeMoudle = 0;
            colorRoot.SetActive(true);
            colorLeftRoot.SetActive(true);
            fashionRoot.SetActive(false);
            SetColorBtnActive(false);
            backBtn.gameObject.SetActive(true);
            SetModelSliderActive(true);
            modelSlider.gameObject.SetActive(true);
            compareFirst.gameObject.SetActive(true);
            revertToFirst.gameObject.SetActive(false);
            UpdateDyeButtonState(false);
            ClearCurUseColor();
            CP_ToggleRegistry_DyeMoudleSwitch.SwitchTo(curdyeMoudle);
            SetDyePropRoot();
            CP_ToggleRegistry_DyeParent.SwitchTo(curFashionType);
            UpdateCompareLastShowOrHide();
            OnUpdateSchemeLable();
        }

        #region MoudleSwitch
        private void OnColorMoudleSwitch(int curToggle, int old)
        {
            if (curToggle == old)
                return;

            ClearMoudleSwitch();
            SetTintColorRoot();
            UpdateDyeButtonState(false);

            curdyeMoudle = curToggle;
            SetBrushPosition(curTintIndex);
            SetDyePropRoot();
            RevertToLastColorAll();
        }

        private void ClearMoudleSwitch()
        {
            colorCaches.Clear();
            foreach (var item in CP_ToggleRegistry_Cs1.toggles)
            {
                item.SetSelected(false, false);
            }
            ClearCurUseColor();
        }
        #endregion

        #region Scheme
        private void OnUpdateSchemeLable()
        {
            m_CPToggle1.SetToggleIsNotChange(false);
            m_CPToggle2.SetToggleIsNotChange(false);
            if (curDyeParentLable == 1)
            {
                if (!curFashionClothes.UnLock)
                {
                    m_CurrentScheme = -1;
                }
                else
                {
                    m_CurrentScheme = curFashionClothes.curUseScheme;
                }
            }
            else if (curDyeParentLable == 2)
            {
                if (!curFashionWeapon.UnLock)
                {
                    m_CurrentScheme = -1;
                }
                else
                {
                    m_CurrentScheme = curFashionWeapon.curUseScheme;
                }
            }
            else if (curDyeParentLable == 3)
            {
                if (!curFashionAccessory.UnLock)
                {
                    m_CurrentScheme = -1;
                }
                else
                {
                    m_CurrentScheme = curFashionAccessory.curUseScheme;
                }
            }
            if (m_CurrentScheme != -1)
            {
                if (!m_SchemeGo.activeSelf)
                {
                    m_SchemeGo.SetActive(true);
                }
                if (m_CurrentScheme == 0)
                {
                    m_CPToggle1.SetToggleIsNotChange(true);
                }
                else if (m_CurrentScheme == 1)
                {
                    m_CPToggle2.SetToggleIsNotChange(true);
                }
                CP_ToggleRegistry_Scheme.SwitchTo(m_CurrentScheme);
            }
            else
            {
                if (m_SchemeGo.activeSelf)
                {
                    m_SchemeGo.SetActive(false);
                }
            }
        }

        private void OnSchemeToggleChanged(int current, int old)
        {
            if (current == old)
            {
                return;
            }
            if (current == 0)
            {
                m_CPToggle2.SetToggleIsNotChange(false);
            }
            else if (current == 1)
            {
                m_CPToggle1.SetToggleIsNotChange(false);
            }
            currentScheme = current;
        }

        private void OnCurrentSchemeChanged()
        {
            colorCaches.Clear();
            foreach (var item in CP_ToggleRegistry_Cs1.toggles)
            {
                item.SetSelected(false, false);
            }
            ResetCurUseColorToScheme(m_CurrentScheme);

            SetTintColorRoot();
            UpdateDyeButtonState(false);
            if (curDyeParentLable == 1)
            {
                if (curFashionClothes != null)
                {
                    Color color_r = curFashionClothes.GetColor(m_CurrentScheme, ETintIndex.R);
                    SetColor(EHeroModelParts.Main, ETintIndex.R, color_r);

                    Color color_g = curFashionClothes.GetColor(m_CurrentScheme, ETintIndex.G);
                    SetColor(EHeroModelParts.Main, ETintIndex.G, color_g);

                    Color color_b = curFashionClothes.GetColor(m_CurrentScheme, ETintIndex.B);
                    SetColor(EHeroModelParts.Main, ETintIndex.B, color_b);

                    Color color_a = curFashionClothes.GetColor(m_CurrentScheme, ETintIndex.A);
                    SetColor(EHeroModelParts.Main, ETintIndex.A, color_a);

                    Sys_Fashion.Instance.FashionSwitchDyeReq(curFashionClothes.Id, (uint)m_CurrentScheme);
                }
            }
            else if (curDyeParentLable == 2)
            {
                if (curFashionWeapon != null)
                {
                    Color color_r = curFashionWeapon.GetColor(m_CurrentScheme, ETintIndex.R);
                    SetColor(EHeroModelParts.Weapon, ETintIndex.R, color_r);

                    Sys_Fashion.Instance.FashionSwitchDyeReq(curFashionWeapon.Id, (uint)m_CurrentScheme);
                }
            }
            else if (curDyeParentLable == 3)
            {
                if (curFashionAccessory != null)
                {
                    EHeroModelParts eHeroModelParts = EHeroModelParts.None;
                    Color color_r = curFashionAccessory.GetColor(m_CurrentScheme, ETintIndex.R);
                    eHeroModelParts = Sys_Fashion.Instance.parts[curFashionAccessory.Id];
                    SetColor(eHeroModelParts, ETintIndex.R, color_r);

                    Sys_Fashion.Instance.FashionSwitchDyeReq(curFashionAccessory.Id, (uint)m_CurrentScheme);
                }
            }
        }

        #endregion

        #region LableChange
        private void onParentColorToggleChanged(int curToggle, int old)
        {
            if (curToggle == 3)
            {
                bAcceColorVaild = true;
            }
            else
            {
                bAcceColorVaild = false;
            }
            CurDyeParentLable = curToggle;
        }

        // 染色界面主页签切换(武器 时装等部位切换)
        private void OnParentColorLableChanged()
        {
            OnUpdateSchemeLable();
            ClearMoudleSwitch();
            RevertToLastColorAll();

            if (curDyeParentLable == 3)
            {
                CP_ToggleRegistry_DyeChild.SwitchTo(4);
                bAcceColorVaild = true;
            }
            else
            {
                uint id = 0;
                if (curDyeParentLable == 1)
                {
                    if (showParts.ContainsKey(EHeroModelParts.Main))
                    {
                        id = showParts[EHeroModelParts.Main];
                        FashionClothes fashionClothes = Sys_Fashion.Instance._FashionClothes.Find(x => x.Id == id);
                        if (fashionClothes != null)
                        {
                            //if (fashionClothes.UnLock)
                            //{
                            curSelectModelParts = EHeroModelParts.Main;
                            SetTintColorRoot();
                            SetColorSelect1Root();
                            SetDyePropRoot();
                            //}
                            //else
                            //{
                            //    string str = CSVLanguage.Instance.GetConfData(2009552).words;
                            //    Sys_Hint.Instance.PushContent_Normal(str);
                            //}
                            if (fashionClothes.cSVFashionClothesData.ColorSwitch == 0)
                            {
                                curdyeMoudle = 0;
                                CP_ToggleRegistry_DyeMoudleSwitch.SwitchTo(curdyeMoudle);
                            }
                        }
                    }
                    else
                    {
                        string str = CSVLanguage.Instance.GetConfData(2009552).words;
                        Sys_Hint.Instance.PushContent_Normal(str);
                    }
                }
                if (curDyeParentLable == 2)
                {
                    if (showParts.ContainsKey(EHeroModelParts.Weapon))
                    {
                        id = showParts[EHeroModelParts.Weapon];
                        FashionWeapon fashionWeapon = Sys_Fashion.Instance._FashionWeapons.Find(x => x.Id == id);
                        if (fashionWeapon != null)
                        {
                            //if (fashionWeapon.UnLock)
                            //{
                            curSelectModelParts = EHeroModelParts.Weapon;
                            SetTintColorRoot();
                            SetColorSelect1Root();
                            SetDyePropRoot();
                            //}
                            //else
                            //{
                            //    string str = CSVLanguage.Instance.GetConfData(2009551).words;
                            //    Sys_Hint.Instance.PushContent_Normal(str);
                            //}
                            if (fashionWeapon.cSVFashionWeaponData.ColorSwitch == 0)
                            {
                                curdyeMoudle = 0;
                                CP_ToggleRegistry_DyeMoudleSwitch.SwitchTo(curdyeMoudle);
                            }
                        }
                    }
                    else
                    {
                        string str = CSVLanguage.Instance.GetConfData(2009551).words;
                        Sys_Hint.Instance.PushContent_Normal(str);
                    }
                }
            }
            //UpdateRecentlyColor();
        }

        // 染色  挂饰列表页签切换
        private void OnChildColorToggleChanged(int curToggle, int old)
        {
            bAcceColorVaild = false;
            EHeroModelParts eHeroModelParts = (EHeroModelParts)curToggle;
            if (showParts.ContainsKey(eHeroModelParts))
            {
                uint id = showParts[eHeroModelParts];
                FashionAccessory fashionAccessory = Sys_Fashion.Instance._FashionAccessories.Find(x => x.Id == id);
                //if (fashionAccessory.UnLock)
                //{
                curFashionAccessory = fashionAccessory;
                curSelectModelParts = eHeroModelParts;
                SetTintColorRoot();
                SetColorSelect1Root();
                SetDyePropRoot();
                //}
                //else
                //{
                //    string content = string.Empty;
                //    switch (eHeroModelParts)
                //    {
                //        case EHeroModelParts.Jewelry_Head:
                //            content = CSVLanguage.Instance.GetConfData(2009537).words;
                //            break;
                //        case EHeroModelParts.Jewelry_Back:
                //            content = CSVLanguage.Instance.GetConfData(2009540).words;
                //            break;
                //        case EHeroModelParts.Jewelry_Waist:
                //            content = CSVLanguage.Instance.GetConfData(2009538).words;
                //            break;
                //        case EHeroModelParts.Jewelry_Face:
                //            content = CSVLanguage.Instance.GetConfData(2009539).words;
                //            break;
                //        default:
                //            break;
                //    }
                //    Sys_Hint.Instance.PushContent_Normal(content);
                //}
            }
            else
            {
                string content = string.Empty;
                switch (eHeroModelParts)
                {
                    case EHeroModelParts.Jewelry_Head:
                        content = CSVLanguage.Instance.GetConfData(2009537).words;
                        break;
                    case EHeroModelParts.Jewelry_Back:
                        content = CSVLanguage.Instance.GetConfData(2009540).words;
                        break;
                    case EHeroModelParts.Jewelry_Waist:
                        content = CSVLanguage.Instance.GetConfData(2009538).words;
                        break;
                    case EHeroModelParts.Jewelry_Face:
                        content = CSVLanguage.Instance.GetConfData(2009539).words;
                        break;
                    default:
                        break;
                }
                Sys_Hint.Instance.PushContent_Normal(content);
            }
        }
        #endregion

        #region Prop
        private void SetDyePropRoot()
        {
            List<List<uint>> items = new List<List<uint>>();
            if (curdyeMoudle == 0)
            {
                if (curDyeParentLable == 1)
                {
                    items = curFashionClothes.cSVFashionClothesData.itemNeed;
                }
                if (curDyeParentLable == 2)
                {
                    items = curFashionWeapon.cSVFashionWeaponData.itemNeed;
                }
                if (curDyeParentLable == 3)
                {
                    items = curFashionAccessory.cSVFashionAccessoryData.itemNeed;
                }
            }
            else
            {
                if (curDyeParentLable == 1)
                {
                    items = curFashionClothes.cSVFashionClothesData.itemNeed1;
                }
                if (curDyeParentLable == 2)
                {
                    items = curFashionWeapon.cSVFashionWeaponData.itemNeed1;
                }
                if (curDyeParentLable == 3)
                {
                    items = curFashionAccessory.cSVFashionAccessoryData.itemNeed1;
                }
            }
            int count = items.Count;
            dyePropEnough = true;
            for (int i = 0; i < dyePropRoot.childCount; i++)
            {
                GameObject go = dyePropRoot.GetChild(i).gameObject;
                if (i < count)
                {
                    if (!go.activeSelf)
                    {
                        go.SetActive(true);
                    }

                    uint itemId = items[i][0];
                    uint needCount = items[i][1];
                    bool enough = Sys_Bag.Instance.GetItemCount(items[i][0]) >= needCount;
                    if (!enough)
                    {
                        dyePropEnough = false;
                    }

                    PropItem propItem = new PropItem();
                    propItem.BindGameObject(go);
                    PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                               (_id: itemId,
                               _count: needCount,
                               _bUseQuailty: true,
                               _bBind: false,
                               _bNew: false,
                               _bUnLock: false,
                               _bSelected: false,
                               _bShowCount: true,
                               _bShowBagCount: true,
                               _bUseClick: true,
                               _onClick: null,
                               _bshowBtnNo: true);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_Fashion, showItem));
                }
                else
                {
                    go.SetActive(false);
                }
            }
        }
        #endregion

        #region Tint

        // 4个染色方案 定制模块 toggle切换 
        private void OnColorSelect1ToggleChanged(int curToggle, int old)
        {
            colorCaches.TryGetValue(curSelectModelParts, out Dictionary<ETintIndex, int> val);
            if (val == null)
            {
                val = new Dictionary<ETintIndex, int>();
            }
            val[curTintIndex] = curToggle;
            colorCaches[curSelectModelParts] = val;

            SetTintColor(curTintIndex, curToggle, true, false);  //定制选颜色 需要同时染色
            color = GetConfigColor(curTintIndex, colorCaches[curSelectModelParts][curTintIndex]);
            if (curDyeParentLable == 1)
            {
                curFashionClothes.SetCurUseColor(curTintIndex, color);
            }
            else if (curDyeParentLable == 2)
            {
                curFashionWeapon.SetCurUseColor(curTintIndex, color);
            }
            else if (curDyeParentLable == 3)
            {
                curFashionAccessory.SetCurUseColor(curTintIndex, color);
            }
            CheckDirty(color);
        }

        private void OnTintChanged(int curToggle, int old)
        {
            CurTintIndex = (ETintIndex)curToggle;
            SetBrushPosition(curTintIndex);
        }

        //初始化所有部位节点(切换外观 武器 染色主页签（色板 定制（需要重新设置））的时候 都需要调用此接口 用于刷新部位节点)
        private void SetTintColorRoot(bool needdye = false)
        {
            uint count = 0;
            if (curDyeParentLable == 1)
            {
                count = curFashionClothes.cSVFashionClothesData.MaxColour - 1;
            }
            if (curDyeParentLable == 2)
            {
                count = curFashionWeapon.cSVFashionWeaponData.MaxColour;
            }
            if (curDyeParentLable == 3)
            {
                count = curFashionAccessory.cSVFashionAccessoryData.MaxColour;
            }
            for (int i = 0; i < tintParent.childCount; i++)
            {
                GameObject go = tintParent.GetChild(i).gameObject;
                if (i < count)
                {
                    if (!go.activeSelf)
                    {
                        go.SetActive(true);
                    }
                    SetTintColor((ETintIndex)i, 0, needdye, true);
                }
                else
                {
                    go.SetActive(false);
                }
                if (curDyeParentLable == 1 && i == tintParent.childCount - 1)
                {
                    tintParent.GetChild(i).gameObject.SetActive(true);
                    SetTintColor((ETintIndex)i, 0, needdye, true);
                }
            }
            CP_ToggleRegistry_Tint.SwitchTo(0);
        }

        //设置单个部位节点颜色
        private void SetTintColor(ETintIndex tintIndex, int curSelectcolorIndex, bool needDye = true, bool firstSet = false)
        {
            Color color = Color.white;
            if (firstSet)
            {
                if (curDyeParentLable == 1)
                {
                    int scheme = m_CurrentScheme;
                    if (scheme == -1)
                    {
                        scheme = curFashionClothes.curUseScheme;
                    }
                    if (scheme == -1)
                    {
                        color = curFashionClothes.GetFirstColor(tintIndex);
                    }
                    else
                    {
                        color = curFashionClothes.GetColor(scheme, tintIndex);
                    }
                }
                if (curDyeParentLable == 2)
                {
                    int scheme = m_CurrentScheme;
                    if (scheme == -1)
                    {
                        scheme = curFashionWeapon.curUseScheme;
                    }
                    if (scheme == -1)
                    {
                        color = curFashionWeapon.GetFirstColor(tintIndex);
                    }
                    else
                    {
                        color = curFashionWeapon.GetColor(scheme, tintIndex);
                    }
                }
                if (curDyeParentLable == 3)
                {
                    int scheme = m_CurrentScheme;
                    if (scheme == -1)
                    {
                        scheme = curFashionAccessory.curUseScheme;
                    }
                    if (scheme == -1)
                    {
                        color = curFashionAccessory.GetFirstColor(tintIndex);
                    }
                    else
                    {
                        color = curFashionAccessory.GetColor(scheme, tintIndex);
                    }
                }
            }
            else
            {
                color = GetConfigColor(tintIndex, curSelectcolorIndex);
            }
            tintParent.GetChild((int)tintIndex).Find("Image_Color").GetComponent<Image>().color = color;
            if (tintIndex == ETintIndex.R)
            {
                TextHelper.SetText(tintParent.GetChild((int)tintIndex).Find("Text").GetComponent<Text>(), 2009541);
            }
            else if (tintIndex == ETintIndex.G)
            {
                TextHelper.SetText(tintParent.GetChild((int)tintIndex).Find("Text").GetComponent<Text>(), 2009542);
            }
            else if (tintIndex == ETintIndex.B)
            {
                TextHelper.SetText(tintParent.GetChild((int)tintIndex).Find("Text").GetComponent<Text>(), 2009543);
            }
            else if (tintIndex == ETintIndex.A)
            {
                TextHelper.SetText(tintParent.GetChild((int)tintIndex).Find("Text").GetComponent<Text>(), 2009544);
            }
            if (needDye)
            {
                SetColor(tintIndex, color);
            }
        }

        //设置定制节点
        private void SetColorSelect1Root()
        {
            Color color0 = GetConfigColor(curTintIndex, 0);
            Color color1 = GetConfigColor(curTintIndex, 1);
            Color color2 = GetConfigColor(curTintIndex, 2);
            Color color3 = GetConfigColor(curTintIndex, 3);
            SetCsColor(color0, color1, color2, color3);
            if (colorCaches.ContainsKey(curSelectModelParts))
            {
                if (colorCaches[curSelectModelParts].ContainsKey(curTintIndex))
                {
                    CP_ToggleRegistry_Cs1.SwitchTo(colorCaches[curSelectModelParts][curTintIndex]);
                }
                else
                {
                    foreach (var item in CP_ToggleRegistry_Cs1.toggles)
                    {
                        item.SetSelected(false, false);
                    }
                }
            }
            else
            {
                foreach (var item in CP_ToggleRegistry_Cs1.toggles)
                {
                    item.SetSelected(false, false);
                }
                UpdateDyeButtonState(false);
            }
        }

        private void SetCsColor(Color color0, Color color1, Color color2, Color color3)
        {
            csColor0.color = color0;
            csColor1.color = color1;
            csColor2.color = color2;
            csColor3.color = color3;
        }

        #endregion

        #region OpCurUseColor
        private void ClearCurUseColor()
        {
            if (curFashionClothes != null)
            {
                curFashionClothes.ResetCurUseColor();
            }
            if (curFashionWeapon != null)
            {
                curFashionWeapon.ResetCurUseColor();
            }
            if (curFashionAccessory != null)
            {
                curFashionAccessory.ResetCurUseColor();
            }
        }

        private void ResetCurUseColorToScheme(int scheme)
        {
            if (curDyeParentLable == 1 && curFashionClothes != null)
            {
                curFashionClothes.ResetCurUseColorToScheme(scheme);
            }
            if (curDyeParentLable == 2 && curFashionWeapon != null)
            {
                curFashionWeapon.ResetCurUseColorToScheme(scheme);
            }
            if (curDyeParentLable == 3 && curFashionAccessory != null)
            {
                curFashionAccessory.ResetCurUseColorToScheme(scheme);
            }
        }

        #endregion

        #region Swatch
        private void SetBrushPosition(ETintIndex eTintIndex)
        {
            if (curdyeMoudle == 0)
                return;
            if (curDyeParentLable == 1)
            {
                Color color = curFashionClothes.GetCurUseColor(eTintIndex);
                float h = 0;
                float s = 0;
                float v = 0;
                Color.RGBToHSV(color, out h, out s, out v);
                brush.anchoredPosition = new Vector2(h * paintCanvas.rect.width, s * paintCanvas.rect.height);
            }
            if (curDyeParentLable == 2)
            {
                Color color = curFashionWeapon.GetCurUseColor(eTintIndex);
                float h = 0;
                float s = 0;
                float v = 0;
                Color.RGBToHSV(color, out h, out s, out v);
                brush.anchoredPosition = new Vector2(h * paintCanvas.rect.width, s * paintCanvas.rect.height);
            }
            if (curDyeParentLable == 3)
            {
                Color color = curFashionAccessory.GetCurUseColor(eTintIndex);
                float h = 0;
                float s = 0;
                float v = 0;
                Color.RGBToHSV(color, out h, out s, out v);
                brush.anchoredPosition = new Vector2(h * paintCanvas.rect.width, s * paintCanvas.rect.height);
            }
        }

        private void OnDrag(BaseEventData eventData)
        {
            Vector2 templocalPoint;
            PointerEventData pointerEventData = eventData as PointerEventData;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(paintCanvas as RectTransform,
                pointerEventData.position, pointerEventData.pressEventCamera, out templocalPoint);
            x = Mathf.Clamp(templocalPoint.x, 0, paintCanvas.rect.width);
            y = Mathf.Clamp(templocalPoint.y, 0, paintCanvas.rect.height);
            brush.anchoredPosition = new Vector2(x, y);
            H = templocalPoint.x / paintCanvas.rect.width;
            S = templocalPoint.y / paintCanvas.rect.height;
            H = Mathf.Clamp01(H);
            S = Mathf.Clamp01(S);
            V = vSlider.value;
            color = Color.HSVToRGB(H, S, V);
            UpdateTintColor(color);
            SetColor(curTintIndex, color);
            CheckDirty(color);
            if (curDyeParentLable == 1)
            {
                curFashionClothes.SetCurUseColor(curTintIndex, color);
            }
            else if (curDyeParentLable == 2)
            {
                curFashionWeapon.SetCurUseColor(curTintIndex, color);
            }
            else if (curDyeParentLable == 3)
            {
                curFashionAccessory.SetCurUseColor(curTintIndex, color);
            }
        }

        private void OnPointClick(BaseEventData eventData)
        {
            Vector2 templocalPoint;
            PointerEventData pointerEventData = eventData as PointerEventData;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(paintCanvas as RectTransform,
                pointerEventData.position, pointerEventData.pressEventCamera, out templocalPoint);
            x = Mathf.Clamp(templocalPoint.x, 0, paintCanvas.rect.width);
            y = Mathf.Clamp(templocalPoint.y, 0, paintCanvas.rect.height);
            brush.anchoredPosition = new Vector2(x, y);
            H = templocalPoint.x / paintCanvas.rect.width;
            S = templocalPoint.y / paintCanvas.rect.height;
            H = Mathf.Clamp01(H);
            S = Mathf.Clamp01(S);
            V = vSlider.value;
            color = Color.HSVToRGB(H, S, V);
            UpdateTintColor(color);
            SetColor(curTintIndex, color);
            CheckDirty(color);
            if (curDyeParentLable == 1)
            {
                curFashionClothes.SetCurUseColor(curTintIndex, color);
            }
            else if (curDyeParentLable == 2)
            {
                curFashionWeapon.SetCurUseColor(curTintIndex, color);
            }
            else if (curDyeParentLable == 3)
            {
                curFashionAccessory.SetCurUseColor(curTintIndex, color);
            }
        }

        private void OnSliderValueChanged(float v)
        {
            V = vSlider.value;
            color = Color.HSVToRGB(H, S, V);
            UpdateTintColor(color);
            SetColor(curTintIndex, color);
            CheckDirty(color);
        }

        private void UpdateTintColor(Color color)
        {
            tintParent.GetChild((int)curTintIndex).Find("Image_Color").GetComponent<Image>().color = color;
        }
        #endregion

        #region SetColor
        private void SetColor(ETintIndex eTintIndex, Color color)
        {
            if (curDyeParentLable == 1)
            {
                curFashionClothes.SetCurUseColor(eTintIndex, color);
                SetColor(EHeroModelParts.Main, eTintIndex, color);
            }
            if (curDyeParentLable == 2)
            {
                curFashionWeapon.SetCurUseColor(eTintIndex, color);
                SetColor(EHeroModelParts.Weapon, eTintIndex, color);
            }
            if (curDyeParentLable == 3)
            {
                curFashionAccessory.SetCurUseColor(eTintIndex, color);
                EHeroModelParts eHeroModelParts = (EHeroModelParts)curFashionAccessory.GetAcceType();
                SetColor(eHeroModelParts, eTintIndex, color);
            }
        }

        private void SetColor(EHeroModelParts eHeroModelParts, ETintIndex eTintIndex, Color color)
        {
            heroDisplay.SetColor(eHeroModelParts, eTintIndex, color);
        }

        #endregion

        #region CheckDirty
        private void OnCheckDirty()
        {
            CheckDirty(color);
        }

        private void CheckDirty(Color color)
        {
            if (curTintIndex == ETintIndex.None)
                return;
            Color32 nextColor = color;
            if (curDyeParentLable == 1)
            {
                curFashionClothes.CheckDirty(curTintIndex, nextColor);
            }
            else if (curDyeParentLable == 2)
            {
                curFashionWeapon.CheckDirty(curTintIndex, nextColor);
            }
            else if (curDyeParentLable == 3)
            {
                curFashionAccessory.CheckDirty(curTintIndex, nextColor);
            }
        }
        #endregion

        #region ButtonState
        private void UpdateDyeButtonState(bool dirty)
        {
            if (dyePropEnough)
            {
                ButtonHelper.Enable(saveDye, dirty);
            }
            else
            {
                ButtonHelper.Enable(saveDye, false);
            }
        }

        private void SetColorBtnActive(bool active)
        {
            dyeBtn.gameObject.SetActive(active);
        }

        private void SetColorBtnActive()
        {
            if (curFashionType == 0)
            {
                dyeBtn.gameObject.SetActive(false);
            }
            if (curFashionType == 1)
            {
                dyeBtn.gameObject.SetActive(true);
            }
            if (curFashionType == 2)
            {
                dyeBtn.gameObject.SetActive(true);
            }
            if (curFashionType == 3)
            {
                dyeBtn.gameObject.SetActive(false);
            }
        }
        #endregion

        private void OnSaveDye()
        {
            if (curDyeParentLable == 1)
            {
                if (!curFashionClothes.UnLock)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002200));
                }
                else
                {
                    int toSaveScheme = 0;
                    if (curFashionClothes.SchemeCount == 0)
                    {
                        toSaveScheme = 1;
                        curFashionClothes.TryDyeFashion(curdyeMoudle == 1, toSaveScheme);
                    }
                    else if (curFashionClothes.SchemeCount == 1)
                    {
                        toSaveScheme = 1 - m_CurrentScheme;
                        string content = LanguageHelper.GetTextContent(590002205, (toSaveScheme + 1).ToString());
                        PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                        () =>
                        {
                            curFashionClothes.TryDyeFashion(curdyeMoudle == 1, toSaveScheme);
                        },
                        null);
                    }
                    else
                    {
                        toSaveScheme = m_CurrentScheme;
                        string content = LanguageHelper.GetTextContent(590002206, (toSaveScheme + 1).ToString());
                        PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                        () =>
                        {
                            curFashionClothes.TryDyeFashion(curdyeMoudle == 1, toSaveScheme);
                        },
                        null);
                    }
                }
            }
            if (curDyeParentLable == 2)
            {
                if (!curFashionWeapon.UnLock)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002200));
                }
                else
                {
                    int toSaveScheme = 0;
                    if (curFashionWeapon.SchemeCount == 0)
                    {
                        toSaveScheme = 1;
                        curFashionWeapon.TryDyeFashion(curdyeMoudle == 1, toSaveScheme);
                    }
                    else if (curFashionWeapon.SchemeCount == 1)
                    {
                        toSaveScheme = 1 - m_CurrentScheme;
                        string content = LanguageHelper.GetTextContent(590002205, (toSaveScheme + 1).ToString());
                        PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                        () =>
                        {
                            curFashionWeapon.TryDyeFashion(curdyeMoudle == 1, toSaveScheme);
                        },
                        null);
                    }
                    else
                    {
                        toSaveScheme = m_CurrentScheme;
                        string content = LanguageHelper.GetTextContent(590002206, (toSaveScheme + 1).ToString());
                        PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                        () =>
                        {
                            curFashionWeapon.TryDyeFashion(curdyeMoudle == 1, toSaveScheme);
                        },
                        null);
                    }
                }
            }
            if (curDyeParentLable == 3)
            {
                if (!curFashionAccessory.UnLock)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002200));
                }
                else
                {
                    int toSaveScheme = 0;
                    if (curFashionAccessory.SchemeCount == 0)
                    {
                        toSaveScheme = 1;
                        curFashionAccessory.TryDyeFashion(curdyeMoudle == 1, toSaveScheme);
                    }
                    else if (curFashionAccessory.SchemeCount == 1)
                    {
                        toSaveScheme = 1 - m_CurrentScheme;
                        string content = LanguageHelper.GetTextContent(590002205, toSaveScheme.ToString());
                        PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                        () =>
                        {
                            curFashionAccessory.TryDyeFashion(curdyeMoudle == 1, toSaveScheme);
                        },
                        null);
                    }
                    else
                    {
                        toSaveScheme = m_CurrentScheme;
                        string content = LanguageHelper.GetTextContent(590002206, (toSaveScheme + 1).ToString());
                        PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                        () =>
                        {
                            curFashionAccessory.TryDyeFashion(curdyeMoudle == 1, toSaveScheme);
                        },
                        null);
                    }
                }
            }
        }

        //保存方案成功(需要切换到要保存的方案上去)
        private void OnSaveDyeSuccess(int toSaveScheme)
        {
            if (m_CurrentScheme == toSaveScheme)
            {
                return;
            }
            if (toSaveScheme == 0)
            {
                m_CPToggle1.SetToggleIsNotChange(true);
            }
            else if (toSaveScheme == 1)
            {
                m_CPToggle2.SetToggleIsNotChange(true);
            }
            CP_ToggleRegistry_Scheme.SwitchTo(toSaveScheme);
            if (toSaveScheme == 0)
            {
                m_CPToggle1.SetToggleIsNotChange(false);
            }
            else if (toSaveScheme == 1)
            {
                m_CPToggle2.SetToggleIsNotChange(false);
            }
        }

        private void SetAcceColorRoot(bool valid)
        {
            acceColorRoot.SetActive(valid);
        }

        //读取颜色初始配置
        private Color GetConfigColor(ETintIndex tintIndex, int curSelectcolorIndex)
        {
            List<List<uint>> lists = new List<List<uint>>();
            switch (tintIndex)
            {
                case ETintIndex.R:
                    if (curDyeParentLable == 1)
                    {
                        uint id_0 = curFashionClothes.cSVFashionClothesData.id * 10000 + Sys_Role.Instance.HeroId;
                        lists = CSVFashionColour.Instance.GetConfData(id_0).FashionColour1;
                    }
                    if (curDyeParentLable == 2) { lists = curFashionWeapon.cSVFashionWeaponData.WeaponColour; }
                    if (curDyeParentLable == 3) { lists = curFashionAccessory.cSVFashionAccessoryData.AccColour; }
                    break;
                case ETintIndex.G:
                    uint id_1 = curFashionClothes.cSVFashionClothesData.id * 10000 + Sys_Role.Instance.HeroId;
                    lists = CSVFashionColour.Instance.GetConfData(id_1).FashionColour2;
                    break;
                case ETintIndex.B:
                    uint id_2 = curFashionClothes.cSVFashionClothesData.id * 10000 + Sys_Role.Instance.HeroId;
                    lists = CSVFashionColour.Instance.GetConfData(id_2).FashionColour3;
                    break;
                case ETintIndex.A:
                    uint id_3 = curFashionClothes.cSVFashionClothesData.id * 10000 + Sys_Role.Instance.HeroId;
                    lists = CSVFashionColour.Instance.GetConfData(id_3).HairColour;
                    break;
                default:
                    break;
            }
            List<uint> colors = lists[curSelectcolorIndex];
            return new Color(colors[0] / 255f, colors[1] / 255f, colors[2] / 255f, 1);
        }

    }
}
