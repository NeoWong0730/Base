namespace Logic
{
	public class UI_Layout
	{
		protected UIMember mMember;		
		public bool Bind(UnityEngine.GameObject go)
		{
			return go.TryGetComponent<UIMember>(out mMember);
		}
		public UnityEngine.RectTransform dasf { get { return (UnityEngine.RectTransform)mMember.mValues[0]; } }
		public UnityEngine.RectTransform ggg { get { return (UnityEngine.RectTransform)mMember.mValues[1]; } }
	}
}
