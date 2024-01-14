using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class NearbyNpcSystem : LevelSystemBase
    {
        //TODO 只需要保留当前在内的列表就行 UI直接根据当前在内的来刷新 无需要判断add remove
        private HashSet<ulong> _nearbyNpcSet = new HashSet<ulong>();
        private HashSet<ulong> _tmpNearbyNpcSet = new HashSet<ulong>();
        private List<Npc> _nearbyNpcAdds = new List<Npc>();
        private bool _needUpdateAll = true;

        public void RequestUpdateAll()
        {
            _needUpdateAll = true;
        }

        public override void OnCreate()
        {
            //每半秒一次执行
            intervalFrame = 48;
        }

        //public void Execute()
        public override void OnUpdate()
        {
            bool hasChange = false;

            //本次所有在内的
            _tmpNearbyNpcSet.Clear();
            //本次新增的
            _nearbyNpcAdds.Clear();

            if (_needUpdateAll)
            {
                _needUpdateAll = false;
                _nearbyNpcSet.Clear();
                hasChange = true;
            }
            
            if (GameCenter.mainHero == null
                || ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.Auto
                || GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
            {
                if (_nearbyNpcSet.Count > 0)
                {
                    _nearbyNpcSet.Clear();
                    hasChange = true;
                }
            }
            else
            {
                //TODO 统一一个数据实例用于数据的传递
                List<Npc> npcsList = GameCenter.npcsList;
                Transform mainHeroTransform = GameCenter.mainHero.transform;

                for (int i = npcsList.Count - 1; i >= 0; --i)
                {
                    Npc npc = npcsList[i];

                    if (!npc.VisualComponent.Visiable)
                    {
                        continue;
                    }

                    CSVNpc.Data npcData = npc.cSVNpcData;
                    if (npcData == null || npcData.WhetherScopenTrigger != 1)
                        continue;

                    ulong npcID = npc.uID;

                    //float distance = npc.DistanceTo(mainHeroTransform) * 10000f;                    
                    //if (distance < npcData.TriggerScopen)
                    if (MathUtlilty.SafeDistanceLess(npc.transform, mainHeroTransform, npcData.TriggerScopen * 0.0001f))
                    {
                        _tmpNearbyNpcSet.Add(npcID);
                        if (!_nearbyNpcSet.Contains(npcID))
                        {
                            _nearbyNpcAdds.Add(npc);
                        }
                    }
                }

                //既没有增加有没有数量变化说明内容没变
                //只当内容改变的时候处理
                if (_nearbyNpcAdds.Count > 0 || _tmpNearbyNpcSet.Count != _nearbyNpcSet.Count)
                {
                    //交换集合
                    _nearbyNpcSet.Clear();
                    HashSet<ulong> tmp = _nearbyNpcSet;
                    _nearbyNpcSet = _tmpNearbyNpcSet;
                    _tmpNearbyNpcSet = tmp;

                    hasChange = true;
                }
                else
                {
                    _tmpNearbyNpcSet.Clear();
                }
            }

            if (hasChange)
            {
                Sys_Npc.Instance.eventEmitter.Trigger(Sys_Npc.EEvents.OnNearNpcChange);
            }
        }

        public override void OnDestroy()
        {
            _tmpNearbyNpcSet.Clear();
            _nearbyNpcSet.Clear();
            _nearbyNpcAdds.Clear();            
        }

        public IReadOnlyList<Npc> GetNewNearNpc()
        {
            return _nearbyNpcAdds;
        }

        public bool IsNearNpc(ulong uid)
        {
            return _nearbyNpcSet.Contains(uid);
        }

        public int Count()
        {
            return _nearbyNpcSet.Count;
        }        
    }
}