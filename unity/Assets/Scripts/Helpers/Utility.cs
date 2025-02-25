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