#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[AddComponentMenu("AVA/VRChat/Animator Controller")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class AvatarAnimatorControllerVRC : AvatarAnimatorController, IEditorOnly
	{
		public VRCAvatarDescriptor.AnimLayerType VRChatLayer = VRCAvatarDescriptor.AnimLayerType.FX;
		public SetupStateVRC.LogicLayer Order = SetupStateVRC.LogicLayer.After;

		public VRCExpressionParameters Parameters;
		public VRCExpressionsMenu Menu;
	}
}

#endif
#endif
