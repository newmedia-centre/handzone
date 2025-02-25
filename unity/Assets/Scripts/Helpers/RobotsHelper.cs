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