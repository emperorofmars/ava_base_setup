#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using System.Linq;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public static class AvatarHandlerRegistryVRChat
	{
		private static readonly List<IAvatarBehaviourHandlerVRChat> DefaultHandlers = new() {
			new AnimationToggleHandlerVRC(),
			new AvatarAnimatorControllerHandlerVRC(),
		};
		private static readonly List<IAvatarBehaviourHandlerVRChat> RegisteredHandlers = new();

		public static void RegisterHandler(IAvatarBehaviourHandlerVRChat Handler)
		{
			if(!RegisteredHandlers.Contains(Handler)) RegisteredHandlers.Add(Handler);
		}

		public static List<IAvatarBehaviourHandlerVRChat> Handlers { get {
			var ret = new Dictionary<System.Type, IAvatarBehaviourHandlerVRChat>();
			var handlers = new List<IAvatarBehaviourHandlerVRChat> (DefaultHandlers);
			handlers.AddRange(RegisteredHandlers);
			foreach(var handler in handlers)
			{
				if(ret.ContainsKey(handler.HandlesBehaviour))
				{
					if(handler.Priority > ret[handler.HandlesBehaviour].Priority)
						ret[handler.HandlesBehaviour] = handler;
				}
				else
				{
					ret.Add(handler.HandlesBehaviour, handler);
				}
			}
			return ret.Values.OrderBy(e => e.Order).ToList();
		} }

	}
}

#endif
#endif
