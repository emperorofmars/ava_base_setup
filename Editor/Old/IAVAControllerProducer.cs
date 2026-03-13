#if UNITY_EDITOR

using UnityEngine;

namespace com.squirrelbite.ava_base_setup.oooold
{
	public abstract class IAVAControllerProducer : MonoBehaviour
	{
		public abstract void Apply(GameObject Root);
	}
}

#endif
