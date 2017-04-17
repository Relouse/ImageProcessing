using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour {

    #region Properties
    
    public Image OriginalImage, ExtraImage;
    [SerializeField] private Button saveButton, undoButton, redoButton, loadButton, ApplyEffectButton;
    [SerializeField] private Text spriteName,spriteSize, coordinatesText;
    
    [SerializeField] private Slider binarizationBorderSlider;

    [SerializeField]
    private GameObject _binarizationWindowTools;
    public GameObject BinarizationWindowTools
    {
        get
        {
            return _binarizationWindowTools;
        }
    }
    private Color[] originalImagePixels;
    public Color[] OriginalImageSpritePixels
    {
        get
        {
            return originalImagePixels;
        }
    }
    
    [SerializeField]
    private Dropdown imageProcessingMethodsDropdown;

    [SerializeField]
    private ColorPicker binarizationColor1, binarizationColor2;
    #region Singleton
    private static UIManager _instance = null;
    public static UIManager Instance
    {
        get
        {
            return _instance;
        }
    }
    #endregion
    #endregion
    #region Methods
    #region UnityMethods
    private void Awake()
    {
        #region Singleton implementation
        if (_instance == null)
        {
            _instance = this;
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
        originalImagePixels = OriginalImage.sprite.texture.GetPixels();
        Texture2D temp = new Texture2D(OriginalImage.sprite.texture.width, OriginalImage.sprite.texture.height);
        temp.SetPixels(originalImagePixels);
        temp.Apply();

        ExtraImage.sprite = Sprite.Create(temp, new Rect(0, 0, temp.width, temp.height), new Vector2(0.5f, 0.5f));
        ExtraImage.preserveAspect = true;

        ChooseEffect(0);
        ApplyEffect();

        imageProcessingMethodsDropdown.onValueChanged.AddListener(ChooseEffect);
        saveButton.onClick.AddListener(SaveImage);
        undoButton.onClick.AddListener(ApplicationManager.Instance.Undo);
        redoButton.onClick.AddListener(ApplicationManager.Instance.Redo);
        ApplyEffectButton.onClick.AddListener(ApplyEffect);
        binarizationColor1.onValueChanged.AddListener(ChangeFirstBinarizationColor);
        binarizationColor2.onValueChanged.AddListener(ChangeSecondBinarizationColor);
        
        binarizationBorderSlider.onValueChanged.AddListener(ChangeBinarizationBorderSliderValueAndText);
        spriteName.text = "File: " + "<b><color=yellow>" + OriginalImage.sprite.name + ".png</color></b>";
        spriteSize.text = "Size: " + "<b><color=yellow>" + OriginalImage.sprite.texture.width + "x" + OriginalImage.sprite.texture.height + "</color></b>";
        coordinatesText.text = "Coordinates: " + "<b><color=yellow>0:0</color></b>";

    }
    #endregion
    public void PrintImageCoordinates(int x, int y)
    {
        coordinatesText.text = "Coordinates: " + "<b><color=yellow>" + x + ":" + y + "</color></b>";
    }
    public void ChangeFistBinarizationColorValue(Color color)
    {
        binarizationColor1.CurrentColor = color;
    }
    public void ChangeSecondBinarizationColorValue(Color color)
    {
        binarizationColor2.CurrentColor = color;
    }
    public void ChangeBinarizationBorderSliderValueAndText(float value)
    {
        ImageProcessingManager.Instance.ChangeBinarizationBorderValue(binarizationBorderSlider.value / binarizationBorderSlider.maxValue);
        binarizationBorderSlider.transform.FindChild("Handle Slide Area/Handle/Text").GetComponent<Text>().text = ((int)value).ToString();
    }
    private void ChangeFirstBinarizationColor(Color color)
    {
        ImageProcessingManager.Instance.FirstBinarizationColor = color;
    }
    private void ChangeSecondBinarizationColor(Color color)
    {
        ImageProcessingManager.Instance.SecondBinarizationColor = color;
    }
    private void ApplyEffect()
    {
        ImageProcessingManager.Instance.AvaibleProcessingMethods["Original"].DynamicInvoke(ExtraImage.sprite);
        ImageProcessingManager.Instance.AvaibleProcessingMethods[imageProcessingMethodsDropdown.options[imageProcessingMethodsDropdown.value].text].DynamicInvoke(ExtraImage.sprite);
        ApplicationManager.Instance.SaveState(OriginalImage.sprite, ExtraImage.sprite, imageProcessingMethodsDropdown.value, binarizationBorderSlider.value, binarizationColor1.CurrentColor, binarizationColor2.CurrentColor);
        //DisableEffectsToolsWindows();
    }
    public void FillImageProcessingMethods()
    {
        imageProcessingMethodsDropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (var item in ImageProcessingManager.Instance.AvaibleProcessingMethods)
        {
            options.Add(new Dropdown.OptionData { text = item.Key });
        }
        imageProcessingMethodsDropdown.AddOptions(options);
    }
    private void DisableEffectsToolsWindows()
    {
        BinarizationWindowTools.SetActive(false);
    }
    private void ChooseEffect(int numb)
    {
        DisableEffectsToolsWindows();
        switch ((ImageProcessingManager.Effects)numb)
        {
            case ImageProcessingManager.Effects.Original:
                break;
            case ImageProcessingManager.Effects.Negative:
                break;
            case ImageProcessingManager.Effects.Binarization:
                BinarizationWindowTools.SetActive(true);
                break;
            case ImageProcessingManager.Effects.ShadesOfGray:
                break;
        }
    }
    private void SaveImage()
    {
        byte[] bytes = ExtraImage.sprite.texture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Image.png", bytes);
    }
    public void SetEffectsDropdownCurrentValue(int i)
    {
        imageProcessingMethodsDropdown.value = i;
    }
    public void SetBinarizationSliderValue(float value)
    {
        binarizationBorderSlider.value = value;
    }
    public void SetUndoButtonInteractable(bool interactable)
    {
        undoButton.interactable = interactable;
    }
    public void SetRedoButtonInteractable(bool interactable)
    {
        redoButton.interactable = interactable;
    }
    #endregion
}
