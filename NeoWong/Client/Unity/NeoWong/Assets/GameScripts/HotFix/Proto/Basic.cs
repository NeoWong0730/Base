// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: basic.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Proto.Basic {

  /// <summary>Holder for reflection information generated from basic.proto</summary>
  public static partial class BasicReflection {

    #region Descriptor
    /// <summary>File descriptor for basic.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static BasicReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgtiYXNpYy5wcm90bxILUHJvdG8uQmFzaWMiKgoHVmVjdG9yMxIJCgF4GAEg",
            "ASgFEgkKAXkYAiABKAUSCQoBehgDIAEoBSKLAQoGRW50aXR5EgsKA3VpZBgB",
            "IAEoBBImCghwb3NpdGlvbhgCIAEoCzIULlByb3RvLkJhc2ljLlZlY3RvcjMS",
            "JwoJZGlyZWN0aW9uGAMgASgLMhQuUHJvdG8uQmFzaWMuVmVjdG9yMxIjCgVz",
            "Y2FsZRgEIAEoCzIULlByb3RvLkJhc2ljLlZlY3RvcjMiEgoQSGVhcnRCZWF0",
            "UmVxdWVzdCITChFIZWFydEJlYXRSZXNwb25zZWIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Proto.Basic.Vector3), global::Proto.Basic.Vector3.Parser, new[]{ "X", "Y", "Z" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Proto.Basic.Entity), global::Proto.Basic.Entity.Parser, new[]{ "Uid", "Position", "Direction", "Scale" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Proto.Basic.HeartBeatRequest), global::Proto.Basic.HeartBeatRequest.Parser, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Proto.Basic.HeartBeatResponse), global::Proto.Basic.HeartBeatResponse.Parser, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class Vector3 : pb::IMessage<Vector3> {
    private static readonly pb::MessageParser<Vector3> _parser = new pb::MessageParser<Vector3>(() => new Vector3());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Vector3> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Proto.Basic.BasicReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Vector3() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Vector3(Vector3 other) : this() {
      x_ = other.x_;
      y_ = other.y_;
      z_ = other.z_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Vector3 Clone() {
      return new Vector3(this);
    }

    /// <summary>Field number for the "x" field.</summary>
    public const int XFieldNumber = 1;
    private int x_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int X {
      get { return x_; }
      set {
        x_ = value;
      }
    }

    /// <summary>Field number for the "y" field.</summary>
    public const int YFieldNumber = 2;
    private int y_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Y {
      get { return y_; }
      set {
        y_ = value;
      }
    }

    /// <summary>Field number for the "z" field.</summary>
    public const int ZFieldNumber = 3;
    private int z_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Z {
      get { return z_; }
      set {
        z_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Vector3);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Vector3 other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (X != other.X) return false;
      if (Y != other.Y) return false;
      if (Z != other.Z) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (X != 0) hash ^= X.GetHashCode();
      if (Y != 0) hash ^= Y.GetHashCode();
      if (Z != 0) hash ^= Z.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (X != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(X);
      }
      if (Y != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Y);
      }
      if (Z != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(Z);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (X != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(X);
      }
      if (Y != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Y);
      }
      if (Z != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Z);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Vector3 other) {
      if (other == null) {
        return;
      }
      if (other.X != 0) {
        X = other.X;
      }
      if (other.Y != 0) {
        Y = other.Y;
      }
      if (other.Z != 0) {
        Z = other.Z;
      }
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
            X = input.ReadInt32();
            break;
          }
          case 16: {
            Y = input.ReadInt32();
            break;
          }
          case 24: {
            Z = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class Entity : pb::IMessage<Entity> {
    private static readonly pb::MessageParser<Entity> _parser = new pb::MessageParser<Entity>(() => new Entity());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Entity> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Proto.Basic.BasicReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Entity() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Entity(Entity other) : this() {
      uid_ = other.uid_;
      Position = other.position_ != null ? other.Position.Clone() : null;
      Direction = other.direction_ != null ? other.Direction.Clone() : null;
      Scale = other.scale_ != null ? other.Scale.Clone() : null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Entity Clone() {
      return new Entity(this);
    }

    /// <summary>Field number for the "uid" field.</summary>
    public const int UidFieldNumber = 1;
    private ulong uid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Uid {
      get { return uid_; }
      set {
        uid_ = value;
      }
    }

    /// <summary>Field number for the "position" field.</summary>
    public const int PositionFieldNumber = 2;
    private global::Proto.Basic.Vector3 position_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Proto.Basic.Vector3 Position {
      get { return position_; }
      set {
        position_ = value;
      }
    }

    /// <summary>Field number for the "direction" field.</summary>
    public const int DirectionFieldNumber = 3;
    private global::Proto.Basic.Vector3 direction_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Proto.Basic.Vector3 Direction {
      get { return direction_; }
      set {
        direction_ = value;
      }
    }

    /// <summary>Field number for the "scale" field.</summary>
    public const int ScaleFieldNumber = 4;
    private global::Proto.Basic.Vector3 scale_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Proto.Basic.Vector3 Scale {
      get { return scale_; }
      set {
        scale_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Entity);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Entity other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Uid != other.Uid) return false;
      if (!object.Equals(Position, other.Position)) return false;
      if (!object.Equals(Direction, other.Direction)) return false;
      if (!object.Equals(Scale, other.Scale)) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Uid != 0UL) hash ^= Uid.GetHashCode();
      if (position_ != null) hash ^= Position.GetHashCode();
      if (direction_ != null) hash ^= Direction.GetHashCode();
      if (scale_ != null) hash ^= Scale.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Uid != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(Uid);
      }
      if (position_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Position);
      }
      if (direction_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(Direction);
      }
      if (scale_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(Scale);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Uid != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Uid);
      }
      if (position_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Position);
      }
      if (direction_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Direction);
      }
      if (scale_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Scale);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Entity other) {
      if (other == null) {
        return;
      }
      if (other.Uid != 0UL) {
        Uid = other.Uid;
      }
      if (other.position_ != null) {
        if (position_ == null) {
          position_ = new global::Proto.Basic.Vector3();
        }
        Position.MergeFrom(other.Position);
      }
      if (other.direction_ != null) {
        if (direction_ == null) {
          direction_ = new global::Proto.Basic.Vector3();
        }
        Direction.MergeFrom(other.Direction);
      }
      if (other.scale_ != null) {
        if (scale_ == null) {
          scale_ = new global::Proto.Basic.Vector3();
        }
        Scale.MergeFrom(other.Scale);
      }
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
            Uid = input.ReadUInt64();
            break;
          }
          case 18: {
            if (position_ == null) {
              position_ = new global::Proto.Basic.Vector3();
            }
            input.ReadMessage(position_);
            break;
          }
          case 26: {
            if (direction_ == null) {
              direction_ = new global::Proto.Basic.Vector3();
            }
            input.ReadMessage(direction_);
            break;
          }
          case 34: {
            if (scale_ == null) {
              scale_ = new global::Proto.Basic.Vector3();
            }
            input.ReadMessage(scale_);
            break;
          }
        }
      }
    }

  }

  public sealed partial class HeartBeatRequest : pb::IMessage<HeartBeatRequest> {
    private static readonly pb::MessageParser<HeartBeatRequest> _parser = new pb::MessageParser<HeartBeatRequest>(() => new HeartBeatRequest());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<HeartBeatRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Proto.Basic.BasicReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeartBeatRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeartBeatRequest(HeartBeatRequest other) : this() {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeartBeatRequest Clone() {
      return new HeartBeatRequest(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as HeartBeatRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(HeartBeatRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(HeartBeatRequest other) {
      if (other == null) {
        return;
      }
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

  public sealed partial class HeartBeatResponse : pb::IMessage<HeartBeatResponse> {
    private static readonly pb::MessageParser<HeartBeatResponse> _parser = new pb::MessageParser<HeartBeatResponse>(() => new HeartBeatResponse());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<HeartBeatResponse> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Proto.Basic.BasicReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeartBeatResponse() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeartBeatResponse(HeartBeatResponse other) : this() {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeartBeatResponse Clone() {
      return new HeartBeatResponse(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as HeartBeatResponse);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(HeartBeatResponse other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(HeartBeatResponse other) {
      if (other == null) {
        return;
      }
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

  #endregion

}

#endregion Designer generated code
