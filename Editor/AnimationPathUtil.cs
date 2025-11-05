#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	public static class AnimationPathUtil
	{
		public static int GetParentDepth(Transform t)
		{
			if(t.parent) return 1 + GetParentDepth(t.parent);
			else return 0;
		}

		public static string GetPath(Transform root, Transform transform, bool relative = false)
		{
			string path = "/" + transform.name;
			while (transform.parent != root && transform.parent != null)
			{
				transform = transform.parent;
				path = "/" + transform.name + path;
			}
			if(relative) path = path[1..];
			return path;
		}

		public static Transform GetRoot(Transform t)
		{
			if(t.parent) return GetRoot(t.parent);
			else return t;
		}

		public static AnimationClip RepathClip(AnimationClip Clip, string Retarget, string Match = "Body")
		{
			if(!Clip) return null;

			var ret = new AnimationClip
			{
				name = Clip.name,
				frameRate = Clip.frameRate,
				wrapMode = Clip.wrapMode
			};

			foreach (var binding in AnimationUtility.GetCurveBindings(Clip))
			{
				AnimationCurve curve = AnimationUtility.GetEditorCurve(Clip, binding);
				var newBinding = binding;
				if(!string.IsNullOrWhiteSpace(binding.path) && binding.path.StartsWith(Match))
				{
					newBinding = new EditorCurveBinding
					{
						path = binding.path.Replace(Match, Retarget),
						propertyName = binding.propertyName,
						type = binding.type
					};
				}
				AnimationUtility.SetEditorCurve(ret, newBinding, curve);
			}
			foreach(var binding in AnimationUtility.GetObjectReferenceCurveBindings(Clip))
			{
				ObjectReferenceKeyframe[] curve = AnimationUtility.GetObjectReferenceCurve(Clip, binding);
				// todo ?
				AnimationUtility.SetObjectReferenceCurve(ret, binding, curve);
			}
			return ret;
		}
	}
}

#endif