// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: basic.proto

#ifndef GOOGLE_PROTOBUF_INCLUDED_basic_2eproto
#define GOOGLE_PROTOBUF_INCLUDED_basic_2eproto

#include <limits>
#include <string>

#include <google/protobuf/port_def.inc>
#if PROTOBUF_VERSION < 3021000
#error This file was generated by a newer version of protoc which is
#error incompatible with your Protocol Buffer headers. Please update
#error your headers.
#endif
#if 3021012 < PROTOBUF_MIN_PROTOC_VERSION
#error This file was generated by an older version of protoc which is
#error incompatible with your Protocol Buffer headers. Please
#error regenerate this file with a newer version of protoc.
#endif

#include <google/protobuf/port_undef.inc>
#include <google/protobuf/io/coded_stream.h>
#include <google/protobuf/arena.h>
#include <google/protobuf/arenastring.h>
#include <google/protobuf/generated_message_bases.h>
#include <google/protobuf/generated_message_util.h>
#include <google/protobuf/metadata_lite.h>
#include <google/protobuf/generated_message_reflection.h>
#include <google/protobuf/message.h>
#include <google/protobuf/repeated_field.h>  // IWYU pragma: export
#include <google/protobuf/extension_set.h>  // IWYU pragma: export
#include <google/protobuf/unknown_field_set.h>
// @@protoc_insertion_point(includes)
#include <google/protobuf/port_def.inc>
#define PROTOBUF_INTERNAL_EXPORT_basic_2eproto
PROTOBUF_NAMESPACE_OPEN
namespace internal {
class AnyMetadata;
}  // namespace internal
PROTOBUF_NAMESPACE_CLOSE

