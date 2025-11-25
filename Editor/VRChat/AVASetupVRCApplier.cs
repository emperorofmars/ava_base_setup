#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using System.Linq;
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

			var outputHolder = ScriptableObject.CreateInstance<AVAOutputHolder>();
			outputHolder.name = avatar.name;
			setupState.OutputHolder = outputHolder;
			AssetDatabase.DeleteAsset(AVAConstants.OUTPUT_PATH + avatar.name + ".asset");
			AssetDatabase.CreateAsset(outputHolder, AVAConstants.OUTPUT_PATH + avatar.name + ".asset");

			foreach(var menu in Setup.AvatarMenus)
				setupState.AvatarMenuControls.AddRange(menu.controls);
			foreach(var subMenu in Setup.AvatarSubMenus)
				setupState.AvatarSubMenuControls.Add(new() { Target = subMenu.Target, MenuControls = new List<VRCExpressionsMenu.Control>(subMenu.Menu.controls) });
			if(Setup.UseFaceTracking && Setup.FaceTrackingSetupType == AVA_FT_Setup_Type.Manual)
				foreach(var menu in Setup.AvatarMenusFaceTracking)
					setupState.AvatarMenuControls.AddRange(menu.controls);
			foreach(var parameter in Setup.AvatarParameters)
				setupState.Parameters.AddRange(parameter.parameters);
			if(Setup.UseFaceTracking && Setup.FaceTrackingSetupType == AVA_FT_Setup_Type.Manual)
				foreach(var parameter in Setup.AvatarParametersFaceTracking)
					setupState.Parameters.AddRange(parameter.parameters);

			foreach(var layer in Setup.LayerPreFT)
				if(layer.ProducerComponent != null)
					layer.ProducerComponent.Apply();
				else
					foreach(var c in layer.Controllers)
						if(AVAConstants.ControllerTypeToIndex.ContainsKey(c.Mapping) && c.Controller)
							setupState.Layers[AVAConstants.ControllerTypeToIndex[c.Mapping]].Pre_FT.Add(c.Controller);

			// Run setup for all layers
			if(Setup.UseFaceTracking)
			{
				if(Setup.FaceTrackingSetupType == AVA_FT_Setup_Type.Automatic)
				{
					if(!Setup.gameObject.TryGetComponent<AVAVRCFTProducer>(out var autoFT))
						autoFT = Setup.gameObject.AddComponent<AVAVRCFTProducer>();
					autoFT.Apply();
				}
				else
				{
					foreach(var layer in Setup.LayerFT)
						if(layer.ProducerComponent != null)
							layer.ProducerComponent.Apply();
						else
							foreach(var c in layer.Controllers)
								if(AVAConstants.ControllerTypeToIndex.ContainsKey(c.Mapping) && c.Controller)
									setupState.Layers[AVAConstants.ControllerTypeToIndex[c.Mapping]].FT.Add(c.Controller);
				}
				foreach(var layer in Setup.LayerFTReact)
					if(layer.ProducerComponent != null)
						layer.ProducerComponent.Apply();
					else
						foreach(var c in layer.Controllers)
							if(AVAConstants.ControllerTypeToIndex.ContainsKey(c.Mapping) && c.Controller)
								setupState.Layers[AVAConstants.ControllerTypeToIndex[c.Mapping]].FT_React.Add(c.Controller);
			}
			foreach(var layer in Setup.LayerManualExpressions)
				if(layer.ProducerComponent != null)
					layer.ProducerComponent.Apply();
				else
					foreach(var c in layer.Controllers)
						if(AVAConstants.ControllerTypeToIndex.ContainsKey(c.Mapping) && c.Controller)
							setupState.Layers[AVAConstants.ControllerTypeToIndex[c.Mapping]].Mut.Add(c.Controller);

			foreach(var layer in Setup.LayerPost)
				if(layer.ProducerComponent != null)
					layer.ProducerComponent.Apply();
				else
					foreach(var c in layer.Controllers)
						if(AVAConstants.ControllerTypeToIndex.ContainsKey(c.Mapping) && c.Controller)
							setupState.Layers[AVAConstants.ControllerTypeToIndex[c.Mapping]].Other.Add(c.Controller);

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

			// Merge controllers
			if(SetupLayer(Setup, setupState, 4, "FX") is var animatorFX && animatorFX != null)
			{
				animatorLayers[4].isDefault = false;
				animatorLayers[4].isEnabled = true;
				animatorLayers[4].animatorController = animatorFX;
				animatorFX.name = "AVA Base Setup FX";

				AssetDatabase.AddObjectToAsset(animatorFX, outputHolder);
			}
			if(SetupLayer(Setup, setupState, 1, "Additive") is var animatorAdditive && animatorAdditive != null)
			{
				animatorLayers[1].isDefault = false;
				animatorLayers[1].isEnabled = true;
				animatorLayers[1].animatorController = animatorAdditive;
				animatorAdditive.name = "AVA Base Setup Additive";

				AssetDatabase.AddObjectToAsset(animatorAdditive, outputHolder);
			}
			// todo other layers perhaps?

			// Merge all parameters
			avatar.expressionParameters = ScriptableObject.CreateInstance<VRCExpressionParameters>();
			avatar.expressionParameters.parameters = System.Array.Empty<VRCExpressionParameters.Parameter>();
			avatar.expressionParameters.name = "AVA Base Setup Parameters";
			AV3ManagerFunctions.AddParameters(avatar, setupState.Parameters, null, true, true);
			AssetDatabase.AddObjectToAsset(avatar.expressionParameters, outputHolder);

			// Merge top level menus
			avatar.expressionsMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
			avatar.expressionsMenu.name = "AVA Base Setup Menu";
			var menuList = new List<VRCExpressionsMenu.Control>(setupState.AvatarMenuControls);
			setupState.AvatarMenuControlsLast.Reverse();
			menuList.AddRange(setupState.AvatarMenuControlsLast);
			AVAVRCUtil.MergeMenuControls(menuList, avatar.expressionsMenu, true, setupState.UnityResourcesToSave);

			// Merge submenus
			foreach(var subMenu in setupState.AvatarSubMenuControls)
			{
				if(string.IsNullOrWhiteSpace(subMenu.Target) || subMenu.MenuControls == null || subMenu.MenuControls.Count == 0) continue;
				var targetPath = subMenu.Target.Split("/");
				VRCExpressionsMenu.Control targetControl = null;
				VRCExpressionsMenu targetMenu = avatar.expressionsMenu;
				foreach(var targetPathElement in targetPath)
				{
					if(targetMenu.controls.Find(c => c.type == VRCExpressionsMenu.Control.ControlType.SubMenu && c.name == targetPathElement) is var next && next != null && next.subMenu != null)
					{
						targetControl = next;
						targetMenu = next.subMenu;
					}
				}
				if(targetMenu && targetControl != null)
				{
					var newSubmenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
					AVAVRCUtil.MergeMenuControls(targetMenu.controls, newSubmenu, true, setupState.UnityResourcesToSave);
					AVAVRCUtil.MergeMenuControls(subMenu.MenuControls, newSubmenu, true, setupState.UnityResourcesToSave);
					newSubmenu.name = targetMenu.name;
					targetControl.subMenu = newSubmenu;
					AssetDatabase.AddObjectToAsset(newSubmenu, outputHolder);
				}
				else
				{
					Debug.LogWarning("Invalid target path for submenu: " + subMenu.Target);
				}
			}

			AssetDatabase.AddObjectToAsset(avatar.expressionsMenu, outputHolder);

			// Save other stuff like generated animations
			foreach(var asset in setupState.UnityResourcesToSave.ToHashSet())
				if(asset)
					AssetDatabase.AddObjectToAsset(asset, outputHolder);

			avatar.baseAnimationLayers = animatorLayers;

			EditorUtility.SetDirty(outputHolder);
			EditorUtility.SetDirty(avatar);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private static AnimatorController SetupLayer(AVABaseSetupVRC Setup, AVASetupStateVRC State, int Layer, string Name)
		{
			var ret = new AnimatorController { name = Name };
			var animatorLayer0 = new AnimatorControllerLayer
			{
				name = "All Parts",
				stateMachine = new AnimatorStateMachine() { name = Name + " - All Parts" }
			};
			State.UnityResourcesToSave.Add(animatorLayer0.stateMachine);
			ret.AddLayer(animatorLayer0);
			ret.AddParameter("GestureLeft", AnimatorControllerParameterType.Int);
			ret.AddParameter("GestureLeftWeight", AnimatorControllerParameterType.Float);
			ret.AddParameter("GestureRight", AnimatorControllerParameterType.Int);
			ret.AddParameter("GestureRightWeight", AnimatorControllerParameterType.Float);

			foreach(var controller in State.Layers[Layer].Pre_FT)
				ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0, null, State.UnityResourcesToSave);

			if(Setup.UseFaceTracking && State.Layers[Layer].FT.Count > 0)
			{
				if(Layer == 4)
				{
					var animatorLayer1 = new AnimatorControllerLayer
					{
						name = "Face Tracking Settings",
						stateMachine = new AnimatorStateMachine { name = "Face Tracking Settings" },
						defaultWeight = 1,
					};
					State.UnityResourcesToSave.Add(animatorLayer1.stateMachine);
					var stateFTOff = new AnimatorState { name = "FT Off", writeDefaultValues = true };
					var stateOn = new AnimatorState { name = "On", writeDefaultValues = true };
					var stateExpressionsOff = new AnimatorState { name = "Expressions Off", writeDefaultValues = true };
					var stateOff = new AnimatorState { name = "Off", writeDefaultValues = true };

					State.UnityResourcesToSave.Add(stateFTOff);
					State.UnityResourcesToSave.Add(stateOn);
					State.UnityResourcesToSave.Add(stateExpressionsOff);
					State.UnityResourcesToSave.Add(stateOff);

					animatorLayer1.stateMachine.AddState(stateFTOff, new Vector3(200, 220));
					animatorLayer1.stateMachine.AddState(stateOn, new Vector3(400, 100));
					animatorLayer1.stateMachine.AddState(stateExpressionsOff, new Vector3(600, 220));
					animatorLayer1.stateMachine.AddState(stateOff, new Vector3(400, 340));

					void addTransition(AnimatorState SourceState, AnimatorStateTransition t)
					{
						SourceState.AddTransition(t);
						State.UnityResourcesToSave.Add(t);
					}

					addTransition(stateFTOff, new AnimatorStateTransition {
						name = "FT off to All on",
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled", threshold = 0 },
						},
					});
					addTransition(stateFTOff, new AnimatorStateTransition {
						name = "FT off to All on",
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled", threshold = 0 },
						},
					});
					addTransition(stateFTOff, new AnimatorStateTransition {
						name = "FT off to Expressions off",
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					addTransition(stateFTOff, new AnimatorStateTransition {
						name = "FT off to Expressions off",
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					addTransition(stateFTOff, new AnimatorStateTransition {
						name = "FT off to All off",
						destinationState = stateOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					addTransition(stateOn, new AnimatorStateTransition {
						name = "All on to FT off",
						destinationState = stateFTOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
						},
					});
					addTransition(stateOn, new AnimatorStateTransition {
						name = "All on to All off",
						destinationState = stateOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					addTransition(stateOn, new AnimatorStateTransition {
						name = "All on to Expressions off",
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					addTransition(stateOn, new AnimatorStateTransition {
						name = "All on to Expressions off",
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					addTransition(stateOff, new AnimatorStateTransition {
						name = "All off to FT off",
						destinationState = stateFTOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
						},
					});
					addTransition(stateOff, new AnimatorStateTransition {
						name = "All off to FT off",
						destinationState = stateFTOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
						},
					});
					addTransition(stateOff, new AnimatorStateTransition {
						name = "All off to Expressions off",
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					addTransition(stateOff, new AnimatorStateTransition {
						name = "All off to Expressions off",
						destinationState = stateExpressionsOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled",  },
						},
					});
					addTransition(stateOff, new AnimatorStateTransition {
						name = "All off to All on",
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
						},
					});
					addTransition(stateOff, new AnimatorStateTransition {
						name = "All off to All on",
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
						},
					});
					addTransition(stateExpressionsOff, new AnimatorStateTransition {
						name = "Expressions off to All on",
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
						},
					});
					addTransition(stateExpressionsOff, new AnimatorStateTransition {
						name = "Expressions off to All on",
						destinationState = stateOn,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
							new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
						},
					});
					addTransition(stateExpressionsOff, new AnimatorStateTransition {
						name = "Expressions off to All off",
						destinationState = stateOff,
						conditions = new AnimatorCondition[] {
							new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
							new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
						},
					});
					addTransition(stateExpressionsOff, new AnimatorStateTransition {
						name = "Expressions off to FT off",
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
						mutLayerCount += controller.layers.Length;

					SetupBehaviour(State, stateOn, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount + mutLayerCount, 1f);
					SetupBehaviour(State, stateExpressionsOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount, 1f);
					SetupBehaviour(State, stateExpressionsOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer + ftLayerCount, mutLayerCount, 0f);
					SetupBehaviour(State, stateFTOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount, 0f);
					SetupBehaviour(State, stateFTOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer + ftLayerCount, mutLayerCount, 1f);
					SetupBehaviour(State, stateOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount + mutLayerCount, 0f);

					ret.AddLayer(animatorLayer1);
				}

				foreach(var controller in State.Layers[Layer].FT)
					ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0, null, State.UnityResourcesToSave);
				foreach(var controller in State.Layers[Layer].FT_React)
					ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0, null, State.UnityResourcesToSave);
				foreach(var controller in State.Layers[Layer].Mut)
					ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0, null, State.UnityResourcesToSave);
				foreach(var controller in State.Layers[Layer].Other)
					ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0, null, State.UnityResourcesToSave);

				return ret;
			}
			else if(State.Layers[Layer].Pre_FT.Count > 0 || State.Layers[Layer].Mut.Count > 0 || State.Layers[Layer].Other.Count > 0)
			{
				foreach(var controller in State.Layers[Layer].Mut)
					ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0, null, State.UnityResourcesToSave);
				foreach(var controller in State.Layers[Layer].Other)
					ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0, null, State.UnityResourcesToSave);
				return ret;
			}
			return null;
		}

		private static void SetupBehaviour(AVASetupStateVRC State, AnimatorState AnimState, VRC_AnimatorLayerControl.BlendableLayer Layer, int LayerStart, int LayerCount, float GoalWeight)
		{
			for(int i = LayerStart; i < LayerStart + LayerCount; i++)
			{
				var behaviour = AnimState.AddStateMachineBehaviour<VRCAnimatorLayerControl>();
				behaviour.playable = Layer;
				behaviour.layer = i;
				behaviour.goalWeight = GoalWeight;
				State.UnityResourcesToSave.Add(behaviour);
			}
		}
	}
}

#endif
#endif
