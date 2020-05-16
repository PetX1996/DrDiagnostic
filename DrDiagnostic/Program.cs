using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace DrDiagnostic
{
    class Program
    {
        private static bool _verbose;
        private static string _inputLogFile;
        private static string _outputLogFile;

        static void Main(string[] args)
        {
            ParseArgs(args);

            Listener listener = new Listener();
            Reader reader = new Reader(listener);
            reader.ReadLogFile(_inputLogFile);

            listener.SaveToFile(_outputLogFile);
        }

        static void ParseArgs(string[] args)
        {
            foreach (string arg in args)
            {
                string[] argToks = arg.Split('=');
                string name = argToks[0].TrimStart('-');
                string value = argToks.Length > 1 ? argToks[1] : null;

                switch (name)
                {
                    case "verbose":
                        _verbose = true;
                        break;
                    case "inputLogFile":
                        _inputLogFile = value;
                        break;
                    case "outputLogFile":
                        _outputLogFile = value;
                        break;
                    default:
                        throw new ApplicationException("Unknown arg '" + arg + "'");
                }
            }
        }
    }
}
