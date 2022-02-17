using DG.Tweening;
using UnityEngine;

using UnityGraphics = UnityEngine.Graphics;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("MicroCrew/FastPostProcessing")]
public sealed class FastPostProcessing : MonoBehaviour {
    public enum ToneMapper {
        None = 0, ACES, Dawson, Hable, Photographic, Reinhart
    }

    private Material m_PostProcessMaterial = null;
    private Vector4 m_UserLutParams;
    private bool m_UserLutEnabled = true;
    private Camera cam;

    [Header("MyStuff")]
    public Camera shadowCam;

    [Header("Material")]
    [SerializeField]
    private Shader m_Shader = null;

    [Header("Sharpen")]
    [SerializeField]
    private bool m_Sharpen = true;
    [Range(0.1f, 4.0f)]
    [SerializeField]
    private float m_SharpenIntensity = 2.0f;
    [Range(0.00005f, 0.0008f)]
    [SerializeField]
    private float m_SharpenSize = 2.0f;

    [Header("Bloom")]
    [SerializeField]
    private bool m_Bloom = true;
    [Range(0.01f, 2048)]
    [SerializeField]
    private float m_BloomSize = 512;
    [Range(0.00f, 3.0f)]
    [SerializeField]
    private float m_BloomAmount = 1.0f;
    [Range(0.0f, 3.0f)]
    [SerializeField]
    private float m_BloomPower = 1.0f;

    [Header("ToneMapper")]
    [SerializeField]
    private ToneMapper m_ToneMapper = ToneMapper.ACES;
    [SerializeField]
    private float m_Exposure = 1.0f;
    [SerializeField]
    private Texture2D m_UserLutTexture = null;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_LutContribution = 0.5f;
    [SerializeField]
    private bool m_Dithering = false;

    [Header("Gamma Correction")]
    [SerializeField]
    private bool m_GammaCorrectionEnabled = false;

    [Header("Vignette")]
    [SerializeField]
    private bool m_Vignette = false;

    [Header("Shadows")]
    public RenderTexture shadowTexture;
    [Header("Grayscale")]
    [SerializeField] private float grayscaleValue;

    #region Properties

    public bool Sharpen {
        get => m_Sharpen;
        set {
            m_Sharpen = value;
            //SetDefine("SHARPEN", m_Sharpen);
        }
    }

    public bool Bloom {
        get => m_Bloom;
        set {
            m_Bloom = value;
            //SetDefine("BLOOM", m_Bloom);
        }
    }

    public ToneMapper Tonemapper {
        get => m_ToneMapper;
        set {
            m_ToneMapper = value;
            UpdateTonemapperDefines();
        }
    }

    public Texture2D UserLUTTexture {
        get => m_UserLutTexture;
        set {
            m_UserLutTexture = value;
            m_UserLutEnabled = m_UserLutTexture != null;

            if (m_UserLutEnabled)
                m_UserLutParams = new Vector4(1f / m_UserLutTexture.width, 1f / m_UserLutTexture.height, m_UserLutTexture.height - 1f, m_LutContribution);

            //SetDefine("USERLUT_TEXTURE", m_UserLutEnabled);
        }
    }

    public bool Dithering {
        get => m_Dithering;
        set {
            m_Dithering = value;
            //SetDefine("DITHERING", m_Dithering);
        }
    }

    public bool GammaCorrection {
        get => m_GammaCorrectionEnabled;
        set {
            m_GammaCorrectionEnabled = value;
            //SetDefine("GAMMA_CORRECTION", m_GammaCorrectionEnabled);
        }
    }

    public bool Vignette {
        get => m_Vignette;
        set {
            m_Vignette = value;
            //SetDefine("VIGNETTE", m_Vignette);
        }
    }

    public float UserLUTContribution {
        get => m_UserLutParams.z;
        set => m_UserLutParams.z = value;
    }

    public Material Material {
        get => m_PostProcessMaterial;
        set {
            m_PostProcessMaterial = value;
            OnEnable();
        }
    }

    public Shader Shader {
        get => m_Shader;
        set {
            if (value == null) {
                Debug.Log("Trying to put a null shader.");
                return;
            }

            m_Shader = value;
            m_PostProcessMaterial = new Material(value);
        }
    }

    #endregion

    private int vignetteColorPropID = -1;

    private void Awake() {
        shadowTexture = new RenderTexture(Screen.width / 4, Screen.height / 4, 0, RenderTextureFormat.ARGB32, 0);
        shadowCam.targetTexture = shadowTexture;
    }

