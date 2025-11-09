#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;

namespace com.squirrelbite.ava_base_setup.vrchat
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

	/// <summary>
	/// Opinionated base setup for VR & V-Tubing avatar expressions.
	/// </summary>
	[AddComponentMenu("AVA/VRChat/Expression Bindings Producer")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(AVABaseSetupVRC))]
	[RequireComponent(typeof(AVAExpressionsVRC))]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class AVAExpressionBindingsVRCProducer : IAVAControllerProducer, IEditorOnly
	{
		public HandDominance HandDominance = HandDominance.Right;
		public List<AvatarExpressionBinding> ExpressionBindings = new();

		public bool CreateEyeJoystickPuppet = true;

		// TODO Toggles, JoystickPuppets, Other stuff, here or in a set of other components

		public void InitBindings()
		{
			var expressions = GetComponent<AVAExpressions>();
			foreach(var expression in expressions.Expressions)
			{
				// Set default bindings
				// TODO vastly expand this logic
				switch (expression.Expression)
				{
					case "smile": ExpressionBindings.Add(new AvatarExpressionBinding() { Expression = expression.Expression, GuestureLeftHand = HandGesture.Fist, UseTriggerIntensity = TriggerIntensity.Left }); break;
					case "blep": ExpressionBindings.Add(new AvatarExpressionBinding() { Expression = expression.Expression, GuestureRightHand = HandGesture.Fist, UseTriggerIntensity = TriggerIntensity.Right }); break;
					case "sad": ExpressionBindings.Add(new AvatarExpressionBinding() { Expression = expression.Expression, GuestureLeftHand = HandGesture.Gun }); break;
					case "angry": ExpressionBindings.Add(new AvatarExpressionBinding() { Expression = expression.Expression, GuestureLeftHand = HandGesture.RockNRoll }); break;
					default: ExpressionBindings.Add(new AvatarExpressionBinding() { Expression = expression.Expression }); break;
				}
			}
		}

		public override void Apply()
		{
			AVAExpressionBindingsVRCApplier.Apply(this);
		}
	}
}

#endif
#endif
