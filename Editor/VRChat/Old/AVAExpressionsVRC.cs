#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using com.squirrelbite.ava_base_setup.oooold;
using UnityEngine;

namespace com.squirrelbite.ava_base_setup.vrchat.oooold
{
	[AddComponentMenu("AVA/VRChat/Expressions")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class AVAExpressionsVRC : AVAExpressions, VRC.SDKBase.IEditorOnly
	{
	}
}

#endif
#endif
