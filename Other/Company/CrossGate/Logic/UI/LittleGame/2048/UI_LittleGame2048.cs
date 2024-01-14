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
//#if USE_ADDRESSABLE_ASSET
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
//#else
//using Lib.AssetLoader;
//#endif

namespace Logic
{
    public class UI_LittleGame2048 : UIBase
    {
        private GameObject numPrefab;
        private GameObject root;
        private GameObject eventBG;
        private GameObject countdown;
        private GameObject headIcon;
        private GameObject temp;
        private GameObject tempparent;
        private Animator animator;
        private Transform headparent;
        private Transform parent;
        private Button close;
        private Button start;
        private Button again;
        private Button rule;
        private Text time;
        private Text desc;
        private Text target;
        private Text targetscore;
        private GameState state = GameState.None;
        private bool bvaild=false;
        private CSVLittleGame_Elimination.Data cSVLittleGame_EliminationData;
        private int actualCount;
        private int targetNum;
        private int targetNumCount;

//#if USE_ADDRESSABLE_ASSET
        AsyncOperationHandle<GameObject> requestRef;
//#else
//        private AssetRequest requestRef = null;
//#endif
        private uint _taskId;
        private uint _gameId;

        protected override void OnOpen(object arg)
        {
            Tuple<uint, object> tuple = arg as Tuple<uint, object>;
            if (tuple != null)
            {
                _taskId = tuple.Item1;
                _gameId = Convert.ToUInt32(tuple.Item2);
            }
        }

        protected override void OnLoaded()
        {
            numPrefab = transform.Find("Animator/View_bottom/numPrefab").gameObject;
            countdown = transform.Find("Animator/GameObject").gameObject;
            root = transform.Find("Animator/View_bottom/root").gameObject;
            parent = transform.Find("Animator/View_bottom/parent");
            tempparent = transform.Find("Animator/View_left/Scroll_View/List").gameObject;
            temp = tempparent.transform.Find("Item").gameObject;
            headparent = transform.Find("Animator/NpcRoot");
            close = transform.Find("Animator/Btn_Close").GetComponent<Button>();
            start = transform.Find("Animator/View_left/Btn_01").GetComponent<Button>();
            again = transform.Find("Animator/Button_Reset").GetComponent<Button>();
            rule = transform.Find("Animator/View_left/Button_Tips").GetComponent<Button>();
            time = transform.Find("Animator/View_Clock/Text_Time").GetComponent<Text>();
            desc = transform.Find("Animator/View_left/Text_Target").GetComponent<Text>();
            target = transform.Find("Animator/View_left/Viewport/Item/Target_Dark/Text").GetComponent<Text>();
            targetscore= transform.Find("Animator/View_left/Viewport/Item/Target_Dark/Text/Text_Number").GetComponent<Text>();
            eventBG = transform.Find("Animator/eventBG").gameObject;
            close.onClick.AddListener(() =>
                {
                    UIManager.CloseUI(EUIID.UI_LittleGmae_2048);
                    UIManager.CloseUI(EUIID.UI_CountDown);
                }
            );
            rule.onClick.AddListener(() =>
            {
                UIManager.OpenUI(EUIID.UI_LittleGame_Tips, false, cSVLittleGame_EliminationData.gameDescribe);
            });
            start.onClick.AddListener(StartGame);
            again.onClick.AddListener(PlayAgain);
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventBG);
            eventListener.AddEventListener(EventTriggerType.PointerDown, OnPointClick);
            eventListener.AddEventListener(EventTriggerType.PointerUp, OnPointUp);
        }

        private void ParseData()
        {
            cSVLittleGame_EliminationData = CSVLittleGame_Elimination.Instance.GetConfData(_gameId);
            targetNumCount = (int)cSVLittleGame_EliminationData.Goal[0];
            targetNum = (int)cSVLittleGame_EliminationData.Goal[1];
        }

