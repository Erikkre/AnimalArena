﻿Shader "Custom/HealthBar"
  {
      Properties
      {
         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
          [Header(Life)]_Color ("Main Color", Color) = (0.2,1,0.2,1)
          _Steps ("Steps", Float) = 1
          _Percent ("Percent", Float) = 1
      
          [Header(Damages)]_DamagesColor ("Damages color", Color) = (1,1,0,1)
          _DamagesPercent ("Damages Percent", Float) = 0
      
      
          [Header(Border)]_BorderColor ("Border color", Color) = (0.1,0.1,0.1,1)
          _BorderWidth ("Border width", Float) = 1
          [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
      
      
          _ImageSize ("Image Size", Vector) = (100, 100, 0, 0)
      }
  
      SubShader
      {
          Tags
          { 
              "Queue"="Transparent" 
              "IgnoreProjector"="True" 
              "RenderType"="Transparent" 
              "PreviewType"="Plane"
              "CanUseSpriteAtlas"="True"
          }
  
          Cull Off
          Lighting Off
          ZWrite Off
          Blend One OneMinusSrcAlpha
  
          Pass
          {
          CGPROGRAM
              #pragma vertex vert
              #pragma fragment frag
              #pragma multi_compile _ PIXELSNAP_ON
              #include "UnityCG.cginc"
              
                      // Use shader model 3.0 target, to get nicer looking lighting
       // #pragma target 3.0
        
              struct appdata_t
              {
                  float4 vertex   : POSITION;
                  float4 color    : COLOR;
                  float2 texcoord : TEXCOORD0;
              };
  
              struct v2f
              {
                  float4 vertex   : SV_POSITION;
                  fixed4 color    : COLOR;
                  half2 texcoord  : TEXCOORD0;
              };
              
              fixed4 _Color;
              half _Steps;
              half _Percent;
              
              fixed4 _DamagesColor;
              half _DamagesPercent;
              
              fixed4 _BorderColor;
              half _BorderWidth;
  
              v2f vert(appdata_t IN)
              {
                  v2f OUT;
                  OUT.vertex = UnityObjectToClipPos(IN.vertex);
                  OUT.texcoord = IN.texcoord;
                  #ifdef PIXELSNAP_ON
                  OUT.vertex = UnityPixelSnap (OUT.vertex);
                  #endif
  
                  return OUT;
              }
  
              sampler2D _MainTex;
              float4 _ImageSize;
  
              fixed4 frag(v2f IN) : SV_Target
              {
                  fixed4 c = tex2D(_MainTex, IN.texcoord);
                  
                  if ( IN.texcoord.x > _Percent + _DamagesPercent )
                  {
                     c.a = 0 ;
                  }
                  else
                  {
                      if ( IN.texcoord.x > _Percent )
                         c *= _DamagesColor ;
                      else
                      {
                         if( (IN.texcoord.x * _ImageSize.x ) % (_ImageSize.x / _Steps) < _BorderWidth )
                             c *= _BorderColor;
                         else if ( IN.texcoord.y * _ImageSize.y < _BorderWidth )
                             c *= _BorderColor;
                         else
                             c *= _Color;
                      }
                  }
                      
                  
                  c.rgb *= c.a;
                  return c;
              }
          ENDCG
          }
      }
  }