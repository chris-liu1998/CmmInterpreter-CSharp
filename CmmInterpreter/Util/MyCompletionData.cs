using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace CmmInterpreter.Util
{
    /// Implements AvalonEdit ICompletionData interface to provide the entries in the
    /// completion drop down.
    public class MyCompletionData : ICompletionData
    {
        public MyCompletionData(string text)
        {
            Text = text;
        }

        public System.Windows.Media.ImageSource Image => null;

        public string Text { get; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content => Text;

        public object Description => "" + Text;
        public double Priority { get; }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text.Substring(1, Text.Length - 1));
        }
    }
}
