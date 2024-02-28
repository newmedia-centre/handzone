using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Robots;
using UnityEngine;
using Plane = Rhino.Geometry.Plane;

public class GHProgram
{
    public static async Task<Program> CreateAsync(List<IToolpath> toolpaths)
    {
        var robot = await GetRobotAsync();

        try
        {
            // TODO: implement toolpath conversion. Seems to only work with only one toolpath.
            return new Program("TestProgram", robot, new [] { toolpaths[0] });
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            return null;
        }
    }

    static async Task<RobotSystem> GetRobotAsync()
    {
        var name = "TUDelft-LAMA-UR5";

        try
        {
            return FileIO.LoadRobotSystem(name, Plane.WorldXY);
        }
        catch (ArgumentException e)
        {
            if (!e.Message.Contains("not found"))
                throw;

            UnityEngine.Debug.Log("TUDelft-LAMA-UR5 robot library not found, installing...");
            await DownloadLibraryAsync();
            return FileIO.LoadRobotSystem(name, Plane.WorldXY);
        }
    }

    static async Task DownloadLibraryAsync()
    {
        var online = new OnlineLibrary();
        await online.UpdateLibraryAsync();
        var bartlett = online.Libraries["TUDelft"];
        await online.DownloadLibraryAsync(bartlett);
    }
}