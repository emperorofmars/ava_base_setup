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

	[AddComponentMenu("AVA/VRChat/Facial Tracking Controller")]
	public class AVASetupVRCFTController : IAVAControllerProducer, IEditorOnly
	{
		public const string VRCFT_TEMPLATES_BASE_PATH = "Packages/adjerry91.vrcft.templates/Animators/";

		public SkinnedMeshRenderer FTMesh;
		public FT_Type Type = FT_Type.Automatic;

		public override void Apply()
		{
#if AVA_BASE_SETUP_VRCFTTEMPLATES
			var setupState = gameObject.GetComponent<AVASetupStateVRC>();

			if(FTMesh == null)
			{
				SkinnedMeshRenderer match = null;
				foreach(var candidate in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
				{
					if(candidate.name.ToLower() == "body" && match == null || AnimationPathUtil.GetParentDepth(match.transform) > AnimationPathUtil.GetParentDepth(candidate.transform))
						match = candidate;
				}
				if(match) FTMesh = match;
			}
			if(FTMesh == null)
			{
				Debug.LogError("FT_Setup: Could not determine SkinnedMeshRenderer for face tracking!");
				return;
			}

			try {
				AnimatorController controllerFX;
				var ftKind = Type == FT_Type.Automatic ? FTTypeMatcher.Match(FTMesh) : (int)Type;
				if(ftKind == (int)FT_Type.UnifiedExpressions)
				{
					controllerFX = AssetDatabase.LoadAssetAtPath<AnimatorController>(VRCFT_TEMPLATES_BASE_PATH + "Unified Expressions Blendshapes/FX - Face Tracking - UE Blendshapes.controller");
					setupState.Parameters.AddRange(AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(VRCFT_TEMPLATES_BASE_PATH + "Unified Expressions Blendshapes/Parameters - Face Tracking - UE Blendshapes.asset").parameters.ToList());
					setupState.FTMenu = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(VRCFT_TEMPLATES_BASE_PATH + "Unified Expressions Blendshapes/Face Tracking Control - UE Blendshapes.asset");
				}
				else if(ftKind == (int)FT_Type.SRanipal)
				{
					controllerFX = AssetDatabase.LoadAssetAtPath<AnimatorController>(VRCFT_TEMPLATES_BASE_PATH + "SRanipal Blendshapes/FX - Face Tracking - SRanipal Blendshapes.controller");
					setupState.Parameters.AddRange(AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(VRCFT_TEMPLATES_BASE_PATH + "SRanipal Blendshapes/Parameters - Face Tracking - SRanipal Blendshapes.asset").parameters.ToList());
					setupState.FTMenu = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(VRCFT_TEMPLATES_BASE_PATH + "SRanipal Blendshapes/Face Tracking Control - SRanipal Blendshapes.asset");
				}
				else if(ftKind == (int)FT_Type.ARKit)
				{
					controllerFX = AssetDatabase.LoadAssetAtPath<AnimatorController>(VRCFT_TEMPLATES_BASE_PATH + "ARkit Blendshapes/FX - Face Tracking - ARKit Blendshapes.controller");
					setupState.Parameters.AddRange(AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(VRCFT_TEMPLATES_BASE_PATH + "ARkit Blendshapes/Parameters - Face Tracking - ARkit Blendshapes.asset").parameters.ToList());
					setupState.FTMenu = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(VRCFT_TEMPLATES_BASE_PATH + "ARkit Blendshapes/Face Tracking Control  - ARKit Blendshapes.asset");
				}
				else
				{
					Debug.LogError("FT_Setup: Could not determine facial tracking type!");
					return;
				}
				setupState.Layers[4].FT.Add(AnimatorCloner.MergeControllers(new AnimatorController(), controllerFX, null, false, 1, AnimationPathUtil.GetPath(transform, FTMesh.transform, true), "Body"));

				var controllerGesture = AssetDatabase.LoadAssetAtPath<AnimatorController>(VRCFT_TEMPLATES_BASE_PATH + "Eye Rotation/Additive - Eye Tracking - Eye Rotation.controller");
				setupState.Layers[1].FT.Add(AnimatorCloner.MergeControllers(new AnimatorController(), controllerGesture, null, false, 1, AnimationPathUtil.GetPath(transform, FTMesh.transform, true), "Body"));
			}
			catch(System.Exception e)
			{
				Debug.LogError("FT_Setup: an error occurred during setup:" + e.Message);
				Debug.LogException(e);
			}
#else
			Debug.LogError("FT_Setup: face tracking templates are not installed!");
#endif
		}
	}
}

#endif
#endif
