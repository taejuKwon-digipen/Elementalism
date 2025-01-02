#if MM_TEXTMESHPRO
using TMPro;
#endif
using UnityEngine;
#if MM_UI
using UnityEngine.UI;
#endif

namespace MoreMountains.Feel
{
	[AddComponentMenu("")]
	public class FeelSpringsDemoSlider : MonoBehaviour
	{
		#if MM_UI
		[Header("Bindings")]
		public Slider TargetSlider;
		#endif
		#if MM_TEXTMESHPRO
		public TMP_Text ValueText;
		#endif
		#if MM_UI
		public float value => TargetSlider.value;
		#else
		public float value => 0f;
		#endif
		
		public void UpdateText()
		{
			#if MM_TEXTMESHPRO
			ValueText.text = TargetSlider.value.ToString("F2");
			#endif
		}
		
	}
}
