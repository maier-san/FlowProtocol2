namespace FlowProtocol2.Core
{
    using FlowProtocol2.Commands;
    using System.Net.Http.Headers;
    using System.Text.RegularExpressions;

    public class ScriptParser
    {
        List<CommandParser> CmdParser;

        public ScriptParser()
        {
            CmdParser = new List<CommandParser>();
            LoadCommands();
        }

        private void LoadCommands()
        {
            CmdParser.Clear();
            CmdParser.Add(CmdInputText.GetComandParser());
        }

        public CmdBaseCommand? ReadScript(string scriptfilepath)
        {
            CmdBaseCommand? startcommand = null;
            CmdBaseCommand? currentcommand = null;
            using (StreamReader sr = new StreamReader(scriptfilepath))
            {
                int linenumber = 0;
                while (sr.Peek() != -1)
                {
                    string? line = sr.ReadLine();
                    linenumber++;
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        line = line.Replace("\t", "    ");
                        int indent = line.Length - line.TrimStart().Length;                        
                        string codeline = line.Trim();
                        foreach (var cp in CmdParser)
                        {
                            if (cp.LineExpression.IsMatch(codeline))
                            {
                                Match m = cp.LineExpression.Match(codeline);
                                ReadingSession rs = new ReadingSession(scriptfilepath, indent, m, currentcommand);
                                CmdBaseCommand nextcommand = cp.CommandCreator(rs);
                                if (startcommand == null)
                                {
                                    startcommand = nextcommand;
                                }
                                if (currentcommand != null)
                                {
                                    currentcommand.NextCommand = nextcommand;
                                }
                                else
                                {
                                    currentcommand = nextcommand;
                                }
                                break;
                            }
                        }
                    }
                }
            }            
            return startcommand;
        }
    }
}