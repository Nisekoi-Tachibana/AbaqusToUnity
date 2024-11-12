Shader "Custom/NodeShader"
{
    Properties  
    {  
        _Color ("Color", Color) = (1, 1, 1, 1)  
        _Cutoff("Alpha Cutoff",Range(0,1)) = 1

    }  
    SubShader  
    {  
            Tags
        {
	        "Queue"="AlphaTest"
	        "IgnoreProjector"="True"
        }

        LOD 200  
      
        
        Pass  
        {  
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag 
            float _Cutoff;


        struct appdata_t
        {
            float4 vertex : POSITION;
            float4 color : COLOR;
        };

        struct v2f
        {
            float4 pos : SV_POSITION;
            float4 color : COLOR;
        };

        v2f vert(appdata_t v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex); // 将顶点转换为裁剪空间  
            o.color = v.color; // 将颜色传递到片段着色器  
            return o;
        }

        fixed4 frag(v2f i) : SV_TARGET
        {
                        //根据已经设定的阈值决定是否着色
            if (i.color.a < _Cutoff)
            {
                discard;
            }
            return i.color; // 根据传入颜色进行着色  
        }
                        ENDCG  
                    }  
      }  
    FallBack"Diffuse"  
}
