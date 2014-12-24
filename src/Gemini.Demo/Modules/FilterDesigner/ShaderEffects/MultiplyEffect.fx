sampler2D input1 : register(s0);
sampler2D input2 : register(s1);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 color1 = tex2D(input1, uv); 
	float4 color2 = tex2D(input2, uv); 
	return color1 * color2;
}