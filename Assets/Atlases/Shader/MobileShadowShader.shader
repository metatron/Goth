// Simplified VertexLit Blended Particle shader. Differences from regular VertexLit Blended Particle one:
// - no AlphaTest
// - no ColorMask

Shader "Custom/MobileShadow" {
Properties {
	_EmisColor ("Emissive Color", Color) = (0,0,0,0)
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="False" "RenderType"="Opaque" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Back ZWrite Off 
	
	Lighting On
	Material { Emission [_EmisColor] }
	ColorMaterial AmbientAndDiffuse

	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}
	//FallBack "Legacy Shaders/Transparent/Cutout/Deffuse"
	FallBack "Standard"
}