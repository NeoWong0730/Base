// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: basic.proto

// This CPP symbol can be defined to use imports that match up to the framework
// imports needed when using CocoaPods.
#if !defined(GPB_USE_PROTOBUF_FRAMEWORK_IMPORTS)
 #define GPB_USE_PROTOBUF_FRAMEWORK_IMPORTS 0
#endif

#if GPB_USE_PROTOBUF_FRAMEWORK_IMPORTS
 #import <Protobuf/GPBProtocolBuffers.h>
#else
 #import "GPBProtocolBuffers.h"
#endif

#if GOOGLE_PROTOBUF_OBJC_VERSION < 30004
#error This file was generated by a newer version of protoc which is incompatible with your Protocol Buffer library sources.
#endif
#if 30004 < GOOGLE_PROTOBUF_OBJC_MIN_SUPPORTED_VERSION
#error This file was generated by an older version of protoc which is incompatible with your Protocol Buffer library sources.
#endif

// @@protoc_insertion_point(imports)

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"

CF_EXTERN_C_BEGIN

@class Vector3;

NS_ASSUME_NONNULL_BEGIN

#pragma mark - BasicRoot

/**
 * Exposes the extension registry for this file.
 *
 * The base class provides:
 * @code
 *   + (GPBExtensionRegistry *)extensionRegistry;
 * @endcode
 * which is a @c GPBExtensionRegistry that includes all the extensions defined by
 * this file and all files that it depends on.
 **/
GPB_FINAL @interface BasicRoot : GPBRootObject
@end

#pragma mark - Vector3

typedef GPB_ENUM(Vector3_FieldNumber) {
  Vector3_FieldNumber_X = 1,
  Vector3_FieldNumber_Y = 2,
  Vector3_FieldNumber_Z = 3,
};

GPB_FINAL @interface Vector3 : GPBMessage

@property(nonatomic, readwrite) int32_t x;

@property(nonatomic, readwrite) int32_t y;

@property(nonatomic, readwrite) int32_t z;

@end

#pragma mark - Entity

typedef GPB_ENUM(Entity_FieldNumber) {
  Entity_FieldNumber_Uid = 1,
  Entity_FieldNumber_Position = 2,
  Entity_FieldNumber_Direction = 3,
  Entity_FieldNumber_Scale = 4,
};

GPB_FINAL @interface Entity : GPBMessage

@property(nonatomic, readwrite) uint64_t uid;

@property(nonatomic, readwrite, strong, null_resettable) Vector3 *position;
/** Test to see if @c position has been set. */
@property(nonatomic, readwrite) BOOL hasPosition;

@property(nonatomic, readwrite, strong, null_resettable) Vector3 *direction;
/** Test to see if @c direction has been set. */
@property(nonatomic, readwrite) BOOL hasDirection;

@property(nonatomic, readwrite, strong, null_resettable) Vector3 *scale;
/** Test to see if @c scale has been set. */
@property(nonatomic, readwrite) BOOL hasScale;

@end

#pragma mark - HeartBeatRequest

GPB_FINAL @interface HeartBeatRequest : GPBMessage

@end

#pragma mark - HeartBeatResponse

GPB_FINAL @interface HeartBeatResponse : GPBMessage

@end

NS_ASSUME_NONNULL_END

CF_EXTERN_C_END

#pragma clang diagnostic pop

// @@protoc_insertion_point(global_scope)
