using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace CmmInterpreter.Util
{
    public class FileHandler
    {
        public string FileName { get; set; }
        public bool IsSaved { get; set; }
        public string OpenFile()
        {
            string text = null;
            var openFileDialog = new OpenFileDialog
            {
                Title = "选择文件",
                Filter = "cmm文件|*.cmm|文本文件|*.txt|所有文件|*.*",
                FileName = string.Empty,
                FilterIndex = 1,
                Multiselect = false,
                RestoreDirectory = true,
            };
            if (openFileDialog.ShowDialog () == true)
            {
                var streamReader = new StreamReader (openFileDialog.FileName, Encoding.UTF8);
                text = streamReader.ReadToEnd();
                streamReader.Close();
            }
            var fullPath = openFileDialog.FileName;
            FileName = Path.GetFileName(fullPath);
            IsSaved = true;
            return text;
        }

        public void SaveFile()
        {

        }
    }
}
