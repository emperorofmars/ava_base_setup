#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using com.squirrelbite.ava_base_setup.util;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[CustomEditor(typeof(AVAExpressionBindingsVRCProducer))]
	public class AVAExpressionBindingsVRCInspector : Editor
	{
		static readonly Color SpacerColor = new(0.17f, 0.17f, 0.17f);

		void OnEnable()
		{
		}

		public override VisualElement CreateInspectorGUI()
		{
			var c = (AVAExpressionBindingsVRCProducer)target;
			VisualElement ui = new();

			var l = Toolkit.AddElement(ui, new Label("<size=+1><font-weight=700>Bind Avatar Expressions to Hand-Gestures</font-weight></size>"));
			l.style.marginTop = l.style.marginBottom = 5;

			var handDominance = Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("HandDominance")));

			var v = Toolkit.AddList(ui, serializedObject.FindProperty("ExpressionBindings"));

			return ui;
		}

		/*public override void OnInspectorGUI()
		{
			var c = (AVAExpressionBindingsVRCProducer)target;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Bind Expressions to Hand-Gestures", EditorStyles.helpBox);

			GUILayout.Space(10f);

			EditorGUILayout.LabelField("Bindings", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("HandDominance"));

			GUILayout.Space(10f);

			var expressions = c.GetComponent<AVAExpressions>();

			using(new EditorGUI.IndentLevelScope())
			{
				if (expressions.Expressions.Count == 0)
				{
					EditorGUILayout.LabelField("No Expressions to bind!");
				}
				else
				{
					var bindingDropdown = expressions.Expressions.Select(e => e.Expression).Distinct().ToList();
					bindingDropdown.Sort();

					for (int i = 0; i < c.ExpressionBindings.Count; i++)
					{
						using(new EditorGUILayout.HorizontalScope(GUI.skin.box))
						{
							GUILayout.Space(10f);
							using(new EditorGUILayout.VerticalScope())
							{
								using(new EditorGUILayout.HorizontalScope())
								{
									using(new EditorGUILayout.VerticalScope())
									{
										GUILayout.Label("Left", GUILayout.ExpandWidth(false));
										c.ExpressionBindings[i].GuestureLeftHand = (HandGesture)EditorGUILayout.EnumPopup(c.ExpressionBindings[i].GuestureLeftHand, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(180));
									}
									GUILayout.Space(5f);
									using(new EditorGUILayout.VerticalScope())
									{
										GUILayout.Label("Right", GUILayout.ExpandWidth(false));
										c.ExpressionBindings[i].GuestureRightHand = (HandGesture)EditorGUILayout.EnumPopup(c.ExpressionBindings[i].GuestureRightHand, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(180));
									}
									GUILayout.Space(5f);
									using(new EditorGUILayout.VerticalScope())
									{
										GUILayout.Label("Use Trigger Intensity", GUILayout.ExpandWidth(false));
										if (c.ExpressionBindings[i].GuestureLeftHand > 0 && c.ExpressionBindings[i].GuestureRightHand > 0)
										{
											c.ExpressionBindings[i].UseTriggerIntensity = (TriggerIntensity)EditorGUILayout.EnumPopup(c.ExpressionBindings[i].UseTriggerIntensity, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(180));
										}
										else
										{
											bool useIntensity = c.ExpressionBindings[i].UseTriggerIntensity > 0;
											bool useIntensityNew = EditorGUILayout.Toggle(useIntensity, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(180));
											if (useIntensity != useIntensityNew)
											{
												if (useIntensityNew)
													c.ExpressionBindings[i].UseTriggerIntensity = c.ExpressionBindings[i].GuestureLeftHand > 0 ? TriggerIntensity.Left : TriggerIntensity.Right;
												else
													c.ExpressionBindings[i].UseTriggerIntensity = TriggerIntensity.None;
											}
										}
									}
									GUILayout.Space(5f);
									using(new EditorGUILayout.VerticalScope())
									{
										GUILayout.Label("Expression");
										var oldSelectedIndex = bindingDropdown.FindIndex(b => b == c.ExpressionBindings[i].Expression);
										if(oldSelectedIndex < 0)
										{
											oldSelectedIndex = 0;
											c.ExpressionBindings[i].Expression = bindingDropdown[oldSelectedIndex];
											EditorUtility.SetDirty(c);
										}
										var newSelectedIndex = EditorGUILayout.Popup(oldSelectedIndex, bindingDropdown.Select(e => e.Length > 0 ? char.ToUpper(e[0]) + e[1..] : e).ToArray(), GUILayout.ExpandWidth(false), GUILayout.MaxWidth(180));
										if (newSelectedIndex != oldSelectedIndex)
										{
											c.ExpressionBindings[i].Expression = bindingDropdown[newSelectedIndex];
											EditorUtility.SetDirty(c);
										}
									}
								}

								if (string.IsNullOrWhiteSpace(c.ExpressionBindings[i].Expression) || c.ExpressionBindings[i].GuestureLeftHand == HandGesture.None && c.ExpressionBindings[i].GuestureRightHand == HandGesture.None)
								{
									EditorGUILayout.HelpBox("Empty Binding! Please select emote and mappings.", MessageType.Error);
									//EditorGUILayout.LabelField("Empty Binding! Please select emote and mappings.");
								}
								else
								{
									for (int nestedI = 0; nestedI < c.ExpressionBindings.Count; nestedI++)
									{
										if (nestedI != i && c.ExpressionBindings[nestedI].GuestureLeftHand == c.ExpressionBindings[i].GuestureLeftHand && c.ExpressionBindings[nestedI].GuestureRightHand == c.ExpressionBindings[i].GuestureRightHand)
										{
											EditorGUILayout.HelpBox("Duplicate Binding! Please select a different mapping.", MessageType.Error);
											//EditorGUILayout.LabelField("Duplicate Binding! Please select a different mapping!");
											break;
										}
									}
								}
							}
							GUILayout.Space(10f);
							if (GUILayout.Button("X", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true))) c.ExpressionBindings.RemoveAt(i);
						}
						if (i < c.ExpressionBindings.Count) GUILayout.Space(10f);
					}
					if (GUILayout.Button("+ Add Binding", GUILayout.MaxWidth(150)))
					{
						c.ExpressionBindings.Add(new AvatarExpressionBinding());
					}
				}
			}

			GUILayout.Space(10f);

			if(EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(c);
			}
		}

		private void DrawHLine()
		{
			GUILayout.Space(5f);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), SpacerColor);
			GUILayout.Space(5f);
		}*/
	}
}

#endif
#endif
