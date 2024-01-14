using System;
using System.Collections.Generic;
using Lib.AssetLoader;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Logic
{
    public class OneTouchCell : UIComponent
    {
        public CrossCell cell;

        public RectTransform rectTransform;
        public Image highlight;
        public Image icon;
        public Animator animator;

        public uint imageId = 0;

        public override void Reset()
        {
            imageId = 0;
        }

        protected override void Loaded()
        {
            animator = gameObject.GetComponent<Animator>();
            highlight = transform.Find("Selected").gameObject.GetComponent<Image>();
            icon = transform.Find("Icon").gameObject.GetComponent<Image>();
            rectTransform = transform as RectTransform;
        }

        public void Refresh(CrossCell cell)
        {
            this.cell = cell;
        }

        // 规则是：邻居间的ImageId不能一样，如果是一张图，则必然死循环
        public void Random(List<int> ls, in List<OneTouchCell> vds)
        {
            while (imageId == 0)
            {
                int index = UnityEngine.Random.Range(0, ls.Count);
                imageId = (uint)ls[index];

                // 九宫格大于邻居个数则执行随机，否则使用最简单的随机，因为可能死循环
                if (ls.Count > cell.neighbours.Count)
                {
                    foreach (var one in cell.neighbours)
                    {
                        if (imageId == vds[one].imageId)
                        {
                            // 如果和邻居有相同的图片id,则重新随机
                            // let imageId == 0, for let while can run continue
                            imageId = 0;
                            break;
                        }
                    }
                }
            }

            icon.gameObject.SetActive(false);
            if (imageId != 0)
            {
                icon.gameObject.SetActive(true);
                ImageHelper.SetIcon(icon, imageId);
            }
        }

        public void SetSelected(bool toSelected)
        {
            highlight.gameObject.SetActive(toSelected);
            if (toSelected)
            {
                animator.Play("UI_LittleGame_OneTouchDraw_Animator_View_bottom_Cell_Open", -1, 0);
            }
            else
            {
                animator.Play("UI_LittleGame_OneTouchDraw_Animator_View_bottom_Cell_Close", -1, 0);
            }
        }
        public bool RectangleContainsScreenPoint(Vector2 screenPosition, Camera camera)
        {
            return cell.use && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPosition, camera);
        }

        public override string ToString()
        {
            return cell.index.ToString() + " " + cell.use;
        }
    }

    public class UI_LittleGame_OneTouchDraw : UIBase, UI_LittleGame_OneTouchDraw_Layout.IListener
    {
        private UI_LittleGame_OneTouchDraw_Layout Layout = new UI_LittleGame_OneTouchDraw_Layout();

        public Camera uiCamera
        {
            get
            {
                return UIManager.mUICamera;
            }
        }

        private List<OneTouchCell> vds = new List<OneTouchCell>(36);

        private bool finalResult = false;
        private bool hasGotResult = false;
        private bool hasStart = false;
        private bool hasPressed = false;
        private bool needRandom = true;
        private bool hasCollected = false;

        private uint taskId;
        private uint id;
        private CSVLittleGame_OneTOuchDraw.Data csv;
        private bool hasLoaded = false;
        private Animator ameiAnimator;

        private AsyncOperationHandle<Texture> assetRequest;

        #region 纯数据
        //纯数据层面
        private int startIndex = 0;
        private CrossGrid grid = new CrossGrid(36, 6);
        private RandomStack<CrossCell> path = new RandomStack<CrossCell>();
        //路径的peek点
        private CrossCell peek
        {
            get
            {
                CrossCell cell;
                path.Peek(out cell, 0);
                return cell;
            }
        }

        private CrossCell lastPeek
        {
            get
            {
                CrossCell cell;
                path.Peek(out cell, 1);
                return cell;
            }
        }
        //是否完全遍历所有的cell
        private bool isTraverseAll
        {
            get { return path.Count > 1 && path.Count == grid.usingCount; }
        }
        //走过的路径中是否存在该点
        private bool Contains(CrossCell cell)
        {
            return path.Contains(cell);
        }
        //是否为Peek的邻接点
        private bool IsPeekNeighbour(CrossCell cell)
        {
            return peek.IsNeighbour(cell.index);
        }
        #endregion

        protected override void OnLoaded()
        {            
            Layout.Parse(gameObject);
            Layout.RegisterEvents(this);

            //手势控制
            Layout.eventTrigger.onDragStart += OnDragStart;
            Layout.eventTrigger.onDrag += OnDrag;
            Layout.eventTrigger.onDragEnd += OnDragEnd;

            Layout.buttonStart.enabled = true;
            TextHelper.SetText(Layout.buttonStartText, 1003013);

            TryCollectVds();
        }
        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, toRegister);
        }
        protected override void OnDestroy() {
            if (assetRequest.IsValid()) {
                AddressablesUtil.Release<Texture>(ref assetRequest, OnCompleted);
            }
        }
        private void TryCollectVds()
        {
            if (hasCollected) { return; }
            hasCollected = false;

            //子节点
            List<Transform> children = GetChildren(Layout.childNode);
            vds.Clear();
            for (int i = 0, count = children.Count; i < count; ++i)
            {
                OneTouchCell one = new OneTouchCell().Init(children[i]) as OneTouchCell;

                vds.Add(one);
            }
        }

        private bool fromOpen = true;
        protected override void OnOpen(object arg)
        {
            needRandom = true;
            Tuple<uint, object> tuple = arg as Tuple<uint, object>;
            finalResult = false;
            if (tuple != null)
            {
                taskId = tuple.Item1;
                id = Convert.ToUInt32(tuple.Item2);
            }

            hasGotResult = false;
            hasStart = false;
            hasPressed = false;
            finalResult = false;

            if (bLoaded)
            {
                Layout.buttonStart.enabled = true;
                TextHelper.SetText(Layout.buttonStartText, 1003013);
                ImageHelper.SetImageGray(Layout.buttonStartImage, false);
            }

            fromOpen = true;
        }
        protected override void OnHide()
        {
            fromOpen = false;
        }
        protected override void OnShow()
        {
            if (fromOpen)
            {
                csv = CSVLittleGame_OneTOuchDraw.Instance.GetConfData(id);
                if (csv != null)
                {
                    TextHelper.SetText(Layout.desc, csv.tips);
                    Layout.iconLoader.Set(csv.image_path);

                    Reset(id);
                    TryRandom();

                    if (!hasLoaded)
                    {
                        hasLoaded = true;
                        LoadHeadIconAssetAsyn(csv.imagepath);
                    }
                }
            }
            else
            {

            }
        }
        protected override void OnShowEnd()
        {
        }
        private void LoadHeadIconAssetAsyn(string path)
        {
            //AssetMananger.Instance.LoadAssetAsyn(path, ref requestRef, (request) =>
            //{
            //    GameObject go = request.Instantiate<GameObject>();
            //    if (null != go)
            //    {
            //        go.transform.SetParent(Layout.npcRoot);
            //        go.transform.localPosition = Vector3.zero;
            //        go.transform.localEulerAngles = Vector3.zero;
            //        go.transform.localScale = Vector3.one;
            //    }

            //    ameiAnimator = go.transform.Find("Amei").GetComponent<Animator>();
            //});
        }

        private void OnCompleted(AsyncOperationHandle<Texture> handle) {
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetTexture("_MainTex", handle.Result);
            materialPropertyBlock.SetColor("_Color", Color.white);
            Layout.lineRenderer.SetPropertyBlock(materialPropertyBlock);
            Layout.lineRenderer.sortingOrder = nSortingOrder + 3;
        }
        // 重置棋盘
        private void Reset(uint id)
        {
            ameiAnimator?.Play("UI_LittleGame_NpcRoot_Standby_Open", -1, 0);

            path.Clear();
            grid.SetAllUse(false);
            grid.SetUsingCount(csv.validId.Count);
            Layout.fx?.gameObject.SetActive(false);
            Layout.arrow.gameObject.SetActive(false);
            for (int i = 0, count = vds.Count; i < count; ++i)
            {
                vds[i].Refresh(grid[i]);
                vds[i].Hide();
            }

            if (csv != null)
            {
                for (int i = 0, count = grid.usingCount; i < count; ++i)
                {
                    int index = csv.validId[i];
                    grid[index].SetUse(true);
                    vds[index].Show();
                }
                for (int i = 0, count = grid.usingCount; i < count; ++i)
                {
                    int index = csv.validId[i];
                    vds[index].SetSelected(false);
                    grid[index].CalaulateNeighbours(grid, 36, 6);
                }

                ImageHelper.SetIcon(Layout.arrow, csv.ArrowId);
                
                AddressablesUtil.LoadAssetAsync<Texture>(ref assetRequest, csv.linePath, OnCompleted);

                startIndex = csv.startId;

                path.Push(grid[startIndex]);
                vds[startIndex].SetSelected(true);

                SetLinePath();

                if (Layout.fx != null)
                {
                    Layout.fx.position = vds[startIndex].transform.position;
                }
            }
        }
        private void TryRandom()
        {
            if (!needRandom) { return; }
            needRandom = true;

            // 随机生成图片
            if (csv != null)
            {
                for (int i = 0, count = grid.usingCount; i < count; ++i)
                {
                    int index = csv.validId[i];
                    vds[index].Random(csv.imageId, vds);
                }
            }
        }

        private EDir GetArrowDir(CrossCell from, CrossCell to)
        {
            return from.GetRelativeDir(grid, to);
        }
        private void SetLinePath()
        {
            int count = path.Count;
            Layout.lineRenderer.positionCount = count;
            for (int i = 0; i < count; ++i)
            {
                Layout.lineRenderer.SetPosition(i, vds[path[i].index].transform.position);
            }
        }
        private void SetArrow(OneTouchCell from, OneTouchCell to)
        {
            if (to != null)
            {
                if (to.cell.index == startIndex)
                {
                    Layout.arrow.gameObject.SetActive(false);
                }
                else
                {
                    EDir dir = GetArrowDir(from.cell, to.cell);
                    switch (dir)
                    {
                        case EDir.Up:
                            Layout.arrow.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                            break;
                        case EDir.Down:
                            Layout.arrow.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
                            break;
                        case EDir.Left:
                            Layout.arrow.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
                            break;
                        case EDir.Right:
                            Layout.arrow.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
                            break;
                    }
                    Layout.arrow.gameObject.SetActive(true);
                    Layout.arrow.transform.SetParent(to.transform, false);
                }
            }
        }

        #region 事件
        private List<Transform> GetChildren(Transform t)
        {
            List<Transform> children = new List<Transform>();
            int childCount = t.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                Transform child = t.GetChild(i);
                children.Add(child);
            }

            return children;
        }

        //正在press的cell
        private OneTouchCell touchingCell
        {
            get
            {
                foreach (var oneTouchCell in vds)
                {
                    bool contains = oneTouchCell.RectangleContainsScreenPoint(Input.mousePosition, uiCamera);
                    if (contains)
                    {
                        return oneTouchCell;
                    }
                }
                return null;
            }
        }

        private void OnDragStart(GameObject go)
        {
            if (!hasStart || hasGotResult) { return; }

            hasPressed = true;
            OneTouchCell currentPressCell = touchingCell;
            if (currentPressCell != null)
            {
                bool isPeekNeighbour = IsPeekNeighbour(currentPressCell.cell);
                if (currentPressCell.cell.index != peek.index && !isPeekNeighbour)
                {
                    //失败
                    OnFail();
                }
            }
        }
        private void OnDrag(GameObject go, Vector2 delta)
        {
            if (!hasStart || hasGotResult) { return; }

            OneTouchCell currentPressCell = touchingCell;
            if (currentPressCell != null)
            {
                bool isPeekNeighbour = IsPeekNeighbour(currentPressCell.cell);
                //DebugUtil.Log(ELogType.eLittleGame, "isPeekNeighbour: " + isPeekNeighbour + "  " + currentPressCell.cell.index);
                //peek的邻接点
                if (isPeekNeighbour)
                {
                    bool contains = Contains(currentPressCell.cell);
                    //path是否包含当前press死亡cell
                    if (!contains)
                    {
                        path.Push(currentPressCell.cell);
                        vds[currentPressCell.cell.index].SetSelected(true);

                        SetArrow(vds[lastPeek.index], currentPressCell);
                        SetLinePath();
                        //DebugUtil.Log(ELogType.eLittleGame, "isTraverseAll: " + isTraverseAll);
                        if (isTraverseAll)
                        {
                            //成功
                            OnSuccess();
                        }
                    }
                    else
                    {
                        if (lastPeek.index == touchingCell.cell.index)
                        {
                           // 回退
                           vds[peek.index].SetSelected(false);
                           path.Pop(out var top);

                           SetArrow(vds[top.index], currentPressCell);
                           SetLinePath();
                        }
                        else
                        {
                            //失败
                            OnFail();
                        }
                    }
                }
            }
        }
        private void OnDragEnd(GameObject go)
        {
            if (!hasStart || hasGotResult) { return; }

            if (isTraverseAll)
            {
                OnSuccess();
            }
            else
            {
                OnFail();
            }
        }

        //断线重连
        private void OnReconnectResult(bool result)
        {
            if (hasPressed && !hasGotResult)
            {
                if (isTraverseAll)
                {
                    OnSuccess();
                }
                else
                {
                    OnFail();
                }
            }
        }
        private void OnOverTime()
        {
            Layout.countDownTime.text = Sys_LittleGame.Instance.GetTimeFormat();

            //如果成功的话，会取消Timer
            //某种失败的情况下也会取消，另外一些失败的情况下，不取消Timer
            if (isTraverseAll)
            {
                OnSuccess();
            }
            else
            {
                OnFail();
            }
        }

        private void OnSuccess()
        {
            hasPressed = false;
            TextHelper.SetText(Layout.buttonStartText, 1003013);
            Layout.buttonStart.enabled = true;
            ImageHelper.SetImageGray(Layout.buttonStartImage, false);

            Sys_LittleGame.Instance.EndGame();
            hasGotResult = true;
            finalResult = true;
            UIManager.OpenUI(EUIID.UI_LittleGame_Result, true, new Tuple<bool, Action>(true, ()=>
            {
                OnReturn_ButtonClicked();
            }));

            ameiAnimator?.Play("UI_LittleGame_NpcRoot_Victory_Open", -1, 0);
            //取消Timer
            DebugUtil.Log(ELogType.eLittleGame, "成功");
        }

        private void OnFail()
        {
            hasPressed = false;
            TextHelper.SetText(Layout.buttonStartText, 1003013);
            Layout.buttonStart.enabled = true;
            ImageHelper.SetImageGray(Layout.buttonStartImage, false);

            Sys_LittleGame.Instance.EndGame();
            hasGotResult = true;
            finalResult = false;
            UIManager.OpenUI(EUIID.UI_LittleGame_Result, true, new Tuple<bool, Action>(false, null));

            ameiAnimator?.Play("UI_LittleGame_NpcRoot_Failure_Open", -1, 0);
            //取消Timer
            DebugUtil.Log(ELogType.eLittleGame, "失败");
        }

        public void OnReturn_ButtonClicked()
        {
            if (taskId != 0 && id != 0 && finalResult)
            {
                Sys_Task.Instance.ReqStepGoalFinishEx(taskId);
            }

            Sys_LittleGame.Instance.EndGame();
            UIManager.CloseUI(EUIID.UI_LittleGame_OneTouchDraw);
            UIManager.CloseUI(EUIID.UI_CountDown);
        }
        public void OnTips_ButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_LittleGame_Tips, true, csv.gameDescribe);
        }

        public void OnStart_ButtonClicked()
        {
            hasStart = false;

            Reset(id);
            UIManager.OpenUI(EUIID.UI_CountDown, true, new Tuple<float, Vector3, Action>(3f, Layout.positionIndexer.position, () =>
            {
                hasGotResult = false;
                hasStart = true;
                Layout.countDownTime.color = Color.white;
                TextHelper.SetText(Layout.buttonStartText, 1003012);
                Layout.buttonStart.enabled = false;
                ImageHelper.SetImageGray(Layout.buttonStartImage, true);
                Layout.fx.gameObject.SetActive(true);
                Sys_LittleGame.Instance.StartGame(csv.time, (remainTime) =>
                {
                    Layout.countDownTime.text = Sys_LittleGame.Instance.GetTimeFormat();
                    if (remainTime <= 10)
                    {
                        Layout.countDownTime.color = Color.red;
                    }
                }, OnOverTime);
            }));
        }
        #endregion
    }
}


