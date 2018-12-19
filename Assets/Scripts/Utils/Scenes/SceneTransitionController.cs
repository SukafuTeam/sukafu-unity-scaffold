using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
	public static SceneTransitionController Instance;

	public bool Locked;
	
	void OnEnable()
	{
		SceneManager.sceneLoaded += FinishedLoading;
	}
	
	
	void OnDisable()
	{
		SceneManager.sceneLoaded -= FinishedLoading;
	}

	public void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}
	
	public void TransitionTo(string scene)
	{
		if (Locked)
			return;
		
		ColorTransition.Instance.Transition(scene);
		Locked = true;
	}

	public IEnumerator Transition(string scene)
	{
		yield return new WaitForSeconds(1f);
		SceneManager.LoadSceneAsync(scene);
	}
	
	public void FinishedLoading(Scene scene, LoadSceneMode mode)
	{
		Locked = false;
	}
}
