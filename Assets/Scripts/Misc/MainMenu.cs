using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour, IPointerEnterHandler
{
    public GameObject teleportedSprite;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("quit");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(MoveSpriteSmoothly(eventData.pointerEnter.transform.position - new Vector3(20f, 0f)));
    }

    private IEnumerator MoveSpriteSmoothly(Vector3 targetPosition)
    {
        float duration = 0.1f;
        float timeElapsed = 0f;
        Vector3 startPosition = teleportedSprite.transform.position;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            float lerpValue = Mathf.Clamp01(timeElapsed / duration);

            teleportedSprite.transform.position = Vector3.Lerp(startPosition, targetPosition, lerpValue);

            yield return null;
        }
    }
}