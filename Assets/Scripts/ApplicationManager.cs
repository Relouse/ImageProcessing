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
    #endregion
}
