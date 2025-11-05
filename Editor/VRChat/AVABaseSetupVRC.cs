#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;
using VRC.SDKBase.Editor.BuildPipeline;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public enum AVA_FT_Setup_Type { Automatic, Manual };

	[AddComponentMenu("AVA/VRChat/AVA Base Setup")]
	public class AVABaseSetupVRC : IAVABaseSetup, IEditorOnly
	{
		public AVA_FT_Setup_Type FaceTrackingSetupType = AVA_FT_Setup_Type.Automatic;
		public List<VRCExpressionsMenu> AvatarMenus = new();
		public List<VRCExpressionsMenu> AvatarMenusFaceTracking = new();
		public List<VRCExpressionParameters> AvatarParameters = new();
		public List<VRCExpressionParameters> AvatarParametersFaceTracking = new();
	}

	[InitializeOnLoad]
	public class AVABaseSetupVRCCallback : IVRCSDKPreprocessAvatarCallback
	{
		// Run before VRCFury (-10000) and ModularAvatar (-11000)
		public int callbackOrder => -20000;

		public bool OnPreprocessAvatar(GameObject Root)
		{
			if (Root.GetComponent<AVABaseSetupVRC>() is var setup && setup != null)
			{
				AVASetupStateVRC setupState = null;
				try
				{
					setupState = setup.gameObject.AddComponent<AVASetupStateVRC>();
					AVASetupVRCApplier.Apply(setup, setupState);
					return true;
				}
				catch (System.Exception exception)
				{
					Debug.LogError(exception);
					return false;
				}
				finally
				{
					foreach(var removeMe in setup.GetComponentsInChildren<IAVAControllerProducer>())
						Object.DestroyImmediate(removeMe);
					Object.DestroyImmediate(setup);
					if(setupState)
						Object.DestroyImmediate(setupState);
				}
			}
			return true; // Nothing to do
		}
	}
}

#endif
#endif