        protected override void OnShow()
        {
            ParseData();
            TextHelper.SetText(desc, cSVLittleGame_EliminationData.tips);
            TextHelper.SetText(target, cSVLittleGame_EliminationData.Goaltips);
            GameEmpty();
            ShowList();
            SetScore();
            if (null==headIcon)
            {
                LoadHeadIconAssetAsyn(cSVLittleGame_EliminationData.image_path);
            }
        }

        private void GameEmpty()
        {
            if (state == GameState.None|| state == GameState.End)
            {
                again.gameObject.SetActive(false);
                ImageHelper.SetImageGray(start.GetComponent<Image>(), false);
                start.interactable = true;
                TextHelper.SetText(start.GetComponentInChildren<Text>(), 1003013);
            }
        }

        private void Gameing()
        {
            if (state == GameState.Playing)
            {
                again.gameObject.SetActive(true);
                ImageHelper.SetImageGray(start.GetComponent<Image>(), true);
                start.interactable = false;
                TextHelper.SetText(start.GetComponentInChildren<Text>(), 1003012);
            }
        }

        protected override void OnClose()
        {
            Sys_LittleGame_2048.Instance.Clear();
            Sys_LittleGame.Instance.EndGame();
            state = GameState.None;
            time.color = Color.white;

            AddressablesUtil.ReleaseInstance(ref requestRef, OnAssetsLoaded);
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_LittleGame.Instance.eventEmitter.Handle(Sys_LittleGame.EEvents.StartGame, StartGame, toRegister);
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, toRegister);
        }
      

        private void ShowList()
        {
            if (tempparent.transform.childCount == targetNum - 1)
                return;
            for (int i = 0; i < targetNum-2; i++)
            {
               FrameworkTool.CreateGameObject(temp, tempparent);
            }
            for (int i = 0; i < tempparent.transform.childCount; i++)
            {
                Transform t = tempparent.transform.GetChild(i);
                ImageHelper.SetIcon(t.Find("Image_Icon1").GetComponent<Image>(), cSVLittleGame_EliminationData.imageId[i]);
                ImageHelper.SetIcon(t.Find("Image_Icon2").GetComponent<Image>(), cSVLittleGame_EliminationData.imageId[i]);
                ImageHelper.SetIcon(t.Find("Image_Icon3").GetComponent<Image>(), cSVLittleGame_EliminationData.imageId[i+1]);
            }
        }

        private void StartGame()
        {
            if (state == GameState.Playing)
                return;
            actualCount = 0;
            SetScore();
            time.color = Color.white;
            UIManager.OpenUI(EUIID.UI_CountDown, false, new Tuple<float,Vector3,Action>(3, countdown.transform.position, OnStartGame));
        }

        private void OnStartGame()
        {
            state = GameState.Playing;
            Gameing();
            Sys_LittleGame_2048.Instance.Clear();
            Sys_LittleGame.Instance.StartGame(cSVLittleGame_EliminationData.time, OnCountDown,null);
            GenerateNumber();
            PlayHeadAnimation("UI_LittleGame_NpcRoot_Standby_Open");
        }

        private void OnCountDown(int _time)
        {
            if (_time<4)
                time.color = Color.red;
        }

        private void PlayAgain()
        {
            if (state== GameState.Playing)
            {
                state = GameState.End;
                Sys_LittleGame.Instance.EndGame();
                Sys_LittleGame_2048.Instance.PlayAgain();
            }
        }

        private void OnReconnectResult(bool result)
        {
            if (state == GameState.Playing)
            {
                Gameing();
            }
        }

        private void GameOver()
        {
            PlayHeadAnimation("UI_LittleGame_NpcRoot_Failure_Open");
            Sys_LittleGame_2048.Instance.success = false;
            UIManager.OpenUI(EUIID.UI_LittleGame_Result, false, new Tuple<bool, Action>(false,null));
            state = GameState.End;
            Sys_LittleGame.Instance.EndGame();
            GameEmpty();
        }

        private void GameWin()
        {
            PlayHeadAnimation("UI_LittleGame_NpcRoot_Victory_Open");
            Sys_LittleGame_2048.Instance.success = true;
            UIManager.OpenUI(EUIID.UI_LittleGame_Result, false, new Tuple<bool, Action>(true, () => 
            {
                UIManager.CloseUI(EUIID.UI_LittleGmae_2048);
                Sys_Task.Instance.ReqStepGoalFinishEx(_taskId);
            }));
            state = GameState.End;
            Sys_LittleGame.Instance.EndGame();
        }

        protected override void OnUpdate()
        {
            time.text = Sys_LittleGame.Instance.GetTimeFormat();

            if (Sys_LittleGame.Instance.TimeOut)
            {
                GameOver();
            }
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    Sys_LittleGame_2048.Instance.Log();
            //}
        }
        
        private void LoadHeadIconAssetAsyn(string path)
        {
            AddressablesUtil.InstantiateAsync(ref requestRef, path, OnAssetsLoaded);
        }

        private void OnAssetsLoaded(AsyncOperationHandle<GameObject> handle)
        {
            headIcon = handle.Result;
            if (null != headIcon)
            {
                headIcon.transform.SetParent(headparent);
                RectTransform rectTransform = headIcon.transform as RectTransform;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }
            animator = headIcon.transform.Find("Amei").GetComponent<Animator>();
            PlayHeadAnimation("UI_LittleGame_NpcRoot_Standby_Open");
        }

        private void PlayHeadAnimation(string path)
        {
            animator.Play(path, -1, 0);
        }

        private GameObject InstantiateGameObject()
        {
            GameObject @object= GameObject.Instantiate<GameObject>(numPrefab);
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
                    if (Sys_LittleGame_2048.Instance.numComponentArray[x][y] != null)
                    {
                        if (Sys_LittleGame_2048.Instance.numComponentArray[x][y].num == targetNum)
                        {
                            count++;
                        }
                    }
                }
            }
            if (count!=actualCount)
            {
                actualCount = count;
                SetScore();
            }

            if (count>=targetNumCount)
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
                    if (Sys_LittleGame_2048.Instance.numComponentArray[x][y] == null)
                    {
                        return false;
                    }
                }
            }

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (Sys_LittleGame_2048.Instance.numComponentArray[x][y].num == Sys_LittleGame_2048.Instance.numComponentArray[x][y + 1].num)
                    {
                        return false;
                    }
                }
            }

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (Sys_LittleGame_2048.Instance.numComponentArray[x][y].num == Sys_LittleGame_2048.Instance.numComponentArray[x + 1][y].num)
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
                        if (Sys_LittleGame_2048.Instance.numComponentArray[x][y] == null)
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
                        if (Sys_LittleGame_2048.Instance.numComponentArray[x][y] == null)
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
            num.BindGameObject(InstantiateGameObject(), root, parent, cSVLittleGame_EliminationData);
            num.SetData(X, Y, number);
            num.InitShow();
            num.InitPos();
            num.setDelayTime(tweenInDelayTime);
            Sys_LittleGame_2048.Instance.numComponentArray[X][Y] = num;
        }

        private Vector3 mouseDownPosition;

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
                            if (Sys_LittleGame_2048.Instance.numComponentArray[x][y] == null) continue;
                            if (preNum == null)
                            {
                                preNum = Sys_LittleGame_2048.Instance.numComponentArray[x][y];
                                index--;
                            }
                            else
                            {
                                if (preNum.num == Sys_LittleGame_2048.Instance.numComponentArray[x][y].num)
                                {
                                    // 合并这两个都移动到目标位置 然后消失 然后创建出来合并后的数字
                                    countCombine++;
                                    GenerateNumber(index, y, preNum.num + 1, 0.3f);
                                    preNum.Disappear();
                                    Sys_LittleGame_2048.Instance.numComponentArray[x][y].Disappear();
                                    preNum = null;
                                    isNeedUpdateComponentArray = false;
                                }
                                else
                                {
                                    preNum = Sys_LittleGame_2048.Instance.numComponentArray[x][y];
                                    index--;
                                }
                            }
                            // move to (index,y)
                            if (Sys_LittleGame_2048.Instance.numComponentArray[x][y].MoveToPosition(index, y, isNeedUpdateComponentArray))
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
                            if (Sys_LittleGame_2048.Instance.numComponentArray[x][y] == null) continue;
                            if (preNum == null)
                            {
                                preNum = Sys_LittleGame_2048.Instance.numComponentArray[x][y];
                                index++;
                            }
                            else
                            {
                                if (preNum.num == Sys_LittleGame_2048.Instance.numComponentArray[x][y].num)
                                {
                                    // 合并这两个都移动到目标位置 然后消失 然后创建出来合并后的数字
                                    countCombine++;
                                    GenerateNumber(index, y, preNum.num + 1, 0.3f);
                                    preNum.Disappear();
                                    Sys_LittleGame_2048.Instance.numComponentArray[x][y].Disappear();
                                    preNum = null;
                                    isNeedUpdateComponentArray = false;
                                }
                                else
                                {
                                    preNum = Sys_LittleGame_2048.Instance.numComponentArray[x][y];
                                    index++;
                                }
                            }
                            // move to (index,y)
                            if (Sys_LittleGame_2048.Instance.numComponentArray[x][y].MoveToPosition(index, y, isNeedUpdateComponentArray))
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
                            if (Sys_LittleGame_2048.Instance.numComponentArray[x][y] == null) continue;
                            if (preNum == null)
                            {
                                preNum = Sys_LittleGame_2048.Instance.numComponentArray[x][y];
                                index++;
                            }
                            else
                            {
                                if (preNum.num == Sys_LittleGame_2048.Instance.numComponentArray[x][y].num)
                                {
                                    // 合并这两个都移动到目标位置 然后消失 然后创建出来合并后的数字
                                    countCombine++;
                                    GenerateNumber(x, index, preNum.num + 1, 0.3f);
                                    preNum.Disappear();
                                    Sys_LittleGame_2048.Instance.numComponentArray[x][y].Disappear();
                                    preNum = null;
                                    isNeedUpdateComponentArray = false;
                                }
                                else
                                {
                                    preNum = Sys_LittleGame_2048.Instance.numComponentArray[x][y];
                                    index++;
                                }
                            }
                            // move to (index,y)
                            if (Sys_LittleGame_2048.Instance.numComponentArray[x][y].MoveToPosition(x, index, isNeedUpdateComponentArray))
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
                            if (Sys_LittleGame_2048.Instance.numComponentArray[x][y] == null) continue;
                            if (preNum == null)
                            {
                                preNum = Sys_LittleGame_2048.Instance.numComponentArray[x][y];
                                index--;
                            }
                            else
                            {
                                if (preNum.num == Sys_LittleGame_2048.Instance.numComponentArray[x][y].num)
                                {
                                    // 合并这两个都移动到目标位置 然后消失 然后创建出来合并后的数字
                                    countCombine++;
                                    GenerateNumber(x, index, preNum.num + 1, 0.3f);
                                    preNum.Disappear();
                                    Sys_LittleGame_2048.Instance.numComponentArray[x][y].Disappear();
                                    preNum = null;
                                    isNeedUpdateComponentArray = false;
                                }
                                else
                                {
                                    preNum = Sys_LittleGame_2048.Instance.numComponentArray[x][y];
                                    index--;
                                }
                            }
                            // move to (index,y)
                            if (Sys_LittleGame_2048.Instance.numComponentArray[x][y].MoveToPosition(x, index, isNeedUpdateComponentArray))
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

            if (CheckWin() )
            {
                GameWin(); 
            }
            if (CheckGameOver())
            {
                GameOver();
            }
        }

        public void OnPointClick(BaseEventData baseEventData)
        {
            mouseDownPosition = Input.mousePosition;
        }

        public void OnPointUp(BaseEventData baseEventData)
        {
            if (Sys_LittleGame.Instance.TimeOut)
                return;

            if (state== GameState.Playing)
                MoveNum();
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
    }
}


