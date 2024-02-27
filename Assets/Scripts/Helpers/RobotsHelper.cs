public class RobotsHelper
{
    public static string GetFirstWord(string text)
    {
        int index = text.IndexOf(' ');

        if (index > -1) { // Check if there is more than one word.
            return text.Substring(0, index).Trim(); // Extract first word.
        }
        return text; // Text is the first word itself.
    }

    public static string[] GetParameters(string text)
    {
        string[] parameters = text.Split(", ");

        return parameters;
    }
    
    public static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;
 
        return angle;
    }

    public static float[] WrapAngle(float[] angles)
    {
        for (int i = 0; i < angles.Length; i++)
        {
            angles[i] = WrapAngle(angles[i]);
        }

        return angles;
    }
}
