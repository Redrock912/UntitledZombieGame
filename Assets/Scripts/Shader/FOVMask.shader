Shader "Custom/FOVMask"
{
	SubShader{
		ZWrite off
		ColorMask 0
		Cull off

		Stencil {
			Ref 1
			Comp always
			Pass replace
		}
		Pass {

		}
	}
}