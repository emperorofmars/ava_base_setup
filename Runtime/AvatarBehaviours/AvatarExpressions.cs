
using System.Collections.Generic;
using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	//[AddComponentMenu("AVA/Generic/Expressions")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class AvatarExpressions : IAvatarBehaviour
	{
		[System.Serializable]
		public class Expression
		{
			public string Mapping = "";
			public AnimationClip Animation = null;
		}

		public List<Expression> Expressions = new();
	}
}
