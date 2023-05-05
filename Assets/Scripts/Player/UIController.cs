using UnityEngine;
using UnityEngine.UI;

public class MoveUI : MonoBehaviour
{
    [SerializeField] private Vector2 offset = new Vector2(5f, -10f);
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float returnSpeed = 10f;

    private RectTransform rectTransform;
    private Vector2 initialPosition;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
    }

    private void FixedUpdate()
    {
        Rigidbody2D playerRigidbody = Player.Instance.GetComponent<Rigidbody2D>();

        if (playerRigidbody.velocity.magnitude == 0f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, initialPosition, returnSpeed * Time.deltaTime);
        }
        else
        {
            Vector2 playerPosition = playerRigidbody.position;
            Vector2 playerVelocity = playerRigidbody.velocity;

            float moveDistance = playerVelocity.magnitude * Time.fixedDeltaTime;

            Vector2 moveDirection = playerVelocity.normalized;
            Vector2 moveVector = moveDirection * moveDistance;

            Vector2 targetPosition = initialPosition + moveVector + offset;
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
}