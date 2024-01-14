using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using UnityEngine;
using Table;
using Google.Protobuf;

namespace Logic
{


    #region WarCommandTaget
    class WarCommandTaget
    {
        public TeamCommandData mWarCommand;

       // List<uint> MyCommandDefault = new List<uint>();
      //  List<uint> EnemyCommandDefault = new List<uint>();


        List<string> mMyCommands = new List<string>();//我方指令
        List<bool> mMyCommandsCustom = new List<bool>();//我方指令是否自定义

        List<string> mEnemyCommands = new List<string>();//敌方指令
        List<bool> mEnemyCommandsCustom = new List<bool>();//敌方指令是否自定义

        public List<string> OwnCommands { get { return mMyCommands; } }
        public List<string> EnemyCommands { get { return mEnemyCommands; } }

 
       // public int OwnCommandDefaultCount { get { return MyCommandDefault.Count; } }
       // public int EnemyCommandDefaultCount { get { return EnemyCommandDefault.Count; } }


        public void ClearWarCommand()
        {
            mMyCommands.Clear();
            mMyCommandsCustom.Clear();

            mEnemyCommands.Clear();
            mEnemyCommandsCustom.Clear();
        }
        //设置战斗指令
        public void SetWarCommand(TeamCommandData data)
        {
            mWarCommand = data;

            UpdataEnmyCommands();
            UpdataMyCommands();
        }


        public void SetWarEnmyCommand(IList<TeamCommand> data)
        {
            mWarCommand.CommandList[0].Commands.Clear();
            mWarCommand.CommandList[0].Commands.AddRange(data);

            UpdataEnmyCommands();
        }

        public void SetWarOwnCommand(IList<TeamCommand> data)
        {
            mWarCommand.CommandList[1].Commands.Clear();
            mWarCommand.CommandList[1].Commands.AddRange(data);

            UpdataMyCommands();
        }

        private void UpdataMyCommands()
        {
            mMyCommands.Clear();
            mMyCommandsCustom.Clear();

            int count = Sys_Team.Instance.MyCommandDefault.Count;

            for (int i = 0; i < count; i++)
            {
                var data = CSVCommand.Instance.GetConfData(Sys_Team.Instance.MyCommandDefault[i]);

                string Name =  LanguageHelper.GetTextContent(data.type_name);

                mMyCommands.Add(Name);
                mMyCommandsCustom.Add(false);
            }

            IList<TeamCommand> commands = mWarCommand.CommandList[1].Commands;
            count = commands.Count;

            for (int i = 0; i < count; i++)
            {
                string Name = commands[i].Command.ToStringUtf8();

                mMyCommands.Add(Name);
                mMyCommandsCustom.Add(true);
            }
        }

        private void UpdataEnmyCommands()
        {
            mEnemyCommands.Clear();
            mEnemyCommandsCustom.Clear();

            int count = Sys_Team.Instance.EnemyCommandDefault.Count;
            for (int i = 0; i < count; i++)
            {
                var data = CSVCommand.Instance.GetConfData(Sys_Team.Instance.EnemyCommandDefault[i]);

                string Name = LanguageHelper.GetTextContent(data.type_name);

                mEnemyCommands.Add(Name);
                mEnemyCommandsCustom.Add(false);
            }

            IList<TeamCommand> commands = mWarCommand.CommandList[0].Commands;
            count = commands.Count;

            for (int i = 0; i < count; i++)
            {
                string Name = commands[i].Command.ToStringUtf8();

                mEnemyCommands.Add(Name);
                mEnemyCommandsCustom.Add(true);
            }
        }



        /// <summary>
        /// 是否有自定义的指令
        /// </summary>
        /// <returns></returns>
        public bool isHadCustomCommand()
        {
            bool bValue0 = isHadTypeCustomCommand(0);

            if (bValue0)
                return true;

            return isHadTypeCustomCommand(1);
        }

        private bool isHadTypeCustomCommand(int index) // 0 敌方  1 友方
        {
            if (index < 0 || index > 1)
                return false;

            IList<TeamCommand> commands = mWarCommand.CommandList[index].Commands;
            int count = commands.Count;
    
            return count > 0;
        }


    }

    #endregion

    partial class Sys_Team : SystemModuleBase<Sys_Team>
    {
        public class CommandItem
        {
            public string Name;
            public int Type; //0 自己 1，敌方
            public int Index = -1;
            public int CommandIndex;
        }

        private List<CommandItem> m_FastCommand = new List<CommandItem>(5) { new CommandItem(), new CommandItem() ,
            new CommandItem() , new CommandItem() , new CommandItem() };

