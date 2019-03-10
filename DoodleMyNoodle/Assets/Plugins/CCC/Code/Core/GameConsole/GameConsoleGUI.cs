using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Internals.GameConsoleInterals
{
    public class GameConsoleGUI : MonoBehaviour, IGameConsoleUI
    {
        [ConfigVar(name: "console.alpha", defaultValue: "0.9", description: "Alpha of console", flags: ConfigVarFlag.Save)]
        static ConfigVar consoleAlpha;

        void Awake()
        {
            input_field.onEndEdit.AddListener(OnSubmit);
        }

        public void Init()
        {
            buildIdText.text = Application.version + " (" + Application.unityVersion + ")";
        }

        public void Shutdown()
        {

        }

        public void OutputString(string s)
        {
            m_Lines.Add(s);
            var count = Mathf.Min(100, m_Lines.Count);
            var start = m_Lines.Count - count;
            text_area.text = string.Join("\n", m_Lines.GetRange(start, count).ToArray());
        }

        public bool IsOpen()
        {
            return panel.gameObject.activeSelf;
        }

        public void SetOpen(bool open)
        {
            // Game.Input.SetBlock(Game.Input.Blocker.Console, open);

            panel.gameObject.SetActive(open);
            if (open)
            {
                input_field.ActivateInputField();
            }
        }

        public void ConsoleUpdate()
        {
            if (Input.GetKeyDown(toggle_console_key) || Input.GetKeyDown(KeyCode.Backslash))
                SetOpen(!IsOpen());

            if (!IsOpen())
                return;

            var c = text_area_background.color;
            c.a = Mathf.Clamp01(consoleAlpha.FloatValue);
            text_area_background.color = c;

            // This is to prevent clicks outside input field from removing focus
            input_field.ActivateInputField();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (input_field.caretPosition == input_field.text.Length && input_field.text.Length > 0)
                {
                    var res = GameConsole.TabComplete(input_field.text);
                    input_field.text = res;
                    input_field.caretPosition = res.Length;
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                input_field.text = GameConsole.HistoryUp(input_field.text);
                m_WantedCaretPosition = input_field.text.Length;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                input_field.text = GameConsole.HistoryDown();
                input_field.caretPosition = input_field.text.Length;
            }
        }

        public void ConsoleLateUpdate()
        {
            // This has to happen here because keys like KeyUp will navigate the caret
            // int the UI event handling which runs between Update and LateUpdate
            if (m_WantedCaretPosition > -1)
            {
                input_field.caretPosition = m_WantedCaretPosition;
                m_WantedCaretPosition = -1;
            }
        }


        void OnSubmit(string value)
        {
            // Only react to this if enter was actually pressed. Submit can also happen by mouseclicks.
            if (!Input.GetKey(KeyCode.Return) && !Input.GetKey(KeyCode.KeypadEnter))
                return;

            input_field.text = "";
            input_field.ActivateInputField();

            GameConsole.EnqueueCommand(value);
        }

        List<string> m_Lines = new List<string>();
        int m_WantedCaretPosition = -1;

        [SerializeField] Transform panel;
        [SerializeField] InputField input_field;
        [SerializeField] Text text_area;
        [SerializeField] Image text_area_background;
        [SerializeField] KeyCode toggle_console_key;
        [SerializeField] Text buildIdText;

    }
}