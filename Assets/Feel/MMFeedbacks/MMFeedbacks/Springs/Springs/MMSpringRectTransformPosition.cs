using UnityEngine;

namespace MoreMountains.Feedbacks
{
	[AddComponentMenu("More Mountains/Springs/MMSpringRectTransformPosition")]
	public class MMSpringRectTransformPosition : MMSpringVector3Component<RectTransform>
	{
		public override Vector3 TargetVector3
		{
			get => Target.anchoredPosition3D;
			set => Target.anchoredPosition3D = value;
		}
	}
}
