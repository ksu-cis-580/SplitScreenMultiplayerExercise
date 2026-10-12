using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CIS580.Input;

/// <summary>
/// Represents an input action that can be triggered by specific buttons or keys.
/// </summary>  
/// <remarks>
/// An input action is a set of buttons or keys that translate to a single action in the
/// game. This is useful for mapping multiple input methods to a single game action
/// (i.e. "W" and Up Arrow and up on the D-Pad).
/// </remarks>
public class InputAction
{
    private readonly Buttons[] _buttons;
    private readonly Keys[] _keys;
    private readonly bool _firstPressOnly;


    // Delegate types for button and key press checks, used to simplify the Occurred method
    private delegate bool ButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex);
    private delegate bool KeyPress(Keys key);


    /// <summary>
    /// Initializes a new instance of the InputAction class with the specified trigger buttons, trigger keys, and first press only flag.
    /// </summary>
    /// <param name="triggerButtons">The buttons that trigger the action</param>
    /// <param name="triggerKeys">The keys that trigger the action</param>
    /// <param name="firstPressOnly">Indicates if the action should only occur on the first press</param>
    public InputAction(Buttons[] triggerButtons, Keys[] triggerKeys, bool firstPressOnly)
    {
        _buttons = triggerButtons != null ? triggerButtons.Clone() as Buttons[] : new Buttons[0];
        _keys = triggerKeys != null ? triggerKeys.Clone() as Keys[] : new Keys [0];
        _firstPressOnly = firstPressOnly;
    }

    /// <summary>
    /// Determines if the input action has occurred based on the current input state.
    /// </summary>
    /// <param name="inputState">Input state to check</param>
    /// <param name="controllingPlayer">The player whose gamepad should be checked, or null to check all gamepads</param>
    /// <param name="playerIndex">The index of the player who triggered the input</param>
    /// <returns>True if the input action has occurred, false otherwise</returns>
    public bool Occurred(InputState inputState, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
    {
        ButtonPress buttonTest;
        KeyPress keyTest;

        if(_firstPressOnly)
        {
            buttonTest = inputState.IsNewButtonPress;
            keyTest = inputState.IsNewKeyPress;
        }
        else
        {
            buttonTest = inputState.IsButtonPressed;
            keyTest = inputState.IsKeyPressed;
        }

        foreach(var button in _buttons)
        {
            if(buttonTest(button, controllingPlayer, out playerIndex))
                return true;
        }
        foreach(var key in _keys)
        {
            if(keyTest(key))
            {
                // The keyboard is shared, so credit it to the controlling player (or player one)
                playerIndex = controllingPlayer ?? PlayerIndex.One;
                return true;
            } 
        }

        // If we get here, no button or key was pressed
        playerIndex = PlayerIndex.One; // Default to player one if no input is detected
        return false;
    }
}