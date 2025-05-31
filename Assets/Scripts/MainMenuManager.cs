using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	public void StartGame()
	{
		Debug.Log("Oyun Baþlatýldý!");

		SceneManager.LoadScene("GameScene");
	}

	public void ExitGame()
	{
		Debug.Log("Oyundan Çýkýldý!");

		Application.Quit();

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}