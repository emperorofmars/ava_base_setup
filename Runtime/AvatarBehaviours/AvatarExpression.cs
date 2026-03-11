
using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	//[AddComponentMenu("AVA/Generic/Expression")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class AvatarExpression : IAvatarBehaviour
	{
		public string Expression = "";
		public AnimationClip Animation = null;
	}
}
