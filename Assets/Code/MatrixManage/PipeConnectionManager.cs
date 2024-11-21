using UnityEngine;

public class PipeConnectionManager : MonoBehaviour
{
    public static PipeConnectionManager Instance { get; private set; }

    private bool isCreatingPipe = false;
    public bool IsCreatingPipe => isCreatingPipe; // Trạng thái đang nối ống
    private GameObject squareFillPrefab; // Prefab tô màu
    private ClickableSquare lastSquare; // Ô cuối cùng đã xử lý
    private Transform matrixParent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogError("Đã tồn tại một PipeConnectionManager khác!");
            return;
        }
        Instance = this;
    }

    public void SetMatrixParent(Transform parent)
    {
        matrixParent = parent;
    }

    public void StartConnection(ClickableSquare square)
    {
        if (square.value != 0) // Kiểm tra giá trị ô hợp lệ
        {
            isCreatingPipe = true;
            lastSquare = square; // Lưu ô bắt đầu
            squareFillPrefab = GetSquareFillPrefabFromValue(square.value); // Lấy prefab tương ứng
            Debug.Log($"Bắt đầu tô màu từ ô {square.name} với giá trị {square.value}");
        }
    }

    public void ContinueConnection(ClickableSquare square)
    {
        if (!isCreatingPipe || square == null || square == lastSquare)
        {
            Debug.Log("Không thể tiếp tục kết nối, ô không hợp lệ hoặc chưa bắt đầu.");
            return;
        }

        if (square.value == 0 && IsAdjacent(lastSquare, square))
        {
            CreateSquareFill(square); // Tô màu ô mới
            lastSquare = square; // Cập nhật ô hiện tại
        }
        else
        {
            Debug.Log($"Ô {square.name} không hợp lệ để tiếp tục. Giá trị: {square.value}");
        }
    }

    public void StopConnection()
    {
        isCreatingPipe = false;
        lastSquare = null;
        squareFillPrefab = null;
        Debug.Log("Kết thúc nối ống.");
    }

    private void CreateSquareFill(ClickableSquare square)
    {
        if (squareFillPrefab == null) return;

        GameObject fill = Instantiate(squareFillPrefab, square.transform.position, Quaternion.identity);
        fill.transform.SetParent(matrixParent, false);
        fill.transform.localScale = Vector3.one;

        square.value = lastSquare.value; // Gán giá trị mới cho ô
        Debug.Log($"Tạo overlay prefab tại ô {square.name} với giá trị {square.value}");
    }

    private bool IsAdjacent(ClickableSquare current, ClickableSquare target)
    {
        Vector2 currentPos = current.transform.position;
        Vector2 targetPos = target.transform.position;

        // Kiểm tra khoảng cách giữa hai ô
        return Vector2.Distance(currentPos, targetPos) < 1.1f; // Điều chỉnh tùy theo kích thước ô
    }

    private GameObject GetSquareFillPrefabFromValue(int value)
    {
        switch (value)
        {
            case 1: return Resources.Load<GameObject>("Square_Fill_Red");
            case 2: return Resources.Load<GameObject>("Square_Fill_Orange");
            case 3: return Resources.Load<GameObject>("Square_Fill_Yellow");
            case 4: return Resources.Load<GameObject>("Square_Fill_Blue");
            case 5: return Resources.Load<GameObject>("Square_Fill_Green");
            case 6: return Resources.Load<GameObject>("Square_Fill_Purple");
            default: return null;
        }
    }
}
