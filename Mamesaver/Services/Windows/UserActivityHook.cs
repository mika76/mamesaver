using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mamesaver.Services.Windows
{
    public interface IActivityHook
    {
        /// <summary>
        ///     Occurs when the user moves the mouse, presses any mouse button or scrolls the wheel
        /// </summary>
        event MouseEventHandler OnMouseActivity;

        /// <summary>
        ///     Occurs when the user presses a key
        /// </summary>
        event KeyEventHandler KeyDown;

        /// <summary>
        ///     Occurs when the user presses and releases
        /// </summary>
        event KeyPressEventHandler KeyPress;

        /// <summary>
        ///     Occurs when the user releases a key
        /// </summary>
        event KeyEventHandler KeyUp;

        /// <summary>
        ///     Installs both mouse and keyboard hooks and starts rasing events
        /// </summary>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        void Start();
    }

    /// <summary>
    ///     This class allows you to tap keyboard and mouse and / or to detect their activity even when an
    ///     application runes in background or does not have any user interface at all. This class raises
    ///     common .NET events with KeyEventArgs and MouseEventArgs so you can easily retrive any information you need.
    /// </summary>
    /// <remarks>
    ///     From http://www.codeproject.com/csharp/globalhook.asp
    ///     By George Mamaladze
    ///</remarks>
    public class UserActivityHook : IActivityHook
    {
        #region Windows structure definitions

        /// <summary>
        ///     The POINT structure defines the x- and y- coordinates of a point.
        /// </summary>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/gdi/rectangl_0tiq.asp
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private class Point
        {
            /// <summary>
            ///     Specifies the x-coordinate of the point.
            /// </summary>
            public int x;

            /// <summary>
            ///     Specifies the y-coordinate of the point.
            /// </summary>
            public int y;
        }

        /// <summary>
        ///     The MSLLHOOKSTRUCT structure contains information about a low-level keyboard input event.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private class MouseLlHookStruct
        {
            /// <summary>
            ///     Specifies a POINT structure that contains the x- and y-coordinates of the cursor, in screen coordinates.
            /// </summary>
            public Point pt;

            /// <summary>
            ///     If the message is WM_MOUSEWHEEL, the high-order word of this member is the wheel delta.
            ///     The low-order word is reserved. A positive value indicates that the wheel was rotated forward,
            ///     away from the user; a negative value indicates that the wheel was rotated backward, toward the user.
            ///     One wheel click is defined as WHEEL_DELTA, which is 120.
            ///     If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP,
            ///     or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released,
            ///     and the low-order word is reserved. This value can be one or more of the following values. Otherwise, mouseData is
            ///     not used.
            ///     XBUTTON1
            ///     The first X button was pressed or released.
            ///     XBUTTON2
            ///     The second X button was pressed or released.
            /// </summary>
            public int mouseData;

            /// <summary>
            ///     Specifies the event-injected flag. An application can use the following value to test the mouse flags. Value
            ///     Purpose
            ///     LLMHF_INJECTED Test the event-injected flag.
            ///     0
            ///     Specifies whether the event was injected. The value is 1 if the event was injected; otherwise, it is 0.
            ///     1-15
            ///     Reserved.
            /// </summary>
            public int flags;

            /// <summary>
            ///     Specifies the time stamp for this message.
            /// </summary>
            public int time;

            /// <summary>
            ///     Specifies extra information associated with the message.
            /// </summary>
            public int dwExtraInfo;
        }


        /// <summary>
        ///     The KBDLLHOOKSTRUCT structure contains information about a low-level keyboard input event.
        /// </summary>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookstructures/cwpstruct.asp
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private class KeyboardHookStruct
        {
            /// <summary>
            ///     Specifies a virtual-key code. The code must be a value in the range 1 to 254.
            /// </summary>
            public int vkCode;

            /// <summary>
            ///     Specifies a hardware scan code for the key.
            /// </summary>
            public int scanCode;

            /// <summary>
            ///     Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
            /// </summary>
            public int flags;

            /// <summary>
            ///     Specifies the time stamp for this message.
            /// </summary>
            public int time;

            /// <summary>
            ///     Specifies extra information associated with the message.
            /// </summary>
            public int dwExtraInfo;
        }

        #endregion

        #region Windows function imports

        /// <summary>
        ///     The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain.
        ///     You would install a hook procedure to monitor the system for certain types of events. These events
        ///     are associated either with a specific thread or with all threads in the same desktop as the calling thread.
        /// </summary>
        /// <param name="idHook">
        ///     [in] Specifies the type of hook procedure to be installed. This parameter can be one of the following values.
        /// </param>
        /// <param name="lpfn">
        ///     [in] Pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a
        ///     thread created by a different process, the lpfn parameter must point to a hook procedure in a dynamic-link
        ///     library (DLL). Otherwise, lpfn can point to a hook procedure in the code associated with the current process.
        /// </param>
        /// <param name="hMod">
        ///     [in] Handle to the DLL containing the hook procedure pointed to by the lpfn parameter.
        ///     The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by
        ///     the current process and if the hook procedure is within the code associated with the current process.
        /// </param>
        /// <param name="dwThreadId">
        ///     [in] Specifies the identifier of the thread with which the hook procedure is to be associated.
        ///     If this parameter is zero, the hook procedure is associated with all existing threads running in the
        ///     same desktop as the calling thread.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is the handle to the hook procedure.
        ///     If the function fails, the return value is NULL. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int SetWindowsHookEx(
            int idHook,
            HookProc lpfn,
            IntPtr hMod,
            int dwThreadId);

        /// <summary>
        ///     The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx
        ///     function.
        /// </summary>
        /// <param name="idHook">
        ///     [in] Handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to
        ///     SetWindowsHookEx.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);

        /// <summary>
        ///     The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain.
        ///     A hook procedure can call this function either before or after processing the hook information.
        /// </summary>
        /// <param name="idHook">Ignored.</param>
        /// <param name="nCode">
        ///     [in] Specifies the hook code passed to the current hook procedure.
        ///     The next hook procedure uses this code to determine how to process the hook information.
        /// </param>
        /// <param name="wParam">
        ///     [in] Specifies the wParam value passed to the current hook procedure.
        ///     The meaning of this parameter depends on the type of hook associated with the current hook chain.
        /// </param>
        /// <param name="lParam">
        ///     [in] Specifies the lParam value passed to the current hook procedure.
        ///     The meaning of this parameter depends on the type of hook associated with the current hook chain.
        /// </param>
        /// <returns>
        ///     This value is returned by the next hook procedure in the chain.
        ///     The current hook procedure must also return this value. The meaning of the return value depends on the hook type.
        ///     For more information, see the descriptions of the individual hook procedures.
        /// </returns>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(
            int idHook,
            int nCode,
            int wParam,
            IntPtr lParam);

        /// <summary>
        ///     The CallWndProc hook procedure is an application-defined or library-defined callback
        ///     function used with the SetWindowsHookEx function. The HOOKPROC type defines a pointer
        ///     to this callback function. CallWndProc is a placeholder for the application-defined
        ///     or library-defined function name.
        /// </summary>
        /// <param name="nCode">
        ///     [in] Specifies whether the hook procedure must process the message.
        ///     If nCode is HC_ACTION, the hook procedure must process the message.
        ///     If nCode is less than zero, the hook procedure must pass the message to the
        ///     CallNextHookEx function without further processing and must return the
        ///     value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        ///     [in] Specifies whether the message was sent by the current thread.
        ///     If the message was sent by the current thread, it is nonzero; otherwise, it is zero.
        /// </param>
        /// <param name="lParam">
        ///     [in] Pointer to a CWPSTRUCT structure that contains details about the message.
        /// </param>
        /// <returns>
        ///     If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx.
        ///     If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx
        ///     and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC
        ///     hooks will not receive hook notifications and may behave incorrectly as a result. If the hook
        ///     procedure does not call CallNextHookEx, the return value should be zero.
        /// </returns>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/callwndproc.asp
        /// </remarks>
        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        /// <summary>
        ///     The ToAscii function translates the specified virtual-key code and keyboard
        ///     state to the corresponding character or characters. The function translates the code
        ///     using the input language and physical keyboard layout identified by the keyboard layout handle.
        /// </summary>
        /// <param name="uVirtKey">
        ///     [in] Specifies the virtual-key code to be translated.
        /// </param>
        /// <param name="uScanCode">
        ///     [in] Specifies the hardware scan code of the key to be translated.
        ///     The high-order bit of this value is set if the key is up (not pressed).
        /// </param>
        /// <param name="lpbKeyState">
        ///     [in] Pointer to a 256-byte array that contains the current keyboard state.
        ///     Each element (byte) in the array contains the state of one key.
        ///     If the high-order bit of a byte is set, the key is down (pressed).
        ///     The low bit, if set, indicates that the key is toggled on. In this function,
        ///     only the toggle bit of the CAPS LOCK key is relevant. The toggle state
        ///     of the NUM LOCK and SCROLL LOCK keys is ignored.
        /// </param>
        /// <param name="lpwTransKey">
        ///     [out] Pointer to the buffer that receives the translated character or characters.
        /// </param>
        /// <param name="fuState">
        ///     [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise.
        /// </param>
        /// <returns>
        ///     If the specified key is a dead key, the return value is negative. Otherwise, it is one of the following values.
        ///     Value Meaning
        ///     0 The specified virtual key has no translation for the current state of the keyboard.
        ///     1 One character was copied to the buffer.
        ///     2 Two characters were copied to the buffer. This usually happens when a dead-key character
        ///     (accent or diacritic) stored in the keyboard layout cannot be composed with the specified
        ///     virtual key to form a single character.
        /// </returns>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
        /// </remarks>
        [DllImport("user32")]
        private static extern int ToAscii(
            int uVirtKey,
            int uScanCode,
            byte[] lpbKeyState,
            byte[] lpwTransKey,
            int fuState);

        /// <summary>
        ///     The GetKeyboardState function copies the status of the 256 virtual keys to the
        ///     specified buffer.
        /// </summary>
        /// <param name="pbKeyState">
        ///     [in] Pointer to a 256-byte array that contains keyboard key states.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
        /// </remarks>
        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        #endregion

        #region Windows constants

        //values from Winuser.h in Microsoft SDK.
        /// <summary>
        ///     Windows NT/2000/XP: Installs a hook procedure that monitors low-level mouse input events.
        /// </summary>
        private const int WhMouseLl = 14;

        /// <summary>
        ///     Windows NT/2000/XP: Installs a hook procedure that monitors low-level keyboard  input events.
        /// </summary>
        private const int WhKeyboardLl = 13;

        /// <summary>
        ///     The WM_LBUTTONDOWN message is posted when the user presses the left mouse button
        /// </summary>
        private const int WmLbuttondown = 0x201;

        /// <summary>
        ///     The WM_RBUTTONDOWN message is posted when the user presses the right mouse button
        /// </summary>
        private const int WmRbuttondown = 0x204;

        /// <summary>
        ///     The WM_LBUTTONDBLCLK message is posted when the user double-clicks the left mouse button
        /// </summary>
        private const int WmLbuttondblclk = 0x203;

        /// <summary>
        ///     The WM_RBUTTONDBLCLK message is posted when the user double-clicks the right mouse button
        /// </summary>
        private const int WmRbuttondblclk = 0x206;

        /// <summary>
        ///     The WM_MOUSEWHEEL message is posted when the user presses the mouse wheel.
        /// </summary>
        private const int WmMousewheel = 0x020A;

        /// <summary>
        ///     The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem
        ///     key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed.
        /// </summary>
        private const int WmKeydown = 0x100;

        /// <summary>
        ///     The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem
        ///     key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed,
        ///     or a keyboard key that is pressed when a window has the keyboard focus.
        /// </summary>
        private const int WmKeyup = 0x101;

        /// <summary>
        ///     The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user
        ///     presses the F10 key (which activates the menu bar) or holds down the ALT key and then
        ///     presses another key. It also occurs when no window currently has the keyboard focus;
        ///     in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that
        ///     receives the message can distinguish between these two contexts by checking the context
        ///     code in the lParam parameter.
        /// </summary>
        private const int WmSyskeydown = 0x104;

        /// <summary>
        ///     The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user
        ///     releases a key that was pressed while the ALT key was held down. It also occurs when no
        ///     window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent
        ///     to the active window. The window that receives the message can distinguish between
        ///     these two contexts by checking the context code in the lParam parameter.
        /// </summary>
        private const int WmSyskeyup = 0x105;

        private const byte VkShift = 0x10;
        private const byte VkCapital = 0x14;

        #endregion

        /// <summary>
        ///     Creates an instance of UserActivityHook object and sets mouse and keyboard hooks.
        /// </summary>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public UserActivityHook()
        {
            Start();
        }

        /// <summary>
        ///     Creates an instance of UserActivityHook object and installs both or one of mouse and/or keyboard hooks and starts
        ///     raising events
        /// </summary>
        /// <param name="installMouseHook"><b>true</b> if mouse events must be monitored</param>
        /// <param name="installKeyboardHook"><b>true</b> if keyboard events must be monitored</param>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        /// <remarks>
        ///     To create an instance without installing hooks call new UserActivityHook(false, false)
        /// </remarks>
        public UserActivityHook(bool installMouseHook, bool installKeyboardHook)
        {
            Start(installMouseHook, installKeyboardHook);
        }

        /// <summary>
        ///     Destruction.
        /// </summary>
        ~UserActivityHook()
        {
            //uninstall hooks and do not throw exceptions
            Stop(true, true, false);
        }

        /// <summary>
        ///     Occurs when the user moves the mouse, presses any mouse button or scrolls the wheel
        /// </summary>
        public event MouseEventHandler OnMouseActivity;

        /// <summary>
        ///     Occurs when the user presses a key
        /// </summary>
        public event KeyEventHandler KeyDown;

        /// <summary>
        ///     Occurs when the user presses and releases
        /// </summary>
        public event KeyPressEventHandler KeyPress;

        /// <summary>
        ///     Occurs when the user releases a key
        /// </summary>
        public event KeyEventHandler KeyUp;


        /// <summary>
        ///     Stores the handle to the mouse hook procedure.
        /// </summary>
        private int _hMouseHook;

        /// <summary>
        ///     Stores the handle to the keyboard hook procedure.
        /// </summary>
        private int _hKeyboardHook;


        /// <summary>
        ///     Declare MouseHookProcedure as HookProc type.
        /// </summary>
        private static HookProc _mouseHookProcedure;

        /// <summary>
        ///     Declare KeyboardHookProcedure as HookProc type.
        /// </summary>
        private static HookProc _keyboardHookProcedure;


        /// <summary>
        /// Installs both mouse and keyboard hooks and starts rasing events
        /// </summary>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Start()
        {
            Start(true, true);
        }

        /// <summary>
        /// Installs both or one of mouse and/or keyboard hooks and starts rasing events
        /// </summary>
        /// <param name="installMouseHook"><b>true</b> if mouse events must be monitored</param>
        /// <param name="installKeyboardHook"><b>true</b> if keyboard events must be monitored</param>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Start(bool installMouseHook, bool installKeyboardHook)
        {
            // install Mouse hook only if it is not installed and must be installed
            if (_hMouseHook == 0 && installMouseHook)
            {
                // Create an instance of HookProc.
                _mouseHookProcedure = MouseHookProc;
                //install hook
                _hMouseHook = SetWindowsHookEx(
                    WhMouseLl,
                    _mouseHookProcedure,
                    Marshal.GetHINSTANCE(
                        Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);
                //If SetWindowsHookEx fails.
                if (_hMouseHook == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    var errorCode = Marshal.GetLastWin32Error();
                    //do cleanup
                    Stop(true, false, false);
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }

            // install Keyboard hook only if it is not installed and must be installed
            if (_hKeyboardHook == 0 && installKeyboardHook)
            {
                // Create an instance of HookProc.
                _keyboardHookProcedure = KeyboardHookProc;
                //install hook
                _hKeyboardHook = SetWindowsHookEx(
                    WhKeyboardLl,
                    _keyboardHookProcedure,
                    Marshal.GetHINSTANCE(
                    Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);
                //If SetWindowsHookEx fails.
                if (_hKeyboardHook == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    var errorCode = Marshal.GetLastWin32Error();
                    //do cleanup
                    Stop(false, true, false);
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        /// <summary>
        /// Stops monitoring both mouse and keyboard events and rasing events.
        /// </summary>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Stop()
        {
            Stop(true, true, true);
        }

        /// <summary>
        /// Stops monitoring both or one of mouse and/or keyboard events and rasing events.
        /// </summary>
        /// <param name="uninstallMouseHook"><b>true</b> if mouse hook must be uninstalled</param>
        /// <param name="uninstallKeyboardHook"><b>true</b> if keyboard hook must be uninstalled</param>
        /// <param name="throwExceptions"><b>true</b> if exceptions which occured during uninstalling must be thrown</param>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Stop(bool uninstallMouseHook, bool uninstallKeyboardHook, bool throwExceptions)
        {
            //if mouse hook set and must be uninstalled
            if (_hMouseHook != 0 && uninstallMouseHook)
            {
                //uninstall hook
                var retMouse = UnhookWindowsHookEx(_hMouseHook);
                //reset invalid handle
                _hMouseHook = 0;
                //if failed and exception must be thrown
                if (retMouse == 0 && throwExceptions)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    var errorCode = Marshal.GetLastWin32Error();
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }

            //if keyboard hook set and must be uninstalled
            if (_hKeyboardHook != 0 && uninstallKeyboardHook)
            {
                //uninstall hook
                var retKeyboard = UnhookWindowsHookEx(_hKeyboardHook);
                //reset invalid handle
                _hKeyboardHook = 0;
                //if failed and exception must be thrown
                if (retKeyboard == 0 && throwExceptions)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    var errorCode = Marshal.GetLastWin32Error();
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }


        /// <summary>
        /// A callback function which will be called every time a mouse activity detected.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        private int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            // if ok and someone listens to our events
            if (nCode >= 0 && OnMouseActivity != null)
            {
                //Marshall the data from callback.
                var mouseHookStruct = (MouseLlHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLlHookStruct));

                //detect button clicked
                var button = MouseButtons.None;
                short mouseDelta = 0;
                switch (wParam)
                {
                    case WmLbuttondown:
                        //case WM_LBUTTONUP: 
                        //case WM_LBUTTONDBLCLK: 
                        button = MouseButtons.Left;
                        break;
                    case WmRbuttondown:
                        //case WM_RBUTTONUP: 
                        //case WM_RBUTTONDBLCLK: 
                        button = MouseButtons.Right;
                        break;
                    case WmMousewheel:
                        //If the message is WM_MOUSEWHEEL, the high-order word of mouseData member is the wheel delta. 
                        //One wheel click is defined as WHEEL_DELTA, which is 120. 
                        //(value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
                        mouseDelta = (short)((mouseHookStruct.mouseData >> 16) & 0xffff);
                        //TODO: X BUTTONS (I havent them so was unable to test)
                        //If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP, 
                        //or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, 
                        //and the low-order word is reserved. This value can be one or more of the following values. 
                        //Otherwise, mouseData is not used. 
                        break;
                }

                //double clicks
                var clickCount = 0;
                if (button != MouseButtons.None)
                    if (wParam == WmLbuttondblclk || wParam == WmRbuttondblclk) clickCount = 2;
                    else clickCount = 1;

                //generate event 
                var e = new MouseEventArgs(
                                                   button,
                                                   clickCount,
                                                   mouseHookStruct.pt.x,
                                                   mouseHookStruct.pt.y,
                                                   mouseDelta);
                //raise it
                OnMouseActivity(this, e);
            }
            //call next hook
            return CallNextHookEx(_hMouseHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// A callback function which will be called every time a keyboard activity detected.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //indicates if any of underlaing events set e.Handled flag
            var handled = false;
            //it was ok and someone listens to events
            if (nCode >= 0 && (KeyDown != null || KeyUp != null || KeyPress != null))
            {
                //read structure KeyboardHookStruct at lParam
                var myKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                //raise KeyDown
                if (KeyDown != null && (wParam == WmKeydown || wParam == WmSyskeydown))
                {
                    var keyData = (Keys)myKeyboardHookStruct.vkCode;
                    var e = new KeyEventArgs(keyData);
                    KeyDown(this, e);
                    handled = e.Handled;
                }

                // raise KeyPress
                if (KeyPress != null && wParam == WmKeydown)
                {
                    var isDownShift = (GetKeyState(VkShift) & 0x80) == 0x80;
                    var isDownCapslock = GetKeyState(VkCapital) != 0;

                    var keyState = new byte[256];
                    GetKeyboardState(keyState);
                    var inBuffer = new byte[2];
                    if (ToAscii(myKeyboardHookStruct.vkCode,
                              myKeyboardHookStruct.scanCode,
                              keyState,
                              inBuffer,
                              myKeyboardHookStruct.flags) == 1)
                    {
                        var key = (char)inBuffer[0];
                        if (isDownCapslock ^ isDownShift && Char.IsLetter(key)) key = Char.ToUpper(key);
                        var e = new KeyPressEventArgs(key);
                        KeyPress(this, e);
                        handled = handled || e.Handled;
                    }
                }

                // raise KeyUp
                if (KeyUp != null && (wParam == WmKeyup || wParam == WmSyskeyup))
                {
                    var keyData = (Keys)myKeyboardHookStruct.vkCode;
                    var e = new KeyEventArgs(keyData);
                    KeyUp(this, e);
                    handled = handled || e.Handled;
                }

            }

            //if event handled in application do not handoff to other listeners
            if (handled)
                return 1;
            return CallNextHookEx(_hKeyboardHook, nCode, wParam, lParam);
        }
    }
}
