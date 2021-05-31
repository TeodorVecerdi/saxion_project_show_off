#ifndef GETLIGHT_H
#define GETLIGHT_H

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightDefinition.cs.hlsl"

void GetLight_float(out float3 Direction, out float3 Color) {
	#ifdef SHADERGRAPH_PREVIEW
	Direction = float3(0.707, 0.707, 0);
	Color = 1;
	#else
	if (_DirectionalLightCount > 0) {
		
		DirectionalLightData light = _DirectionalLightDatas[0];
		Direction = -light.forward.xyz;
		Color = light.color;
	} else {
		Direction = float3(1, 0, 0);
		Color = 0;
	}
	#endif
}

#endif
