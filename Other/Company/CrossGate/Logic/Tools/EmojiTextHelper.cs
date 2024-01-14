using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Framework;
using Table;
using Lib.Core;

namespace Logic
{
    public enum ERichType
    {
        /// <summary>
        /// 表情 [id]
        /// </summary>
        Emoji = 0,      
        /// <summary>
        /// 名字 [@id]
        /// </summary>
        Name = 1,
        /// <summary>
        /// 道具 [#id]
        /// </summary>
        Item = 2,
        /// <summary>
        /// 任务 [!id]
        /// </summary>
        Task = 3,
        /// <summary>
        /// 加好友 [^id]
        /// </summary>
        AddFriend = 4,
        /// <summary>
        /// 组队 [$id]
        /// </summary>
        JoinTeam = 5,
        /// <summary>
        /// 商城道具 [$id]
        /// </summary>
        MallItem = 6,
        /// <summary>
        /// 交易行道具 [*id]
        /// </summary>
        TradeItem = 7,
        /// <summary>
        /// 称号 [%id]
        /// </summary>
        TitleItem = 8,  
        /// <summary>
        /// 特殊词 [&id]
        /// </summary>
        //SpecialWords = 9,
        /// <summary>
        /// 拍卖行
        /// </summary>
        Auction = 10,
        /// <summary>
        /// 家族委托
        /// </summary>
        FamilyConsign=11,
        /// <summary>
        /// 家族红包
        /// </summary>
        FamilyRedPacket=12,
        /// <summary>
        /// 战斗录像
        /// </summary>
        FightRecode = 13,
        /// <summary>
        /// 成就
        /// </summary>
        Achievement = 14,
        /// <summary>
        /// 法兰商队
        /// </summary>
        MerchantFleet = 15,
    }

    public static class EmojiTextHelper
    {
        //public static readonly Regex s_RichRegex = new Regex("/<(.*)>.</\1>|<(.) />");
        //public static readonly Regex s_EmojiRegex = new Regex("\\[[@|#|%|$].*?\\]");
        public static readonly Regex s_EmojiRegex = new Regex("\\[(.*?)\\]");

        //private static readonly StringBuilder tempStringBuilder = new StringBuilder();

        private static readonly string gEmojiStart = "<quad emoji=";
        private static readonly string gEmojiMid = " size=";
        private static readonly string gEmojiEnd = " />";

        public static readonly string gColorStart = "<color=#";
        public static readonly string gColorEnd = "]</color>";

        //前面空格不能少
        public static readonly string gNameTag = " a=1_";
        public static readonly string gItemTag = " a=2_";
        public static readonly string gTaskTag = " a=3_";
        public static readonly string gAddFriendTag = " a=4_";
        public static readonly string gJoinTeamTag = " a=5_";
        //6 摆摊
        public static readonly string gTradeItemTag = " a=7_";
        public static readonly string gTitleItemTag = " a=8_";
        public static readonly string gPetItemTag = " a=9_";
        //10 拍卖行

        //11家族委托
        public static readonly string gFamilyConsign = " a=11_";

        //12家族红包
        public static readonly string gFamilyRedPacket = " a=12_";

        //13战斗录像
        public static readonly string gVideo = " a=13_";

        //14成就
        public static readonly string gAchievement = " a=14_";
        //15法兰商队
        public static readonly string gMerchant = " a=15_";


        public static string ParseChatRichText(EmojiAsset emojiAsset, Packet.ChatExtMsg chatExtMsg, string srcText, ulong paramID = 0, uint size = 48)
        {
            int offsetIndex = 0;
            StringBuilder tempStringBuilder = StringBuilderPool.GetTemporary();

            //把表情标签全部匹配出来
             MatchCollection matchs = s_EmojiRegex.Matches(srcText);
            int matchsCount = matchs.Count;

            //将特殊文本纪录到容器
            for (int i = 0; i < matchsCount; ++i)
            {
                Match match = matchs[i];
                char flag = srcText[match.Index + 1];
                //将从上一个文本段的结束到下个特殊文本段之间的内容加入
                tempStringBuilder.Append(srcText, offsetIndex, match.Index - offsetIndex);

                switch (flag)
                {
                    case '@':
                        {
                            //玩家名称[@Name#ID]
                            _ParsePlayerName(tempStringBuilder, srcText, match);
                        }
                        break;

                    case '^':
                        {
                            //[^加为好友]
                            _ParseAddFriend(tempStringBuilder, srcText, match, paramID);
                        }
                        break;

                    case '#':
                        {
                            if (chatExtMsg != null && chatExtMsg.Item != null)
                            {
                                //物品 [#dataIndex]
                                _ParseItem(tempStringBuilder, srcText, match, chatExtMsg.Item);
                            }
                            else
                            {
                                //TODO 当物品数据为空时 当成普通文本处理, 或者直接丢弃
                                tempStringBuilder.Append(srcText, match.Index, match.Length);
                            }
                        }
                        break;

                    case '!':
                        {
                            //任务 [!TaskID]
                            _ParseTask(tempStringBuilder, srcText, match);
                        }
                        break;

                    case '$':
                        {
                            //申请入队 [$TeamID]
                            _ParseJoinTeam(tempStringBuilder, srcText, match);
                        }
                        break;

                    case '%':
                        {
                            //称号 [%TeamID]
                            _ParseTitle(tempStringBuilder, srcText, match);
                        }
                        break; 

                    case '~':
                        {
                            if (chatExtMsg != null && chatExtMsg.AchData != null)
                            {
                                //成就 [~dataIndex]
                                _ParseAchievement(tempStringBuilder, srcText, match, chatExtMsg.AchData);
                            }
                        }
                        break;

                    default:
                        {
                            string sKey = srcText.Substring(match.Index + 1, match.Length - 2);
                            if (uint.TryParse(sKey, out uint key) && key != 0 && emojiAsset.ContainsKey(key))
                            {
                                //表情
                                _ParseEmoji(tempStringBuilder, srcText, match, key, size);
                            }
                            else
                            {
                                //是普通文本直接加入
                                tempStringBuilder.Append(srcText, match.Index, match.Length);
                            }
                        }
                        break;
                }

                //将偏移设置为当前文本段最后
                offsetIndex = match.Index + match.Length;
            }

            if (offsetIndex < srcText.Length)
            {
                tempStringBuilder.Append(srcText, offsetIndex, srcText.Length - offsetIndex);
            }

            string rlt = StringBuilderPool.ReleaseTemporaryAndToString(tempStringBuilder);
            return rlt;
        }

