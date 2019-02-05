﻿//////////////////////
////   GLOBALS
//////////////////////
Texture2D textures[2];
SamplerState SampleType;

cbuffer LightBuffer
{
	float4 ambientColor;
	float4 diffuseColor;
	float3 lightDirection;
	float specularPower;
	float4 specularColor;
};

cbuffer ShaderParams
{
    int EnableTexture;
};

cbuffer Shader_601151254Params
{
    float4 C002MaterialColour;
};

//////////////////////
////   TYPES
//////////////////////
struct PixelInputType
{
	float4 position : SV_POSITION;
    float3 normal : NORMAL;
	float2 tex0 : TEXCOORD0;
	float2 tex7 : TEXCOORD1;
	float3 viewDirection : TEXCOORD2;
};

float4 CalculateColor(PixelInputType input, float4 color)
{
    float3 lightDir;
    float lightIntensity;
    float3 reflection;
    float4 specular;

    // Set the default output color to the ambient light value for all pixels.
    color = ambientColor;

	// Initialize the specular color.
    specular = float4(0.0f, 0.0f, 0.0f, 0.0f);

	// Invert the light direction for calculations.
    lightDir = -lightDirection;

	// Calculate the amount of the light on this pixel.
    lightIntensity = saturate(dot(input.normal, lightDir));

    if (lightIntensity > 0.0f)
    {
		// Determine the final diffuse color based on the diffuse color and the amount of the light intensity.
        color += (diffuseColor * lightIntensity);

		// Saturate the ambient and diffuse color.
        color = saturate(color);

		// Calculate the reflection vector based on the light intensity, normal vector, and light direction.
        reflection = normalize(2 * lightIntensity * input.normal - lightDir);

		// Determine the amount of the specular light based on the reflection vector, viewing direction, and specular power.
        specular = pow(saturate(dot(reflection, input.viewDirection)), specularPower);
    }
    color = saturate(color + specular);
    return color;
}

float4 LightPixelShader(PixelInputType input) : SV_TARGET
{
    float4 color = float4(0.0f, 0.0f, 0.0f, 0.0f);
	float4 diffuseTextureColor;
	float4 aoTextureColor;

    diffuseTextureColor = textures[0].Sample(SampleType, input.tex0);
    aoTextureColor = textures[1].Sample(SampleType, input.tex7);

    color = CalculateColor(input, color);
	color = color * diffuseTextureColor;

	return color;
}

float4 PS_601151254(PixelInputType input) : SV_TARGET
{
    float4 color = float4(0.0f, 0.0f, 0.0f, 0.0f);

    color = CalculateColor(input, color);
    color = color * C002MaterialColour;

    return color;
}