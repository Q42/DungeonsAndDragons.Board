Shader "Fog of War/Fog" {
	Properties {
		_MainTex ("noise", 2D) = "white" {}
	}
	CGINCLUDE;
	
	#include "UnityCG.cginc"
	#include "Camera.cginc"
	
	#define MARCH(STEPS,MAPLOD) for(int i=0; i<STEPS; i++) { float3  pos = ro + t*rd; if( pos.y<-3.0 || pos.y>2.0 || sum.a > 0.99 ) break; float den = MAPLOD( pos ); if( den>0.01 ) { float dif =  clamp((den - MAPLOD(pos+0.3*sundir))/0.6, 0.0, 1.0 ); sum = integrate( sum, dif, den, bgcol, t ); } t += max(0.05,0.02*t); }
    
    uniform sampler2D _MainTex;
    
    struct v2f
	{
		float4 fragColor : COLOR;
		float4 fragCoord : SV_POSITION;
		float4 texcoord : TEXCOORD0;
		float4 screenCoord : TEXCOORD1;
	};
	
	float3 sundir = normalize(float3(-1.0,0.0,-1.0));
	
	float noise (in float3 x) {
	    float3 p = floor(x);
	    float3 f = frac(x);
	    
	    f = f * f * (3.0 - 2.0 * f);
	    
	    float2 uv = (p.xy + float2(37.0, 17.0) * p.z) + f.xy;
	    float2 rg = tex2Dlod(_MainTex, float4((uv + 0.5) / 256.0, 0.0, 0.0)).yx;
	    
	    return -1.0 + 2.0 * lerp(rg.x, rg.yx, f.z);
	}
	
	float map5(in float3 p) {
        float3 q = p - float3(0.0, 0.1, 1.0) * unity_DeltaTime;
        float f;
        f = 0.50000 * noise(q); q = q * 2.02;
        f += 0.25000 * noise(q); q = q * 2.03;
        f += 0.12500 * noise(q); q = q * 2.01;
        f += 0.06250 * noise(q); q = q * 2.02;
        f += 0.03125 * noise(q);
        
        return clamp(1.5 - p.y - 2.0 + 1.75 * f, 0.0, 1.0);
	}
	
	float map4(in float3 p) {
	    float3 q = p - float3(0.0, 0.1, 1.0) * unity_DeltaTime;
	    float f;
	    f = 0.50000 * noise(q); q = q * 2.02;
        f += 0.25000 * noise(q); q = q * 2.03;
        f += 0.12500 * noise(q); q = q * 2.01;
        f += 0.06250 * noise(q);
        
        return clamp(1.5 - p.y - 2.0 + 1.75 * f, 0.0, 1.0);
	}
	
	float map3(in float3 p) {
	    float3 q = p - float3(0.0, 0.1, 1.0) * unity_DeltaTime;
	    float f;
	    f = 0.50000 * noise(q); q = q * 2.02;
        f += 0.25000 * noise(q); q = q * 2.03;
        f += 0.12500 * noise(q);
        
        return clamp(1.5 - p.y - 2.0 + 1.75 * f, 0.0, 1.0);
	}
	
	float map2(in float3 p) {
        float3 q = p - float3(0.0,0.1,1.0) * unity_DeltaTime;
        float f;
        f = 0.50000 * noise(q); q = q * 2.02;
        f += 0.25000 * noise(q);
        return clamp(1.5 - p.y - 2.0 + 1.75 * f, 0.0, 1.0);
	}
	
	float4 integrate( in float4 sum, in float dif, in float den, in float3 bgcol, in float t )
    {
        float3 lin = float3(0.65, 0.7, 0.75) * 1.4 + float3(1.0, 0.6, 0.3) * dif;        
        float4 col = float4(lerp(float4(1.0, 0.95, 0.8, 0.0), float3(0.25, 0.3, 0.35), den), den);
        
        col.xyz *= lin;
        col.xyz = lerp(col.xyz, bgcol, 1.0 - exp(-0.003 * t * t));
        col.a *= 0.4;
        col.rgb *= col.a;
        
        return sum + col * (1.0 - sum.a);
    }
    
    float4 raymarch(in float3 ro, in float3 rd, in float3 bgcol, in float2 px)
    {
        float4 sum = float4(0, 0, 0, 0);
    
        float t = 0.0;
    
        MARCH(30, map5);
        MARCH(30, map4);
        MARCH(30, map3);
        MARCH(30, map2);
    
        return clamp(sum, 0.0, 1.0);
    }
    
    float4 render(in float3 ro, in float3 rd, in float2 px)
    {
        float sun = clamp(dot(sundir, rd), 0.0, 1.0);
        float3 col = float3(0.6, 0.71, 0.75) - rd.y * 0.2 * float3(1.0, 0.5, 1.0) + 0.15 * 0.5;
        col += 0.2*float3(1.0, .6, 0.1) * pow(sun, 8.0);
    
        float4 res = raymarch(ro, rd, col, px);
        col = col * (1.0 - res.w) + res.xyz;
        
        col += 0.2 * float3(1.0, 0.4, 0.2) * pow(sun, 3.0);
    
        return float4(col, 1.0);
    }
    	
	float4 fragMain(v2f i) {
        float2 q = i.screenCoord.xy / i.screenCoord.w;
		float2 p = 2.0 * (q - 0.5);
    
        float3 ro, rd;
        computeCamera(p, ro, rd);
        
        return render(ro, rd, i.fragCoord - 0.5);
	}
	
	v2f vert(appdata_base v)
	{
		v2f o;
		o.fragCoord = UnityObjectToClipPos(v.vertex);
		o.screenCoord = ComputeScreenPos(o.fragCoord);
		return o;
	}
	
	float frag(v2f i) : SV_Target{ return fragMain(i); }
	
	ENDCG
	SubShader {
	    ZTest Always Cull Off ZWrite Off
	    
	    Pass {
            CGPROGRAM
            #include "UnityCG.cginc"
            
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
	    }
	}

	FallBack off
}
