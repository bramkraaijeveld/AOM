using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProceduralTerrain))]
public class ProceduralTerrainEditor : Editor {
	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		if (GUILayout.Button("Update Map"))
			((ProceduralTerrain)target).InitMap();
		if (GUILayout.Button("Update Noise"))
			((ProceduralTerrain)target).InitNoise();
		if (GUILayout.Button("Update Erosion"))
			((ProceduralTerrain)target).InitErosion();
	}
}
