using UnityEngine;

public class InputController {

    public static float Horizontal
    {
        get { return Input.GetAxis("Horizontal"); }
    }
	
    public static float Vertical
    {
        get { return Input.GetAxis("Vertical"); }
    }
	
    public static bool Up
    {
        get { return Input.GetKeyDown(KeyCode.UpArrow); }
    }
	
    public static bool Right
    {
        get { return Input.GetKey(KeyCode.RightArrow); }
    }
	
    public static bool Down
    {
        get { return Input.GetKey(KeyCode.DownArrow); }
    }
	
    public static bool Left
    {
        get { return Input.GetKey(KeyCode.LeftArrow); }
    }

    public static bool W
    {
        get { return Input.GetKeyDown(KeyCode.W); }
    }
	
    public static bool D
    {
        get { return Input.GetKey(KeyCode.D); }
    }
	
    public static bool S
    {
        get { return Input.GetKey(KeyCode.S); }
    }
	
    public static bool A
    {
        get { return Input.GetKeyDown(KeyCode.A); }
    }
	
    public static bool Z
    {
        get { return Input.GetKeyDown(KeyCode.Z); }
    }
	
    public static bool Restart
    {
        get { return Input.GetKeyDown(KeyCode.R); }
    }

    public static bool Undo
    {
        get { return Input.GetKeyDown(KeyCode.Z); }
    }

    public static bool Jump
    {
        get { return Input.GetButton("Jump"); }
    }
	
    public static bool JumpDown
    {
        get { return Input.GetButtonDown("Jump"); }
    }

    public static bool Dash
    {
        get { return Input.GetButtonDown("Dash"); }
    }
}