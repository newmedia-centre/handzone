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

using System.Collections.Generic;
using Schema.Socket.Grasshopper;
using Unity.VisualScripting;
using UnityEngine;

#endregion

/// <summary>
/// The SessionMeshes class is responsible for managing and displaying
/// mesh data received from a Grasshopper session. It handles the
/// initialization of mesh filters, clearing existing meshes, and
/// loading new meshes into the scene based on data received from
/// the server. This class subscribes to events from the SessionClient
/// to update the mesh visuals in real-time.
/// </summary>
public class SessionMeshes : MonoBehaviour
{
    public List<MeshFilter> meshFilters = new();

    /// <summary>
    /// Initializes the SessionMeshes instance by subscribing to the
    /// OnGHMeshes event from the SessionClient and clearing existing meshes.
    /// This method is called when the script instance is being loaded.
    /// </summary>
    private void Start()
    {
        if (SessionClient.Instance)
            SessionClient.Instance.OnGHMeshes += LoadGHMeshes;

        ClearMeshes();
    }

    /// <summary>
    /// Clears the meshes from all mesh filters in the list.
    /// This method removes the current mesh data and destroys any
    /// associated colliders for each mesh filter.
    /// </summary>
    private void ClearMeshes()
    {
        foreach (var meshFilter in meshFilters)
        {
            meshFilter.mesh.Clear();

            var _collider = meshFilter.GetComponent<Collider>();
            if (_collider) Destroy(_collider);
        }
    }

    /// <summary>
    /// Loads new Grasshopper mesh data into the specified mesh filters.
    /// This method is called when new mesh data is received from the server.
    /// It sets the mesh for each mesh filter based on the provided mesh data.
    /// </summary>
    /// <param name="meshData">The Grasshopper mesh data containing the new meshes.</param>
    private void LoadGHMeshes(GrasshopperMeshesIn meshData)
    {
        SetMesh(meshFilters[0], meshData.Mesh1);
        SetMesh(meshFilters[1], meshData.Mesh2);
        SetMesh(meshFilters[2], meshData.Mesh3);
        SetMesh(meshFilters[3], meshData.Mesh4);
        SetMesh(meshFilters[4], meshData.Mesh5);
    }

    /// <summary>
    /// Sets the mesh for a specific mesh filter based on the provided mesh data.
    /// This method updates the mesh filter's mesh with new vertices and triangles,
    /// recalculates normals and bounds, and updates the associated box collider.
    /// </summary>
    /// <param name="meshFilter">The mesh filter to update.</param>
    /// <param name="meshData">The mesh data to set for the mesh filter.</param>
    private void SetMesh(MeshFilter meshFilter, MeshData meshData)
    {
        if (meshData == null)
        {
            meshFilter.mesh.Clear();
            return;
        }

        var newMesh = Utility.CreateMeshFromGrasshopperMesh(meshData);
        meshFilter.mesh.Clear();
        meshFilter.mesh.vertices = newMesh.vertices;
        meshFilter.mesh.triangles = newMesh.triangles;
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateBounds();

        var boxCollider = meshFilter.GetOrAddComponent<BoxCollider>();
        if (boxCollider)
        {
            boxCollider.size = meshFilter.mesh.bounds.size;
            boxCollider.center = meshFilter.mesh.bounds.center;
        }
    }
}