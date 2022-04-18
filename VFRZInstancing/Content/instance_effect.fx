#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
	

//uniform float2 TextureSize = float2(2048, 2048);

//Number of Textures inside of the Texture3D
uniform float NumberOf2DTextures;
uniform matrix WorldViewProjection;

uniform Texture3D SpriteTexture;

sampler SpriteTextureSampler : register(s0) = sampler_state 
{
    Texture = (SpriteTexture);
};

//8 bytes in total
struct StaticVSinput
{
	float4 Position : COLOR0; // only xyz are needed
	float4 TexCoord : COLOR1; //only x/y is needed
};

//16 bytes in total
struct DynamicVSinput
{
	float3 InstanceTransform : POSITION0;
	float4 AtlasCoord : COLOR2; //x/y for column/row z for image index and w for ShadowColor
	int NewAtlasCoord : TEXCOORD3;
};

//16 byte + 12 byte = 28 bytes
struct InstancingVSoutput
{
	float4 Position : SV_POSITION;
	float3 TexCoord : TEXCOORD0;
};

cbuffer ShaderData : register(b0)
{
    float2 ImageSizeArray[256];
};

// constants
// cannot be defined above with default values, so we wrap values in functions instead?
inline int2   TextureSize()        { return int2(2048, 2048); }
inline int2   TileSize()           { return int2(32, 16); }
inline float2 NormalisedTileSize() { return float2(0.015625f, 0.0078125f); }

InstancingVSoutput InstancingVS(in StaticVSinput input, in DynamicVSinput input1)
{
	int atlasIndex = (input1.NewAtlasCoord & 0b11111111110000000000000000000000) >> 22;

	int2 tilePositionInAtlas = int2(
		(input1.NewAtlasCoord & 0b00000000001111110000000000000000) >> 16,
		(input1.NewAtlasCoord & 0b00000000000000001111111000000000) >> 9);

	int2 tileSizeInAtlas = int2(
		1 + ((input1.NewAtlasCoord & 0b00000000000000000000000111100000) >> 5),
		1 + (input1.NewAtlasCoord & 0b00000000000000000000000000011111));


	InstancingVSoutput output;

	// calculate position with camera
	input.Position.xy = (input.Position.xy - float2(0.5f, 1.0f)) * (tileSizeInAtlas * TileSize());

	output.Position = mul(
		float4(input.Position.xyz + input1.InstanceTransform, 1),
		WorldViewProjection);

	
	if (input.TexCoord.x >= 0 || input.TexCoord.y >= 0)
	{
		output.TexCoord = float3(
			(input.TexCoord.xy * (tileSizeInAtlas * NormalisedTileSize())) + (tilePositionInAtlas * NormalisedTileSize()),
			atlasIndex / NumberOf2DTextures);
	}
	else
	{
		float2 imageSize = ImageSizeArray[atlasIndex];

		float2 NumberOfTextures = TextureSize() / imageSize;

		output.TexCoord = float3(
			(input.TexCoord.xy / NumberOfTextures) + (tilePositionInAtlas * NormalisedTileSize()),
			atlasIndex / NumberOf2DTextures);
	}

	return output;
}

InstancingVSoutput InstancingVS0(in StaticVSinput input, in DynamicVSinput input1)
{
	// Colors * 255 because its between 0 - 1
	float4 atlasCoordinate = input1.AtlasCoord * 255;
	
	// actual Image Index
	float index = atlasCoordinate.z * 256 + atlasCoordinate.w;
	
	// get texture Size in the atlas
	float2 imageSize = ImageSizeArray[index];
	//float2 imageSize = float2(32, 64);
	
	// how many Images are possible inside of the big texture
	float2 NumberOfTextures = float2(2048, 2048) / float2(imageSize.x, imageSize.y); // all Images are 2048 x 2048 because 3DTexture doesnt support more and give blackscreen if bigger, maybe because old opengl 3_0
	
	input.Position.xy = input.Position.xy * imageSize - float2(imageSize.x / 2, imageSize.y);
	
	InstancingVSoutput output;

	// calculate position with camera
	output.Position = mul(
		float4(input.Position.xyz + input1.InstanceTransform, 1),
		WorldViewProjection);

	output.TexCoord = float3(
		(input.TexCoord.xy / NumberOfTextures.xy) + (1.0f / NumberOfTextures.xy * atlasCoordinate.xy),
		(index + 0.1f) / NumberOf2DTextures); // +0.1f / NumberOf2DTextures because texture3d want some between value?
	
	return output;
}

float4 InstancingPS(InstancingVSoutput input) : SV_TARGET
{
	float4 caluclatedColor = SpriteTexture.Sample(SpriteTextureSampler, input.TexCoord);
	
	//// 173 => 123 fps
	//if (caluclatedColor.a == 0) {
	//	clip(-1);
	//}
	
	return caluclatedColor;
}

technique Instancing
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL InstancingVS();
		PixelShader = compile PS_SHADERMODEL InstancingPS();
	}
};