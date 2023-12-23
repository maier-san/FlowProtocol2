namespace FlowProtocol2.Core
{
    using FlowProtocol2.Commands;
    using System.Text.RegularExpressions;

    public class ScriptParser
    {
        private List<CommandParser> CmdParser;
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
            CmdParser.Add(CmdUrlEncode.GetComandParser());
            CmdParser.Add(CmdReplace.GetComandParser());
            CmdParser.Add(CmdCalculate.GetComandParser());
            CmdParser.Add(CmdRound.GetComandParser());
            CmdParser.Add(CmdRandom.GetComandParser());
            CmdParser.Add(CmdEndParagraph.GetComandParser());
            CmdParser.Add(CmdInclude.GetComandParser());
            // Hier weitere Parser hinzufügen
        }

        public ScriptInfo ReadScript(RunContext rc, string scriptfilepath, int startindent)
        {
            ScriptInfo sinfo = new ScriptInfo();
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
                        int indent = startindent + line.Length - line.TrimStart().Length;
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
                                    if (sinfo.StartCommand == null)
                                    {
                                        sinfo.StartCommand = currentcommand;
                                    }
                                    if (previouscommand != null)
                                    {
                                        previouscommand.SetNextCommand(currentcommand);
                                    }
                                    previouscommand = currentcommand;
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
            return sinfo;
        }
    }

    public class ScriptInfo
    {
        public CmdBaseCommand? StartCommand { get; set; }        
    }
}