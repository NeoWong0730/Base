// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: user.proto

// This CPP symbol can be defined to use imports that match up to the framework
// imports needed when using CocoaPods.
#if !defined(GPB_USE_PROTOBUF_FRAMEWORK_IMPORTS)
 #define GPB_USE_PROTOBUF_FRAMEWORK_IMPORTS 0
#endif

#if GPB_USE_PROTOBUF_FRAMEWORK_IMPORTS
 #import <Protobuf/GPBProtocolBuffers_RuntimeSupport.h>
#else
 #import "GPBProtocolBuffers_RuntimeSupport.h"
#endif

#import <stdatomic.h>

#import "User.pbobjc.h"
// @@protoc_insertion_point(imports)

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"

#pragma mark - UserRoot

@implementation UserRoot

// No extensions in the file and no imports, so no need to generate
// +extensionRegistry.

@end

#pragma mark - UserRoot_FileDescriptor

static GPBFileDescriptor *UserRoot_FileDescriptor(void) {
  // This is called by +initialize so there is no need to worry
  // about thread safety of the singleton.
  static GPBFileDescriptor *descriptor = NULL;
  if (!descriptor) {
    GPB_DEBUG_CHECK_RUNTIME_VERSIONS();
    descriptor = [[GPBFileDescriptor alloc] initWithPackage:@"Proto.User"
                                                     syntax:GPBFileSyntaxProto3];
  }
  return descriptor;
}

#pragma mark - Enum UserCode

GPBEnumDescriptor *UserCode_EnumDescriptor(void) {
  static _Atomic(GPBEnumDescriptor*) descriptor = nil;
  if (!descriptor) {
    static const char *valueNames =
        "Success\000Usernameinvalid\000Passwordinvalid\000"
        "Usernameerror\000Passworderror\000Usernameexis"
        "ted\000";
    static const int32_t values[] = {
        UserCode_Success,
        UserCode_Usernameinvalid,
        UserCode_Passwordinvalid,
        UserCode_Usernameerror,
        UserCode_Passworderror,
        UserCode_Usernameexisted,
    };
    GPBEnumDescriptor *worker =
        [GPBEnumDescriptor allocDescriptorForName:GPBNSStringifySymbol(UserCode)
                                       valueNames:valueNames
                                           values:values
                                            count:(uint32_t)(sizeof(values) / sizeof(int32_t))
                                     enumVerifier:UserCode_IsValidValue];
    GPBEnumDescriptor *expected = nil;
    if (!atomic_compare_exchange_strong(&descriptor, &expected, worker)) {
      [worker release];
    }
  }
  return descriptor;
}

BOOL UserCode_IsValidValue(int32_t value__) {
  switch (value__) {
    case UserCode_Success:
    case UserCode_Usernameinvalid:
    case UserCode_Passwordinvalid:
    case UserCode_Usernameerror:
    case UserCode_Passworderror:
    case UserCode_Usernameexisted:
      return YES;
    default:
      return NO;
  }
}

#pragma mark - UserRegisterRequest

@implementation UserRegisterRequest

@dynamic username;
@dynamic password;

typedef struct UserRegisterRequest__storage_ {
  uint32_t _has_storage_[1];
  NSString *username;
  NSString *password;
} UserRegisterRequest__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "username",
        .dataTypeSpecific.clazz = Nil,
        .number = UserRegisterRequest_FieldNumber_Username,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(UserRegisterRequest__storage_, username),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
      {
        .name = "password",
        .dataTypeSpecific.clazz = Nil,
        .number = UserRegisterRequest_FieldNumber_Password,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(UserRegisterRequest__storage_, password),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[UserRegisterRequest class]
                                     rootClass:[UserRoot class]
                                          file:UserRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(UserRegisterRequest__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - UserRegisterResponse

@implementation UserRegisterResponse

@dynamic code;
@dynamic message;

typedef struct UserRegisterResponse__storage_ {
  uint32_t _has_storage_[1];
  UserCode code;
  NSString *message;
} UserRegisterResponse__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "code",
        .dataTypeSpecific.enumDescFunc = UserCode_EnumDescriptor,
        .number = UserRegisterResponse_FieldNumber_Code,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(UserRegisterResponse__storage_, code),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldHasEnumDescriptor | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeEnum,
      },
      {
        .name = "message",
        .dataTypeSpecific.clazz = Nil,
        .number = UserRegisterResponse_FieldNumber_Message,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(UserRegisterResponse__storage_, message),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[UserRegisterResponse class]
                                     rootClass:[UserRoot class]
                                          file:UserRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(UserRegisterResponse__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

int32_t UserRegisterResponse_Code_RawValue(UserRegisterResponse *message) {
  GPBDescriptor *descriptor = [UserRegisterResponse descriptor];
  GPBFieldDescriptor *field = [descriptor fieldWithNumber:UserRegisterResponse_FieldNumber_Code];
  return GPBGetMessageRawEnumField(message, field);
}

void SetUserRegisterResponse_Code_RawValue(UserRegisterResponse *message, int32_t value) {
  GPBDescriptor *descriptor = [UserRegisterResponse descriptor];
  GPBFieldDescriptor *field = [descriptor fieldWithNumber:UserRegisterResponse_FieldNumber_Code];
  GPBSetMessageRawEnumField(message, field, value);
}

#pragma mark - UserLoginRequest

@implementation UserLoginRequest

@dynamic username;
@dynamic password;

typedef struct UserLoginRequest__storage_ {
  uint32_t _has_storage_[1];
  NSString *username;
  NSString *password;
} UserLoginRequest__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "username",
        .dataTypeSpecific.clazz = Nil,
        .number = UserLoginRequest_FieldNumber_Username,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(UserLoginRequest__storage_, username),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
      {
        .name = "password",
        .dataTypeSpecific.clazz = Nil,
        .number = UserLoginRequest_FieldNumber_Password,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(UserLoginRequest__storage_, password),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[UserLoginRequest class]
                                     rootClass:[UserRoot class]
                                          file:UserRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(UserLoginRequest__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - UserLoginResponse

@implementation UserLoginResponse

@dynamic code;
@dynamic message;

typedef struct UserLoginResponse__storage_ {
  uint32_t _has_storage_[1];
  UserCode code;
  NSString *message;
} UserLoginResponse__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "code",
        .dataTypeSpecific.enumDescFunc = UserCode_EnumDescriptor,
        .number = UserLoginResponse_FieldNumber_Code,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(UserLoginResponse__storage_, code),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldHasEnumDescriptor | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeEnum,
      },
      {
        .name = "message",
        .dataTypeSpecific.clazz = Nil,
        .number = UserLoginResponse_FieldNumber_Message,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(UserLoginResponse__storage_, message),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[UserLoginResponse class]
                                     rootClass:[UserRoot class]
                                          file:UserRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(UserLoginResponse__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

int32_t UserLoginResponse_Code_RawValue(UserLoginResponse *message) {
  GPBDescriptor *descriptor = [UserLoginResponse descriptor];
  GPBFieldDescriptor *field = [descriptor fieldWithNumber:UserLoginResponse_FieldNumber_Code];
  return GPBGetMessageRawEnumField(message, field);
}

void SetUserLoginResponse_Code_RawValue(UserLoginResponse *message, int32_t value) {
  GPBDescriptor *descriptor = [UserLoginResponse descriptor];
  GPBFieldDescriptor *field = [descriptor fieldWithNumber:UserLoginResponse_FieldNumber_Code];
  GPBSetMessageRawEnumField(message, field, value);
}


#pragma clang diagnostic pop

// @@protoc_insertion_point(global_scope)