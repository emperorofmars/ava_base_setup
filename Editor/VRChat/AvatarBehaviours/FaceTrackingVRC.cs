#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using UnityEngine;
using VRC.SDKBase;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[AddComponentMenu("AVA/VRChat/Behaviours/Face Tracking")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	[DisallowMultipleComponent]
	public class FaceTrackingVRC : IAvatarBehaviour, IEditorOnly
	{
		public override bool AllowMultiple => false;
		public SkinnedMeshRenderer FTMesh;
		public FT_Setup FTSetup = FT_Setup.Automatic;
		public FT_Type FTType = FT_Type.Unknown;
		public bool RemoveEyetrackingDrivers = false;
	}
}

#endif
#endif
