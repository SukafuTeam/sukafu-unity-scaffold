using UnityEngine;

public class InputController {

	public static bool Up
	{
		get { return Input.GetKeyDown(KeyCode.UpArrow); }
	}
	
	public static bool Right
	{
		get { return Input.GetKeyDown(KeyCode.RightArrow); }
	}
	
	public static bool Down
	{
		get { return Input.GetKeyDown(KeyCode.DownArrow); }
	}
	
	public static bool Left
	{
		get { return Input.GetKeyDown(KeyCode.LeftArrow); }
	}

	public static bool Restart
	{
		get { return Input.GetKeyDown(KeyCode.R); }
	}

	public static bool Undo
	{
		get { return Input.GetKeyDown(KeyCode.Z); }
	}
}
