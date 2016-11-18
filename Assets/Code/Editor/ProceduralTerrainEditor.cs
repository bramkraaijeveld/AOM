using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProceduralTerrain))]
public class ProceduralTerrainEditor : Editor {
	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		ProceduralTerrain pt = (ProceduralTerrain)target;
		
		if (GUILayout.Button("Update Map")) pt.InitMap();
		if (GUILayout.Button("Update Noise")) pt.InitNoise();
		if (GUILayout.Button(pt.erosion_running ? "Stop Erosion" : "Start Erosion")) pt.ToggleErosion();
	}
}