        public static string AppendPlayerName(string srcText, string name, ulong roleID)
        {
            StringBuilder tempStringBuilder = StringBuilderPool.GetTemporary();
            tempStringBuilder.Append(gColorStart);
            tempStringBuilder.Append(Constants.gChatColor_Name);
            tempStringBuilder.Append(gNameTag);            
            tempStringBuilder.Append(roleID);                     //RoleID为参数
            tempStringBuilder.Append('>');
            tempStringBuilder.Append('[');
            tempStringBuilder.Append(name);
            tempStringBuilder.Append(gColorEnd);
            tempStringBuilder.Append(srcText);
            string rlt = StringBuilderPool.ReleaseTemporaryAndToString(tempStringBuilder);
            return rlt;
        }

        private static void _ParsePlayerName(StringBuilder tempStringBuilder, string srcText, Match match)
        {
            string s = srcText.Substring(match.Index + 2, match.Length - 3);
            string[] ss = s.Split('#');

            string name = string.Empty;
            ulong roleID = 0;

            if (ss.Length > 1)
            {
                name = ss[0];
                ulong.TryParse(ss[1], out roleID);
            }
            else if (ss.Length > 0)
            {
                name = ss[0];
            }

            tempStringBuilder.Append(gColorStart);
            tempStringBuilder.Append(Constants.gChatColor_Name);            
            tempStringBuilder.Append(gNameTag);
            tempStringBuilder.Append(roleID);
            tempStringBuilder.Append('>');
            tempStringBuilder.Append('[');
            tempStringBuilder.Append(name);
            tempStringBuilder.Append(gColorEnd);
        }

        private static void _ParseAddFriend(StringBuilder tempStringBuilder, string srcText, Match match, ulong paramID)
        {
            tempStringBuilder.Append(gColorStart);
            tempStringBuilder.Append(Constants.gChatColorAddFriend);
            tempStringBuilder.Append(gAddFriendTag);
            tempStringBuilder.Append(paramID.ToString());
            tempStringBuilder.Append('>');
            tempStringBuilder.Append('[');
            tempStringBuilder.Append(srcText, match.Index + 2, match.Length - 3);
            tempStringBuilder.Append(gColorEnd);
        }

        private static void _ParseItem(StringBuilder tempStringBuilder, string srcText, Match match, IList<Packet.ItemCommonData> datas)
        {
            string sIndex = srcText.Substring(match.Index + 2, match.Length - 3);
            int dataIndex = 0;
            if (int.TryParse(sIndex, out dataIndex))
            {
                Packet.ItemCommonData item = datas[dataIndex];

                CSVItem.Data itemData = CSVItem.Instance.GetConfData(item.Id);
                string itemName = LanguageHelper.GetTextContent(itemData.name_id);

                uint colorIndex = item.Quality - 1u;
                if (colorIndex >= Constants.gChatColors_Item.Length || colorIndex < 0)
                {
                    colorIndex = 0;
                }
                string color = Constants.gChatColors_Item[colorIndex];

                tempStringBuilder.Append(gColorStart);
                tempStringBuilder.Append(color);
                tempStringBuilder.Append(gItemTag);
                tempStringBuilder.Append(dataIndex.ToString());  //数据Index
                tempStringBuilder.Append('>');
                tempStringBuilder.Append('[');
                tempStringBuilder.Append(itemName);              //itemName
                tempStringBuilder.Append(gColorEnd);
            }
        }

