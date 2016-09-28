/// <class>ThemedImageEffect</class>
/// <description>Converts an input image into an image that blends in with the target background.</description>

/// <summary>The input image</summary>
sampler2D Input : register(S0);

/// <summary>Background color of the image.</summary>
/// <defaultValue>#FFF6F6F6</defaultValue>
float4 Background : register(C0);

/// <summary>1.0 if the image should be rendered enabled, 0.0 if it should be disabled (grayscaled).</summary>
/// <minValue>0.0</minValue>
/// <maxValue>1.0</maxValue>
/// <defaultValue>0.0</defaultValue>
float IsEnabled : register(C1);

float3 rgb2hsl(in float4 RGB)
{
	float r = RGB.r; // Red, range: [0..1]
	float g = RGB.g; // Green, range: [0..1]
	float b = RGB.b; // Blue, range: [0..1]
	     
	float maxChannel = (r > g && r > b) ? r : (g > b) ? g : b;
	float minChannel = (r < g && r < b) ? r : (g < b) ? g : b;
		
	float h = (maxChannel + minChannel) / 2.0f; // Hue, range: [0..1]
	float s = (maxChannel + minChannel) / 2.0f; // Saturation, range: [0..1]
	float l = (maxChannel + minChannel) / 2.0f; // Lightness, range: [0..1]
	  
	if (maxChannel == minChannel)
	{
	    h = s = 0.0f;
	}
	else
	{
		float d = maxChannel - minChannel;
		s = (l > 0.5f) ? d / (2.0f - maxChannel - minChannel) : d / (maxChannel + minChannel);
		
		if (r > g && r > b) // maxChannel == r
		    h = (g - b) / d + (g < b ? 6.0f : 0.0f);
		else if (g > b) // maxChannel == g
		    h = (b - r) / d + 2.0f;
		else // maxChannel = b
		    h = (r - g) / d + 4.0f;
	
	    h /= 6.0f;
	}
	  
	return float3(h, s, l);
}

float hue2rgb(in float p, in float q, in float t)
{
	if (t < 0.0f) t += 1.0f;
	if (t > 1.0f) t -= 1.0f;
	if (t < 1.0f / 6.0f) return p + (q - p) * 6.0f * t;
	if (t < 1.0f / 2.0f) return q;
	if (t < 2.0f / 3.0f) return p + (q - p) * (2.0f / 3.0f - t) * 6.0f;
	return p;
}

float3 hsl2rgb(in float3 HSL)
{
	float h = HSL[0];
	float s = HSL[1];
	float l = HSL[2];
		
	float q = (l < 0.5f) ? l * (1.0f + s) : l + s - l * s;
	float p = 2.0f * l - q;
	
	float r = hue2rgb(p, q, h + 1.0f / 3.0f);
	float g = hue2rgb(p, q, h);
	float b = hue2rgb(p, q, h - 1.0f / 3.0f);
	  
	return float3(r, g, b);
}

float3 rgb2gray(in float3 RGB) 
{
	// https://en.wikipedia.org/wiki/Relative_luminance
	// https://en.wikipedia.org/wiki/SRGB
	// Convert the gamma compressed RGB values to linear RGB
	RGB.r = (RGB.r <= 0.04045f) ? RGB.r / 12.92f : pow((RGB.r + 0.055f) / 1.055f, 2.4f);
	RGB.g = (RGB.g <= 0.04045f) ? RGB.g / 12.92f : pow((RGB.g + 0.055f) / 1.055f, 2.4f);
	RGB.b = (RGB.b <= 0.04045f) ? RGB.b / 12.92f : pow((RGB.b + 0.055f) / 1.055f, 2.4f);
	float y = dot(float3(0.2126f, 0.7152f, 0.0722f), RGB.rgb);
	return y <= 0.0031308f ? 12.92f * y : 1.055f * pow(y, 1.0f/2.4f) - 0.055f;
}

float4 main(float2 uv : TEXCOORD) : COLOR 
{ 
	// This performs two conversions.
	// 1. The lightness of the image is transformed so that the constant "halo" lightness blends in with the background. 
	// - This has the effect of eliminating the halo visually. 
	// - The "halo" lightness is an immutable constant, and is not calculated from the input image.
	// 2. The image is converted to grayscale if the IsEnabled parameter is 0.

	// Refer to http://stackoverflow.com/questions/36778989/vs2015-icon-guide-color-inversion
		
	// First, loopk up original image color
	float4 pxRGBA = tex2D(Input, uv.xy);		
		
	// For performance reason, the WPF multiplies each color channel 
	// by the alpha channel before sending the Input into HLSL shader.
	// So let's convert the color to be non-premultiplied.
    if (pxRGBA.a > 0.0) pxRGBA.rgb /= pxRGBA.a; 
    
    // Convert the color space from RGB to HSL.
    float3 pxHSL = rgb2hsl(pxRGBA); 
		
	// Define lightness of default outlined color(#F6F6F6)
	float haloHSL2 = 0.96470588235294f;
		
	// Lightness of Background color is bgHSL[2]
	float3 bgHSL = rgb2hsl(Background);
		
	if (bgHSL[2] < 0.5f)
	{
	  	haloHSL2 = 1.0f - haloHSL2;
	  	pxHSL[2] = 1.0f - pxHSL[2];
	}
	  
	if(pxHSL[2] <= haloHSL2)
	{
	  	pxHSL[2] = bgHSL[2] / haloHSL2 * pxHSL[2];
	}
	else
	{
	  	pxHSL[2] = (1.0f - bgHSL[2]) / (1.0f - haloHSL2) * (pxHSL[2] - 1.0f) + 1.0f;
	}
		
	// Convert the color space from HSL to RGB.
	pxRGBA.rgb = hsl2rgb(pxHSL);
	
	// if !IsEnabled, convert to grayscale.
	if (IsEnabled == 0.0f)
	{
	  	pxRGBA.rgb = rgb2gray(pxRGBA.rgb);
	}
	  
	// Convert the color to a PreMultiplied color.
	pxRGBA.rgb *= pxRGBA.a; 
	  
	return pxRGBA;
}