#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System;
using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.ava_base_setup.util;
using com.squirrelbite.ava_base_setup.vrchat.VRLabs.AV3Manager;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public class FaceTrackingHandlerVRC : IAvatarBehaviourHandlerVRChat
	{
		public override Type HandlesBehaviour => typeof(FaceTrackingVRC);
		public override uint Priority => 1;
		public override uint Order => 100;
		public override string Label => "VRChat Face Tracking";

		public static (AnimatorController Controller, VRCExpressionParameters Parameters, VRCExpressionsMenu Menu) LoadTemplates(FaceTrackingVRC Behaviour)
		{
			AnimatorController controllerFX = null;
			VRCExpressionParameters parameters = null;
			VRCExpressionsMenu menuFT = null;
			var ftKind = Behaviour.FTSetup == FT_Setup.Automatic ? FTTypeMatcher.Match(Behaviour.FTMesh) : (int)Behaviour.FTType;
			if(ftKind == (int)FT_Type.UnifiedExpressions)
			{
				controllerFX = AssetDatabase.LoadAssetAtPath<AnimatorController>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "Unified Expressions Blendshapes/FX - Face Tracking - UE Blendshapes.controller");
				parameters = AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "Unified Expressions Blendshapes/Parameters - Face Tracking - UE Blendshapes.asset");
				menuFT = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "Unified Expressions Blendshapes/Face Tracking Control - UE Blendshapes.asset");
			}
			else if(ftKind == (int)FT_Type.UnifiedExpressionsTongueSteps)
			{
				controllerFX = AssetDatabase.LoadAssetAtPath<AnimatorController>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "Unified Expressions Blendshapes/FX - Face Tracking - UE Blendshapes TongueSteps.controller");
				parameters = AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "Unified Expressions Blendshapes/Parameters - Face Tracking - UE Blendshapes TongueSteps.asset");
				menuFT = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "Unified Expressions Blendshapes/Face Tracking Control - UE Blendshapes.asset");
			}
			else if(ftKind == (int)FT_Type.SRanipal)
			{
				controllerFX = AssetDatabase.LoadAssetAtPath<AnimatorController>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "SRanipal Blendshapes/FX - Face Tracking - SRanipal Blendshapes.controller");
				parameters = AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "SRanipal Blendshapes/Parameters - Face Tracking - SRanipal Blendshapes.asset");
				menuFT = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "SRanipal Blendshapes/Face Tracking Control - SRanipal Blendshapes.asset");
			}
			else if(ftKind == (int)FT_Type.ARKit)
			{
				controllerFX = AssetDatabase.LoadAssetAtPath<AnimatorController>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "ARkit Blendshapes/FX - Face Tracking - ARKit Blendshapes.controller");
				parameters = AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "ARkit Blendshapes/Parameters - Face Tracking - ARkit Blendshapes.asset");
				menuFT = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "ARkit Blendshapes/Face Tracking Control  - ARKit Blendshapes.asset");
			}
			else
			{
				Debug.LogError("FT_Setup: Could not determine facial tracking type!");
			}
			return (controllerFX, parameters, menuFT);
		}

		public override void Handle(AvatarHandlerContextVRChat Context, IAvatarBehaviour Behaviour)
		{
			var ftBehaviour = Behaviour as FaceTrackingVRC;

#if AVA_BASE_SETUP_VRCFTTEMPLATES
			var FTMesh = ftBehaviour.FTMesh != null ? ftBehaviour.FTMesh : FTTypeMatcher.DetectFaceMesh(Context.Avatar.gameObject);
			if(FTMesh == null)
			{
				Debug.LogError("FT_Setup: Could not determine SkinnedMeshRenderer for face tracking!");
				return;
			}

			try {
				(var controllerFX, var parameters, var menuFT) = LoadTemplates(ftBehaviour);
				if(!controllerFX || !parameters || !menuFT)
					return;

				Context.RegisterParameters(parameters);

				var motionMatch = "Body";
				var motionRetarget = AnimationPathUtil.GetPath(Context.Avatar.transform, FTMesh.transform, true);

				var tmpMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
				var ftSubMenu = AVAVRCUtil.MergeMenuControls(menuFT.controls, ScriptableObject.CreateInstance<VRCExpressionsMenu>(), true, Context.UnityResourcesToSave);
				ftSubMenu.name = menuFT.name;
				Context.SaveResource(ftSubMenu);
				tmpMenu.controls.Add(new()
				{
					icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/adjerry91.vrcft.templates/Icons/FaceTrackingIcon2.png"),
					name = "Face Tracking",
					type = VRCExpressionsMenu.Control.ControlType.SubMenu,
					subMenu = ftSubMenu,
				});

				foreach(var control in tmpMenu.controls)
					Context.RegisterMenuControl("", 1000, control);

				var AnimationRepather = new System.Func<AnimationClip, AnimationClip>(SourceClip => {
					var newClip = AnimationPathUtil.RepathClip(SourceClip, motionRetarget, motionMatch);
					Context.SaveResource(newClip);
					return newClip;
				});

				var mergedFX = AnimatorCloner.MergeControllers(new AnimatorController(), controllerFX, null, false, 1, AnimationRepather, Context.UnityResourcesToSave);
				if(ftBehaviour.RemoveEyetrackingDrivers)
					RemoveUnwantedDrivers<VRCAnimatorTrackingControl>(mergedFX, "Tracking_State", new() { "Eye Tracking Enabled", "Eye Tracking Disabled" });
				Context.RegisterController(VRCAvatarDescriptor.AnimLayerType.FX, SetupStateVRC.LogicLayer.FaceTracking, mergedFX);

				var controllerAdditive = AssetDatabase.LoadAssetAtPath<AnimatorController>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "Eye Rotation/Additive - Eye Tracking - Eye Rotation.controller");
				var mergedAdditive = AnimatorCloner.MergeControllers(new AnimatorController(), controllerAdditive, null, false, 1, AnimationRepather, Context.UnityResourcesToSave);
				if(ftBehaviour.RemoveEyetrackingDrivers)
					RemoveUnwantedDrivers<VRCAnimatorTrackingControl>(mergedAdditive, "Eye_Tracking_State", new() { "Native Eye Tracking", "VRCFT Eye Tracking" });
				Context.RegisterController(VRCAvatarDescriptor.AnimLayerType.Additive, SetupStateVRC.LogicLayer.FaceTracking, mergedAdditive);

				Context.SetFaceTrackingEnabled(true);
				Context.SetUseLayerWeightDrivers(true);
			}
			catch(System.Exception e)
			{
				Debug.LogError("FT_Setup: an error occurred during setup:" + e.Message);
				Debug.LogException(e);
			}
