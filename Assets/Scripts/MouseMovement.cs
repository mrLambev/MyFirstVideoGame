using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 200f;

    float xRotation = 0f;
    float yRotation = 0f;

    public float topClamp = -90f;
    public float bottomClamp = 90f; 

    void Start()
    {
        // Блокируем курсор и делаем его невидимым, чтобы не мешал:
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Получаем ввод мыши:
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Поворачиваемся вокруг оси X:
        xRotation -= mouseY;

        // Ограничиваем повороты вверх-вниз:
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        // Поворачиваемся вокруг оси Y:
        yRotation += mouseX;

        // Производим трансформации на основе ввдоа игрока:
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
