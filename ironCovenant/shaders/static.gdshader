shader_type canvas_item;

uniform float amount = 40.0;
uniform float intensity = 1;

void fragment() {
	vec2 uv = UV*0.05;
	float a = fract(sin(dot(UV, vec2(12.9898, 78.233) * TIME)) * 438.5453) * intensity;
	vec4 col = texture(TEXTURE, UV);
	col.a *= pow(a, amount);
	COLOR.a = col.a;
}