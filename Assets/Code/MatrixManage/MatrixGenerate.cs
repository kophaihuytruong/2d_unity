using System;
using System.IO;
using UnityEngine;

public class MatrixGenerate : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject groundSquarePrefab;
    public GameObject disconnectPipeRedPrefab;
    public GameObject disconnectPipeOrangePrefab;
    public GameObject disconnectPipeYellowPrefab;
    public GameObject disconnectPipeBluePrefab;
    public GameObject disconnectPipeGreenPrefab;
    public GameObject disconnectPipePurplePrefab;

    [Header("Map Settings")]
    public string relativeFilePath = "map_txt/easy/map1_easy.txt";
    public float spacing = 0.0f;

    private Transform matrixParent;

    void Start()
    {
        GenerateMatrixFromMapFile();
    }

    /// <summary>
    /// Đọc dữ liệu từ file và khởi tạo ma trận.
    /// </summary>
    private void GenerateMatrixFromMapFile()
    {
        ClearOldMatrix();

        int[,] mapData = ReadMapDataFromFile(relativeFilePath, out int size);

        Renderer squareRenderer = groundSquarePrefab.GetComponent<Renderer>();
        float squareSize = squareRenderer.bounds.size.x; // Vì là ma trận vuông, sử dụng cùng 1 kích thước cho cả width và height.

        CreateMatrixParent();

        // Tính toán vị trí bắt đầu
        float totalMatrixSize = size * squareSize + (size - 1) * spacing;

        Vector3 startPosition = new Vector3(
            -totalMatrixSize / 2 + squareSize / 2,
            totalMatrixSize / 2 - squareSize / 2,
            0
        );

        // Đặt ma trận ở trung tâm `matrixGenerator`
        matrixParent.position = transform.position;

        // Tạo ma trận
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Vector3 position = startPosition + new Vector3(
                    j * (squareSize + spacing),
                    -i * (squareSize + spacing),
                    0
                );

                // Tạo ô nền của ma trận
                CreateSquare(position);

                // Tạo prefab overlay dựa vào giá trị từ file map
                if (mapData[i, j] != 0)
                {
                    CreateOverlayPrefab(mapData[i, j], position, squareSize);
                }
            }
        }
    }

    /// <summary>
    /// Đọc dữ liệu từ file map và trả về ma trận cùng kích thước.
    /// </summary>
    private int[,] ReadMapDataFromFile(string filePath, out int size)
    {
        string fullPath = Path.Combine(Application.dataPath, filePath);
        string[] lines = File.ReadAllLines(fullPath);

        size = int.Parse(lines[0]);

        int[,] mapData = new int[size, size];
        for (int i = 0; i < size; i++)
        {
            string[] rowValues = lines[i + 1].Split(' ');
            for (int j = 0; j < size; j++)
            {
                mapData[i, j] = int.Parse(rowValues[j]);
            }
        }

        return mapData;
    }
    /// <summary>
    /// Xóa ma trận cũ trước khi tạo lại.
    /// </summary>
    private void ClearOldMatrix()
    {
        if (matrixParent != null)
        {
            Destroy(matrixParent.gameObject);
        }
    }

    /// <summary>
    /// Tạo object cha cho toàn bộ ma trận.
    /// </summary>
    private void CreateMatrixParent()
    {
        GameObject matrixObject = new GameObject("MatrixParent");
        matrixParent = matrixObject.transform;
        matrixParent.SetParent(transform, false);
        matrixParent.localPosition = Vector3.zero;

        // Liên kết với PipeConnectionManager
        PipeConnectionManager pipeConnectionManager = FindObjectOfType<PipeConnectionManager>();
        if (pipeConnectionManager != null)
        {
            pipeConnectionManager.SetMatrixParent(matrixParent);
        }
    }

    /// <summary>
    /// Tạo một ô vuông (nền) tại vị trí chỉ định.
    /// </summary>
    private void CreateSquare(Vector3 position)
    {
        GameObject instance = Instantiate(groundSquarePrefab, position, Quaternion.identity);
        instance.transform.SetParent(matrixParent, false);
    }

    /// <summary>
    /// Tạo prefab overlay (ví dụ: DisconnectPipe) tại vị trí chỉ định.
    /// </summary>
    private void CreateOverlayPrefab(int value, Vector3 position, float size)
    {
        GameObject prefabToInstantiate = GetPrefabFromValue(value);

        if (prefabToInstantiate != null)
        {
            GameObject instance = Instantiate(prefabToInstantiate, position, Quaternion.identity);
            instance.transform.SetParent(matrixParent, false);

            ResizePrefab(instance, size);

            // Đặt giá trị và trạng thái của ClickableSquare
            var clickable = instance.AddComponent<ClickableSquare>();
            clickable.value = value; // Gán giá trị ma trận cho ô
            clickable.isStartPoint = value != 0; // Chỉ các ô có giá trị khác 0 là điểm bắt đầu

            Debug.Log($"Tạo overlay prefab tại ô có giá trị {value}");
        }
    }
    /// <summary>
    /// Lấy prefab tương ứng với giá trị từ file map.
    /// </summary>
    private GameObject GetPrefabFromValue(int value)
    {
        switch (value)
        {
            case 1: return disconnectPipeRedPrefab;
            case 2: return disconnectPipeOrangePrefab;
            case 3: return disconnectPipeYellowPrefab;
            case 4: return disconnectPipeBluePrefab;
            case 5: return disconnectPipeGreenPrefab;
            case 6: return disconnectPipePurplePrefab;
            default: return null;
        }
    }

    /// <summary>
    /// Resize prefab để vừa với ô ma trận.
    /// </summary>
    private void ResizePrefab(GameObject prefabInstance, float targetSize)
    {
        Renderer renderer = prefabInstance.GetComponent<Renderer>();
        if (renderer != null)
        {
            Vector3 currentSize = renderer.bounds.size;

            float scale = targetSize / currentSize.x; // Sử dụng cùng một scale cho cả hai chiều (ma trận vuông)

            prefabInstance.transform.localScale = new Vector3(
                prefabInstance.transform.localScale.x * scale,
                prefabInstance.transform.localScale.y * scale,
                prefabInstance.transform.localScale.z
            );
        }
    }
}
