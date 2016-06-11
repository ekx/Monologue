using UnityEngine;
using UnityEditor;

namespace Monologue
{
	[CustomEditor(typeof(Monologue))]
	public class MonologueEditor : Editor
	{
		public override void OnInspectorGUI()
		{			
			serializedObject.Update();
            var monologue = target as Monologue;

			EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);

            monologue.SecondsPerChar = EditorGUILayout.FloatField("Seconds Per Char", monologue.SecondsPerChar);
			monologue.BeepTrigger = (BeepTrigger) EditorGUILayout.EnumPopup("Beep Trigger", monologue.BeepTrigger);			
			monologue.Volume = EditorGUILayout.Slider("Volume", monologue.Volume, 0f, 1f);
			monologue.Pitch = EditorGUILayout.Slider("Pitch", monologue.Pitch, -3f, 3f);

            EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Beep Settings", EditorStyles.boldLabel);

            monologue.BeepType = (BeepType)EditorGUILayout.EnumPopup("Beep Type", monologue.BeepType);

			if (monologue.BeepType == BeepType.AudioSample)
			{
				SerializedProperty Sample = serializedObject.FindProperty("Sample");
				EditorGUILayout.PropertyField(Sample);
			}
			else
			{
				monologue.WaveType = (WaveType)EditorGUILayout.EnumPopup("Wave Type", monologue.WaveType);
                monologue.BeepLengthSeconds = EditorGUILayout.FloatField("Beep Length Seconds", monologue.BeepLengthSeconds);
                monologue.BaseFrequency = EditorGUILayout.IntSlider("Base Frequency", monologue.BaseFrequency, 20, 20000);
				monologue.BaseVolume = EditorGUILayout.Slider("Base Volume", monologue.BaseVolume, 0f, 1f);
            }

            EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);

			SerializedProperty OnTextOutputFinished = serializedObject.FindProperty("OnTextOutputFinished");
			EditorGUILayout.PropertyField(OnTextOutputFinished);

            EditorGUILayout.Separator();
            
			var audioSource = monologue.gameObject.GetComponent<AudioSource>();
            if (!audioSource.playOnAwake && monologue.BeepType == BeepType.Generated)
			{
				EditorGUILayout.HelpBox("The AudioSource component must be set to \"Play On Awake\" for generated sound to work.", MessageType.Error);
			}

            EditorGUILayout.HelpBox("Other MeshEffects (e.g. Shadow, Outline) must be added after the Monologue script for it to work properly.", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
	}
}