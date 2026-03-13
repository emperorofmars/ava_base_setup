#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.ava_base_setup.oooold;
using com.squirrelbite.ava_base_setup.vrchat.VRLabs.AV3Manager;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.squirrelbite.ava_base_setup.vrchat.oooold
{
	public static class AVASetupVRCApplier
	{
		public static void Apply(AVABaseSetupVRC Setup, AVASetupStateVRC Root)
		{
			var avatar = Root.gameObject.GetComponent<VRCAvatarDescriptor>();

			var outputHolder = ScriptableObject.CreateInstance<AVAOutputHolder>();
			outputHolder.name = avatar.name;
			Root.OutputHolder = outputHolder;
			AssetDatabase.DeleteAsset(AVAConstantsVRC.OUTPUT_PATH + avatar.name + ".asset");
			AssetDatabase.CreateAsset(outputHolder, AVAConstantsVRC.OUTPUT_PATH + avatar.name + ".asset");

			foreach(var menu in Setup.AvatarMenus)
				Root.AvatarMenuControls.AddRange(menu.controls);
			foreach(var subMenu in Setup.AvatarSubMenus)
				Root.AvatarSubMenuControls.Add(new() { Target = subMenu.Target, MenuControls = new List<VRCExpressionsMenu.Control>(subMenu.Menu.controls) });
			if(Setup.UseFaceTracking && Setup.FaceTrackingSetupType == FT_Setup.Manual)
				foreach(var menu in Setup.AvatarMenusFaceTracking)
					Root.AvatarMenuControls.AddRange(menu.controls);
			foreach(var parameter in Setup.AvatarParameters)
				Root.Parameters.AddRange(parameter.parameters);
			if(Setup.UseFaceTracking && Setup.FaceTrackingSetupType == FT_Setup.Manual)
				foreach(var parameter in Setup.AvatarParametersFaceTracking)
					Root.Parameters.AddRange(parameter.parameters);

			foreach(var layer in Setup.LayerPreFT)
				if(layer.ProducerComponent != null)
					layer.ProducerComponent.Apply(Root.gameObject);
				else
					foreach(var c in layer.Controllers)
						if(AVAConstantsVRC.ControllerTypeToIndex.ContainsKey(c.Mapping) && c.Controller)
							Root.Layers[AVAConstantsVRC.ControllerTypeToIndex[c.Mapping]].Pre_FT.Add(c.Controller);

			// Run setup for all layers
			if(Setup.UseFaceTracking)
			{
				if(Setup.FaceTrackingSetupType == FT_Setup.Automatic)
				{
					if(!Setup.gameObject.TryGetComponent<AVAFaceTrackingProducerVRC>(out var autoFT))
						autoFT = Setup.gameObject.AddComponent<AVAFaceTrackingProducerVRC>();
					autoFT.Apply(Root.gameObject);
				}
				else
				{
					foreach(var layer in Setup.LayerFT)
						if(layer.ProducerComponent != null)
							layer.ProducerComponent.Apply(Root.gameObject);
						else
							foreach(var c in layer.Controllers)
								if(AVAConstantsVRC.ControllerTypeToIndex.ContainsKey(c.Mapping) && c.Controller)
									Root.Layers[AVAConstantsVRC.ControllerTypeToIndex[c.Mapping]].FT.Add(c.Controller);
				}
				foreach(var layer in Setup.LayerFTReact)
					if(layer.ProducerComponent != null)
						layer.ProducerComponent.Apply(Root.gameObject);
					else
						foreach(var c in layer.Controllers)
							if(AVAConstantsVRC.ControllerTypeToIndex.ContainsKey(c.Mapping) && c.Controller)
								Root.Layers[AVAConstantsVRC.ControllerTypeToIndex[c.Mapping]].FT_React.Add(c.Controller);
			}
			foreach(var layer in Setup.LayerManualExpressions)
				if(layer.ProducerComponent != null)
					layer.ProducerComponent.Apply(Root.gameObject);
				else
					foreach(var c in layer.Controllers)
						if(AVAConstantsVRC.ControllerTypeToIndex.ContainsKey(c.Mapping) && c.Controller)
							Root.Layers[AVAConstantsVRC.ControllerTypeToIndex[c.Mapping]].Mut.Add(c.Controller);

			foreach(var layer in Setup.LayerPost)
				if(layer.ProducerComponent != null)
					layer.ProducerComponent.Apply(Root.gameObject);
				else
					foreach(var c in layer.Controllers)
						if(AVAConstantsVRC.ControllerTypeToIndex.ContainsKey(c.Mapping) && c.Controller)
							Root.Layers[AVAConstantsVRC.ControllerTypeToIndex[c.Mapping]].Other.Add(c.Controller);

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
			if(SetupLayer(Setup, Root, 4, "FX") is var animatorFX && animatorFX != null)
			{
				animatorLayers[4].isDefault = false;
				animatorLayers[4].isEnabled = true;
				animatorLayers[4].animatorController = animatorFX;
				animatorFX.name = "AVA Base Setup FX";

				AssetDatabase.AddObjectToAsset(animatorFX, outputHolder);
			}
			if(SetupLayer(Setup, Root, 1, "Additive") is var animatorAdditive && animatorAdditive != null)
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
			AV3ManagerFunctions.AddParameters(avatar, Root.Parameters, null, true, true);
			AssetDatabase.AddObjectToAsset(avatar.expressionParameters, outputHolder);

			// Merge top level menus
			avatar.expressionsMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
			avatar.expressionsMenu.name = "AVA Base Setup Menu";
			var menuList = new List<VRCExpressionsMenu.Control>(Root.AvatarMenuControls);
			Root.AvatarMenuControlsLast.Reverse();
			menuList.AddRange(Root.AvatarMenuControlsLast);
			AVAVRCUtil.MergeMenuControls(menuList, avatar.expressionsMenu, true, Root.UnityResourcesToSave);

			// Merge submenus
			foreach(var subMenu in Root.AvatarSubMenuControls)
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
					AVAVRCUtil.MergeMenuControls(targetMenu.controls, newSubmenu, true, Root.UnityResourcesToSave);
					AVAVRCUtil.MergeMenuControls(subMenu.MenuControls, newSubmenu, true, Root.UnityResourcesToSave);
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
			foreach(var asset in Root.UnityResourcesToSave.ToHashSet())
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
				if(Layer == 4 && Setup.UseLayerWeightDrivers)
					SetupFTLayerWeightControl.Apply(Setup, State, ret, Layer);

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
	}
}

#endif
#endif
