using Lib.Core;
using Logic;
using Packet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Table;

public class InputCache
{    
    const string sItemFormatRev = "[#{0}]"; //物品和宠物
    const string sTitleFormatRev = "[%{0}]";
    const string sTaskFormatRev = "[!{0}]";    
    const string sAchievementFormatRev = "[~{0}]";    
    const string sItemFormat = "{0}[{1}]";

    const string sFormat = "{0}[{1}]";    //道具[xxx] | 宠物[xxx]
    //protected static readonly StringBuilder tempStringBuilder = new StringBuilder();
    public static readonly Regex sEmojiRegex = new Regex("\\[(.*?)\\]");
    
    List<Packet.ItemCommonData> itemDatas = new List<ItemCommonData>();
    List<Packet.AchievementData> achievementDatas = new List<AchievementData>();
    public Action onChange = null;
    private int nTextCountLimit = 256;

    public bool bCheckGM = false;

    /// <summary>
    /// 再本地文件中标识顺序用的
    /// </summary>
    internal uint nRecodeIndex = 0;
    private string sContent = String.Empty;
    private Dictionary<string, ulong> mItems = new Dictionary<string, ulong>();
    private Dictionary<string, uint> mPets = new Dictionary<string, uint>();
    private Dictionary<string, uint> mTitles = new Dictionary<string, uint>();
    private Dictionary<string, uint> mTasks = new Dictionary<string, uint>();
    private Dictionary<string, uint> mAchievements = new Dictionary<string, uint>();
    private HashSet<char> mRemoveChars = new HashSet<char>() { '\n', '\t', '\r', ' ' };

    public InputCache(bool checkGM = false)
    {
        bCheckGM = checkGM;
    }

    public void SetLimitCount(int limitCount)
    {
        nTextCountLimit = limitCount < 0 ? 256 : limitCount;
    }

    public void CopyFrom(InputCache cache, bool bClearList = false)
    {
        mItems.Clear();
        mPets.Clear();
        mTitles.Clear();
        mTasks.Clear();
        mAchievements.Clear();

        SetContent(cache.sContent, false);

        foreach (var kv in cache.mItems)
        {
            mItems.Add(kv.Key, kv.Value);
        }

        foreach (var kv in cache.mPets)
        {
            mPets.Add(kv.Key, kv.Value);
        }

        foreach (var kv in cache.mTitles)
        {
            mTitles.Add(kv.Key, kv.Value);
        }

        foreach (var kv in cache.mTasks)
        {
            mTasks.Add(kv.Key, kv.Value);
        }

        foreach (var kv in cache.mAchievements)
        {
            mAchievements.Add(kv.Key, kv.Value);
        }

        onChange?.Invoke();
    }

    public void Clear()
    {
        mItems.Clear();
        mPets.Clear();
        mTitles.Clear();
        mTasks.Clear();
        mAchievements.Clear();

        sContent = string.Empty;        
        onChange?.Invoke();
    }

    public int AddContent(string content, int caretPosition = -1)
    {
        int len = sContent.Length;

        if (content.Length + sContent.Length > nTextCountLimit)
        {
            return Sys_Chat.Chat_Count_Up_Limit;
        }

        if(caretPosition < 0 || caretPosition >= sContent.Length)
        {
            SetContent(sContent + content);
        }
        else
        {
            SetContent(sContent.Insert(caretPosition, content));
        }

        //sContent += content;
        //onChange?.Invoke();
        //Sys_Chat.Instance.eventEmitter.Trigger(EEvents.InputChange);
        return Sys_Chat.Chat_Success;
    }

    public void SetContent(string content, bool checkWord = true)
    {
        if (string.Equals(sContent, content, StringComparison.Ordinal))
            return;

        bool isGM = false;
        if (bCheckGM && content.StartsWith(Sys_Chat.Chat_GM_Header, StringComparison.Ordinal))
        {
            sContent = content;
            isGM = true;
        }

        if (!isGM)
        {
            if (content.Length > nTextCountLimit)
            {
                sContent = content.Remove(nTextCountLimit);
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2007908));
            }
            else
            {
                sContent = content;
            }

