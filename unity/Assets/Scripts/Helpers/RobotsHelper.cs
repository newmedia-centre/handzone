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

/// <summary>
/// A utility class that provides helper methods for robot-related operations.
/// This class includes methods for extracting words from strings, 
/// splitting parameters, and wrapping angles to ensure they are within 
/// the range of -180 to 180 degrees.
/// </summary>
public class RobotsHelper
{
    /// <summary>
    /// Extracts the first word from a given text.
    /// If the text contains spaces, it returns the substring before the first space.
    /// Otherwise, it returns the entire text.
    /// </summary>
    /// <param name="text">The input string from which to extract the first word.</param>
    /// <returns>The first word as a string.</returns>
    public static string GetFirstWord(string text)
    {
        var index = text.IndexOf(' ');

        if (index > -1) // Check if there is more than one word.
            return text.Substring(0, index).Trim(); // Extract first word.
        return text; // Text is the first word itself.
    }

    /// <summary>
    /// Splits a given text into an array of parameters based on a comma and space delimiter.
    /// </summary>
    /// <param name="text">The input string to be split into parameters.</param>
    /// <returns>An array of strings representing the parameters.</returns>
    public static string[] GetParameters(string text)
    {
        var parameters = text.Split(", ");

        return parameters;
    }

    /// <summary>
    /// Wraps an angle to ensure it is within the range of -180 to 180 degrees.
    /// </summary>
    /// <param name="angle">The angle in degrees to be wrapped.</param>
    /// <returns>The wrapped angle in degrees.</returns>
    public static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

    /// <summary>
    /// Wraps an array of angles to ensure each angle is within the range of -180 to 180 degrees.
    /// </summary>
    /// <param name="angles">An array of angles in degrees to be wrapped.</param>
    /// <returns>The array of wrapped angles in degrees.</returns>
    public static float[] WrapAngle(float[] angles)
    {
        for (var i = 0; i < angles.Length; i++) angles[i] = WrapAngle(angles[i]);

        return angles;
    }
}