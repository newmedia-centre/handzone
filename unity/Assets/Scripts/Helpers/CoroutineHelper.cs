// Copyright 2024 NewMedia Centre - Delft University of Technology
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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