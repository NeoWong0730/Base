using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Logic.Core;

namespace Logic
{
    public class UIListNumberParam
    {
        public List<string> nameList;
        public Vector3 Pos = Vector3.zero;
        public RectTransform Rec;
    }
    public class UI_ListNumber_Ceil
    {
        private Text txt_Name;
        public void BindGameObject(Transform transform)
        {
            txt_Name = transform.Find("Text_Name").GetComponent<Text>();
        }

        public void SetData(string _str)
        {
            txt_Name.text = _str;
        }
    }
    public class UI_ListNumber : UIBase
    {
        #region 界面显示
        private Button closeBtn;
        private Transform transAni;
        private RectTransform recTran;
        private InfinityGrid PanelScrollGrid;
        UIListNumberParam listParam;
        #endregion
        #region 系统函数

        protected override void OnOpen(object arg)
        {
            if (arg!=null)
            {
                listParam = (UIListNumberParam)arg;
            }

        }
        protected override void OnLoaded()
        {
            transAni = transform.Find("Animator/Content");
            recTran = transform.Find("Animator/Content").GetComponent<RectTransform>();
            closeBtn = transform.Find("Black").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseButtonClicked);
            PanelScrollGrid = transform.Find("Animator/Content/ScrollView").GetComponent<InfinityGrid>();
            PanelScrollGrid.CellCount = 10;
            PanelScrollGrid.onCreateCell += OnCreateCell;
            PanelScrollGrid.onCellChange += OnCellChange;
        }

        protected override void OnShow()
        {
            if (listParam!=null)
            {
                if (!listParam.Pos.Equals(Vector3.zero))
                {
                    Vector3 _lVec = TransformCaculate(listParam.Pos);
                    Vector2 screenPoint = new Vector2(_lVec.x, _lVec.y);
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(gameObject.GetComponent<RectTransform>(), screenPoint, CameraManager.mUICamera, out Vector3 pos);
                    transAni.position = pos;
                }
                PanelScrollGrid.CellCount = listParam.nameList.Count;
                PanelScrollGrid.ForceRefreshActiveCell();
            }
            
        }
        #endregion
        #region Function
        private Vector3 TransformCaculate(Vector3 _vec)
        {
            Vector3 _NewVect=_vec;
            if (_vec.x > Sys_OperationalActivity.Instance.ScrollViewVect.x)
            {
                _NewVect += new Vector3((listParam.Rec.rect.width + recTran.rect.width )* (-1), recTran.rect.height*2, 0);
            }
            else
            {
                _NewVect += new Vector3(listParam.Rec.rect.width+ recTran.rect.width*2, recTran.rect.height*2, 0);
            }

            return _NewVect;
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_ListNumber_Ceil entry = cell.mUserData as UI_ListNumber_Ceil;
            entry.SetData(listParam.nameList[index]);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_ListNumber_Ceil entry = new UI_ListNumber_Ceil();
            entry.BindGameObject(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_ListNumber);
        }
        #endregion
    }
}