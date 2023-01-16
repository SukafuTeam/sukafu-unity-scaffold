using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputController {

    public static float Horizontal
    {
        get { return Input.GetAxis("Horizontal"); }
    }

    public static Vector2 LeftStick
    {
        get
        {
            return Gamepad.current == null ? Vector2.zero : Gamepad.current.leftStick.ReadValue();
        }
    }

    public static bool MoveRight(float threshold = 0.2f)
    {
        var keyHor = InputController.Horizontal;
        var padHor = InputController.LeftStick.x;
        return keyHor > threshold || padHor > threshold;
    }
    
    public static bool MoveLeft(float threshold = 0.2f)
    {
        var absThreshold = Mathf.Abs(threshold);
        
        var keyHor = InputController.Horizontal;
        var padHor = InputController.LeftStick.x;

        return keyHor < -absThreshold || padHor < -absThreshold;
    }
	
    public static float Vertical
    {
        get { return Input.GetAxis("Vertical"); }
    }
	
    public static bool Up
    {
        get
        {
            var upArrow = Input.GetKeyDown(KeyCode.UpArrow);

            if (Gamepad.current == null)
                return upArrow;
            var gamepadUp = Gamepad.current.dpad.up.isPressed;

            return upArrow || gamepadUp;
        }
    }
	
    public static bool Right
    {
        get
        {
            var rightArrow = Input.GetKeyDown(KeyCode.RightArrow);

            if (Gamepad.current == null)
                return rightArrow;
            var gamepadRight = Gamepad.current.dpad.right.isPressed;

            return rightArrow || gamepadRight;
        }
    }
	
    public static bool Down
    {
        get
        {
            var downArrow = Input.GetKeyDown(KeyCode.DownArrow);

            if (Gamepad.current == null)
                return downArrow;
            var gamepadDown = Gamepad.current.dpad.down.isPressed;

            return downArrow || gamepadDown;
        }
    }
	
    public static bool Left
    {
        get
        {
            var leftArrow = Input.GetKeyDown(KeyCode.LeftArrow);

            if (Gamepad.current == null)
                return leftArrow;
            var gamepadLeft = Gamepad.current.dpad.left.isPressed;

            return leftArrow || gamepadLeft;
        }
    }
    
    public static bool Jump
    {
        get
        {
            var keyJump = Input.GetKey(KeyCode.Z);
            if (Gamepad.current == null)
                return keyJump;
            var gamePadJump = Gamepad.current.buttonSouth.isPressed;
            return keyJump || gamePadJump;
        }
    }
	
    public static bool JumpDown
    {
        get
        {
            var keyJump = Input.GetKeyDown(KeyCode.Z);
            if (Gamepad.current == null)
                return keyJump;
            var gamePadJump = Gamepad.current.buttonSouth.wasPressedThisFrame;
            return keyJump || gamePadJump;
        }
    }

    public static bool DashDown
    {
        get
        {
            var keyDash = Input.GetKeyDown(KeyCode.X);
            if (Gamepad.current == null)
                return keyDash;
            var gamePadDash = Gamepad.current.buttonWest.wasPressedThisFrame;
            return keyDash || gamePadDash;
        }
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
}