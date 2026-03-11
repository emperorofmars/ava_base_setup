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

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public static class AvatarSetupVRChatApplier
	{
		public static void Apply(VRCAvatarDescriptor Avatar, AvatarBaseSetupVRChat Setup)
		{
			var state = new SetupStateVRC();
			var context = new AvatarHandlerContextVRChat(Avatar, Setup, state);

			// TODO
			/*if(Setup.UseFaceTracking)
			{
				if(Setup.FaceTrackingSetupType == FT_Setup.Automatic)
				{
					if(!Setup.gameObject.TryGetComponent<AVAFaceTrackingProducerVRC>(out var autoFT))
						autoFT = Setup.gameObject.AddComponent<AVAFaceTrackingProducerVRC>();
					autoFT.Apply(Root.gameObject);
				}
			}*/

			foreach(var handler in AvatarHandlerRegistryVRChat.Handlers)
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
			AV3ManagerFunctions.AddParameters(Avatar, State.Parameters, null, true, true);
			AssetDatabase.AddObjectToAsset(Avatar.expressionParameters, outputHolder);

			// Merge top level menus
			Avatar.expressionsMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
			Avatar.expressionsMenu.name = "Menu Root";
			/*var menuList = new List<VRCExpressionsMenu.Control>(State.AvatarMenuControls);
			State.AvatarMenuControlsLast.Reverse();
			menuList.AddRange(State.AvatarMenuControlsLast);
			AVAVRCUtil.MergeMenuControls(menuList, Avatar.expressionsMenu, true, State.UnityResourcesToSave);

			// Merge submenus
			foreach(var subMenu in State.AvatarSubMenuControls)
			{
				if(string.IsNullOrWhiteSpace(subMenu.Target) || subMenu.MenuControls == null || subMenu.MenuControls.Count == 0) continue;
				var targetPath = subMenu.Target.Split("/");
				VRCExpressionsMenu.Control targetControl = null;
				VRCExpressionsMenu targetMenu = Avatar.expressionsMenu;
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
					AVAVRCUtil.MergeMenuControls(targetMenu.controls, newSubmenu, true, State.UnityResourcesToSave);
					AVAVRCUtil.MergeMenuControls(subMenu.MenuControls, newSubmenu, true, State.UnityResourcesToSave);
					newSubmenu.name = targetMenu.name;
					targetControl.subMenu = newSubmenu;
					AssetDatabase.AddObjectToAsset(newSubmenu, outputHolder);
				}
				else
				{
					Debug.LogWarning("Invalid target path for submenu: " + subMenu.Target);
				}
			}*/

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
			var _weight = "_weight";
			ret.AddParameter(new AnimatorControllerParameter {
				name = _weight,
				type = AnimatorControllerParameterType.Float,
				defaultFloat = 1,
			});
			ret.AddParameter("GestureLeft", AnimatorControllerParameterType.Int);
			ret.AddParameter("GestureLeftWeight", AnimatorControllerParameterType.Float);
			ret.AddParameter("GestureRight", AnimatorControllerParameterType.Int);
			ret.AddParameter("GestureRightWeight", AnimatorControllerParameterType.Float);

			if(Layer == VRCAvatarDescriptor.AnimLayerType.FX && State.GetLayer(Layer).DirectBlendPre.Count > 0)
				ret = AnimatorCloner.MergeControllers(ret, MergeDirectBlendTrees(State, State.GetLayer(Layer).DirectBlendPre, "DirectBlend Pre", _weight), null, false, 0, null, State.UnityResourcesToSave);

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
				ret = AnimatorCloner.MergeControllers(ret, MergeDirectBlendTrees(State, State.GetLayer(Layer).DirectBlendAfter, "DirectBlend After", _weight), null, false, 0, null, State.UnityResourcesToSave);

			foreach(var controller in State.GetLayer(Layer).ControllersAfter)
				ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0, null, State.UnityResourcesToSave);
			foreach(var controller in State.GetLayer(Layer).ControllersAdditive)
				ret = AnimatorCloner.MergeControllers(ret, controller, null, false, 0, null, State.UnityResourcesToSave);

			return ret;
		}

		private static AnimatorController MergeDirectBlendTrees(SetupStateVRC State, List<BlendTree> Blendtrees, string Name, string WeightParam)
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
			foreach(var blendtree in Blendtrees)
			{
				childMotions.Add(new ChildMotion {
					motion = blendtree,
					directBlendParameter = WeightParam,
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

