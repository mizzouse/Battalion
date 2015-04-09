//----------------------------------------------------
//--                                                --
//--             freeinfantry.org                   --
//--               Shader code                      --
//--                                                --
//----------------------------------------------------

//------- Constants --------
float4x4 View;
float4x4 Projection;
float4x4 World;
float4x4 LightWorldProjection;
float4 Color;
float3 LightDir;
float LightPower;
float Ambient;
bool EnableLighting;
float3 CameraPosition;
float ReflectivePower;

float fogNear = 50.0f;
float fogFar = 230.0f;
float fogThinning = 10.0f;
float fogAltitude = 10.0f;
float4 fogColor = float4(0.8f, 0.8f, 0.8f, 1.0f);

bool clipping;
float4 clippingPlane;
float waterElevation;
float4 waterColorDark;
float4 waterColorLight;
float4x4 ReflectionView;
float WaveHeight;
float WaveLength;
float WindForce;
float3 WindDirection;

float TotalTime;

//Sample name how-to for custom textures
//Define above your sampler
//Texture -texture name-;
//sampler TextureSampler = sampler_state {
//											texture = <texture name>;
//											magfilter = LINEAR or ANISOTROPIC;
//											minfilter = LINEAR or ANISOTROPIC;
//											mipfilter = LINEAR or ANISOTROPIC;
//											AddressU = mirror OR wrap;
//											AddressV = mirror OR wrap; };

//------- Texture Samplers --------
Texture xTexture;
sampler TextureSampler = sampler_state {		texture = <xTexture>;
												magfilter = LINEAR;
												minfilter = LINEAR;
												mipfilter = LINEAR;
												AddressU = mirror;
												AddressV = mirror; };
Texture Texture0;
sampler SandTextureSampler = sampler_state {	texture = <Texture0>; 
												magfilter = LINEAR; 
												minfilter = LINEAR; 
												mipfilter = LINEAR; 
												AddressU = wrap; 
												AddressV = wrap; };
Texture Texture1;
sampler GrassTextureSampler = sampler_state {	texture = <Texture1>;
												magfilter = LINEAR;
												minfilter = LINEAR;
												mipfilter = LINEAR;
												AddressU = wrap;
												AddressV = wrap; };
Texture Texture2;
sampler RockTextureSampler = sampler_state {	texture = <Texture2>;
												magfilter = LINEAR;
												minfilter = LINEAR;
												mipfilter = LINEAR;
												AddressU = wrap;
												AddressV = wrap; };
Texture Texture3;
sampler SnowTextureSampler = sampler_state {	texture = <Texture3>;
												magfilter = LINEAR;
												minfilter = LINEAR;
												mipfilter = LINEAR;
												AddressU = wrap;
												AddressV = wrap; };
Texture HeightMap;
sampler HeightMapSampler = sampler_state {		texture = <HeightMap>;
												minfilter = LINEAR;
												magfilter = LINEAR;
												mipfilter = LINEAR;
												AddressU = Wrap;
												AddressV = Wrap;
												AddressW = Wrap; };
Texture ShadowMap;	
sampler ShadowMapSampler = sampler_state {		texture = <ShadowMap>; 
												MinFilter = POINT; 
												MagFilter = POINT;
												MipFilter = NONE;  
												AddressU = Clamp;
												AddressV = Clamp;	 
												AddressW  = Wrap; };
Texture RefractionMap;
sampler RefractionMapSampler = sampler_state {	texture = <RefractionMap>;
												MagFilter = LINEAR;
												MinFilter = LINEAR;
												MipFilter = LINEAR;
												AddressU = mirror;
												AddressV = mirror; };
Texture ReflectionMap;
sampler ReflectionMapSampler = sampler_state {  texture = <ReflectionMap>;
												MagFilter = LINEAR;
												MinFilter = LINEAR;
												MipFilter = LINEAR;
												AddressU = mirror;
												AddressV = mirror; };
Texture BumpMap;
sampler2D BumpMapSampler = sampler_state {		texture = <BumpMap>;
												magfilter = LINEAR; 
												minfilter = LINEAR; 
												mipfilter = LINEAR; 
												AddressU  = Wrap;
												AddressV  = Wrap; 
												AddressW  = Wrap; };

// ------- Structures for all inputs/outputs -------
struct VS_INPUT
{
    float4 Position			: POSITION0;
    float3 Normal			: NORMAL0;
	float2 TextureCoords	: TEXCOORD0;
};

struct VSWATER_INPUT
{
	float4 Position			: POSITION0;
	float2 TextureCoords	: TEXCOORD0;
};

