// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;


class Vignetting : PostEffectsBase {
	
	public float intensity = 0.375f;
	public float chromaticAberration = 0.2f;
	public float blur = 0.1f;
	public float blurSpread = 1.5f;
	
    // needed shaders & materials
	
	public Shader vignetteShader;
	public Material vignetteMaterial;
	
	public Shader separableBlurShader;
	public Material separableBlurMaterial;	
	
	public Shader chromAberrationShader;
	public Material chromAberrationMaterial;
	
	bool CheckResources (){	
		 //CheckSupport (false);	
	
		// vignetteMaterial = CheckShaderAndCreateMaterial (vignetteShader, vignetteMaterial);
		// separableBlurMaterial = CheckShaderAndCreateMaterial (separableBlurShader, separableBlurMaterial);
		// chromAberrationMaterial = CheckShaderAndCreateMaterial (chromAberrationShader, chromAberrationMaterial);
		
		// if(!isSupported)
		// 	ReportAutoDisable ();
		return true;// isSupported;		
	}
	
	void  OnRenderImage ( RenderTexture source ,   RenderTexture destination  ){	
		if(CheckResources()==false) {
			Graphics.Blit (source, destination);
			return;
		}	
				
		float widthOverHeight = (1.0f * source.width) / (1.0f * source.height);
		float oneOverBaseSize = 1.0f / 512.0f;				
		
		RenderTexture color = RenderTexture.GetTemporary (source.width, source.height, 0);	
		RenderTexture halfRezColor = RenderTexture.GetTemporary ((int)(source.width / 2.0f), (int) (source.height / 2.0f), 0);		
		RenderTexture quarterRezColor = RenderTexture.GetTemporary ((int)(source.width / 4.0f), (int) (source.height / 4.0f), 0);	
		RenderTexture secondQuarterRezColor = RenderTexture.GetTemporary ((int)(source.width / 4.0f), (int) (source.height / 4.0f), 0);	
				
		Graphics.Blit (source, halfRezColor, chromAberrationMaterial, 0);
		Graphics.Blit (halfRezColor, quarterRezColor);	
				
		for (int it = 0; it < 2; it++ ) {
			separableBlurMaterial.SetVector ("offsets", new Vector4 (0.0f, blurSpread * oneOverBaseSize, 0.0f, 0.0f));	
			Graphics.Blit (quarterRezColor, secondQuarterRezColor, separableBlurMaterial); 
			separableBlurMaterial.SetVector ("offsets", new Vector4 (blurSpread * oneOverBaseSize / widthOverHeight, 0.0f, 0.0f, 0.0f));	
			Graphics.Blit (secondQuarterRezColor, quarterRezColor, separableBlurMaterial);		
		}		
		
		vignetteMaterial.SetFloat ("_Intensity", intensity);
		vignetteMaterial.SetFloat ("_Blur", blur);
		vignetteMaterial.SetTexture ("_VignetteTex", quarterRezColor);
		Graphics.Blit (source, color, vignetteMaterial); 				
		
		chromAberrationMaterial.SetFloat ("_ChromaticAberration", chromaticAberration);
		Graphics.Blit (color, destination, chromAberrationMaterial, 1);	
		
		RenderTexture.ReleaseTemporary (color);
		RenderTexture.ReleaseTemporary (halfRezColor);			
		RenderTexture.ReleaseTemporary (quarterRezColor);	
		RenderTexture.ReleaseTemporary (secondQuarterRezColor);	
	
	}

}