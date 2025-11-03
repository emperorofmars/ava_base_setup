#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT
#if AVA_BASE_SETUP_VRCFTTEMPLATES

using System;
using com.squirrelbite.ava_base_setup.vrchat.VRLabs.AV3Manager;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public static class AVASetupVRCFTApplier
	{
		public static void Apply(AVABaseSetupVRC Setup)
		{
			var avatar = Setup.gameObject.GetComponent<VRCAvatarDescriptor>();
			var setupState = Setup.gameObject.AddComponent<AVASetupStateVRC>();

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

			avatar.expressionParameters = ScriptableObject.CreateInstance<VRCExpressionParameters>();
			avatar.expressionParameters.parameters = Array.Empty<VRCExpressionParameters.Parameter>();
			AV3ManagerFunctions.AddParameters(avatar, setupState.Parameters, null, true, true);

			avatar.expressionsMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
			AV3ManagerFunctions.AddSubMenu(avatar, setupState.FTMenu, setupState.FTMenu.name, null, null, null, true, true);

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
				ret = AnimatorCloner.MergeControllers(ret, State.Layers[Layer].Pre_FT, null, false, 1);

			if(Setup.UseFacialTracking && State.Layers[Layer].FT)
			{
				var animatorLayer1 = new AnimatorControllerLayer
				{
					name = "Face Tracking Settings",
					stateMachine = new AnimatorStateMachine(),
					defaultWeight = 1,
				};
				// todo layer toggle
				ret.AddLayer(animatorLayer1);

				ret = AnimatorCloner.MergeControllers(ret, State.Layers[Layer].FT, null, false, 1);
				if(State.Layers[Layer].FT_React)
					ret = AnimatorCloner.MergeControllers(ret, State.Layers[Layer].FT_React, null, false, 1);

				if(State.Layers[Layer].Mut)
				{
					// todo layer toggle
					ret = AnimatorCloner.MergeControllers(ret, State.Layers[Layer].Mut, null, false, 1);
				}
				if(State.Layers[Layer].Other)
					ret = AnimatorCloner.MergeControllers(ret, State.Layers[Layer].Other, null, false, 1);

				return ret;
			}
			else if(State.Layers[Layer].Mut || State.Layers[Layer].Other)
			{
				if(State.Layers[Layer].Mut)
					ret = AnimatorCloner.MergeControllers(ret, State.Layers[Layer].Mut, null, false, 1);
				if(State.Layers[Layer].Other)
					ret = AnimatorCloner.MergeControllers(ret, State.Layers[Layer].Other, null, false, 1);

				return ret;
			}
			return null;
		}
	}
}

#endif
#endif
#endif
