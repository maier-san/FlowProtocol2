namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den Include-Befehl
    /// </summary>
    public class CmdInclude : CmdBaseCommand
    {
        public string ScriptNameOrPath { get; set; }
        public string BaseKey { get; set; }
        public string Ignore { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Include ([^;]*\.fps)\s*(; BaseKey=[A-Za-z0-9\$\(\)]*)?(.*)", (rc, m) => CreateIncludeCommand(rc, m));
        }

        private static CmdBaseCommand CreateIncludeCommand(ReadContext rc, Match m)
        {
            CmdInclude cmd = new CmdInclude(rc);
            cmd.ScriptNameOrPath = m.Groups[1].Value.Trim();
            cmd.BaseKey = m.Groups[2].Value.Trim();
            cmd.Ignore = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdInclude(ReadContext readcontext) : base(readcontext)
        {
            ScriptNameOrPath = string.Empty;
            BaseKey = string.Empty;
            Ignore = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            if (!string.IsNullOrEmpty(Ignore))
            {
                rc.SetError(ReadContext, "Unerwartete Sequenz gefunden",
                    $"Die Sequenz '{Ignore}' kann an dieser Stelle nicht interpretiert werden und wird ignoriert.");
            }
            string expandedScriptNameOrPath = ReplaceVars(rc, ScriptNameOrPath).Replace('|', Path.DirectorySeparatorChar);
            string absolutescriptfilepath = ExpandPath(rc, expandedScriptNameOrPath, out bool fileexists);
            if (!rc.ScriptRepository.ContainsKey(absolutescriptfilepath))
            {
                if (!fileexists)
                {
                    rc.SetError(ReadContext, "Skriptdatei nicht gefunden",
                        $"Die Skriptdatei '{absolutescriptfilepath}' konnte nicht gefunden werden. Die Skriptausführung wird abgebrochen.");
                    return null;
                }
                ScriptParser sp = new ScriptParser();
                var newScriptinfo = sp.ReadScript(rc, absolutescriptfilepath, Indent);
                rc.ScriptRepository[absolutescriptfilepath] = newScriptinfo;
            }
            var sinfo = rc.ScriptRepository[absolutescriptfilepath];
            if (sinfo.StartCommand != null)
            {
                string expandedBaseKey = ReplaceVars(rc, BaseKey.Replace("; BaseKey=", string.Empty)).Trim();
                if (NextCommand != null) rc.ReturnStack.Push(new EntryPoint(NextCommand, rc.BaseKey));
                rc.BaseKey = expandedBaseKey;
                return sinfo.StartCommand;
            }
            return NextCommand;
        }
    }
}