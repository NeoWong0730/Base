using System.Collections;
using System.Collections.Generic;
using Table;
using Net;
using Packet;
using Lib.Core;
using UnityEngine;
using Logic.Core;
using static Packet.CmdCookPrepareConfirmNtf.Types;
using static Packet.CmdCookCookReq.Types;
using Google.Protobuf.Collections;
using System;

namespace Logic
{
    public partial class Sys_Cooking : SystemModuleBase<Sys_Cooking>, ISystemModuleUpdate
    {
        public Dictionary<uint, AttrCooking> usingCookings = new Dictionary<uint, AttrCooking>();
        private ChangeScaleCooking m_ScaleUsingCooking;

        public uint GetScaleFoodId()
        {
            if (m_ScaleUsingCooking != null)
            {
                return m_ScaleUsingCooking.ItemId;
            }
            return 0;
        }

        public class AttrCooking
        {
            public uint ItemId
            {
                get;
                private set;
            }
            private uint m_EndTime;

            public bool b_Valid
            {
                get
                {
                    return Sys_Time.Instance.GetServerTime() < m_EndTime;
                }
            }

            public void SetData(uint itemId, uint endTime)
            {
                ItemId = itemId;
                m_EndTime = endTime;
            }

            public string GetTime()
            {
                uint time = Sys_Time.Instance.GetServerTime();
                if (time < m_EndTime)
                {
                    uint remainTime = (uint)(m_EndTime - time);
                    return LanguageHelper.TimeToString(remainTime, LanguageHelper.TimeFormat.Type_6);
                }
                else
                {
                    CmdCookUseFoodEndReq cmdCookUseFoodEndReq = new CmdCookUseFoodEndReq();
                    cmdCookUseFoodEndReq.FoodId = ItemId;
                    NetClient.Instance.SendMessage((ushort)CmdCook.UseFoodEndReq, cmdCookUseFoodEndReq);
                    return LanguageHelper.GetTextContent(1003084);
                }
            }
        }

        public class ChangeScaleCooking
        {
            public uint ItemId
            {
                get;
                private set;
            }

            public CSVItem.Data cSVItemData
            {
                get;
                private set;
            }

            private uint m_EndTime;
            private bool b_NeedUpdate;

            public void SetData(uint itemId, uint endTime)
            {
                ItemId = itemId;
                m_EndTime = endTime;
                cSVItemData = CSVItem.Instance.GetConfData(ItemId);
                b_NeedUpdate = true;

                if (GameCenter.mainHero != null)
                {
                    uint rat_X = cSVItemData.fun_value[0];
                    uint rat_Y = cSVItemData.fun_value[1];
                    uint rat_Z = cSVItemData.fun_value[2];
                    GameCenter.mainHero.ChangeModelScale(rat_X / 100f, rat_Y / 100f, rat_Z / 100f);

                    if (rat_Y >= 100)
                    {
                        Sys_HUD.Instance.eventEmitter.Trigger<ulong, uint>(Sys_HUD.EEvents.OnScaleUp, GameCenter.mainHero.UID, rat_Y);
                    }
                    else if (rat_Y < 100)
                    {
                        Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnResetScale, GameCenter.mainHero.UID);
                    }
                }
            }

            public void Update()
            {
                if (!b_NeedUpdate)
                {
                    return;
                }
                uint time = Sys_Time.Instance.GetServerTime();
                if (time >= m_EndTime)
                {
                    b_NeedUpdate = false;
                    if (GameCenter.mainHero != null)
                    {
                        GameCenter.mainHero.ResetModelScale();
                        Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnResetScale, GameCenter.mainHero.UID);
                        CmdCookUseFoodEndReq cmdCookUseFoodEndReq = new CmdCookUseFoodEndReq();
                        cmdCookUseFoodEndReq.FoodId = ItemId;
                        NetClient.Instance.SendMessage((ushort)CmdCook.UseFoodEndReq, cmdCookUseFoodEndReq);
                    }
                }
            }
        }
    }

}


