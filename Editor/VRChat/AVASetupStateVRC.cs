#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public class AVASetupStateVRC : MonoBehaviour
	{
		[System.Serializable]
		public class AnimatorControllerLayer
		{
			public AnimatorController Pre_FT;
			public AnimatorController FT;
			public AnimatorController FT_React;
			public AnimatorController Mut;
			public AnimatorController Other;
		}

		public List<AnimatorControllerLayer> Layers = new() { new(), new(), new(), new(), new() };

		public List<VRCExpressionParameters.Parameter> Parameters = new();
		public VRCExpressionsMenu FTMenu;
	}
}

#endif
#endif
