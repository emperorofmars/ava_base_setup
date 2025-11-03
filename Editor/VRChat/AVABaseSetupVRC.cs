#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;
using VRC.SDKBase.Editor.BuildPipeline;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public enum AVA_FT_Setup_Type { Automatic, Manual };
	public enum FacialTrackingMenu { AVA_Improved, VRCFT_Templates_Official, Manual };

	public class AVABaseSetupVRC : IAVABaseSetup, IEditorOnly
	{
		public AVA_FT_Setup_Type FacialTrackingSetupType = AVA_FT_Setup_Type.Automatic;
		public FacialTrackingMenu FacialTrackingMenu = FacialTrackingMenu.AVA_Improved;
		public VRCExpressionsMenu FacialTrackingMenuManual;
		public bool CreateEyePuppet = true;
		public bool OverwriteAlreadyMappedControllers = true;
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
				try
				{
					AVASetupVRCFTApplier.Apply(setup);
					return true;
				}
				catch (System.Exception exception)
				{
					Debug.LogError(exception);
					return false;
				}
				finally
				{
#if UNITY_EDITOR
					Object.DestroyImmediate(setup);
#else
					Object.Destroy(setup);
#endif
				}
			}
			return true; // Nothing to do
		}
	}
}

#endif
#endif
