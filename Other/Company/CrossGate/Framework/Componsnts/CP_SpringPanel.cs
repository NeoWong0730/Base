using UnityEngine;

/// <summary>
/// Similar to SpringPosition, but also moves the panel's clipping. Works in local coordinates.
/// </summary>
public class CP_SpringPanel : MonoBehaviour
{
    public System.Action onFinished;

    public static CP_SpringPanel current;
	public Vector3 targetWorldPos = Vector3.zero;

	public float strength = 10f;

	private void LateUpdate()
	{
	    AdvanceTowardsPosition();
	}

    /// <summary>
    /// Advance toward the target position.
	/// </summary>
	protected virtual void AdvanceTowardsPosition()
	{
		float delta = Time.deltaTime;
		bool trigger = false;
		Vector3 before = transform.position;
		Vector3 after = NGUIMath.SpringLerp(before, targetWorldPos, strength, delta);
		if ((after - targetWorldPos).sqrMagnitude < 0.01f)
		{
			after = targetWorldPos;
            // disable self
			enabled = false;
			trigger = true;
		}
		transform.position = after;
		if (trigger && onFinished != null)
		{
			current = this;
			onFinished();
			current = null;
		}
    }

    /// <summary>
    /// Start the tweening process.
    /// </summary>
    public static CP_SpringPanel Begin(GameObject go, Vector3 targetPos, System.Action onFinished = null)
	{
        CP_SpringPanel sp = go.GetComponent<CP_SpringPanel>();
        if (sp == null)
        {
            sp = go.AddComponent<CP_SpringPanel>();
        }
		sp.targetWorldPos = targetPos;
		sp.onFinished = onFinished;
        // enable self
		sp.enabled = true;
		return sp;
	}
}
