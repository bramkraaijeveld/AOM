using UnityEngine;
using System.Collections;

public class Erosion {

    public static ComputeShader shader = Resources.Load<ComputeShader>("Erosion");

    public Erosion(float dt = 0.001f, float gravity = 9.81f, float area = 1, float length = 1){
        shader.SetFloat("dt", dt);
        shader.SetFloat("g", gravity);
        shader.SetFloat("a", area);
        shader.SetFloat("l", length);
    }

    public IEnumerator Thermal(RenderTexture terrain, int iterations, int cycles, float amount, float talus){
        int kernel = shader.FindKernel("Thermal");

        RenderTexture rock = TextureUtil.Copy(terrain);
        RenderTexture soil = TextureUtil.Float(terrain.width, amount);

        RenderTexture flux_l = TextureUtil.Float(terrain.width);
        RenderTexture flux_r = TextureUtil.Float(terrain.width);
        RenderTexture flux_t = TextureUtil.Float(terrain.width);
        RenderTexture flux_b = TextureUtil.Float(terrain.width);

        shader.SetTexture(kernel, "terrain", terrain);
        shader.SetTexture(kernel, "rock", rock);
        shader.SetTexture(kernel, "soil", soil);

        shader.SetTexture(kernel, "flux_l", flux_l);
        shader.SetTexture(kernel, "flux_r", flux_r);
        shader.SetTexture(kernel, "flux_t", flux_t);
        shader.SetTexture(kernel, "flux_b", flux_b);

        shader.SetFloat("talus", talus);

        for (int i=0; i<iterations; i++){
            for (int j=0; j<cycles; j++){
                shader.SetInt("i", 0); shader.Dispatch(kernel, terrain.width / 8, terrain.height / 8, 1);
                shader.SetInt("i", 1); shader.Dispatch(kernel, terrain.width / 8, terrain.height / 8, 1);
            }
            yield return null;
        }

        rock.Release();
        soil.Release();

        flux_l.Release();
        flux_r.Release();
        flux_t.Release();
        flux_b.Release();
    }
}