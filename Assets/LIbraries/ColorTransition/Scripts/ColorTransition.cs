using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Camera))]
public class ColorTransition : MonoBehaviour
{
    public static ColorTransition Instance;
    
    public bool ShouldStart;
    
    private float _value;

//    private Material _transitionMaterial;
    public Material TransitionMaterial;
//    {
//        get
//        {
//            return _transitionMaterial ?? (_transitionMaterial = new Material(Shader.Find("Custom/ColorTransition")));
//        }
//    }
    
    public Texture TransitionTexture;
    public Color TransitionColor = Color.black;

    public float Speed = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!ShouldStart)
        {
            TransitionMaterial.SetFloat("_Cutoff", 0);
            return;
        }

        TransitionMaterial.SetTexture("_TransitionTex", TransitionTexture);
        
        TransitionMaterial.SetFloat("_Cutoff", 0.99f);
        TransitionMaterial.SetColor("_Color", TransitionColor);
        _value = 1f;
        StartCoroutine(FadeInTransition());
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, TransitionMaterial);
    }

    public void Transition(string nextScene)
    {
        _value = 0;
        TransitionMaterial.SetTexture("_TransitionTex", TransitionTexture);
        StartCoroutine(FadeOutTransition(nextScene));
    }
    
    IEnumerator FadeOutTransition(string nextScene)
    {
        while(_value < 1f)
        {
            _value += Time.deltaTime * Speed;
            TransitionMaterial.SetFloat("_Cutoff", _value);
            if (_value > 0.99f)
            {
                yield return new WaitForSeconds(0.2f);
                FinishedTransition(nextScene);
            }
            
            yield return new WaitForEndOfFrame();
        }        
        yield return null;
    }
    
    void FinishedTransition(string nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }
    
    IEnumerator FadeInTransition()
    {
        while(_value > 0.0f)
        {
            _value -= Time.deltaTime * Speed;
            TransitionMaterial.SetFloat("_Cutoff", _value);

            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
