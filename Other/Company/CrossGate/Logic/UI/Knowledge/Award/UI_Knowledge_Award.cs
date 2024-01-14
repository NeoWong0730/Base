using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Knowledge_Award : UIBase
    {
        private InfinityGridLayoutGroup gridGroup;
        private Dictionary<GameObject, UI_Knowledge_Award_Cell> dicCells = new Dictionary<GameObject, UI_Knowledge_Award_Cell>();
        private int visualGridCount;

        private Sys_Knowledge.ETypes _eType;
        
        protected override void OnLoaded()
        {            
            Button btnClose = transform.Find("Black").GetComponent<Button>();
            btnClose.onClick.AddListener(()=>{ this.CloseSelf(); });

            gridGroup = transform.Find("Animator/View_Content/Scroll View01/Viewport/Content").GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 5;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform tran = gridGroup.transform.GetChild(i);

                UI_Knowledge_Award_Cell cell = new UI_Knowledge_Award_Cell();
                cell.Init(tran);
                dicCells.Add(tran.gameObject, cell);
            }
        }

        protected override void OnOpen(object arg)
        {            
            if (arg != null)
                _eType = (Sys_Knowledge.ETypes)(arg);
        }

        protected override void OnShow()
        {         
            UpdateInfo();
        }
        
        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dicCells.ContainsKey(trans.gameObject))
            {
                UI_Knowledge_Award_Cell cell = dicCells[trans.gameObject];
                cell.UpdateInfo(_eType, index);
            }
        }

        private void UpdateInfo()
        {
            List<uint> listStages = CSVStageReward.Instance.GetConfData((uint)_eType).stage;

            visualGridCount = listStages.Count;
            gridGroup.SetAmount(visualGridCount);
        }
    }
}