            if (checkWord && !string.IsNullOrWhiteSpace(sContent))
            {
                //sContent = Sys_WordInput.Instance.LimitLengthAndFilter(sContent, nTextCountLimit);
                sContent = sContent.Replace('<', '*').Replace('>', '*');                
            }
        }

        if (string.IsNullOrWhiteSpace(sContent))
        {
            mItems.Clear();
            mPets.Clear();
            mTitles.Clear();
            mAchievements.Clear();
        }

        onChange?.Invoke();
        //Sys_Chat.Instance.eventEmitter.Trigger(EEvents.InputChange);
    }

    public string GetContent()
    {
        return sContent;
    }

    public int AddItemContent(ItemData itemData, int caretPosition = -1)
    {
        string itemNane0 = LanguageHelper.GetTextContent(itemData.cSVItemData.name_id);
        int index = 0;

        string itemNane = itemNane0;
        while (true)
        {
            if (!mItems.TryGetValue(itemNane, out ulong itemUUID))
            {
                mItems.Add(itemNane, itemData.Uuid);
                break;
            }
            else if (itemData.Uuid == itemUUID)
            {
                //已经有了物品直接不再添加
                if (sContent.Contains('[' + itemNane + ']'))
                {
                    return Sys_Chat.Chat_Success;
                }
                else
                {
                    break;
                }
            }
            ++index;
            itemNane = itemNane0 + index.ToString();
        }

        string content = string.Format(sFormat, LanguageHelper.GetTextContent(2007916), itemNane);
        int rlt = AddContent(content, caretPosition);

        Sys_Chat.Instance.eventEmitter.Trigger(Sys_Chat.EEvents.InputChange);
        Sys_Society.Instance.eventEmitter.Trigger(Sys_Society.EEvents.InputChange);

        return rlt;
    }

    public int AddPetContent(ClientPet petData, int caretPosition = -1)
    {
        string petName0 = LanguageHelper.GetTextContent(petData.petData.name);
        int index = 0;

        string petName = petName0;
        while (true)
        {
            if (!mPets.TryGetValue(petName, out uint petUUID))
            {
                mPets.Add(petName, petData.GetPetUid());
                break;
            }
            else if (petData.GetPetUid() == petUUID)
            {
                //已经有了物品直接不再添加
                if (sContent.Contains('[' + petName + ']'))
                {
                    return Sys_Chat.Chat_Success;
                }
                else
                {
                    break;
                }
            }
            ++index;
            petName = petName0 + index.ToString();
        }

        string content = string.Format(sFormat, LanguageHelper.GetTextContent(2007917), petName);
        int rlt = AddContent(content, caretPosition);

        Sys_Chat.Instance.eventEmitter.Trigger(Sys_Chat.EEvents.InputChange);
        Sys_Society.Instance.eventEmitter.Trigger(Sys_Society.EEvents.InputChange);

        return rlt;
    }

    public int AddTitleContent(Title titleData, int caretPosition = -1)
    {
        string titleName = LanguageHelper.GetTextContent(titleData.cSVTitleData.titleLan);
        if (string.IsNullOrWhiteSpace(titleName))
        {
            return Sys_Chat.Chat_Success;
        }

        if (!mTitles.TryGetValue(titleName, out uint titleID))
        {
            mTitles.Add(titleName, titleData.Id);
        }
        else
        {
            //已经有了物品直接不再添加
            if (sContent.Contains('[' + titleName + ']'))
            {
                return Sys_Chat.Chat_Success;
            }
        }

        string content = string.Format(sFormat, LanguageHelper.GetTextContent(2007915), titleName);
        int rlt = AddContent(content, caretPosition);

        Sys_Chat.Instance.eventEmitter.Trigger(Sys_Chat.EEvents.InputChange);
        Sys_Society.Instance.eventEmitter.Trigger(Sys_Society.EEvents.InputChange);

        return rlt;
    }

    public int AddTask(uint task, int caretPosition = -1)
    {
        CSVTask.Data taskData = CSVTask.Instance.GetConfData(task);
        string taskName = LanguageHelper.GetTaskTextContent(taskData.taskName);

        if (!mTasks.TryGetValue(taskName, out uint taskID))
        {
            mTasks.Add(taskName, task);
        }
        else
        {
            //已经有了物品直接不再添加
            if (sContent.Contains('[' + taskName + ']'))
            {
                return Sys_Chat.Chat_Success;
            }
        }

        string content;
        if (taskData.taskCategory == (uint)ETaskCategory.Trunk)
        {
            content = string.Format(sFormat, LanguageHelper.GetTextContent(2007910), taskName);
        }
        else
        {
            content = string.Format(sFormat, LanguageHelper.GetTextContent(2007911), taskName);
        }

        int rlt = AddContent(content, caretPosition);

        Sys_Chat.Instance.eventEmitter.Trigger(Sys_Chat.EEvents.InputChange);
        Sys_Society.Instance.eventEmitter.Trigger(Sys_Society.EEvents.InputChange);

        return rlt;
    }

    public static void AppendIgnore(StringBuilder stringBuilder, string value, int startIndex, int count, HashSet<char> cs)
    {
        for (int i = 0; i < count; ++i)
        {
            char v = value[i + startIndex];
            if (v != '\n' && v != '\t' && v != '\r' && v != ' ')
            {
                stringBuilder.Append(v);
            }
        }
    }
    public int AddAchievement(AchievementDataCell achData, int caretPosition = -1)
    {
        string achName = LanguageHelper.GetAchievementContent(achData.csvAchievementData.Achievement_Title);
        if (!mAchievements.TryGetValue(achName, out uint achId))
        {
            mAchievements.Add(achName, achData.tid);
        }
        else
        {
            //已经有了成就直接不再添加
            if (sContent.Contains('[' + achName + ']'))
            {
                return Sys_Chat.Chat_Success;
            }
        }

        string content = string.Format(sFormat, LanguageHelper.GetTextContent(5878), achName);
        int rlt = AddContent(content, caretPosition);

        Sys_Chat.Instance.eventEmitter.Trigger(Sys_Chat.EEvents.InputChange);
        Sys_Society.Instance.eventEmitter.Trigger(Sys_Society.EEvents.InputChange);

        return rlt;
    }

    public string GetSendContent(out ChatExtMsg datas)
    {
        datas = null;

        //sContent = string.IsNullOrWhiteSpace(sContent) ? sContent : sContent.Replace(" ", "\u00A0");
        StringBuilder tempStringBuilder = StringBuilderPool.GetTemporary();        

        if (mTitles.Count < 1 && mItems.Count < 1 && mPets.Count < 1 && mTasks.Count < 1 && mAchievements.Count < 1)
        {
            AppendIgnore(tempStringBuilder, sContent, 0, sContent.Length, mRemoveChars);
            //tempStringBuilder.AppendIgnore(sContent, 0, sContent.Length, mRemoveChars);
        }
        else
        {            
            //TODO 发送前检查不需要的Item
            int itemCount = 0;
            int achCount = 0;
            int offsetIndex = 0;
            MatchCollection matchs = sEmojiRegex.Matches(sContent);
            for (int i = 0; i < matchs.Count; ++i)
            {
                if (i > 0)
                {
                    offsetIndex = matchs[i - 1].Index + matchs[i - 1].Length;
                }

                Match match = matchs[i];
                string key = match.Groups[1].Value;
                if (match.Index > offsetIndex)
                {
                    AppendIgnore(tempStringBuilder, sContent, offsetIndex, match.Index - offsetIndex, mRemoveChars);
                    //tempStringBuilder.AppendIgnore(sContent, offsetIndex, match.Index - offsetIndex, mRemoveChars);
                }

                if (mItems.TryGetValue(key, out ulong itemUUID))//检查物品
                {
                    ItemData itemData = Sys_Bag.Instance.GetItemDataByUuid(itemUUID);
                    if (itemData == null)
                        continue;

                    Packet.ItemCommonData item = new Packet.ItemCommonData();

                    item.BoxId = itemData.BoxId;
                    item.Uuid = itemData.Uuid;
                    item.Id = itemData.Id;
                    item.Position = itemData.Position;
                    item.Count = itemData.Count;
                    item.Quality = itemData.Quality;
                    item.Equipment = itemData.Equip;
                    item.Essence = itemData.essence;
                    item.Crystal = itemData.crystal;
                    item.PetUnit = itemData.Pet;
                    item.SrcUIId = itemData.SrcUIId;
                    item.Ornament = itemData.ornament;
                    item.PetEquip = itemData.petEquip;
                    itemDatas.Add(item);

                    tempStringBuilder.AppendFormat(sItemFormatRev, itemCount.ToString());
                    ++itemCount;
                    continue;
                }
                else if (mPets.TryGetValue(key, out uint petUUID))//检查宠物
                {
                    ClientPet petData = Sys_Pet.Instance.GetPetByUId(petUUID);
                    if (petData == null)
                        continue;

                    Packet.ItemCommonData item = new Packet.ItemCommonData();                    

                    item.Id = petData.petData.id;
                    item.PetUnit = petData.petUnit;
                    item.Quality = (uint)Sys_Pet.Instance.GetPetQuality(petData.petUnit);

                    itemDatas.Add(item);

                    tempStringBuilder.AppendFormat(sItemFormatRev, itemCount.ToString());
                    ++itemCount;
                    continue;
                }
                else if (mTitles.TryGetValue(key, out uint titleID))//称号
                {
                    tempStringBuilder.AppendFormat(sTitleFormatRev, titleID.ToString());
                }
                else if (mTasks.TryGetValue(key, out uint taskID))//称号
                {
                    tempStringBuilder.AppendFormat(sTaskFormatRev, taskID.ToString());
                }
                else if (mAchievements.TryGetValue(key, out uint achievementID))//成就
                {
                    AchievementDataCell dataCell = Sys_Achievement.Instance.GetAchievementByTid(achievementID);
                    AchievementData data = new AchievementData();
                    data.Id = achievementID;
                    if (dataCell.CheckIsMerge())
                    {
                        if(dataCell.CheckIsSelf())
                            data.Timestamp = dataCell.timestamp;
                        else
                            data.Timestamp = 0;
                        for (int j = 0; j < dataCell.achHistoryList.Count; j++)
                        {
                            RoleAchievementHistory historyData = new RoleAchievementHistory();
                            historyData.Servername = dataCell.achHistoryList[i].serverNameByte;
                            historyData.Timestamp = dataCell.achHistoryList[i].timestamp;
                            data.HistoryDatas.Add(historyData);
                        }
                    }
                    else
                        data.Timestamp = dataCell.timestamp;
                    achievementDatas.Add(data);
                    tempStringBuilder.AppendFormat(sAchievementFormatRev, achCount.ToString());
                    ++achCount;
                }
                else
                {
                    AppendIgnore(tempStringBuilder, sContent, match.Index, match.Length, mRemoveChars);
                    //tempStringBuilder.AppendIgnore(sContent, match.Index, match.Length, mRemoveChars);
                }
            }

            if (matchs.Count > 0)
            {
                Match match = matchs[matchs.Count - 1];
                offsetIndex = match.Index + match.Length;
            }
            AppendIgnore(tempStringBuilder, sContent, offsetIndex, sContent.Length - offsetIndex, mRemoveChars);
            //tempStringBuilder.AppendIgnore(sContent, offsetIndex, sContent.Length - offsetIndex, mRemoveChars);

            if (itemDatas.Count > 0)
            {
                datas = new ChatExtMsg();
                datas.Item.AddRange(itemDatas);
            }
            if (achievementDatas.Count > 0)
            {
                if (datas == null)
                    datas = new ChatExtMsg();
                datas.AchData.AddRange(achievementDatas);
            }

            itemDatas.Clear();
            achievementDatas.Clear();
        }

        string rlt = StringBuilderPool.ReleaseTemporaryAndToString(tempStringBuilder);
        //rlt = SpecialWords.CheckAndReplace(rlt);
        return rlt;
    }

    public bool Equals(InputCache x, InputCache y)
    {
        return string.Equals(x.sContent, y.sContent, StringComparison.Ordinal);
    }

    public int GetHashCode(InputCache obj)
    {
        return sContent.GetHashCode();
    }

    public int WriteToFile(string path, byte[] buffer)
    {
        System.IO.FileStream fileStream;
        fileStream = System.IO.File.Open(path, System.IO.FileMode.OpenOrCreate);

        fileStream.SetLength(0);
        fileStream.Position = 0;

        System.IO.BinaryWriter binaryWriter = new System.IO.BinaryWriter(fileStream);
        WriteHelper.WriteString(binaryWriter, sContent);

        WriteHelper.Write(binaryWriter, mItems.Count);
        Dictionary<string, ulong>.Enumerator itemEnumerator = mItems.GetEnumerator();
        while (itemEnumerator.MoveNext())
        {
            WriteHelper.WriteString(binaryWriter, itemEnumerator.Current.Key);
            WriteHelper.Write(binaryWriter, itemEnumerator.Current.Value);
        }

        WriteHelper.Write(binaryWriter, mPets.Count);
        Dictionary<string, uint>.Enumerator petEnumerator = mPets.GetEnumerator();
        while (petEnumerator.MoveNext())
        {
            WriteHelper.WriteString(binaryWriter, petEnumerator.Current.Key);
            WriteHelper.Write(binaryWriter, petEnumerator.Current.Value);
        }

        WriteHelper.Write(binaryWriter, mTitles.Count);
        Dictionary<string, uint>.Enumerator titleEnumerator = mTitles.GetEnumerator();
        while (titleEnumerator.MoveNext())
        {
            WriteHelper.WriteString(binaryWriter, titleEnumerator.Current.Key);
            WriteHelper.Write(binaryWriter, titleEnumerator.Current.Value);
        }

        WriteHelper.Write(binaryWriter, mTasks.Count);
        Dictionary<string, uint>.Enumerator taskEnumerator = mTasks.GetEnumerator();
        while (taskEnumerator.MoveNext())
        {
            WriteHelper.WriteString(binaryWriter, taskEnumerator.Current.Key);
            WriteHelper.Write(binaryWriter, taskEnumerator.Current.Value);
        }

        WriteHelper.Write(binaryWriter, mAchievements.Count);
        Dictionary<string, uint>.Enumerator achievementEnumerator = mAchievements.GetEnumerator();
        while (achievementEnumerator.MoveNext())
        {
            WriteHelper.WriteString(binaryWriter, achievementEnumerator.Current.Key);
            WriteHelper.Write(binaryWriter, achievementEnumerator.Current.Value);
        }

        WriteHelper.Write(binaryWriter, nRecodeIndex);

        binaryWriter.Dispose();
        binaryWriter.Close();
        fileStream.Dispose();
        fileStream.Close();
        return 0;
    }

    public int ReadFromFile(string path, byte[] buffer)
    {
        System.IO.FileStream fileStream;
        if (!System.IO.File.Exists(path))
        {
            return 0;
        }

        fileStream = System.IO.File.Open(path, System.IO.FileMode.Open);
        fileStream.Position = 0;

        System.IO.BinaryReader binaryWriter = new System.IO.BinaryReader(fileStream);
        sContent = ReadHelper.ReadString(binaryWriter);

        int itemCount = ReadHelper.ReadInt(binaryWriter);
        mItems.Clear();
        for (int i = 0; i < itemCount; ++i)
        {
            string k = ReadHelper.ReadString(binaryWriter);
            ulong v = ReadHelper.ReadUInt64(binaryWriter);
            mItems.Add(k, v);
        }

        int petCount = ReadHelper.ReadInt(binaryWriter);
        mPets.Clear();
        for (int i = 0; i < petCount; ++i)
        {
            string k = ReadHelper.ReadString(binaryWriter);
            uint v = ReadHelper.ReadUInt(binaryWriter);
            mPets.Add(k, v);
        }

        int titleCount = ReadHelper.ReadInt(binaryWriter);
        mTitles.Clear();
        for (int i = 0; i < titleCount; ++i)
        {
            string k = ReadHelper.ReadString(binaryWriter);
            uint v = ReadHelper.ReadUInt(binaryWriter);
            mTitles.Add(k, v);
        }

        int taskCount = ReadHelper.ReadInt(binaryWriter);
        mTasks.Clear();
        for (int i = 0; i < taskCount; ++i)
        {
            string k = ReadHelper.ReadString(binaryWriter);
            uint v = ReadHelper.ReadUInt(binaryWriter);
            mTasks.Add(k, v);
        }

        int achievementCount = ReadHelper.ReadInt(binaryWriter);
        mAchievements.Clear();
        for (int i = 0; i < achievementCount; ++i)
        {
            string k = ReadHelper.ReadString(binaryWriter);
            uint v = ReadHelper.ReadUInt(binaryWriter);
            mAchievements.Add(k, v);
        }

        nRecodeIndex = ReadHelper.ReadUInt(binaryWriter);

        binaryWriter.Dispose();
        binaryWriter.Close();
        fileStream.Dispose();
        fileStream.Close();
        return 0;
    }
}

