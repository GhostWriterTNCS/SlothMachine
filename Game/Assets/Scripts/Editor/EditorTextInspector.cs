using UnityEditor;

[CustomEditor(typeof(TextManager))]
public class EditorTextInspector : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		/*TextManager myScript = (TextManager)target;
		if (GUILayout.Button("Refresh")) {
			//myScript.RefreshText();
		}*/
	}
}