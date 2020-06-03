using BZWHLLLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListInputFilesScript
{
    class NStarController
    {
        private string username;
        private string password;
        public WhllObj bzhao = null;
        private string sessionName = null;

        public NStarController(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        internal bool Login()
        {
            bzhao = new WhllObj();
            sessionName = bzhao.NewSession(1, "Session5.zmd");
            bzhao.Connect(sessionName);
            bzhao.SetCursor(42, 6);
            bzhao.SendKey(username);
            bzhao.SendKey("<Tab>");
            bzhao.SendKey(password);
            bzhao.SendKey("<Enter>");
            bzhao.WaitReady(10, 1);
            return !bzhao.PSGetText(80 * 43, 1).Contains("To reset your password");
        }

        internal void Close()
        {
            if (bzhao != null && sessionName != null) bzhao.DeleteSession(sessionName);
        }

        internal String GetString(int row, int col, int len)
        {
            Object string1 = null;
            bzhao.ReadScreen(out string1, len, row, col);

            return (string)string1;
            



            


           
        }

        public string[] PrintScreen()
        {
            string rawText = bzhao.PSGetText(80 * 43, 1);
            string[] screen = new string[43];
            for (int i = 0; i < 43; i++)
            {
                screen[i] = rawText.Substring(80 * i, 80);
            }
            return screen;
        }

        internal void Reset()
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {

            }
            Login();
            bzhao.SendKey("<PF6>");
            bzhao.WaitReady(10, 3);
            bzhao.SendKey("rcse");
            bzhao.SendKey("<Enter>");
            bzhao.WaitReady(10, 1);
            bzhao.SendKey("<Enter>");
            bzhao.WaitReady(10, 1);
        }

        internal List<string> PrintCode()
        {
            string rawText = bzhao.PSGetText(80 * 43, 1);
            string[] screen = new string[43];
            for (int i = 0; i < 43; i++)
            {
                screen[i] = rawText.Substring(80 * i, 80);
            }
            return screen.Skip(3).Take(39).ToList();
        }
    }
}
