#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[CustomEditor(typeof(AVAExpressionsProducer))]
	public class AVAExpressionsInspector : Editor
	{
		static readonly Color SpacerColor = new(0.17f, 0.17f, 0.17f);

		void OnEnable()
		{
		}

		public override void OnInspectorGUI()
		{
			var c = (AVAExpressionsProducer)target;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Bind Expressions to Hand-Gestures", EditorStyles.helpBox);

			GUILayout.Space(10f);

			EditorGUILayout.LabelField("Expressions", EditorStyles.boldLabel);
			GUILayout.Space(5f);
			using(new EditorGUI.IndentLevelScope())
				for (int i = 0; i < c.Emotes.Count; i++)
				{
					using(new EditorGUILayout.HorizontalScope(GUI.skin.box))
					{
						c.Emotes[i].Emote = EditorGUILayout.TextField(c.Emotes[i].Emote);
						EditorGUILayout.PropertyField(serializedObject.FindProperty("Emotes").GetArrayElementAtIndex(i).FindPropertyRelative("Animation"), new GUIContent());
						GUILayout.Space(5f);
						if (GUILayout.Button("X", GUILayout.ExpandWidth(false))) c.Emotes.RemoveAt(i);
					}

					if (i < c.Emotes.Count) GUILayout.Space(5f);
				}
			if (GUILayout.Button("+ Add Expression", GUILayout.MaxWidth(150)))
				c.Emotes.Add(new AvatarEmote());

			GUILayout.Space(5f);
			DrawHLine();
			GUILayout.Space(5f);

			EditorGUILayout.LabelField("Bindings", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("HandDominance"));

			GUILayout.Space(10f);

			using(new EditorGUI.IndentLevelScope())
			{
				if (c.Emotes.Count == 0)
				{
					EditorGUILayout.LabelField("No Expressions to bind!");
				}
				else
				{
					var bindingDropdown = c.Emotes.Select(e => e.Emote).Distinct().ToList();
					bindingDropdown.Sort();

					for (int i = 0; i < c.EmoteBindings.Count; i++)
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
										c.EmoteBindings[i].GuestureLeftHand = (HandGesture)EditorGUILayout.EnumPopup(c.EmoteBindings[i].GuestureLeftHand, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(180));
									}
									GUILayout.Space(5f);
									using(new EditorGUILayout.VerticalScope())
									{
										GUILayout.Label("Right", GUILayout.ExpandWidth(false));
										c.EmoteBindings[i].GuestureRightHand = (HandGesture)EditorGUILayout.EnumPopup(c.EmoteBindings[i].GuestureRightHand, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(180));
									}
									GUILayout.Space(5f);
									using(new EditorGUILayout.VerticalScope())
									{
										GUILayout.Label("Use Trigger Intensity", GUILayout.ExpandWidth(false));
										if (c.EmoteBindings[i].GuestureLeftHand > 0 && c.EmoteBindings[i].GuestureRightHand > 0)
										{
											c.EmoteBindings[i].UseTriggerIntensity = (TriggerIntensity)EditorGUILayout.EnumPopup(c.EmoteBindings[i].UseTriggerIntensity, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(180));
										}
										else
										{
											bool useIntensity = c.EmoteBindings[i].UseTriggerIntensity > 0;
											bool useIntensityNew = EditorGUILayout.Toggle(useIntensity, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(180));
											if (useIntensity != useIntensityNew)
											{
												if (useIntensityNew)
													c.EmoteBindings[i].UseTriggerIntensity = c.EmoteBindings[i].GuestureLeftHand > 0 ? TriggerIntensity.Left : TriggerIntensity.Right;
												else
													c.EmoteBindings[i].UseTriggerIntensity = TriggerIntensity.None;
											}
										}
									}
									GUILayout.Space(5f);
									using(new EditorGUILayout.VerticalScope())
									{
										GUILayout.Label("Expression");
										var oldSelectedIndex = bindingDropdown.FindIndex(b => b == c.EmoteBindings[i].Emote);
										if(oldSelectedIndex < 0)
										{
											oldSelectedIndex = 0;
											c.EmoteBindings[i].Emote = bindingDropdown[oldSelectedIndex];
											EditorUtility.SetDirty(c);
										}
										var newSelectedIndex = EditorGUILayout.Popup(oldSelectedIndex, bindingDropdown.Select(e => e.Length > 0 ? char.ToUpper(e[0]) + e[1..] : e).ToArray(), GUILayout.ExpandWidth(false), GUILayout.MaxWidth(180));
										if (newSelectedIndex != oldSelectedIndex)
										{
											c.EmoteBindings[i].Emote = bindingDropdown[newSelectedIndex];
											EditorUtility.SetDirty(c);
										}
									}
								}

								if (string.IsNullOrWhiteSpace(c.EmoteBindings[i].Emote) || c.EmoteBindings[i].GuestureLeftHand == HandGesture.None && c.EmoteBindings[i].GuestureRightHand == HandGesture.None)
								{
									EditorGUILayout.HelpBox("Empty Binding! Please select emote and mappings.", MessageType.Error);
									//EditorGUILayout.LabelField("Empty Binding! Please select emote and mappings.");
								}
								else
								{
									for (int nestedI = 0; nestedI < c.EmoteBindings.Count; nestedI++)
									{
										if (nestedI != i && c.EmoteBindings[nestedI].GuestureLeftHand == c.EmoteBindings[i].GuestureLeftHand && c.EmoteBindings[nestedI].GuestureRightHand == c.EmoteBindings[i].GuestureRightHand)
										{
											EditorGUILayout.HelpBox("Duplicate Binding! Please select a different mapping.", MessageType.Error);
											//EditorGUILayout.LabelField("Duplicate Binding! Please select a different mapping!");
											break;
										}
									}
								}
							}
							GUILayout.Space(10f);
							if (GUILayout.Button("X", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true))) c.EmoteBindings.RemoveAt(i);
						}
						if (i < c.EmoteBindings.Count) GUILayout.Space(10f);
					}
					if (GUILayout.Button("+ Add Binding", GUILayout.MaxWidth(150)))
					{
						c.EmoteBindings.Add(new AvatarEmoteBinding());
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
		}
	}
}

#endif
#endif
