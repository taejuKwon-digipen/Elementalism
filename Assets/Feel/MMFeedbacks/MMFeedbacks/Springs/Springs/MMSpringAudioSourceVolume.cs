using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
	[AddComponentMenu("More Mountains/Springs/MMSpringAudioSourceVolume")]
	public class MMSpringAudioSourceVolume : MMSpringFloatComponent<AudioSource>
	{
		public override float TargetFloat
		{
			get => Target.volume;
			set => Target.volume = value; 
		}
	}
}
