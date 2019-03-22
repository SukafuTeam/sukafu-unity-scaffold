using UnityEngine;

[RequireComponent(typeof(PlataformController))]
public class DefaultPlayerController : MonoBehaviour
{
	private PlataformController _plataform;
	public PlataformController Plataform
	{
		get { return _plataform ?? (_plataform = GetComponent<PlataformController>()); }
	}
	
	void Update () {
		if(Input.GetKey(KeyCode.LeftArrow))
			Plataform.MoveLeft();
		if(Input.GetKey(KeyCode.RightArrow))
			Plataform.MoveRight();
		if(Input.GetKeyDown(KeyCode.Space))
			Plataform.Jump();
	}
}
