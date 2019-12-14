using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace CmmInterpreter.Util
{
    /// <summary>
    /// 文件工具类，用来操作文件
    /// </summary>
    public class FileHandler
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool IsSaved { get; set; }
        public string OpenFile()
        {
            try
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
                if (openFileDialog.ShowDialog() == true)
                {
                    var streamReader = new StreamReader(openFileDialog.FileName, Encoding.UTF8);
                    text = streamReader.ReadToEnd();
                    streamReader.Close();
                }

                FilePath = openFileDialog.FileName;
                FileName = Path.GetFileName(FilePath);
                IsSaved = true;
                return text;
            }
            catch (IOException)
            {
                MessageBox.Show("打开失败！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            
        }

        public string OpenDir()
        {
            var dialog = new FolderBrowserDialog {Description = "请选择文件路径"};
            string path = null;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path = dialog.SelectedPath;
            }

            return path;
        }

        public void SaveFile(string text)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "保存文件",
                    Filter = "cmm文件|*.cmm|文本文件|*.txt|所有文件|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = true,
                };
                if (string.IsNullOrEmpty(FilePath))
                {
                    var result = saveFileDialog.ShowDialog();
                    if (result != true)
                    {
                        IsSaved = false;
                        return;
                    }
                    FilePath = saveFileDialog.FileName;
                }

                if (FilePath == "")
                {
                    return;
                }
                var streamWriter = new StreamWriter(FilePath, false, Encoding.UTF8);
                streamWriter.WriteLine(text);
                streamWriter.Close();
                FileName = Path.GetFileName(FilePath);
                IsSaved = true;
            }
            catch (IOException)
            {
               MessageBox.Show("保存失败！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
        public void SaveFileAs(string text)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "另存为文件",
                    Filter = "cmm文件|*.cmm|文本文件|*.txt|所有文件|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = true,
                };
                if (saveFileDialog.ShowDialog() != true) return;
                var streamWriter = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8);
                streamWriter.WriteLine(text);
                streamWriter.Close();
            }
            catch (IOException)
            {
                MessageBox.Show("保存失败！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