struct VSCOLOR_OUTPUT
{
	float4 Position		: POSITION;
	float4 Color		: COLOR0;
	float3 Normal		: TEXCOORD0;
	float3 LightingFactor : TEXCOORD1;
};

struct VSTEX_OUTPUT
{
	float4 Position		: POSITION;
	float2 TexCoord		: TEXCOORD0;
	float3 LightingFactor : TEXCOORD1;
	float4 clipDistances : TEXCOORD2;
};

struct VSMULTI_OUTPUT
{
    float4 Position     : POSITION;
    float2 TexCoord     : TEXCOORD0;
	float3 Normal		: TEXCOORD1;
    float4 TexWeights	: TEXCOORD2;
    float4 WorldPos		: TEXCOORD3;
	float4 LightDirect  : TEXCOORD4;
	float LightingFactor : TEXCOORD5;
	float Depth			: TEXCOORD6;
	float4 clipDistances : TEXCOORD7;
};

struct VSWATER_OUTPUT
{
	float4 Position		: POSITION;
	float4 RefPos		: TEXCOORD1;
	float2 BumpPos		: TEXCOORD2;
	float4 RefractPos	: TEXCOORD3;
	float4 CameraPos	: TEXCOORD4;
};

// ================ Creating Texture Weights ================
float4 CreateTexWeights( float4 Position )
{
	float4 texWeights = float4(0,0,0,0);

	//Note: we use position y because our vertices uses heightdata[x,y] for y
    texWeights.x = clamp(1.0f - abs(Position.y - 0) / 8.0f, 0, 1);
    texWeights.y = clamp(1.0f - abs(Position.y - 10) / 6.0f, 0, 1);
    texWeights.z = clamp(1.0f - abs(Position.y - 20) / 6.0f, 0, 1);
    texWeights.w = clamp(1.0f - abs(Position.y - 30) / 6.0f, 0, 1);
	float total = texWeights.x;
	total += texWeights.y;
	total += texWeights.z;
	total += texWeights.w;
	texWeights.x /= total;
	texWeights.y /= total;
	texWeights.z /= total;
	texWeights.w /= total;

	return texWeights;
}

// ================= For Shadow Mapping =================

// ------- Computing shadow colors -------
// For any shader that supports shadow mapping
float4 ComputeShadowing(float4 worldPOS, float4 Color)
{
	// Find the position of the pixel in the light
	float4 lightingPosition = mul(worldPOS, LightWorldProjection);

	// Find the position in the shadow map for this pixel
	float2 ShadowTexCoord = 0.5f * lightingPosition.xy /
									lightingPosition.w +
									float2 (0.5f, 0.5f);
	ShadowTexCoord.y = 1.0f - ShadowTexCoord.y;

    // Get the current depth stored in the shadow map
    float4 shadowInfo = tex2D(ShadowMapSampler, ShadowTexCoord);
    float shadowdepth = shadowInfo.r;
    float shadowOpacity = 0.5f + 0.5f * (1 - shadowInfo.g);
    
    // Calculate the current pixel depth
    float ourdepth = (lightingPosition.z / lightingPosition.w);

    // Check to see if this pixel is in front or behind the value in the shadow map
    if ( shadowdepth < ourdepth)
    {
        // Shadow the pixel by lowering the intensity
        Color *= float4(shadowOpacity, shadowOpacity, shadowOpacity, 1);
    };
    
    return Color;
}

struct DrawShadowMap_VSIn
{
	float4 Position : POSITION0;
	float3 Normal	: NORMAL0;
};

struct CreateShadowMap_VSOut
{
	float4 Position : POSITION;
	float Depth		: TEXCOORD0;
};

// Creates the model into light space and renders out the depth
CreateShadowMap_VSOut CreateShadowMap_VS(DrawShadowMap_VSIn input)
{
	CreateShadowMap_VSOut Output;
	Output.Position = mul(input.Position, mul(World, LightWorldProjection));
	Output.Depth = Output.Position.z / Output.Position.w;

	return Output;
}

// Saves the depth to a 32bit floating texture
float4 CreateShadowMap_PS(CreateShadowMap_VSOut input) : COLOR
{
	return float4(input.Depth, 1, 0, 1);
}

//------- Technique: CreateShadowMap --------
technique CreateShadowMap
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 CreateShadowMap_VS();
		PixelShader = compile ps_2_0 CreateShadowMap_PS();
	}
}
// ================= End Shadow Mapping =================

// ======= Our Conversion Structures =======
// These structures take the defined input from applying our effects
// and then transforms it into needed info per effect

