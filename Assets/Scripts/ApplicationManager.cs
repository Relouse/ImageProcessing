using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour {

    #region Properties
    #region Singleton
    private static ApplicationManager instance = null;
    public static ApplicationManager Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion
    private int currentState, maxStates;
    
    private struct State
    {
        public Color[] original, edited;
    }
    private List<State> states = new List<State>();
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
    
    #endregion

    public void SaveState(Sprite originalSprite, Sprite extraSprite)
    {
        
        states.Add(new State { original = originalSprite.texture.GetPixels(), edited = extraSprite.texture.GetPixels() });
        maxStates = states.Count;
        currentState = maxStates - 1;
    }
    public void Undo()
    {
        if (currentState > 0)
        {
            currentState--;
            SetSpritesByCurrentState();
        }
    }
    public void Redo()
    {
        if(currentState < maxStates - 1)
        {
            currentState++;
            SetSpritesByCurrentState();
        }
    }
    private void SetSpritesByCurrentState()
    {
        UIManager.Instance.originalImage.sprite.texture.SetPixels(states[currentState].original);
        UIManager.Instance.originalImage.sprite.texture.Apply();
        UIManager.Instance.extraImage.sprite.texture.SetPixels(states[currentState].edited);
        UIManager.Instance.extraImage.sprite.texture.Apply();
    }
    #endregion
}