        public List<CommandItem> FastCommand { get { return m_FastCommand; } }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="commandindex"></param>
        /// <param name="index"></param>
        public void SetFastCommand(int type, int fastIndex,int commandindex)
        {

            int findIndex = FastCommand.FindIndex(o => o.Index >= 0 && o.Type == type && o.CommandIndex == commandindex);

            if (findIndex >= 0)
            {
                RemoveFastCommand(findIndex);
            }

            int defaultcount = type == 0 ? PlayerOwnCommandDefaultCount : PlayerEnemyCommandDefaultCount;

            bool iscustom = commandindex >= defaultcount;

            if (iscustom)
                commandindex = commandindex - defaultcount;

            Send_Message_EditQuickCommandReq(true, (uint)fastIndex + 1, iscustom, type == 0, (uint)commandindex);
        }

        public void RemoveFastCommand(int index)
        {
            var value = FastCommand[index];

            if (value.Index < 0)
                return;

            int defaultcount = value.Type == 0 ? PlayerOwnCommandDefaultCount : PlayerEnemyCommandDefaultCount;

            bool iscustom = value.CommandIndex >= defaultcount;

            int commandIndex = value.CommandIndex;

            if (iscustom)
                commandIndex -= defaultcount;

            Send_Message_EditQuickCommandReq(false, (uint)index + 1, iscustom, value.Type == 0, (uint)commandIndex);

            value.Index = -1;
            value.Name = string.Empty;
            value.CommandIndex = 0;

            eventEmitter.Trigger(EEvents.EditQuickCommandRes);
        }


        private void RestFastCommand()
        {
            int count = FastCommand.Count;

            for (int i = 0; i < count; i++)
            {
                FastCommand[i].Index = -1;
                FastCommand[i].Name = string.Empty;
            }
        }
        private void ParseFastCommand(IList<TeamCommand> commands, int type, bool isDefaults)
        {
            int count = commands.Count;

            int defaultcount = type == 0 ? PlayerOwnCommandDefaultCount : PlayerEnemyCommandDefaultCount;

            for (int i = 0; i < count; i++)
            {
                var value = commands[i];
                if (value.QuickTag > 0)
                {
                    int index = (int)value.QuickTag - 1;

                    int fastcount = m_FastCommand.Count;

                    CommandItem item = m_FastCommand[index];
                    if (item == null)
                    {
                        item = new CommandItem();
                        m_FastCommand[index] = item;
                    }

                    item.Index = index;
                    item.Type = type;

                    int cindex = isDefaults ? i : (defaultcount + i);
                    
                    item.Name =  type == 0 ?  PlayerCommandTaget.OwnCommands[cindex] : PlayerCommandTaget.EnemyCommands[cindex];

                    item.CommandIndex = cindex;
                }
            }
        }
        private void UpdateFastCommand()
        {
            RestFastCommand();

            ParseFastCommand(PlayerCommandTaget.mWarCommand.CommandList[0].Commands, 1, false);
            ParseFastCommand(PlayerCommandTaget.mWarCommand.CommandList[0].Defaults, 1, true);

            ParseFastCommand(PlayerCommandTaget.mWarCommand.CommandList[1].Commands, 0, false);
            ParseFastCommand(PlayerCommandTaget.mWarCommand.CommandList[1].Defaults, 0, true);

        }
    }
    #region // 战斗指令
    partial class Sys_Team : SystemModuleBase<Sys_Team>
    {

        public List<uint> MyCommandDefault = new List<uint>();
        public List<uint> EnemyCommandDefault = new List<uint>();

        private WarCommandTaget PlayerCommandTaget = new WarCommandTaget();

        public List<string> PlayerOwnCommands { get { return PlayerCommandTaget.OwnCommands; } }

        public int PlayerOwnCommandDefaultCount { get { return MyCommandDefault.Count; } }

        public List<string> PlayerEnemyCommands { get { return PlayerCommandTaget.EnemyCommands; } }

        public int PlayerEnemyCommandDefaultCount { get { return EnemyCommandDefault.Count; } }



        private void ReadWarCommandConfig()
        {

            ReadWarDefaultCommandConfig();


        }
        private  void ClearPlayerCommandTag()
        {
            
            PlayerCommandTaget.ClearWarCommand();
        }


        public void ReadWarDefaultCommandConfig()
        {
            var commandConfig = CSVCommand.Instance.GetAll();

            foreach (var kvp in commandConfig)
            {
                if (kvp.type_id == 0)
                {
                    MyCommandDefault.Add(kvp.id);
                }
                else if (kvp.type_id == 1)
                {
                    EnemyCommandDefault.Add(kvp.id);
                }
            }
        }

        #region msg

        //战斗指令
        protected void Notify_Message_CommandDataNtf(NetMsg msg)
        {
            CmdTeamCommandDataNtf info = NetMsgUtil.Deserialize<CmdTeamCommandDataNtf>(CmdTeamCommandDataNtf.Parser, msg);

            SetPlayerWarCommand(info.Data);

            eventEmitter.Trigger(EEvents.WarCommandNtf);

        }