VSCOLOR_OUTPUT ColoredVS( VS_INPUT input)
{
	VSCOLOR_OUTPUT Output;

	float4x4 preViewProjection = mul(View, Projection);
	float4x4 preWorldViewProjection = mul(World, preViewProjection);

	Output.Position = mul(input.Position, preWorldViewProjection);
	Output.Color = Color;

	Output.Normal = normalize(mul(input.Normal, World));
	Output.LightingFactor = 1;
	if (EnableLighting)
		Output.LightingFactor = saturate(dot(Output.Normal, -LightDir));

	return Output;
}

VSCOLOR_OUTPUT ColorNoShadingVS( VS_INPUT input )
{
    VSCOLOR_OUTPUT Output;

	float4x4 preViewProjection = mul(View, Projection);
	float4x4 preWorldViewProjection = mul(World, preViewProjection);

	Output.Position = mul(input.Position, preWorldViewProjection);
	Output.Color = Color;
	Output.Normal = normalize(mul(input.Normal, World));
	Output.LightingFactor = 0;

	return Output;
}

VSTEX_OUTPUT TexturedVS( VS_INPUT input )
{
    VSTEX_OUTPUT Output;

	float4x4 preViewProject = mul(View, Projection);
	float4x4 preWorldVP = mul(World, preViewProject);

	Output.Position = mul(input.Position, preWorldVP);
	Output.TexCoord = input.TextureCoords;
	
	float3 Normal = normalize(mul(normalize(input.Normal), World));
	Output.LightingFactor = 1;
	if (EnableLighting)
		Output.LightingFactor = saturate(dot(Normal, -LightDir));

	//Output.Depth = Output.Position.z / Output.Position.w;

	if (clipping)
		Output.clipDistances = dot(Output.Position, clippingPlane);

	return Output;
}

VSMULTI_OUTPUT MultiTexturedVS( VS_INPUT input)    
{
    VSMULTI_OUTPUT Output;
	
	float4x4 ViewProject = mul(View, Projection);
	float4x4 WorldViewProjection = mul(World, ViewProject);  
    Output.Position = mul(input.Position, WorldViewProjection);
    Output.WorldPos = mul(input.Position, World);
    Output.Normal = normalize(mul(input.Normal, World));
	Output.TexCoord = input.TextureCoords;

	Output.TexWeights = CreateTexWeights(input.Position);

	Output.LightDirect.xyz = -LightDir;
	Output.LightDirect.w = 1;
	if (EnableLighting)
		Output.LightingFactor = saturate(dot(Output.Normal, -LightDir));
	Output.Depth = Output.Position.z / Output.Position.w;

	if (clipping)
		Output.clipDistances = dot(Output.WorldPos, clippingPlane);

    return Output;    
}

VSWATER_OUTPUT WaterVS( VSWATER_INPUT input )
{
	VSWATER_OUTPUT Output;

	float4x4 ViewProject = mul(View, Projection);
	float4x4 WorldViewProject = mul(World, ViewProject);
	float4x4 preViewProjectReflection = mul(ReflectionView, Projection);
	float4x4 preWorldReflection = mul(World, preViewProjectReflection);

	float3 windDir = normalize(WindDirection);
	float3 Dir = cross(WindDirection, float3(0,1,0));
	float yDot = dot(input.TextureCoords, WindDirection.xz);
	float xDot = dot(input.TextureCoords, Dir.xz);
	float2 moveDir = float2(xDot, yDot);
	moveDir.y += TotalTime * WindForce;

	Output.Position = mul(input.Position, WorldViewProject);
	Output.RefPos = mul(input.Position, preWorldReflection);

	Output.BumpPos = moveDir / WaveLength;
	Output.RefractPos = Output.Position;
	Output.CameraPos = mul(input.Position, World);

	return Output;
}

// ======= End Conversion Structs =======

//------- Technique: Colored --------
float4 ColoredPS(VSCOLOR_OUTPUT PSIn) : COLOR0
{
	float4 Color = PSIn.Color;
    Color.rgb *= saturate(PSIn.LightingFactor + Ambient);    
    
    return Color;
}

technique Colored
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 ColoredVS();
        PixelShader = compile ps_2_0 ColoredPS();
    }
}

//------- Technique: ColorNoShading --------
float4 ColorNoShadingPS(VSCOLOR_OUTPUT PSIn) : COLOR0
{
	return float4(PSIn.Color);
}

technique ColorNoShading
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 ColorNoShadingVS();
		PixelShader = compile ps_2_0 ColorNoShadingPS();
	}
}

