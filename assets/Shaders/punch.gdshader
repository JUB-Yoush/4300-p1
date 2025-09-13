shader_type canvas_item;
uniform bool isFlashing = false;
uniform float flashVal_r = 1.0;
uniform float flashVal_g = 1.0;
uniform float flashVal_b = 1.0;
void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec4 input_color = texture(TEXTURE, UV);
	if (isFlashing) {
		COLOR = vec4(1.0, 1.0, 1.0, input_color.a); //white
	} else {
	COLOR = vec4(input_color.r, input_color.g, input_color.b, input_color.a); //regular tone

	}





}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
