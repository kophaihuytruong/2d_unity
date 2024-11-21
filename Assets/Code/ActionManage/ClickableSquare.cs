using UnityEngine;

public class ClickableSquare : MonoBehaviour
{
    public int value; // Giá trị của ô (0: trống, 1-6: các giá trị màu)
    public bool isStartPoint = false; // Đánh dấu ô đã được chọn là điểm bắt đầu

    private void OnMouseDown()
    {
        // Kiểm tra nếu ô được click có giá trị hợp lệ
        if (value != 0 && !isStartPoint)
        {
            isStartPoint = true; // Đánh dấu ô là điểm bắt đầu
            Debug.Log($"Ô {gameObject.name} đã được chọn làm điểm bắt đầu với giá trị {value}");

            // Gửi ô đã chọn đến PipeConnectionManager
            PipeConnectionManager.Instance.StartConnection(this);
        }
        else
        {
            Debug.Log($"Ô {gameObject.name} không hợp lệ để làm điểm bắt đầu. Giá trị: {value}");
        }
    }

    private void OnMouseOver()
    {
        // Chỉ kiểm tra khi đã bắt đầu nối ống
        if (PipeConnectionManager.Instance.IsCreatingPipe)
        {
            PipeConnectionManager.Instance.ContinueConnection(this);
        }
    }

    private void OnMouseExit()
    {
        // Chỉ log khi đã bắt đầu nối ống
        if (PipeConnectionManager.Instance.IsCreatingPipe)
        {
            Debug.Log($"Đã rời khỏi ô {gameObject.name}");
        }
    }
}
