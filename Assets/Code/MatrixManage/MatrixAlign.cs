using UnityEngine;

public class MatrixAlign : MonoBehaviour
{
    public GameObject matrixArea; // Khu vực chứa ma trận

    void Start()
    {
        AlignToMatrixArea();
    }

    private void AlignToMatrixArea()
    {
        if (matrixArea == null)
        {
            Debug.LogError("matrixArea chưa được gán!");
            return;
        }

        // Lấy vị trí và đảm bảo đối tượng nằm ở trung tâm của matrixArea
        transform.position = matrixArea.transform.position;

    }
}
