using Schema.Socket.Grasshopper;
using UnityEngine;

public class SessionMeshes : MonoBehaviour
{
    public MeshFilter[] meshReferences;
    // TODO: Fix arrays that not working when setting mesh references.
    public MeshFilter meshRef1;
    public MeshFilter meshRef2;
    public MeshFilter meshRef3;
    public MeshFilter meshRef4;
    public MeshFilter meshRef5;
    
    // Start is called before the first frame update
    private void Start()
    {
        if (SessionClient.Instance)
          SessionClient.Instance.OnGHMeshes += LoadGHMeshes;
    }

    private void LoadGHMeshes(GrasshopperMeshesIn meshData)
    {
        SetMesh(meshRef1.mesh, meshData.Mesh1);
        SetMesh(meshRef2.mesh, meshData.Mesh2);
        SetMesh(meshRef3.mesh, meshData.Mesh3);
        SetMesh(meshRef4.mesh, meshData.Mesh4);
        SetMesh(meshRef5.mesh, meshData.Mesh5);
    }

    private void SetMesh(Mesh mesh, MeshData meshData)
    {
        var newMesh = Utility.CreateMeshFromGrasshopperMesh(meshData);
        mesh.vertices = newMesh.vertices;
        mesh.triangles = newMesh.triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
