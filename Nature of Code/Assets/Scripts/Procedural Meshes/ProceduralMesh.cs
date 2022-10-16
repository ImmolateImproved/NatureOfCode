using ProceduralMeshes;
using ProceduralMeshes.Generators;
using ProceduralMeshes.Streams;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour
{
    public enum MeshType
    {
        SquareGrid, SharedSquareGrid
    };

    private Mesh mesh;

    private MeshJobScheduleDelegate[] jobs;

    public MeshType meshType;

    [Range(1, 50)]
    public int resolution = 1;

    private void Awake()
    {
        InitMeshJobs();
        InitMesh();
    }

    private void Update()
    {
        GenerateMesh();
        enabled = false;
    }

    private void OnValidate()
    {
        enabled = true;
    }

    private void InitMeshJobs()
    {
        jobs = new MeshJobScheduleDelegate[]
        {
            MeshJob<SquareGrid, SingleStream>.ScheduleParallel,
            MeshJob<SharedSquareGrid, SingleStream>.ScheduleParallel
        };
    }

    private void InitMesh()
    {
        mesh = new Mesh
        {
            name = "Procedural Mesh"
        };

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void GenerateMesh()
    {
        var meshDataArray = Mesh.AllocateWritableMeshData(1);
        var meshData = meshDataArray[0];

        jobs[(int)meshType](mesh, meshData, resolution, default).Complete();

        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
    }
}