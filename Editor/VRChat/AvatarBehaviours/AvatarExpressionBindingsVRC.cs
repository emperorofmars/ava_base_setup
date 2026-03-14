#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using UnityEngine;
using VRC.SDKBase;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[AddComponentMenu("AVA/VRChat/Behaviours/Expression Bindings")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(AvatarExpressionsVRC))]
	public class AvatarExpressionBindingsVRC : AvatarExpressionBindings, IEditorOnly
	{
		public void InitBindings()
		{
			var expressions = GetComponent<AvatarExpressionsVRC>();
			foreach(var expression in expressions.Expressions)
			{
				// Set default bindings
				// TODO vastly expand this logic
				switch (expression.Mapping)
				{
					case "smile": ExpressionBindings.Add(new AvatarExpressionBinding() { Expression = expression.Mapping, GuestureLeftHand = HandGesture.Fist, UseTriggerIntensity = TriggerIntensity.Left }); break;
					case "blep": ExpressionBindings.Add(new AvatarExpressionBinding() { Expression = expression.Mapping, GuestureRightHand = HandGesture.Fist, UseTriggerIntensity = TriggerIntensity.Right }); break;
					case "sad": ExpressionBindings.Add(new AvatarExpressionBinding() { Expression = expression.Mapping, GuestureLeftHand = HandGesture.Gun }); break;
					case "angry": ExpressionBindings.Add(new AvatarExpressionBinding() { Expression = expression.Mapping, GuestureLeftHand = HandGesture.RockNRoll }); break;
					default: ExpressionBindings.Add(new AvatarExpressionBinding() { Expression = expression.Mapping }); break;
				}
			}
		}
	}
}

#endif
#endif
