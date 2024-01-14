using UnityEngine;

namespace Logic
{
    public class NpcActiveListenerSystem : LevelSystemBase
    {
        float lastTime = 0;
        float cd = 1f;

        public override void OnUpdate()
        {
            if (Time.unscaledTime < lastTime)
                return;

            lastTime = Time.unscaledTime + cd;

            if (GameCenter.mainHero == null)
                return;

            for (int i = 0, len = GameCenter.npcsList.Count; i < len; ++i)
            {
                Excute(GameCenter.npcsList[i].ActiveListenerComponent);
            }
        }

        private void Excute(NpcActiveListenerComponent npcActiveListenerComponent)
        {
            lastTime = Time.unscaledTime;

            if (npcActiveListenerComponent.Npc.cSVNpcData.ActivationRecord == 1)
            {
                if (npcActiveListenerComponent.Npc.VisualComponent.Visiable)
                {
                    npcActiveListenerComponent.Npc.VisualComponent.Checking();
                }
                if (npcActiveListenerComponent.Npc != null && !Sys_Npc.Instance.IsActivatedNpc(npcActiveListenerComponent.Npc.cSVNpcData.id) && npcActiveListenerComponent.Npc.VisualComponent != null && npcActiveListenerComponent.Npc.VisualComponent.Visiable)
                {
                    if (!npcActiveListenerComponent.Npc.Contains(GameCenter.mainHero.transform))
                    {
                        return;
                    }

                    Sys_Npc.Instance.ReqNpcActivateNpc(npcActiveListenerComponent.Npc.uID);
                }
            }
            else
            {
                if (CanActiveNpc(npcActiveListenerComponent))
                {
                    if (!Sys_FunctionOpen.Instance.IsOpen(npcActiveListenerComponent.FunctionId, false))
                        return;

                    if (npcActiveListenerComponent.Npc != null)
                    {
                        Sys_Npc.Instance.ReqNpcActivateNpc(npcActiveListenerComponent.Npc.uID);
                    }
                }
            }

            //if (npcActiveListenerComponent.Npc != null && npcActiveListenerComponent.Npc.cSVNpcData.ActivationRecord == 1 && !Sys_Npc.Instance.IsActivatedNpc(npcActiveListenerComponent.Npc.cSVNpcData.id) && npcActiveListenerComponent.Npc.VisualComponent != null && npcActiveListenerComponent.Npc.VisualComponent.Visiable)
            //{
            //    if (!npcActiveListenerComponent.Npc.Contains(GameCenter.mainHero.transform))
            //    {
            //        return;
            //    }

            //    Sys_Npc.Instance.ReqNpcActivateNpc(npcActiveListenerComponent.Npc.uID);
            //}
            //else
            //{
            //    if (CanActiveNpc(npcActiveListenerComponent))
            //    {
            //        if (!Sys_FunctionOpen.Instance.IsOpen(npcActiveListenerComponent.FunctionId, false))
            //            return;

            //        if (npcActiveListenerComponent.Npc != null)
            //        {
            //            Sys_Npc.Instance.ReqNpcActivateNpc(npcActiveListenerComponent.Npc.uID);
            //        }
            //        //Debug.LogErrorFormat("active==={0}", _npcId);
            //    }
            //}
        }

        bool CanActiveNpc(NpcActiveListenerComponent npcActiveListenerComponent)
        {
            if (!npcActiveListenerComponent.NoteNpc)
                return false;

            if (npcActiveListenerComponent.ResPointData == null)
                return false;

            if (npcActiveListenerComponent.ResPointData.markState)
                return false;

            if (GameCenter.mainHero == null)
                return false;

            if (npcActiveListenerComponent.Npc.Contains(GameCenter.mainHero.transform))
            {
                return true;
            }

            return false;
        }
    }
}