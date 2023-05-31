Shader "Custom/StencilBuffer"
{
	Properties
	{
		[IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
	}

	Subshader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"RenderPipeline" = "UniversalPipeline"
			"Queue" = "Geometry"
		}
	

		Pass
		{
			Blend Zero One
			Zwrite Off

			Stencil
			{
				Ref [_StencilID]
				Comp Always
				Pass Replace
				Fail Keep
			}
		}
	}
}