#else
			Debug.LogWarning("FT_Setup: face tracking templates are not installed!");
#endif
		}

		public override List<(string Parameter, VRCExpressionParameters.ValueType ValueType)> GetParameters(IAvatarBehaviour Behaviour)
		{
			var ftBehaviour = Behaviour as FaceTrackingVRC;
			var ret = new List<(string Parameter, VRCExpressionParameters.ValueType ValueType)>();
#if AVA_BASE_SETUP_VRCFTTEMPLATES
			(var controllerFX, var parameters, var menuFT) = LoadTemplates(ftBehaviour);
			if(!controllerFX || !parameters || !menuFT)
				return ret;
			if(parameters && parameters.parameters.Count() > 0)
				foreach(var parameter in parameters.parameters)
					if(parameter.networkSynced)
						ret.Add((parameter.name, parameter.valueType));
#endif
			return ret;
		}

		private static void RemoveUnwantedDrivers<T>(AnimatorController Controller, string LayerName, List<string> StateNames)
		{
			if(Controller.layers.FirstOrDefault(l => l.name == LayerName) is var layer && layer != null)
				foreach(var state in layer.stateMachine.states)
					if(StateNames.Contains(state.state.name))
						for(int i = 0; i < state.state.behaviours.Length; i++)
							if(state.state.behaviours[i] is T)
							{
								var behaviours = state.state.behaviours;
								ArrayUtility.RemoveAt(ref behaviours, i);
								state.state.behaviours = behaviours;
								i--;
							}
		}

		public override VisualElement CreateGUI(IAvatarBehaviour Behaviour)
		{
			var ftBehaviour = Behaviour as FaceTrackingVRC;

			var ftKind = ftBehaviour.FTSetup == FT_Setup.Automatic ? FTTypeMatcher.Match(ftBehaviour.FTMesh) : (int)ftBehaviour.FTType;

			return ftKind >= 0 ? new Label("Face Fracking Setup: " + ((FT_Type)ftKind).ToString()) : new Label("Avatar doesn't support known face tracking method!");
		}
	}
}

#endif
#endif
