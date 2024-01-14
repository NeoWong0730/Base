using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraInputProvider : MonoBehaviour, AxisState.IInputAxisProvider
{
    public InputAction mInputLook;
    public InputAction mInputScale;

    public float GetAxisValue(int axis)
    {
        switch (axis)
        {
            case 0:
                return mInputLook != null && mInputLook.WasPerformedThisFrame() ? mInputLook.ReadValue<Vector2>().x : 0;
            case 1:
                return mInputLook != null && mInputLook.WasPerformedThisFrame() ? mInputLook.ReadValue<Vector2>().y : 0;
            case 2:
                return mInputScale != null && mInputScale.WasPerformedThisFrame() ? mInputScale.ReadValue<float>() : 0;
        }

        return 0;
    }
}
