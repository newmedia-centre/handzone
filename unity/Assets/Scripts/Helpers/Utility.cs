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

using System.Collections;
using System.Linq;
using Schema.Socket.Grasshopper;
using Schema.Socket.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3D = Schema.Socket.Unity.Vector3D;

#endregion

/// <summary>
/// The Utility class provides various static helper methods for common operations
/// related to transformations, rotations, and scene management in Unity.
/// This includes methods for rotating points around a pivot, transforming 
/// Unity's Transform to a SixDofPosition, and loading scenes asynchronously.
/// </summary>
public class Utility : MonoBehaviour
{
    /// <summary>
    /// Rotates a point around a pivot by specified angles.
    /// </summary>
    /// <param name="point">The point to rotate.</param>
    /// <param name="pivot">The pivot point around which to rotate.</param>
    /// <param name="angles">The rotation angles in degrees.</param>
    /// <returns>The rotated point as a Vector3.</returns>
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return RotatePointAroundPivot(point, pivot, Quaternion.Euler(angles));
    }

    /// <summary>
    /// Rotates a point around a pivot using a Quaternion rotation.
    /// </summary>
    /// <param name="point">The point to rotate.</param>
    /// <param name="pivot">The pivot point around which to rotate.</param>
    /// <param name="rotation">The rotation as a Quaternion.</param>
    /// <returns>The rotated point as a Vector3.</returns>
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        return rotation * (point - pivot) + pivot;
    }

    /// <summary>
    /// Transforms a Unity Transform into a SixDofPosition structure.
    /// </summary>
    /// <param name="transform">The Transform to convert.</param>
    /// <returns>A SixDofPosition representing the position and rotation.</returns>
    public static SixDofPosition TransformToSixDofPosition(Transform transform)
    {
        return new SixDofPosition
        {
            Position = new Vector3D
            {
                X = transform.position.x,
                Y = transform.position.y,
                Z = transform.position.z
            },
            Rotation = new Vector3D
            {
                X = transform.rotation.eulerAngles.x,
                Y = transform.rotation.eulerAngles.y,
                Z = transform.rotation.eulerAngles.z
            }
        };
    }

    /// <summary>
    /// Sets the position and rotation of a Unity Transform based on a SixDofPosition.
    /// </summary>
    /// <param name="sixDofPosition">The SixDofPosition containing the desired position and rotation.</param>
    /// <param name="transform">The Transform to update.</param>
    public static void SixDofPositionToTransform(SixDofPosition sixDofPosition, Transform transform)
    {
        transform.position = new Vector3
        {
            x = (float)sixDofPosition.Position.X,
            y = (float)sixDofPosition.Position.Y,
            z = (float)sixDofPosition.Position.Z
        };
        transform.eulerAngles = new Vector3
        {
            x = (float)sixDofPosition.Rotation.X,
            y = (float)sixDofPosition.Rotation.Y,
            z = (float)sixDofPosition.Rotation.Z
        };
    }

    public static Mesh CreateMeshFromGrasshopperMesh(MeshData ghMesh)
    {
        var mesh = new Mesh
        {
            vertices = ghMesh.Vertices.Select(v => new Vector3((float)v.X, (float)v.Y, (float)v.Z)).ToArray(),
            triangles = ghMesh.Tris.SelectMany(x => new[] { (int)x[0], (int)x[1], (int)x[2] }).ToArray()
        };

        return mesh;
    }

    /// <summary>
    /// Loads a scene asynchronously.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public static IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Start loading the scene
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        // Wait until the scene is fully loaded
        while (asyncLoad is { isDone: false }) yield return null;
    }
}