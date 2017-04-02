using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour {

    #region Properties
    
    public Image OriginalImage, ExtraImage;
    [SerializeField] private Button saveButton, undoButton, redoButton, loadButton, binarizationApplyButton;
    [SerializeField] private Text spriteName,spriteSize;
    
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

        imageProcessingMethodsDropdown.onValueChanged.AddListener(ChooseEffect);
        saveButton.onClick.AddListener(SaveImage);
        undoButton.onClick.AddListener(ApplicationManager.Instance.Undo);
        redoButton.onClick.AddListener(ApplicationManager.Instance.Redo);
        binarizationApplyButton.onClick.AddListener(ApplyBinarizationButton);
        binarizationBorderSlider.onValueChanged.AddListener(ChangeBinarizationBorderSliderValueAndText);
        spriteName.text = "File: " + "<b><color=yellow>" + OriginalImage.sprite.name + ".png</color></b>";
        spriteSize.text = "Size: " + "<b><color=yellow>" + OriginalImage.sprite.texture.width + "x" + OriginalImage.sprite.texture.height + "</color></b>";
    }
    #endregion
    public void ChangeBinarizationBorderSliderValueAndText(float value)
    {
        ImageProcessingManager.Instance.ChangeBinarizationBorderValue(binarizationBorderSlider.value / binarizationBorderSlider.maxValue);
        binarizationBorderSlider.transform.FindChild("Handle Slide Area/Handle/Text").GetComponent<Text>().text = ((int)value).ToString();
    }
   private void ApplyBinarizationButton()
    {
        BinarizationWindowTools.SetActive(false);
        ImageProcessingManager.Instance.ApplyBinarization();
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
    private void ChooseEffect(int numb)
    {
        if (ApplicationManager.Instance.DoingRedoOrUndo) return;
        ImageProcessingManager.Instance.AvaibleProcessingMethods[imageProcessingMethodsDropdown.options[numb].text].DynamicInvoke(ExtraImage.sprite);
        ApplicationManager.Instance.SaveState(OriginalImage.sprite, ExtraImage.sprite, numb);
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
