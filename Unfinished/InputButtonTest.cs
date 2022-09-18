using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class InputButtonTest : UdonSharpBehaviour
    {
        public TextMeshPro tmpText;
        private string[] lines = new string[]
        {
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
        };

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.None))
                KeyDown("None");
            if (Input.GetKeyDown(KeyCode.Backspace))
                KeyDown("Backspace");
            if (Input.GetKeyDown(KeyCode.Tab))
                KeyDown("Tab");
            if (Input.GetKeyDown(KeyCode.Clear))
                KeyDown("Clear");
            if (Input.GetKeyDown(KeyCode.Return))
                KeyDown("Return");
            if (Input.GetKeyDown(KeyCode.Pause))
                KeyDown("Pause");
            if (Input.GetKeyDown(KeyCode.Escape))
                KeyDown("Escape");
            if (Input.GetKeyDown(KeyCode.Space))
                KeyDown("Space");
            if (Input.GetKeyDown(KeyCode.Exclaim))
                KeyDown("Exclaim");
            if (Input.GetKeyDown(KeyCode.DoubleQuote))
                KeyDown("DoubleQuote");
            if (Input.GetKeyDown(KeyCode.Hash))
                KeyDown("Hash");
            if (Input.GetKeyDown(KeyCode.Dollar))
                KeyDown("Dollar");
            if (Input.GetKeyDown(KeyCode.Percent))
                KeyDown("Percent");
            if (Input.GetKeyDown(KeyCode.Ampersand))
                KeyDown("Ampersand");
            if (Input.GetKeyDown(KeyCode.Quote))
                KeyDown("Quote");
            if (Input.GetKeyDown(KeyCode.LeftParen))
                KeyDown("LeftParen");
            if (Input.GetKeyDown(KeyCode.RightParen))
                KeyDown("RightParen");
            if (Input.GetKeyDown(KeyCode.Asterisk))
                KeyDown("Asterisk");
            if (Input.GetKeyDown(KeyCode.Plus))
                KeyDown("Plus");
            if (Input.GetKeyDown(KeyCode.Comma))
                KeyDown("Comma");
            if (Input.GetKeyDown(KeyCode.Minus))
                KeyDown("Minus");
            if (Input.GetKeyDown(KeyCode.Period))
                KeyDown("Period");
            if (Input.GetKeyDown(KeyCode.Slash))
                KeyDown("Slash");
            if (Input.GetKeyDown(KeyCode.Alpha0))
                KeyDown("Alpha0");
            if (Input.GetKeyDown(KeyCode.Alpha1))
                KeyDown("Alpha1");
            if (Input.GetKeyDown(KeyCode.Alpha2))
                KeyDown("Alpha2");
            if (Input.GetKeyDown(KeyCode.Alpha3))
                KeyDown("Alpha3");
            if (Input.GetKeyDown(KeyCode.Alpha4))
                KeyDown("Alpha4");
            if (Input.GetKeyDown(KeyCode.Alpha5))
                KeyDown("Alpha5");
            if (Input.GetKeyDown(KeyCode.Alpha6))
                KeyDown("Alpha6");
            if (Input.GetKeyDown(KeyCode.Alpha7))
                KeyDown("Alpha7");
            if (Input.GetKeyDown(KeyCode.Alpha8))
                KeyDown("Alpha8");
            if (Input.GetKeyDown(KeyCode.Alpha9))
                KeyDown("Alpha9");
            if (Input.GetKeyDown(KeyCode.Colon))
                KeyDown("Colon");
            if (Input.GetKeyDown(KeyCode.Semicolon))
                KeyDown("Semicolon");
            if (Input.GetKeyDown(KeyCode.Less))
                KeyDown("Less");
            if (Input.GetKeyDown(KeyCode.Equals))
                KeyDown("Equals");
            if (Input.GetKeyDown(KeyCode.Greater))
                KeyDown("Greater");
            if (Input.GetKeyDown(KeyCode.Question))
                KeyDown("Question");
            if (Input.GetKeyDown(KeyCode.At))
                KeyDown("At");
            if (Input.GetKeyDown(KeyCode.LeftBracket))
                KeyDown("LeftBracket");
            if (Input.GetKeyDown(KeyCode.Backslash))
                KeyDown("Backslash");
            if (Input.GetKeyDown(KeyCode.RightBracket))
                KeyDown("RightBracket");
            if (Input.GetKeyDown(KeyCode.Caret))
                KeyDown("Caret");
            if (Input.GetKeyDown(KeyCode.Underscore))
                KeyDown("Underscore");
            if (Input.GetKeyDown(KeyCode.BackQuote))
                KeyDown("BackQuote");
            if (Input.GetKeyDown(KeyCode.A))
                KeyDown("A");
            if (Input.GetKeyDown(KeyCode.B))
                KeyDown("B");
            if (Input.GetKeyDown(KeyCode.C))
                KeyDown("C");
            if (Input.GetKeyDown(KeyCode.D))
                KeyDown("D");
            if (Input.GetKeyDown(KeyCode.E))
                KeyDown("E");
            if (Input.GetKeyDown(KeyCode.F))
                KeyDown("F");
            if (Input.GetKeyDown(KeyCode.G))
                KeyDown("G");
            if (Input.GetKeyDown(KeyCode.H))
                KeyDown("H");
            if (Input.GetKeyDown(KeyCode.I))
                KeyDown("I");
            if (Input.GetKeyDown(KeyCode.J))
                KeyDown("J");
            if (Input.GetKeyDown(KeyCode.K))
                KeyDown("K");
            if (Input.GetKeyDown(KeyCode.L))
                KeyDown("L");
            if (Input.GetKeyDown(KeyCode.M))
                KeyDown("M");
            if (Input.GetKeyDown(KeyCode.N))
                KeyDown("N");
            if (Input.GetKeyDown(KeyCode.O))
                KeyDown("O");
            if (Input.GetKeyDown(KeyCode.P))
                KeyDown("P");
            if (Input.GetKeyDown(KeyCode.Q))
                KeyDown("Q");
            if (Input.GetKeyDown(KeyCode.R))
                KeyDown("R");
            if (Input.GetKeyDown(KeyCode.S))
                KeyDown("S");
            if (Input.GetKeyDown(KeyCode.T))
                KeyDown("T");
            if (Input.GetKeyDown(KeyCode.U))
                KeyDown("U");
            if (Input.GetKeyDown(KeyCode.V))
                KeyDown("V");
            if (Input.GetKeyDown(KeyCode.W))
                KeyDown("W");
            if (Input.GetKeyDown(KeyCode.X))
                KeyDown("X");
            if (Input.GetKeyDown(KeyCode.Y))
                KeyDown("Y");
            if (Input.GetKeyDown(KeyCode.Z))
                KeyDown("Z");
            if (Input.GetKeyDown(KeyCode.LeftCurlyBracket))
                KeyDown("LeftCurlyBracket");
            if (Input.GetKeyDown(KeyCode.Pipe))
                KeyDown("Pipe");
            if (Input.GetKeyDown(KeyCode.RightCurlyBracket))
                KeyDown("RightCurlyBracket");
            if (Input.GetKeyDown(KeyCode.Tilde))
                KeyDown("Tilde");
            if (Input.GetKeyDown(KeyCode.Delete))
                KeyDown("Delete");
            if (Input.GetKeyDown(KeyCode.Keypad0))
                KeyDown("Keypad0");
            if (Input.GetKeyDown(KeyCode.Keypad1))
                KeyDown("Keypad1");
            if (Input.GetKeyDown(KeyCode.Keypad2))
                KeyDown("Keypad2");
            if (Input.GetKeyDown(KeyCode.Keypad3))
                KeyDown("Keypad3");
            if (Input.GetKeyDown(KeyCode.Keypad4))
                KeyDown("Keypad4");
            if (Input.GetKeyDown(KeyCode.Keypad5))
                KeyDown("Keypad5");
            if (Input.GetKeyDown(KeyCode.Keypad6))
                KeyDown("Keypad6");
            if (Input.GetKeyDown(KeyCode.Keypad7))
                KeyDown("Keypad7");
            if (Input.GetKeyDown(KeyCode.Keypad8))
                KeyDown("Keypad8");
            if (Input.GetKeyDown(KeyCode.Keypad9))
                KeyDown("Keypad9");
            if (Input.GetKeyDown(KeyCode.KeypadPeriod))
                KeyDown("KeypadPeriod");
            if (Input.GetKeyDown(KeyCode.KeypadDivide))
                KeyDown("KeypadDivide");
            if (Input.GetKeyDown(KeyCode.KeypadMultiply))
                KeyDown("KeypadMultiply");
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
                KeyDown("KeypadMinus");
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
                KeyDown("KeypadPlus");
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
                KeyDown("KeypadEnter");
            if (Input.GetKeyDown(KeyCode.KeypadEquals))
                KeyDown("KeypadEquals");
            if (Input.GetKeyDown(KeyCode.UpArrow))
                KeyDown("UpArrow");
            if (Input.GetKeyDown(KeyCode.DownArrow))
                KeyDown("DownArrow");
            if (Input.GetKeyDown(KeyCode.RightArrow))
                KeyDown("RightArrow");
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                KeyDown("LeftArrow");
            if (Input.GetKeyDown(KeyCode.Insert))
                KeyDown("Insert");
            if (Input.GetKeyDown(KeyCode.Home))
                KeyDown("Home");
            if (Input.GetKeyDown(KeyCode.End))
                KeyDown("End");
            if (Input.GetKeyDown(KeyCode.PageUp))
                KeyDown("PageUp");
            if (Input.GetKeyDown(KeyCode.PageDown))
                KeyDown("PageDown");
            if (Input.GetKeyDown(KeyCode.F1))
                KeyDown("F1");
            if (Input.GetKeyDown(KeyCode.F2))
                KeyDown("F2");
            if (Input.GetKeyDown(KeyCode.F3))
                KeyDown("F3");
            if (Input.GetKeyDown(KeyCode.F4))
                KeyDown("F4");
            if (Input.GetKeyDown(KeyCode.F5))
                KeyDown("F5");
            if (Input.GetKeyDown(KeyCode.F6))
                KeyDown("F6");
            if (Input.GetKeyDown(KeyCode.F7))
                KeyDown("F7");
            if (Input.GetKeyDown(KeyCode.F8))
                KeyDown("F8");
            if (Input.GetKeyDown(KeyCode.F9))
                KeyDown("F9");
            if (Input.GetKeyDown(KeyCode.F10))
                KeyDown("F10");
            if (Input.GetKeyDown(KeyCode.F11))
                KeyDown("F11");
            if (Input.GetKeyDown(KeyCode.F12))
                KeyDown("F12");
            if (Input.GetKeyDown(KeyCode.F13))
                KeyDown("F13");
            if (Input.GetKeyDown(KeyCode.F14))
                KeyDown("F14");
            if (Input.GetKeyDown(KeyCode.F15))
                KeyDown("F15");
            if (Input.GetKeyDown(KeyCode.Numlock))
                KeyDown("Numlock");
            if (Input.GetKeyDown(KeyCode.CapsLock))
                KeyDown("CapsLock");
            if (Input.GetKeyDown(KeyCode.ScrollLock))
                KeyDown("ScrollLock");
            if (Input.GetKeyDown(KeyCode.RightShift))
                KeyDown("RightShift");
            if (Input.GetKeyDown(KeyCode.LeftShift))
                KeyDown("LeftShift");
            if (Input.GetKeyDown(KeyCode.RightControl))
                KeyDown("RightControl");
            if (Input.GetKeyDown(KeyCode.LeftControl))
                KeyDown("LeftControl");
            if (Input.GetKeyDown(KeyCode.RightAlt))
                KeyDown("RightAlt");
            if (Input.GetKeyDown(KeyCode.LeftAlt))
                KeyDown("LeftAlt");
            if (Input.GetKeyDown(KeyCode.RightCommand))
                KeyDown("RightCommand");
            if (Input.GetKeyDown(KeyCode.RightApple))
                KeyDown("RightApple");
            if (Input.GetKeyDown(KeyCode.LeftCommand))
                KeyDown("LeftCommand");
            if (Input.GetKeyDown(KeyCode.LeftApple))
                KeyDown("LeftApple");
            if (Input.GetKeyDown(KeyCode.LeftWindows))
                KeyDown("LeftWindows");
            if (Input.GetKeyDown(KeyCode.RightWindows))
                KeyDown("RightWindows");
            if (Input.GetKeyDown(KeyCode.AltGr))
                KeyDown("AltGr");
            if (Input.GetKeyDown(KeyCode.Help))
                KeyDown("Help");
            if (Input.GetKeyDown(KeyCode.Print))
                KeyDown("Print");
            if (Input.GetKeyDown(KeyCode.SysReq))
                KeyDown("SysReq");
            if (Input.GetKeyDown(KeyCode.Break))
                KeyDown("Break");
            if (Input.GetKeyDown(KeyCode.Menu))
                KeyDown("Menu");
            if (Input.GetKeyDown(KeyCode.Mouse0))
                KeyDown("Mouse0");
            if (Input.GetKeyDown(KeyCode.Mouse1))
                KeyDown("Mouse1");
            if (Input.GetKeyDown(KeyCode.Mouse2))
                KeyDown("Mouse2");
            if (Input.GetKeyDown(KeyCode.Mouse3))
                KeyDown("Mouse3");
            if (Input.GetKeyDown(KeyCode.Mouse4))
                KeyDown("Mouse4");
            if (Input.GetKeyDown(KeyCode.Mouse5))
                KeyDown("Mouse5");
            if (Input.GetKeyDown(KeyCode.Mouse6))
                KeyDown("Mouse6");
            if (Input.GetKeyDown(KeyCode.JoystickButton0))
                KeyDown("JoystickButton0");
            if (Input.GetKeyDown(KeyCode.JoystickButton1))
                KeyDown("JoystickButton1");
            if (Input.GetKeyDown(KeyCode.JoystickButton2))
                KeyDown("JoystickButton2");
            if (Input.GetKeyDown(KeyCode.JoystickButton3))
                KeyDown("JoystickButton3");
            if (Input.GetKeyDown(KeyCode.JoystickButton4))
                KeyDown("JoystickButton4");
            if (Input.GetKeyDown(KeyCode.JoystickButton5))
                KeyDown("JoystickButton5");
            if (Input.GetKeyDown(KeyCode.JoystickButton6))
                KeyDown("JoystickButton6");
            if (Input.GetKeyDown(KeyCode.JoystickButton7))
                KeyDown("JoystickButton7");
            if (Input.GetKeyDown(KeyCode.JoystickButton8))
                KeyDown("JoystickButton8");
            if (Input.GetKeyDown(KeyCode.JoystickButton9))
                KeyDown("JoystickButton9");
            if (Input.GetKeyDown(KeyCode.JoystickButton10))
                KeyDown("JoystickButton10");
            if (Input.GetKeyDown(KeyCode.JoystickButton11))
                KeyDown("JoystickButton11");
            if (Input.GetKeyDown(KeyCode.JoystickButton12))
                KeyDown("JoystickButton12");
            if (Input.GetKeyDown(KeyCode.JoystickButton13))
                KeyDown("JoystickButton13");
            if (Input.GetKeyDown(KeyCode.JoystickButton14))
                KeyDown("JoystickButton14");
            if (Input.GetKeyDown(KeyCode.JoystickButton15))
                KeyDown("JoystickButton15");
            if (Input.GetKeyDown(KeyCode.JoystickButton16))
                KeyDown("JoystickButton16");
            if (Input.GetKeyDown(KeyCode.JoystickButton17))
                KeyDown("JoystickButton17");
            if (Input.GetKeyDown(KeyCode.JoystickButton18))
                KeyDown("JoystickButton18");
            if (Input.GetKeyDown(KeyCode.JoystickButton19))
                KeyDown("JoystickButton19");
            if (Input.GetKeyDown(KeyCode.Joystick1Button0))
                KeyDown("Joystick1Button0");
            if (Input.GetKeyDown(KeyCode.Joystick1Button1))
                KeyDown("Joystick1Button1");
            if (Input.GetKeyDown(KeyCode.Joystick1Button2))
                KeyDown("Joystick1Button2");
            if (Input.GetKeyDown(KeyCode.Joystick1Button3))
                KeyDown("Joystick1Button3");
            if (Input.GetKeyDown(KeyCode.Joystick1Button4))
                KeyDown("Joystick1Button4");
            if (Input.GetKeyDown(KeyCode.Joystick1Button5))
                KeyDown("Joystick1Button5");
            if (Input.GetKeyDown(KeyCode.Joystick1Button6))
                KeyDown("Joystick1Button6");
            if (Input.GetKeyDown(KeyCode.Joystick1Button7))
                KeyDown("Joystick1Button7");
            if (Input.GetKeyDown(KeyCode.Joystick1Button8))
                KeyDown("Joystick1Button8");
            if (Input.GetKeyDown(KeyCode.Joystick1Button9))
                KeyDown("Joystick1Button9");
            if (Input.GetKeyDown(KeyCode.Joystick1Button10))
                KeyDown("Joystick1Button10");
            if (Input.GetKeyDown(KeyCode.Joystick1Button11))
                KeyDown("Joystick1Button11");
            if (Input.GetKeyDown(KeyCode.Joystick1Button12))
                KeyDown("Joystick1Button12");
            if (Input.GetKeyDown(KeyCode.Joystick1Button13))
                KeyDown("Joystick1Button13");
            if (Input.GetKeyDown(KeyCode.Joystick1Button14))
                KeyDown("Joystick1Button14");
            if (Input.GetKeyDown(KeyCode.Joystick1Button15))
                KeyDown("Joystick1Button15");
            if (Input.GetKeyDown(KeyCode.Joystick1Button16))
                KeyDown("Joystick1Button16");
            if (Input.GetKeyDown(KeyCode.Joystick1Button17))
                KeyDown("Joystick1Button17");
            if (Input.GetKeyDown(KeyCode.Joystick1Button18))
                KeyDown("Joystick1Button18");
            if (Input.GetKeyDown(KeyCode.Joystick1Button19))
                KeyDown("Joystick1Button19");
            if (Input.GetKeyDown(KeyCode.Joystick2Button0))
                KeyDown("Joystick2Button0");
            if (Input.GetKeyDown(KeyCode.Joystick2Button1))
                KeyDown("Joystick2Button1");
            if (Input.GetKeyDown(KeyCode.Joystick2Button2))
                KeyDown("Joystick2Button2");
            if (Input.GetKeyDown(KeyCode.Joystick2Button3))
                KeyDown("Joystick2Button3");
            if (Input.GetKeyDown(KeyCode.Joystick2Button4))
                KeyDown("Joystick2Button4");
            if (Input.GetKeyDown(KeyCode.Joystick2Button5))
                KeyDown("Joystick2Button5");
            if (Input.GetKeyDown(KeyCode.Joystick2Button6))
                KeyDown("Joystick2Button6");
            if (Input.GetKeyDown(KeyCode.Joystick2Button7))
                KeyDown("Joystick2Button7");
            if (Input.GetKeyDown(KeyCode.Joystick2Button8))
                KeyDown("Joystick2Button8");
            if (Input.GetKeyDown(KeyCode.Joystick2Button9))
                KeyDown("Joystick2Button9");
            if (Input.GetKeyDown(KeyCode.Joystick2Button10))
                KeyDown("Joystick2Button10");
            if (Input.GetKeyDown(KeyCode.Joystick2Button11))
                KeyDown("Joystick2Button11");
            if (Input.GetKeyDown(KeyCode.Joystick2Button12))
                KeyDown("Joystick2Button12");
            if (Input.GetKeyDown(KeyCode.Joystick2Button13))
                KeyDown("Joystick2Button13");
            if (Input.GetKeyDown(KeyCode.Joystick2Button14))
                KeyDown("Joystick2Button14");
            if (Input.GetKeyDown(KeyCode.Joystick2Button15))
                KeyDown("Joystick2Button15");
            if (Input.GetKeyDown(KeyCode.Joystick2Button16))
                KeyDown("Joystick2Button16");
            if (Input.GetKeyDown(KeyCode.Joystick2Button17))
                KeyDown("Joystick2Button17");
            if (Input.GetKeyDown(KeyCode.Joystick2Button18))
                KeyDown("Joystick2Button18");
            if (Input.GetKeyDown(KeyCode.Joystick2Button19))
                KeyDown("Joystick2Button19");
            if (Input.GetKeyDown(KeyCode.Joystick3Button0))
                KeyDown("Joystick3Button0");
            if (Input.GetKeyDown(KeyCode.Joystick3Button1))
                KeyDown("Joystick3Button1");
            if (Input.GetKeyDown(KeyCode.Joystick3Button2))
                KeyDown("Joystick3Button2");
            if (Input.GetKeyDown(KeyCode.Joystick3Button3))
                KeyDown("Joystick3Button3");
            if (Input.GetKeyDown(KeyCode.Joystick3Button4))
                KeyDown("Joystick3Button4");
            if (Input.GetKeyDown(KeyCode.Joystick3Button5))
                KeyDown("Joystick3Button5");
            if (Input.GetKeyDown(KeyCode.Joystick3Button6))
                KeyDown("Joystick3Button6");
            if (Input.GetKeyDown(KeyCode.Joystick3Button7))
                KeyDown("Joystick3Button7");
            if (Input.GetKeyDown(KeyCode.Joystick3Button8))
                KeyDown("Joystick3Button8");
            if (Input.GetKeyDown(KeyCode.Joystick3Button9))
                KeyDown("Joystick3Button9");
            if (Input.GetKeyDown(KeyCode.Joystick3Button10))
                KeyDown("Joystick3Button10");
            if (Input.GetKeyDown(KeyCode.Joystick3Button11))
                KeyDown("Joystick3Button11");
            if (Input.GetKeyDown(KeyCode.Joystick3Button12))
                KeyDown("Joystick3Button12");
            if (Input.GetKeyDown(KeyCode.Joystick3Button13))
                KeyDown("Joystick3Button13");
            if (Input.GetKeyDown(KeyCode.Joystick3Button14))
                KeyDown("Joystick3Button14");
            if (Input.GetKeyDown(KeyCode.Joystick3Button15))
                KeyDown("Joystick3Button15");
            if (Input.GetKeyDown(KeyCode.Joystick3Button16))
                KeyDown("Joystick3Button16");
            if (Input.GetKeyDown(KeyCode.Joystick3Button17))
                KeyDown("Joystick3Button17");
            if (Input.GetKeyDown(KeyCode.Joystick3Button18))
                KeyDown("Joystick3Button18");
            if (Input.GetKeyDown(KeyCode.Joystick3Button19))
                KeyDown("Joystick3Button19");
            if (Input.GetKeyDown(KeyCode.Joystick4Button0))
                KeyDown("Joystick4Button0");
            if (Input.GetKeyDown(KeyCode.Joystick4Button1))
                KeyDown("Joystick4Button1");
            if (Input.GetKeyDown(KeyCode.Joystick4Button2))
                KeyDown("Joystick4Button2");
            if (Input.GetKeyDown(KeyCode.Joystick4Button3))
                KeyDown("Joystick4Button3");
            if (Input.GetKeyDown(KeyCode.Joystick4Button4))
                KeyDown("Joystick4Button4");
            if (Input.GetKeyDown(KeyCode.Joystick4Button5))
                KeyDown("Joystick4Button5");
            if (Input.GetKeyDown(KeyCode.Joystick4Button6))
                KeyDown("Joystick4Button6");
            if (Input.GetKeyDown(KeyCode.Joystick4Button7))
                KeyDown("Joystick4Button7");
            if (Input.GetKeyDown(KeyCode.Joystick4Button8))
                KeyDown("Joystick4Button8");
            if (Input.GetKeyDown(KeyCode.Joystick4Button9))
                KeyDown("Joystick4Button9");
            if (Input.GetKeyDown(KeyCode.Joystick4Button10))
                KeyDown("Joystick4Button10");
            if (Input.GetKeyDown(KeyCode.Joystick4Button11))
                KeyDown("Joystick4Button11");
            if (Input.GetKeyDown(KeyCode.Joystick4Button12))
                KeyDown("Joystick4Button12");
            if (Input.GetKeyDown(KeyCode.Joystick4Button13))
                KeyDown("Joystick4Button13");
            if (Input.GetKeyDown(KeyCode.Joystick4Button14))
                KeyDown("Joystick4Button14");
            if (Input.GetKeyDown(KeyCode.Joystick4Button15))
                KeyDown("Joystick4Button15");
            if (Input.GetKeyDown(KeyCode.Joystick4Button16))
                KeyDown("Joystick4Button16");
            if (Input.GetKeyDown(KeyCode.Joystick4Button17))
                KeyDown("Joystick4Button17");
            if (Input.GetKeyDown(KeyCode.Joystick4Button18))
                KeyDown("Joystick4Button18");
            if (Input.GetKeyDown(KeyCode.Joystick4Button19))
                KeyDown("Joystick4Button19");
            if (Input.GetKeyDown(KeyCode.Joystick5Button0))
                KeyDown("Joystick5Button0");
            if (Input.GetKeyDown(KeyCode.Joystick5Button1))
                KeyDown("Joystick5Button1");
            if (Input.GetKeyDown(KeyCode.Joystick5Button2))
                KeyDown("Joystick5Button2");
            if (Input.GetKeyDown(KeyCode.Joystick5Button3))
                KeyDown("Joystick5Button3");
            if (Input.GetKeyDown(KeyCode.Joystick5Button4))
                KeyDown("Joystick5Button4");
            if (Input.GetKeyDown(KeyCode.Joystick5Button5))
                KeyDown("Joystick5Button5");
            if (Input.GetKeyDown(KeyCode.Joystick5Button6))
                KeyDown("Joystick5Button6");
            if (Input.GetKeyDown(KeyCode.Joystick5Button7))
                KeyDown("Joystick5Button7");
            if (Input.GetKeyDown(KeyCode.Joystick5Button8))
                KeyDown("Joystick5Button8");
            if (Input.GetKeyDown(KeyCode.Joystick5Button9))
                KeyDown("Joystick5Button9");
            if (Input.GetKeyDown(KeyCode.Joystick5Button10))
                KeyDown("Joystick5Button10");
            if (Input.GetKeyDown(KeyCode.Joystick5Button11))
                KeyDown("Joystick5Button11");
            if (Input.GetKeyDown(KeyCode.Joystick5Button12))
                KeyDown("Joystick5Button12");
            if (Input.GetKeyDown(KeyCode.Joystick5Button13))
                KeyDown("Joystick5Button13");
            if (Input.GetKeyDown(KeyCode.Joystick5Button14))
                KeyDown("Joystick5Button14");
            if (Input.GetKeyDown(KeyCode.Joystick5Button15))
                KeyDown("Joystick5Button15");
            if (Input.GetKeyDown(KeyCode.Joystick5Button16))
                KeyDown("Joystick5Button16");
            if (Input.GetKeyDown(KeyCode.Joystick5Button17))
                KeyDown("Joystick5Button17");
            if (Input.GetKeyDown(KeyCode.Joystick5Button18))
                KeyDown("Joystick5Button18");
            if (Input.GetKeyDown(KeyCode.Joystick5Button19))
                KeyDown("Joystick5Button19");
            if (Input.GetKeyDown(KeyCode.Joystick6Button0))
                KeyDown("Joystick6Button0");
            if (Input.GetKeyDown(KeyCode.Joystick6Button1))
                KeyDown("Joystick6Button1");
            if (Input.GetKeyDown(KeyCode.Joystick6Button2))
                KeyDown("Joystick6Button2");
            if (Input.GetKeyDown(KeyCode.Joystick6Button3))
                KeyDown("Joystick6Button3");
            if (Input.GetKeyDown(KeyCode.Joystick6Button4))
                KeyDown("Joystick6Button4");
            if (Input.GetKeyDown(KeyCode.Joystick6Button5))
                KeyDown("Joystick6Button5");
            if (Input.GetKeyDown(KeyCode.Joystick6Button6))
                KeyDown("Joystick6Button6");
            if (Input.GetKeyDown(KeyCode.Joystick6Button7))
                KeyDown("Joystick6Button7");
            if (Input.GetKeyDown(KeyCode.Joystick6Button8))
                KeyDown("Joystick6Button8");
            if (Input.GetKeyDown(KeyCode.Joystick6Button9))
                KeyDown("Joystick6Button9");
            if (Input.GetKeyDown(KeyCode.Joystick6Button10))
                KeyDown("Joystick6Button10");
            if (Input.GetKeyDown(KeyCode.Joystick6Button11))
                KeyDown("Joystick6Button11");
            if (Input.GetKeyDown(KeyCode.Joystick6Button12))
                KeyDown("Joystick6Button12");
            if (Input.GetKeyDown(KeyCode.Joystick6Button13))
                KeyDown("Joystick6Button13");
            if (Input.GetKeyDown(KeyCode.Joystick6Button14))
                KeyDown("Joystick6Button14");
            if (Input.GetKeyDown(KeyCode.Joystick6Button15))
                KeyDown("Joystick6Button15");
            if (Input.GetKeyDown(KeyCode.Joystick6Button16))
                KeyDown("Joystick6Button16");
            if (Input.GetKeyDown(KeyCode.Joystick6Button17))
                KeyDown("Joystick6Button17");
            if (Input.GetKeyDown(KeyCode.Joystick6Button18))
                KeyDown("Joystick6Button18");
            if (Input.GetKeyDown(KeyCode.Joystick6Button19))
                KeyDown("Joystick6Button19");
            if (Input.GetKeyDown(KeyCode.Joystick7Button0))
                KeyDown("Joystick7Button0");
            if (Input.GetKeyDown(KeyCode.Joystick7Button1))
                KeyDown("Joystick7Button1");
            if (Input.GetKeyDown(KeyCode.Joystick7Button2))
                KeyDown("Joystick7Button2");
            if (Input.GetKeyDown(KeyCode.Joystick7Button3))
                KeyDown("Joystick7Button3");
            if (Input.GetKeyDown(KeyCode.Joystick7Button4))
                KeyDown("Joystick7Button4");
            if (Input.GetKeyDown(KeyCode.Joystick7Button5))
                KeyDown("Joystick7Button5");
            if (Input.GetKeyDown(KeyCode.Joystick7Button6))
                KeyDown("Joystick7Button6");
            if (Input.GetKeyDown(KeyCode.Joystick7Button7))
                KeyDown("Joystick7Button7");
            if (Input.GetKeyDown(KeyCode.Joystick7Button8))
                KeyDown("Joystick7Button8");
            if (Input.GetKeyDown(KeyCode.Joystick7Button9))
                KeyDown("Joystick7Button9");
            if (Input.GetKeyDown(KeyCode.Joystick7Button10))
                KeyDown("Joystick7Button10");
            if (Input.GetKeyDown(KeyCode.Joystick7Button11))
                KeyDown("Joystick7Button11");
            if (Input.GetKeyDown(KeyCode.Joystick7Button12))
                KeyDown("Joystick7Button12");
            if (Input.GetKeyDown(KeyCode.Joystick7Button13))
                KeyDown("Joystick7Button13");
            if (Input.GetKeyDown(KeyCode.Joystick7Button14))
                KeyDown("Joystick7Button14");
            if (Input.GetKeyDown(KeyCode.Joystick7Button15))
                KeyDown("Joystick7Button15");
            if (Input.GetKeyDown(KeyCode.Joystick7Button16))
                KeyDown("Joystick7Button16");
            if (Input.GetKeyDown(KeyCode.Joystick7Button17))
                KeyDown("Joystick7Button17");
            if (Input.GetKeyDown(KeyCode.Joystick7Button18))
                KeyDown("Joystick7Button18");
            if (Input.GetKeyDown(KeyCode.Joystick7Button19))
                KeyDown("Joystick7Button19");
            if (Input.GetKeyDown(KeyCode.Joystick8Button0))
                KeyDown("Joystick8Button0");
            if (Input.GetKeyDown(KeyCode.Joystick8Button1))
                KeyDown("Joystick8Button1");
            if (Input.GetKeyDown(KeyCode.Joystick8Button2))
                KeyDown("Joystick8Button2");
            if (Input.GetKeyDown(KeyCode.Joystick8Button3))
                KeyDown("Joystick8Button3");
            if (Input.GetKeyDown(KeyCode.Joystick8Button4))
                KeyDown("Joystick8Button4");
            if (Input.GetKeyDown(KeyCode.Joystick8Button5))
                KeyDown("Joystick8Button5");
            if (Input.GetKeyDown(KeyCode.Joystick8Button6))
                KeyDown("Joystick8Button6");
            if (Input.GetKeyDown(KeyCode.Joystick8Button7))
                KeyDown("Joystick8Button7");
            if (Input.GetKeyDown(KeyCode.Joystick8Button8))
                KeyDown("Joystick8Button8");
            if (Input.GetKeyDown(KeyCode.Joystick8Button9))
                KeyDown("Joystick8Button9");
            if (Input.GetKeyDown(KeyCode.Joystick8Button10))
                KeyDown("Joystick8Button10");
            if (Input.GetKeyDown(KeyCode.Joystick8Button11))
                KeyDown("Joystick8Button11");
            if (Input.GetKeyDown(KeyCode.Joystick8Button12))
                KeyDown("Joystick8Button12");
            if (Input.GetKeyDown(KeyCode.Joystick8Button13))
                KeyDown("Joystick8Button13");
            if (Input.GetKeyDown(KeyCode.Joystick8Button14))
                KeyDown("Joystick8Button14");
            if (Input.GetKeyDown(KeyCode.Joystick8Button15))
                KeyDown("Joystick8Button15");
            if (Input.GetKeyDown(KeyCode.Joystick8Button16))
                KeyDown("Joystick8Button16");
            if (Input.GetKeyDown(KeyCode.Joystick8Button17))
                KeyDown("Joystick8Button17");
            if (Input.GetKeyDown(KeyCode.Joystick8Button18))
                KeyDown("Joystick8Button18");
            if (Input.GetKeyDown(KeyCode.Joystick8Button19))
                KeyDown("Joystick8Button19");
        }

        private void KeyDown(string name)
        {
            // NOTE: there is a smart way to do this by purely storing the length of the lines and then using substrings... but who cares!
            for (int i = 0; i < 9; i++)
                lines[i] = lines[i + 1];
            lines[9] = name;
            tmpText.text = string.Join("\n", lines, 0, 10);
            Debug.Log(name);
        }
    }
}
