
using System.Collections.Generic;
using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	//[AddComponentMenu("AVA/Generic/Puppet")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class Puppet : IAvatarBehaviour
	{
		public enum PuppetType { D1 = 1, D2 = 2 }

		[System.Serializable]
		public class AnimationMapping
		{
			public Vector2 Position;
			public AnimationClip Animation;
		}

		public PuppetType Type;
		public bool IsPersistent = false;
		public bool IsOverridable = false;
		public Texture2D Icon;
		public List<AnimationMapping> Blendtree = new();
	}
}
