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
		public string ParameterEnabledName => string.IsNullOrWhiteSpace(ForceParameterEnabledName) ? AVAConstants.PARAMETER_PREFIX + "/PuppetEnabled" + Name.Replace(" ", "") + GetInstanceID().ToString().Replace("-", "_") : ForceParameterEnabledName;
		public string ForceParameterXName;
		public string ParameterXName => string.IsNullOrWhiteSpace(ForceParameterXName) ? AVAConstants.PARAMETER_PREFIX + "/PuppetX" + Name.Replace(" ", "") + GetInstanceID().ToString().Replace("-", "_") : ForceParameterXName;
		public string ForceParameterYName;
		public string ParameterYName => string.IsNullOrWhiteSpace(ForceParameterYName) ? AVAConstants.PARAMETER_PREFIX + "/PuppetY" + Name.Replace(" ", "") + GetInstanceID().ToString().Replace("-", "_") : ForceParameterYName;
	}
}

#endif
#endif
