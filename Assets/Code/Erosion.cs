using UnityEngine;
using System.Collections;

public class Erosion {

    public static ComputeShader shader = Resources.Load<ComputeShader>("Erosion");

    private int iterations;
    private int cycles;

    public Erosion(int iterations, int cycles, float dt = 0.001f, float gravity = 9.81f, float area = 1, float length = 1){
        this.iterations = iterations;
        this.cycles = cycles;

        shader.SetFloat("dt", dt);
        shader.SetFloat("g", gravity);
        shader.SetFloat("a", area);
        shader.SetFloat("l", length);
    }

    public IEnumerator Hydraulic(RenderTexture terrain, float rain, float evaporation){
        int kernel = shader.FindKernel("Hydraulic");
        int passes = 3;

        RenderTexture rock = TextureUtil.Copy(terrain);
        RenderTexture water = TextureUtil.Float(terrain.width);

        RenderTexture flux_l = TextureUtil.Float(terrain.width);
        RenderTexture flux_r = TextureUtil.Float(terrain.width);
        RenderTexture flux_t = TextureUtil.Float(terrain.width);
        RenderTexture flux_b = TextureUtil.Float(terrain.width);

        shader.SetTexture(kernel, "terrain", terrain);
        shader.SetTexture(kernel, "rock", rock);
        shader.SetTexture(kernel, "water", water);

        shader.SetTexture(kernel, "flux_l", flux_l);
        shader.SetTexture(kernel, "flux_r", flux_r);
        shader.SetTexture(kernel, "flux_t", flux_t);
        shader.SetTexture(kernel, "flux_b", flux_b);

        shader.SetFloat("rain", rain);
        shader.SetFloat("evaporation", evaporation);

        uint gx, gy, gz;
        shader.GetKernelThreadGroupSizes(kernel, out gx, out gy, out gz);

        for (int i=0; i<iterations; i++){
            for (int j=0; j<cycles; j++){
                for (int p=0; p<passes; p++){
                    shader.SetInt("p", p); shader.Dispatch(kernel, (int)(terrain.width / gx), (int)(terrain.height / gy), 1);
                }
            }
            yield return null;
        }

        rock.Release();
        water.Release();

        flux_l.Release();
        flux_r.Release();
        flux_t.Release();
        flux_b.Release();
    }

    public IEnumerator Thermal(RenderTexture terrain, float amount, float talus, float friction){
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
        shader.SetFloat("friction", friction);
        
        uint gx, gy, gz;
        shader.GetKernelThreadGroupSizes(kernel, out gx, out gy, out gz);

        for (int i=0; i<iterations; i++){
            for (int j=0; j<cycles; j++){
                shader.SetInt("p", 0); shader.Dispatch(kernel, (int)(terrain.width / gx), (int)(terrain.height / gy), 1);
                shader.SetInt("p", 1); shader.Dispatch(kernel, (int)(terrain.width / gx), (int)(terrain.height / gy), 1);
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