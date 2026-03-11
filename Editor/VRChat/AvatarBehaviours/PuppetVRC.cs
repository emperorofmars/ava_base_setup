#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using UnityEngine;
using VRC.SDKBase;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[AddComponentMenu("AVA/VRChat/Puppet")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class PuppetVRC : Puppet, IEditorOnly
	{
	}
}

#endif
#endif
