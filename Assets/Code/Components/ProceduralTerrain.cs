using UnityEngine;

public class ProceduralTerrain : MonoBehaviour {
	private RenderTexture heightmap;
	private GameObject root;

	[HeaderAttribute("Random")]
	public int seed;

	[HeaderAttribute("Material")]
	public Material material;
	public float height;

	[HeaderAttribute("Map")]
	public int size;
	public int chunkSize;

	[HeaderAttribute("Noise")]
	public int octaves;
	public float scale;
	public float persistence;

	[HeaderAttribute("Erosion")]
	public int iterations;
	public int cycles;
	public float dt;
	public float g;
	public float pipeArea;
	public float pipeLength;
	public float amount;
	public float talus;

	// Live Controls go Here
	public void OnValidate() {
		material.SetFloat("height", height);
	}

	public void InitMap() {
		// Create RenderTexture
		if (heightmap != null) heightmap.Release();
		heightmap = new RenderTexture(size, size, 0, RenderTextureFormat.RFloat);
		heightmap.enableRandomWrite = true;
		heightmap.Create();

		// Update Material
		material.SetTexture("terrain", heightmap);
		material.SetFloat("height", height);
		material.SetInt("size", size);
		
		// Create Mesh
		if (root != null) DestroyImmediate(root);
		root = new SquareGrid(size, chunkSize, material).GameObject();

		InitNoise();
	}

	public void InitNoise() {
		Noise.Perlin(heightmap, octaves, scale, persistence);
	}

	public void InitErosion() {
		StartCoroutine(new Erosion(dt, g, pipeArea, pipeLength).Thermal(heightmap, iterations, cycles, amount, talus));
	}
	
	public void Start(){
		InitMap();
	}

	public void OnDestroy(){
		heightmap.Release();
	}
}
