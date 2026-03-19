// Sprite Shadow Shader - AllenDevs
 
Shader "Sprites/Custom/SpriteShadow"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Sprite Color", Color) = (1,1,1,1)
        _ShadowColor("Shadow Color", Color) = (0,0,0,0.5)
        _ShadowOffset("Shadow Offset", Vector) = (0.05, -0.05, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _Color;
            float4 _ShadowColor;
            float4 _ShadowOffset;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                // Original sprite position
                float4 pos = TransformObjectToHClip(IN.positionOS.xyz);

                // Draw shadow first by offsetting vertex
                float4 shadowPos = pos;
                shadowPos.xy += _ShadowOffset.xy * _ScreenParams.xy;

                // Assign shadow color
                OUT.color = _ShadowColor;
                OUT.uv = IN.uv;
                OUT.positionCS = shadowPos;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                return tex * IN.color; // Multiply by color (shadow or sprite)
            }
            ENDHLSL
        }
    }

    Fallback "Unlit/Transparent"
}