using System;
using System.Collections.Generic;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;
namespace Logic
{
    public class UI_LadderPvp_MemMenu:UIComponent
    {

        //0 暂离,1 离开,2 升为队长,3 请离队伍,4 召回,5 申请带队,6 回归队伍

        uint[] CommandArray = { 2002092, 2002094, 2002095, 2002096, 2002098, 2002091, 2002047 };

        public List<int> Commands = new List<int>();

        UI_LadderPvp_MemMenu_Layout m_layout = new UI_LadderPvp_MemMenu_Layout();

        public ulong MemID { get; set; } = 0;
        protected override void Loaded()
        {
            m_layout.Load(transform);

            m_layout.BtnClose.onClick.AddListener(OnClickClose);
        }

        private void OnClickClose()
        {
            Hide();
        }
        public void Open(Vector3 pos)
        {

            this.Show();

            m_layout.TransMenu.position = pos;

            int count = Commands.Count;

            m_layout.mCommandGroup.SetChildSize(count);

            for (int i = 0; i < count; i++)
            {
                var item = m_layout.mCommandGroup.getAt(i);

                if (item == null)
                    continue;

                item.mTexName.text = LanguageHelper.GetTextContent(CommandArray[Commands[i]]);
                item.ID = Commands[i];
                item.OnClickBtn = OnClickBtn;
            }
        }

        private void OnClickBtn(int index)
        {
            switch (index)
            {
                case 0:
                    Sys_Team.Instance.ApplyLeave(Sys_Role.Instance.RoleId);
                    break;
                case 1:
                    Sys_Team.Instance.ApplyExitTeam();
                    break;
                case 2:
                    Sys_Team.Instance.ApplyToCaptaion(MemID);
                    break;
                case 3:
                    Sys_Team.Instance.KickMemTeam(MemID);
                    break;
                case 4:
                    Sys_Team.Instance.ApplyCallBack(MemID);
                    break;
                case 5:
                    Sys_Team.Instance.ApplyLeader(MemID);
                    break;
                case 6:
                    Sys_Team.Instance.ApplyComeBack(MemID);
                    break;

            }

            Hide();
        }
        public int GetCommands(bool isCap, bool bselectown,bool bselectleave)
        {
            Commands.Clear();

            if (isCap)
            {
                if (bselectown)
                {
                    Commands.Add(0);
                    Commands.Add(1);
                }
                else
                {
                    if (!bselectleave)
                        Commands.Add(2);
                    else
                        Commands.Add(4);

                    Commands.Add(3);

                    
                }
            }
            else
            {
                if (bselectown)
                {
                    if (!bselectleave)
                        Commands.Add(5);

                    Commands.Add(bselectleave ? 6 : 0);

                    Commands.Add(1);
                }
                else
                {
                    
                }
            }

            return Commands.Count;
        }
    }
}
