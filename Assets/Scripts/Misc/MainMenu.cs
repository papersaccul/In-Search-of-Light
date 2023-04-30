using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IDeselectHandler
{
    public GameObject teleportedSprite;
    private UnityEngine.UI.Button lastSelectedButton;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UnityEngine.UI.Button button = eventData.pointerEnter.GetComponent<UnityEngine.UI.Button>();

        if (button != null)
        {
            lastSelectedButton = button;

            button.Select();
            button.OnPointerEnter(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnityEngine.UI.Button button = eventData.pointerEnter.GetComponent<UnityEngine.UI.Button>();

        if (button != null)
            button.OnPointerExit(eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
        StartCoroutine(MoveSpriteSmoothly(eventData.selectedObject.transform.position - new Vector3(20f, 0f)));

        UnityEngine.UI.Button selectedButton = eventData.selectedObject.GetComponent<UnityEngine.UI.Button>();

        if (selectedButton != null)
            selectedButton.Select();

    }

    public void OnDeselect(BaseEventData eventData)
    {
        StartCoroutine(DeselectAndReselect(eventData)); // If do not wait for the end of the frame, then eventData.selectedObject will return the previous object instead of null
    }

    private IEnumerator DeselectAndReselect(BaseEventData eventData)
    {
        yield return new WaitForEndOfFrame();

        if (eventData.selectedObject == null)
            EventSystem.current.SetSelectedGameObject(lastSelectedButton.gameObject);
    }

    private IEnumerator MoveSpriteSmoothly(Vector3 targetPosition)
    {
        yield return new WaitForEndOfFrame();

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