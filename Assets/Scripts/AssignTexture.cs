using UnityEngine;
using UnityEngine.InputSystem;

public class AssignTexture : MonoBehaviour
{
    public InputActionReference inputActionReference;
    public ComputeShader shader;
    public int texResolution = 256;
    
    private Renderer _renderer;
    private RenderTexture _outputTexture;
    private int _kernelHandle;
    
    private static readonly int Result = Shader.PropertyToID("Result");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    private void OnEnable()
    {
        inputActionReference.action.performed += OnActionPerformed;
        inputActionReference.action.Enable();
    }

    private void OnDisable()
    {
        inputActionReference.action.performed -= OnActionPerformed;
        inputActionReference.action.Disable();
    }

    private void OnActionPerformed(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            DispatchShader(texResolution/8, texResolution/8);
        }
    }

    private void Start()
    {
        _outputTexture = new RenderTexture(texResolution, texResolution, 0)
        {
            enableRandomWrite = true
        };
        _outputTexture.Create();

        _renderer = GetComponent<Renderer>();
        _renderer.enabled = true;
        InitShader();
    }

    private void InitShader()
    {
        _kernelHandle = shader.FindKernel("CSMain");
        shader.SetTexture(_kernelHandle, Result, _outputTexture);
        _renderer.material.SetTexture(MainTex, _outputTexture);
        
        DispatchShader(texResolution/16, texResolution/16);
    }

    private void DispatchShader(int x, int y)
    {
        shader.Dispatch(_kernelHandle, x, y, 1); 
    }
    
    
}
