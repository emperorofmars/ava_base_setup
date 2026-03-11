
using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	//[AddComponentMenu("AVA/Generic/Idle")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class Idle : IAvatarBehaviour
	{
		public AnimationClip IdleAnimation = null;
		public bool IsAdditive = true;
	}
}
