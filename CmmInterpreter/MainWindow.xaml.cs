using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CmmInterpreter.Exceptions;
using CmmInterpreter.Lexical_Analyzer;
using CmmInterpreter.Syntactic_Analyzer;
using Microsoft.Win32;
using CmmInterpreter.Util;

namespace CmmInterpreter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ListViewArea.Visibility = Visibility.Collapsed;
            TreeViewArea.Visibility = Visibility.Collapsed;
            Splitter.Visibility = Visibility.Collapsed;
            StopButton.IsEnabled = false;
            Title = "Cmm解释器 ——Untitled*";
        }
        private string FileName { get; set; }
        private bool IsSaved { get; set; }
        private Thread _thread;
        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fileHandler = new FileHandler();
            TextEditor.Text = fileHandler.OpenFile();
            FileName = fileHandler.FileName;
            IsSaved = fileHandler.IsSaved;
            Title = "Cmm解释器 ——" + FileName;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_thread != null && _thread.IsAlive)
            {
                _thread.Abort();
            }
                
            while (_thread != null && _thread.ThreadState != ThreadState.Aborted)
            {
                Thread.Sleep(100);
            }
            StopButton.IsEnabled = false;
        }

        private void OpenFileItem_Click(object sender, RoutedEventArgs e)
        {
            var fileHandler = new FileHandler();
            TextEditor.Text = fileHandler.OpenFile();
            FileName = fileHandler.FileName;
            IsSaved = fileHandler.IsSaved;
            Title = "Cmm解释器 ——" + FileName;
        }

        private void SaveFileItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveAsFileItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            if (IsSaved)
            {
                Application.Current.Shutdown();
            }
            var result = MessageBox.Show("This File is not saved, \nare you sure to exit?", "警告", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            IsSaved = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsSaved)
            {
                Application.Current.Shutdown();
            }
            else
            {
                var result = MessageBox.Show("This File is not saved, are you sure to exit?", "警告", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
           
        }

        private void RunLexerButton_Click(object sender, RoutedEventArgs e)
        {
            TreeViewArea.Visibility = Visibility.Collapsed;
            Splitter.Visibility = Visibility.Collapsed;
            var lexer = new Lexer();
            StopButton.IsEnabled = true;
            ResultTextBox.Focus();
            lexer.Chars = TextEditor.Text.ToCharArray();
            var threadStart = new ThreadStart(lexer.LexAnalyze);
            _thread = new Thread(threadStart) {IsBackground = true};
            _thread.Start();
            ResultTextBox.Text = "···········Lexer Analyzing...\n\n";
            while(_thread.IsAlive)
                Thread.Sleep(10);
            ResultTextBox.Text += lexer.PrintResult();
            ResultTextBox.Text += "\n···········Analysis done!";
            StopButton.IsEnabled = false;
            if (ListViewRadioButton.IsChecked != true || NoneRadioButton.IsChecked == true)
            {
                ListViewArea.Visibility = Visibility.Collapsed;
                Splitter.Visibility = Visibility.Collapsed;
                return;
            }
            ListViewArea.Visibility = Visibility.Visible;
            Splitter.Visibility = Visibility.Visible;
        }

        private void RunParserButton_Click(object sender, RoutedEventArgs e)
        {
            ListViewArea.Visibility = Visibility.Collapsed;
            Splitter.Visibility = Visibility.Collapsed;
            var parser = new Parser(); //此处可使用同步线程，不过为了简单起见，就不做同步线程了
            var lexer = new Lexer();
            StopButton.IsEnabled = true;
            ResultTextBox.Focus();
            lexer.Chars = TextEditor.Text.ToCharArray();
            lexer.LexAnalyze();
            ResultTextBox.Text = "···········Parser Analyzing...\n\n";
            if (lexer.ErrorInfoStrb.ToString().Length == 0)
            {
                parser.Tokens = lexer.Words;
                var threadStart = new ThreadStart(parser.SyntaxAnalyze);
                _thread = new Thread(threadStart) { IsBackground = true };
                _thread.Start();
                while (_thread.IsAlive)
                    Thread.Sleep(10);
                if (!parser.IsParseError && parser.SyntaxTree != null)
                    ResultTextBox.Text += TreeNode.PrintNode(parser.SyntaxTree, "");
                ResultTextBox.Text += parser.Error;
                ResultTextBox.Text += "\n···········Syntactic Analysis done!";
                StopButton.IsEnabled = false;
                if (TreeViewRadioButton.IsChecked != true || NoneRadioButton.IsChecked == true)
                {
                    TreeViewArea.Visibility = Visibility.Collapsed;
                    Splitter.Visibility = Visibility.Collapsed;
                    return;
                }
                TreeViewArea.Visibility = Visibility.Visible;
                Splitter.Visibility = Visibility.Visible;
            }
            else
            {
                ResultTextBox.Text += lexer.ErrorInfoStrb.ToString();
                ResultTextBox.Text += "\n···········Lexical Analysis Failed!\n";
                ResultTextBox.Text += "\n···········Syntactic Analysis Not Implemented!";
            }
            
        }

        private void RunInterpreterButton_Click(object sender, RoutedEventArgs e)
        {
            StopButton.IsEnabled = true;
            ResultTextBox.Focus();
        }

        private void OpenDirItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OpenDirButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