public class InputCacheRecord
{   
    private string _dir;
    public byte[] buffer = new byte[1024];
    public readonly List<InputCache> mContentRecords;
    private uint _currentIndex = 0;

    //序列化方式改变后需要提升版本号 来放弃原有的记录文件
    private const uint version = 3;

    public InputCacheRecord(int maxCount, string dir)
    {        
        _dir = dir;
        mContentRecords = new List<InputCache>(maxCount);
    }

    public void WriteAll()
    {
        for (int i = 0; i < mContentRecords.Count; ++i)
        {
            mContentRecords[i].nRecodeIndex = (uint)i;
            string path = Lib.AssetLoader.AssetPath.GetPersistentFullPath(_dir + "/ls/" + (mContentRecords.Count - i - 1).ToString());
            mContentRecords[i].WriteToFile(path, buffer);
        }

        _currentIndex = (uint)mContentRecords.Count;
    }

    public int ReadInputCache()
    {
        bool needClear = false;
        string versionPath = Lib.AssetLoader.AssetPath.GetPersistentFullPath(_dir + "/lsversion");
        if (System.IO.File.Exists(versionPath))
        {
            string s = System.IO.File.ReadAllText(versionPath);
            if (!uint.TryParse(s, out uint versionLocal) || versionLocal != version)
            {
                needClear = true;
            }
        }
        else
        {
            needClear = true;
        }

        string dir = Lib.AssetLoader.AssetPath.GetPersistentFullPath(_dir + "/ls");
        if (needClear)
        {
            DebugUtil.Log(ELogType.eNone, "聊天输入数据结构变更 清除旧的聊天输入记录");
            if (System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.Delete(dir, true);
            }
        }

        if (!System.IO.Directory.Exists(dir))
        {
            System.IO.Directory.CreateDirectory(dir);
        }

        System.IO.File.WriteAllText(versionPath, version.ToString());

        string[] paths = System.IO.Directory.GetFiles(dir);
        int fileCount = Math.Min(paths.Length, mContentRecords.Capacity);

        for (int i = 0; i < fileCount; ++i)
        {
            string path = paths[i];
            if (System.IO.File.Exists(path))
            {
                InputCache inputCache = new InputCache();
                inputCache.ReadFromFile(path, buffer);

                int j = 0;
                while (j < mContentRecords.Count)
                {
                    if (inputCache.nRecodeIndex > mContentRecords[j].nRecodeIndex)
                    {
                        break;
                    }
                    ++j;
                }

                mContentRecords.Insert(j, inputCache);
                System.IO.File.Delete(path);
            }
        }

        WriteAll();

        return 0;
    }

