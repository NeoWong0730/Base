using UnityEngine;

namespace Logic
{
    public class MovementSystem : LvPlaySystemBase
    {
        private static float _lastCustomInterpolationStartTime = -1f;
        private static float _lastCustomInterpolationDeltaTime = -1f;

        public bool AutoSimulation = true;
        public bool Interpolate = true;

        public override void OnFixedUpdate()
        {
            if (AutoSimulation)
            {
                float deltaTime = Time.deltaTime;

                if (Interpolate)
                {
                    PreSimulationInterpolationUpdate(deltaTime);
                }

                Simulate(deltaTime);

                if (Interpolate)
                {
                    PostSimulationInterpolationUpdate(deltaTime);
                }
            }
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();

            if (Interpolate)
            {
                CustomInterpolationUpdate();
            }
        }

        void PreSimulationInterpolationUpdate(float deltaTime)
        {       
            foreach (var item in mData.mActorList)
            {
                if (item.mMovementController != null)
                {
                    KinematicCharacterMotor motor = item.mMovementController.Motor;

                    motor.InitialTickPosition = motor.TransientPosition;
                    motor.InitialTickRotation = motor.TransientRotation;

                    motor.Transform.SetPositionAndRotation(motor.TransientPosition, motor.TransientRotation);
                }
            }
        }

        void Simulate(float deltaTime)
        {
            foreach (var item in mData.mActorList)
            {
                if (item.mMovementController != null)
                {
                    KinematicCharacterMotor motor = item.mMovementController.Motor;
                    motor.UpdatePhase1(deltaTime);
                }
            }

            foreach (var item in mData.mActorList)
            {
                if (item.mMovementController != null)
                {
                    KinematicCharacterMotor motor = item.mMovementController.Motor;
                    motor.UpdatePhase2(deltaTime);
                    motor.Transform.SetPositionAndRotation(motor.TransientPosition, motor.TransientRotation);
                }
            }
        }

        void PostSimulationInterpolationUpdate(float deltaTime)
        {
            _lastCustomInterpolationStartTime = Time.time;
            _lastCustomInterpolationDeltaTime = deltaTime;

            foreach (var item in mData.mActorList)
            {
                if (item.mMovementController != null)
                {
                    KinematicCharacterMotor motor = item.mMovementController.Motor;
                    motor.Transform.SetPositionAndRotation(motor.InitialTickPosition, motor.InitialTickRotation);
                }
            }
        }

        void CustomInterpolationUpdate()
        {
            float interpolationFactor = Mathf.Clamp01((Time.time - _lastCustomInterpolationStartTime) / _lastCustomInterpolationDeltaTime);          

            foreach (var item in mData.mActorList)
            {
                if (item.mMovementController != null)
                {
                    KinematicCharacterMotor motor = item.mMovementController.Motor;
                    motor.Transform.SetPositionAndRotation(
                   Vector3.Lerp(motor.InitialTickPosition, motor.TransientPosition, interpolationFactor),
                   Quaternion.Slerp(motor.InitialTickRotation, motor.TransientRotation, interpolationFactor));
                }
            }
        }
    }
}