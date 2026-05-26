#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public static class HandlerRegistryVRChat
	{
		private static readonly List<IAvatarBehaviourHandlerVRChat> DefaultHandlers = new() {
			new AvatarAnimatorControllerHandlerVRC(),
			new FaceTrackingHandlerVRC(),
			new PuppetHandlerVRC(),
			new AnimationToggleHandlerVRC(),
			new IdleHandlerVRC(),
			new AvatarExpressionBindingsHandlerVRC(),
			new AnimationGrabToggleHandlerVRC(),
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

		public static IAvatarBehaviourHandlerVRChat GetHandler(IAvatarBehaviour Behaviour)
		{
			if(Handlers.FindAll(h => h.HandlesBehaviour == Behaviour.GetType()) is List<IAvatarBehaviourHandlerVRChat> handlers && handlers.Count > 0)
			{
				handlers.Sort((a, b) => (int)a.Priority - (int)b.Priority);
				return handlers[0];
			}
			else
				return null;
		}

	}
}

#endif
#endif
