
using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	//[AddComponentMenu("AVA/Generic/AnimationToggle")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class AnimationToggle : IAvatarBehaviour
	{
		[Tooltip("Menu icon")]
		public Texture2D Icon;
		[Tooltip("On animation")]
		public AnimationClip On;
		[Tooltip("Off animation")]
		public AnimationClip Off;
		[Tooltip("Enabled by default")]
		public bool DefaultOn = false;
		[Tooltip("Can other behaviours override this setting")]
		public bool IsOverridable;
	}
}
