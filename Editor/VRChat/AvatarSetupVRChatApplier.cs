#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using VRC.SDK3.Avatars.Components;
using System.Linq;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.ScriptableObjects;
using UnityEditor.Animations;
using com.squirrelbite.ava_base_setup.vrchat.VRLabs.AV3Manager;
using System.Collections.Generic;
using System.Globalization;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public static class AvatarSetupVRChatApplier
	{
		public static void Apply(VRCAvatarDescriptor Avatar, AvatarBaseSetupVRChat Setup)
		{
			var state = new SetupStateVRC();
			var context = new AvatarHandlerContextVRChat(Avatar, Setup, state);

			foreach(var handler in HandlerRegistryVRChat.Handlers)
			{
				foreach(IAvatarBehaviour behaviour in Setup.GetComponentsInChildren(handler.HandlesBehaviour).Cast<IAvatarBehaviour>())
				{
					handler.Handle(context, behaviour);
				}
			}

			CreateOutput(Avatar, Setup, state);
		}

		public static void CreateOutput(VRCAvatarDescriptor Avatar, AvatarBaseSetupVRChat Setup, SetupStateVRC State)
		{
			var outputHolder = ScriptableObject.CreateInstance<OutputHolderVRChat>();
			outputHolder.name = Avatar.name;
			State.OutputHolder = outputHolder;
			AssetDatabase.DeleteAsset(AVAConstants.OUTPUT_PATH + Avatar.name + ".asset");
			AssetDatabase.CreateAsset(outputHolder, AVAConstants.OUTPUT_PATH + Avatar.name + ".asset");

			// Ensure controller layers on avatar descriptor
			Avatar.customizeAnimationLayers = true;

			var animatorLayers = new VRCAvatarDescriptor.CustomAnimLayer[]
			{
				new () { type = VRCAvatarDescriptor.AnimLayerType.Base, isDefault = true },
				new () { type = VRCAvatarDescriptor.AnimLayerType.Additive, isDefault = true },
				new () { type = VRCAvatarDescriptor.AnimLayerType.Gesture, isDefault = true },
				new () { type = VRCAvatarDescriptor.AnimLayerType.Action, isDefault = true },
				new () { type = VRCAvatarDescriptor.AnimLayerType.FX, isDefault = true },
			};
			if(Avatar.baseAnimationLayers != null && Avatar.baseAnimationLayers.Length == 5)
			{
				for(int i = 0; i < 5; i++)
				{
					if(Avatar.baseAnimationLayers[i].isDefault == false && Avatar.baseAnimationLayers[i].isEnabled == true && Avatar.baseAnimationLayers[i].animatorController != null)
					{
						animatorLayers[i].isDefault = false;
						animatorLayers[i].isEnabled = true;
						animatorLayers[i].animatorController = Avatar.baseAnimationLayers[i].animatorController;
					}
				}
			}

			// Merge controllers
			if(SetupLayer(State, VRCAvatarDescriptor.AnimLayerType.FX) is var animatorFX && animatorFX != null)
			{
				animatorLayers[4].isDefault = false;
				animatorLayers[4].isEnabled = true;
				animatorLayers[4].animatorController = animatorFX;
				animatorFX.name = "FX";

				AssetDatabase.AddObjectToAsset(animatorFX, outputHolder);
			}
			if(SetupLayer(State, VRCAvatarDescriptor.AnimLayerType.Additive) is var animatorAdditive && animatorAdditive != null)
			{
				animatorLayers[1].isDefault = false;
				animatorLayers[1].isEnabled = true;
				animatorLayers[1].animatorController = animatorAdditive;
				animatorAdditive.name = "Additive";

				AssetDatabase.AddObjectToAsset(animatorAdditive, outputHolder);
			}
			// todo other layers perhaps?

			// Merge all parameters
			Avatar.expressionParameters = ScriptableObject.CreateInstance<VRCExpressionParameters>();
			Avatar.expressionParameters.parameters = System.Array.Empty<VRCExpressionParameters.Parameter>();
			Avatar.expressionParameters.name = "Parameters";
			if(Setup.BaseParameters && Setup.BaseParameters.parameters != null)
				AV3ManagerFunctions.AddParameters(Avatar, Setup.BaseParameters.parameters, null, true, true);
			AV3ManagerFunctions.AddParameters(Avatar, State.Parameters, null, true, true);
			AssetDatabase.AddObjectToAsset(Avatar.expressionParameters, outputHolder);


			// Merge top level menu
			Avatar.expressionsMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
			Avatar.expressionsMenu.name = "Menu Root";
			if(Setup.BaseMenu && Setup.BaseMenu.controls != null)
				AVAVRCUtil.MergeMenuControls(Setup.BaseMenu.controls, Avatar.expressionsMenu, true, State.UnityResourcesToSave);

			// Merge registered menu controls
			foreach(var (target, menuControls) in State.Menus)
			{
				// TODO handle root entries after all have been created
				if(string.IsNullOrEmpty(target) || target == "/")
				{
					var sortedRoot = menuControls.MenuControls.ToList().OrderBy(c => c.Key).SelectMany(c => c.Value).ToList();
					AVAVRCUtil.MergeMenuControls(sortedRoot, Avatar.expressionsMenu, true, State.UnityResourcesToSave);
					continue;
				}
				var targetPath = target.Split("/");
				VRCExpressionsMenu.Control targetControl = null;
				VRCExpressionsMenu targetMenu = Avatar.expressionsMenu;
				foreach(var targetPathElement in targetPath)
				{
					var controlName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(targetPathElement);
					var next = targetMenu.controls.Find(c => c.type == VRCExpressionsMenu.Control.ControlType.SubMenu && c.name.ToLower() == targetPathElement.ToLower());
					if(next != null)
					{
						targetControl = next;
						if(next.subMenu != null)
							targetMenu = next.subMenu;
						else
						{
							targetMenu = next.subMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
							targetMenu.name = controlName;
							AssetDatabase.AddObjectToAsset(next.subMenu, outputHolder);
						}
					}
					else
					{
						targetControl = new VRCExpressionsMenu.Control {
							type = VRCExpressionsMenu.Control.ControlType.SubMenu,
							name = controlName,
							subMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>(),
							subParameters = new VRCExpressionsMenu.Control.Parameter[] {},
						};
						targetControl.subMenu.name = controlName;
						targetMenu.controls.Add(targetControl);
						targetMenu = targetControl.subMenu;
						AssetDatabase.AddObjectToAsset(targetControl.subMenu, outputHolder);
					}
				}
				if(!targetMenu || targetControl == null)
				{
					Debug.LogWarning("Invalid target path for submenu: " + target);
					continue;
				}
				var sorted = menuControls.MenuControls.ToList().OrderBy(c => c.Key).SelectMany(c => c.Value).ToList();
				var newSubmenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
				AVAVRCUtil.MergeMenuControls(targetMenu.controls, newSubmenu, true, State.UnityResourcesToSave);
				AVAVRCUtil.MergeMenuControls(sorted, newSubmenu, true, State.UnityResourcesToSave);
				newSubmenu.name = targetMenu.name;
				targetControl.subMenu = newSubmenu;
				AssetDatabase.AddObjectToAsset(newSubmenu, outputHolder);
			}

			if(Avatar.expressionsMenu)
				AssetDatabase.AddObjectToAsset(Avatar.expressionsMenu, outputHolder);

			// Save other stuff like generated animations
			foreach(var asset in State.UnityResourcesToSave.ToHashSet())
				if(asset)
					AssetDatabase.AddObjectToAsset(asset, outputHolder);

			Avatar.baseAnimationLayers = animatorLayers;

			EditorUtility.SetDirty(outputHolder);
			EditorUtility.SetDirty(Avatar);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private static AnimatorController SetupLayer(SetupStateVRC State, VRCAvatarDescriptor.AnimLayerType Layer)
		{
			var ret = new AnimatorController { name = Layer.ToString() };
			var animatorLayer0 = new AnimatorControllerLayer
			{
				name = "All Parts",
				stateMachine = new AnimatorStateMachine() { name = Layer.ToString() + " - All Parts" }
			};
			State.UnityResourcesToSave.Add(animatorLayer0.stateMachine);
			ret.AddLayer(animatorLayer0);
			foreach(var param in State.GetLayer(Layer).ControllerParameters)
				if(ret.parameters.FirstOrDefault(p => p.name == param.Name) == null)
					ret.AddParameter(new AnimatorControllerParameter {
						name = param.Name,
						type = param.Type,
						defaultFloat = param.DefaultValue,
						defaultInt = (int)param.DefaultValue,
						defaultBool = param.DefaultValue > 0.5,
					});

			if(Layer == VRCAvatarDescriptor.AnimLayerType.FX && State.GetLayer(Layer).DirectBlendPre.Count > 0)
				ret = AnimatorCloner.MergeControllers(ret, MergeDirectBlendTrees(State, State.GetLayer(Layer).DirectBlendPre, "DirectBlend Pre"), null, false, 0, null, State.UnityResourcesToSave);

			foreach(var controller in State.GetLayer(Layer).ControllersPre)
				ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0, null, State.UnityResourcesToSave);

			if(State.FaceTrackingEnabled && State.GetLayer(Layer).ControllersFaceTracking.Count > 0)
			{
				if(Layer == VRCAvatarDescriptor.AnimLayerType.FX && State.UseLayerWeightDrivers)
					SetupFTLayerWeightControlVRC.Apply(State, ret, Layer);

				foreach(var controller in State.GetLayer(Layer).ControllersFaceTracking)
					ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0, null, State.UnityResourcesToSave);
				foreach(var controller in State.GetLayer(Layer).ControllersFaceTrackingReact)
					ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0, null, State.UnityResourcesToSave);
			}

			foreach(var controller in State.GetLayer(Layer).ControllersExpression)
				ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0, null, State.UnityResourcesToSave);

			if(Layer == VRCAvatarDescriptor.AnimLayerType.FX && State.GetLayer(Layer).DirectBlendAfter.Count > 0)
				ret = AnimatorCloner.MergeControllers(ret, MergeDirectBlendTrees(State, State.GetLayer(Layer).DirectBlendAfter, "DirectBlend After"), null, false, 0, null, State.UnityResourcesToSave);

			foreach(var controller in State.GetLayer(Layer).ControllersAfter)
				ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0, null, State.UnityResourcesToSave);
			foreach(var controller in State.GetLayer(Layer).ControllersAdditive)
				ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0, null, State.UnityResourcesToSave);

			return ret;
		}

		private static AnimatorController MergeDirectBlendTrees(SetupStateVRC State, List<(BlendTree Blendtree, string Parameter)> Blendtrees, string Name)
		{
			var ret = new AnimatorController { name = Name };
			var animatorLayer = new AnimatorControllerLayer
			{
				name = Name,
				stateMachine = new AnimatorStateMachine() { name = Name }
			};
			State.UnityResourcesToSave.Add(animatorLayer.stateMachine);

			var directBlend = new BlendTree {
				name = Name,
				blendType = BlendTreeType.Direct,
			};
			var animatorState = animatorLayer.stateMachine.AddState(Name);
			animatorState.motion = directBlend;

			var childMotions = new List<ChildMotion>();
			foreach(var (Blendtree, Parameter) in Blendtrees)
			{
				childMotions.Add(new ChildMotion {
					motion = Blendtree,
					directBlendParameter = Parameter,
					timeScale = 1,
				});
			}
			directBlend.children = childMotions.ToArray();

			State.UnityResourcesToSave.Add(animatorState);
			ret.AddLayer(animatorLayer);
			return ret;
		}
	}
}

#endif
#endif

