using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace ChatApplication.UserControls {
    static class ParagraphExtension {
        public static void AddText(this Paragraph para, string text, SolidColorBrush color = null) {
            para.Inlines.Add(new Run(text) {
                Foreground = color ?? Brushes.Black
            });
        }
    }
}
