namespace FlowProtocol2.Core
{
    using FlowProtocol2.Commands;
    using System.Text.RegularExpressions;

    public class ScriptParser
    {
        private List<CommandParser> CmdParser;

        public CmdBaseCommand? StartCommand { get; set; }
        public CmdBaseCommand? LastCommand { get; set; }

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
            CmdParser.Add(CmdSetTitel.GetComandParser());
            CmdParser.Add(CmdSetSection.GetComandParser());
            CmdParser.Add(CmdAddText.GetComandParser());
            CmdParser.Add(CmdAddLink.GetComandParser());
            CmdParser.Add(CmdAddCode.GetComandParser());
            CmdParser.Add(CmdOptionGroup.GetComandParser());
            CmdParser.Add(CmdOptionValue.GetComandParser());
            CmdParser.Add(CmdExecute.GetComandParser());
            CmdParser.Add(CmdIf.GetComandParser());
            CmdParser.Add(CmdElse.GetComandParser());
            CmdParser.Add(CmdCamelCase.GetComandParser());
            // Hier weitere Parser hinzufügen
        }

        public void ReadScript(RunContext rc, string scriptfilepath)
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
                        if (!codeline.StartsWith("//"))
                        {
                            bool hasmatch = false;
                            ReadContext readcontext = new ReadContext(scriptfilepath, indent, linenumber, codeline);
                            foreach (var cp in CmdParser)
                            {
                                if (cp.LineExpression.IsMatch(codeline))
                                {
                                    Match m = cp.LineExpression.Match(codeline);
                                    currentcommand = cp.CommandCreator(readcontext, m);
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
                                    hasmatch = true;
                                    break;
                                }
                            }
                            if (!hasmatch)
                            {
                                rc.SetError(readcontext, "Parsing Exception", "Die Zeile kann nicht interpretiert werden.");
                            }
                        }
                    }
                }
            }
        }
    }
}