using System.Text;

namespace FullSys
{
    public static partial class StringBuilderExtensions
    {
        public static StringBuilder AppendHtml (this StringBuilder sb, string source)
        {
            foreach (char ch in source)
                if (ch == '"')
                    sb.Append ("&quot;");
                else if (ch == '&')
                    sb.Append ("&amp;");
                else if (ch == '\'')
                    sb.Append ("&apos;");
                else if (ch == '<')
                    sb.Append ("&lt;");
                else if (ch == '>')
                    sb.Append ("&gt;");
                else
                    sb.Append (ch);

            return sb;
        }
    }
}
