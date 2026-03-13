#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using com.squirrelbite.ava_base_setup.oooold;
using UnityEngine;
using VRC.SDKBase;

namespace com.squirrelbite.ava_base_setup.vrchat.oooold
{
	[AddComponentMenu("AVA/VRChat/Control Setup Producer")]
	[RequireComponent(typeof(AVABaseSetupVRC))]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class ControlSetupProducerVRC : IAVAControllerProducer, IEditorOnly
	{
		[System.Serializable]
		public class Toggle
		{
			public string Name;
			public string Parameter;
			public bool Default = false;
			public Texture2D Icon;
			public AnimationClip On;
			public AnimationClip Off;
		}

		public List<Toggle> Toggles = new();

		public override void Apply(GameObject Root)
		{

		}
	}
}

#endif
#endif
