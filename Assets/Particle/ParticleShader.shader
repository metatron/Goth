Shader "TransparentIssue" {
    Properties {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }
   
    Category {
        Tags {"Queue"="AlphaTest" "IgnoreProjector"="False" "RenderType"="Transparent"}
        Alphatest Greater 0
        Lighting Off
        ColorMask RGB
        Cull Off
        ZWrite Off
        Fog {Mode off}
   
        SubShader {
            Pass {
                Lighting Off
                BindChannels {
                    Bind "Vertex", vertex
                    Bind "Texcoord", texcoord
                    Bind "Color", color
                }
 
                Blend SrcAlpha OneMinusSrcAlpha
                SetTexture [_MainTex] {
                    combine primary * texture
                }
            }
        }
    }
}