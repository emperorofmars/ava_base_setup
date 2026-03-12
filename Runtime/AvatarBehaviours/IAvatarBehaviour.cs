
using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	public abstract class IAvatarBehaviour : MonoBehaviour
	{
		public string Name;
		public virtual bool AllowMultiple => true;
	}
}
