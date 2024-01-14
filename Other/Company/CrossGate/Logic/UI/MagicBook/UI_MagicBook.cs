using DG.Tweening;
using Framework;
using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{
    public class MagicbookEvt
    {
        public EMagicBookViewType type;
        public uint subChapterId;
    }
        

    public class UI_MagicBook_Layout
    {
        public Transform transform;
        public Button closeBtn;
        public Transform tabListGo;
   
        public CP_Toggle magicBookToggle;
        public CP_Toggle teachingToggle;

        public GameObject magicBookRedGo;
        public GameObject teachingRedGo;
        public GameObject magicBookViewGo;
        public GameObject teachingViewGo;
        public GameObject strategyrViewGo;
        public GameObject magicBookRedPoint;
        public GameObject teachRedPoint;

        public void Init(Transform transform)
        {
            this.transform = transform;
            closeBtn = transform.Find("Animator/View_Title/Image_Bottom/Btn_Close").GetComponent<Button>();
            tabListGo = transform.Find("Animator/Scroll_View");
            magicBookRedGo = transform.Find("Animator/Menu/Scroll View/Viewport/Content/Toggle/Image_Dot").gameObject;
            teachingRedGo = transform.Find("Animator/Menu/Scroll View/Viewport/Content/Toggle (1)/Image_Dot").gameObject;
            magicBookViewGo = transform.Find("Animator/View_Right").gameObject;
            teachingViewGo = transform.Find("Animator/View_Teaching").gameObject;
            strategyrViewGo = transform.Find("Animator/View_Strategy").gameObject;
            magicBookRedPoint = transform.Find("Animator/Menu/Scroll View/Viewport/Content/Toggle/Image_Dot").gameObject;
            teachRedPoint = transform.Find("Animator/Menu/Scroll View/Viewport/Content/Toggle (1)/Image_Dot").gameObject;

            magicBookToggle = transform.Find("Animator/Menu/Scroll View/Viewport/Content/Toggle").GetComponent<CP_Toggle>();
            teachingToggle = transform.Find("Animator/Menu/Scroll View/Viewport/Content/Toggle (1)").GetComponent<CP_Toggle>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnCloseBtnClicked);
            magicBookToggle.onValueChanged.AddListener(listener.OnMagicBookValueChange);
            teachingToggle.onValueChanged.AddListener(listener.OnTeachingValueChange);
        }

        public interface IListener
        {
            void OnCloseBtnClicked();
            void OnMagicBookValueChange(bool isOn);
            void OnTeachingValueChange(bool isOn);
        }
    }

    public class UI_MagicChapterSub
    {
        private Transform transform;
        private CP_Toggle toggle;
        private Text textShow;
        private Text textLight;
        private GameObject redGo;
        private GameObject LockGo;
        private Action<uint> action;
        private CSVChapterInfo.Data cSVGenusData;
        public void Init(Transform _transform)
        {
            transform = _transform;
            toggle = transform.gameObject.GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (null != cSVGenusData)
                {
                    OnToggleClick(isOn, cSVGenusData.id);
                }
            });

            textShow = transform.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
            textLight = transform.Find("Image_Menu_Light/Text_Menu_Light").GetComponent<Text>();

            redGo = transform.Find("Image").gameObject;
            LockGo = transform.Find("Image_Lock").gameObject;
        }

        private void OnToggleClick(bool _select, uint _index)
        {
            if (_select)
            {
                StringBuilder btnEventStr = StringBuilderPool.GetTemporary();
                if (Sys_Role.Instance.Role.Level < cSVGenusData.ChapterOpenlv)
                {
                    btnEventStr.Append("Lock_MagicBookChapterToggle_Id:");
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10761, cSVGenusData.ChapterOpenlv.ToString()));
                }
                else
                {
                    btnEventStr.Append("Unlock_MagicBookChapterToggle_Id:");
                    action?.Invoke(_index);
                }
                btnEventStr.Append(_index.ToString());
                UIManager.HitButton(EUIID.UI_MagicBook, StringBuilderPool.ReleaseTemporaryAndToString(btnEventStr));
            }
        }

        public void AddListener(Action<uint> _action)
        {
            action = _action;
        }

        public void OnSelect(bool _select)
        {
            toggle.SetSelected(_select, false);
        }

        public void SetLockAndRedPointState()
        {
            bool isUnlock = Sys_Role.Instance.Role.Level >= cSVGenusData.ChapterOpenlv;
            bool isShowRed =false;
            if (cSVGenusData.category == 1)
            {
                isShowRed = Sys_MagicBook.Instance.CheckChapterHasReward(cSVGenusData.id);
            }
            else if(cSVGenusData.category == 2|| cSVGenusData.category == 3)
            {
                if (cSVGenusData.id == 207)
                {
                    isShowRed = false;
                }
                else
                {
                    isShowRed = Sys_MagicBook.Instance.CheckTeachChapterHasUnFinish(cSVGenusData.id);
                }
            }
            redGo.gameObject.SetActive(null != cSVGenusData && isShowRed && isUnlock);
            toggle.SetToggleIsNotChange(isUnlock);
            LockGo.SetActive(!isUnlock);
        }

        public void ToggleOff()
        {
            toggle.SetSelected(false, false);
        }

        public void SetInfo(uint _index)
        {
            cSVGenusData = CSVChapterInfo.Instance.GetConfData(_index);
            RefreshData();
        }

        public void RefreshData()
        {
            if (null != cSVGenusData)
            {
                SetLockAndRedPointState();
                TextHelper.SetText(textShow, cSVGenusData.ChapterNum);
                TextHelper.SetText(textLight, cSVGenusData.ChapterNum);
            }
        }
    }

    public class UI_MagicChapterList
    {
        private Transform transform;
        private InfinityGridLayoutGroup gridGroup;
        private Dictionary<GameObject, UI_MagicChapterSub> dicCells = new Dictionary<GameObject, UI_MagicChapterSub>();
        private List<UI_MagicChapterSub> scrpCtrlList = new List<UI_MagicChapterSub>();
        private int visualGridCount;

        private IListener listener;
        private uint curSelectIndex;
        private List<uint> listIds = new List<uint>();

        public void Init(Transform _transform)
        {
            transform = _transform;
            gridGroup = transform.Find("TabList").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 9;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform tran = gridGroup.transform.GetChild(i);

                UI_MagicChapterSub cell = new UI_MagicChapterSub();
                cell.Init(tran);
                cell.AddListener(OnSelectIndex);
                dicCells.Add(tran.gameObject, cell);
                scrpCtrlList.Add(cell);
            }
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dicCells.ContainsKey(trans.gameObject))
            {
                UI_MagicChapterSub cell = dicCells[trans.gameObject];
                cell.SetInfo(listIds[index]);
                cell.OnSelect(listIds[index] == curSelectIndex);
            }
        }

        private void OnSelectIndex(uint index)
        {
            curSelectIndex = index;
            listener?.OnSelectListIndex(index);
        }

        public void Hide()
        {
            for (int i = 0; i < scrpCtrlList.Count; i++)
            {
                scrpCtrlList[i].ToggleOff();
            }
        }

        public void RefreshCeil()
        {
            for (int i = 0; i < scrpCtrlList.Count; i++)
            {
                scrpCtrlList[i].RefreshData();
            }
        }

        public void Show(uint cur, EMagicBookViewType type, List<uint> infoList,bool isToggle)
        {
            listIds.Clear();
            for (int i = 0; i < infoList.Count; ++i)
            {
                CSVChapterInfo.Instance.TryGetValue(infoList[i], out CSVChapterInfo.Data cSVChapterInfoData);
                if (cSVChapterInfoData != null && cSVChapterInfoData.category == (uint)type)
                {
                    listIds.Add(infoList[i]);
                }
            }
            if (isToggle)
            {
                if (listIds.Count > 0)
                {
                    curSelectIndex = listIds[0];
                }
                else
                {
                    curSelectIndex = cur;
                }
            }
            else
            {
                if (curSelectIndex == 0&& listIds.Count > 0)
                {
                    curSelectIndex = listIds[0];
                }
                else
                {
                    curSelectIndex = cur;
                }
            }
            visualGridCount = listIds.Count;
            gridGroup.SetAmount(visualGridCount);
            OnSelectIndex(curSelectIndex);
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnSelectListIndex(uint _typeId);
        }
    }

    public class UI_MagicBookSysCeil : IDisposable
    {
        public CSVChapterFunctionList.Data sysData;
        private Action<uint> action;
        public Transform transform;
        private Button showBtn;
        private Button functionBtn;
        private Image sysIconImage;
        private Text sysNameText;
        private Slider sysSlider;
        private Text sysProgressText;
        private GameObject redGo;
        private Text openTipsText;
        private RectTransform arrow;
        public void Init(Transform transform)
        {
            this.transform = transform;
            showBtn = transform.Find("Image_bg").GetComponent<Button>();
            showBtn.onClick.AddListener(ShowBtnClicked);
            functionBtn = transform.Find("Btn_Look").GetComponent<Button>();
            functionBtn.onClick.AddListener(DetailBtnClick);
            sysIconImage = transform.Find("Image/Image_Icon").GetComponent<Image>();
            sysNameText = transform.Find("Text_Name").GetComponent<Text>();
            sysSlider = transform.Find("Slider/Slider_Exp").GetComponent<Slider>();
            sysProgressText = transform.Find("Slider/Text_Percent").GetComponent<Text>();
            arrow = transform.Find("Image_Arrow").GetComponent<RectTransform>();
            openTipsText = transform.Find("Object_Empty/Text").GetComponent<Text>();
            redGo = transform.Find("Image_Dot").gameObject;
        }

        public void SetData(CSVChapterFunctionList.Data data, Action<uint> action)
        {
            sysData = data;
            this.action = action;
            RefressData();
            SetArrowState(false);
        }

        public void RemoveEvent()
        {
            if(null != showBtn)
            {
                showBtn.onClick.RemoveListener(ShowBtnClicked);
            }

            if(null != functionBtn)
            {
                functionBtn.onClick.RemoveListener(DetailBtnClick);
            }
        }

        private void DetailBtnClick()
        {
            if (null != sysData)
            {
                UIManager.OpenUI(EUIID.UI_MagicBook_Detail, false, sysData.id);
                UIManager.HitButton(EUIID.UI_MagicBook, "OpenMagicBookDetail:" + sysData.id.ToString());
            }
        }

        private void ShowBtnClicked()
        {
            if (null != sysData && null != action)
            {
                if (Sys_MagicBook.Instance.IsSysUnlcok(sysData))
                {
                    action.Invoke(sysData.id);
                }
                else
                {
                    UIManager.HitButton(EUIID.UI_MagicBook, "Unlock_MagicBookSystem_Id:" + sysData.id.ToString());
                }
            }
        }

        public void RefressData()
        {
            if (null != sysData)
            {
                TextHelper.SetText(sysNameText, sysData.SystemLanguage);
                ImageHelper.SetIcon(sysIconImage, sysData.SysIcon);
                bool unlock = Sys_MagicBook.Instance.IsSysUnlcok(sysData);
                sysSlider.transform.parent.gameObject.SetActive(unlock);
                openTipsText.transform.parent.gameObject.SetActive(!unlock);
                functionBtn.enabled = unlock;
                ButtonHelper.Enable(functionBtn, unlock);
                if (unlock)
                {
                    sysSlider.maxValue = Sys_MagicBook.Instance.GetTaskSubBySysId(sysData.id).Count;
                    ChapterSys sever_chapterSys = Sys_MagicBook.Instance.GetSeverChapterSysByChapterId(sysData.SystemType);
                    float value = 0;
                    if (null != sever_chapterSys)
                    {
                        value = sever_chapterSys.Process;
                    }
                    sysSlider.value = value;
                    TextHelper.SetText(sysProgressText, string.Format("{0}/{1}", sysSlider.value.ToString(), sysSlider.maxValue.ToString()));
                }
                else
                {
                    TextHelper.SetText(openTipsText, Sys_MagicBook.Instance.GetUnlockText(sysData));
                }

                redGo.SetActive(unlock && Sys_MagicBook.Instance.CheckSysReward(sysData.id));
            }
        }


        public void SetArrowState(bool isExpand)
        {
            float rotateZ = isExpand ? -90f : 0f;
            arrow.localRotation = Quaternion.Euler(0f, 0f, rotateZ);
        }

        public void Dispose()
        {

        }
    }

    public class UI_MagicBookSysTaskCeil : IDisposable
    {
        public CSVChapterSysTask.Data sysTaskData;
        public Transform transform;
        private Button goBtn;
        private Button acceptBtn;
        private Button againBtn;
        private GameObject finishGo;
        private Text sysTaskNameText;
        private Text sysTaskDesText;
        private Transform view;
        public void Init(Transform transform)
        {
            this.transform = transform;
            goBtn = transform.Find("Btn_Go").GetComponent<Button>();
            sysTaskNameText = transform.Find("Text_Title").GetComponent<Text>();
            sysTaskDesText = transform.Find("Text_Title/Text_Des").GetComponent<Text>();
            acceptBtn = transform.Find("Btn_Accept").GetComponent<Button>();
            againBtn = transform.Find("Object_Finish/Btn_Again").GetComponent<Button>();
            finishGo = transform.Find("Object_Finish/Image_Finish").gameObject;
            view = transform.Find("Scroll_View/Viewport");
            acceptBtn?.onClick.AddListener(AcceptBtnClick); // 领取
            againBtn?.onClick.AddListener(AgainClicked); // 再次体验
            goBtn?.onClick.AddListener(GoBtnClicked);
            ButtonHelper.AddButtonCtrl(acceptBtn);
            ButtonHelper.AddButtonCtrl(againBtn);
            ButtonHelper.AddButtonCtrl(goBtn);
        }

        public void SetData(CSVChapterSysTask.Data data)
        {
            sysTaskData = data;
            RefressData();
        }

        public void RemoveEvent()
        {
            if (null != acceptBtn)
            {
                acceptBtn.onClick.RemoveListener(AcceptBtnClick);
            }

            if (null != againBtn)
            {
                againBtn.onClick.RemoveListener(GoBtnClicked);
            }

            if (null != goBtn)
            {
                goBtn.onClick.RemoveListener(GoBtnClicked);
            }
        }

        private void AcceptBtnClick()
        {
            if (null != sysTaskData)
            {
                Sys_MagicBook.Instance.MagicDictGetSysTaskAwardReq(sysTaskData.id);
                UIManager.HitButton(EUIID.UI_MagicBook, "Accept:" + sysTaskData.id.ToString());
            }
        }

        private void AgainClicked()
        {
            if (null != sysTaskData)
            {
                Sys_MagicBook.Instance.MagicBookTaskGo(sysTaskData);
                UIManager.HitButton(EUIID.UI_MagicBook, "Again:" + sysTaskData.id.ToString());
            }
        }

        private void GoBtnClicked()
        {
            if(null != sysTaskData)
            {
                Sys_MagicBook.Instance.MagicBookTaskGo(sysTaskData);
                UIManager.HitButton(EUIID.UI_MagicBook, "Goto:" + sysTaskData.id.ToString());
            }            
        }

        public void RefressData()
        {
            if (null != sysTaskData)
            {
                TextHelper.SetText(sysTaskNameText, sysTaskData.TaskName);
                TextHelper.SetText(sysTaskDesText, sysTaskData.TaskNameLag);
                List<ItemIdCount> temp = CSVDrop.Instance.GetDropItem(sysTaskData.DropWatchId);
                int count = temp.Count;
                FrameworkTool.DestroyChildren(view.gameObject);
                for (int i = 0; i < count; i++)
                {
                    PropItem propItem = PropIconLoader.GetAsset(new PropIconLoader.ShowItemData(temp[i].id, temp[i].count,
                                      true, false, false, false, false, _bShowCount: true), view, EUIID.UI_MagicBook);
                    propItem.transform.localScale = new Vector3(0.9f, 0.9f, 1);
                }

                TaskRefreshState();
            }
        }

        private void TaskRefreshState()
        {
            EMaigcBookTaskState taskState = Sys_MagicBook.Instance.GetTaskStateByTaskId(sysTaskData);
            acceptBtn.gameObject.SetActive(taskState == EMaigcBookTaskState.UnFinished);
            goBtn.gameObject.SetActive(taskState == EMaigcBookTaskState.Hand);
            againBtn.gameObject.SetActive(sysTaskData.TryAgain == 1 && taskState == EMaigcBookTaskState.Finished);
            againBtn.transform.parent.gameObject.SetActive(taskState == EMaigcBookTaskState.Finished);
            finishGo.SetActive(sysTaskData.TryAgain == 0 && taskState == EMaigcBookTaskState.Finished);
        }

        public void Dispose()
        {
        }
    }

    public class UI_MagicBook_RightView
    {
        private Transform transform;
        private Text chapterTitleText;
        private Text chapterNameText;
        private Slider chapterSlider;
        private Text chapterProgressText;
        private GameObject redGo;
        private Button rewardBtn;

        private VerticalLayoutGroup layouGroup;
        private ScrollRect chapterSubSysScroll;
        private Transform chapterSubParent;
        private RectTransform viewport;
        private Transform subSmallGo;
        private RectTransform bigGoRect;
        private CSVChapterInfo.Data chapterInfoData;
        private List<UI_MagicBookSysCeil> sysCeilList = new List<UI_MagicBookSysCeil>();

        private List<UI_MagicBookSysTaskCeil> sysCeilTaskList = new List<UI_MagicBookSysTaskCeil>();
        private uint currentChapterId;
        private uint currentSysId;
        private bool canReward;
        public void Init(Transform _transform)
        {
            transform = _transform;
            chapterTitleText = transform.Find("Progress/Image_Name/Text").GetComponent<Text>();
            chapterNameText = transform.Find("Progress/Text_Des").GetComponent<Text>();
            chapterSlider = transform.Find("Progress/Slider_Exp").GetComponent<Slider>();
            chapterSlider.minValue = 0;
            chapterProgressText = transform.Find("Progress/Text_Percent").GetComponent<Text>();

            redGo = transform.Find("Award/RawImage").gameObject;
            rewardBtn = transform.Find("Award/Image_Icon").GetComponent<Button>();
            rewardBtn.onClick.AddListener(RewardBtnClicked);
            ButtonCtrl ctrl = rewardBtn.gameObject.AddComponent<ButtonCtrl>();
            ctrl.button = rewardBtn;
            chapterSubSysScroll = transform.Find("ListScroll").GetComponent<ScrollRect>();
            chapterSubParent = transform.Find("ListScroll/Viewport/Item");
            layouGroup = chapterSubParent.GetComponent<VerticalLayoutGroup>();
            bigGoRect = transform.Find("ListScroll/Viewport/Item/List_Big").GetComponent<RectTransform>();
            viewport = transform.Find("ListScroll/Viewport").GetComponent<RectTransform>();
            subSmallGo = transform.Find("ListScroll/Viewport/SmallItem");
        }

        public void ShowCeilClicked(uint sysId)
        {
            StringBuilder btnEventStr = StringBuilderPool.GetTemporary();
            if (currentSysId != sysId)
            {
                btnEventStr.Append("ExPand_MagicBookSystem_Id:");
                currentSysId = sysId;
                GerSysTaskCeil(sysId);
            }
            else
            {
                btnEventStr.Append("Collapses_MagicBookSystem_Id:");
                currentSysId = 0;
                subSmallGo.SetParent(viewport);
                RecycleTaskCeil();
                for (int i = 0; i < sysCeilList.Count; i++)
                {
                    sysCeilList[i].SetArrowState(false);
                }
                subSmallGo.gameObject.SetActive(false);
                FrameworkTool.ForceRebuildLayout(viewport.gameObject);
            }
            btnEventStr.Append(sysId.ToString());
            UIManager.HitButton(EUIID.UI_MagicBook, StringBuilderPool.ReleaseTemporaryAndToString(btnEventStr));

        }

        private void RecycleTaskCeil()
        {
            for (int i = 0; i < sysCeilTaskList.Count; i++)
            {
                UI_MagicBookSysTaskCeil uI_MagicBookSysTaskCeil = sysCeilTaskList[i];
                uI_MagicBookSysTaskCeil.RemoveEvent();
                PoolManager.Recycle(uI_MagicBookSysTaskCeil);
            }
            sysCeilTaskList.Clear();
        }

        public void RefreshChapterTop()
        {
            if (chapterInfoData != null)
            {
                if (chapterInfoData.ChapterDescribe != 0)
                {
                    TextHelper.SetText(chapterTitleText, chapterInfoData.ChapterDescribe);
                }
                TextHelper.SetText(chapterNameText, chapterInfoData.ChapterNum);
                chapterSlider.maxValue = chapterInfoData.ChapterTaskNum + 0.0f;
                Chapter server_Chapter = Sys_MagicBook.Instance.GetSeverChapterByChapterId(currentChapterId);

                float value = 0;
                if (null != server_Chapter)
                {
                    value = server_Chapter.Process;
                }
                chapterSlider.value = value;
                canReward = Sys_MagicBook.Instance.CheckChapterReward(chapterInfoData.id);
                redGo.SetActive(canReward);
                TextHelper.SetText(chapterProgressText, string.Format("{0}/{1}", value.ToString(), chapterInfoData.ChapterTaskNum.ToString()));
            }
        }

        private void RewardBtnClicked()
        {
            StringBuilder btnEventStr = StringBuilderPool.GetTemporary();
            if (!canReward)
            {
                btnEventStr.Append("Lock_MagicBookChapterReward:");
                UIManager.OpenUI(EUIID.UI_MagicBook_Tips, false, currentChapterId);
            }
            else
            {
                btnEventStr.Append("Unlock_MagicBookChapterReward:");
                Sys_MagicBook.Instance.MagicDictGetChapterAwardReq(currentChapterId);
            }
            btnEventStr.Append(currentChapterId.ToString());
            UIManager.HitButton(EUIID.UI_MagicBook, StringBuilderPool.ReleaseTemporaryAndToString(btnEventStr));
        }

        public void RefreshChapter(uint id)
        {
            currentChapterId = id;
            currentSysId = 0;
            chapterInfoData = CSVChapterInfo.Instance.GetConfData(id);
            RefreshChapterTop();
            ReviewChapterSys();
        }

        private void ReviewChapterSys()
        {
            subSmallGo.SetParent(viewport);
            subSmallGo.gameObject.SetActive(false);

            for (int i = 0; i < sysCeilList.Count; i++)
            {
                UI_MagicBookSysCeil uI_MagicBookSysCeil = sysCeilList[i];
                uI_MagicBookSysCeil.RemoveEvent();
                PoolManager.Recycle(uI_MagicBookSysCeil);
            }
            sysCeilList.Clear();
            bool hasReward = Sys_MagicBook.Instance.CheckChapterHasReward(currentChapterId);
            List<CSVChapterFunctionList.Data>  sysList = new List<CSVChapterFunctionList.Data>();
            if (hasReward)
            {
                var rewardList = Sys_MagicBook.Instance.GetCanRewardSysListByChapterId(currentChapterId);
                if (rewardList.Count > 1)
                {
                    rewardList.Sort(CompFun);
                }
                var unRewardList = Sys_MagicBook.Instance.GetUnRewardSysListByChapterId(currentChapterId);
                if (unRewardList.Count > 1)
                {
                    unRewardList.Sort(CompFun);
                }
                sysList.AddRange(rewardList);
                sysList.AddRange(unRewardList);
            }
            else
            {
                sysList.AddRange(Sys_MagicBook.Instance.GetSysListByChapterId(currentChapterId));
                if (sysList.Count > 1)
                {
                    sysList.Sort(CompFun);
                }
            }
            FrameworkTool.CreateChildList(chapterSubParent, sysList.Count);
            int count = sysList.Count;
            for (int i = 0; i < count; i++)
            {
                Transform tran = chapterSubParent.GetChild(i);
                UI_MagicBookSysCeil ceil = PoolManager.Fetch<UI_MagicBookSysCeil>();
                ceil.Init(tran);
                ceil.SetData(sysList[i], ShowCeilClicked);
                sysCeilList.Add(ceil);
            }
            FrameworkTool.ForceRebuildLayout(viewport.gameObject);
        }

        private int CompFun(CSVChapterFunctionList.Data a, CSVChapterFunctionList.Data b)
        {
            return ((int)a.AccOrder - (int)b.AccOrder);
        }

        private void GerSysTaskCeil(uint sysId)
        {
            subSmallGo.SetParent(viewport);
            RecycleTaskCeil();
            List<CSVChapterSysTask.Data>  sysTaskList = Sys_MagicBook.Instance.GetTaskSubBySysId(sysId);            
            FrameworkTool.CreateChildList(subSmallGo, sysTaskList.Count);
            int count = sysTaskList.Count;
            for (int i = 0; i < count; i++)
            {
                Transform tran = subSmallGo.GetChild(i);

                UI_MagicBookSysTaskCeil ceil = PoolManager.Fetch<UI_MagicBookSysTaskCeil>();                
                ceil.Init(tran);
                ceil.SetData(sysTaskList[i]);
                sysCeilTaskList.Add(ceil);
            }

            int sub_Index = 0;
            for (int i = 0; i < sysCeilList.Count; i++)
            {
                if (sysCeilList[i].sysData.id == sysId)
                {
                    sysCeilList[i].SetArrowState(true);
                    sub_Index = sysCeilList[i].transform.GetSiblingIndex();
                }
                else
                {
                    sysCeilList[i].SetArrowState(false);
                }
            }
            subSmallGo.gameObject.SetActive(true);
            subSmallGo.SetParent(chapterSubParent);
            subSmallGo.SetSiblingIndex(sub_Index + 1);
            FrameworkTool.ForceRebuildLayout(viewport.gameObject);
            SetPosView(sub_Index);
        }

        public void SetPosView(int select)
        {
            float spaY = layouGroup.spacing;
            float celly = bigGoRect.sizeDelta.y;
            float p_y = select * celly + select * spaY;
            chapterSubSysScroll.StopMovement();
            Vector3 pos = chapterSubSysScroll.content.localPosition;
            chapterSubSysScroll.content.DOLocalMoveY(Mathf.Min(viewport.sizeDelta.y < 0 ? 0 : viewport.sizeDelta.y, p_y), 0.3f);// new Vector2(0, , 0);
        }

        public void RefreshAllCeil()
        {
            for (int i = 0; i < sysCeilList.Count; i++)
            {
                sysCeilList[i].RefressData();
            }

            for (int i = 0; i < sysCeilTaskList.Count; i++)
            {
                sysCeilTaskList[i].RefressData();
            }            
        }

        public void PoolRec()
        {
            for (int i = 0; i < sysCeilList.Count; i++)
            {
                PoolManager.Recycle(sysCeilList[i]);
            }

            for (int i = 0; i < sysCeilTaskList.Count; i++)
            {
                PoolManager.Recycle(sysCeilTaskList[i]);
            }
        }
    }

    public class UI_MagicBook : UIBase, UI_MagicBook_Layout.IListener, UI_MagicChapterList.IListener
    {
        private UI_MagicBook_Layout layout = new UI_MagicBook_Layout(); 
        private UI_MagicBook_RightView rightView;
        private UI_Teaching teachingView;
        //private UI_Strategy strategyView;
        private UI_MagicChapterList chapterList;
        private uint currentChapterId;
        private MagicbookEvt evt;
        private EMagicBookViewType curViewType = EMagicBookViewType.MagicBook;
        private List<uint> listIds = new List<uint>();
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            chapterList = new UI_MagicChapterList();
            chapterList.Init(layout.tabListGo);
            chapterList.RegisterListener(this);
            rightView = new UI_MagicBook_RightView();
            rightView.Init(layout.magicBookViewGo.transform);
            teachingView = new UI_Teaching();
            teachingView.Init(layout.teachingViewGo);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_MagicBook.Instance.eventEmitter.Handle(Sys_MagicBook.EEvents.TaskReward, UpdateCeilUI, toRegister);
            Sys_MagicBook.Instance.eventEmitter.Handle(Sys_MagicBook.EEvents.ChapterReward, UpdateTopUI, toRegister);
            Sys_MagicBook.Instance.eventEmitter.Handle(Sys_MagicBook.EEvents.OnNeedCheckItemRed, UpdateCeilUI, toRegister);
            Sys_MagicBook.Instance.eventEmitter.Handle(Sys_MagicBook.EEvents.TeachingProcessUpdate, TeachingProcessUpdate, toRegister);

        }

        protected override void OnOpen(object arg)
        {
            if (arg != null )
            {
                evt = arg as MagicbookEvt;
                curViewType = evt.type;
                currentChapterId = evt.subChapterId;
            }
        }

        protected override void OnShow()
        {
            if (curViewType == EMagicBookViewType.Teaching && Sys_MagicBook.Instance.CheckTeachShowRedPoint())
            {
                layout.teachingToggle.SetSelected(true, true);
            }
            else
            {
                layout.magicBookToggle.SetSelected(true, true);
            }

            //curViewType = EMagicBookViewType.MagicBook;
            InitChapterInfo();
            chapterList.Show(currentChapterId, curViewType, listIds, false);
            rightView.RefreshChapter(currentChapterId);
            layout.magicBookRedPoint.SetActive(Sys_MagicBook.Instance.CheckMagicReward());
            layout.teachingRedGo.SetActive(Sys_MagicBook.Instance.CheckTeachShowRedPoint());
        }

        private void InitChapterInfo()
        {
            if (listIds.Count == 0)
            {
                listIds.AddRange(CSVChapterInfo.Instance.GetKeys());
                listIds.Sort();
            }
            if (currentChapterId == 0 && listIds.Count > 0)
            {
                if (curViewType == EMagicBookViewType.Teaching && Sys_MagicBook.Instance.CheckTeachShowRedPoint())
                {
                    currentChapterId = listIds[1];
                }
                else
                {
                    currentChapterId = listIds[0];
                }
            }
        }

        private void UpdateCeilUI()
        {
            rightView.RefreshChapterTop();
            rightView.RefreshAllCeil();
            chapterList.RefreshCeil();
            teachingView.RefreshChapterTop();
            layout.magicBookRedPoint.SetActive(Sys_MagicBook.Instance.CheckMagicReward());
            layout.teachingRedGo.SetActive(Sys_MagicBook.Instance.CheckTeachShowRedPoint());
        }

        private void UpdateTopUI()
        {
            rightView.RefreshChapterTop();
            teachingView.RefreshChapterTop();
            chapterList.RefreshCeil();
            layout.magicBookRedPoint.SetActive(Sys_MagicBook.Instance.CheckMagicReward());
            layout.teachingRedGo.SetActive(Sys_MagicBook.Instance.CheckTeachShowRedPoint());
        }

        private void TeachingProcessUpdate()
        {
            teachingView.RefreshCeilProcess();
        }

        protected override void OnHide()
        {
            chapterList.Hide();
        }

        public void OnCloseBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_MagicBook, "Close");
            CloseSelf();
        }

        public void OnSelectListIndex(uint _typeId)
        {
            currentChapterId = _typeId;
            if (curViewType == EMagicBookViewType.MagicBook)
            {
                rightView.RefreshChapter(currentChapterId);
            }
            else if (curViewType == EMagicBookViewType.Teaching)
            {
                teachingView.RefreshChapter(currentChapterId);
            }
        }

        public void OnMagicBookValueChange(bool isOn)
        {
            if (isOn)
            {
                layout.magicBookViewGo.SetActive(true);
                layout.teachingViewGo.SetActive(false);
                layout.strategyrViewGo.SetActive(false);
                curViewType = EMagicBookViewType.MagicBook;
                chapterList.Show(currentChapterId, curViewType, listIds, true);
            }
        }

        public void OnTeachingValueChange(bool isOn)
        {
            if (isOn)
            {
                layout.magicBookViewGo.SetActive(false);
                layout.teachingViewGo.SetActive(true);
                layout.strategyrViewGo.SetActive(false);
                curViewType = EMagicBookViewType.Teaching;
                chapterList.Show(currentChapterId, curViewType, listIds, true);
            }
        }
    }
}