        private static void _ParseTask(StringBuilder tempStringBuilder, string srcText, Match match)
        {
            string sIndex = srcText.Substring(match.Index + 2, match.Length - 3);
            uint taskID = 0;
            if (uint.TryParse(sIndex, out taskID))
            {
                CSVTask.Data taskData = CSVTask.Instance.GetConfData(taskID);
                string taskName = LanguageHelper.GetTaskTextContent(taskData.taskName);

                //if (taskData.taskCategory == (uint)ETaskCategory.Trunk)
                //{
                //    //"主线"
                //    tempStringBuilder.Append(LanguageHelper.GetTextContent(2007910));
                //}
                //else
                //{
                //    //"支线"
                //    tempStringBuilder.Append(LanguageHelper.GetTextContent(2007911));
                //}

                //RichTextData richTextData = new RichTextData((int)ERichType.Task, (ulong)dataIndex, mTempTextBuilder.Length, taskName.Length + 2, new Color32(0x86, 0x1c, 0x36, 255));
                //richTextData.eRichType = (int)ERichType.Task;
                //richTextData.index = mTempTextBuilder.Length;
                //richTextData.count = taskName.Length + 2;         //减去#的显示 和后面+ID的显示 保留 [名字] 的个数
                //richTextData.parameter = (ulong)dataIndex;        //(ulong)dataIndex
                //richTextData.color = new Color32(0x86, 0x1c, 0x36, 255);

                tempStringBuilder.Append(gColorStart);
                tempStringBuilder.Append(Constants.gChatColorTask);
                tempStringBuilder.Append(gTaskTag);
                tempStringBuilder.Append(taskID.ToString());  //data
                tempStringBuilder.Append('>');
                tempStringBuilder.Append('[');
                tempStringBuilder.Append(taskName);              //displayContent
                tempStringBuilder.Append(gColorEnd);
            }
        }

        private static void _ParseJoinTeam(StringBuilder tempStringBuilder, string srcText, Match match)
        {
            string strTeamID = srcText.Substring(match.Index + 2, match.Length - 3);
            ulong teamID = 0;
            if (ulong.TryParse(strTeamID, out teamID))
            {
                string joinTeam = LanguageHelper.GetTextContent(2002151);

                tempStringBuilder.Append(gColorStart);
                tempStringBuilder.Append(Constants.gChatColorJoinTeam);
                tempStringBuilder.Append(gJoinTeamTag);
                tempStringBuilder.Append(teamID.ToString()); //data
                tempStringBuilder.Append('>');
                tempStringBuilder.Append('[');
                tempStringBuilder.Append(joinTeam);          //displayContent
                tempStringBuilder.Append(gColorEnd);
            }
        }

        private static void _ParseEmoji(StringBuilder tempStringBuilder, string srcText, Match match, uint id, uint size)
        {
            tempStringBuilder.Append(gEmojiStart);
            tempStringBuilder.Append(id.ToString());
            tempStringBuilder.Append(gEmojiMid);
            tempStringBuilder.Append(size.ToString());
            tempStringBuilder.Append(gEmojiEnd);
        }

        private static void _ParseTitle(StringBuilder tempStringBuilder, string srcText, Match match)
        {
            string sIndex = srcText.Substring(match.Index + 2, match.Length - 3);
            uint titleID = 0;
            if (uint.TryParse(sIndex, out titleID))
            {
                //Title title = Sys_Title.Instance.GetTitleData(titleID);
                //string titleName = LanguageHelper.GetTextContent(title.cSVTitleData.titleLan);

                string titleName = Sys_Title.Instance.GetTitleConfigName(titleID);
                string color = Constants.gChatColorTitle;

                tempStringBuilder.Append(gColorStart);
                tempStringBuilder.Append(color);
                tempStringBuilder.Append(gTitleItemTag);
                tempStringBuilder.Append(titleID.ToString());  //itemID
                tempStringBuilder.Append('>');
                tempStringBuilder.Append('[');
                tempStringBuilder.Append(titleName);              //itemName
                tempStringBuilder.Append(gColorEnd);
            }
        }
        private static void _ParseAchievement(StringBuilder tempStringBuilder, string srcText, Match match, IList<Packet.AchievementData> datas)
        {
            string sIndex = srcText.Substring(match.Index + 2, match.Length - 3);
            if (int.TryParse(sIndex, out int dataIndex))
            {
                Packet.AchievementData achData = datas[dataIndex];
                AchievementDataCell dataCell = Sys_Achievement.Instance.GetAchievementByTid(achData.Id);
                string color = Constants.gChatColorAchievement;

                tempStringBuilder.Append(gColorStart);
                tempStringBuilder.Append(color);
                tempStringBuilder.Append(gAchievement);
                tempStringBuilder.Append(dataIndex.ToString());
                tempStringBuilder.Append('>');
                tempStringBuilder.Append('[');
                tempStringBuilder.Append(LanguageHelper.GetAchievementContent(dataCell.csvAchievementData.Achievement_Title));
                tempStringBuilder.Append(gColorEnd);
            }
        }
    }
}