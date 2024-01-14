// Copyright Unity Technologies 2019 // https://github.com/Unity-Technologies/animation-jobs-samples //
// The original file can be downloaded from https://github.com/Unity-Technologies/animation-jobs-samples/blob/master/Assets/animation-jobs-samples/Runtime/AnimationJobs/DampingJob.cs
// This file has been modified:
// - Moved into the Animancer.Examples.Jobs namespace.
// - Removed the contents of ProcessRootMotion since it is unnecessary.

#pragma warning disable IDE0054 // Use compound assignment

using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;

namespace Framework
{
    public struct BoneDampingMultiJob : IDisposableAnimationJob
    {
        public NativeArray<int3> jointInfos;//x = parentIndex  y = childIndex z = childCount
        public NativeArray<TransformStreamHandle> jointHandles;
        public NativeArray<Vector3> localPositions;
        public NativeArray<Quaternion> localRotations;

        public NativeArray<Vector3> positions;
        public NativeArray<Quaternion> rotations;

        public NativeArray<Vector3> velocities;

        public void Dispose()
        {
            jointInfos.Dispose();
            jointHandles.Dispose();
            localPositions.Dispose();
            localRotations.Dispose();

            positions.Dispose();
            rotations.Dispose();

            velocities.Dispose();
        }

        public void ProcessRootMotion(AnimationStream stream)
        {

        }

        public void ProcessAnimation(AnimationStream stream)
        {
            if (jointHandles.Length < 2)
                return;

            ComputeDampedPositions(stream);
            ComputeJointLocalRotations(stream);
        }

        private void ComputeDampedPositions(AnimationStream stream)
        {
            for (var i = 0; i < jointHandles.Length; ++i)
            {
                int parentIndex = jointInfos[i].x;

                //isRoot
                if (parentIndex < 0)
                {
                    positions[i] = jointHandles[i].GetPosition(stream);
                    rotations[i] = jointHandles[i].GetRotation(stream);
                }
                else
                {
                    var parentPosition = positions[parentIndex];
                    var parentRotation = rotations[parentIndex];

                    // The target position is the global position, without damping.
                    var newPosition = parentPosition + (parentRotation * localPositions[i]);

                    // Apply damping on this target.
                    var velocity = velocities[i];// + Vector3.down * 5;
                    newPosition = Vector3.SmoothDamp(positions[i], newPosition, ref velocity, 0.15f, Mathf.Infinity, stream.deltaTime);

                    // Apply constraint: keep original length between joints.
                    newPosition = parentPosition + (newPosition - parentPosition).normalized * localPositions[i].magnitude;

                    // Save new velocity and position for next frame.
                    velocities[i] = velocity;
                    positions[i] = newPosition;
                    rotations[i] = parentRotation * localRotations[i];
                }                
            }
        }

        private void ComputeJointLocalRotations(AnimationStream stream)
        {
            //var parentRotation = rootHandle.GetRotation(stream);
            //var parentRotation = rootHandle.GetRotation(stream);

            for (var i = 0; i < jointHandles.Length; ++i)
            {                
                if (jointInfos[i].z < 1)
                {
                    continue;
                }

                int parentIndex = jointInfos[i].x;
                if (parentIndex < 0)
                {
                    //已经获取过了
                    //rotations[i] = jointHandles[i].GetRotation(stream);
                }
                else
                {
                    var parentRotation = rotations[parentIndex];

                    // Get the current joint rotation.
                    var rotation = parentRotation * localRotations[i];

                    // Get the current joint direction.
                    //var direction = (rotation * localPositions[i + 1]).normalized;
                    // Get the wanted joint direction.
                    //var newDirection = (positions[i + 1] - positions[i]).normalized;

                    int childStart = jointInfos[i].y;
                    int childEnd = jointInfos[i].z + jointInfos[i].y;
                    var direction = Vector3.zero;
                    var newDirection = Vector3.zero;
                    for (int childIndex = childStart; childIndex < childEnd; ++childIndex)
                    {
                        direction = (direction + (rotation * localPositions[childIndex]).normalized).normalized;
                        newDirection = (newDirection + (positions[childIndex] - positions[i]).normalized).normalized;                        
                    }
                    direction = direction.normalized;
                    newDirection = newDirection.normalized;

                    // Compute the rotation from the current direction to the new direction.
                    var currentToNewRotation = Quaternion.FromToRotation(direction, newDirection);

                    // Pre-rotate the current rotation, to get the new global rotation.
                    rotation = currentToNewRotation * rotation;

                    // Set the new local rotation.
                    var newLocalRotation = Quaternion.Inverse(parentRotation) * rotation;
                    jointHandles[i].SetLocalRotation(stream, newLocalRotation);

                    // Set the new rotation.
                    rotations[i] = rotation;
                }
            }
        }
    }
}
