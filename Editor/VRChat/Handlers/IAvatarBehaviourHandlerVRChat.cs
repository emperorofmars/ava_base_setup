#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using UnityEngine.UIElements;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public abstract class IAvatarBehaviourHandlerVRChat
	{
		public abstract System.Type HandlesBehaviour { get; }
		public abstract uint Priority { get; }
		public abstract uint Order { get; }
		public abstract string Label { get; }

		public abstract void Handle(AvatarHandlerContextVRChat Context, IAvatarBehaviour Behaviour);

		public abstract List<(string Parameter, VRCExpressionParameters.ValueType ValueType)> GetParameters(IAvatarBehaviour Behaviour);

		public virtual VisualElement CreateGUI(IAvatarBehaviour Behaviour) { return null; }
	}
}

#endif
#endif
