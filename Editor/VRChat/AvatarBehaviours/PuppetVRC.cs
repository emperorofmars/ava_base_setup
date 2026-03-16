#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using UnityEngine;
using VRC.SDKBase;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[AddComponentMenu("AVA/VRChat/Behaviours/Puppet")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class PuppetVRC : Puppet, IEditorOnly
	{
		public string ForceParameterEnabledName;
		public string ParameterEnabledName => string.IsNullOrWhiteSpace(ForceParameterEnabledName) ? "puppet_enabled_" + Name.Replace(" ", "_") + GetInstanceID() : ForceParameterEnabledName;
		public string ForceParameterXName;
		public string ParameterXName => string.IsNullOrWhiteSpace(ForceParameterXName) ? "puppet_x_" + Name.Replace(" ", "_") + GetInstanceID() : ForceParameterXName;
		public string ForceParameterYName;
		public string ParameterYName => string.IsNullOrWhiteSpace(ForceParameterYName) ? "puppet_y_" + Name.Replace(" ", "_") + GetInstanceID() : ForceParameterYName;
	}
}

#endif
#endif
