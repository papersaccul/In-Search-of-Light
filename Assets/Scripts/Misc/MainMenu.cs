using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        teleportedSprite.transform.position = eventData.pointerEnter.transform.position - new Vector3(32f, 0f);
        teleportedSprite.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        teleportedSprite.SetActive(false);
    }
}