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

using System.IO;
using Newtonsoft.Json;
using UnityEngine;

#endregion

public class DataSaver : MonoBehaviour
{
    /**
     * Call this function with a / in front, otherwise you put data in the a directory above the actual one.
     */
    public static void TrySaveData<T>(string filePath, T data)
    {
        Debug.Assert(filePath.ToCharArray()[0] == '/',
            "The filePath: " + filePath +
            " did not contain a / at the beginning. You should have a / at the beginning of your filepath.");

        var _path = Application.persistentDataPath + filePath;
        var _dir = Path.GetDirectoryName(_path);

        Debug.Log("Saving at: " + _path);

        if (string.IsNullOrEmpty(_dir))
        {
            Debug.LogError("Could not set the correct directory path. Check path name");
            return;
        }

        if (!Directory.Exists(_dir)) Directory.CreateDirectory(_dir);

        if (!File.Exists(_path))
        {
            var _stream = File.Create(_path);
            _stream.Close();
        }

        File.WriteAllText(_path, JsonConvert.SerializeObject(data));
    }

    public static T? TryLoadData<T>(string filePath) where T : struct
    {
        Debug.Assert(filePath.ToCharArray()[0] == '/',
            "The filePath: " + filePath +
            " did not contain a / at the beginning. You should have a / at the beginning of your filepath.");

        var _path = Application.persistentDataPath + filePath;

        Debug.Log("Loading from: " + _path);

        if (File.Exists(_path)) return JsonConvert.DeserializeObject<T>(File.ReadAllText(_path));

        return null;
    }

    /*
     * Test code for this class can be found here
     * This has been disable for now, since it does not have any use for now
     */

    // public static void TestCode()
    // {
    //     //Good Case
    //     TrySaveData("/Test", 1);
    //     Assert.AreEqual(1, TryLoadData<int>("/Test"));
    //     
    //     //File not found
    //     Assert.AreEqual(null, TryLoadData<TestStruct>("/NotExist/Path"));
    //     
    //     //No slash at the beginning
    //     TrySaveData("Test", 1);
    //     TryLoadData<int>("Test");
    //
    //     TestStruct _data = new TestStruct(1, 5.55f, "TestName");
    //     TrySaveData<TestStruct>("/folder/data", _data);
    //     TestStruct? _readData = TryLoadData<TestStruct>("/folder/data");
    //     Assert.AreEqual(_data, _readData);
    //
    //     String persistantPath = Application.persistentDataPath;
    //     
    //     File.Delete(persistantPath + "/Test");
    //     File.Delete(persistantPath + "Test");
    //     File.Delete(persistantPath + "/folder/data");
    //     Directory.Delete(persistantPath + "/folder");
    // }
    //
    // public struct TestStruct
    // {
    //     public int number1;
    //     public float number2;
    //     public String name;
    //
    //     public TestStruct(int number1, float number2, String name)
    //     {
    //         this.number1 = number1;
    //         this.number2 = number2;
    //         this.name = name;
    //     }
    // }
}