#if UNITY_EDITOR

using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	public abstract class IAVAControllerProducer : MonoBehaviour
	{
		public abstract void Apply();
	}
}

#endif
