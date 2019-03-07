﻿using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;

[Title("Procedural", "Noise", "3D Voronoi Noise")]
public class VoronoiNoise3D : CodeFunctionNode
{
    public VoronoiNoise3D() {
        name = "3D Voronoi Noise";
    }

    protected override MethodInfo GetFunctionToConvert() {
        return GetType().GetMethod("GetVoronoiNoise3D",
            BindingFlags.Static | BindingFlags.NonPublic);
    }

	static string GetVoronoiNoise3D(
    [Slot(0, Binding.WorldSpacePosition)] Vector3 XYZ,
    [Slot(1, Binding.None, 10, 0, 0, 0)] Vector1 Scale,
    [Slot(2, Binding.None, 1, 0, 0, 0)] Vector1 Jitter01,
    [Slot(3, Binding.None)] out Vector1 Out) {
        return
            @"
{
    Out = fBm_F1_F0(XYZ, 1, Scale, 2, 0.5, Jitter01);
} 
";
    }

    public override void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode) {
        registry.ProvideFunction("mod", s => s.Append(@"
            float3 mod(float3 x, float y) { return x - y * floor(x/y); }
        "));

        registry.ProvideFunction("Permutation", s => s.Append(@"
            float3 Permutation(float3 x) 
            {
              return mod((34.0 * x + 1.0) * x, 289.0);
            }
        "));

        registry.ProvideFunction("inoise", s => s.Append(@"
            float2 inoise(float3 P, float jitter)
            {			
             float3 Pi = mod(floor(P), 289.0);
              float3 Pf = frac(P);
             float3 oi = float3(-1.0, 0.0, 1.0);
             float3 of = float3(-0.5, 0.5, 1.5);
             float3 px = Permutation(Pi.x + oi);
             float3 py = Permutation(Pi.y + oi);

             float3 p, ox, oy, oz, dx, dy, dz;
             float2 F = 1e6;

             for(int i = 0; i < 3; i++)
             {
              for(int j = 0; j < 3; j++)
              {
               p = Permutation(px[i] + py[j] + Pi.z + oi); // pij1, pij2, pij3

               ox = frac(p*0.142857142857) - 0.428571428571;
               oy = mod(floor(p*0.142857142857),7.0)*0.142857142857 - 0.428571428571;

               p = Permutation(p);

               oz = frac(p*0.142857142857) - 0.428571428571;

               dx = Pf.x - of[i] + jitter*ox;
               dy = Pf.y - of[j] + jitter*oy;
               dz = Pf.z - of + jitter*oz;

               float3 d = dx * dx + dy * dy + dz * dz; // dij1, dij2 and dij3, squared

               //Find lowest and second lowest distances
               for(int n = 0; n < 3; n++)
               {
                if(d[n] < F[0])
                {
                 F[1] = F[0];
                 F[0] = d[n];
                }
                else if(d[n] < F[1])
                {
                 F[1] = d[n];
                }
               }
              }
             }

             return F;
            }
        "));

        registry.ProvideFunction("fBm_F1_F0", s => s.Append(@"
            float fBm_F1_F0(float3 p, int octaves, float _Frequency, float _Lacunarity, float _Gain, float _Jitter)
            {
             float freq = _Frequency, amp = 0.5;
             float sum = 0;	
             for(int i = 0; i < octaves; i++) 
             {
              float2 F = inoise(p * freq, _Jitter) * amp;

              sum += 0.1 + sqrt(F[1]) - sqrt(F[0]);

              freq *= _Lacunarity;
              amp *= _Gain;
             }
             return sum;
            }
        "));

        //registry.ProvideFunction("fBm_F0", s => s.Append(@"
        //    float fBm_F0(float3 p, int octaves, float _Frequency, float _Lacunarity, float _Gain, float _Jitter)
        //    {
        //     float freq = _Frequency, amp = 0.5;
        //     float sum = 0;	
        //     for(int i = 0; i < octaves; i++) 
        //     {
        //      float2 F = inoise(p * freq, _Jitter) * amp;

        //      sum += 0.1 + sqrt(F[0]);

        //      freq *= _Lacunarity;
        //      amp *= _Gain;
        //     }
        //     return sum;
        //    }
        //"));

        base.GenerateNodeFunction(registry, graphContext, generationMode);
    }
}
