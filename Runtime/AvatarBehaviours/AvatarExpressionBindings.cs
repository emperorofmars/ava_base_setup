
using System.Collections.Generic;
using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	//[AddComponentMenu("AVA/Generic/Expression Bindings")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	[DisallowMultipleComponent]
	public class AvatarExpressionBindings : IAvatarBehaviour
	{
		public enum EnumTriggerIntensity { None = 0, Left = 1, Right = 2 };
		public enum EnumHandDominance { Explicit, Left, Right };
		public enum EnumHandGesture { None, Fist, Open, Point, Peace, RockNRoll, Gun, ThumbsUp };

		[System.Serializable]
		public class AvatarExpressionBinding
		{
			public string Expression;
			public EnumHandGesture GuestureLeftHand = EnumHandGesture.None;
			public EnumHandGesture GuestureRightHand = EnumHandGesture.None;
			public EnumTriggerIntensity UseTriggerIntensity = EnumTriggerIntensity.None;
		}

		public EnumHandDominance HandDominance = EnumHandDominance.Right;
		public List<AvatarExpressionBinding> ExpressionBindings = new();
	}
}
