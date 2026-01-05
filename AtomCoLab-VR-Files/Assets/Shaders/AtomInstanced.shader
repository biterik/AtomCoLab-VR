// AtomCoLab-VR - Large-Scale Atomistic Visualization
// GPU Instanced Atom Shader
// Copyright (c) 2024-2025 AtomCoLab-VR Contributors
// SPDX-License-Identifier: PolyForm-Noncommercial-1.0.0

Shader "AtomCoLab/AtomInstanced"
{
    Properties
    {
        _AtomScale ("Atom Scale", Float) = 1.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows addshadow vertex:vert
        #pragma instancing_options procedural:setup
        #pragma target 4.5

        // Atom data structure (must match C# AtomData struct)
        struct AtomData
        {
            float3 position;
            int atomType;
            float3 velocity;
            float property;
        };

        // GPU buffers
        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            StructuredBuffer<AtomData> _AtomBuffer;
            StructuredBuffer<float4> _AtomTypes; // rgb = color, a = radius
        #endif

        float _AtomScale;
        float _Smoothness;
        float _Metallic;

        struct Input
        {
            float3 worldPos;
            float4 color : COLOR;
        };

        // Instance setup - called per instance
        void setup()
        {
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                AtomData atom = _AtomBuffer[unity_InstanceID];
                float4 typeInfo = _AtomTypes[atom.atomType];
                
                float radius = typeInfo.a * _AtomScale;
                float3 position = atom.position;

                // Build transformation matrix
                unity_ObjectToWorld = float4x4(
                    radius, 0, 0, position.x,
                    0, radius, 0, position.y,
                    0, 0, radius, position.z,
                    0, 0, 0, 1
                );

                // Inverse for normals
                unity_WorldToObject = float4x4(
                    1.0/radius, 0, 0, -position.x/radius,
                    0, 1.0/radius, 0, -position.y/radius,
                    0, 0, 1.0/radius, -position.z/radius,
                    0, 0, 0, 1
                );
            #endif
        }

        // Vertex shader
        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                AtomData atom = _AtomBuffer[unity_InstanceID];
                float4 typeInfo = _AtomTypes[atom.atomType];
                o.color = float4(typeInfo.rgb, 1.0);
            #else
                o.color = float4(1, 0, 1, 1); // Magenta = error color
            #endif
        }

        // Surface shader
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = IN.color.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = 1.0;
        }
        ENDCG
    }

    // Fallback for older hardware
    FallBack "Standard"
}
