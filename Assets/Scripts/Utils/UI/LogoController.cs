using System.Collections;
using DG.Tweening;
//using Firebase;
//using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogoController : MonoBehaviour
{
	public float FadeTime;
	public float WaitTime;

	public Image LogoFilter;

	public GameObject Logo;

	void Start ()
	{
		StartCoroutine(LogoRoutine());
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
//		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://kraken-escape.firebaseio.com/");
	}

	public IEnumerator LogoRoutine()
	{
		Logo.transform.DOScale(Vector3.one * Logo.transform.localScale.x * 1.4f, WaitTime + FadeTime);
		yield return new WaitForSeconds(WaitTime);
		LogoFilter.DOFade(1, FadeTime);
		yield return new WaitForSeconds(FadeTime);
		SceneManager.LoadScene("scene_game");
	}
}
