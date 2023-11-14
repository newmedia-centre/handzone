using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Robots;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Plane = Rhino.Geometry.Plane;

public class GrasshopperSyncProgram
{
    public static async Task<Program> CreateAsync(string json)
    {
        var robot = await GetRobotAsync();

        var toolpaths = GetToolpathsAsync(json);

        return new Program("GrasshopperSyncProgram", robot, toolpaths);
    }
    
    static async Task<RobotSystem> GetRobotAsync()
    {
        var cellName = "TUDelft-LAMA-UR5";

        try
        {
            return FileIO.LoadRobotSystem(cellName, Plane.WorldXY);
        }
        catch (ArgumentException e)
        {
            if (!e.Message.Contains("not found"))
                throw;

            UnityEngine.Debug.Log("TUDelft-LAMA-UR5 robot library not found, installing...");
            await DownloadLibraryAsync();
            return FileIO.LoadRobotSystem(cellName, Plane.WorldXY);
        }
    }

    static List<IToolpath> GetToolpathsAsync(string json)
    {
        List<IToolpath> toolpaths = new List<IToolpath>();
        var toolpathsJson = JObject.Parse(json).SelectToken("Toolpaths");
        var serializer = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Include,
            FloatParseHandling = FloatParseHandling.Double,
            TypeNameHandling = TypeNameHandling.Auto,
            // TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
        };

        
        if (toolpathsJson != null)
        {
            toolpaths = JsonConvert.DeserializeObject<List<IToolpath>>(toolpathsJson.ToString(), serializer);
        }
        return toolpaths;
    }

    static async Task DownloadLibraryAsync()
    {
        var online = new OnlineLibrary();
        await online.UpdateLibraryAsync();
        var bartlett = online.Libraries["TUDelft"];
        await online.DownloadLibraryAsync(bartlett);
    }
}
