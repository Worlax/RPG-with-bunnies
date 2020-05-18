using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(Stats))]
public class StatsEditor: Editor
{
	SerializedProperty Level;
	SerializedProperty Exp;
	SerializedProperty ExpForLevelUp;
	SerializedProperty CurrentHealth;

	void OnEnable()
	{
		Level = serializedObject.FindProperty("Level");
		Exp = serializedObject.FindProperty("Exp");
		ExpForLevelUp = serializedObject.FindProperty("ExpForLevelUp");
		CurrentHealth = serializedObject.FindProperty("CurrentHealth");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(Level);
		EditorGUILayout.PropertyField(Exp);
		EditorGUILayout.PropertyField(ExpForLevelUp);
		EditorGUILayout.PropertyField(CurrentHealth);
		serializedObject.ApplyModifiedProperties();

		DrawDefaultInspector();
	}
}
