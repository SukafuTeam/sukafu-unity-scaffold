using UnityEngine;

public class ChangeScene : MonoBehaviour {

	public void LoadScene(string scene)
	{
		SceneTransitionController.Instance.TransitionTo(scene);
	}
}
