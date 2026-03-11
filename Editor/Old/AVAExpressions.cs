#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	[System.Serializable]
	public class AvatarExpressionOld
	{
		public string Expression = "";
		public AnimationClip Animation = null;
	}

	[DisallowMultipleComponent]
	//[AddComponentMenu("AVA/Generic/Expressions")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class AVAExpressions : MonoBehaviour
	{
		public List<AvatarExpressionOld> Expressions = new();
	}
}

#endif
