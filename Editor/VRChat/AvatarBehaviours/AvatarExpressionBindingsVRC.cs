#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using UnityEngine;
using VRC.SDKBase;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[AddComponentMenu("AVA/VRChat/Expression Bindings")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	[DisallowMultipleComponent]
	public class AvatarExpressionBindingsVRC : AvatarExpressionBindings, IEditorOnly
	{
	}
}

#endif
#endif
