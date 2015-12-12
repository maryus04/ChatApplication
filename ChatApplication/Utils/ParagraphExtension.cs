using System.Windows.Documents;
using System.Windows.Media;

namespace ChatApplication.Utils {
    static class ParagraphExtension {
        public static void AddText(this Paragraph para, string text, SolidColorBrush color = null) {
            para.Inlines.Add(new Run(text) {
                Foreground = color ?? Brushes.Black
            });
        }
    }
}
