using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;

    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    bool isMoving;

    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Определяем, стоим ли мы на земле:
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Сбрасываем до ускорения по умолчанию:
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Получаем ввод:
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Создаем двигающийся вектор:
        Vector3 move = transform.right * x + transform.forward * z; // (right - красная ось; forward - синяя ось)

        // Двигаем по-настоящему:
        characterController.Move(move * speed * Time.deltaTime);

        // Проверить, может ли игрок прыгать:
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            // По-настоящему прыгаем:
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Падаем вниз:
        velocity.y += gravity * Time.deltaTime;

        // Выполняем прыжок:
        characterController.Move(velocity * Time.deltaTime);

        if (lastPosition != gameObject.transform.position && isGrounded == true)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        lastPosition = gameObject.transform.position;
    }
}