//------- Technique: Textured --------
float4 TexturedPS(VSTEX_OUTPUT PSIn) : COLOR0
{
	if (clipping)
		clip(PSIn.clipDistances);

	float4 Color = tex2D(TextureSampler, PSIn.TexCoord);
    Color.rgb *= saturate(PSIn.LightingFactor + Ambient);

    return Color;
}

technique Textured
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 TexturedVS();
        PixelShader = compile ps_2_0 TexturedPS();
    }
}

//------- Technique: MultiTextured --------
float4 MultiTexturedPS(VSMULTI_OUTPUT PSIn) : COLOR0
{
	if (clipping)
		clip(PSIn.clipDistances);

    float lightingFactor = 1;
    if (EnableLighting)
        lightingFactor = saturate(saturate(dot(PSIn.Normal, PSIn.LightDirect)) + Ambient);

    float blendDistance = 0.99f;
    float blendWidth = 0.005f;
    float blendFactor = clamp((PSIn.Depth - blendDistance) / blendWidth, 0, 1);

    float4 farColor;
    farColor = tex2D(SandTextureSampler, PSIn.TexCoord) * PSIn.TexWeights.x;
    farColor += tex2D(GrassTextureSampler, PSIn.TexCoord) * PSIn.TexWeights.y;
    farColor += tex2D(RockTextureSampler, PSIn.TexCoord) * PSIn.TexWeights.z;
    farColor += tex2D(SnowTextureSampler, PSIn.TexCoord) * PSIn.TexWeights.w;

	//We shadow map here before our texture increases
	farColor = ComputeShadowing(PSIn.WorldPos, farColor);
     
    float4 nearColor;
    float2 nearTextureCoords = PSIn.TexCoord * 3;
    nearColor = tex2D(SandTextureSampler, nearTextureCoords) * PSIn.TexWeights.x;
    nearColor += tex2D(GrassTextureSampler, nearTextureCoords) * PSIn.TexWeights.y;
    nearColor += tex2D(RockTextureSampler, nearTextureCoords) * PSIn.TexWeights.z;
    nearColor += tex2D(SnowTextureSampler, nearTextureCoords) * PSIn.TexWeights.w;
	
	nearColor = ComputeShadowing(PSIn.WorldPos, nearColor);

    float4 Color = lerp(nearColor, farColor, blendFactor);
    Color *= lightingFactor;

    return Color;
}

technique MultiTextured
{
	pass Pass0
	{
		VertexShader = compile vs_1_1 MultiTexturedVS();
		PixelShader = compile ps_2_0 MultiTexturedPS();
	}
}

//------- Technique: Water --------
float4 WaterPS( VSWATER_OUTPUT input ) : COLOR
{
	float4 bumpColor = tex2D(BumpMapSampler, input.BumpPos);
	float2 perturbed = WaveHeight * (bumpColor.rg - 0.5f) * 2.0f;

	float2 projectReflectCoords;
	projectReflectCoords.x = input.RefPos.x / input.RefPos.w / 2.0f + 0.5f;
	projectReflectCoords.y = -input.RefPos.y / input.RefPos.w / 2.0f + 0.5f;
	float2 perturbedCoords = projectReflectCoords + perturbed;
	float4 reflectiveColor = tex2D(ReflectionMapSampler, perturbedCoords);

	float2 projectRefractCoords;
	projectRefractCoords.x = input.RefractPos.x / input.RefractPos.w / 2.0f + 0.5f;
	projectRefractCoords.y = -input.RefractPos.y / input.RefractPos.w / 2.0f + 0.5f;
	float2 perturbedRefractCoords = projectRefractCoords + perturbed;
	float4 refractiveColor = tex2D(RefractionMapSampler, perturbedRefractCoords);

	float3 eyeView = normalize(CameraPosition - input.CameraPos);
	float3 normal = (bumpColor.rbg - 0.5f) * 2.0f;
	float fresnel = saturate(dot(eyeView, normal));
	float3 reflectionVector = -reflect(LightDir, normal);
	float specular = dot(normalize(reflectionVector), normalize(eyeView));

	float4 combined = lerp(reflectiveColor, refractiveColor, fresnel);
	float4 Color = lerp(combined, waterColorLight, 0.2f);
	specular = pow(abs(specular), 256);
	Color.rgb += specular * ReflectivePower / 100.0f;

	return Color;
}

technique Water
{
	pass Pass0
	{
		VertexShader = compile vs_1_1 WaterVS();
		PixelShader = compile ps_2_0 WaterPS();
	}
}
