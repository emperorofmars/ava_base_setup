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
	public class AvatarEmote
	{
		public string Emote;
		public AnimationClip Animation;
	}

	[System.Serializable]
	public class AvatarEmoteBinding
	{
		public string Emote;
		public HandGesture GuestureLeftHand = HandGesture.None;
		public HandGesture GuestureRightHand = HandGesture.None;
		public TriggerIntensity UseTriggerIntensity = TriggerIntensity.None;
	}

	/// <summary>
	/// Opinionated base setup for VR & V-Tubing avatar expressions.
	/// </summary>
	[AddComponentMenu("AVA/VRChat/Expressions Producer")]
	public class AVAExpressionsProducer : IAVAControllerProducer, IEditorOnly
	{
		public HandDominance HandDominance = HandDominance.Right;
		public List<AvatarEmote> Emotes = new();
		public List<AvatarEmoteBinding> EmoteBindings = new();

		public bool CreateEyeJoystickPuppet = true;

		// TODO Toggles, JoystickPuppets, Other stuff, here or in a set of other components

		public void AddAvatarEmote(AvatarEmote Emote)
		{
			Emotes.Add(Emote);

			// Set default bindings
			// TODO vastly expand this logic
			switch (Emote.Emote)
			{
				case "smile": EmoteBindings.Add(new AvatarEmoteBinding() { Emote = Emote.Emote, GuestureLeftHand = HandGesture.Fist, UseTriggerIntensity = TriggerIntensity.Left }); break;
				case "blep": EmoteBindings.Add(new AvatarEmoteBinding() { Emote = Emote.Emote, GuestureRightHand = HandGesture.Fist, UseTriggerIntensity = TriggerIntensity.Right }); break;
				case "sad": EmoteBindings.Add(new AvatarEmoteBinding() { Emote = Emote.Emote, GuestureLeftHand = HandGesture.Gun }); break;
				case "angry": EmoteBindings.Add(new AvatarEmoteBinding() { Emote = Emote.Emote, GuestureLeftHand = HandGesture.RockNRoll }); break;
				default: EmoteBindings.Add(new AvatarEmoteBinding() { Emote = Emote.Emote }); break;
			}
		}

		public override void Apply()
		{
			AVAExpressionsApplierVRC.Apply(this);
		}
	}
}

#endif
#endif
