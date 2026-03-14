#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using UnityEngine;
using VRC.SDKBase;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[AddComponentMenu("AVA/VRChat/Behaviours/AnimationToggle")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class AnimationToggleVRC : AnimationToggle, IEditorOnly
	{
	}
}

#endif
#endif
