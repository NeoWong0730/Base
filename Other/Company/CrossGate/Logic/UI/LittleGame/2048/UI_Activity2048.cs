using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Logic.Core;
using System;
using Table;
using UnityEngine.EventSystems;
using Lib.Core;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic
{

    public class UI_Activity2048 : UIBase
    {
        private GameObject numPrefab;
        private GameObject root;
        private GameObject eventBG;
        private GameObject countdown;
        private GameObject temp;
        private GameObject tempparent;
        private Transform parent;
        private Button btnClose;
        private Button btnStart;
        private Button btnAgain;
        private Button btnRule;
        private Text desc;
        private Text target;
        private Text targetscore;
        private GameState state = GameState.None;
        private CSVSynthesis.Data csvActivityData;
        private int actualCount;
        private int targetNum;
        private int targetNumCount;

        private Vector3 mouseDownPosition;

        public Num[][] numComponentArray = new Num[4][];

        private Timer timer;
        private Text txtTime;
        private float activityCD;
        private float useTime;

        private InfinityGrid infinity;

        #region 系统函数
        protected override void OnOpen(object arg)
        {

        }
        protected override void OnLoaded()
        {
            Sys_Activity_2048.Instance.ReqActivity2048Data();
            Parse();
        }
        protected override void OnShow()
        {
            txtTime.text = "00:00";
            UpdateView();
        }
        protected override void OnClose()
        {
            timer?.Cancel();
            ClearMatrix();
            state = GameState.None;
            //txtTime.color = Color.white;
        }

        protected override void ProcessEvents(bool toRegister)
        {
            //Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, toRegister);
            Sys_Activity_2048.Instance.eventEmitter.Handle(Sys_Activity_2048.EEvents.OnActivity2048DataUpdate, OnActivity2048DataUpdate, toRegister);
        }
        #endregion
        #region func
        private void Parse()
        {
            numPrefab = transform.Find("Animator/View_bottom/numPrefab").gameObject;
            countdown = transform.Find("Animator/GameObject").gameObject;
            root = transform.Find("Animator/View_bottom/root").gameObject;
            parent = transform.Find("Animator/View_bottom/parent");
            tempparent = transform.Find("Animator/View_left/Scroll_View/List").gameObject;
            temp = tempparent.transform.Find("Item").gameObject;
            btnClose = transform.Find("Animator/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            btnStart = transform.Find("Animator/View_left/Btn_01").GetComponent<Button>();
            btnStart.onClick.AddListener(OnBtnStartClick);
            btnAgain = transform.Find("Animator/Button_Reset").GetComponent<Button>();
            btnAgain.onClick.AddListener(OnBtnAgainClick);
            btnRule = transform.Find("Animator/View_left/Button_Tips").GetComponent<Button>();
            btnRule.onClick.AddListener(OnBtnRuleClick);
            txtTime = transform.Find("Animator/View_Clock/Text_Time").GetComponent<Text>();
            desc = transform.Find("Animator/View_left/Text_Target").GetComponent<Text>();
            target = transform.Find("Animator/View_left/Viewport/Item/Target_Dark/Text").GetComponent<Text>();
            targetscore = transform.Find("Animator/View_left/Viewport/Item/Target_Dark/Text/Text_Number").GetComponent<Text>();
            eventBG = transform.Find("Animator/eventBG").gameObject;
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventBG);
            eventListener.AddEventListener(EventTriggerType.PointerDown, OnPointClick);
            eventListener.AddEventListener(EventTriggerType.PointerUp, OnPointUp);

            infinity = transform.Find("Animator/View_Award/ScrollView").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;

            numComponentArray[0] = new Num[] { null, null, null, null };
            numComponentArray[1] = new Num[] { null, null, null, null };
            numComponentArray[2] = new Num[] { null, null, null, null };
            numComponentArray[3] = new Num[] { null, null, null, null };
        }
        private void UpdateView()
        {
            csvActivityData = CSVSynthesis.Instance.GetConfData(Sys_Activity_2048.Instance.curActivityId);
            if (csvActivityData != null)
            {
                targetNumCount = (int)csvActivityData.Goal[0];
                targetNum = (int)csvActivityData.Goal[1];
                UpdateRewardView();

                TextHelper.SetText(desc, csvActivityData.tips);
                TextHelper.SetText(target, csvActivityData.Goaltips);
                GameEmpty();
                ShowList();
                SetScore();
            }
        }
        private void UpdateRewardView()
        {
            infinity.CellCount = csvActivityData.Reward_Id.Count;
            infinity.ForceRefreshActiveCell();
        }
        private void StartTime()
        {
            timer?.Cancel();
            var activityData = CSVSynthesis.Instance.GetConfData(Sys_Activity_2048.Instance.curActivityId);
            if (activityData != null)
            {
                activityCD = activityData.time;
                timer = Timer.Register(activityCD, OnActivityTimerComplete, OnActivityTimerUpdate, false, false);
            }

        }
        private void GameEmpty()
        {
            if (state == GameState.None || state == GameState.End)
            {
                btnAgain.gameObject.SetActive(false);
                ImageHelper.SetImageGray(btnStart.GetComponent<Image>(), false);
                btnStart.interactable = true;
                TextHelper.SetText(btnStart.GetComponentInChildren<Text>(), 1003013);
            }
        }

        private void Gameing()
        {
            if (state == GameState.Playing)
            {
                btnAgain.gameObject.SetActive(true);
                ImageHelper.SetImageGray(btnStart.GetComponent<Image>(), true);
                btnStart.interactable = false;
                TextHelper.SetText(btnStart.GetComponentInChildren<Text>(), 1003012);
            }
        }
        private void ShowList()
        {
            if (tempparent.transform.childCount == targetNum - 1)
                return;
            for (int i = 0; i < targetNum - 2; i++)
            {
                FrameworkTool.CreateGameObject(temp, tempparent);
            }
            for (int i = 0; i < tempparent.transform.childCount; i++)
            {
                Transform t = tempparent.transform.GetChild(i);
                ImageHelper.SetIcon(t.Find("Image_Icon1").GetComponent<Image>(), csvActivityData.imageId[i]);
                ImageHelper.SetIcon(t.Find("Image_Icon2").GetComponent<Image>(), csvActivityData.imageId[i]);
                ImageHelper.SetIcon(t.Find("Image_Icon3").GetComponent<Image>(), csvActivityData.imageId[i + 1]);
            }
        }
        private void StartGame()
        {
            if (state == GameState.Playing)
                return;
            actualCount = 0;
            timer?.Cancel();
            SetScore();
            txtTime.color = Color.white;
            UIManager.OpenUI(EUIID.UI_CountDown, false, new Tuple<float, Vector3, Action>(3, countdown.transform.position, OnStartGame));
        }
        private void PlayAgain()
        {
            if (state == GameState.Playing)
            {
                state = GameState.End;
                StartGame();
            }
        }
        private void GameOver()
        {
            var prama = new Activity2048ResultPrama()
            {
                IsWin = false
            };
            UIManager.OpenUI(EUIID.UI_Activity2048Result, false, prama);
            state = GameState.End;
            timer?.Cancel();
            GameEmpty();
        }

        private void GameWin()
        {
            timer?.Cancel();
            Sys_Activity_2048.Instance.ReqActivity2048ReportTime((uint)useTime);
            var prama = new Activity2048ResultPrama()
            {
                IsWin = true,
                Time = (uint)useTime
            };
            UIManager.OpenUI(EUIID.UI_Activity2048Result, false, prama);
            state = GameState.End;
            GameEmpty();
        }
        private GameObject InstantiateGameObject()
        {
            GameObject @object = GameObject.Instantiate<GameObject>(numPrefab);
            @object.SetActive(true);
            return @object;
        }
        private void SetScore()
        {
            targetscore.text = actualCount.ToString() + "/" + targetNumCount.ToString();
        }
        private bool CheckWin()
        {
            int count = 0;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (numComponentArray[x][y] != null)
                    {
                        if (numComponentArray[x][y].num == targetNum)
                        {
                            count++;
                        }
                    }
                }
            }
            if (count != actualCount)
            {
                actualCount = count;
                SetScore();
            }

            if (count >= targetNumCount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckGameOver()
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (numComponentArray[x][y] == null)
                    {
                        return false;
                    }
                }
            }

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (numComponentArray[x][y].num == numComponentArray[x][y + 1].num)
                    {
                        return false;
                    }
                }
            }

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (numComponentArray[x][y].num == numComponentArray[x + 1][y].num)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public void GenerateNumber(int posX = -1, int posY = -1, int number = 1, float tweenInDelayTime = 0)
        {
            int X = -1; int Y = -1;
            if (posX == -1 || posY == -1)
            {
                int countOfEmptyNum = 0;
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        if (numComponentArray[x][y] == null)
                            countOfEmptyNum++;
                    }
                }
                if (countOfEmptyNum == 0)
                {
                    return;
                }
                int randomNum = UnityEngine.Random.Range(1, countOfEmptyNum + 1);
                int index = 0;
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        if (numComponentArray[x][y] == null)
                        {
                            index++;
                            if (index == randomNum)
                            {
                                X = x; Y = y;
                                goto flag;
                            }
                        }
                    }
                }
            }
            else
            {
                X = posX;
                Y = posY;
            }
        flag:
            Num num = new Num();
            num.BindGameObject(InstantiateGameObject(), root, parent, csvActivityData.imageId);
            num.SetData(X, Y, number);
            num.InitShow();
            num.InitPos();
            num.setDelayTime(tweenInDelayTime);
            numComponentArray[X][Y] = num;
        }


        private void MoveNum()
        {
            bool isAnyNumMove = false;
            int countCombine = 0;
            TouchDir dir = GetTouchDir();
            switch (dir)
            {
                case TouchDir.None:
                    return;
                case TouchDir.Right:
                    for (int y = 0; y < 4; y++)
                    {
                        Num preNum = null;
                        int index = 4;
                        for (int x = 3; x >= 0; x--)
                        {
                            bool isNeedUpdateComponentArray = true;
                            if (numComponentArray[x][y] == null) continue;
                            if (preNum == null)
                            {
                                preNum = numComponentArray[x][y];
                                index--;
                            }
                            else
                            {
                                if (preNum.num == numComponentArray[x][y].num)
                                {
                                    // 合并这两个都移动到目标位置 然后消失 然后创建出来合并后的数字
                                    countCombine++;
                                    GenerateNumber(index, y, preNum.num + 1, 0.3f);
                                    preNum.Disappear();
                                    numComponentArray[x][y].Disappear();
                                    preNum = null;
                                    isNeedUpdateComponentArray = false;
                                }
                                else
                                {
                                    preNum = numComponentArray[x][y];
                                    index--;
                                }
                            }
                            // move to (index,y)
                            if (numComponentArray[x][y].MoveToPosition(index, y, numComponentArray, isNeedUpdateComponentArray))
                            {
                                isAnyNumMove = true;
                            }
                        }
                    }
                    break;
                case TouchDir.Left:
                    for (int y = 0; y < 4; y++)
                    {
                        Num preNum = null;
                        int index = -1;
                        for (int x = 0; x < 4; x++)
                        {
                            bool isNeedUpdateComponentArray = true;
                            if (numComponentArray[x][y] == null) continue;
                            if (preNum == null)
                            {
                                preNum = numComponentArray[x][y];
                                index++;
                            }
                            else
                            {
                                if (preNum.num == numComponentArray[x][y].num)
                                {
                                    // 合并这两个都移动到目标位置 然后消失 然后创建出来合并后的数字
                                    countCombine++;
                                    GenerateNumber(index, y, preNum.num + 1, 0.3f);
                                    preNum.Disappear();
                                    numComponentArray[x][y].Disappear();
                                    preNum = null;
                                    isNeedUpdateComponentArray = false;
                                }
                                else
                                {
                                    preNum = numComponentArray[x][y];
                                    index++;
                                }
                            }
                            // move to (index,y)
                            if (numComponentArray[x][y].MoveToPosition(index, y, numComponentArray, isNeedUpdateComponentArray))
                            {
                                isAnyNumMove = true;
                            }
                        }
                    }
                    break;
                case TouchDir.Top:
                    for (int x = 0; x < 4; x++)
                    {
                        Num preNum = null;
                        int index = -1;
                        for (int y = 0; y < 4; y++)
                        {
                            bool isNeedUpdateComponentArray = true;
                            if (numComponentArray[x][y] == null) continue;
                            if (preNum == null)
                            {
                                preNum = numComponentArray[x][y];
                                index++;
                            }
                            else
                            {
                                if (preNum.num == numComponentArray[x][y].num)
                                {
                                    // 合并这两个都移动到目标位置 然后消失 然后创建出来合并后的数字
                                    countCombine++;
                                    GenerateNumber(x, index, preNum.num + 1, 0.3f);
                                    preNum.Disappear();
                                    numComponentArray[x][y].Disappear();
                                    preNum = null;
                                    isNeedUpdateComponentArray = false;
                                }
                                else
                                {
                                    preNum = numComponentArray[x][y];
                                    index++;
                                }
                            }
                            // move to (index,y)
                            if (numComponentArray[x][y].MoveToPosition(x, index, numComponentArray, isNeedUpdateComponentArray))
                            {
                                isAnyNumMove = true;
                            }
                        }
                    }
                    break;
                case TouchDir.Bottom:
                    for (int x = 0; x < 4; x++)
                    {
                        Num preNum = null;
                        int index = 4;
                        for (int y = 3; y >= 0; y--)
                        {
                            bool isNeedUpdateComponentArray = true;
                            if (numComponentArray[x][y] == null) continue;
                            if (preNum == null)
                            {
                                preNum = numComponentArray[x][y];
                                index--;
                            }
                            else
                            {
                                if (preNum.num == numComponentArray[x][y].num)
                                {
                                    // 合并这两个都移动到目标位置 然后消失 然后创建出来合并后的数字
                                    countCombine++;
                                    GenerateNumber(x, index, preNum.num + 1, 0.3f);
                                    preNum.Disappear();
                                    numComponentArray[x][y].Disappear();
                                    preNum = null;
                                    isNeedUpdateComponentArray = false;
                                }
                                else
                                {
                                    preNum = numComponentArray[x][y];
                                    index--;
                                }
                            }
                            // move to (index,y)
                            if (numComponentArray[x][y].MoveToPosition(x, index, numComponentArray, isNeedUpdateComponentArray))
                            {
                                isAnyNumMove = true;
                            }
                        }
                    }
                    break;
            }

            if (countCombine > 0)
            {

            }

            if (isAnyNumMove)
                GenerateNumber();

            if (CheckWin())
            {
                GameWin();
            }
            if (CheckGameOver())
            {
                GameOver();
            }
        }
        private TouchDir GetTouchDir()
        {
            Vector3 touchOffset = Input.mousePosition - mouseDownPosition;
            if (Mathf.Abs(touchOffset.x) > Mathf.Abs(touchOffset.y) && Mathf.Abs(touchOffset.x) > 20)
            {
                if (touchOffset.x > 0)
                {
                    return TouchDir.Right;
                }
            }
            if (Mathf.Abs(touchOffset.x) > Mathf.Abs(touchOffset.y) && Mathf.Abs(touchOffset.x) > 20)
            {
                if (touchOffset.x < 0)
                {
                    return TouchDir.Left;
                }
            }
            if (Mathf.Abs(touchOffset.x) < Mathf.Abs(touchOffset.y) && Mathf.Abs(touchOffset.y) > 20)
            {
                if (touchOffset.y < 0)
                {
                    return TouchDir.Bottom;
                }
            }
            if (Mathf.Abs(touchOffset.x) < Mathf.Abs(touchOffset.y) && Mathf.Abs(touchOffset.y) > 20)
            {
                if (touchOffset.y > 0)
                {
                    return TouchDir.Top;
                }
            }
            return TouchDir.None;
        }
        public void ClearMatrix()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Num num = numComponentArray[i][j];
                    if (num != null)
                    {
                        num.Dispose();
                        numComponentArray[i][j] = null;
                    }
                }
            }
        }
        #endregion
        #region event
        private void OnBtnCloseClick()
        {
            UIManager.CloseUI(EUIID.UI_CountDown);
            this.CloseSelf();
        }
        private void OnActivity2048DataUpdate()
        {
            UpdateRewardView();
        }
        private void OnStartGame()
        {
            state = GameState.Playing;
            Gameing();
            ClearMatrix();
            StartTime();
            GenerateNumber();
        }
        private void OnReconnectResult(bool result)
        {
            if (state == GameState.Playing)
            {
                Gameing();
            }
        }
        public void OnPointClick(BaseEventData baseEventData)
        {
            mouseDownPosition = Input.mousePosition;
        }
        public void OnPointUp(BaseEventData baseEventData)
        {
            if (state == GameState.Playing)
                MoveNum();
        }
        private void OnActivityTimerComplete()
        {
            GameOver();
        }
        private void OnActivityTimerUpdate(float time)
        {
            if (activityCD >= time && txtTime != null)
            {
                uint mTime = (uint)(activityCD - time);
                uint minute = mTime / 60;
                uint second = mTime % 60;
                txtTime.text = string.Format("{0}:{1}", minute.ToString("D2"), second.ToString("D2"));
                if(mTime < 4)
                {
                    txtTime.color = Color.red;
                }
                useTime = time;
            }
        }
        private void OnBtnStartClick()
        {
            StartGame();
        }
        private void OnBtnAgainClick()
        {
            PlayAgain();
        }
        private void OnBtnRuleClick()
        {
            UIManager.OpenUI(EUIID.UI_LittleGame_Tips, false, csvActivityData.gameDescribe);
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_Activity2048Reward mCell = new UI_Activity2048Reward();
            mCell.Init(cell.mRootTransform.transform);
            cell.BindUserData(mCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_Activity2048Reward mCell = cell.mUserData as UI_Activity2048Reward;
            mCell.UpdateCellView(index);
        }
        #endregion

        #region Class
        public class UI_Activity2048Reward
        {
            private Transform transform;
            private Text txtFinishTime;
            private PropItem propItem;

            private ItemIdCount itemIdCountData;
            private int rewardIndex;
            /// <summary>
            /// 最大时间 小于等于这个时间视为完成
            /// </summary>
            private uint maxTime;

            public void Init(Transform _trans)
            {
                transform = _trans;
                txtFinishTime = transform.Find("Text_Name").GetComponent<Text>();
                propItem = new PropItem();
                propItem.BindGameObject(transform.Find("PropItem").gameObject);
            }

            public void UpdateCellView(int _index)
            {
                rewardIndex = _index;
                var csvData = CSVSynthesis.Instance.GetConfData(Sys_Activity_2048.Instance.curActivityId);
                if (csvData != null)
                {
                    maxTime = csvData.Reward_Time[rewardIndex];
                    uint dropId = csvData.Reward_Id[rewardIndex];
                    txtFinishTime.text = Sys_Activity_2048.Instance.GetRewardTimeText(maxTime);
                    var dropItems = CSVDrop.Instance.GetDropItem(dropId);
                    itemIdCountData = dropItems[0];
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(dropItems[0].id, dropItems[0].count, true, false, false, false, false, true, false, true, OnItemClick, true, false);
                    propItem.SetData(itemData, EUIID.UI_Activity2048);

                    bool canGet = Sys_Activity_2048.Instance.CheckRewardCanGet(maxTime);
                    bool isGet = Sys_Activity_2048.Instance.CheckRewardIsGet(rewardIndex);
                    propItem.SetBreathing(canGet && !isGet);
                    propItem.SetGot(isGet);
                }
            }


            private void OnItemClick(PropItem itemData)
            {
                bool canGet = Sys_Activity_2048.Instance.CheckRewardCanGet(maxTime);
                bool isGet = Sys_Activity_2048.Instance.CheckRewardIsGet(rewardIndex);
                if (canGet && !isGet)
                {
                    //请求领奖
                    Sys_Activity_2048.Instance.ReqActivity2048GetAward((uint)rewardIndex);
                }
                else
                {
                    PropIconLoader.ShowItemData showItemData = new PropIconLoader.ShowItemData(itemIdCountData.id, itemIdCountData.count, false, false, false, false, false, false, true);
                    UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Decompose, showItemData));
                }
            }
        }
        #endregion
    }
}
