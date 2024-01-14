using Logic;
using Packet;
using Table;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;

public static class ErrorCodeHelper
{
    public static void PushErrorCode(uint mask, string normalContent, string chatContent, uint messageId = 0,CmdSocialSysTipNtf response = null, bool isSelf = false)
    {
        if (!string.IsNullOrWhiteSpace(normalContent))
        {
            if (((uint)SysTipPos.ErrorCode & mask) != 0)
            {
                Sys_Hint.Instance.PushContent_Normal(normalContent);
            }

            if (((uint)SysTipPos.Center & mask) != 0)
            {
                Sys_Hint.Instance.PushContent_Normal(normalContent);
            }

            if (((uint)SysTipPos.Notice & mask) != 0)
            {
                Sys_Hint.Instance.PushContent_Normal(normalContent);
            }

            if (((uint)SysTipPos.Marquee & mask) != 0)
            {
                Sys_Hint.Instance.PushContent_Marquee(normalContent, messageId, isSelf, response);
            }

            if (((uint)SysTipPos.Bottom & mask) != 0)
            {
                Sys_Hint.Instance.PushContent_CommonInfo(normalContent);
            }
        }

        if (!string.IsNullOrWhiteSpace(chatContent))
        {
            if (((uint)SysTipPos.Sys & mask) != 0)
            {
                if (((uint)SysTipPos.Marquee & mask) != 0 || ((uint)SysTipPos.Bottom & mask) != 0)
                    Sys_Chat.Instance.PushMessage(ChatType.System, Sys_Chat.Instance.gSystemChatBaseInfo, chatContent, Sys_Chat.EMessageProcess.None);
                else
                    Sys_Chat.Instance.PushMessage(ChatType.Person, Sys_Chat.Instance.gSystemChatBaseInfo, chatContent, Sys_Chat.EMessageProcess.None);
            }

            if (((uint)SysTipPos.World & mask) != 0)
            {
                Sys_Chat.Instance.PushMessage(ChatType.World, Sys_Chat.Instance.gSystemChatBaseInfo, chatContent, Sys_Chat.EMessageProcess.None);
            }

            if (((uint)SysTipPos.Local & mask) != 0)
            {
                Sys_Chat.Instance.PushMessage(ChatType.Local, Sys_Chat.Instance.gSystemChatBaseInfo, chatContent, Sys_Chat.EMessageProcess.None);
            }

            if (((uint)SysTipPos.Team & mask) != 0)
            {
                Sys_Chat.Instance.PushMessage(ChatType.Team, Sys_Chat.Instance.gSystemChatBaseInfo, chatContent, Sys_Chat.EMessageProcess.None);
            }

            if (((uint)SysTipPos.Guild & mask) != 0)
            {
                Sys_Chat.Instance.PushMessage(ChatType.Guild, Sys_Chat.Instance.gSystemChatBaseInfo, chatContent, Sys_Chat.EMessageProcess.None);
            }
        }
    }

    public static void PushErrorCode(CmdSocialSysTipNtf response)
    {
        CSVErrorCode.Data csv = CSVErrorCode.Instance.GetConfData(response.MsgId);
        if (csv == null)
        {
            return;
        }

        uint mask = csv.pos;
        if (mask == 0)
        {
            Debug.LogErrorFormat("语言表id为{0}的pos没有正确填写,所以不能飘字", csv.id);
        }

        bool hasNormalContent = ((uint)(SysTipPos.ErrorCode | SysTipPos.Center | SysTipPos.Notice | SysTipPos.Marquee | SysTipPos.Bottom) & mask) != 0;
        bool hasChatContent = ((uint)(SysTipPos.Sys | SysTipPos.World | SysTipPos.Local | SysTipPos.Team | SysTipPos.Guild) & mask) != 0;

        string normalContent = hasNormalContent ? _ParserSysTip(response.Fields, csv, false) : null;
        string chatContent = hasChatContent ? _ParserSysTip(response.Fields, csv, true) : null;

        PushErrorCode(mask, normalContent, chatContent, response.MsgId,response);
    }

    public static string _ParserSysTip(pbc::RepeatedField<global::Packet.SysTipField> fields, CSVErrorCode.Data csv, bool isChat)
    {
        int count = fields.Count;
        string[] contents = new string[count];

        for (int i = 0; i < count; ++i)
        {
            global::Packet.SysTipField field = fields[i];

            SysTipFieldType tipFieldType = (SysTipFieldType)field.Type;
            switch (tipFieldType)
            {
                case SysTipFieldType.SysTipFieldNone:
                    break;
                case SysTipFieldType.SysTipFieldNumber:
                    contents[i] = field.Number.ToString();
                    break;
                case SysTipFieldType.SysTipFieldString:
                    contents[i] = field.Context.ToStringUtf8();
                    break;
                case SysTipFieldType.SysTipFieldMsgId:
                    contents[i] = LanguageHelper.GetTextContent(field.FillMsgId);
                    break;
                case SysTipFieldType.SysTipFieldTime:
                    contents[i] = Sys_Time.ConvertToLocalTime(field.Time).ToLongDateString();
                    break;
                case SysTipFieldType.SysTipFieldTimeStamp:
                    break;
                case SysTipFieldType.SysTipFieldRoleName:
                    // 聊天序列化格式
                    if (isChat)
                    {
                        contents[i] = string.Format("[@{0}#{1}]", field.Role.RoleName.ToStringUtf8(), field.Role.RoleId.ToString());
                    }
                    else
                    {
                        contents[i] = field.Role.RoleName.ToStringUtf8();
                    }
                    break;
                case SysTipFieldType.SysTipFieldItem:
                    uint itemId = field.Item.ItemInfoId;
                    var csvItem = CSVItem.Instance.GetConfData(itemId);
                    if (csvItem != null)
                    {
                        uint colorIndex = field.Item.Color - 1u;
                        colorIndex = colorIndex <= 0 ? 0 : colorIndex;
                        string color = Constants.gChatColors_Item[colorIndex];
                        if (isChat) 
                        {
                            contents[i] = string.Format("<color=#{0} a=6_{1}>{2}</color>", color, itemId.ToString(), LanguageHelper.GetTextContent(csvItem.name_id));
                        }
                        else 
                        {
                            contents[i] = string.Format("<color=#{0}>{1}</color>", color, LanguageHelper.GetTextContent(csvItem.name_id));
                        }
                    }
                    break;
                case SysTipFieldType.SysTipFieldAchievement:
                    uint achievementId = field.Achievement.AchievementId;
                    AchievementDataCell achievementData = Sys_Achievement.Instance.GetAchievementByTid(achievementId);
                    if (isChat)
                        contents[i] = string.Format("<color=#30dbe1 a=14_{0}>[{1}]</color>", achievementId, LanguageHelper.GetAchievementContent(achievementData.csvAchievementData.Achievement_Title));
                    else
                        contents[i] = string.Format("<color=#30dbe1>{0}</color>", LanguageHelper.GetAchievementContent(achievementData.csvAchievementData.Achievement_Title));
                    break;
                case SysTipFieldType.SysTipFieldCareer:
                    CSVCareer.Data data = CSVCareer.Instance.GetConfData(field.Career);
                    contents[i] = LanguageHelper.GetTextContent(data.name);
                    break;
                default:
                    break;
            }
        }

        return LanguageHelper.GetErrorCodeContent(csv, contents);
    }
}