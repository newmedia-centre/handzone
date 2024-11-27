using Schema.Socket.Grasshopper;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class SessionMeshes : MonoBehaviour
{
    public List<MeshFilter> meshFilters = new();

    private void Start()
    {
        if (SessionClient.Instance)
            SessionClient.Instance.OnGHMeshes += LoadGHMeshes;

        ClearMeshes();
    }

    private void ClearMeshes()
    {
        foreach (var meshFilter in meshFilters)
        {
            meshFilter.mesh.Clear();
            
            var _collider = meshFilter.GetComponent<Collider>();
            if (_collider)
            {
                Destroy(_collider);
            }
        }
    }

    private void LoadGHMeshes(GrasshopperMeshesIn meshData)
    {
        SetMesh(meshFilters[0], meshData.Mesh1);
        SetMesh(meshFilters[1], meshData.Mesh2);
        SetMesh(meshFilters[2], meshData.Mesh3);
        SetMesh(meshFilters[3], meshData.Mesh4);
        SetMesh(meshFilters[4], meshData.Mesh5);
    }

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
