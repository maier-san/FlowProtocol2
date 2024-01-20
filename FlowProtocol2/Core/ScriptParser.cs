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
            CmdParser.Add(CmdSetTitle.GetComandParser());
            CmdParser.Add(CmdSetSection.GetComandParser());
            CmdParser.Add(CmdAddText.GetComandParser());
            CmdParser.Add(CmdAddLink.GetComandParser());
            CmdParser.Add(CmdAddCode.GetComandParser());
            CmdParser.Add(CmdOptionGroup.GetComandParser());
            CmdParser.Add(CmdOptionValue.GetComandParser());
            CmdParser.Add(CmdExecute.GetComandParser());
            CmdParser.Add(CmdIf.GetComandParser());
            CmdParser.Add(CmdElseIf.GetComandParser());
            CmdParser.Add(CmdElse.GetComandParser());
            CmdParser.Add(CmdCamelCase.GetComandParser());
            CmdParser.Add(CmdUrlEncode.GetComandParser());
            CmdParser.Add(CmdReplace.GetComandParser());
            CmdParser.Add(CmdCalculate.GetComandParser());
            CmdParser.Add(CmdRound.GetComandParser());
            CmdParser.Add(CmdRandom.GetComandParser());
            CmdParser.Add(CmdEndParagraph.GetComandParser());
            CmdParser.Add(CmdInclude.GetComandParser());
            CmdParser.Add(CmdImplies.GetComandParser());
            CmdParser.Add(CmdAddHelpLine.GetComandParser());
            CmdParser.Add(CmdAddHelpLink.GetComandParser());
            CmdParser.Add(CmdAddHelpText.GetComandParser());
            CmdParser.Add(CmdSetInputTitle.GetComandParser());
            CmdParser.Add(CmdSetDateTime.GetComandParser());
            CmdParser.Add(CmdAddTo.GetComandParser());
            CmdParser.Add(CmdDoWhile.GetComandParser());
            CmdParser.Add(CmdForEach.GetComandParser());
            CmdParser.Add(CmdLoop.GetComandParser());
            CmdParser.Add(CmdSplit.GetComandParser());
            CmdParser.Add(CmdTrim.GetComandParser());
            CmdParser.Add(CmdEvalExpression.GetComandParser());
            CmdParser.Add(CmdForEachLine.GetComandParser());
            CmdParser.Add(CmdRegExMatch.GetComandParser());
            CmdParser.Add(CmdSetInputSection.GetComandParser());
            CmdParser.Add(CmdSetInputDescription.GetComandParser());
            CmdParser.Add(CmdEnd.GetComandParser());
            CmdParser.Add(CmdDefineSub.GetComandParser());
            CmdParser.Add(CmdReturn.GetComandParser());
            CmdParser.Add(CmdGoSub.GetComandParser());
            CmdParser.Add(CmdJumpMark.GetComandParser());
            CmdParser.Add(CmdGoTo.GetComandParser());
            CmdParser.Add(CmdMoveSection.GetComandParser());
            CmdParser.Add(CmdAddNewKey.GetComandParser());
            CmdParser.Add(CmdSetStopCounter.GetComandParser());
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
                string lastline = string.Empty;
                while (sr.Peek() != -1)
                {
                    string? line = sr.ReadLine();                    
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        string trline = line.TrimStart();
                        if (trline.StartsWith("//"))
                        {
                            // Kommentar, ignorieren.
                        }
                        else if (trline.StartsWith("__"))
                        {
                            // Umgebrochene Zeile: Füge diese wieder aneinander
                            lastline += trline[2..];
                        }
                        else
                        {
                            // Keine weiteren Umbrüche mehr: Interpretiere die letzte Zeile
                            if (lastline != string.Empty)
                            {
                                ParseLine(rc, scriptfilepath, startindent, sinfo, ref currentcommand, ref previouscommand,
                                    linenumber, lastline);
                            }
                            lastline = line;
                        }
                    }
                    linenumber++;
                }
                if (lastline != string.Empty)
                {
                    // Interpretiere abschließend noch die letzte Zeile
                    ParseLine(rc, scriptfilepath, startindent, sinfo, ref currentcommand, ref previouscommand,
                        linenumber, lastline);
                }
            }
            return sinfo;
        }

        private void ParseLine(RunContext rc, string scriptfilepath, int startindent, ScriptInfo sinfo,
            ref CmdBaseCommand? currentcommand, ref CmdBaseCommand? previouscommand, int linenumber, string line)
        {
            line = line.Replace("\t", "    ");
            int indent = startindent + line.Length - line.TrimStart().Length;
            string codeline = line.Trim();
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

    public class ScriptInfo
    {
        public CmdBaseCommand? StartCommand { get; set; }
    }
}