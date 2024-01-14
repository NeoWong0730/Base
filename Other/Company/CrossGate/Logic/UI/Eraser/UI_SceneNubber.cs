using UnityEngine;
using System.Collections;
using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using Lib.Core;
using System;
using Framework;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Logic
{
    public class UI_SceneNubber : UIBase
    {
        private RectTransform rectTransform;
        private Transform showRoot;
        private Transform showParent;
        private Button skipButton;
        private GameObject animRoot;
        private GameObject guide;
        private Animator animator;

        private int row;
        private int column;
        private float height;
        private float weith;
        private Vector2 lastPoint;
        private Vector2 point;
        private Vector2 offest;
        private float boundary;
        private float skipTime;
        private float waitEndTime;
        private float dragBeginTime;
        private float animRootShowTime;

        private CSVSceneNubber.Data cSVSceneNubberData;
        private List<uint> areas = new List<uint>();      //区域编号
        private List<uint> serials = new List<uint>();  //ui编号
        private bool allserialsRemoved = false;
        private bool reachMaxWaitTime = false;
        private bool canDrag = false;
        private bool bAnimRootActive=false;
        private uint taskId;
        private uint gameId;

        private Vector3 _startRot;
        private Vector3 _endRot;
        private Vector3 _startPos;
        private Vector3 _endPos;

        private bool _bGuideShowed=false;
        private uint firsttaskId;

        protected override void OnOpen(object arg)
        {
            Tuple<uint, object> tuple = arg as Tuple<uint, object>;
            if (tuple != null)
            {
                taskId = tuple.Item1;
                gameId = Convert.ToUInt32(tuple.Item2);
            }
        }


        protected override void OnLoaded()
        {
            animRoot = transform.Find("Animator").gameObject;
            guide = animRoot.transform.Find("Guide/Image_Guide_Animator").gameObject;
            animator = guide.GetComponent<Animator>();
            rectTransform = transform.Find("Animator/dragBg") as RectTransform;
            skipButton = transform.Find("Animator/SkipButton").GetComponent<Button>();
            showRoot = transform.Find("Animator/show");
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(rectTransform);
            eventListener.AddEventListener(EventTriggerType.PointerDown, PointerDown);
            eventListener.AddEventListener(EventTriggerType.PointerUp, PointerUp);
            eventListener.AddEventListener(EventTriggerType.Drag, Drag);
            skipButton.onClick.AddListener(Skip);
            GameCenter.mCameraController.RecordLastCameraData();
            animRoot.SetActive(false);

            firsttaskId = uint.Parse(CSVParam.Instance.GetConfData(809).str_value);
        }

        protected override void OnShow()
        {
            height = rectTransform.rect.height;
            weith = rectTransform.rect.width;
            ParseData();
            skipButton.gameObject.SetActive(false);
            animRoot.gameObject.SetActive(false);
            for (int i = 0; i < showParent.childCount; i++)
            {
                showParent.GetChild(i).gameObject.SetActive(true);
            }
        }

        protected override void OnShowEnd()
        {
            SetCamera();
        }
        
        protected override void OnHide()
        {
            ClearData();
            RevertCamera();
            Sys_Task.Instance.ReqStepGoalFinishEx(taskId);
        }

        private void ParseData()
        {
            cSVSceneNubberData = CSVSceneNubber.Instance.GetConfData(gameId);

            foreach (var item in cSVSceneNubberData.zozeId)
            {
                areas.Add((uint)item);
            }

            column = row = (int)Mathf.Sqrt(areas.Count);
            boundary = cSVSceneNubberData.minimumMove;
            skipTime = cSVSceneNubberData.skipTime;
            waitEndTime = cSVSceneNubberData.waitTime;
            dragBeginTime = cSVSceneNubberData.openTime + cSVSceneNubberData.timeDelay;
            animRootShowTime = cSVSceneNubberData.openTime;

            showParent = showRoot.GetChild((int)cSVSceneNubberData.material - 1);

            for (int i = 0; i < showParent.childCount; i++)
            {
                serials.Add((uint)i);
            }
        }

        private void ClearData()
        {
            serials.Clear();
            areas.Clear();
            allserialsRemoved = false;
            reachMaxWaitTime = false;
            bAnimRootActive = false;
            canDrag = false;
        }

        protected override void OnUpdate()
        {
            dragBeginTime -= deltaTime;

            if (dragBeginTime<=0)
                canDrag = true;

            if (!bAnimRootActive)
            {
                animRootShowTime -= deltaTime;
                if (animRootShowTime <= 0)
                {
                    animRoot.SetActive(true);
                    bAnimRootActive = true;
                }
            }

            if (!canDrag)  return;

            skipTime -= deltaTime;

            if(!TaskHelper.HasFinished(firsttaskId)&& !_bGuideShowed)
            {
                guide.SetActive(true);
                animator.Play("UI_SceneNubber_Animator_Guide_Open", -1, 0);
                _bGuideShowed = true;
            }

            if (skipTime <= 0) 
                reachMaxWaitTime = true;
            
            if (reachMaxWaitTime)
            {
                reachMaxWaitTime = false;
                skipButton.gameObject.SetActive(true);
            }

            if (allserialsRemoved)
            {
                waitEndTime -= deltaTime;
                if (waitEndTime <= 0)
                {
                    allserialsRemoved = false;
                    EndShow();
                }
            }
        }


        private void SetCamera()
        {
            CameraData cameraData = new CameraData
            {
                pith = cSVSceneNubberData.camera[0] / 10000f,
                yaw = cSVSceneNubberData.camera[1] / 10000f,
                roll = cSVSceneNubberData.camera[2] / 10000f,
                distance = cSVSceneNubberData.camera[3] / 10000f,
                fov = cSVSceneNubberData.camera[7] / 10000f,
                lookPointOffset = new Vector3(cSVSceneNubberData.camera[4] / 10000f, cSVSceneNubberData.camera[5] / 10000f, cSVSceneNubberData.camera[6] / 10000f)
            };
            GameCenter.mCameraController.SetCameraData(cameraData, cSVSceneNubberData.returnTime);

            _startRot = GameCenter.mainHero.transform.eulerAngles;
            _endRot = new Vector3(cSVSceneNubberData.dialogueParameter[3] / 10000f, cSVSceneNubberData.dialogueParameter[4] / 10000f,
                cSVSceneNubberData.dialogueParameter[5] / 10000f);
            _startPos = GameCenter.mainHero.transform.position;
            _endPos = new Vector3(cSVSceneNubberData.consultPosition[0] / 10000, cSVSceneNubberData.consultPosition[1] / 10000, cSVSceneNubberData.consultPosition[2] / 10000) +
                new Vector3(cSVSceneNubberData.dialogueParameter[0] / 10000f, cSVSceneNubberData.dialogueParameter[1] / 10000f, cSVSceneNubberData.dialogueParameter[2] / 10000f);
            DOTween.To(() => GameCenter.mainHero.transform.position, x => GameCenter.mainHero.transform.position = x, _endPos, cSVSceneNubberData.returnTime).SetEase(Ease.Linear);
            DOTween.To(() => GameCenter.mainHero.transform.eulerAngles, x => GameCenter.mainHero.transform.eulerAngles = x, _endRot, cSVSceneNubberData.returnTime).SetEase(Ease.Linear);

            if (GameCenter.mainHero != null)
            {
                GameCenter.mainHero.SetLayerHide();
            }

            //foreach (var actor in GameCenter.otherActorsDic.Values)
            //{
            //    actor.gameObject?.SetActive(false);
            //}
        }

        private void RevertCamera()
        {
            GameCenter.mCameraController.RevertToLastCameraData(0);

            DOTween.To(() => GameCenter.mainHero.transform.position, x => GameCenter.mainHero.transform.position = x, _startPos, 0).SetEase(Ease.Linear);
            DOTween.To(() => GameCenter.mainHero.transform.eulerAngles, x =>
            GameCenter.mainHero.transform.eulerAngles = x, _startRot,0).SetEase(Ease.Linear).onComplete+=()=> 
            {
                if (GameCenter.mainHero != null)
                {
                    GameCenter.mainHero.ReturnCacheLayer();
                }
            };
            //foreach (var actor in GameCenter.otherActorsDic.Values)
            //{
            //    actor.gameObject?.SetActive(true);
            //}
        }


        private void Skip()
        {
            for (int i = 0; i < showParent.childCount; i++)
            {
                if (serials.Contains((uint)i))
                {
                    PlayExitAnim(i);
                }
            }
            allserialsRemoved = true;
        }

        private void PlayExitAnim(int index)
        {
            GameObject child = showParent.GetChild(index).gameObject;
            child.GetComponent<Animator>().Play("cloud" + index, -1, 0);
            //child.SetActive(false);
        }

        private void EndShow()
        {
            UIManager.CloseUI(EUIID.UI_SceneNubber);
        }

        public void PointerDown(BaseEventData eventData)
        {
            PointerEventData pointerEventData = eventData as PointerEventData;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pointerEventData.position, pointerEventData.pressEventCamera, out lastPoint);
        }

        public void PointerUp(BaseEventData eventData)
        {
            PointerEventData pointerEventData = eventData as PointerEventData;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pointerEventData.position, pointerEventData.pressEventCamera, out point);
            if (serials.Count == 0)
                allserialsRemoved = true;
        }

        public void Drag(BaseEventData eventData)
        {
            if (!canDrag)
                return;

            guide.SetActive(false);

            if (allserialsRemoved)
                return;

            if (serials.Count == 0)
                allserialsRemoved = true;
           
            PointerEventData pointerEventData = eventData as PointerEventData;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pointerEventData.position, pointerEventData.pressEventCamera, out point);

            offest = point - lastPoint;
            if (offest.magnitude >= boundary)
            {
                GetAreaIndex();
                lastPoint = point;
            }
        }
        
        private void GetAreaIndex()
        {
            int index_x = GetValueIndex(0, weith, lastPoint.x, column);
            int index_y = GetValueIndex(0, height, lastPoint.y, row);
            int index = (index_y - 1) * column + index_x;
            List<uint> clouds =cSVSceneNubberData.moveId[index-1];
            foreach (var item in clouds)
            {
                if (serials.Remove(item))
                {
                    PlayExitAnim((int)item);
                }
            }
        }

        private int GetValueIndex(float min, float max, float curValue, int count)
        {
            int index = 0;
            float detla = (max - min) / count;
            for (int i = 1; i < count; i++)
            {
                if (curValue >= min + detla * i && curValue < min + detla * (i + 1))
                {
                    index = i;
                    break;
                }
            }
            return index + 1;
        }
    }
}

