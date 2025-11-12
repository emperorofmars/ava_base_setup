#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public static class AVAVRCUtil
	{
		public const string VRCFT_TEMPLATES_BASE_PATH = "Packages/adjerry91.vrcft.templates/Animators/";

		public static VRCExpressionsMenu MergeMenuControls(List<VRCExpressionsMenu.Control> Source, VRCExpressionsMenu Target, bool deepClone = true, List<Object> AddCreatedObjects = null)
		{
			foreach(var control in Source)
			{
				var newControl = new VRCExpressionsMenu.Control() {
					icon = control.icon,
					labels = control.labels,
					name = control.name,
					parameter = control.parameter,
					style = control.style,
					subMenu = control.subMenu,
					subParameters = control.subParameters,
					type = control.type,
					value = control.value
				};
				if(deepClone && control.subMenu)
				{
					newControl.subMenu = MergeMenuControls(control.subMenu.controls, ScriptableObject.CreateInstance<VRCExpressionsMenu>(), true, AddCreatedObjects);
					newControl.subMenu.name = control.subMenu.name;
					AddCreatedObjects?.Add(newControl.subMenu);
				}
				Target.controls.Add(newControl);
			}
			return Target;
		}

		public static bool CheckVRCFTTemplates()
		{
			#if AVA_BASE_SETUP_VRCFTTEMPLATES
				if(AssetDatabase.LoadAssetAtPath<AnimatorController>(VRCFT_TEMPLATES_BASE_PATH + "Unified Expressions Blendshapes/FX - Face Tracking - UE Blendshapes.controller"))
					return true;
				else
					return false;
			#else
				return false;
			#endif
		}

		public static int MatchFTType(GameObject Root)
		{
			SkinnedMeshRenderer ftMesh = null;
			if(Root.TryGetComponent<AVASetupVRCFTProducer>(out var ftProducer))
				ftMesh = ftProducer.FTMesh;
			if(!ftMesh)
				ftMesh = FTTypeMatcher.DetectFaceMesh(AnimationPathUtil.GetRoot(Root.transform).gameObject);
			if(ftMesh)
				return FTTypeMatcher.Match(ftMesh);
			else
				return -1;
		}
	}
}

#endif
#endif
