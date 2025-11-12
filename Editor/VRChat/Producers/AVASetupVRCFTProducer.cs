#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Linq;
using com.squirrelbite.ava_base_setup.vrchat.VRLabs.AV3Manager;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public enum FT_Type { Automatic = -1, UnifiedExpressions = 0, SRanipal = 1, ARKit = 2 };

	[AddComponentMenu("AVA/VRChat/Face Tracking Producer")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(AVABaseSetupVRC))]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class AVASetupVRCFTProducer : IAVAControllerProducer, IEditorOnly
	{

		public SkinnedMeshRenderer FTMesh;
		public FT_Type FTType = FT_Type.Automatic;

		public override void Apply()
		{
#if AVA_BASE_SETUP_VRCFTTEMPLATES
			var setupState = gameObject.GetComponent<AVASetupStateVRC>();

			FTMesh = FTMesh != null ? FTMesh : FTTypeMatcher.DetectFaceMesh(AnimationPathUtil.GetRoot(transform).gameObject);
			if(FTMesh == null)
			{
				Debug.LogError("FT_Setup: Could not determine SkinnedMeshRenderer for face tracking!");
				return;
			}

			try {
				AnimatorController controllerFX;
				VRCExpressionsMenu menuFT;
				var ftKind = FTType == FT_Type.Automatic ? FTTypeMatcher.Match(FTMesh) : (int)FTType;
				if(ftKind == (int)FT_Type.UnifiedExpressions)
				{
					controllerFX = AssetDatabase.LoadAssetAtPath<AnimatorController>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "Unified Expressions Blendshapes/FX - Face Tracking - UE Blendshapes.controller");
					setupState.Parameters.AddRange(AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "Unified Expressions Blendshapes/Parameters - Face Tracking - UE Blendshapes.asset").parameters.ToList());
					menuFT = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "Unified Expressions Blendshapes/Face Tracking Control - UE Blendshapes.asset");
				}
				else if(ftKind == (int)FT_Type.SRanipal)
				{
					controllerFX = AssetDatabase.LoadAssetAtPath<AnimatorController>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "SRanipal Blendshapes/FX - Face Tracking - SRanipal Blendshapes.controller");
					setupState.Parameters.AddRange(AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "SRanipal Blendshapes/Parameters - Face Tracking - SRanipal Blendshapes.asset").parameters.ToList());
					menuFT = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "SRanipal Blendshapes/Face Tracking Control - SRanipal Blendshapes.asset");
				}
				else if(ftKind == (int)FT_Type.ARKit)
				{
					controllerFX = AssetDatabase.LoadAssetAtPath<AnimatorController>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "ARkit Blendshapes/FX - Face Tracking - ARKit Blendshapes.controller");
					setupState.Parameters.AddRange(AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "ARkit Blendshapes/Parameters - Face Tracking - ARkit Blendshapes.asset").parameters.ToList());
					menuFT = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "ARkit Blendshapes/Face Tracking Control  - ARKit Blendshapes.asset");
				}
				else
				{
					Debug.LogError("FT_Setup: Could not determine facial tracking type!");
					return;
				}

				var motionMatch = "Body";
				var motionRetarget = AnimationPathUtil.GetPath(transform, FTMesh.transform, true);

				var tmpMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
				var ftSubMenu = AVAVRCUtil.MergeMenuControls(menuFT.controls, ScriptableObject.CreateInstance<VRCExpressionsMenu>(), true, setupState.UnityResourcesToSave);
				ftSubMenu.name = menuFT.name;
				setupState.UnityResourcesToSave.Add(ftSubMenu);
				tmpMenu.controls.Add(new()
				{
					icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/adjerry91.vrcft.templates/Icons/FaceTrackingIcon2.png"),
					name = "Face Tracking",
					type = VRCExpressionsMenu.Control.ControlType.SubMenu,
					subMenu = ftSubMenu,
				});
				setupState.AvatarMenuControlsLast.AddRange(tmpMenu.controls);

				var AnimationRepather = new System.Func<AnimationClip, AnimationClip>(SourceClip => {
					var newClip = AnimationPathUtil.RepathClip(SourceClip, motionRetarget, motionMatch);
					setupState.UnityResourcesToSave.Add(newClip);
					return newClip;
				});

				setupState.Layers[4].FT.Add(AnimatorCloner.MergeControllers(new AnimatorController(), controllerFX, null, false, 1, AnimationRepather, setupState.UnityResourcesToSave));

				var controllerGesture = AssetDatabase.LoadAssetAtPath<AnimatorController>(AVAVRCUtil.VRCFT_TEMPLATES_BASE_PATH + "Eye Rotation/Additive - Eye Tracking - Eye Rotation.controller");
				setupState.Layers[1].FT.Add(AnimatorCloner.MergeControllers(new AnimatorController(), controllerGesture, null, false, 1, AnimationRepather, setupState.UnityResourcesToSave));
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
	}
}

#endif
#endif
