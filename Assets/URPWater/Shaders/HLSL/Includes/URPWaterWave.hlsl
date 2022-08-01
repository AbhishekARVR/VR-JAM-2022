//-----------------------------------------------------------------------------
// Copyright(C) Yan Verde - All Rights Reserved
// Copyright protected under Unity Asset Store EULA
// Refer to https://unity3d.com/legal/as_terms for more informations
// -----------------------------------------------------------------------------
// URP Water
// Author : Yan Verde
// Date : April 10, 2021
// -----------------------------------------------------------------------------

#ifndef URPWATER_WAVE_INCLUDED
#define URPWATER_WAVE_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"   
#include "URPWaterHelpers.hlsl"
#include "URPWaterVariables.hlsl"

struct WaveData {
	float4 wave;
	float speed;
};

float3 GerstnerWave(WaveData data, float3 p, inout float3 tangent, inout float3 binormal)
{
	// to do in inspector
	//float angle = wave.x;
	//float2 dir = float2(cos(angle), sin(angle));

	float speed = _Time.y * data.speed;
	float steepness = data.wave.z * 0.1;
	float wavelength = data.wave.w;
	float k = 6.28318 / wavelength; // 2 * PI
	float c = sqrt(9.8 / k);
	float2 d = normalize(data.wave.xy);
	float f = k * (dot(d, p.xz) - c * speed);
	float a = steepness / k;
	float sinF = sin(f);
	float cosF = cos(f);
	float sinSteepness = steepness * sinF;
	float cosSteepness = steepness * cosF;

	tangent += float3(
		-d.x * d.x * sinSteepness,
		d.x * cosSteepness,
		-d.x * d.y * sinSteepness
		);
	binormal += float3(
		-d.x * d.y * sinSteepness,
		d.y * cosSteepness,
		-d.y * d.y * sinSteepness
		);
	return float3(
		d.x * (a * cosF),
		a * sinF,
		d.y * (a * cosF)
		);
}


void ComputeWaves(inout Attributes v)
{
	
	#if _DISPLACEMENTMODE_GERSTNER
		float3 gridPoint = TransformObjectToWorld(v.vertex.xyz);
		float3 tangent = float3(1, 0, 0);
		float3 binormal = float3(0, 0, 1);
		float3 p = gridPoint;

		WaveData waveData[4];
		waveData[0].wave = _GerstnerWaveA;
		waveData[0].speed = _GerstnerSpeedA;

		waveData[1].wave = _GerstnerWaveB;
		waveData[1].speed = _GerstnerSpeedB;

		waveData[2].wave = _GerstnerWaveC;
		waveData[2].speed = _GerstnerSpeedC;

		waveData[3].wave = _GerstnerWaveD;
		waveData[3].speed = _GerstnerSpeedD;


		UNITY_LOOP
		for (uint i = 0; i < _WaveCount; i++)
		{
			p += GerstnerWave(waveData[i], gridPoint, tangent, binormal);
		}

		float3 normal = normalize(cross(binormal, tangent));


		p = TransformWorldToObject(p);

		float fakeOffset = (p.y + _WaveEffectsBoost) - v.vertex.y;
		float amplitude = _WaveAmplitude;

		#if _DISPLACEMENT_MASK_ON
		amplitude *= v.color.b;
		#endif

		v.waveHeight = amplitude * (fakeOffset * 0.5 + 0.5);
		v.vertex.xyz = lerp(v.vertex.xyz, p, amplitude);
		v.normal = lerp(v.normal, normal, amplitude * _WaveNormal);
	#endif
}

#endif
