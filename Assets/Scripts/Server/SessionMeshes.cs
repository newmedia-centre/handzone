using Schema.Socket.Grasshopper;
using Unity.VisualScripting;
using UnityEngine;

public class SessionMeshes : MonoBehaviour
{
    public MeshFilter[] meshReferences;
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
        
        meshRef1.mesh.Clear();
        meshRef2.mesh.Clear();
        meshRef3.mesh.Clear();
        meshRef4.mesh.Clear();
        meshRef5.mesh.Clear();
    }

    private void LoadGHMeshes(GrasshopperMeshesIn meshData)
    {
        // TODO: Fix arrays that not working when setting mesh references. So now we are setting them one by one.
        SetMesh(meshRef1.mesh, meshData.Mesh1);
        SetMesh(meshRef2.mesh, meshData.Mesh2);
        SetMesh(meshRef3.mesh, meshData.Mesh3);
        SetMesh(meshRef4.mesh, meshData.Mesh4);
        SetMesh(meshRef5.mesh, meshData.Mesh5);
    }

    /// <summary>
    /// Sets the mesh of the mesh filter to the incoming mesh data.
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="meshData"></param>
    private void SetMesh(Mesh mesh, MeshData meshData)
    {
        if (meshData == null)
        {
            mesh.Clear();
            return;
        }
        
        var newMesh = Utility.CreateMeshFromGrasshopperMesh(meshData);
        mesh.vertices = newMesh.vertices;
        mesh.triangles = newMesh.triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        // Set the mesh collider to the new mesh bounds
        // mesh.GetComponent<BoxCollider>().size = mesh.bounds.size;
        // mesh.GetComponent<BoxCollider>().center = mesh.bounds.center;
        
        // TODO: Set the box collider size of the Grasshopper Object
    }
}