    public int RecodeInputCache(InputCache cache)
    {
        InputCache newCache = null;

        int index = mContentRecords.FindIndex((x) => { return string.Equals(x.GetContent(), cache.GetContent(), StringComparison.Ordinal); });
        if (index >= 0)
        {
            newCache = mContentRecords[index];

            mContentRecords.RemoveAt(index);
        }
        else if (mContentRecords.Count == mContentRecords.Capacity)
        {
            newCache = mContentRecords[mContentRecords.Count - 1];

            mContentRecords.RemoveAt(mContentRecords.Count - 1);
        }
        else
        {
            newCache = new InputCache();
            newCache.nRecodeIndex = _currentIndex;
        }

        newCache.CopyFrom(cache);
        mContentRecords.Insert(0, newCache);

        string path = Lib.AssetLoader.AssetPath.GetPersistentFullPath(_dir + "/ls/" + newCache.nRecodeIndex.ToString());
        if(System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        newCache.nRecodeIndex = _currentIndex;
        path = Lib.AssetLoader.AssetPath.GetPersistentFullPath(_dir + "/ls/" + newCache.nRecodeIndex.ToString());
        newCache.WriteToFile(path, buffer);

        //当序号超出的时候 重新从0开始排
        ++_currentIndex;
        if (_currentIndex >= uint.MaxValue)
        {
            WriteAll();
        }
        return 0;
    }
}