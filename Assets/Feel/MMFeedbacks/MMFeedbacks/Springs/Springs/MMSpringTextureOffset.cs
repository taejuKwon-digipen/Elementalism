using UnityEngine;

namespace MoreMountains.Feedbacks
{
	[AddComponentMenu("More Mountains/Springs/MMSpringTextureOffset")]
	public class MMSpringTextureOffset : MMSpringVector2Component<Renderer>
	{
		public override Vector2 TargetVector2
		{
			get => Target.material.mainTextureOffset;
			set => Target.material.mainTextureOffset = value;
		}
	}
}
