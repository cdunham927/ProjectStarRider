
    //Sam Gates

#define MAX_STEPS 48
#define SHADOW_STEPS 12
#define VOLUME_LENGTH 15.
#define SHADOW_LENGTH 2.
Shader "Unlit/Testing"
{
//FBM taken from XT95 https://www.shadertoy.com/view/lss3zr
    mat3 m = mat3(0.00, 0.80, 0.60,
        -0.80, 0.36, -0.48,
        -0.60, -0.48, 0.64);
    float hash(float n)
    {
        return fract(sin(n) * 43758.5453);
    }

    float noise(in vec3 x)
    {
        vec3 p = floor(x);
        vec3 f = fract(x);

        f = f * f * (3.0 - 2.0 * f);

        float n = p.x + p.y * 57.0 + 113.0 * p.z;

        float res = mix(mix(mix(hash(n + 0.0), hash(n + 1.0), f.x),
            mix(hash(n + 57.0), hash(n + 58.0), f.x), f.y),
            mix(mix(hash(n + 113.0), hash(n + 114.0), f.x),
                mix(hash(n + 170.0), hash(n + 171.0), f.x), f.y), f.z);
        return res;
    }

    float fbm(vec3 p)
    {
        float f;
        f = 0.5000 * noise(p); p = m * p * 2.02;
        f += 0.2500 * noise(p); p = m * p * 2.03;
        f += 0.1250 * noise(p); p = m * p * 2.01;
        f += 0.0625 * noise(p);
        return f;
    }

    //--------------------------------------------------

    float map(vec3 p) {

        float f = fbm(p - vec3(0, 0.5, 1.0) * iTime * .25);

        float sph = 1.0 - length(p * vec3(.5, 1, .5)) + f * 3.5;

        return min(max(0.0, sph), 1.0);

    }

    float jitter;

    vec4 cloudMarch(vec3 p, vec3 r) {

        float density = 0.0;
        float stepLen = VOLUME_LENGTH / float(MAX_STEPS);
        float shadowStepLength = SHADOW_LENGTH / float(SHADOW_STEPS);

        float cloudDensity = 20.0;
        vec3 cloudColor = vec3(1.0, .9, .8);

        vec3 light = vec3(1., 2., 1.);

        vec4 sum = vec4(vec3(0.), 1.);

        vec3 pos = p + r * jitter * stepLen;

        for (int i = 0; i < MAX_STEPS; i++) {

            if (sum.a < .1)break;

            float d = map(pos);

            if (d > 0.001) {

                vec3 lp = pos + light * jitter * shadowStepLength;
                float shadow = 0.;

                for (int x = 0; x < SHADOW_STEPS; x++) {

                    lp += light * shadowStepLength;
                    float lightSample = map(lp);
                    shadow += lightSample;


                }

                density = clamp((d / float(MAX_STEPS)) * cloudDensity, 0.0, 1.0);
                float s = exp((-shadow / float(SHADOW_STEPS)) * 3.);

                sum.rgb += vec3(s * density) * cloudColor * sum.a;

                sum.a *= 1. - density;
                sum.rgb += exp(-map(pos + vec3(0, .25, 0.0)) * .2) * density * vec3(.15, .45, 1.1) * sum.a;
            }
            pos += r * stepLen;

        }
        return sum;

    }

    mat3 camera(vec3 ro, vec3 ta, float cr) {
        vec3 cw = normalize(ta - ro);
        vec3 cp = vec3(sin(cr), cos(cr), 0.);
        vec3 cu = normalize(cross(cw, cp));
        vec3 cv = normalize(cross(cu, cw));
        return mat3(cu, cv, cw);
    }

    void mainImage(out vec4 fragColor, in vec2 fragCoord)
    {

        vec2 p = (fragCoord.xy * 2.0 - iResolution.xy) / min(iResolution.x, iResolution.y);
        jitter = hash(p.x + p.y * 57.0 + iTime);
        vec3 ro = vec3(sin(iTime * .5) * 10., -1, cos(iTime * .5) * 10.);
        vec3 ta = vec3(0, 1, 0);
        mat3 cam = camera(ro, ta, 0.0);
        vec3 ray = cam * normalize(vec3(p, 1.75));

        vec4 res = cloudMarch(ro, ray);
        res = pow(res, vec4(2.0 / 2.2));
        vec3 col = res.rgb + mix(vec3(0.), vec3(0., 0, .1), res.a);
        // Output to screen
        fragColor = vec4(col, 1.0);
    }
}
