#if UNITY_EDITOR

using UnityEditor.Animations;
using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	//[AddComponentMenu("AVA/Generic/AnimationToggle")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class AvatarAnimatorController : IAvatarBehaviour
	{
		public AnimatorController Controller;
	}
}

#endif
