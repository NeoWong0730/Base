// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: classicboss.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Packet {

  #region Enums
  /// <summary>
  ///5300
  /// </summary>
  public enum CmdClassicBoss {
    None = 0,
    /// <summary>
    ///上线数据通知
    /// </summary>
    DataNtf = 5401,
    /// <summary>
    ///战斗结束通知
    /// </summary>
    ResultNtf = 5402,
    /// <summary>
    ///npc前发起投票准备进战斗
    /// </summary>
    StartVoteReq = 5403,
  }

  public enum ErrorClassicBoss {
    None = 0,
    /// <summary>
    ///相关表格未找到
    /// </summary>
    CsvnotFound = 5402,
    /// <summary>
    ///功能未开启
    /// </summary>
    FuctionNotOpen = 5403,
    /// <summary>
    ///未解锁
    /// </summary>
    NotUnlock = 5404,
    /// <summary>
    ///无法进入战斗
    /// </summary>
    CannotEnterBattle = 5405,
    /// <summary>
    ///有队员暂离，无法进入战斗
    /// </summary>
    MemTmpLeaveCannotEnter = 5406,
  }

  public enum ClassicBossActiveReason {
    None = 0,
    /// <summary>
    ///挑战奖励(经典头目战)
    /// </summary>
    BattleReward = 5403,
  }

  #endregion

  #region Messages
  public sealed class ClassicBossData : pb::IMessage {
    private static readonly pb::MessageParser<ClassicBossData> _parser = new pb::MessageParser<ClassicBossData>(() => new ClassicBossData());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ClassicBossData> Parser { get { return _parser; } }

    /// <summary>Field number for the "rewardLimit" field.</summary>
    public const int RewardLimitFieldNumber = 1;
    private global::Packet.ResLimit rewardLimit_;
    /// <summary>
    ///已使用奖励次数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Packet.ResLimit RewardLimit {
      get { return rewardLimit_; }
      set {
        rewardLimit_ = value;
      }
    }

    /// <summary>Field number for the "exploredBossIds" field.</summary>
    public const int ExploredBossIdsFieldNumber = 2;
    private static readonly pb::FieldCodec<uint> _repeated_exploredBossIds_codec
        = pb::FieldCodec.ForUInt32(18);
    private readonly pbc::RepeatedField<uint> exploredBossIds_ = new pbc::RepeatedField<uint>();
    /// <summary>
    ///已经挑战过的bossid
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<uint> ExploredBossIds {
      get { return exploredBossIds_; }
    }

    /// <summary>Field number for the "lastBossId" field.</summary>
    public const int LastBossIdFieldNumber = 3;
    private uint lastBossId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint LastBossId {
      get { return lastBossId_; }
      set {
        lastBossId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (rewardLimit_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(RewardLimit);
      }
      exploredBossIds_.WriteTo(output, _repeated_exploredBossIds_codec);
      if (LastBossId != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(LastBossId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (rewardLimit_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(RewardLimit);
      }
      size += exploredBossIds_.CalculateSize(_repeated_exploredBossIds_codec);
      if (LastBossId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(LastBossId);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            if (rewardLimit_ == null) {
              rewardLimit_ = new global::Packet.ResLimit();
            }
            input.ReadMessage(rewardLimit_);
            break;
          }
          case 18:
          case 16: {
            exploredBossIds_.AddEntriesFrom(input, _repeated_exploredBossIds_codec);
            break;
          }
          case 24: {
            LastBossId = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CmdClassicBossDataNtf : pb::IMessage {
    private static readonly pb::MessageParser<CmdClassicBossDataNtf> _parser = new pb::MessageParser<CmdClassicBossDataNtf>(() => new CmdClassicBossDataNtf());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdClassicBossDataNtf> Parser { get { return _parser; } }

    /// <summary>Field number for the "data" field.</summary>
    public const int DataFieldNumber = 1;
    private global::Packet.ClassicBossData data_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Packet.ClassicBossData Data {
      get { return data_; }
      set {
        data_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (data_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Data);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (data_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Data);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            if (data_ == null) {
              data_ = new global::Packet.ClassicBossData();
            }
            input.ReadMessage(data_);
            break;
          }
        }
      }
    }

  }

  public sealed class CmdClassicBossResultNtf : pb::IMessage {
    private static readonly pb::MessageParser<CmdClassicBossResultNtf> _parser = new pb::MessageParser<CmdClassicBossResultNtf>(() => new CmdClassicBossResultNtf());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdClassicBossResultNtf> Parser { get { return _parser; } }

    /// <summary>Field number for the "classicBossId" field.</summary>
    public const int ClassicBossIdFieldNumber = 5;
    private uint classicBossId_;
    /// <summary>
    ///也用来更新本地挑战过的bossId
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ClassicBossId {
      get { return classicBossId_; }
      set {
        classicBossId_ = value;
      }
    }

    /// <summary>Field number for the "result" field.</summary>
    public const int ResultFieldNumber = 1;
    private uint result_;
    /// <summary>
    ///战斗结果，见上enum
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Result {
      get { return result_; }
      set {
        result_ = value;
      }
    }

    /// <summary>Field number for the "firstExplore" field.</summary>
    public const int FirstExploreFieldNumber = 2;
    private bool firstExplore_;
    /// <summary>
    ///是否为首次挑战
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool FirstExplore {
      get { return firstExplore_; }
      set {
        firstExplore_ = value;
      }
    }

    /// <summary>Field number for the "rewardLimit" field.</summary>
    public const int RewardLimitFieldNumber = 3;
    private global::Packet.ResLimit rewardLimit_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Packet.ResLimit RewardLimit {
      get { return rewardLimit_; }
      set {
        rewardLimit_ = value;
      }
    }

    /// <summary>Field number for the "rewards" field.</summary>
    public const int RewardsFieldNumber = 4;
    private static readonly pb::FieldCodec<global::Packet.CmdClassicBossResultNtf.Types.Reward> _repeated_rewards_codec
        = pb::FieldCodec.ForMessage(34, global::Packet.CmdClassicBossResultNtf.Types.Reward.Parser);
    private readonly pbc::RepeatedField<global::Packet.CmdClassicBossResultNtf.Types.Reward> rewards_ = new pbc::RepeatedField<global::Packet.CmdClassicBossResultNtf.Types.Reward>();
    /// <summary>
    ///奖励
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Packet.CmdClassicBossResultNtf.Types.Reward> Rewards {
      get { return rewards_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Result != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Result);
      }
      if (FirstExplore != false) {
        output.WriteRawTag(16);
        output.WriteBool(FirstExplore);
      }
      if (rewardLimit_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(RewardLimit);
      }
      rewards_.WriteTo(output, _repeated_rewards_codec);
      if (ClassicBossId != 0) {
        output.WriteRawTag(40);
        output.WriteUInt32(ClassicBossId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ClassicBossId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ClassicBossId);
      }
      if (Result != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Result);
      }
      if (FirstExplore != false) {
        size += 1 + 1;
      }
      if (rewardLimit_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(RewardLimit);
      }
      size += rewards_.CalculateSize(_repeated_rewards_codec);
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            Result = input.ReadUInt32();
            break;
          }
          case 16: {
            FirstExplore = input.ReadBool();
            break;
          }
          case 26: {
            if (rewardLimit_ == null) {
              rewardLimit_ = new global::Packet.ResLimit();
            }
            input.ReadMessage(rewardLimit_);
            break;
          }
          case 34: {
            rewards_.AddEntriesFrom(input, _repeated_rewards_codec);
            break;
          }
          case 40: {
            ClassicBossId = input.ReadUInt32();
            break;
          }
        }
      }
    }

    #region Nested types
    /// <summary>Container for nested types declared in the CmdClassicBossResultNtf message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static class Types {
      public enum ResultType {
        Fail = 0,
        SuccUnlock = 1,
        SuccReward = 2,
        SuccNoReward = 3,
      }

      public sealed class Reward : pb::IMessage {
        private static readonly pb::MessageParser<Reward> _parser = new pb::MessageParser<Reward>(() => new Reward());
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<Reward> Parser { get { return _parser; } }

        /// <summary>Field number for the "itemId" field.</summary>
        public const int ItemIdFieldNumber = 1;
        private uint itemId_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public uint ItemId {
          get { return itemId_; }
          set {
            itemId_ = value;
          }
        }

        /// <summary>Field number for the "count" field.</summary>
        public const int CountFieldNumber = 2;
        private long count_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public long Count {
          get { return count_; }
          set {
            count_ = value;
          }
        }

        /// <summary>Field number for the "item" field.</summary>
        public const int ItemFieldNumber = 3;
        private global::Packet.Item item_;
        /// <summary>
        ///获得道具的详细属性
        /// </summary>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public global::Packet.Item Item {
          get { return item_; }
          set {
            item_ = value;
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void WriteTo(pb::CodedOutputStream output) {
          if (ItemId != 0) {
            output.WriteRawTag(8);
            output.WriteUInt32(ItemId);
          }
          if (Count != 0L) {
            output.WriteRawTag(16);
            output.WriteInt64(Count);
          }
          if (item_ != null) {
            output.WriteRawTag(26);
            output.WriteMessage(Item);
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int CalculateSize() {
          int size = 0;
          if (ItemId != 0) {
            size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ItemId);
          }
          if (Count != 0L) {
            size += 1 + pb::CodedOutputStream.ComputeInt64Size(Count);
          }
          if (item_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Item);
          }
          return size;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void MergeFrom(pb::CodedInputStream input) {
          uint tag;
          while ((tag = input.ReadTag()) != 0) {
            switch(tag) {
              default:
                input.SkipLastField();
                break;
              case 8: {
                ItemId = input.ReadUInt32();
                break;
              }
              case 16: {
                Count = input.ReadInt64();
                break;
              }
              case 26: {
                if (item_ == null) {
                  item_ = new global::Packet.Item();
                }
                input.ReadMessage(item_);
                break;
              }
            }
          }
        }

      }

    }
    #endregion

  }

  public sealed class CmdClassicBossStartVoteReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdClassicBossStartVoteReq> _parser = new pb::MessageParser<CmdClassicBossStartVoteReq>(() => new CmdClassicBossStartVoteReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdClassicBossStartVoteReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "classicBossId" field.</summary>
    public const int ClassicBossIdFieldNumber = 1;
    private uint classicBossId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ClassicBossId {
      get { return classicBossId_; }
      set {
        classicBossId_ = value;
      }
    }

    /// <summary>Field number for the "uNpcId" field.</summary>
    public const int UNpcIdFieldNumber = 2;
    private ulong uNpcId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong UNpcId {
      get { return uNpcId_; }
      set {
        uNpcId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ClassicBossId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(ClassicBossId);
      }
      if (UNpcId != 0UL) {
        output.WriteRawTag(16);
        output.WriteUInt64(UNpcId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ClassicBossId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ClassicBossId);
      }
      if (UNpcId != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(UNpcId);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            ClassicBossId = input.ReadUInt32();
            break;
          }
          case 16: {
            UNpcId = input.ReadUInt64();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///投票时附带数据
  /// </summary>
  public sealed class ClassicBossCliVoteData : pb::IMessage {
    private static readonly pb::MessageParser<ClassicBossCliVoteData> _parser = new pb::MessageParser<ClassicBossCliVoteData>(() => new ClassicBossCliVoteData());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ClassicBossCliVoteData> Parser { get { return _parser; } }

    /// <summary>Field number for the "mems" field.</summary>
    public const int MemsFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Packet.ClassicBossCliVoteData.Types.Mem> _repeated_mems_codec
        = pb::FieldCodec.ForMessage(10, global::Packet.ClassicBossCliVoteData.Types.Mem.Parser);
    private readonly pbc::RepeatedField<global::Packet.ClassicBossCliVoteData.Types.Mem> mems_ = new pbc::RepeatedField<global::Packet.ClassicBossCliVoteData.Types.Mem>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Packet.ClassicBossCliVoteData.Types.Mem> Mems {
      get { return mems_; }
    }

    /// <summary>Field number for the "classicBossId" field.</summary>
    public const int ClassicBossIdFieldNumber = 2;
    private uint classicBossId_;
    /// <summary>
    ///表id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ClassicBossId {
      get { return classicBossId_; }
      set {
        classicBossId_ = value;
      }
    }

    /// <summary>Field number for the "uNpcId" field.</summary>
    public const int UNpcIdFieldNumber = 3;
    private ulong uNpcId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong UNpcId {
      get { return uNpcId_; }
      set {
        uNpcId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      mems_.WriteTo(output, _repeated_mems_codec);
      if (ClassicBossId != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(ClassicBossId);
      }
      if (UNpcId != 0UL) {
        output.WriteRawTag(24);
        output.WriteUInt64(UNpcId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += mems_.CalculateSize(_repeated_mems_codec);
      if (ClassicBossId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ClassicBossId);
      }
      if (UNpcId != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(UNpcId);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            mems_.AddEntriesFrom(input, _repeated_mems_codec);
            break;
          }
          case 16: {
            ClassicBossId = input.ReadUInt32();
            break;
          }
          case 24: {
            UNpcId = input.ReadUInt64();
            break;
          }
        }
      }
    }

    #region Nested types
    /// <summary>Container for nested types declared in the ClassicBossCliVoteData message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static class Types {
      public sealed class Mem : pb::IMessage {
        private static readonly pb::MessageParser<Mem> _parser = new pb::MessageParser<Mem>(() => new Mem());
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<Mem> Parser { get { return _parser; } }

        /// <summary>Field number for the "roleId" field.</summary>
        public const int RoleIdFieldNumber = 1;
        private ulong roleId_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public ulong RoleId {
          get { return roleId_; }
          set {
            roleId_ = value;
          }
        }

        /// <summary>Field number for the "leftTimes" field.</summary>
        public const int LeftTimesFieldNumber = 2;
        private uint leftTimes_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public uint LeftTimes {
          get { return leftTimes_; }
          set {
            leftTimes_ = value;
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void WriteTo(pb::CodedOutputStream output) {
          if (RoleId != 0UL) {
            output.WriteRawTag(8);
            output.WriteUInt64(RoleId);
          }
          if (LeftTimes != 0) {
            output.WriteRawTag(16);
            output.WriteUInt32(LeftTimes);
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int CalculateSize() {
          int size = 0;
          if (RoleId != 0UL) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(RoleId);
          }
          if (LeftTimes != 0) {
            size += 1 + pb::CodedOutputStream.ComputeUInt32Size(LeftTimes);
          }
          return size;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void MergeFrom(pb::CodedInputStream input) {
          uint tag;
          while ((tag = input.ReadTag()) != 0) {
            switch(tag) {
              default:
                input.SkipLastField();
                break;
              case 8: {
                RoleId = input.ReadUInt64();
                break;
              }
              case 16: {
                LeftTimes = input.ReadUInt32();
                break;
              }
            }
          }
        }

      }

    }
    #endregion

  }

  #endregion

}

#endregion Designer generated code