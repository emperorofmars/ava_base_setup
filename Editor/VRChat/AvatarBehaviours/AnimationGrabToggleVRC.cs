#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using UnityEngine;
using VRC.SDK3.Dynamics.Contact.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[AddComponentMenu("AVA/VRChat/Behaviours/AnimationGrabToggle")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class AnimationGrabToggleVRC : AnimationToggleVRC
	{
		public VRCContactReceiver Contact;
		public VRCPhysBoneCollider Collider;
		public string Hand = "right";

		public string GrabEnabledParameter => ParameterName + "_enabled";
	}
}

#endif
#endif
