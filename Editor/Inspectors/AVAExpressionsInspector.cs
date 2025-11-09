#if UNITY_EDITOR

using com.squirrelbite.ava_base_setup.util;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.squirrelbite.ava_base_setup
{
	[CustomPropertyDrawer(typeof(AvatarExpression))]
	public class AvatarExpressionDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			return new UnityPlzAvatarExpression(property);
		}
	}

	sealed class UnityPlzAvatarExpression : VisualElement
	{
		private readonly SerializedProperty _property;

		public UnityPlzAvatarExpression(SerializedProperty property)
		{
			_property = property;
			var ui = this;
			ui.style.flexDirection = FlexDirection.Row;
			ui.style.alignItems = Align.Stretch;
			var e = Toolkit.AddElement(ui, new PropertyField(property.FindPropertyRelative("Expression"), ""));
			e.style.minWidth = 120;
			e.style.flexGrow = 1;
			var a = Toolkit.AddElement(ui, new PropertyField(property.FindPropertyRelative("Animation"), ""));
			a.style.minWidth = 120;
			a.style.flexGrow = 1.5f;
		}
	}

	[CustomEditor(typeof(AVAExpressions))]
	public class AVAExpressionsInspector : Editor
	{
		static readonly Color SpacerColor = new(0.17f, 0.17f, 0.17f);

		private int FTMatch = -1;

		void OnEnable()
		{
		}

		public override VisualElement CreateInspectorGUI()
		{
			var c = (AVAExpressions)target;
			VisualElement ui = new();

			var list = Toolkit.AddList(ui, serializedObject.FindProperty("Expressions"), new Label("<size=+1><font-weight=700>Avatar Expressions</font-weight></size>"));

			return ui;
		}
	}
}

#endif
