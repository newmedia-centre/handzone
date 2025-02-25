/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

#region

using UnityEngine;

#endregion

public class CoroutineHelper : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the CoroutineHelper class.
    /// This class is responsible for managing coroutines in a way that allows
    /// for easy access throughout the application.
    /// </summary>
    private static CoroutineHelper _instance;

    /// <summary>
    /// Gets the singleton instance of the CoroutineHelper.
    /// If the instance does not exist, it creates a new GameObject
    /// and attaches the CoroutineHelper component to it.
    /// </summary>
    public static CoroutineHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CoroutineHelper>();
                if (_instance == null)
                {
                    var go = new GameObject("CoroutineHelper");
                    _instance = go.AddComponent<CoroutineHelper>();
                    DontDestroyOnLoad(go);
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// It ensures that there is only one instance of CoroutineHelper
    /// and that it persists across scene loads.
    /// </summary>
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}