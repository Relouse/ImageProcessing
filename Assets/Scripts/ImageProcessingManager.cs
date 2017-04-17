using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageProcessingManager : MonoBehaviour {

    #region Variables
    private Dictionary<string, System.Delegate> avaibleProcessingMethods = new Dictionary<string, System.Delegate>();
    public Dictionary<string, System.Delegate> AvaibleProcessingMethods
    {
        get
        {
            return avaibleProcessingMethods;
        }
    }
    private delegate void ApplyEffect(Sprite sprite);
    public enum Effects
    {
        Original,
        Negative,
        Binarization,
        ShadesOfGray
    }
    private Effects _currentEffect;
    public Effects CurrentEffect
    {
        get
        {
            return _currentEffect;
        }
    }
    private Color _firstBinarizationColor, _secondBinarizationColor;
    public Color FirstBinarizationColor { get { return _firstBinarizationColor; } set { _firstBinarizationColor = value; } }
    public Color SecondBinarizationColor { get { return _secondBinarizationColor; } set { _secondBinarizationColor = value; } }
    #region Singleton
    private static ImageProcessingManager instance = null;
    public static ImageProcessingManager Instance
    {
        get
        {
            return instance;
        }
    }
    private float _binarizationBorder;
    
    #endregion
    #endregion
    #region Methods
    #region UnityMethods
    private void Awake()
    {
        #region Singleton implementation
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
        #endregion
    }
    private void Start()
    {
        avaibleProcessingMethods.Add("Original", new ApplyEffect(ApplyOriginal));
        avaibleProcessingMethods.Add("Negative", new ApplyEffect(ApplyNegative));
        avaibleProcessingMethods.Add("Binarization", new ApplyEffect(ApplyBinarization));
        avaibleProcessingMethods.Add("Shades of gray", new ApplyEffect(ApplyShadesOfGray));
        //avaibleProcessingMethods.Add("Brightness", new ApplyEffect(ApplyShadesOfGray));
        if (avaibleProcessingMethods != null)
            UIManager.Instance.FillImageProcessingMethods();

    }
    #endregion
    private void ApplyOriginal(Sprite sprite)
    {
        sprite.texture.SetPixels(UIManager.Instance.OriginalImageSpritePixels);
        sprite.texture.Apply();
        _currentEffect = Effects.Original;
    }
    
    private void ApplyNegative(Sprite sprite)
    {
        Color[] pixels = sprite.texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color(1 - pixels[i].r, 1 - pixels[i].g, 1 - pixels[i].b);
        }
        sprite.texture.SetPixels(pixels);
        sprite.texture.Apply();
        _currentEffect = Effects.Negative;
    }
    public void ChangeBinarizationBorderValue(float value)
    {
        _binarizationBorder = value;
        
    }
    
    public void ApplyBinarization(Sprite sprite)
    {
        Color[] pixels = sprite.texture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = GetColorBrightness(pixels[i]) > _binarizationBorder ? _firstBinarizationColor : _secondBinarizationColor;
        }
        sprite.texture.SetPixels(pixels);
        sprite.texture.Apply();
        _currentEffect = Effects.Binarization;
    }
    private float GetColorBrightness(Color color)
    {
        float temp = Mathf.Sqrt(color.r * color.r * 0.241f + color.g * color.g * 0.691f + color.b * color.b * 0.068f);
        return temp;
    }
    private void ApplyShadesOfGray(Sprite sprite)
    {
        Color[] pixels = sprite.texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            float tempFl = pixels[i].r * 0.3f + pixels[i].g * 0.59f + pixels[i].b * 0.11f;

            pixels[i] = new Color(tempFl, tempFl, tempFl);
        }
        sprite.texture.SetPixels(pixels);
        sprite.texture.Apply();
        _currentEffect = Effects.ShadesOfGray;
    }
    public void ApplyBrightness(Sprite sprite)
    {

    }
    #endregion
}
