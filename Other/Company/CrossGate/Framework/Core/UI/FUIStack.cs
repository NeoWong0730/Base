using Lib.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core.UI
{
    public class FUIStack
    {
        public enum EUIStackEvent : int
        {
            HasEvent = 0,

            BeginEnter = 1,
            EndEnter = 2,
            BeginExit = 4,
            EndExit = 8,

            ReduceFrameRateChange = 16,
            ReduceMainCameraQualityChange = 32,
            ReadyHideMainCameraChange = 64,
            RealHideMainCameraChange = 128,

            OnSortOrderChanged = 256,
        }

        public uint stackID { get; private set; } = 0;

        public EventEmitter<EUIStackEvent> eventEmitter = new EventEmitter<EUIStackEvent>();
        public int nEventFlage = 0;

        //因为有UI开启动画, 所以数据上需要影藏相机和实际上影藏相机的时机有时间差
        /// <summary>
        /// 数据上有正在打开的需要影藏相机的UI的个数
        /// </summary>
        public int nReadyHideMainCameraRef { get; private set; }
        /// <summary>
        /// 实际上已经打开的需要影藏相机的UI的个数
        /// </summary>
        public int nRealHideMainCameraRef { get; private set; }
        public int nReduceFrameRateRef { get; private set; }
        public int nReduceMainCameraQualityRef { get; private set; }

        internal void ReduceFrameRate()
        {
            ++nReduceFrameRateRef;
            if (nReduceFrameRateRef == 1)
            {
                eventEmitter.Trigger<uint, bool>(EUIStackEvent.ReduceFrameRateChange, stackID, true);
            }
        }

        internal void CancelReduceFrameRate()
        {
            if (nReduceFrameRateRef <= 0)
            {
                DebugUtil.LogErrorFormat("CancelReduceFrameRate 引用次数已经为 {0}, 检测调用逻辑是否错误", nReduceFrameRateRef);
                nReduceFrameRateRef = 0;
            }
            else
            {
                --nReduceFrameRateRef;
            }

            if (nReduceFrameRateRef == 0)
            {
                eventEmitter.Trigger<uint, bool>(EUIStackEvent.ReduceFrameRateChange, stackID, false);
            }
        }

        internal void ReduceMainCameraQuality()
        {
            ++nReduceMainCameraQualityRef;

            if (nReduceMainCameraQualityRef == 1)
            {
                eventEmitter.Trigger<uint, bool>(EUIStackEvent.ReduceMainCameraQualityChange, stackID, true);
            }
        }

        internal void CancelReduceMainCameraQuality()
        {
            if (nReduceMainCameraQualityRef <= 0)
            {
                DebugUtil.LogErrorFormat("nReduceMainCameraQualityRef 引用次数已经为 {0}, 检测调用逻辑是否错误", nReduceMainCameraQualityRef);
                nReduceMainCameraQualityRef = 0;
            }
            else
            {
                --nReduceMainCameraQualityRef;
            }

            if (nReduceMainCameraQualityRef == 0)
            {
                eventEmitter.Trigger<uint, bool>(EUIStackEvent.ReduceMainCameraQualityChange, stackID, false);
            }
        }

        internal void ReadyHideMainCamera()
        {
            ++nReadyHideMainCameraRef;

            if (nReadyHideMainCameraRef == 1)
            {
                eventEmitter.Trigger<uint, bool>(EUIStackEvent.ReadyHideMainCameraChange, stackID, true);
            }
        }

        internal void CancelReadyHideMainCamera()
        {
            if (nReadyHideMainCameraRef <= 0)
            {
                DebugUtil.LogErrorFormat("nReadyHideMainCameraRef 引用次数已经为 {0}, 检测调用逻辑是否错误", nReadyHideMainCameraRef);
                nReadyHideMainCameraRef = 0;
            }
            else
            {
                --nReadyHideMainCameraRef;
            }

            if (nReadyHideMainCameraRef == 0)
            {
                eventEmitter.Trigger<uint, bool>(EUIStackEvent.ReadyHideMainCameraChange, stackID, false);
            }
        }

        internal void RealHideMainCamera()
        {
            ++nRealHideMainCameraRef;

            if (nRealHideMainCameraRef == 1)
            {
                eventEmitter.Trigger<uint, bool>(EUIStackEvent.RealHideMainCameraChange, stackID, true);
            }
        }

        internal void CancelRealHideMainCamera()
        {
            if (nRealHideMainCameraRef <= 0)
            {
                DebugUtil.LogErrorFormat("nRealHideMainCameraRef 引用次数已经为 {0}, 检测调用逻辑是否错误", nRealHideMainCameraRef);
                nRealHideMainCameraRef = 0;
            }
            else
            {
                --nRealHideMainCameraRef;
            }

            if (nRealHideMainCameraRef == 0)
            {
                eventEmitter.Trigger<uint, bool>(EUIStackEvent.RealHideMainCameraChange, stackID, false);
            }
        }
        public FUIStack()
        {
            mUIDic = new Dictionary<int, FUIBase>();
            mUIList = new List<FUIBase>();
            mStack = new List<FUIBase>();
        }

        public FUIStack(int capacity)
        {
            mUIDic = new Dictionary<int, FUIBase>(capacity);
            mUIList = new List<FUIBase>(capacity);
            mStack = new List<FUIBase>(capacity);
        }
        internal Transform mRoot;
        internal Camera mUICamera;

        protected readonly Dictionary<int, FUIBase> mUIDic = null;
        protected readonly List<FUIBase> mUIList = null;
        protected readonly List<FUIBase> mStack = null;
        protected Stack<int> subUIStackTemp = new Stack<int>(4);
        protected List<FUIBase> needMoveUITemp = new List<FUIBase>(4);

        public int nMinOrder = 500;
        public int nMaxOrder = 10000;

        protected bool bStackDirty = false;
        protected int bHideStack = 0;        

        public void Init(Transform root, Camera camera)
        {
            mRoot = root;
            mUICamera = camera;
        }
        public virtual void PreloadUI(int id, UIConfigData configData)
        {
            GetOrCreate(id, configData, true, out FUIBase ui);
        }

        public void SendMsg(int id, object arg = null)
        {
            if (TryGetUI((int)id, out FUIBase ui)/* && ui.bLoaded*/)
            {
                ui.OnSetData(arg);
            }
        }

        protected virtual FUIBase CreateInstance(UIConfigData configData)
        {
            return System.Activator.CreateInstance(configData.script) as FUIBase;
        }

        protected virtual bool GetOrCreate(int id, UIConfigData configData, bool isPreload, out FUIBase ui)
        {
            if (!TryGetUI((int)id, out ui))
            {
                if (configData == null)
                {
                    return false;
                }

                ui = CreateInstance(configData);
                if (ui == null)
                {
                    return false;
                }
                mUIDic.Add(id, ui);
                mUIList.Add(ui);
                ui.Init(id, configData, this, isPreload);
            }

            return true;
        }

        public void OpenUI(int id, UIConfigData configData, bool immediate = false, object arg = null, int parentID = 0)
        {
            if (!GetOrCreate(id, configData, false, out FUIBase ui))
            {
                return;
            }

            //每次打开的时候置空父UI的ID
            ui.nParentID = 0;

            bool bIgnoreStack = ui.ContainsOptions(EUIOption.eIgnoreStack);
            bool hasDontHideBeforeIfHasFlag = ui.ContainsOptions(EUIOption.eDontHideBeforeIfHasFlag);
            bool bHideBeforeUI = ui.ContainsOptions(EUIOption.eHideBeforeUI);

            bool bSubUINeedHide = false;

            //不需要加入UI栈自动管理
            if (bIgnoreStack)
            {
                ui.SetSortingOrder(configData.order >= 0 ? configData.order + nMaxOrder : configData.order + nMinOrder);
            }
            else
            {
                bool isSubUI = false;
                if (!bHideBeforeUI && parentID > 0)
                {
                    if (TryGetUI(parentID, out FUIBase parent) && parent.bInStack && parent.isOpen)
                    {
                        isSubUI = true;
                        bSubUINeedHide = !parent.isVisibleAndOpen;
                    }
                    else
                    {
                        DebugUtil.LogWarningFormat("OpenUI id = {0} : parentID = {1} 不在堆栈中或没开", id.ToString(), parentID.ToString());
                    }
                }

                //isSubUI
                if (isSubUI)
                {
                    needMoveUITemp.Clear();

                    if (ui.bInStack && GetSubUIRange(ui, out int start, out int end))
                    {
                        for (int i = start; i < end; ++i)
                        {
                            needMoveUITemp.Add(mStack[i]);
                        }
                        mStack.RemoveRange(start, end - start);
                    }
                    else
                    {
                        needMoveUITemp.Add(ui);
                    }

                    FUIBase currentUI = null;
                    bool isVaild = false;
                    int index = 0;

                    subUIStackTemp.Clear();

                    for (; index < mStack.Count; ++index)
                    {
                        currentUI = mStack[index];

                        if (isVaild)
                        {
                            //如果当前的父UI 是栈顶的UI 则继续压栈
                            while (subUIStackTemp.Count > 0 && currentUI.nParentID != subUIStackTemp.Peek())
                            {
                                subUIStackTemp.Pop();
                            }

                            if (subUIStackTemp.Count > 0)
                            {
                                subUIStackTemp.Push(currentUI.nID);
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            //从第一个检测到的父UI开始为有效的Index
                            if (currentUI.nID == parentID)
                            {
                                isVaild = true;
                                subUIStackTemp.Push(currentUI.nID);
                            }
                        }
                    }
                    
                    mStack.InsertRange(index, needMoveUITemp);
                    ui.bInStack = true; //设置自身在Stack里面
                    //其他的本身就在Stack里面所以不再设置

                    mStack[index].nParentID = parentID;

                    bStackDirty = true;
                }
                else
                {
                    //将最后打开的UI放置到栈顶
                    if (TopUIID() != id)
                    {
                        int sort = mStack.Count > 0 ? mStack[mStack.Count - 1].nSortingOrder : nMinOrder;
                        sort += 10;

                        if (ui.bInStack)
                        {
                            mStack.Remove(ui);
                        }

                        mStack.Add(ui);
                        ui.bInStack = true;

                        if (sort >= nMaxOrder)
                        {
                            //标记栈需要刷新层级
                            bStackDirty = true;
                        }
                        else
                        {
                            ui.SetSortingOrder(sort);
                        }
                    }
                }
            }

            //计算前面UI关闭的时间
            float maxExitTime = 0f;
            if (bHideBeforeUI)
            {
                //并且不是立即打开
                //并且尚未处于显示状态
                //bool needFade = !immediate && !ui.isVisible;

                FUIBase otherUI = null;
                float exitTime = 0;
                int startHide = mStack.Count - 1;

                //如果是标记了需要关闭前面UI,并且忽略堆栈的UI
                if (bIgnoreStack)
                {
                    if (!ui.bRefHideStack)
                    {
                        ui.bRefHideStack = true;
                        ++bHideStack;
                    }
                }
                else
                {
                    --startHide;
                }

                for (int i = startHide; i >= 0; --i)
                {
                    otherUI = mStack[i];

                    //如果标记了当前UI不关闭前置UI 并且前置UI也标记了不被关闭
                    if (hasDontHideBeforeIfHasFlag)
                    {
                        //如果遇到的UI标记为eDontHideFlag
                        if (otherUI.ContainsOptions(EUIOption.eDontHideFlag))
                        {
                            //如果遇到的是主UI 则设置为遇到的主UI的标记
                            if (otherUI.ContainsOptions(EUIOption.eHideBeforeUI))
                            {
                                hasDontHideBeforeIfHasFlag = otherUI.ContainsOptions(EUIOption.eDontHideBeforeIfHasFlag);
                            }
                            continue;
                        }//如果遇到的是主UI 并且该UI没有标记为eDontHideFlag
                        else if (otherUI.ContainsOptions(EUIOption.eHideBeforeUI))
                        {
                            //结束这个标记
                            hasDontHideBeforeIfHasFlag = false;
                        }
                    }

                    exitTime = otherUI.Hide(EUIState.Hide, true);

                    //exitTime = otherUI.Hide(EUIState.Hide, immediate);
                    //if (needFade)
                    //{
                    //    maxExitTime = Mathf.Max(exitTime, maxExitTime);
                    //}
                }
            }

            bool hide = false;
            if (!bIgnoreStack)
            {
                hide = bHideStack > 0;
            }

            ui.Open(maxExitTime, hide, arg);

            if (bSubUINeedHide)
            {
                ui.Hide(EUIState.Hide, true);
            }
        }

        public void ClearStackUI(bool destroy)
        {
            mStack.Clear();
            FUIBase ui;
            for (int i = mUIList.Count - 1; i >= 0; --i)
            {
                ui = mUIList[i];
                if (!ui.ContainsOptions(EUIOption.eIgnoreStack))
                {
                    ui.Hide(destroy ? EUIState.Destroy : EUIState.Close, true);
                    ui.OnForeQuit();
                }
            }
        }

        public void HideUI(int id, EUIState state, bool immediate)
        {
            if (!TryGetUI((int)id, out FUIBase ui))
                return;

            bool showStack = false;
            if (ui.ContainsOptions(EUIOption.eIgnoreStack | EUIOption.eHideBeforeUI))
            {
                if (ui.bRefHideStack)
                {
                    ui.bRefHideStack = false;
                    --bHideStack;
                }
            }
            showStack = bHideStack <= 0;

            //
            if (ui.ContainsOptions(EUIOption.eHideBeforeUI))
            {
                FUIBase otherUI = null;
                //检测是否关闭了最上层的UI
                bool isTop = false;
                //关闭区间内的所有UI
                float maxExitTime = 0f;

                if (ui.ContainsOptions(EUIOption.eIgnoreStack))
                {
                    isTop = true;
                }
                else
                {
                    int top = mStack.Count;
                    int bottom = 0;

                    //获得关闭UI 以及 附加UI范围
                    for (int i = mStack.Count - 1; i >= 0; --i)
                    {
                        bottom = i;
                        otherUI = mStack[i];

                        if (otherUI == ui)
                        {
                            break;
                        }
                        else if (otherUI.ContainsOptions(EUIOption.eHideBeforeUI))
                        {
                            top = i;
                        }
                    }

                    //检测是否关闭了最上层的UI
                    isTop = top == mStack.Count;

                    //关闭区间内的所有UI
                    for (int i = top - 1; i >= bottom; --i)
                    {
                        otherUI = mStack[i];
                        otherUI.bInStack = false;

                        float exitTime = otherUI.Hide(i == bottom ? state : EUIState.Close, immediate);
                        if (isTop)
                        {
                            maxExitTime = Mathf.Max(maxExitTime, exitTime);
                        }
                    }

                    mStack.RemoveRange(bottom, top - bottom);
                }

                //如果是关闭了最上层的UI
                //则 自动开启栈中的UI直到主UI
                if (showStack && isTop)
                {
                    bool hasDontHideBeforeIfHasFlag = false;
                    bool hasMainUI = false;

                    for (int i = mStack.Count - 1; i >= 0; --i)
                    {
                        if (hasMainUI)
                        {
                            otherUI = mStack[i];
                            if (otherUI.ContainsOptions(EUIOption.eDontHideFlag))
                            {
                                otherUI.Show(maxExitTime, false);
                                if (otherUI.ContainsOptions(EUIOption.eHideBeforeUI))
                                {
                                    hasDontHideBeforeIfHasFlag = otherUI.ContainsOptions(EUIOption.eDontHideBeforeIfHasFlag);
                                }
                            }
                            else if (otherUI.ContainsOptions(EUIOption.eHideBeforeUI))
                            {
                                break;
                            }
                        }
                        else
                        {
                            otherUI = mStack[i];
                            otherUI.Show(maxExitTime, false);
                            if (otherUI.ContainsOptions(EUIOption.eHideBeforeUI))
                            {
                                hasMainUI = true;
                                hasDontHideBeforeIfHasFlag = otherUI.ContainsOptions(EUIOption.eDontHideBeforeIfHasFlag);
                                if (hasDontHideBeforeIfHasFlag == false)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (GetSubUIRange(ui, out int start, out int end))
                {
                    //关闭区间内的所有UI
                    //不包括本身
                    for (int i = start + 1; i < end; ++i)
                    {
                        FUIBase otherUI = mStack[i];
                        otherUI.bInStack = false;
                        float exitTime = otherUI.Hide(state, immediate);
                    }

                    mStack.RemoveRange(start + 1, end - start - 1);
                }
            }
            ui.Hide(state, immediate);

            //如果是考虑堆栈的
            //并且是主UI
            //则自动计算需要关闭的附加UI
            //以及 自动开启离站顶最近的一组UI
            if (!ui.ContainsOptions(EUIOption.eIgnoreStack))
            {
                ui.bInStack = false;
                mStack.Remove(ui);
            }
        }

        private bool GetSubUIRange(FUIBase ui, out int start, out int end)
        {
            FUIBase currentUI = null;
            bool isVaild = false;
            subUIStackTemp.Clear();
            start = 0;
            end = mStack.Count;

            for (int index = 0; index < mStack.Count; ++index)
            {
                currentUI = mStack[index];

                if (isVaild)
                {
                    //如果当前的父UI 是栈顶的UI 则继续压栈
                    while (subUIStackTemp.Count > 0 && currentUI.nParentID != subUIStackTemp.Peek())
                    {
                        subUIStackTemp.Pop();
                    }

                    if (subUIStackTemp.Count > 0)
                    {
                        subUIStackTemp.Push(currentUI.nID);
                    }
                    else
                    {
                        end = index;
                        break;
                    }
                }
                else
                {
                    //从自己开始为有效的Index
                    if (currentUI == ui)
                    {
                        isVaild = true;
                        start = index;
                        subUIStackTemp.Push(currentUI.nID);
                    }
                }
            }

            return isVaild;
        }

        public FUIBase GetUI(int id)
        {
            FUIBase ui = null;
            mUIDic.TryGetValue(id, out ui);
            return ui;
        }

        public bool TryGetUI(int id, out FUIBase ui)
        {
            return mUIDic.TryGetValue(id, out ui);
        }

        public FUIBase TopUI()
        {
            if (mStack.Count > 0)
            {
                FUIBase ui = mStack[mStack.Count - 1];
                return ui;
            }
            return null;
        }

        public int TopUIID()
        {
            if (mStack.Count > 0)
            {
                FUIBase ui = mStack[mStack.Count - 1];
                return ui.nID;
            }
            return -1;
        }

        public int LastUIID()
        {
            if (mStack.Count > 0)
            {
                int index = mStack.Count - 2;
                if (index >= 0  && index < mStack.Count)
                {
                    FUIBase ui = mStack[index];
                    return ui.nID;
                }
            }
            return -1;
        }

        public void UpdateState()
        {
            FUIBase ui = null;
            for (int i = mUIList.Count - 1; i >= 0; --i)
            {
                ui = mUIList[i];
                ui.UpdateState(Time.deltaTime);
                if (ui.eState == EUIState.Destroy)
                {
                    mUIList.RemoveAt(i);
                    mUIDic.Remove(ui.nID);
                }
            }

            if (bStackDirty)
            {
                bStackDirty = false;
                for (int i = mStack.Count - 1; i >= 0; --i)
                {
                    mStack[i].SetSortingOrder(i * 10 + 10 + nMinOrder);
                }
            }

            if (nEventFlage != 0)
            {
                eventEmitter.Trigger<uint, int>(FUIStack.EUIStackEvent.HasEvent, stackID, nEventFlage);
                nEventFlage = 0;
            }
        }

        public void Update()
        {            
            for (int i = mUIList.Count - 1; i >= 0; --i)
            {
                mUIList[i].Update(Time.time, Time.unscaledTime);
            }
        }

        public void LateUpdate(float dt, float usdt)
        {            
            for (int i = mUIList.Count - 1; i >= 0; --i)
            {                
                mUIList[i].LateUpdate(dt, usdt);
            }
        }
    }
}