using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor: Editor
{
	SerializedProperty grid;
	SerializedProperty windowsRoot;
	SerializedProperty inventoryPrefab;
	SerializedProperty equipmentPrefab;

	void OnEnable()
	{
		grid = serializedObject.FindProperty("grid");
		windowsRoot = serializedObject.FindProperty("windowsRoot");
		inventoryPrefab = serializedObject.FindProperty("inventoryPrefab");
		equipmentPrefab = serializedObject.FindProperty("equipmentPrefab");
	}

	bool showAssignment = true;

	public override void OnInspectorGUI()
	{
		GUIStyle foldoutStyle = EditorStyles.foldout;
		foldoutStyle.fontStyle = FontStyle.Bold;

		if (showAssignment = EditorGUILayout.Foldout(showAssignment, "Assignment", true, foldoutStyle))
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(grid);
			EditorGUILayout.PropertyField(windowsRoot);
			EditorGUILayout.PropertyField(inventoryPrefab);
			EditorGUILayout.PropertyField(equipmentPrefab);
			serializedObject.ApplyModifiedProperties();
		}

		DrawDefaultInspector();
	}
}
