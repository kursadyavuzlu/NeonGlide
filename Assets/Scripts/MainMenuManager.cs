using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	public void StartGame()
	{
		Debug.Log("Oyun Ba�lat�ld�!");

		SceneManager.LoadScene("GameScene");
	}

	public void ExitGame()
	{
		Debug.Log("Oyundan ��k�ld�!");

		Application.Quit();

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}