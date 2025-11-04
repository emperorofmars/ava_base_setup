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
		const string OUTPUT_PATH = "Packages/com.squirrelbite.ava_base_setup/Output/";

		public static void Apply(AVABaseSetupVRC Setup, AVASetupStateVRC setupState)
		{
			var avatar = Setup.gameObject.GetComponent<VRCAvatarDescriptor>();

			var outputHolder = ScriptableObject.CreateInstance<AVAOutputHolder>();
			outputHolder.name = avatar.name;
			AssetDatabase.DeleteAsset(OUTPUT_PATH + avatar.name + ".asset");
			AssetDatabase.CreateAsset(outputHolder, OUTPUT_PATH + avatar.name + ".asset");

			if(Setup.LayerPostAdditiveController)
				setupState.Layers[1].Other.Add(Setup.LayerPostAdditiveController);

			foreach(var layer in Setup.LayerPreFT)
				if(layer.ProducerComponent != null)
					layer.ProducerComponent.Apply();
				else
					setupState.Layers[4].Pre_FT.Add(layer.Controller);

			if(Setup.UseFacialTracking)
			{
				if(Setup.FacialTrackingSetupType == AVA_FT_Setup_Type.Automatic)
				{
					if(!Setup.gameObject.TryGetComponent<AVASetupVRCFTController>(out var autoFT))
						autoFT = Setup.gameObject.AddComponent<AVASetupVRCFTController>();
					autoFT.Apply();
				}
				else
				{
					foreach(var layer in Setup.LayerFT)
						if(layer.ProducerComponent != null)
							layer.ProducerComponent.Apply();
						else
							setupState.Layers[4].FT.Add(layer.Controller);
				}
				foreach(var layer in Setup.LayerFTReact)
					if(layer.ProducerComponent != null)
						layer.ProducerComponent.Apply();
					else
						setupState.Layers[4].FT_React.Add(layer.Controller);
			}
			foreach(var layer in Setup.LayerManualExpressions)
				if(layer.ProducerComponent != null)
					layer.ProducerComponent.Apply();
				else
					setupState.Layers[4].Mut.Add(layer.Controller);

			foreach(var layer in Setup.LayerPost)
				if(layer.ProducerComponent != null)
					layer.ProducerComponent.Apply();
				else
					setupState.Layers[4].Other.Add(layer.Controller);

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
			if(SetupLayer(Setup, setupState, 4, "FX") is var animatorFX && animatorFX != null)
			{
				animatorLayers[4].isDefault = false;
				animatorLayers[4].isEnabled = true;
				if(animatorLayers[4].animatorController is AnimatorController existingControllerFX && existingControllerFX != null)
					animatorLayers[4].animatorController = AnimatorCloner.MergeControllers(animatorFX, existingControllerFX, null, false, 0);
				else
					animatorLayers[4].animatorController = animatorFX;
				animatorFX.name = "FX";

				AssetDatabase.AddObjectToAsset(animatorFX, outputHolder);
			}
			if(SetupLayer(Setup, setupState, 1, "Gesture") is var animatorGesture && animatorGesture != null)
			{
				animatorLayers[1].isDefault = false;
				animatorLayers[1].isEnabled = true;
				if(animatorLayers[1].animatorController is AnimatorController existingControllerGesture && existingControllerGesture != null)
					animatorLayers[1].animatorController = AnimatorCloner.MergeControllers(animatorGesture, existingControllerGesture, null, false, 0);
				else
					animatorLayers[1].animatorController = animatorGesture;
				animatorGesture.name = "Gesture";

				AssetDatabase.AddObjectToAsset(animatorGesture, outputHolder);
			}

			if(avatar.expressionParameters == null)
			{
				avatar.expressionParameters = ScriptableObject.CreateInstance<VRCExpressionParameters>();
				avatar.expressionParameters.parameters = System.Array.Empty<VRCExpressionParameters.Parameter>();
				avatar.expressionParameters.name = "AVA Base Setup Parameters";

				AssetDatabase.AddObjectToAsset(avatar.expressionParameters, outputHolder);
			}
			AV3ManagerFunctions.AddParameters(avatar, setupState.Parameters, null, true, true);

			if(avatar.expressionsMenu == null)
			{
				avatar.expressionsMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
				avatar.expressionsMenu.name = "AVA Base Setup Menu";

				AssetDatabase.AddObjectToAsset(avatar.expressionsMenu, outputHolder);
			}
			AV3ManagerFunctions.AddSubMenu(avatar, setupState.FTMenu, "Face Tracking", null, new VRCExpressionsMenu.Control.Parameter(), AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/adjerry91.vrcft.templates/Icons/FaceTrackingIcon2.png"), true, true);

			AssetDatabase.SaveAssets();

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

			foreach(var controller in State.Layers[Layer].Pre_FT)
				ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0);

			if(Setup.UseFacialTracking && State.Layers[Layer].FT.Count > 0)
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
							new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled", threshold = 0 },
						},
					});
					stateFTOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled", threshold = 0 },
						},
					});
					stateFTOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					stateFTOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					stateFTOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					stateOn.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateFTOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
						},
					});
					stateOn.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					stateOn.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					stateOn.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					stateOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateFTOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
						},
					});
					stateOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateFTOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
						},
					});
					stateOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					stateOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled",  },
						},
					});
					stateOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
						},
					});
					stateOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
						},
					});
					stateExpressionsOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
						},
					});
					stateExpressionsOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
						},
					});
					stateExpressionsOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					stateExpressionsOff.AddTransition(new AnimatorStateTransition
					{
						destinationState = stateFTOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
						},
					});

					var startLayer = ret.layers.Length + 2;
					int ftLayerCount = -1;
					foreach(var controller in State.Layers[Layer].FT)
						ftLayerCount += controller.layers.Length;
					foreach(var controller in State.Layers[Layer].FT_React)
						ftLayerCount += controller.layers.Length;
					var mutLayerCount = 0;
					foreach(var controller in State.Layers[Layer].Mut)
						ftLayerCount += controller.layers.Length;

					SetupBehaviour(stateOn, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount + mutLayerCount, 1);
					SetupBehaviour(stateExpressionsOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount, 1);
					SetupBehaviour(stateExpressionsOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer + ftLayerCount, mutLayerCount, 0);
					SetupBehaviour(stateFTOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount, 0);
					SetupBehaviour(stateFTOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer + ftLayerCount, mutLayerCount, 1);
					SetupBehaviour(stateOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount + mutLayerCount, 0);

					ret.AddLayer(animatorLayer1);
				}

				foreach(var controller in State.Layers[Layer].FT)
					ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0);
				foreach(var controller in State.Layers[Layer].FT_React)
					ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0);
				foreach(var controller in State.Layers[Layer].Mut)
					ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0);
				foreach(var controller in State.Layers[Layer].Other)
					ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0);

				return ret;
			}
			else if(State.Layers[Layer].Pre_FT.Count > 0 || State.Layers[Layer].Mut.Count > 0 || State.Layers[Layer].Other.Count > 0)
			{
				foreach(var controller in State.Layers[Layer].Mut)
					ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0);
				foreach(var controller in State.Layers[Layer].Other)
					ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0);
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
