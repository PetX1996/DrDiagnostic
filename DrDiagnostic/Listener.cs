using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DrDiagnostic
{
    class Listener
    {
        StringBuilder sb = new StringBuilder();

        public Listener()
        { }

        public void WriteLine(string text)
        {
            sb.Append(text);
        }

        public void SaveToFile(string fileFullPath)
        {
            File.WriteAllText(fileFullPath, sb.ToString());
        }
    }
}
