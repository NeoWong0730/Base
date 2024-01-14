using UnityEngine;

namespace Logic
{
    public class MovementSystem : LvPlaySystemBase
    {
        private static float _lastCustomInterpolationStartTime = -1f;
        private static float _lastCustomInterpolationDeltaTime = -1f;

        private float timer;
        private int currentTick;
        private float minTimeBetweenTicks = 1f / SERVER_TICK_RATE;
        private const float SERVER_TICK_RATE = 20;

        public override bool CanUpdate()
        {
            return true;
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;

            while (timer >= minTimeBetweenTicks)
            {
                timer -= minTimeBetweenTicks;
                HandleTick();
                currentTick++;
            }
        }  

        public override void OnLateUpdate()
        {
            CustomInterpolationUpdate();
        }

        void HandleTick()
        {
            Reconciliation(minTimeBetweenTicks);
            Simulate(minTimeBetweenTicks);
        }

        void Reconciliation(float deltaTime)
        {

        }

        void Simulate(float deltaTime)
        {
            PreSimulationInterpolationUpdate(deltaTime);

            foreach (var item in mData.mActorList)
            {
                if (item.mMovementController != null)
                {
                    KinematicCharacterMotor motor = item.mMovementController.Motor;
                    motor.UpdatePhase1(deltaTime);
                    motor.UpdatePhase2(deltaTime);
                    motor.Transform.SetPositionAndRotation(motor.TransientPosition, motor.TransientRotation);
                }
            }

            PostSimulationInterpolationUpdate(deltaTime);
        }

        void PreSimulationInterpolationUpdate(float deltaTime)
        {       
            foreach (var item in mData.mActorList)
            {
                KinematicCharacterMotor motor = item.mMovementController.Motor;

                motor.InitialTickPosition = motor.TransientPosition;
                motor.InitialTickRotation = motor.TransientRotation;

                motor.Transform.SetPositionAndRotation(motor.TransientPosition, motor.TransientRotation);
            }
        }

        void PostSimulationInterpolationUpdate(float deltaTime)
        {
            _lastCustomInterpolationStartTime = Time.time;
            _lastCustomInterpolationDeltaTime = deltaTime;

            foreach (var item in mData.mActorList)
            {
                KinematicCharacterMotor motor = item.mMovementController.Motor;

                motor.Transform.SetPositionAndRotation(motor.InitialTickPosition, motor.InitialTickRotation);
            }
        }

        void CustomInterpolationUpdate()
        {
            float interpolationFactor = Mathf.Clamp01((Time.time - _lastCustomInterpolationStartTime) / _lastCustomInterpolationDeltaTime);

            foreach (var item in mData.mActorList)
            {
                KinematicCharacterMotor motor = item.mMovementController.Motor;

                motor.Transform.SetPositionAndRotation(
                    Vector3.Lerp(motor.InitialTickPosition, motor.TransientPosition, interpolationFactor),
                         Quaternion.Slerp(motor.InitialTickRotation, motor.TransientRotation, interpolationFactor));
            }
        }
    }
}