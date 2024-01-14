// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: scene.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Client {

  #region Enums
  public enum enSecondScene {
    None = 0,
    /// <summary>
    ///进入地图第一条消息
    /// </summary>
    NtfIntoFirst = 1,
    /// <summary>
    ///主角全量数据(数据过大时会发多条)
    /// </summary>
    NtfModuleData = 2,
    /// <summary>
    ///主角数据推送完毕
    /// </summary>
    NtfIntoFinish = 3,
    /// <summary>
    ///视野内所有物件(排除自己)
    /// </summary>
    NtfViewAll = 4,
    /// <summary>
    ///视野添加物件
    /// </summary>
    NtfViewAdd = 5,
    /// <summary>
    ///视野删除物件
    /// </summary>
    NtfViewRemove = 6,
    /// <summary>
    ///访问npc
    /// </summary>
    ReqVisitNpc = 7,
    /// <summary>
    ///对话结束
    /// </summary>
    ReqCloseChat = 8,
    /// <summary>
    ///实际最大
    /// </summary>
    RealMax = 9,
  }

  #endregion

  #region Messages
  public sealed class SecondScene_Ntf_IntoFirst : pb::IMessage {
    private static readonly pb::MessageParser<SecondScene_Ntf_IntoFirst> _parser = new pb::MessageParser<SecondScene_Ntf_IntoFirst>(() => new SecondScene_Ntf_IntoFirst());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SecondScene_Ntf_IntoFirst> Parser { get { return _parser; } }

    /// <summary>Field number for the "sceneid" field.</summary>
    public const int SceneidFieldNumber = 1;
    private uint sceneid_;
    /// <summary>
    ///场景Id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Sceneid {
      get { return sceneid_; }
      set {
        sceneid_ = value;
      }
    }

    /// <summary>Field number for the "direct" field.</summary>
    public const int DirectFieldNumber = 2;
    private uint direct_;
    /// <summary>
    ///方向
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Direct {
      get { return direct_; }
      set {
        direct_ = value;
      }
    }

    /// <summary>Field number for the "pos" field.</summary>
    public const int PosFieldNumber = 3;
    private global::Common.ClientPos pos_;
    /// <summary>
    ///坐标
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Common.ClientPos Pos {
      get { return pos_; }
      set {
        pos_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Sceneid != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Sceneid);
      }
      if (Direct != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Direct);
      }
      if (pos_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(Pos);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Sceneid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Sceneid);
      }
      if (Direct != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Direct);
      }
      if (pos_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Pos);
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
            Sceneid = input.ReadUInt32();
            break;
          }
          case 16: {
            Direct = input.ReadUInt32();
            break;
          }
          case 26: {
            if (pos_ == null) {
              pos_ = new global::Common.ClientPos();
            }
            input.ReadMessage(pos_);
            break;
          }
        }
      }
    }

  }

  public sealed class SecondScene_Ntf_ModuleData : pb::IMessage {
    private static readonly pb::MessageParser<SecondScene_Ntf_ModuleData> _parser = new pb::MessageParser<SecondScene_Ntf_ModuleData>(() => new SecondScene_Ntf_ModuleData());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SecondScene_Ntf_ModuleData> Parser { get { return _parser; } }

    /// <summary>Field number for the "scene" field.</summary>
    public const int SceneFieldNumber = 2;
    private global::Common.ModuleScene scene_;
    /// <summary>
    /// common.ModuleLogin       login   = 1;//登陆
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Common.ModuleScene Scene {
      get { return scene_; }
      set {
        scene_ = value;
      }
    }

    /// <summary>Field number for the "item" field.</summary>
    public const int ItemFieldNumber = 5;
    private global::Common.ModuleItem item_;
    /// <summary>
    /// common.ModuleMove        move    = 3;//移动
    /// common.ModuleChat        chat    = 4;//聊天
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Common.ModuleItem Item {
      get { return item_; }
      set {
        item_ = value;
      }
    }

    /// <summary>Field number for the "task" field.</summary>
    public const int TaskFieldNumber = 7;
    private global::Common.ModuleTask task_;
    /// <summary>
    /// common.ModuleProp        prop    = 6;//属性
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Common.ModuleTask Task {
      get { return task_; }
      set {
        task_ = value;
      }
    }

    /// <summary>Field number for the "fight" field.</summary>
    public const int FightFieldNumber = 8;
    private global::Common.ModuleFight fight_;
    /// <summary>
    ///战斗
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Common.ModuleFight Fight {
      get { return fight_; }
      set {
        fight_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (scene_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Scene);
      }
      if (item_ != null) {
        output.WriteRawTag(42);
        output.WriteMessage(Item);
      }
      if (task_ != null) {
        output.WriteRawTag(58);
        output.WriteMessage(Task);
      }
      if (fight_ != null) {
        output.WriteRawTag(66);
        output.WriteMessage(Fight);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (scene_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Scene);
      }
      if (item_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Item);
      }
      if (task_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Task);
      }
      if (fight_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Fight);
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
          case 18: {
            if (scene_ == null) {
              scene_ = new global::Common.ModuleScene();
            }
            input.ReadMessage(scene_);
            break;
          }
          case 42: {
            if (item_ == null) {
              item_ = new global::Common.ModuleItem();
            }
            input.ReadMessage(item_);
            break;
          }
          case 58: {
            if (task_ == null) {
              task_ = new global::Common.ModuleTask();
            }
            input.ReadMessage(task_);
            break;
          }
          case 66: {
            if (fight_ == null) {
              fight_ = new global::Common.ModuleFight();
            }
            input.ReadMessage(fight_);
            break;
          }
        }
      }
    }

  }

  public sealed class SecondScene_Ntf_IntoFinish : pb::IMessage {
    private static readonly pb::MessageParser<SecondScene_Ntf_IntoFinish> _parser = new pb::MessageParser<SecondScene_Ntf_IntoFinish>(() => new SecondScene_Ntf_IntoFinish());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SecondScene_Ntf_IntoFinish> Parser { get { return _parser; } }

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

  public sealed class SecondScene_Ntf_ViewAll : pb::IMessage {
    private static readonly pb::MessageParser<SecondScene_Ntf_ViewAll> _parser = new pb::MessageParser<SecondScene_Ntf_ViewAll>(() => new SecondScene_Ntf_ViewAll());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SecondScene_Ntf_ViewAll> Parser { get { return _parser; } }

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

  public sealed class SecondScene_Ntf_ViewAdd : pb::IMessage {
    private static readonly pb::MessageParser<SecondScene_Ntf_ViewAdd> _parser = new pb::MessageParser<SecondScene_Ntf_ViewAdd>(() => new SecondScene_Ntf_ViewAdd());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SecondScene_Ntf_ViewAdd> Parser { get { return _parser; } }

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

  public sealed class SecondScene_Ntf_ViewRemove : pb::IMessage {
    private static readonly pb::MessageParser<SecondScene_Ntf_ViewRemove> _parser = new pb::MessageParser<SecondScene_Ntf_ViewRemove>(() => new SecondScene_Ntf_ViewRemove());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SecondScene_Ntf_ViewRemove> Parser { get { return _parser; } }

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

  public sealed class SecondScene_Req_VisitNpc : pb::IMessage {
    private static readonly pb::MessageParser<SecondScene_Req_VisitNpc> _parser = new pb::MessageParser<SecondScene_Req_VisitNpc>(() => new SecondScene_Req_VisitNpc());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SecondScene_Req_VisitNpc> Parser { get { return _parser; } }

    /// <summary>Field number for the "npcuid" field.</summary>
    public const int NpcuidFieldNumber = 1;
    private uint npcuid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Npcuid {
      get { return npcuid_; }
      set {
        npcuid_ = value;
      }
    }

    /// <summary>Field number for the "eventid" field.</summary>
    public const int EventidFieldNumber = 2;
    private uint eventid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Eventid {
      get { return eventid_; }
      set {
        eventid_ = value;
      }
    }

    /// <summary>Field number for the "param" field.</summary>
    public const int ParamFieldNumber = 3;
    private uint param_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Param {
      get { return param_; }
      set {
        param_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Npcuid != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Npcuid);
      }
      if (Eventid != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Eventid);
      }
      if (Param != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(Param);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Npcuid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Npcuid);
      }
      if (Eventid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Eventid);
      }
      if (Param != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Param);
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
            Npcuid = input.ReadUInt32();
            break;
          }
          case 16: {
            Eventid = input.ReadUInt32();
            break;
          }
          case 24: {
            Param = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class SecondScene_Req_CloseChat : pb::IMessage {
    private static readonly pb::MessageParser<SecondScene_Req_CloseChat> _parser = new pb::MessageParser<SecondScene_Req_CloseChat>(() => new SecondScene_Req_CloseChat());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SecondScene_Req_CloseChat> Parser { get { return _parser; } }

    /// <summary>Field number for the "npcuid" field.</summary>
    public const int NpcuidFieldNumber = 1;
    private uint npcuid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Npcuid {
      get { return npcuid_; }
      set {
        npcuid_ = value;
      }
    }

    /// <summary>Field number for the "chatid" field.</summary>
    public const int ChatidFieldNumber = 2;
    private uint chatid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Chatid {
      get { return chatid_; }
      set {
        chatid_ = value;
      }
    }

    /// <summary>Field number for the "param" field.</summary>
    public const int ParamFieldNumber = 3;
    private uint param_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Param {
      get { return param_; }
      set {
        param_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Npcuid != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Npcuid);
      }
      if (Chatid != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Chatid);
      }
      if (Param != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(Param);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Npcuid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Npcuid);
      }
      if (Chatid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Chatid);
      }
      if (Param != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Param);
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
            Npcuid = input.ReadUInt32();
            break;
          }
          case 16: {
            Chatid = input.ReadUInt32();
            break;
          }
          case 24: {
            Param = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
