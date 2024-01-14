using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;
using System;
using UnityEngine;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Energyspar_Main_Left : UIComponent
    {
        public bool isRecverRefresh = false;

        /// <summary>
        /// 视图布局-01固定6
        /// </summary>
        private Transform energyGronp1;
        private UICircleLayoutGroup energyGronp2;
        public GameObject ceilGo;
        private UI_Energyspar_Main_Left_Ceil mainEnergysparGo;
        private uint selectId;

        private Dictionary<uint, UI_Energyspar_Main_Left_Ceil> allEnerguspar = new Dictionary<uint, UI_Energyspar_Main_Left_Ceil>();
        private IListener listener;

        protected override void Loaded()
        {
            energyGronp1 = transform.Find("SparGroup1");
            energyGronp2 = transform.Find("SparGroup2").GetComponent<UICircleLayoutGroup>();
            ceilGo = transform.Find("MainSpar").gameObject;
            mainEnergysparGo = new UI_Energyspar_Main_Left_Ceil();
            mainEnergysparGo.Init(ceilGo.transform);
        }

        private void RefrshDicData()
        {
            FrameworkTool.DestroyChildren(energyGronp1.gameObject);
            FrameworkTool.DestroyChildren(energyGronp2.gameObject);
            allEnerguspar.Clear();
            uint careerId = Sys_Role.Instance.Role.Career;
            if (careerId == 100)
            {
                careerId = 101;
            }             
            List<CSVStone.Data>  tempList = Sys_StoneSkill.Instance.GetCareerStone(careerId);
            int group1Num = 0;
            uint selectId = 0;
            for (int i = 0; i < tempList.Count; i++)
            {
                if(tempList[i].career_limit != null)
                {
                    mainEnergysparGo.SetCeilData(tempList[i]);
                    mainEnergysparGo.AddActionListen(OnSelect);
                    allEnerguspar.Add(tempList[i].id, mainEnergysparGo);
                    selectId = tempList[i].id;                    
                }
                else
                {
                    GameObject go;
                    if (group1Num < 6)
                    {
                        go = GameObject.Instantiate<GameObject>(ceilGo, energyGronp1);                       
                        group1Num++;
                    }
                    else
                    {
                        go = GameObject.Instantiate<GameObject>(ceilGo, energyGronp2.transform);
                    }
                    UI_Energyspar_Main_Left_Ceil ceil = new UI_Energyspar_Main_Left_Ceil();
                    ceil.Init(go.transform);
                    ceil.SetCeilData(tempList[i]);
                    ceil.AddActionListen(OnSelect);
                    allEnerguspar.Add(tempList[i].id, ceil);
                }                
            }
            if(energyGronp2.transform.childCount > 0)
            {
                energyGronp2.m_Sapce = 360.0f / energyGronp2.transform.childCount;
            }            
            listener?.OnSelect(selectId);
        }

        public void ResetEx(uint stoneId)
        {
            ResetCeil(stoneId);
        }

        private void ResetCeil(uint stoneId)
        {
            if(allEnerguspar.ContainsKey(stoneId))
            {
                allEnerguspar[stoneId].Reset();
            }            
        }

        public void AdvanceStone(uint stoneId, Transform trans)
        {
            if(allEnerguspar.ContainsKey(stoneId))
            {
                allEnerguspar[stoneId].AdvanceStoneAni(trans);
            }
        }

        public void ActiveStone(uint stoneId)
        {
            if (allEnerguspar.ContainsKey(stoneId))
            {
                allEnerguspar[stoneId].ActiveAni();
                allEnerguspar[stoneId].Reset();
            }
        }

        public void ShowWaitAnimator()
        {
            List<UI_Energyspar_Main_Left_Ceil> dataList = new List<UI_Energyspar_Main_Left_Ceil>(allEnerguspar.Values);
            for (int i = 0; i < dataList.Count; i++)
            {
                UI_Energyspar_Main_Left_Ceil item = dataList[i];
                if (item.isWaitAni)
                {
                    item.PlayWaitAnimator();
                }
            }
        }

        public void WaiteAnimator()
        {

            List<UI_Energyspar_Main_Left_Ceil> dataList = new List<UI_Energyspar_Main_Left_Ceil>(allEnerguspar.Values);
            for (int i = 0; i < dataList.Count; i++)
            {
                UI_Energyspar_Main_Left_Ceil item = dataList[i];
                if (null != item.energysparData.career_limit)
                {
                    item.WaiteAnimator();
                }
            }
        }

        public override void Show()
        {
            base.Show();
            RefrshDicData();
        }

        public override void Hide()
        {
            base.Hide();
        }


        public void OnSelect(UI_Energyspar_Main_Left_Ceil ceil)
        {
            listener?.OnSelect(ceil.energysparData.id);           
        }

        public void SetSelect(uint id)
        {
            List<uint> keyList = new List<uint>(allEnerguspar.Keys);
            for (int i = 0; i < keyList.Count; i++)
            {
                uint key = keyList[i];
                UI_Energyspar_Main_Left_Ceil value = allEnerguspar[key];
                value.SetSelectState(id == key);
            }
        }

        public void Register(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnSelect(uint id);
        }
    }
}
