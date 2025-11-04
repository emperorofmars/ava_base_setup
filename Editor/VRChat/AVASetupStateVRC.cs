#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[AddComponentMenu("")]
	public class AVASetupStateVRC : MonoBehaviour
	{
		[System.Serializable]
		public class AnimatorControllerLayer
		{
			public List<AnimatorController> Pre_FT = new();
			public List<AnimatorController> FT = new();
			public List<AnimatorController> FT_React = new();
			public List<AnimatorController> Mut = new();
			public List<AnimatorController> Other = new();
		}

		public List<AnimatorControllerLayer> Layers = new() { new(), new(), new(), new(), new() };

		public List<VRCExpressionParameters.Parameter> Parameters = new();
		public VRCExpressionsMenu FTMenu;

		public List<Object> UnityResourcesToStoreIfDesired = new();
	}
}

#endif
#endif
