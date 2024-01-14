using Framework;
using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Logic
{
    public class Num
    {
        private GameObject mLayoutOwner;

        private GameObject mGO;

        private Transform mParent;

        private RectTransform rectTransform;

        private GridLayoutGroup gridLayoutGroup;

        private Vector3 mTargetPos;

        public int num;//表示2的幂次方

        private CSVLittleGame_Elimination.Data cSVLittleGame_EliminationData;

        private List<uint> listImageId;

        public int x;

        public int y;

        private float tweenInDelayTime = 0;

        private CoroutineHandler mCoroutineHandler;

        private uint imageId;

        public void BindGameObject(GameObject gameObject, GameObject root, Transform parent, CSVLittleGame_Elimination.Data _cSVLittleGame_EliminationData)
        {
            mGO = gameObject;
            rectTransform = mGO.transform as RectTransform;
            mLayoutOwner = root;
            mParent = parent;
            mGO.transform.SetParent(mParent);
            mGO.transform.localPosition = Vector3.zero;
            mGO.transform.localScale = Vector3.zero;
            gridLayoutGroup = root.GetComponent<GridLayoutGroup>();
            cSVLittleGame_EliminationData = _cSVLittleGame_EliminationData;
            listImageId = cSVLittleGame_EliminationData.imageId;
        }
        /// <summary>
        /// 重写方法 直接传ListImageID
        /// </summary>
        public void BindGameObject(GameObject gameObject, GameObject root, Transform parent, List<uint> imageIds)
        {
            mGO = gameObject;
            rectTransform = mGO.transform as RectTransform;
            mLayoutOwner = root;
            mParent = parent;
            mGO.transform.SetParent(mParent);
            mGO.transform.localPosition = Vector3.zero;
            mGO.transform.localScale = Vector3.zero;
            gridLayoutGroup = root.GetComponent<GridLayoutGroup>();
            listImageId = imageIds;
        }


        public void SetData(int _x, int _y, int _num)
        {
            x = _x;
            y = _y;
            num = _num;
            imageId = listImageId[num - 1];
        }

        public void InitShow()
        {
            int _result = 0;
            _result = 2 << num - 1;
            ImageHelper.SetIcon(mGO.GetComponent<Image>(), imageId);
        }

        public void InitPos()
        {
            SetTargetPos();
            rectTransform.anchoredPosition = mTargetPos;
        }


        public void setDelayTime(float delayTime)
        {
            if (delayTime==0)
            {
                mGO.transform.Find("Fx_ui_2048").gameObject.SetActive(true);
            }
            this.tweenInDelayTime = delayTime;
            if (mCoroutineHandler != null)
            {
                CoroutineManager.Instance.Stop(mCoroutineHandler);
                mCoroutineHandler = null;
            }
            mCoroutineHandler = CoroutineManager.Instance.StartHandler(StartShow());
        }

        private IEnumerator StartShow()
        {
            yield return new WaitForSeconds(tweenInDelayTime);
            mGO.transform.DOScale(Vector3.one, 0.1f);
        }

        private bool isDisappear = false;
        public void Disappear()
        {
            isDisappear = true;
        }

        public bool MoveToPosition(int targetX, int targetY, bool isNeedUpdateComponentArray = true)
        {
            bool temp = x != targetX || y != targetY;

            if (temp)
            {
                Sys_LittleGame_2048.Instance.numComponentArray[x][y] = null;
            }
            x = targetX;
            y = targetY;
            if (isNeedUpdateComponentArray)
                Sys_LittleGame_2048.Instance.numComponentArray[x][y] = this;

            SetTargetPos();

            rectTransform.DOAnchorPos(mTargetPos, 0.2f).onComplete += OnMoveComplete;

            return temp;
        }
        /// <summary>
        /// 重写移动函数
        /// </summary>
        public bool MoveToPosition(int targetX, int targetY, Num[][] array, bool isNeedUpdateComponentArray = true)
        {
            bool temp = x != targetX || y != targetY;

            if (temp)
            {
                array[x][y] = null;
            }
            x = targetX;
            y = targetY;
            if (isNeedUpdateComponentArray)
                array[x][y] = this;

            SetTargetPos();

            rectTransform.DOAnchorPos(mTargetPos, 0.2f).onComplete += OnMoveComplete;

            return temp;
        }

        private void OnMoveComplete()
        {
            if (isDisappear)
            {
                Dispose();
            }
        }

        private void SetTargetPos()
        {
            //mTargetPos = new Vector2(x * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x) + 0.5f * gridLayoutGroup.cellSize.x,
            //    -y * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y) - 0.5f * gridLayoutGroup.cellSize.y);
            mTargetPos = new Vector2(x * (92 + 0) + 0.5f * 92,
               -y * (92 + 0) - 0.5f * 92);
        }

        public void Dispose()
        {
            mGO.transform.Find("Fx_ui_2048_01").gameObject.SetActive(true);
           // Sys_LittleGame_2048.Instance.RecycleNum(this);
            mGO.transform.DOScale(Vector3.zero, 0.5f).onComplete = () =>
            {
                GameObject.Destroy(mGO);
            };
        }
    }
}


