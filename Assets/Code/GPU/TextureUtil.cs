using UnityEngine;

public class TextureUtil {
	private static ComputeShader shader = Resources.Load<ComputeShader>("util");

	public static RenderTexture Float4(int size) {
		RenderTexture texture = new RenderTexture(size, size, 0, RenderTextureFormat.ARGBFloat);
		texture.enableRandomWrite = true;
		texture.Create();
		return texture;
	}

	public static RenderTexture Float2(int size) {
		RenderTexture texture = new RenderTexture(size, size, 0, RenderTextureFormat.RGFloat);
		texture.enableRandomWrite = true;
		texture.Create();
		return texture;
	}

	public static RenderTexture Float(int size, float value = 0) {
		RenderTexture texture = new RenderTexture(size, size, 0, RenderTextureFormat.RFloat);
		texture.enableRandomWrite = true;
		texture.Create();

		if (value != 0){
			int kernel = shader.FindKernel("Init");
			shader.SetFloat("value", value);
			shader.SetTexture(kernel, "output", texture);
			shader.Dispatch(kernel, texture.width / 8, texture.height / 8, 1);
		}

		return texture;
	}

	public static RenderTexture Copy(RenderTexture input, RenderTexture output = null) {
		if (output == null){
			output = new RenderTexture(input.width, input.height, 0, RenderTextureFormat.RFloat);
			output.enableRandomWrite = true;
			output.Create();
		}

		int kernel = shader.FindKernel("Copy");
		shader.SetTexture(kernel, "input", input);
		shader.SetTexture(kernel, "output", output);

		shader.Dispatch(kernel, input.width / 8, input.height / 8, 1);

		return output;
	}
}
