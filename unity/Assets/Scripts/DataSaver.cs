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