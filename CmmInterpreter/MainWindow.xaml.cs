using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;
using CmmInterpreter.Exceptions;
using CmmInterpreter.Lexical_Analyzer;
using CmmInterpreter.Semantic_Analyzer;
using CmmInterpreter.Syntactic_Analyzer;
using Microsoft.Win32;
using CmmInterpreter.Util;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;

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
        private readonly FileHandler _fileHandler = new FileHandler();
        private string FileDir = null;
        private bool IsNew;
       
        private TreeNodeData GetSyntacticTreeView(TreeNode tree)
        {

            var node = new TreeNodeData
            {
                DisplayName = tree.TypeToString(), Name = new Token(tree.DataType).TypeToString()
            };
            if (tree.Value != null)
            {
                node.Name += $" : {tree.Value}";
                var valNode = new TreeNodeData
                {
                    DisplayName = tree.Value, Name = new Token(tree.DataType).TypeToString()
                };
                node.Children.Add(valNode);
            }

            if (tree.LeftNode != null)
            {
               node.Children.Add(GetSyntacticTreeView(tree.LeftNode));
            }
            if (tree.MiddleNode != null)
            {
                node.Children.Add(GetSyntacticTreeView(tree.MiddleNode));
            }
            if (tree.RightNode != null)
            {
                node.Children.Add(GetSyntacticTreeView(tree.RightNode));
            }
            if (tree.NextNode != null)
            {
                node.Children.Add(GetSyntacticTreeView(tree.NextNode));
            }

            return node;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            TextEditor.Text = _fileHandler.OpenFile();
            FileName = _fileHandler.FileName;
            IsSaved = _fileHandler.IsSaved;
            if (FileName == "")
            {
                FileName = "Untitled";
                IsSaved = false;
            }

            if (!IsSaved)
                Title = $"Cmm解释器 ——{FileName}*";
            else
            {
                Title = $"Cmm解释器 ——{FileName}";
                IsNew = false;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _fileHandler.SaveFile(TextEditor.Text);
            FileName = _fileHandler.FileName;
            IsSaved = _fileHandler.IsSaved;
            if (string.IsNullOrEmpty(FileName))
            {
                FileName = "Untitled";
                IsSaved = false;
            }
            else
            {
                if (!IsSaved)
                    Title = $"Cmm解释器 ——{FileName}*";
                else
                {
                    Title = $"Cmm解释器 ——{FileName}";
                    if (!string.IsNullOrEmpty(FileDir) && IsNew)
                    {
                        var rootDirectoryInfo = new DirectoryInfo(FileDir);
                        var itemList = new List<FileTreeNode> { GetFileTree(rootDirectoryInfo) };
                        FileTreeView.ItemsSource = itemList;
                    }

                    IsNew = false;
                }

            }
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

        private void SaveAsFileItem_Click(object sender, RoutedEventArgs e)
        {
            _fileHandler.SaveFileAs(TextEditor.Text);
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            if (IsSaved)
            {
                Application.Current.Shutdown();
            }
            var result = MessageBox.Show("This File is not saved, \nare you sure to exit?", "警告", MessageBoxButton.YesNo,MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            RefreshFoldStrategy(TextEditor);
            IsSaved = false;
            if (FileName != null)
                Title = Title = $"Cmm解释器 ——{FileName}*";
            else
                Title = "Cmm解释器 ——Untitled*";
        }

        private void RefreshFoldStrategy(TextEditor editor)
        {
            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(editor.TextArea);
                ((BraceFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, editor.Document);
            }
            else
            {
                if (foldingManager != null)
                {
                    FoldingManager.Uninstall(foldingManager);
                    foldingManager = null;
                }
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsSaved)
            {
                Application.Current.Shutdown();
            }
            else
            {
                var result = MessageBox.Show("This File is not saved, are you sure to exit?", "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
           
        }

        private void RunLexerButton_Click(object sender, RoutedEventArgs e)
        {
            ListViewArea.Items.Clear();
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
            foreach (var t in lexer.Words)
            {
                var data = new TokenData(t.LineNo, t.Value, t.Type, t.TypeToString());
                ListViewArea.Items.Add(data);
            }
        }

        private void RunParserButton_Click(object sender, RoutedEventArgs e)
        {
            ListViewArea.Items.Clear();
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

                var itemList = new List<TreeNodeData> {GetSyntacticTreeView(parser.SyntaxTree)};
                TreeViewArea.Visibility = Visibility.Visible;
                Splitter.Visibility = Visibility.Visible;
                TreeViewArea.ItemsSource = itemList;
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
            ListViewArea.Items.Clear();
            ListViewArea.Visibility = Visibility.Collapsed;
            Splitter.Visibility = Visibility.Collapsed;
            var parser = new Parser(); //此处可使用同步线程，不过为了简单起见，就不做同步线程了
            var lexer = new Lexer();
            StopButton.IsEnabled = true;
            ResultTextBox.Focus();
            var instructions = new InstructionGenerator();
            lexer.Chars = TextEditor.Text.ToCharArray();
            lexer.LexAnalyze();
            ResultTextBox.Text = "···········Interpreter Analyzing...\n\n";
            if (lexer.ErrorInfoStrb.ToString().Length == 0)
            {
                parser.Tokens = lexer.Words;
                parser.SyntaxAnalyze();
                if (!parser.IsParseError)
                {
                    instructions.Tree = parser.SyntaxTree;
                    var threadStart = new ThreadStart(instructions.GenerateInstructions);
                    _thread = new Thread(threadStart) { IsBackground = true };
                    _thread.Start();
                    while (_thread.IsAlive)
                        Thread.Sleep(10);
                    if (instructions.Error == null)
                        foreach (var i in instructions.Codes)
                        {
                            ResultTextBox.Text += i.ToString();
                        }
                    else
                    {
                        ResultTextBox.Text += instructions.Error;
                    }
                    ResultTextBox.Text += "\n···········Semantic Analysis done!";
                }
                else
                {
                    ResultTextBox.Text += parser.Error;
                    ResultTextBox.Text += "\n···········Syntactic Analysis Failed!\n";
                    ResultTextBox.Text += "\n···········Semantic Analysis Not Implemented!";
                }
               
               
                StopButton.IsEnabled = false;
                if (TreeViewRadioButton.IsChecked != true || NoneRadioButton.IsChecked == true)
                {
                    TreeViewArea.Visibility = Visibility.Collapsed;
                    Splitter.Visibility = Visibility.Collapsed;
                }

            }
            else
            {
                ResultTextBox.Text += lexer.ErrorInfoStrb.ToString();
                ResultTextBox.Text += "\n···········Lexical Analysis Failed!\n";
                ResultTextBox.Text += "\n···········Syntactic Analysis Not Implemented!";
            }
        }

        private void OpenDirButton_Click(object sender, RoutedEventArgs e)
        {
            var path = _fileHandler.OpenDir();
            if (path != null)
            {
                FileDir = path;
                var rootDirectoryInfo = new DirectoryInfo(path);
                var itemList = new List<FileTreeNode> {GetFileTree(rootDirectoryInfo)};
                FileTreeView.ItemsSource = itemList;
            }
        }

        private FileTreeNode GetFileTree(DirectoryInfo directoryInfo)
        {
            var directoryNode = new FileTreeNode {Name = directoryInfo.Name, IsFile = false, Children = new List<FileTreeNode>(), Icon = @"Resources/Folder.png" };
            foreach (var directory in directoryInfo.GetDirectories())
                directoryNode.Children.Add(GetFileTree(directory));

            foreach (var file in directoryInfo.GetFiles())
            {
                if(file.Extension == ".txt" || file.Extension == ".cmm")
                    directoryNode.Children.Add(new FileTreeNode { Name = file.Name, IsFile = true, Icon = @"Resources/CodeFile.png", Path = file.FullName });
            }
            return directoryNode;
        }

        private void NewFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSaved)
            {
                var result = MessageBox.Show("This File is not saved, are you sure to create a new File?", "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No) return;
            }
            TextEditor.Clear();
            FileName = "Untitled";
            _fileHandler.FilePath = "";
            Title = "Cmm解释器 ——Untitled*";
            IsNew = true;
        }

        private void RunCodeButton_Click(object sender, RoutedEventArgs e)
        {
            ListViewArea.Items.Clear();
            ListViewArea.Visibility = Visibility.Collapsed;
            Splitter.Visibility = Visibility.Collapsed;
            var parser = new Parser(); //此处可使用同步线程，不过为了简单起见，就不做同步线程了
            var lexer = new Lexer();
            StopButton.IsEnabled = true;
            ResultTextBox.Focus();
            var instructions = new InstructionGenerator();
            var interpreter = new Interpreter();
            lexer.Chars = TextEditor.Text.ToCharArray();
            lexer.LexAnalyze();
            ResultTextBox.Text = "···········Running Code...\n\n";
            if (lexer.ErrorInfoStrb.ToString().Length == 0)
            {
                parser.Tokens = lexer.Words;
                parser.SyntaxAnalyze();
                if (!parser.IsParseError)
                {
                    instructions.Tree = parser.SyntaxTree;
                    instructions.GenerateInstructions();
                    if (instructions.Error == null)
                    {
                        interpreter.Codes = instructions.Codes;
                        interpreter.RunCode();
                        if (interpreter.Error == null)
                        {
                            ResultTextBox.Text += $"{interpreter.result}\n";
                            ResultTextBox.Text += "\n···········Process Complete!";
                        }
                           
                        else
                        {
                            ResultTextBox.Text += interpreter.Error;
                            ResultTextBox.Text += "\n···········Process Failed!";
                        }
                    }
                    else
                    {
                        ResultTextBox.Text += instructions.Error;
                        ResultTextBox.Text += "\n···········Semantic Analysis Failed!\n";
                        ResultTextBox.Text += "\n···········Code Is Not Running!";
                    }
                }
                else
                {
                    ResultTextBox.Text += parser.Error;
                    ResultTextBox.Text += "\n···········Syntactic Analysis Failed!\n";
                    ResultTextBox.Text += "\n···········Semantic Analysis Not Implemented!";
                }


                StopButton.IsEnabled = false;
                if (TreeViewRadioButton.IsChecked != true || NoneRadioButton.IsChecked == true)
                {
                    TreeViewArea.Visibility = Visibility.Collapsed;
                    Splitter.Visibility = Visibility.Collapsed;
                }
              
            }
            else
            {
                ResultTextBox.Text += lexer.ErrorInfoStrb.ToString();
                ResultTextBox.Text += "\n···········Lexical Analysis Failed!\n";
                ResultTextBox.Text += "\n···········Syntactic Analysis Not Implemented!";
            }
        }

        private FoldingManager foldingManager;
        private BraceFoldingStrategy foldingStrategy;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SearchPanel.Install(TextEditor.TextArea);
            //设置语法规则
            var name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".Util.Cmm.xshd";

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            foldingManager = FoldingManager.Install(TextEditor.TextArea);
            foldingStrategy = new BraceFoldingStrategy();
            //foldingStrategy.UpdateFoldings(foldingManager, TextEditor.Document);
            TextEditor.TextArea.TextEntering += TextEditor_TextArea_TextEntering;
            TextEditor.TextArea.TextEntered += TextEditor_TextArea_TextEntered;


            //TextEditor.Text = DispContentValue;
            //foldingStrategy.UpdateFoldings(foldingManager, TextEditor.Document);
            using (var s = assembly.GetManifestResourceStream(name))
            {
                using (var reader = new XmlTextReader(s))
                {
                    var xshd = HighlightingLoader.LoadXshd(reader);
                    TextEditor.SyntaxHighlighting = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
                }
            }
        }
        

        private void TextEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            switch (e.Text)
            {
                case "p":
                {
                    var completionWindow = new CompletionWindow(TextEditor.TextArea);
                    var data = completionWindow.CompletionList.CompletionData;
                    data.Add(new MyCompletionData("print();"));
                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                    break;
                }
                case "i":
                {
                    var completionWindow = new CompletionWindow(TextEditor.TextArea);
                    var data = completionWindow.CompletionList.CompletionData;
                    data.Add(new MyCompletionData("int"));
                    data.Add(new MyCompletionData("if()"));
                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                    break;
                }
                case "r":
                {
                    var completionWindow = new CompletionWindow(TextEditor.TextArea);
                    var data = completionWindow.CompletionList.CompletionData;
                    data.Add(new MyCompletionData("real"));
                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                    break;
                }
                case "s":
                {
                    var completionWindow = new CompletionWindow(TextEditor.TextArea);
                    var data = completionWindow.CompletionList.CompletionData;
                    data.Add(new MyCompletionData("scan();"));
                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                    break;
                }
                case "e":
                {
                    var completionWindow = new CompletionWindow(TextEditor.TextArea);
                    var data = completionWindow.CompletionList.CompletionData;
                    data.Add(new MyCompletionData("else"));
                    data.Add(new MyCompletionData("else if()"));
                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                    break;
                }
                case "w":
                {
                    var completionWindow = new CompletionWindow(TextEditor.TextArea);
                    var data = completionWindow.CompletionList.CompletionData;
                    data.Add(new MyCompletionData("while()"));
                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                    break;
                }
                case "n":
                {
                    var completionWindow = new CompletionWindow(TextEditor.TextArea);
                    var data = completionWindow.CompletionList.CompletionData;
                    data.Add(new MyCompletionData("NULL"));
                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                    break;
                }
                case "c":
                {
                    var completionWindow = new CompletionWindow(TextEditor.TextArea);
                    var data = completionWindow.CompletionList.CompletionData;
                    data.Add(new MyCompletionData("char"));
                    data.Add(new MyCompletionData("continue;"));
                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                    break;
                }
                case "b":
                {
                    var completionWindow = new CompletionWindow(TextEditor.TextArea);
                    var data = completionWindow.CompletionList.CompletionData;
                    data.Add(new MyCompletionData("break;"));
                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                    break;
                }
            }
        }
        CompletionWindow completionWindow;
        public void TextEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        private void FileTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FileTreeView.SelectedItem != null)
            {
                var item = FileTreeView.SelectedItem as FileTreeNode;
                if (item.IsFile)
                {
                    if (File.Exists(item.Path))
                    {
                        var streamReader = new StreamReader(item.Path, Encoding.UTF8);
                        var text = streamReader.ReadToEnd();
                        streamReader.Close();
                        TextEditor.Text = text;
                        FileName = item.Name;
                        IsSaved = true;
                        Title = $"Cmm解释器 ——{FileName}";
                        _fileHandler.FilePath = item.Path;
                        return;
                    }
                    FileName = item.Name;
                    IsSaved = true;
                    Title = $"Cmm解释器 ——{FileName}";
                   

                }
            }
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
