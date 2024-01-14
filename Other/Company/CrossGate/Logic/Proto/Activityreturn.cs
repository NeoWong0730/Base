// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: activityreturn.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Packet {

  #region Enums
  /// <summary>
  /// 9300+
  /// </summary>
  public enum CmdActivityReturn {
    None = 0,
    /// <summary>
    ///回归活动信息
    /// </summary>
    InfoNtf = 9301,
    /// <summary>
    ///请求领取回归赠礼
    /// </summary>
    TakeGiftsReq = 9302,
    TakeGiftsRes = 9303,
    /// <summary>
    ///回归试炼任务更新
    /// </summary>
    TaskUpdateNtf = 9304,
    /// <summary>
    ///回归试炼任务领取奖励
    /// </summary>
    TaskRewardReq = 9305,
    TaskRewardRes = 9306,
    /// <summary>
    ///回归试炼任务领取积分奖励
    /// </summary>
    TaskPointRewardReq = 9307,
    TaskPointRewardRes = 9308,
    /// <summary>
    /// 回归爱心值信息请求
    /// </summary>
    LovePointDataReq = 9309,
    /// <summary>
    /// 回归爱心值信息返回
    /// </summary>
    LovePointDataRes = 9310,
    /// <summary>
    /// 回归爱心值奖励领取
    /// </summary>
    LovePointRewardReq = 9311,
    /// <summary>
    /// 回归爱心值奖励返回
    /// </summary>
    LovePointRewardRes = 9312,
  }

  public enum ErrorActivityReturn {
    /// <summary>
    ///正常
    /// </summary>
    None = 0,
    /// <summary>
    ///配置错误
    /// </summary>
    CsvErr = 9301,
    /// <summary>
    /// 活动未开启
    /// </summary>
    NotOpen = 9302,
    /// <summary>
    /// 回归赠礼已领取
    /// </summary>
    GiftsTake = 9303,
    /// <summary>
    /// 回归试炼任务未完成
    /// </summary>
    TaskUnfinished = 9304,
    /// <summary>
    /// 回归试炼奖励已领取
    /// </summary>
    TaskTake = 9305,
    /// <summary>
    /// 回归试炼请求id错误
    /// </summary>
    TaskIdErr = 9306,
    /// <summary>
    /// 回归试炼积分不足
    /// </summary>
    NoPoint = 9307,
    /// <summary>
    /// 请求活动天数错误
    /// </summary>
    ActDay = 9308,
    /// <summary>
    /// 回归爱心值奖励兑换上限
    /// </summary>
    LovePointRewardMax = 9309,
    /// <summary>
    /// 回归爱心值不足
    /// </summary>
    LovePointLess = 9310,
    /// <summary>
    /// 玩家等级不合法
    /// </summary>
    RoleLevelErr = 9311,
    /// <summary>
    /// 回归爱心值已达上限
    /// </summary>
    LovePointMax = 9312,
  }

  public enum ActivityReturnActiveReason {
    None = 0,
    /// <summary>
    ///回归赠礼
    /// </summary>
    Gifts = 9301,
    /// <summary>
    ///回归试炼
    /// </summary>
    Task = 9302,
    /// <summary>
    ///回归爱心值兑换
    /// </summary>
    LovePoint = 9303,
  }

  #endregion

  #region Messages
  /// <summary>
  ///回归活动信息
  /// </summary>
  public sealed class CmdActivityReturnInfoNtf : pb::IMessage {
    private static readonly pb::MessageParser<CmdActivityReturnInfoNtf> _parser = new pb::MessageParser<CmdActivityReturnInfoNtf>(() => new CmdActivityReturnInfoNtf());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdActivityReturnInfoNtf> Parser { get { return _parser; } }

    /// <summary>Field number for the "openTime" field.</summary>
    public const int OpenTimeFieldNumber = 1;
    private uint openTime_;
    /// <summary>
    ///活动开始时间，0为未开启
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint OpenTime {
      get { return openTime_; }
      set {
        openTime_ = value;
      }
    }

    /// <summary>Field number for the "activityGroup" field.</summary>
    public const int ActivityGroupFieldNumber = 2;
    private uint activityGroup_;
    /// <summary>
    ///所在分组
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ActivityGroup {
      get { return activityGroup_; }
      set {
        activityGroup_ = value;
      }
    }

    /// <summary>Field number for the "giftsData" field.</summary>
    public const int GiftsDataFieldNumber = 3;
    private global::Packet.GiftsData giftsData_;
    /// <summary>
    ///回归赠礼
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Packet.GiftsData GiftsData {
      get { return giftsData_; }
      set {
        giftsData_ = value;
      }
    }

    /// <summary>Field number for the "returnTaskData" field.</summary>
    public const int ReturnTaskDataFieldNumber = 4;
    private global::Packet.ReturnTaskData returnTaskData_;
    /// <summary>
    ///回归试炼
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Packet.ReturnTaskData ReturnTaskData {
      get { return returnTaskData_; }
      set {
        returnTaskData_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (OpenTime != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(OpenTime);
      }
      if (ActivityGroup != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(ActivityGroup);
      }
      if (giftsData_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(GiftsData);
      }
      if (returnTaskData_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(ReturnTaskData);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (OpenTime != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(OpenTime);
      }
      if (ActivityGroup != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ActivityGroup);
      }
      if (giftsData_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(GiftsData);
      }
      if (returnTaskData_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(ReturnTaskData);
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
            OpenTime = input.ReadUInt32();
            break;
          }
          case 16: {
            ActivityGroup = input.ReadUInt32();
            break;
          }
          case 26: {
            if (giftsData_ == null) {
              giftsData_ = new global::Packet.GiftsData();
            }
            input.ReadMessage(giftsData_);
            break;
          }
          case 34: {
            if (returnTaskData_ == null) {
              returnTaskData_ = new global::Packet.ReturnTaskData();
            }
            input.ReadMessage(returnTaskData_);
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///回归赠礼
  /// </summary>
  public sealed class GiftsData : pb::IMessage {
    private static readonly pb::MessageParser<GiftsData> _parser = new pb::MessageParser<GiftsData>(() => new GiftsData());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<GiftsData> Parser { get { return _parser; } }

    /// <summary>Field number for the "lastOffline" field.</summary>
    public const int LastOfflineFieldNumber = 1;
    private uint lastOffline_;
    /// <summary>
    ///上次离线时间
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint LastOffline {
      get { return lastOffline_; }
      set {
        lastOffline_ = value;
      }
    }

    /// <summary>Field number for the "takeReward" field.</summary>
    public const int TakeRewardFieldNumber = 2;
    private bool takeReward_;
    /// <summary>
    ///false-未领取，true-已领取
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool TakeReward {
      get { return takeReward_; }
      set {
        takeReward_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (LastOffline != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(LastOffline);
      }
      if (TakeReward != false) {
        output.WriteRawTag(16);
        output.WriteBool(TakeReward);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (LastOffline != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(LastOffline);
      }
      if (TakeReward != false) {
        size += 1 + 1;
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
            LastOffline = input.ReadUInt32();
            break;
          }
          case 16: {
            TakeReward = input.ReadBool();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///请求领取回归赠礼
  /// </summary>
  public sealed class CmdActivityReturnTakeGiftsReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdActivityReturnTakeGiftsReq> _parser = new pb::MessageParser<CmdActivityReturnTakeGiftsReq>(() => new CmdActivityReturnTakeGiftsReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdActivityReturnTakeGiftsReq> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
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
        }
      }
    }

  }

  public sealed class CmdActivityReturnTakeGiftsRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdActivityReturnTakeGiftsRes> _parser = new pb::MessageParser<CmdActivityReturnTakeGiftsRes>(() => new CmdActivityReturnTakeGiftsRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdActivityReturnTakeGiftsRes> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
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
        }
      }
    }

  }

  /// <summary>
  ///回归试炼
  /// </summary>
  public sealed class ReturnTaskData : pb::IMessage {
    private static readonly pb::MessageParser<ReturnTaskData> _parser = new pb::MessageParser<ReturnTaskData>(() => new ReturnTaskData());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ReturnTaskData> Parser { get { return _parser; } }

    /// <summary>Field number for the "taskList" field.</summary>
    public const int TaskListFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Packet.ReturnTaskData.Types.TaskItem> _repeated_taskList_codec
        = pb::FieldCodec.ForMessage(10, global::Packet.ReturnTaskData.Types.TaskItem.Parser);
    private readonly pbc::RepeatedField<global::Packet.ReturnTaskData.Types.TaskItem> taskList_ = new pbc::RepeatedField<global::Packet.ReturnTaskData.Types.TaskItem>();
    /// <summary>
    ///任务列表
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Packet.ReturnTaskData.Types.TaskItem> TaskList {
      get { return taskList_; }
    }

    /// <summary>Field number for the "point" field.</summary>
    public const int PointFieldNumber = 2;
    private uint point_;
    /// <summary>
    ///总积分
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Point {
      get { return point_; }
      set {
        point_ = value;
      }
    }

    /// <summary>Field number for the "pointReward" field.</summary>
    public const int PointRewardFieldNumber = 3;
    private static readonly pb::FieldCodec<uint> _repeated_pointReward_codec
        = pb::FieldCodec.ForUInt32(26);
    private readonly pbc::RepeatedField<uint> pointReward_ = new pbc::RepeatedField<uint>();
    /// <summary>
    ///已领取的积分奖励id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<uint> PointReward {
      get { return pointReward_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      taskList_.WriteTo(output, _repeated_taskList_codec);
      if (Point != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Point);
      }
      pointReward_.WriteTo(output, _repeated_pointReward_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += taskList_.CalculateSize(_repeated_taskList_codec);
      if (Point != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Point);
      }
      size += pointReward_.CalculateSize(_repeated_pointReward_codec);
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
            taskList_.AddEntriesFrom(input, _repeated_taskList_codec);
            break;
          }
          case 16: {
            Point = input.ReadUInt32();
            break;
          }
          case 26:
          case 24: {
            pointReward_.AddEntriesFrom(input, _repeated_pointReward_codec);
            break;
          }
        }
      }
    }

    #region Nested types
    /// <summary>Container for nested types declared in the ReturnTaskData message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static class Types {
      public sealed class TaskItem : pb::IMessage {
        private static readonly pb::MessageParser<TaskItem> _parser = new pb::MessageParser<TaskItem>(() => new TaskItem());
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<TaskItem> Parser { get { return _parser; } }

        /// <summary>Field number for the "taskId" field.</summary>
        public const int TaskIdFieldNumber = 1;
        private uint taskId_;
        /// <summary>
        ///id
        /// </summary>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public uint TaskId {
          get { return taskId_; }
          set {
            taskId_ = value;
          }
        }

        /// <summary>Field number for the "num" field.</summary>
        public const int NumFieldNumber = 2;
        private uint num_;
        /// <summary>
        ///完成的数量
        /// </summary>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public uint Num {
          get { return num_; }
          set {
            num_ = value;
          }
        }

        /// <summary>Field number for the "takeReward" field.</summary>
        public const int TakeRewardFieldNumber = 3;
        private bool takeReward_;
        /// <summary>
        ///false-未领取，true-已领取
        /// </summary>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TakeReward {
          get { return takeReward_; }
          set {
            takeReward_ = value;
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void WriteTo(pb::CodedOutputStream output) {
          if (TaskId != 0) {
            output.WriteRawTag(8);
            output.WriteUInt32(TaskId);
          }
          if (Num != 0) {
            output.WriteRawTag(16);
            output.WriteUInt32(Num);
          }
          if (TakeReward != false) {
            output.WriteRawTag(24);
            output.WriteBool(TakeReward);
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int CalculateSize() {
          int size = 0;
          if (TaskId != 0) {
            size += 1 + pb::CodedOutputStream.ComputeUInt32Size(TaskId);
          }
          if (Num != 0) {
            size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Num);
          }
          if (TakeReward != false) {
            size += 1 + 1;
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
              case 16: {
                Num = input.ReadUInt32();
                break;
              }
              case 24: {
                TakeReward = input.ReadBool();
                break;
              }
            }
          }
        }

      }

    }
    #endregion

  }

  /// <summary>
  ///回归试炼任务更新
  /// </summary>
  public sealed class CmdActivityReturnTaskUpdateNtf : pb::IMessage {
    private static readonly pb::MessageParser<CmdActivityReturnTaskUpdateNtf> _parser = new pb::MessageParser<CmdActivityReturnTaskUpdateNtf>(() => new CmdActivityReturnTaskUpdateNtf());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdActivityReturnTaskUpdateNtf> Parser { get { return _parser; } }

    /// <summary>Field number for the "taskId" field.</summary>
    public const int TaskIdFieldNumber = 1;
    private uint taskId_;
    /// <summary>
    ///id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint TaskId {
      get { return taskId_; }
      set {
        taskId_ = value;
      }
    }

    /// <summary>Field number for the "num" field.</summary>
    public const int NumFieldNumber = 2;
    private uint num_;
    /// <summary>
    ///完成的数量
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Num {
      get { return num_; }
      set {
        num_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (TaskId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(TaskId);
      }
      if (Num != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Num);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (TaskId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(TaskId);
      }
      if (Num != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Num);
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
          case 16: {
            Num = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///回归试炼任务领取奖励
  /// </summary>
  public sealed class CmdActivityReturnTaskRewardReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdActivityReturnTaskRewardReq> _parser = new pb::MessageParser<CmdActivityReturnTaskRewardReq>(() => new CmdActivityReturnTaskRewardReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdActivityReturnTaskRewardReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "taskId" field.</summary>
    public const int TaskIdFieldNumber = 1;
    private uint taskId_;
    /// <summary>
    ///id
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
      if (TaskId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(TaskId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
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
            TaskId = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CmdActivityReturnTaskRewardRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdActivityReturnTaskRewardRes> _parser = new pb::MessageParser<CmdActivityReturnTaskRewardRes>(() => new CmdActivityReturnTaskRewardRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdActivityReturnTaskRewardRes> Parser { get { return _parser; } }

    /// <summary>Field number for the "taskId" field.</summary>
    public const int TaskIdFieldNumber = 1;
    private uint taskId_;
    /// <summary>
    ///id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint TaskId {
      get { return taskId_; }
      set {
        taskId_ = value;
      }
    }

    /// <summary>Field number for the "takeReward" field.</summary>
    public const int TakeRewardFieldNumber = 2;
    private bool takeReward_;
    /// <summary>
    ///false-未领取，true-已领取
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool TakeReward {
      get { return takeReward_; }
      set {
        takeReward_ = value;
      }
    }

    /// <summary>Field number for the "point" field.</summary>
    public const int PointFieldNumber = 3;
    private uint point_;
    /// <summary>
    ///总积分
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Point {
      get { return point_; }
      set {
        point_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (TaskId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(TaskId);
      }
      if (TakeReward != false) {
        output.WriteRawTag(16);
        output.WriteBool(TakeReward);
      }
      if (Point != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(Point);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (TaskId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(TaskId);
      }
      if (TakeReward != false) {
        size += 1 + 1;
      }
      if (Point != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Point);
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
          case 16: {
            TakeReward = input.ReadBool();
            break;
          }
          case 24: {
            Point = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///回归试炼任务领取积分奖励
  /// </summary>
  public sealed class CmdActivityReturnTaskPointRewardReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdActivityReturnTaskPointRewardReq> _parser = new pb::MessageParser<CmdActivityReturnTaskPointRewardReq>(() => new CmdActivityReturnTaskPointRewardReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdActivityReturnTaskPointRewardReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "infoId" field.</summary>
    public const int InfoIdFieldNumber = 1;
    private uint infoId_;
    /// <summary>
    ///id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint InfoId {
      get { return infoId_; }
      set {
        infoId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (InfoId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(InfoId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (InfoId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(InfoId);
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
            InfoId = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CmdActivityReturnTaskPointRewardRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdActivityReturnTaskPointRewardRes> _parser = new pb::MessageParser<CmdActivityReturnTaskPointRewardRes>(() => new CmdActivityReturnTaskPointRewardRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdActivityReturnTaskPointRewardRes> Parser { get { return _parser; } }

    /// <summary>Field number for the "infoId" field.</summary>
    public const int InfoIdFieldNumber = 1;
    private uint infoId_;
    /// <summary>
    ///id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint InfoId {
      get { return infoId_; }
      set {
        infoId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (InfoId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(InfoId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (InfoId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(InfoId);
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
            InfoId = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CmdActivityReturnLovePointDataReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdActivityReturnLovePointDataReq> _parser = new pb::MessageParser<CmdActivityReturnLovePointDataReq>(() => new CmdActivityReturnLovePointDataReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdActivityReturnLovePointDataReq> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
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
        }
      }
    }

  }

  public sealed class CmdActivityReturnLovePointDataRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdActivityReturnLovePointDataRes> _parser = new pb::MessageParser<CmdActivityReturnLovePointDataRes>(() => new CmdActivityReturnLovePointDataRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdActivityReturnLovePointDataRes> Parser { get { return _parser; } }

    /// <summary>Field number for the "lovePoint" field.</summary>
    public const int LovePointFieldNumber = 1;
    private uint lovePoint_;
    /// <summary>
    /// 回归爱心值(总)
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint LovePoint {
      get { return lovePoint_; }
      set {
        lovePoint_ = value;
      }
    }

    /// <summary>Field number for the "dailyList" field.</summary>
    public const int DailyListFieldNumber = 2;
    private static readonly pb::FieldCodec<global::Packet.CmdActivityReturnLovePointDataRes.Types.pointDefine> _repeated_dailyList_codec
        = pb::FieldCodec.ForMessage(18, global::Packet.CmdActivityReturnLovePointDataRes.Types.pointDefine.Parser);
    private readonly pbc::RepeatedField<global::Packet.CmdActivityReturnLovePointDataRes.Types.pointDefine> dailyList_ = new pbc::RepeatedField<global::Packet.CmdActivityReturnLovePointDataRes.Types.pointDefine>();
    /// <summary>
    /// 回归爱心值(每日明细)
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Packet.CmdActivityReturnLovePointDataRes.Types.pointDefine> DailyList {
      get { return dailyList_; }
    }

    /// <summary>Field number for the "rewards" field.</summary>
    public const int RewardsFieldNumber = 3;
    private static readonly pb::FieldCodec<global::Packet.CmdActivityReturnLovePointDataRes.Types.pointDefine> _repeated_rewards_codec
        = pb::FieldCodec.ForMessage(26, global::Packet.CmdActivityReturnLovePointDataRes.Types.pointDefine.Parser);
    private readonly pbc::RepeatedField<global::Packet.CmdActivityReturnLovePointDataRes.Types.pointDefine> rewards_ = new pbc::RepeatedField<global::Packet.CmdActivityReturnLovePointDataRes.Types.pointDefine>();
    /// <summary>
    /// 奖励购买次数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Packet.CmdActivityReturnLovePointDataRes.Types.pointDefine> Rewards {
      get { return rewards_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (LovePoint != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(LovePoint);
      }
      dailyList_.WriteTo(output, _repeated_dailyList_codec);
      rewards_.WriteTo(output, _repeated_rewards_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (LovePoint != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(LovePoint);
      }
      size += dailyList_.CalculateSize(_repeated_dailyList_codec);
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
            LovePoint = input.ReadUInt32();
            break;
          }
          case 18: {
            dailyList_.AddEntriesFrom(input, _repeated_dailyList_codec);
            break;
          }
          case 26: {
            rewards_.AddEntriesFrom(input, _repeated_rewards_codec);
            break;
          }
        }
      }
    }

    #region Nested types
    /// <summary>Container for nested types declared in the CmdActivityReturnLovePointDataRes message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static class Types {
      public sealed class pointDefine : pb::IMessage {
        private static readonly pb::MessageParser<pointDefine> _parser = new pb::MessageParser<pointDefine>(() => new pointDefine());
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<pointDefine> Parser { get { return _parser; } }

        /// <summary>Field number for the "id" field.</summary>
        public const int IdFieldNumber = 1;
        private uint id_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public uint Id {
          get { return id_; }
          set {
            id_ = value;
          }
        }

        /// <summary>Field number for the "value" field.</summary>
        public const int ValueFieldNumber = 2;
        private uint value_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public uint Value {
          get { return value_; }
          set {
            value_ = value;
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void WriteTo(pb::CodedOutputStream output) {
          if (Id != 0) {
            output.WriteRawTag(8);
            output.WriteUInt32(Id);
          }
          if (Value != 0) {
            output.WriteRawTag(16);
            output.WriteUInt32(Value);
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int CalculateSize() {
          int size = 0;
          if (Id != 0) {
            size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Id);
          }
          if (Value != 0) {
            size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Value);
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
                Id = input.ReadUInt32();
                break;
              }
              case 16: {
                Value = input.ReadUInt32();
                break;
              }
            }
          }
        }

      }

    }
    #endregion

  }

  public sealed class CmdActivityReturnLovePointRewardReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdActivityReturnLovePointRewardReq> _parser = new pb::MessageParser<CmdActivityReturnLovePointRewardReq>(() => new CmdActivityReturnLovePointRewardReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdActivityReturnLovePointRewardReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "index" field.</summary>
    public const int IndexFieldNumber = 1;
    private uint index_;
    /// <summary>
    /// 奖励下标
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
      if (Index != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Index);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
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
            Index = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CmdActivityReturnLovePointRewardRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdActivityReturnLovePointRewardRes> _parser = new pb::MessageParser<CmdActivityReturnLovePointRewardRes>(() => new CmdActivityReturnLovePointRewardRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdActivityReturnLovePointRewardRes> Parser { get { return _parser; } }

    /// <summary>Field number for the "index" field.</summary>
    public const int IndexFieldNumber = 1;
    private uint index_;
    /// <summary>
    /// 奖励下标
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Index {
      get { return index_; }
      set {
        index_ = value;
      }
    }

    /// <summary>Field number for the "lovePoint" field.</summary>
    public const int LovePointFieldNumber = 2;
    private uint lovePoint_;
    /// <summary>
    /// 回归爱心值(总)
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint LovePoint {
      get { return lovePoint_; }
      set {
        lovePoint_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Index != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Index);
      }
      if (LovePoint != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(LovePoint);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Index != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Index);
      }
      if (LovePoint != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(LovePoint);
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
            Index = input.ReadUInt32();
            break;
          }
          case 16: {
            LovePoint = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