    private void OnEnable() {
        cam = GetComponent<Camera>();

        if (m_Shader == null) {
            m_Shader = Shader.Find("Demonixis/FastPostProcessing");
            vignetteColorPropID = m_Shader.FindPropertyIndex("_VignetteColor");
        }

        if (m_PostProcessMaterial == null)
            m_PostProcessMaterial = new Material(m_Shader);

        m_PostProcessMaterial.SetTexture("_ShadowTex", shadowTexture);                

        m_UserLutEnabled = m_UserLutTexture != null;

        if (m_UserLutEnabled)
            m_UserLutParams = new Vector4(1.0f / m_UserLutTexture.width, 1.0f / m_UserLutTexture.height, m_UserLutTexture.height - 1.0f, m_LutContribution);

        //SetDefine("SHARPEN", m_Sharpen);
        //SetDefine("BLOOM", m_Bloom);
        //SetDefine("DITHERING", m_Dithering);
        //SetDefine("USERLUT_TEXTURE", m_UserLutEnabled && m_UserLutTexture != null);
        //SetDefine("GAMMA_CORRECTION", m_GammaCorrectionEnabled);
        //SetDefine("VIGNETTE", m_Vignette);

        UpdateTonemapperDefines();
    }

    private void OnDestroy() {
        shadowTexture.Release();
    }

    private void UpdateTonemapperDefines() {
        //SetDefine("TONEMAPPER_ACES", m_ToneMapper == ToneMapper.ACES);
        //SetDefine("TONEMAPPER_HABLE", m_ToneMapper == ToneMapper.Hable);
        //SetDefine("TONEMAPPER_PHOTOGRAPHIC", m_ToneMapper == ToneMapper.Photographic);
        //SetDefine("TONEMAPPER_DAWSON", m_ToneMapper == ToneMapper.Dawson);
        //SetDefine("TONEMAPPER_REINHART", m_ToneMapper == ToneMapper.Reinhart);
    }

    private void SetDefine(string define, bool isEnabled) {
        if (m_PostProcessMaterial == null) {
            Debug.LogWarning("Post Process Material is null");
            return;
        }

        if (isEnabled)
            m_PostProcessMaterial.EnableKeyword(define);
        else
            m_PostProcessMaterial.DisableKeyword(define);
    }

    private Tween grayscaleTween;
    private bool grayscaleEnabled = false;

    public void EnableGraysacle(bool enable, bool instant) {
        if(grayscaleTween != null) {
            grayscaleTween.Kill(true);
        }

        const float grayscaleEnabledValue = 0.8f;

        if(grayscaleEnabled) {
            if(!enable) {
                if (instant) {
                    grayscaleValue = 0f;
                } else {
                    grayscaleTween = DOTween.To(() => grayscaleValue, x => grayscaleValue = x, 0f, 0.5f).OnComplete(() => grayscaleTween = null);
                }
            }
        } else {
            if(enable) {
                if (instant) {
                    grayscaleValue = grayscaleEnabledValue;
                } else {
                    grayscaleTween = DOTween.To(() => grayscaleValue, x => grayscaleValue = x, grayscaleEnabledValue, 0.5f).OnComplete(() => grayscaleTween = null);
                }
            }
        }

        grayscaleEnabled = enable;
    }

    public void SetVignetteColor(Color color) {
        if(vignetteColorPropID == -1) {
            vignetteColorPropID = Shader.FindPropertyIndex("_VignetteColor");
        }


        //[OPTIMIZE]
        m_PostProcessMaterial.SetColor("_VignetteColor", color);
        //m_PostProcessMaterial.SetColor(vignetteColorPropID, color);
    }

#if UNITY_EDITOR
    private void OnValidate() => OnEnable();
#endif

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (m_PostProcessMaterial == null || m_PostProcessMaterial?.shader == null) {
            Debug.LogWarning("Material Or Shader NULL");
            UnityGraphics.Blit(source, destination);
            return;
        }

        if (m_Sharpen) {
            m_PostProcessMaterial.SetFloat("_SharpenSize", m_SharpenSize);
            m_PostProcessMaterial.SetFloat("_SharpenIntensity", m_SharpenIntensity);
        }

        if (m_Bloom) {
            m_PostProcessMaterial.SetFloat("_BloomSize", m_BloomSize);
            m_PostProcessMaterial.SetFloat("_BloomAmount", m_BloomAmount);
            m_PostProcessMaterial.SetFloat("_BloomPower", m_BloomPower);
        }

        if (m_ToneMapper != ToneMapper.None)
            m_PostProcessMaterial.SetFloat("_Exposure", m_Exposure);

        if (m_UserLutEnabled) {
            m_PostProcessMaterial.SetVector("_UserLutParams", m_UserLutParams);
            m_PostProcessMaterial.SetTexture("_UserLutTex", m_UserLutTexture);
        }

        m_PostProcessMaterial.SetMatrix("_CameraToWorldMatrix", (cam.projectionMatrix * cam.worldToCameraMatrix).inverse);
        m_PostProcessMaterial.SetFloat("_GrayscaleValue", grayscaleValue);

        UnityGraphics.Blit(source, destination, m_PostProcessMaterial);
    }
}