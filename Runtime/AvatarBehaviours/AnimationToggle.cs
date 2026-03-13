
using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	//[AddComponentMenu("AVA/Generic/AnimationToggle")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class AnimationToggle : IAvatarBehaviour
	{
		public Texture2D Icon;
		public AnimationClip On;
		public AnimationClip Off;
		public bool IsOverridable;
	}
}
