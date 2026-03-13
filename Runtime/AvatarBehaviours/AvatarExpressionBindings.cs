
using System.Collections.Generic;
using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	//[AddComponentMenu("AVA/Generic/Expression Bindings")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	[DisallowMultipleComponent]
	public class AvatarExpressionBindings : IAvatarBehaviour
	{
		public enum TriggerIntensity { None = 0, Left = 1, Right = 2 };
		public enum HandDominance { Explicit, Left, Right };
		public enum HandGesture { None, Fist, Open, Point, Peace, RockNRoll, Gun, ThumbsUp };

		[System.Serializable]
		public class AvatarExpressionBinding
		{
			public string Expression;
			public HandGesture GuestureLeftHand = HandGesture.None;
			public HandGesture GuestureRightHand = HandGesture.None;
			public TriggerIntensity UseTriggerIntensity = TriggerIntensity.None;
		}

		public HandDominance UseHandDominance = HandDominance.Right;
		public List<AvatarExpressionBinding> ExpressionBindings = new();
	}
}
