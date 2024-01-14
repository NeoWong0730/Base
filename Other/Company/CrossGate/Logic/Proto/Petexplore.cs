// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: petexplore.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Packet {

  #region Enums
  /// <summary>
  ///7950 宠物探险
  /// </summary>
  public enum CmdPetExplore {
    None = 0,
    /// <summary>
    ///先不用请求数据
    /// </summary>
    InfoReq = 7950,
    /// <summary>
    ///探险所有数据
    /// </summary>
    InfoNtf = 7951,
    /// <summary>
    ///开始探险
    /// </summary>
    StartReq = 7952,
    /// <summary>
    ///探险返回
    /// </summary>
    StartRes = 7953,
    /// <summary>
    ///领取探险奖励
    /// </summary>
    FinishGetAwardReq = 7954,
    /// <summary>
    /// </summary>
    FinishGetAwardRes = 7955,
    /// <summary>
    ///领取冒险点奖励
    /// </summary>
    GetPointAwardReq = 7956,
    GetPointAwardRes = 7957,
  }

  public enum ErrorPetExplore {
    None = 0,
    /// <summary>
    ///配置错误
    /// </summary>
    CsvnotFound = 7950,
    /// <summary>
    ///未领取的探险任务太多,请领取后再派遣新的任务
    /// </summary>
    TaskTooMuch = 7951,
    /// <summary>
    ///冒险点数未达标不能领取
    /// </summary>
    GetScoreFail = 7952,
    /// <summary>
    ///重复领取冒险点奖励
    /// </summary>
    GetScoreFailRepeat = 7953,
    /// <summary>
    ///活动未开启
    /// </summary>
    ActNotOpen = 7954,
    /// <summary>
    ///未找到要探险的宠物
    /// </summary>
    PetUidError = 7956,
    /// <summary>
    ///未找到要探险的宠物配置
    /// </summary>
    PetCfgNotFound = 7957,
    /// <summary>
    ///未找到任务
    /// </summary>
    NotFindTask = 7958,
    /// <summary>
    ///任务尚未开始探险
    /// </summary>
    TaskNotStart = 7959,
    /// <summary>
    ///正在探险
    /// </summary>
    TaskExploring = 7960,
    /// <summary>
    ///探险已完成未领取奖励
    /// </summary>
    TaskExplored = 7961,
    /// <summary>
    ///探险奖励已经领取
    /// </summary>
    TaskAwardRecieved = 7962,
    /// <summary>
    ///最多只能派出3个探险宠物
    /// </summary>
    PetToMuch = 7963,
    /// <summary>
    ///探险失败,有的宠物已经在探险
    /// </summary>
    PetAlreadyExploring = 7964,
    /// <summary>
    ///掉落配置错误
    /// </summary>
    DropIdError = 7965,
    /// <summary>
    ///不存在冒险点奖励
    /// </summary>
    PointIndexError = 7966,
    /// <summary>
    ///活动即将结束,不能开始新的探险
    /// </summary>
    ComingToEnd = 7967,
  }

  public enum PetExploreItemReason {
    None = 0,
    /// <summary>
    ///宠物探险奖励
    /// </summary>
    Gift = 7950,
    /// <summary>
    ///宠物探险冒险点数奖励
    /// </summary>
    ExploreGift = 7951,
  }

  public enum ExploreAwardState {
    /// <summary>
    ///0:未领取
    /// </summary>
    Unreceived = 0,
    /// <summary>
    ///1:失败
    /// </summary>
    Fail = 1,
    /// <summary>
    ///2:成功
    /// </summary>
    Suc = 2,
    /// <summary>
    ///3:大成功
    /// </summary>
    BigSuc = 3,
  }

  #endregion

  #region Messages
  public sealed class CmdPetExploreInfoReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdPetExploreInfoReq> _parser = new pb::MessageParser<CmdPetExploreInfoReq>(() => new CmdPetExploreInfoReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdPetExploreInfoReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "actId" field.</summary>
    public const int ActIdFieldNumber = 1;
    private uint actId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ActId {
      get { return actId_; }
      set {
        actId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ActId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(ActId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ActId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ActId);
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
            ActId = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class ExplorePetInfo : pb::IMessage {
    private static readonly pb::MessageParser<ExplorePetInfo> _parser = new pb::MessageParser<ExplorePetInfo>(() => new ExplorePetInfo());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ExplorePetInfo> Parser { get { return _parser; } }

    /// <summary>Field number for the "petUid" field.</summary>
    public const int PetUidFieldNumber = 1;
    private uint petUid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint PetUid {
      get { return petUid_; }
      set {
        petUid_ = value;
      }
    }

    /// <summary>Field number for the "petInfoId" field.</summary>
    public const int PetInfoIdFieldNumber = 2;
    private uint petInfoId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint PetInfoId {
      get { return petInfoId_; }
      set {
        petInfoId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (PetUid != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(PetUid);
      }
      if (PetInfoId != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(PetInfoId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (PetUid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(PetUid);
      }
      if (PetInfoId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(PetInfoId);
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
            PetUid = input.ReadUInt32();
            break;
          }
          case 16: {
            PetInfoId = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class ExploreTaskInfo : pb::IMessage {
    private static readonly pb::MessageParser<ExploreTaskInfo> _parser = new pb::MessageParser<ExploreTaskInfo>(() => new ExploreTaskInfo());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ExploreTaskInfo> Parser { get { return _parser; } }

    /// <summary>Field number for the "taskId" field.</summary>
    public const int TaskIdFieldNumber = 1;
    private uint taskId_;
    /// <summary>
    ///taskId = 配置表任务Id*1000+活动天数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint TaskId {
      get { return taskId_; }
      set {
        taskId_ = value;
      }
    }

    /// <summary>Field number for the "petInfos" field.</summary>
    public const int PetInfosFieldNumber = 2;
    private static readonly pb::FieldCodec<global::Packet.ExplorePetInfo> _repeated_petInfos_codec
        = pb::FieldCodec.ForMessage(18, global::Packet.ExplorePetInfo.Parser);
    private readonly pbc::RepeatedField<global::Packet.ExplorePetInfo> petInfos_ = new pbc::RepeatedField<global::Packet.ExplorePetInfo>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Packet.ExplorePetInfo> PetInfos {
      get { return petInfos_; }
    }

    /// <summary>Field number for the "endTick" field.</summary>
    public const int EndTickFieldNumber = 3;
    private uint endTick_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint EndTick {
      get { return endTick_; }
      set {
        endTick_ = value;
      }
    }

    /// <summary>Field number for the "awardState" field.</summary>
    public const int AwardStateFieldNumber = 4;
    private uint awardState_;
    /// <summary>
    ///奖励标记 enum ExploreAwardState
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint AwardState {
      get { return awardState_; }
      set {
        awardState_ = value;
      }
    }

    /// <summary>Field number for the "levelSuc" field.</summary>
    public const int LevelSucFieldNumber = 5;
    private uint levelSuc_;
    /// <summary>
    ///等级成功率
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint LevelSuc {
      get { return levelSuc_; }
      set {
        levelSuc_ = value;
      }
    }

    /// <summary>Field number for the "scoreSuc" field.</summary>
    public const int ScoreSucFieldNumber = 6;
    private uint scoreSuc_;
    /// <summary>
    ///评分成功率
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ScoreSuc {
      get { return scoreSuc_; }
      set {
        scoreSuc_ = value;
      }
    }

    /// <summary>Field number for the "elementSuc" field.</summary>
    public const int ElementSucFieldNumber = 7;
    private uint elementSuc_;
    /// <summary>
    ///元素成功率
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ElementSuc {
      get { return elementSuc_; }
      set {
        elementSuc_ = value;
      }
    }

    /// <summary>Field number for the "raceSuc" field.</summary>
    public const int RaceSucFieldNumber = 8;
    private uint raceSuc_;
    /// <summary>
    ///种族成功率
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint RaceSuc {
      get { return raceSuc_; }
      set {
        raceSuc_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (TaskId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(TaskId);
      }
      petInfos_.WriteTo(output, _repeated_petInfos_codec);
      if (EndTick != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(EndTick);
      }
      if (AwardState != 0) {
        output.WriteRawTag(32);
        output.WriteUInt32(AwardState);
      }
      if (LevelSuc != 0) {
        output.WriteRawTag(40);
        output.WriteUInt32(LevelSuc);
      }
      if (ScoreSuc != 0) {
        output.WriteRawTag(48);
        output.WriteUInt32(ScoreSuc);
      }
      if (ElementSuc != 0) {
        output.WriteRawTag(56);
        output.WriteUInt32(ElementSuc);
      }
      if (RaceSuc != 0) {
        output.WriteRawTag(64);
        output.WriteUInt32(RaceSuc);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (TaskId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(TaskId);
      }
      size += petInfos_.CalculateSize(_repeated_petInfos_codec);
      if (EndTick != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(EndTick);
      }
      if (AwardState != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(AwardState);
      }
      if (LevelSuc != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(LevelSuc);
      }
      if (ScoreSuc != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ScoreSuc);
      }
      if (ElementSuc != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ElementSuc);
      }
      if (RaceSuc != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(RaceSuc);
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
            TaskId = input.ReadUInt32();
            break;
          }
          case 18: {
            petInfos_.AddEntriesFrom(input, _repeated_petInfos_codec);
            break;
          }
          case 24: {
            EndTick = input.ReadUInt32();
            break;
          }
          case 32: {
            AwardState = input.ReadUInt32();
            break;
          }
          case 40: {
            LevelSuc = input.ReadUInt32();
            break;
          }
          case 48: {
            ScoreSuc = input.ReadUInt32();
            break;
          }
          case 56: {
            ElementSuc = input.ReadUInt32();
            break;
          }
          case 64: {
            RaceSuc = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CmdPetExploreInfoNtf : pb::IMessage {
    private static readonly pb::MessageParser<CmdPetExploreInfoNtf> _parser = new pb::MessageParser<CmdPetExploreInfoNtf>(() => new CmdPetExploreInfoNtf());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdPetExploreInfoNtf> Parser { get { return _parser; } }

    /// <summary>Field number for the "actId" field.</summary>
    public const int ActIdFieldNumber = 1;
    private uint actId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ActId {
      get { return actId_; }
      set {
        actId_ = value;
      }
    }

    /// <summary>Field number for the "explorePoint" field.</summary>
    public const int ExplorePointFieldNumber = 2;
    private uint explorePoint_;
    /// <summary>
    ///冒险点数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ExplorePoint {
      get { return explorePoint_; }
      set {
        explorePoint_ = value;
      }
    }

    /// <summary>Field number for the "pointAwardState" field.</summary>
    public const int PointAwardStateFieldNumber = 3;
    private uint pointAwardState_;
    /// <summary>
    ///冒险点数奖励领取状态 低位到高位依次对应0,1,2...状态
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint PointAwardState {
      get { return pointAwardState_; }
      set {
        pointAwardState_ = value;
      }
    }

    /// <summary>Field number for the "toStartTask" field.</summary>
    public const int ToStartTaskFieldNumber = 4;
    private static readonly pb::FieldCodec<uint> _repeated_toStartTask_codec
        = pb::FieldCodec.ForUInt32(34);
    private readonly pbc::RepeatedField<uint> toStartTask_ = new pbc::RepeatedField<uint>();
    /// <summary>
    ///未开始的探险任务Id taskId = 配置表任务Id*1000+活动天数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<uint> ToStartTask {
      get { return toStartTask_; }
    }

    /// <summary>Field number for the "taskInfos" field.</summary>
    public const int TaskInfosFieldNumber = 5;
    private static readonly pb::FieldCodec<global::Packet.ExploreTaskInfo> _repeated_taskInfos_codec
        = pb::FieldCodec.ForMessage(42, global::Packet.ExploreTaskInfo.Parser);
    private readonly pbc::RepeatedField<global::Packet.ExploreTaskInfo> taskInfos_ = new pbc::RepeatedField<global::Packet.ExploreTaskInfo>();
    /// <summary>
    ///已经开始的任务和未领取的任务
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Packet.ExploreTaskInfo> TaskInfos {
      get { return taskInfos_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ActId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(ActId);
      }
      if (ExplorePoint != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(ExplorePoint);
      }
      if (PointAwardState != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(PointAwardState);
      }
      toStartTask_.WriteTo(output, _repeated_toStartTask_codec);
      taskInfos_.WriteTo(output, _repeated_taskInfos_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ActId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ActId);
      }
      if (ExplorePoint != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ExplorePoint);
      }
      if (PointAwardState != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(PointAwardState);
      }
      size += toStartTask_.CalculateSize(_repeated_toStartTask_codec);
      size += taskInfos_.CalculateSize(_repeated_taskInfos_codec);
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
            ActId = input.ReadUInt32();
            break;
          }
          case 16: {
            ExplorePoint = input.ReadUInt32();
            break;
          }
          case 24: {
            PointAwardState = input.ReadUInt32();
            break;
          }
          case 34:
          case 32: {
            toStartTask_.AddEntriesFrom(input, _repeated_toStartTask_codec);
            break;
          }
          case 42: {
            taskInfos_.AddEntriesFrom(input, _repeated_taskInfos_codec);
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///开始探险
  /// </summary>
  public sealed class CmdPetExploreStartReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdPetExploreStartReq> _parser = new pb::MessageParser<CmdPetExploreStartReq>(() => new CmdPetExploreStartReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdPetExploreStartReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "actId" field.</summary>
    public const int ActIdFieldNumber = 1;
    private uint actId_;
    /// <summary>
    ///taskId = 配置表任务Id*1000+活动天数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ActId {
      get { return actId_; }
      set {
        actId_ = value;
      }
    }

    /// <summary>Field number for the "taskId" field.</summary>
    public const int TaskIdFieldNumber = 2;
    private uint taskId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint TaskId {
      get { return taskId_; }
      set {
        taskId_ = value;
      }
    }

    /// <summary>Field number for the "petList" field.</summary>
    public const int PetListFieldNumber = 3;
    private static readonly pb::FieldCodec<uint> _repeated_petList_codec
        = pb::FieldCodec.ForUInt32(26);
    private readonly pbc::RepeatedField<uint> petList_ = new pbc::RepeatedField<uint>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<uint> PetList {
      get { return petList_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ActId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(ActId);
      }
      if (TaskId != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(TaskId);
      }
      petList_.WriteTo(output, _repeated_petList_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ActId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ActId);
      }
      if (TaskId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(TaskId);
      }
      size += petList_.CalculateSize(_repeated_petList_codec);
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
            ActId = input.ReadUInt32();
            break;
          }
          case 16: {
            TaskId = input.ReadUInt32();
            break;
          }
          case 26:
          case 24: {
            petList_.AddEntriesFrom(input, _repeated_petList_codec);
            break;
          }
        }
      }
    }

  }

  public sealed class CmdPetExploreStartRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdPetExploreStartRes> _parser = new pb::MessageParser<CmdPetExploreStartRes>(() => new CmdPetExploreStartRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdPetExploreStartRes> Parser { get { return _parser; } }

    /// <summary>Field number for the "actId" field.</summary>
    public const int ActIdFieldNumber = 1;
    private uint actId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ActId {
      get { return actId_; }
      set {
        actId_ = value;
      }
    }

    /// <summary>Field number for the "taskInfo" field.</summary>
    public const int TaskInfoFieldNumber = 2;
    private global::Packet.ExploreTaskInfo taskInfo_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Packet.ExploreTaskInfo TaskInfo {
      get { return taskInfo_; }
      set {
        taskInfo_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ActId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(ActId);
      }
      if (taskInfo_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(TaskInfo);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ActId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ActId);
      }
      if (taskInfo_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(TaskInfo);
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
            ActId = input.ReadUInt32();
            break;
          }
          case 18: {
            if (taskInfo_ == null) {
              taskInfo_ = new global::Packet.ExploreTaskInfo();
            }
            input.ReadMessage(taskInfo_);
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///领取探险奖励
  /// </summary>
  public sealed class CmdPetExploreFinishGetAwardReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdPetExploreFinishGetAwardReq> _parser = new pb::MessageParser<CmdPetExploreFinishGetAwardReq>(() => new CmdPetExploreFinishGetAwardReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdPetExploreFinishGetAwardReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "actId" field.</summary>
    public const int ActIdFieldNumber = 1;
    private uint actId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ActId {
      get { return actId_; }
      set {
        actId_ = value;
      }
    }

    /// <summary>Field number for the "taskId" field.</summary>
    public const int TaskIdFieldNumber = 2;
    private uint taskId_;
    /// <summary>
    ///taskId = 配置表任务Id*1000+活动天数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint TaskId {
      get { return taskId_; }
      set {
        taskId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ActId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(ActId);
      }
      if (TaskId != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(TaskId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ActId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ActId);
      }
      if (TaskId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(TaskId);
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
            ActId = input.ReadUInt32();
            break;
          }
          case 16: {
            TaskId = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CmdPetExploreFinishGetAwardRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdPetExploreFinishGetAwardRes> _parser = new pb::MessageParser<CmdPetExploreFinishGetAwardRes>(() => new CmdPetExploreFinishGetAwardRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdPetExploreFinishGetAwardRes> Parser { get { return _parser; } }

    /// <summary>Field number for the "actId" field.</summary>
    public const int ActIdFieldNumber = 1;
    private uint actId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ActId {
      get { return actId_; }
      set {
        actId_ = value;
      }
    }

    /// <summary>Field number for the "taskId" field.</summary>
    public const int TaskIdFieldNumber = 2;
    private uint taskId_;
    /// <summary>
    ///taskId = 配置表任务Id*1000+活动天数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint TaskId {
      get { return taskId_; }
      set {
        taskId_ = value;
      }
    }

    /// <summary>Field number for the "awardState" field.</summary>
    public const int AwardStateFieldNumber = 3;
    private uint awardState_;
    /// <summary>
    ///奖励标记 enum ExploreAwardState
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint AwardState {
      get { return awardState_; }
      set {
        awardState_ = value;
      }
    }

    /// <summary>Field number for the "items" field.</summary>
    public const int ItemsFieldNumber = 4;
    private static readonly pb::FieldCodec<global::Packet.SimpleItem> _repeated_items_codec
        = pb::FieldCodec.ForMessage(34, global::Packet.SimpleItem.Parser);
    private readonly pbc::RepeatedField<global::Packet.SimpleItem> items_ = new pbc::RepeatedField<global::Packet.SimpleItem>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Packet.SimpleItem> Items {
      get { return items_; }
    }

    /// <summary>Field number for the "explorePoint" field.</summary>
    public const int ExplorePointFieldNumber = 5;
    private uint explorePoint_;
    /// <summary>
    ///冒险点数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ExplorePoint {
      get { return explorePoint_; }
      set {
        explorePoint_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ActId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(ActId);
      }
      if (TaskId != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(TaskId);
      }
      if (AwardState != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(AwardState);
      }
      items_.WriteTo(output, _repeated_items_codec);
      if (ExplorePoint != 0) {
        output.WriteRawTag(40);
        output.WriteUInt32(ExplorePoint);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ActId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ActId);
      }
      if (TaskId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(TaskId);
      }
      if (AwardState != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(AwardState);
      }
      size += items_.CalculateSize(_repeated_items_codec);
      if (ExplorePoint != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ExplorePoint);
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
            ActId = input.ReadUInt32();
            break;
          }
          case 16: {
            TaskId = input.ReadUInt32();
            break;
          }
          case 24: {
            AwardState = input.ReadUInt32();
            break;
          }
          case 34: {
            items_.AddEntriesFrom(input, _repeated_items_codec);
            break;
          }
          case 40: {
            ExplorePoint = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///领取冒险点奖励
  /// </summary>
  public sealed class CmdPetExploreGetPointAwardReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdPetExploreGetPointAwardReq> _parser = new pb::MessageParser<CmdPetExploreGetPointAwardReq>(() => new CmdPetExploreGetPointAwardReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdPetExploreGetPointAwardReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "actId" field.</summary>
    public const int ActIdFieldNumber = 1;
    private uint actId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ActId {
      get { return actId_; }
      set {
        actId_ = value;
      }
    }

    /// <summary>Field number for the "index" field.</summary>
    public const int IndexFieldNumber = 2;
    private uint index_;
    /// <summary>
    ///0开始
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Index {
      get { return index_; }
      set {
        index_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ActId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(ActId);
      }
      if (Index != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Index);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ActId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ActId);
      }
      if (Index != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Index);
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
            ActId = input.ReadUInt32();
            break;
          }
          case 16: {
            Index = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CmdPetExploreGetPointAwardRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdPetExploreGetPointAwardRes> _parser = new pb::MessageParser<CmdPetExploreGetPointAwardRes>(() => new CmdPetExploreGetPointAwardRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdPetExploreGetPointAwardRes> Parser { get { return _parser; } }

    /// <summary>Field number for the "actId" field.</summary>
    public const int ActIdFieldNumber = 1;
    private uint actId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ActId {
      get { return actId_; }
      set {
        actId_ = value;
      }
    }

    /// <summary>Field number for the "scoreAwardState" field.</summary>
    public const int ScoreAwardStateFieldNumber = 2;
    private uint scoreAwardState_;
    /// <summary>
    ///低位到高位依次对应0,1,2...状态
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ScoreAwardState {
      get { return scoreAwardState_; }
      set {
        scoreAwardState_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ActId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(ActId);
      }
      if (ScoreAwardState != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(ScoreAwardState);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ActId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ActId);
      }
      if (ScoreAwardState != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ScoreAwardState);
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
            ActId = input.ReadUInt32();
            break;
          }
          case 16: {
            ScoreAwardState = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