        //修改战斗指令反馈
        protected void Notify_Message_EditCommandRes(NetMsg msg)
        {
            CmdTeamEditCommandRes info = NetMsgUtil.Deserialize<CmdTeamEditCommandRes>(CmdTeamEditCommandRes.Parser, msg);

            if (info.Self)
                SetPlayerWarOwnCommand(info.Commands);
            else
                SetPlayerWarEnmyCommand(info.Commands);

            eventEmitter.Trigger<int>(EEvents.WarCommandRes, (info.Self ? 1 : 0));
        }

        /// <summary>
        /// 修改快捷指令反馈
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_EditQuickCommandRes(NetMsg msg)
        {
            CmdTeamEditQuickCommandRes info = NetMsgUtil.Deserialize<CmdTeamEditQuickCommandRes>(CmdTeamEditQuickCommandRes.Parser, msg);

            SetPlayerWarCommand(info.Data);

            eventEmitter.Trigger(EEvents.EditQuickCommandRes);
        }

        /// <summary>
        /// 修改战斗指令
        /// </summary>
        protected void Send_Message_EditCommandReq(bool isself, uint index, string strName)
        {

            CmdTeamEditCommandReq info = new CmdTeamEditCommandReq()
            {
                Self = isself,
                Index = index,
                Command = string.IsNullOrEmpty(strName) ? null : ByteString.CopyFromUtf8(strName)
            };
            NetClient.Instance.SendMessage((ushort)CmdTeam.EditCommandReq, info);
        }

        /// <summary>
        /// 重置战斗指令
        /// </summary>

        protected void Send_Message_ResetAllCommandReq()
        {

            CmdTeamResetAllCommandReq info = new CmdTeamResetAllCommandReq()
            {
            };
            NetClient.Instance.SendMessage((ushort)CmdTeam.ResetAllCommandReq, info);
        }

        /// <summary>
        /// 编辑快捷战斗指令
        /// </summary>
        protected void Send_Message_EditQuickCommandReq(bool op,uint index, bool isCustom,bool isSelf,uint commandIndex )
        {

            CmdTeamEditQuickCommandReq info = new CmdTeamEditQuickCommandReq();

            info.Tag = index;
            info.Set = op;
            info.Custom = isCustom;
            info.Self = isSelf;
            info.Index = commandIndex;

            NetClient.Instance.SendMessage((ushort)CmdTeam.EditQuickCommandReq, info);
        }


        #endregion



        #region //战斗指令

        /// <summary>
        /// 添加战斗指令
        /// </summary>
        /// <param name="isOwn"> false 敌方，true 我方</param>
        /// <param name="index">指令下标，添加时为一个新的位置</param>
        /// <param name="strName"></param>
        public void ApplyWarEditAdd(bool isOwn, uint index, string strName)
        {
            Send_Message_EditCommandReq(isOwn, index, strName);
        }

        /// <summary>
        /// 删除战斗指令
        /// </summary>
        /// <param name="isOwn">false 敌方，true 我方</param>
        /// <param name="index">指令下标</param>
        public void ApplyWarEditRemove(bool isOwn, uint index)
        {
            Send_Message_EditCommandReq(isOwn, index, string.Empty);
        }

        /// <summary>
        /// 修改战斗指令
        /// </summary>
        /// <param name="isOwn">false 敌方，true 我方</param>
        /// <param name="index">指令下标</param>
        public void ApplyWarEditChange(bool isOwn, uint index, string strName)
        {
            Send_Message_EditCommandReq(isOwn, index, strName);
        }

        /// <summary>
        /// 重置战斗指令
        /// </summary>
        public void ApplyRestAllWarCommand()
        {
            Send_Message_ResetAllCommandReq();
  
        }
        #endregion



        private void SetPlayerWarCommand(TeamCommandData data)
        {
            PlayerCommandTaget.SetWarCommand(data);

            UpdateFastCommand();

        }


        public void SetPlayerWarEnmyCommand(IList<TeamCommand> data)
        {
            PlayerCommandTaget.SetWarEnmyCommand(data);
        }

        public void SetPlayerWarOwnCommand(IList<TeamCommand> data)
        {
            PlayerCommandTaget.SetWarOwnCommand(data);
        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="value1">指令小标</param>
        /// <param name="value2">0 敌方，1 友方</param>
        /// <returns></returns>
        public string getPlayerTagString(uint value1, uint value2)
        {
            var list = value2 == 0 ? PlayerEnemyCommands : PlayerOwnCommands;

            if (list.Count <= (int)value1)
                return string.Empty;

            return list[(int)value1];
        }


        public  bool isHadCustomCommand()
        {
            return PlayerCommandTaget.isHadCustomCommand();
        }
    }

    #endregion

}
