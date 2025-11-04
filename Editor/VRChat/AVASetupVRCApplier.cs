#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using com.squirrelbite.ava_base_setup.vrchat.VRLabs.AV3Manager;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public static class AVASetupVRCApplier
	{
		public static void Apply(AVABaseSetupVRC Setup, AVASetupStateVRC setupState)
		{
			var avatar = Setup.gameObject.GetComponent<VRCAvatarDescriptor>();

			// Run the setup producer-components
			if(Setup.PreFacialTracking.ProducerComponent != null)
				Setup.PreFacialTracking.ProducerComponent.Apply();
			else
				setupState.Layers[4].Pre_FT = Setup.PreFacialTracking.Controller;

			if(Setup.UseFacialTracking)
			{
				if(Setup.FacialTrackingSetupType == AVA_FT_Setup_Type.Automatic)
				{
					Setup.FacialTracking.ProducerComponent = Setup.gameObject.GetComponent<AVASetupVRCFTController>();
					if(Setup.FacialTracking.ProducerComponent == null)
						Setup.FacialTracking.ProducerComponent = Setup.gameObject.AddComponent<AVASetupVRCFTController>();
				}
				if(Setup.FacialTracking.ProducerComponent != null)
					Setup.FacialTracking.ProducerComponent.Apply();
				else
					setupState.Layers[4].FT = Setup.FacialTracking.Controller;

				if(Setup.FacialTrackingReact.ProducerComponent != null)
					Setup.FacialTrackingReact.ProducerComponent.Apply();
				else
					setupState.Layers[4].FT_React = Setup.FacialTrackingReact.Controller;
			}

			if(Setup.ManualExpressions.ProducerComponent != null)
				Setup.ManualExpressions.ProducerComponent.Apply();
			else
				setupState.Layers[4].Mut = Setup.ManualExpressions.Controller;

			if(Setup.OtherFunctionality.ProducerComponent != null)
				Setup.OtherFunctionality.ProducerComponent.Apply();
			else
				setupState.Layers[4].Other = Setup.OtherFunctionality.Controller;

			// Ensure layers
			avatar.customizeAnimationLayers = true;

			var animatorLayers = new VRCAvatarDescriptor.CustomAnimLayer[]
			{
				new () { type = VRCAvatarDescriptor.AnimLayerType.Base, isDefault = true },
				new () { type = VRCAvatarDescriptor.AnimLayerType.Additive, isDefault = true },
				new () { type = VRCAvatarDescriptor.AnimLayerType.Gesture, isDefault = true },
				new () { type = VRCAvatarDescriptor.AnimLayerType.Action, isDefault = true },
				new () { type = VRCAvatarDescriptor.AnimLayerType.FX, isDefault = true },
			};
			if(avatar.baseAnimationLayers != null && avatar.baseAnimationLayers.Length == 5)
			{
				for(int i = 0; i < 5; i++)
				{
					if(avatar.baseAnimationLayers[i].isDefault == false && avatar.baseAnimationLayers[i].isEnabled == true && avatar.baseAnimationLayers[i].animatorController != null)
					{
						animatorLayers[i].isDefault = false;
						animatorLayers[i].isEnabled = true;
						animatorLayers[i].animatorController = avatar.baseAnimationLayers[i].animatorController;
					}
				}
			}

			// Setup controllers
			// todo merge with mapped controller as an option ?
			if(SetupLayer(Setup, setupState, 4, "FX") is var animatorFX && animatorFX != null)
			{
				animatorLayers[4].isDefault = false;
				animatorLayers[4].isEnabled = true;
				animatorLayers[4].animatorController = animatorFX;
			}
			if(SetupLayer(Setup, setupState, 1, "Gesture") is var animatorGesture && animatorGesture != null)
			{
				animatorLayers[1].isDefault = false;
				animatorLayers[1].isEnabled = true;
				animatorLayers[1].animatorController = animatorGesture;
			}

			if(avatar.expressionParameters == null)
			{
				avatar.expressionParameters = ScriptableObject.CreateInstance<VRCExpressionParameters>();
				avatar.expressionParameters.parameters = System.Array.Empty<VRCExpressionParameters.Parameter>();
			}
			AV3ManagerFunctions.AddParameters(avatar, setupState.Parameters, null, true, true);

			if(avatar.expressionsMenu == null)
				avatar.expressionsMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
			AV3ManagerFunctions.AddSubMenu(avatar, setupState.FTMenu, "Face Tracking", null, null, AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/adjerry91.vrcft.templates/Icons/FaceTrackingIcon2.png"), true, true);

			avatar.baseAnimationLayers = animatorLayers;
		}

		private static AnimatorController SetupLayer(AVABaseSetupVRC Setup, AVASetupStateVRC State, int Layer, string Name)
		{
			var ret = new AnimatorController { name = Name };
			var animatorLayer0 = new AnimatorControllerLayer
			{
				name = "All Parts",
				stateMachine = new AnimatorStateMachine()
			};
			ret.AddLayer(animatorLayer0);
			ret.AddParameter("GestureLeft", AnimatorControllerParameterType.Int);
			ret.AddParameter("GestureLeftWeight", AnimatorControllerParameterType.Float);
			ret.AddParameter("GestureRight", AnimatorControllerParameterType.Int);
			ret.AddParameter("GestureRightWeight", AnimatorControllerParameterType.Float);

			if(State.Layers[Layer].Pre_FT)
				ret = AnimatorCloner.MergeControllers(ret, State.Layers[Layer].Pre_FT, null, false, 0);

			if(Setup.UseFacialTracking && State.Layers[Layer].FT)
			{
				if(Layer == 4)
				{
					var animatorLayer1 = new AnimatorControllerLayer
					{
						name = "Face Tracking Settings",
						stateMachine = new AnimatorStateMachine(),
						defaultWeight = 1,
					};
					var stateFTOff = new AnimatorState { name = "FT Off", writeDefaultValues = true };
					var stateOn = new AnimatorState { name = "On", writeDefaultValues = true };
					var stateExpressionsOff = new AnimatorState { name = "Expressions Off", writeDefaultValues = true };
					var stateOff = new AnimatorState { name = "Off", writeDefaultValues = true };

					animatorLayer1.stateMachine.AddState(stateFTOff, new Vector3(200, 220));
					animatorLayer1.stateMachine.AddState(stateOn, new Vector3(400, 100));
					animatorLayer1.stateMachine.AddState(stateExpressionsOff, new Vector3(600, 220));
					animatorLayer1.stateMachine.AddState(stateOff, new Vector3(400, 340));

					stateFTOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() {mode = AnimatorConditionMode.Less, parameter = "FacialExpressionsDisabled", threshold = 0.1f },
						},
					});
					stateFTOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() {mode = AnimatorConditionMode.Less, parameter = "FacialExpressionsDisabled", threshold = 0.1f },
						},
					});
					stateFTOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() {mode = AnimatorConditionMode.Greater, parameter = "FacialExpressionsDisabled", threshold = 0.9f },
						},
					});
					stateFTOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() {mode = AnimatorConditionMode.Greater, parameter = "FacialExpressionsDisabled", threshold = 0.9f },
						},
					});
					stateFTOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOff,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() {mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() {mode = AnimatorConditionMode.Greater, parameter = "FacialExpressionsDisabled", threshold = 0.9f },
						},
					});
					stateOn.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateFTOff,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() {mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() {mode = AnimatorConditionMode.Less, parameter = "FacialExpressionsDisabled", threshold = 0.1f },
						},
					});
					stateOn.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOff,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() {mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() {mode = AnimatorConditionMode.Greater, parameter = "FacialExpressionsDisabled", threshold = 0.9f },
						},
					});
					stateOn.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() {mode = AnimatorConditionMode.Greater, parameter = "FacialExpressionsDisabled", threshold = 0.9f },
						},
					});
					stateOn.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() {mode = AnimatorConditionMode.Greater, parameter = "FacialExpressionsDisabled", threshold = 0.9f },
						},
					});
					stateOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateFTOff,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() {mode = AnimatorConditionMode.Less, parameter = "FacialExpressionsDisabled", threshold = 0.1f },
						},
					});
					stateOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateFTOff,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() {mode = AnimatorConditionMode.Less, parameter = "FacialExpressionsDisabled", threshold = 0.1f },
						},
					});
					stateOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() {mode = AnimatorConditionMode.Greater, parameter = "FacialExpressionsDisabled", threshold = 0.9f },
						},
					});
					stateOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() {mode = AnimatorConditionMode.Greater, parameter = "FacialExpressionsDisabled", threshold = 0.9f },
						},
					});
					stateOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() {mode = AnimatorConditionMode.Less, parameter = "FacialExpressionsDisabled", threshold = 0.1f },
						},
					});
					stateOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() {mode = AnimatorConditionMode.Less, parameter = "FacialExpressionsDisabled", threshold = 0.1f },
						},
					});
					stateExpressionsOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() {mode = AnimatorConditionMode.Less, parameter = "FacialExpressionsDisabled", threshold = 0.1f },
						},
					});
					stateExpressionsOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() {mode = AnimatorConditionMode.Less, parameter = "FacialExpressionsDisabled", threshold = 0.1f },
						},
					});
					stateExpressionsOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOff,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() {mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() {mode = AnimatorConditionMode.Greater, parameter = "FacialExpressionsDisabled", threshold = 0.9f },
						},
					});
					stateExpressionsOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateFTOff,
						conditions = new AnimatorCondition[] {
							new() {mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() {mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() {mode = AnimatorConditionMode.Less, parameter = "FacialExpressionsDisabled", threshold = 0.1f },
						},
					});

					var startLayer = ret.layers.Length + 1;
					var ftLayerCount = State.Layers[Layer].FT.layers.Length + (State.Layers[Layer].FT_React ? State.Layers[Layer].FT_React.layers.Length : 0);
					var mutLayerCount = State.Layers[Layer].Mut ? State.Layers[Layer].Mut.layers.Length : 0;

					SetupBehaviour(stateOn, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount + mutLayerCount, 1);
					SetupBehaviour(stateExpressionsOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount, 1);
					SetupBehaviour(stateExpressionsOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer + ftLayerCount, mutLayerCount, 0);
					SetupBehaviour(stateFTOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount, 0);
					SetupBehaviour(stateFTOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer + ftLayerCount, mutLayerCount, 1);
					SetupBehaviour(stateOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount + mutLayerCount, 0);

					ret.AddLayer(animatorLayer1);
				}

				ret = AnimatorCloner.MergeControllers(ret, State.Layers[Layer].FT, null, false, 0);
				if(State.Layers[Layer].FT_React)
					ret = AnimatorCloner.MergeControllers(ret, State.Layers[Layer].FT_React, null, false, 0);

				if(State.Layers[Layer].Mut)
				{
					// todo layer toggle
					ret = AnimatorCloner.MergeControllers(ret, State.Layers[Layer].Mut, null, false, 0);
				}
				if(State.Layers[Layer].Other)
					ret = AnimatorCloner.MergeControllers(ret, State.Layers[Layer].Other, null, false, 0);

				return ret;
			}
			else if(State.Layers[Layer].Mut || State.Layers[Layer].Other)
			{
				if(State.Layers[Layer].Mut)
					ret = AnimatorCloner.MergeControllers(ret, State.Layers[Layer].Mut, null, false, 0);
				if(State.Layers[Layer].Other)
					ret = AnimatorCloner.MergeControllers(ret, State.Layers[Layer].Other, null, false, 0);

				return ret;
			}
			return null;
		}

		private static void SetupBehaviour(AnimatorState State, VRC_AnimatorLayerControl.BlendableLayer Layer, int LayerStart, int LayerCount, float GoalWeight)
		{
			for(int i = LayerStart; i < LayerStart + LayerCount; i++)
			{
				var behaviour = State.AddStateMachineBehaviour<VRCAnimatorLayerControl>();
				behaviour.playable = Layer;
				behaviour.layer = i;
				behaviour.goalWeight = GoalWeight;
			}
		}
	}
}

#endif
#endif
