namespace FlowProtocol2.Core
{
    using FlowProtocol2.Commands;
    using System.Text.RegularExpressions;

    public class ScriptParser
    {        
        private List<CommandParser> CmdParser;

        public CmdBaseCommand? StartCommand {get; set;}
        public CmdBaseCommand? LastCommand {get; set;}

        public ScriptParser()
        {
            CmdParser = new List<CommandParser>();
            LoadCommands();
        }

        private void LoadCommands()
        {
            CmdParser.Clear();
            CmdParser.Add(CmdInputText.GetComandParser());
            CmdParser.Add(CmdOutputText.GetComandParser());
            CmdParser.Add(CmdSet.GetComandParser());
            // Hier weitere Parser hinzufügen
        }

        public void ReadScript(string scriptfilepath)
        {            
            CmdBaseCommand? currentcommand = null;
            CmdBaseCommand? previouscommand = null;
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
                                ReadContext rs = new ReadContext(scriptfilepath, indent, linenumber, codeline, m);
                                currentcommand = cp.CommandCreator(rs);
                                if (StartCommand == null)
                                {
                                    StartCommand = currentcommand;
                                }
                                if (previouscommand != null)
                                {
                                    previouscommand.SetNextCommand(currentcommand);
                                }
                                previouscommand = currentcommand;                                
                                LastCommand = currentcommand;
                                break;
                            }
                        }
                    }
                }
            }            
        }
    }
}