// Internal implementation detail -- do not use these members.
struct TableStruct_basic_2eproto {
  static const uint32_t offsets[];
};
extern const ::PROTOBUF_NAMESPACE_ID::internal::DescriptorTable descriptor_table_basic_2eproto;
namespace Proto {
namespace Basic {
class Entity;
struct EntityDefaultTypeInternal;
extern EntityDefaultTypeInternal _Entity_default_instance_;
class HeartBeatRequest;
struct HeartBeatRequestDefaultTypeInternal;
extern HeartBeatRequestDefaultTypeInternal _HeartBeatRequest_default_instance_;
class HeartBeatResponse;
struct HeartBeatResponseDefaultTypeInternal;
extern HeartBeatResponseDefaultTypeInternal _HeartBeatResponse_default_instance_;
class Vector3;
struct Vector3DefaultTypeInternal;
extern Vector3DefaultTypeInternal _Vector3_default_instance_;
}  // namespace Basic
}  // namespace Proto
PROTOBUF_NAMESPACE_OPEN
template<> ::Proto::Basic::Entity* Arena::CreateMaybeMessage<::Proto::Basic::Entity>(Arena*);
template<> ::Proto::Basic::HeartBeatRequest* Arena::CreateMaybeMessage<::Proto::Basic::HeartBeatRequest>(Arena*);
template<> ::Proto::Basic::HeartBeatResponse* Arena::CreateMaybeMessage<::Proto::Basic::HeartBeatResponse>(Arena*);
template<> ::Proto::Basic::Vector3* Arena::CreateMaybeMessage<::Proto::Basic::Vector3>(Arena*);
PROTOBUF_NAMESPACE_CLOSE
namespace Proto {
namespace Basic {

// ===================================================================

class Vector3 final :
    public ::PROTOBUF_NAMESPACE_ID::Message /* @@protoc_insertion_point(class_definition:Proto.Basic.Vector3) */ {
 public:
  inline Vector3() : Vector3(nullptr) {}
  ~Vector3() override;
  explicit PROTOBUF_CONSTEXPR Vector3(::PROTOBUF_NAMESPACE_ID::internal::ConstantInitialized);

  Vector3(const Vector3& from);
  Vector3(Vector3&& from) noexcept
    : Vector3() {
    *this = ::std::move(from);
  }

  inline Vector3& operator=(const Vector3& from) {
    CopyFrom(from);
    return *this;
  }
  inline Vector3& operator=(Vector3&& from) noexcept {
    if (this == &from) return *this;
    if (GetOwningArena() == from.GetOwningArena()
  #ifdef PROTOBUF_FORCE_COPY_IN_MOVE
        && GetOwningArena() != nullptr
  #endif  // !PROTOBUF_FORCE_COPY_IN_MOVE
    ) {
      InternalSwap(&from);
    } else {
      CopyFrom(from);
    }
    return *this;
  }

  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* descriptor() {
    return GetDescriptor();
  }
  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* GetDescriptor() {
    return default_instance().GetMetadata().descriptor;
  }
  static const ::PROTOBUF_NAMESPACE_ID::Reflection* GetReflection() {
    return default_instance().GetMetadata().reflection;
  }
  static const Vector3& default_instance() {
    return *internal_default_instance();
  }
  static inline const Vector3* internal_default_instance() {
    return reinterpret_cast<const Vector3*>(
               &_Vector3_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    0;

  friend void swap(Vector3& a, Vector3& b) {
    a.Swap(&b);
  }
  inline void Swap(Vector3* other) {
    if (other == this) return;
  #ifdef PROTOBUF_FORCE_COPY_IN_SWAP
    if (GetOwningArena() != nullptr &&
        GetOwningArena() == other->GetOwningArena()) {
   #else  // PROTOBUF_FORCE_COPY_IN_SWAP
    if (GetOwningArena() == other->GetOwningArena()) {
  #endif  // !PROTOBUF_FORCE_COPY_IN_SWAP
      InternalSwap(other);
    } else {
      ::PROTOBUF_NAMESPACE_ID::internal::GenericSwap(this, other);
    }
  }
  void UnsafeArenaSwap(Vector3* other) {
    if (other == this) return;
    GOOGLE_DCHECK(GetOwningArena() == other->GetOwningArena());
    InternalSwap(other);
  }

  // implements Message ----------------------------------------------

  Vector3* New(::PROTOBUF_NAMESPACE_ID::Arena* arena = nullptr) const final {
    return CreateMaybeMessage<Vector3>(arena);
  }
  using ::PROTOBUF_NAMESPACE_ID::Message::CopyFrom;
  void CopyFrom(const Vector3& from);
  using ::PROTOBUF_NAMESPACE_ID::Message::MergeFrom;
  void MergeFrom( const Vector3& from) {
    Vector3::MergeImpl(*this, from);
  }
  private:
  static void MergeImpl(::PROTOBUF_NAMESPACE_ID::Message& to_msg, const ::PROTOBUF_NAMESPACE_ID::Message& from_msg);
  public:
  PROTOBUF_ATTRIBUTE_REINITIALIZES void Clear() final;
  bool IsInitialized() const final;

  size_t ByteSizeLong() const final;
  const char* _InternalParse(const char* ptr, ::PROTOBUF_NAMESPACE_ID::internal::ParseContext* ctx) final;
  uint8_t* _InternalSerialize(
      uint8_t* target, ::PROTOBUF_NAMESPACE_ID::io::EpsCopyOutputStream* stream) const final;
  int GetCachedSize() const final { return _impl_._cached_size_.Get(); }

  private:
  void SharedCtor(::PROTOBUF_NAMESPACE_ID::Arena* arena, bool is_message_owned);
  void SharedDtor();
  void SetCachedSize(int size) const final;
  void InternalSwap(Vector3* other);

  private:
  friend class ::PROTOBUF_NAMESPACE_ID::internal::AnyMetadata;
  static ::PROTOBUF_NAMESPACE_ID::StringPiece FullMessageName() {
    return "Proto.Basic.Vector3";
  }
  protected:
  explicit Vector3(::PROTOBUF_NAMESPACE_ID::Arena* arena,
                       bool is_message_owned = false);
  public:

  static const ClassData _class_data_;
  const ::PROTOBUF_NAMESPACE_ID::Message::ClassData*GetClassData() const final;

  ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadata() const final;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  enum : int {
    kXFieldNumber = 1,
    kYFieldNumber = 2,
    kZFieldNumber = 3,
  };
  // int32 x = 1;
  void clear_x();
  int32_t x() const;
  void set_x(int32_t value);
  private:
  int32_t _internal_x() const;
  void _internal_set_x(int32_t value);
  public:

  // int32 y = 2;
  void clear_y();
  int32_t y() const;
  void set_y(int32_t value);
  private:
  int32_t _internal_y() const;
  void _internal_set_y(int32_t value);
  public:

  // int32 z = 3;
  void clear_z();
  int32_t z() const;
  void set_z(int32_t value);
  private:
  int32_t _internal_z() const;
  void _internal_set_z(int32_t value);
  public:

  // @@protoc_insertion_point(class_scope:Proto.Basic.Vector3)
 private:
  class _Internal;

  template <typename T> friend class ::PROTOBUF_NAMESPACE_ID::Arena::InternalHelper;
  typedef void InternalArenaConstructable_;
  typedef void DestructorSkippable_;
  struct Impl_ {
    int32_t x_;
    int32_t y_;
    int32_t z_;
    mutable ::PROTOBUF_NAMESPACE_ID::internal::CachedSize _cached_size_;
  };
  union { Impl_ _impl_; };
  friend struct ::TableStruct_basic_2eproto;
};
// -------------------------------------------------------------------

class Entity final :
    public ::PROTOBUF_NAMESPACE_ID::Message /* @@protoc_insertion_point(class_definition:Proto.Basic.Entity) */ {
 public:
  inline Entity() : Entity(nullptr) {}
  ~Entity() override;
  explicit PROTOBUF_CONSTEXPR Entity(::PROTOBUF_NAMESPACE_ID::internal::ConstantInitialized);

  Entity(const Entity& from);
  Entity(Entity&& from) noexcept
    : Entity() {
    *this = ::std::move(from);
  }

  inline Entity& operator=(const Entity& from) {
    CopyFrom(from);
    return *this;
  }
  inline Entity& operator=(Entity&& from) noexcept {
    if (this == &from) return *this;
    if (GetOwningArena() == from.GetOwningArena()
  #ifdef PROTOBUF_FORCE_COPY_IN_MOVE
        && GetOwningArena() != nullptr
  #endif  // !PROTOBUF_FORCE_COPY_IN_MOVE
    ) {
      InternalSwap(&from);
    } else {
      CopyFrom(from);
    }
    return *this;
  }

  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* descriptor() {
    return GetDescriptor();
  }
  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* GetDescriptor() {
    return default_instance().GetMetadata().descriptor;
  }
  static const ::PROTOBUF_NAMESPACE_ID::Reflection* GetReflection() {
    return default_instance().GetMetadata().reflection;
  }
  static const Entity& default_instance() {
    return *internal_default_instance();
  }
  static inline const Entity* internal_default_instance() {
    return reinterpret_cast<const Entity*>(
               &_Entity_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    1;

  friend void swap(Entity& a, Entity& b) {
    a.Swap(&b);
  }
  inline void Swap(Entity* other) {
    if (other == this) return;
  #ifdef PROTOBUF_FORCE_COPY_IN_SWAP
    if (GetOwningArena() != nullptr &&
        GetOwningArena() == other->GetOwningArena()) {
   #else  // PROTOBUF_FORCE_COPY_IN_SWAP
    if (GetOwningArena() == other->GetOwningArena()) {
  #endif  // !PROTOBUF_FORCE_COPY_IN_SWAP
      InternalSwap(other);
    } else {
      ::PROTOBUF_NAMESPACE_ID::internal::GenericSwap(this, other);
    }
  }
  void UnsafeArenaSwap(Entity* other) {
    if (other == this) return;
    GOOGLE_DCHECK(GetOwningArena() == other->GetOwningArena());
    InternalSwap(other);
  }

  // implements Message ----------------------------------------------

  Entity* New(::PROTOBUF_NAMESPACE_ID::Arena* arena = nullptr) const final {
    return CreateMaybeMessage<Entity>(arena);
  }
  using ::PROTOBUF_NAMESPACE_ID::Message::CopyFrom;
  void CopyFrom(const Entity& from);
  using ::PROTOBUF_NAMESPACE_ID::Message::MergeFrom;
  void MergeFrom( const Entity& from) {
    Entity::MergeImpl(*this, from);
  }
  private:
  static void MergeImpl(::PROTOBUF_NAMESPACE_ID::Message& to_msg, const ::PROTOBUF_NAMESPACE_ID::Message& from_msg);
  public:
  PROTOBUF_ATTRIBUTE_REINITIALIZES void Clear() final;
  bool IsInitialized() const final;

  size_t ByteSizeLong() const final;
  const char* _InternalParse(const char* ptr, ::PROTOBUF_NAMESPACE_ID::internal::ParseContext* ctx) final;
  uint8_t* _InternalSerialize(
      uint8_t* target, ::PROTOBUF_NAMESPACE_ID::io::EpsCopyOutputStream* stream) const final;
  int GetCachedSize() const final { return _impl_._cached_size_.Get(); }

  private:
  void SharedCtor(::PROTOBUF_NAMESPACE_ID::Arena* arena, bool is_message_owned);
  void SharedDtor();
  void SetCachedSize(int size) const final;
  void InternalSwap(Entity* other);

  private:
  friend class ::PROTOBUF_NAMESPACE_ID::internal::AnyMetadata;
  static ::PROTOBUF_NAMESPACE_ID::StringPiece FullMessageName() {
    return "Proto.Basic.Entity";
  }
  protected:
  explicit Entity(::PROTOBUF_NAMESPACE_ID::Arena* arena,
                       bool is_message_owned = false);
  public:

  static const ClassData _class_data_;
  const ::PROTOBUF_NAMESPACE_ID::Message::ClassData*GetClassData() const final;

  ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadata() const final;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  enum : int {
    kPositionFieldNumber = 2,
    kDirectionFieldNumber = 3,
    kScaleFieldNumber = 4,
    kUidFieldNumber = 1,
  };
  // .Proto.Basic.Vector3 position = 2;
  bool has_position() const;
  private:
  bool _internal_has_position() const;
  public:
  void clear_position();
  const ::Proto::Basic::Vector3& position() const;
  PROTOBUF_NODISCARD ::Proto::Basic::Vector3* release_position();
  ::Proto::Basic::Vector3* mutable_position();
  void set_allocated_position(::Proto::Basic::Vector3* position);
  private:
  const ::Proto::Basic::Vector3& _internal_position() const;
  ::Proto::Basic::Vector3* _internal_mutable_position();
  public:
  void unsafe_arena_set_allocated_position(
      ::Proto::Basic::Vector3* position);
  ::Proto::Basic::Vector3* unsafe_arena_release_position();

  // .Proto.Basic.Vector3 direction = 3;
  bool has_direction() const;
  private:
  bool _internal_has_direction() const;
  public:
  void clear_direction();
  const ::Proto::Basic::Vector3& direction() const;
  PROTOBUF_NODISCARD ::Proto::Basic::Vector3* release_direction();
  ::Proto::Basic::Vector3* mutable_direction();
  void set_allocated_direction(::Proto::Basic::Vector3* direction);
  private:
  const ::Proto::Basic::Vector3& _internal_direction() const;
  ::Proto::Basic::Vector3* _internal_mutable_direction();
  public:
  void unsafe_arena_set_allocated_direction(
      ::Proto::Basic::Vector3* direction);
  ::Proto::Basic::Vector3* unsafe_arena_release_direction();

  // .Proto.Basic.Vector3 scale = 4;
  bool has_scale() const;
  private:
  bool _internal_has_scale() const;
  public:
  void clear_scale();
  const ::Proto::Basic::Vector3& scale() const;
  PROTOBUF_NODISCARD ::Proto::Basic::Vector3* release_scale();
  ::Proto::Basic::Vector3* mutable_scale();
  void set_allocated_scale(::Proto::Basic::Vector3* scale);
  private:
  const ::Proto::Basic::Vector3& _internal_scale() const;
  ::Proto::Basic::Vector3* _internal_mutable_scale();
  public:
  void unsafe_arena_set_allocated_scale(
      ::Proto::Basic::Vector3* scale);
  ::Proto::Basic::Vector3* unsafe_arena_release_scale();

  // uint64 uid = 1;
  void clear_uid();
  uint64_t uid() const;
  void set_uid(uint64_t value);
  private:
  uint64_t _internal_uid() const;
  void _internal_set_uid(uint64_t value);
  public:

  // @@protoc_insertion_point(class_scope:Proto.Basic.Entity)
 private:
  class _Internal;

  template <typename T> friend class ::PROTOBUF_NAMESPACE_ID::Arena::InternalHelper;
  typedef void InternalArenaConstructable_;
  typedef void DestructorSkippable_;
  struct Impl_ {
    ::Proto::Basic::Vector3* position_;
    ::Proto::Basic::Vector3* direction_;
    ::Proto::Basic::Vector3* scale_;
    uint64_t uid_;
    mutable ::PROTOBUF_NAMESPACE_ID::internal::CachedSize _cached_size_;
  };
  union { Impl_ _impl_; };
  friend struct ::TableStruct_basic_2eproto;
};
// -------------------------------------------------------------------

class HeartBeatRequest final :
    public ::PROTOBUF_NAMESPACE_ID::internal::ZeroFieldsBase /* @@protoc_insertion_point(class_definition:Proto.Basic.HeartBeatRequest) */ {
 public:
  inline HeartBeatRequest() : HeartBeatRequest(nullptr) {}
  explicit PROTOBUF_CONSTEXPR HeartBeatRequest(::PROTOBUF_NAMESPACE_ID::internal::ConstantInitialized);

  HeartBeatRequest(const HeartBeatRequest& from);
  HeartBeatRequest(HeartBeatRequest&& from) noexcept
    : HeartBeatRequest() {
    *this = ::std::move(from);
  }

  inline HeartBeatRequest& operator=(const HeartBeatRequest& from) {
    CopyFrom(from);
    return *this;
  }
  inline HeartBeatRequest& operator=(HeartBeatRequest&& from) noexcept {
    if (this == &from) return *this;
    if (GetOwningArena() == from.GetOwningArena()
  #ifdef PROTOBUF_FORCE_COPY_IN_MOVE
        && GetOwningArena() != nullptr
  #endif  // !PROTOBUF_FORCE_COPY_IN_MOVE
    ) {
      InternalSwap(&from);
    } else {
      CopyFrom(from);
    }
    return *this;
  }

  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* descriptor() {
    return GetDescriptor();
  }
  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* GetDescriptor() {
    return default_instance().GetMetadata().descriptor;
  }
  static const ::PROTOBUF_NAMESPACE_ID::Reflection* GetReflection() {
    return default_instance().GetMetadata().reflection;
  }
  static const HeartBeatRequest& default_instance() {
    return *internal_default_instance();
  }
  static inline const HeartBeatRequest* internal_default_instance() {
    return reinterpret_cast<const HeartBeatRequest*>(
               &_HeartBeatRequest_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    2;

  friend void swap(HeartBeatRequest& a, HeartBeatRequest& b) {
    a.Swap(&b);
  }
  inline void Swap(HeartBeatRequest* other) {
    if (other == this) return;
  #ifdef PROTOBUF_FORCE_COPY_IN_SWAP
    if (GetOwningArena() != nullptr &&
        GetOwningArena() == other->GetOwningArena()) {
   #else  // PROTOBUF_FORCE_COPY_IN_SWAP
    if (GetOwningArena() == other->GetOwningArena()) {
  #endif  // !PROTOBUF_FORCE_COPY_IN_SWAP
      InternalSwap(other);
    } else {
      ::PROTOBUF_NAMESPACE_ID::internal::GenericSwap(this, other);
    }
  }
  void UnsafeArenaSwap(HeartBeatRequest* other) {
    if (other == this) return;
    GOOGLE_DCHECK(GetOwningArena() == other->GetOwningArena());
    InternalSwap(other);
  }

  // implements Message ----------------------------------------------

  HeartBeatRequest* New(::PROTOBUF_NAMESPACE_ID::Arena* arena = nullptr) const final {
    return CreateMaybeMessage<HeartBeatRequest>(arena);
  }
  using ::PROTOBUF_NAMESPACE_ID::internal::ZeroFieldsBase::CopyFrom;
  inline void CopyFrom(const HeartBeatRequest& from) {
    ::PROTOBUF_NAMESPACE_ID::internal::ZeroFieldsBase::CopyImpl(*this, from);
  }
  using ::PROTOBUF_NAMESPACE_ID::internal::ZeroFieldsBase::MergeFrom;
  void MergeFrom(const HeartBeatRequest& from) {
    ::PROTOBUF_NAMESPACE_ID::internal::ZeroFieldsBase::MergeImpl(*this, from);
  }
  public:

  private:
  friend class ::PROTOBUF_NAMESPACE_ID::internal::AnyMetadata;
  static ::PROTOBUF_NAMESPACE_ID::StringPiece FullMessageName() {
    return "Proto.Basic.HeartBeatRequest";
  }
  protected:
  explicit HeartBeatRequest(::PROTOBUF_NAMESPACE_ID::Arena* arena,
                       bool is_message_owned = false);
  public:

  static const ClassData _class_data_;
  const ::PROTOBUF_NAMESPACE_ID::Message::ClassData*GetClassData() const final;

  ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadata() const final;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  // @@protoc_insertion_point(class_scope:Proto.Basic.HeartBeatRequest)
 private:
  class _Internal;

  template <typename T> friend class ::PROTOBUF_NAMESPACE_ID::Arena::InternalHelper;
  typedef void InternalArenaConstructable_;
  typedef void DestructorSkippable_;
  struct Impl_ {
  };
  friend struct ::TableStruct_basic_2eproto;
};
// -------------------------------------------------------------------

class HeartBeatResponse final :
    public ::PROTOBUF_NAMESPACE_ID::internal::ZeroFieldsBase /* @@protoc_insertion_point(class_definition:Proto.Basic.HeartBeatResponse) */ {
 public:
  inline HeartBeatResponse() : HeartBeatResponse(nullptr) {}
  explicit PROTOBUF_CONSTEXPR HeartBeatResponse(::PROTOBUF_NAMESPACE_ID::internal::ConstantInitialized);

  HeartBeatResponse(const HeartBeatResponse& from);
  HeartBeatResponse(HeartBeatResponse&& from) noexcept
    : HeartBeatResponse() {
    *this = ::std::move(from);
  }

  inline HeartBeatResponse& operator=(const HeartBeatResponse& from) {
    CopyFrom(from);
    return *this;
  }
  inline HeartBeatResponse& operator=(HeartBeatResponse&& from) noexcept {
    if (this == &from) return *this;
    if (GetOwningArena() == from.GetOwningArena()
  #ifdef PROTOBUF_FORCE_COPY_IN_MOVE
        && GetOwningArena() != nullptr
  #endif  // !PROTOBUF_FORCE_COPY_IN_MOVE
    ) {
      InternalSwap(&from);
    } else {
      CopyFrom(from);
    }
    return *this;
  }

  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* descriptor() {
    return GetDescriptor();
  }
  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* GetDescriptor() {
    return default_instance().GetMetadata().descriptor;
  }
  static const ::PROTOBUF_NAMESPACE_ID::Reflection* GetReflection() {
    return default_instance().GetMetadata().reflection;
  }
  static const HeartBeatResponse& default_instance() {
    return *internal_default_instance();
  }
  static inline const HeartBeatResponse* internal_default_instance() {
    return reinterpret_cast<const HeartBeatResponse*>(
               &_HeartBeatResponse_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    3;

  friend void swap(HeartBeatResponse& a, HeartBeatResponse& b) {
    a.Swap(&b);
  }
  inline void Swap(HeartBeatResponse* other) {
    if (other == this) return;
  #ifdef PROTOBUF_FORCE_COPY_IN_SWAP
    if (GetOwningArena() != nullptr &&
        GetOwningArena() == other->GetOwningArena()) {
   #else  // PROTOBUF_FORCE_COPY_IN_SWAP
    if (GetOwningArena() == other->GetOwningArena()) {
  #endif  // !PROTOBUF_FORCE_COPY_IN_SWAP
      InternalSwap(other);
    } else {
      ::PROTOBUF_NAMESPACE_ID::internal::GenericSwap(this, other);
    }
  }
  void UnsafeArenaSwap(HeartBeatResponse* other) {
    if (other == this) return;
    GOOGLE_DCHECK(GetOwningArena() == other->GetOwningArena());
    InternalSwap(other);
  }

  // implements Message ----------------------------------------------

  HeartBeatResponse* New(::PROTOBUF_NAMESPACE_ID::Arena* arena = nullptr) const final {
    return CreateMaybeMessage<HeartBeatResponse>(arena);
  }
  using ::PROTOBUF_NAMESPACE_ID::internal::ZeroFieldsBase::CopyFrom;
  inline void CopyFrom(const HeartBeatResponse& from) {
    ::PROTOBUF_NAMESPACE_ID::internal::ZeroFieldsBase::CopyImpl(*this, from);
  }
  using ::PROTOBUF_NAMESPACE_ID::internal::ZeroFieldsBase::MergeFrom;
  void MergeFrom(const HeartBeatResponse& from) {
    ::PROTOBUF_NAMESPACE_ID::internal::ZeroFieldsBase::MergeImpl(*this, from);
  }
  public:

  private:
  friend class ::PROTOBUF_NAMESPACE_ID::internal::AnyMetadata;
  static ::PROTOBUF_NAMESPACE_ID::StringPiece FullMessageName() {
    return "Proto.Basic.HeartBeatResponse";
  }
  protected:
  explicit HeartBeatResponse(::PROTOBUF_NAMESPACE_ID::Arena* arena,
                       bool is_message_owned = false);
  public:

  static const ClassData _class_data_;
  const ::PROTOBUF_NAMESPACE_ID::Message::ClassData*GetClassData() const final;

  ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadata() const final;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  // @@protoc_insertion_point(class_scope:Proto.Basic.HeartBeatResponse)
 private:
  class _Internal;

  template <typename T> friend class ::PROTOBUF_NAMESPACE_ID::Arena::InternalHelper;
  typedef void InternalArenaConstructable_;
  typedef void DestructorSkippable_;
  struct Impl_ {
  };
  friend struct ::TableStruct_basic_2eproto;
};
// ===================================================================


// ===================================================================

#ifdef __GNUC__
  #pragma GCC diagnostic push
  #pragma GCC diagnostic ignored "-Wstrict-aliasing"
#endif  // __GNUC__
// Vector3

// int32 x = 1;
inline void Vector3::clear_x() {
  _impl_.x_ = 0;
}
inline int32_t Vector3::_internal_x() const {
  return _impl_.x_;
}
inline int32_t Vector3::x() const {
  // @@protoc_insertion_point(field_get:Proto.Basic.Vector3.x)
  return _internal_x();
}
inline void Vector3::_internal_set_x(int32_t value) {
  
  _impl_.x_ = value;
}
inline void Vector3::set_x(int32_t value) {
  _internal_set_x(value);
  // @@protoc_insertion_point(field_set:Proto.Basic.Vector3.x)
}

// int32 y = 2;
inline void Vector3::clear_y() {
  _impl_.y_ = 0;
}
inline int32_t Vector3::_internal_y() const {
  return _impl_.y_;
}
inline int32_t Vector3::y() const {
  // @@protoc_insertion_point(field_get:Proto.Basic.Vector3.y)
  return _internal_y();
}
inline void Vector3::_internal_set_y(int32_t value) {
  
  _impl_.y_ = value;
}
inline void Vector3::set_y(int32_t value) {
  _internal_set_y(value);
  // @@protoc_insertion_point(field_set:Proto.Basic.Vector3.y)
}

// int32 z = 3;
inline void Vector3::clear_z() {
  _impl_.z_ = 0;
}
inline int32_t Vector3::_internal_z() const {
  return _impl_.z_;
}
inline int32_t Vector3::z() const {
  // @@protoc_insertion_point(field_get:Proto.Basic.Vector3.z)
  return _internal_z();
}
inline void Vector3::_internal_set_z(int32_t value) {
  
  _impl_.z_ = value;
}
inline void Vector3::set_z(int32_t value) {
  _internal_set_z(value);
  // @@protoc_insertion_point(field_set:Proto.Basic.Vector3.z)
}

// -------------------------------------------------------------------

// Entity

// uint64 uid = 1;
inline void Entity::clear_uid() {
  _impl_.uid_ = uint64_t{0u};
}
inline uint64_t Entity::_internal_uid() const {
  return _impl_.uid_;
}
inline uint64_t Entity::uid() const {
  // @@protoc_insertion_point(field_get:Proto.Basic.Entity.uid)
  return _internal_uid();
}
inline void Entity::_internal_set_uid(uint64_t value) {
  
  _impl_.uid_ = value;
}
inline void Entity::set_uid(uint64_t value) {
  _internal_set_uid(value);
  // @@protoc_insertion_point(field_set:Proto.Basic.Entity.uid)
}

// .Proto.Basic.Vector3 position = 2;
inline bool Entity::_internal_has_position() const {
  return this != internal_default_instance() && _impl_.position_ != nullptr;
}
inline bool Entity::has_position() const {
  return _internal_has_position();
}
inline void Entity::clear_position() {
  if (GetArenaForAllocation() == nullptr && _impl_.position_ != nullptr) {
    delete _impl_.position_;
  }
  _impl_.position_ = nullptr;
}
inline const ::Proto::Basic::Vector3& Entity::_internal_position() const {
  const ::Proto::Basic::Vector3* p = _impl_.position_;
  return p != nullptr ? *p : reinterpret_cast<const ::Proto::Basic::Vector3&>(
      ::Proto::Basic::_Vector3_default_instance_);
}
inline const ::Proto::Basic::Vector3& Entity::position() const {
  // @@protoc_insertion_point(field_get:Proto.Basic.Entity.position)
  return _internal_position();
}
inline void Entity::unsafe_arena_set_allocated_position(
    ::Proto::Basic::Vector3* position) {
  if (GetArenaForAllocation() == nullptr) {
    delete reinterpret_cast<::PROTOBUF_NAMESPACE_ID::MessageLite*>(_impl_.position_);
  }
  _impl_.position_ = position;
  if (position) {
    
  } else {
    
  }
  // @@protoc_insertion_point(field_unsafe_arena_set_allocated:Proto.Basic.Entity.position)
}
inline ::Proto::Basic::Vector3* Entity::release_position() {
  
  ::Proto::Basic::Vector3* temp = _impl_.position_;
  _impl_.position_ = nullptr;
#ifdef PROTOBUF_FORCE_COPY_IN_RELEASE
  auto* old =  reinterpret_cast<::PROTOBUF_NAMESPACE_ID::MessageLite*>(temp);
  temp = ::PROTOBUF_NAMESPACE_ID::internal::DuplicateIfNonNull(temp);
  if (GetArenaForAllocation() == nullptr) { delete old; }
#else  // PROTOBUF_FORCE_COPY_IN_RELEASE
  if (GetArenaForAllocation() != nullptr) {
    temp = ::PROTOBUF_NAMESPACE_ID::internal::DuplicateIfNonNull(temp);
  }
#endif  // !PROTOBUF_FORCE_COPY_IN_RELEASE
  return temp;
}
inline ::Proto::Basic::Vector3* Entity::unsafe_arena_release_position() {
  // @@protoc_insertion_point(field_release:Proto.Basic.Entity.position)
  
  ::Proto::Basic::Vector3* temp = _impl_.position_;
  _impl_.position_ = nullptr;
  return temp;
}
inline ::Proto::Basic::Vector3* Entity::_internal_mutable_position() {
  
  if (_impl_.position_ == nullptr) {
    auto* p = CreateMaybeMessage<::Proto::Basic::Vector3>(GetArenaForAllocation());
    _impl_.position_ = p;
  }
  return _impl_.position_;
}
inline ::Proto::Basic::Vector3* Entity::mutable_position() {
  ::Proto::Basic::Vector3* _msg = _internal_mutable_position();
  // @@protoc_insertion_point(field_mutable:Proto.Basic.Entity.position)
  return _msg;
}
inline void Entity::set_allocated_position(::Proto::Basic::Vector3* position) {
  ::PROTOBUF_NAMESPACE_ID::Arena* message_arena = GetArenaForAllocation();
  if (message_arena == nullptr) {
    delete _impl_.position_;
  }
  if (position) {
    ::PROTOBUF_NAMESPACE_ID::Arena* submessage_arena =
        ::PROTOBUF_NAMESPACE_ID::Arena::InternalGetOwningArena(position);
    if (message_arena != submessage_arena) {
      position = ::PROTOBUF_NAMESPACE_ID::internal::GetOwnedMessage(
          message_arena, position, submessage_arena);
    }
    
  } else {
    
  }
  _impl_.position_ = position;
  // @@protoc_insertion_point(field_set_allocated:Proto.Basic.Entity.position)
}

// .Proto.Basic.Vector3 direction = 3;
inline bool Entity::_internal_has_direction() const {
  return this != internal_default_instance() && _impl_.direction_ != nullptr;
}
inline bool Entity::has_direction() const {
  return _internal_has_direction();
}
inline void Entity::clear_direction() {
  if (GetArenaForAllocation() == nullptr && _impl_.direction_ != nullptr) {
    delete _impl_.direction_;
  }
  _impl_.direction_ = nullptr;
}
inline const ::Proto::Basic::Vector3& Entity::_internal_direction() const {
  const ::Proto::Basic::Vector3* p = _impl_.direction_;
  return p != nullptr ? *p : reinterpret_cast<const ::Proto::Basic::Vector3&>(
      ::Proto::Basic::_Vector3_default_instance_);
}
inline const ::Proto::Basic::Vector3& Entity::direction() const {
  // @@protoc_insertion_point(field_get:Proto.Basic.Entity.direction)
  return _internal_direction();
}
inline void Entity::unsafe_arena_set_allocated_direction(
    ::Proto::Basic::Vector3* direction) {
  if (GetArenaForAllocation() == nullptr) {
    delete reinterpret_cast<::PROTOBUF_NAMESPACE_ID::MessageLite*>(_impl_.direction_);
  }
  _impl_.direction_ = direction;
  if (direction) {
    
  } else {
    
  }
  // @@protoc_insertion_point(field_unsafe_arena_set_allocated:Proto.Basic.Entity.direction)
}
inline ::Proto::Basic::Vector3* Entity::release_direction() {
  
  ::Proto::Basic::Vector3* temp = _impl_.direction_;
  _impl_.direction_ = nullptr;
#ifdef PROTOBUF_FORCE_COPY_IN_RELEASE
  auto* old =  reinterpret_cast<::PROTOBUF_NAMESPACE_ID::MessageLite*>(temp);
  temp = ::PROTOBUF_NAMESPACE_ID::internal::DuplicateIfNonNull(temp);
  if (GetArenaForAllocation() == nullptr) { delete old; }
#else  // PROTOBUF_FORCE_COPY_IN_RELEASE
  if (GetArenaForAllocation() != nullptr) {
    temp = ::PROTOBUF_NAMESPACE_ID::internal::DuplicateIfNonNull(temp);
  }
#endif  // !PROTOBUF_FORCE_COPY_IN_RELEASE
  return temp;
}
inline ::Proto::Basic::Vector3* Entity::unsafe_arena_release_direction() {
  // @@protoc_insertion_point(field_release:Proto.Basic.Entity.direction)
  
  ::Proto::Basic::Vector3* temp = _impl_.direction_;
  _impl_.direction_ = nullptr;
  return temp;
}
inline ::Proto::Basic::Vector3* Entity::_internal_mutable_direction() {
  
  if (_impl_.direction_ == nullptr) {
    auto* p = CreateMaybeMessage<::Proto::Basic::Vector3>(GetArenaForAllocation());
    _impl_.direction_ = p;
  }
  return _impl_.direction_;
}
inline ::Proto::Basic::Vector3* Entity::mutable_direction() {
  ::Proto::Basic::Vector3* _msg = _internal_mutable_direction();
  // @@protoc_insertion_point(field_mutable:Proto.Basic.Entity.direction)
  return _msg;
}
inline void Entity::set_allocated_direction(::Proto::Basic::Vector3* direction) {
  ::PROTOBUF_NAMESPACE_ID::Arena* message_arena = GetArenaForAllocation();
  if (message_arena == nullptr) {
    delete _impl_.direction_;
  }
  if (direction) {
    ::PROTOBUF_NAMESPACE_ID::Arena* submessage_arena =
        ::PROTOBUF_NAMESPACE_ID::Arena::InternalGetOwningArena(direction);
    if (message_arena != submessage_arena) {
      direction = ::PROTOBUF_NAMESPACE_ID::internal::GetOwnedMessage(
          message_arena, direction, submessage_arena);
    }
    
  } else {
    
  }
  _impl_.direction_ = direction;
  // @@protoc_insertion_point(field_set_allocated:Proto.Basic.Entity.direction)
}

// .Proto.Basic.Vector3 scale = 4;
inline bool Entity::_internal_has_scale() const {
  return this != internal_default_instance() && _impl_.scale_ != nullptr;
}
inline bool Entity::has_scale() const {
  return _internal_has_scale();
}
inline void Entity::clear_scale() {
  if (GetArenaForAllocation() == nullptr && _impl_.scale_ != nullptr) {
    delete _impl_.scale_;
  }
  _impl_.scale_ = nullptr;
}
inline const ::Proto::Basic::Vector3& Entity::_internal_scale() const {
  const ::Proto::Basic::Vector3* p = _impl_.scale_;
  return p != nullptr ? *p : reinterpret_cast<const ::Proto::Basic::Vector3&>(
      ::Proto::Basic::_Vector3_default_instance_);
}
inline const ::Proto::Basic::Vector3& Entity::scale() const {
  // @@protoc_insertion_point(field_get:Proto.Basic.Entity.scale)
  return _internal_scale();
}
inline void Entity::unsafe_arena_set_allocated_scale(
    ::Proto::Basic::Vector3* scale) {
  if (GetArenaForAllocation() == nullptr) {
    delete reinterpret_cast<::PROTOBUF_NAMESPACE_ID::MessageLite*>(_impl_.scale_);
  }
  _impl_.scale_ = scale;
  if (scale) {
    
  } else {
    
  }
  // @@protoc_insertion_point(field_unsafe_arena_set_allocated:Proto.Basic.Entity.scale)
}
inline ::Proto::Basic::Vector3* Entity::release_scale() {
  
  ::Proto::Basic::Vector3* temp = _impl_.scale_;
  _impl_.scale_ = nullptr;
#ifdef PROTOBUF_FORCE_COPY_IN_RELEASE
  auto* old =  reinterpret_cast<::PROTOBUF_NAMESPACE_ID::MessageLite*>(temp);
  temp = ::PROTOBUF_NAMESPACE_ID::internal::DuplicateIfNonNull(temp);
  if (GetArenaForAllocation() == nullptr) { delete old; }
#else  // PROTOBUF_FORCE_COPY_IN_RELEASE
  if (GetArenaForAllocation() != nullptr) {
    temp = ::PROTOBUF_NAMESPACE_ID::internal::DuplicateIfNonNull(temp);
  }
#endif  // !PROTOBUF_FORCE_COPY_IN_RELEASE
  return temp;
}
inline ::Proto::Basic::Vector3* Entity::unsafe_arena_release_scale() {
  // @@protoc_insertion_point(field_release:Proto.Basic.Entity.scale)
  
  ::Proto::Basic::Vector3* temp = _impl_.scale_;
  _impl_.scale_ = nullptr;
  return temp;
}
inline ::Proto::Basic::Vector3* Entity::_internal_mutable_scale() {
  
  if (_impl_.scale_ == nullptr) {
    auto* p = CreateMaybeMessage<::Proto::Basic::Vector3>(GetArenaForAllocation());
    _impl_.scale_ = p;
  }
  return _impl_.scale_;
}
inline ::Proto::Basic::Vector3* Entity::mutable_scale() {
  ::Proto::Basic::Vector3* _msg = _internal_mutable_scale();
  // @@protoc_insertion_point(field_mutable:Proto.Basic.Entity.scale)
  return _msg;
}
inline void Entity::set_allocated_scale(::Proto::Basic::Vector3* scale) {
  ::PROTOBUF_NAMESPACE_ID::Arena* message_arena = GetArenaForAllocation();
  if (message_arena == nullptr) {
    delete _impl_.scale_;
  }
  if (scale) {
    ::PROTOBUF_NAMESPACE_ID::Arena* submessage_arena =
        ::PROTOBUF_NAMESPACE_ID::Arena::InternalGetOwningArena(scale);
    if (message_arena != submessage_arena) {
      scale = ::PROTOBUF_NAMESPACE_ID::internal::GetOwnedMessage(
          message_arena, scale, submessage_arena);
    }
    
  } else {
    
  }
  _impl_.scale_ = scale;
  // @@protoc_insertion_point(field_set_allocated:Proto.Basic.Entity.scale)
}

// -------------------------------------------------------------------

// HeartBeatRequest

// -------------------------------------------------------------------

// HeartBeatResponse

#ifdef __GNUC__
  #pragma GCC diagnostic pop
#endif  // __GNUC__
// -------------------------------------------------------------------

// -------------------------------------------------------------------

// -------------------------------------------------------------------


// @@protoc_insertion_point(namespace_scope)

}  // namespace Basic
}  // namespace Proto

// @@protoc_insertion_point(global_scope)

#include <google/protobuf/port_undef.inc>
#endif  // GOOGLE_PROTOBUF_INCLUDED_GOOGLE_PROTOBUF_INCLUDED_basic_2eproto
