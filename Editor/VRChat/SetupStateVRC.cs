#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	// Holds the manually added and programmatically produced controllers, which are to be combined.
	[System.Serializable]
	public class SetupStateVRC
	{
		[System.Serializable]
		public class AnimatorControllerVRCLayer
		{
			public List<(string Name, AnimatorControllerParameterType Type)> ControllerParameters = new();

			public List<BlendTree> DirectBlendPre = new();
			public List<AnimatorController> ControllersPre = new();

			public List<AnimatorController> ControllersFaceTracking = new();
			public List<AnimatorController> ControllersFaceTrackingReact = new();
			public List<AnimatorController> ControllersExpression = new();

			public List<BlendTree> DirectBlendAfter = new();
			public List<AnimatorController> ControllersAfter = new();

			public List<AnimatorController> ControllersAdditive = new();
		}

		[System.Serializable]
		public class MenuControlVRC
		{
			public string Target;
			public List<VRCExpressionsMenu.Control> MenuControls = new();
		}

		public enum LogicLayer { Top, FaceTracking, FaceTrackingReact, Expressions, After, Additive }

		public bool FaceTrackingEnabled = false;
		public bool UseLayerWeightDrivers = false;

		public AnimatorControllerVRCLayer LayerFX = new();
		public AnimatorControllerVRCLayer LayerAdditive = new();

		public AnimatorControllerVRCLayer GetLayer(VRCAvatarDescriptor.AnimLayerType Layer)
		{
			if(Layer == VRCAvatarDescriptor.AnimLayerType.FX) return LayerFX;
			else if(Layer == VRCAvatarDescriptor.AnimLayerType.Additive) return LayerAdditive;
			else throw new System.Exception("Only FX and Additive layers are supported!");
		}

		public List<VRCExpressionParameters.Parameter> Parameters = new();
		public List<(string TargetPath, int Order, VRCExpressionsMenu.Control MenuControl)> MenuControls = new();

		public List<Object> UnityResourcesToSave = new();

		public Object OutputHolder;
	}
}

#endif
#endif
