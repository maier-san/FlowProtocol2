namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den Include-Befehl
    /// </summary>
    public class CmdInclude : CmdBaseCommand
    {
        public string ScriptName { get; set; }
        public string BaseKey { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Include ([A-Za-z0-9\.]*)", (rc, m) => CreateIncludeCommand(rc, m));
        }
        public static CommandParser GetCommandParserWithBaseKey()
        {
            return new CommandParser(@"^~Include ([A-Za-z0-9\.]*)\s*;\s*BaseKey\s*=\s*([A-Za-z0-9$]*)", (rc, m) => CreateIncludeCommandWithBaseKey(rc, m));
        }

        private static CmdBaseCommand CreateIncludeCommand(ReadContext rc, Match m)
        {
            CmdInclude cmd = new CmdInclude(rc);
            cmd.ScriptName = m.Groups[1].Value.Trim();
            return cmd;
        }

        private static CmdBaseCommand CreateIncludeCommandWithBaseKey(ReadContext rc, Match m)
        {
            CmdInclude cmd = new CmdInclude(rc);
            cmd.ScriptName = m.Groups[1].Value.Trim();
            cmd.BaseKey = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdInclude(ReadContext readcontext) : base(readcontext)
        {
            ScriptName = string.Empty;
            BaseKey = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string scriptFilePath = rc.ScriptPath + Path.DirectorySeparatorChar + ReplaceVars(rc, ScriptName);
            if (!rc.ScriptRepository.ContainsKey(scriptFilePath))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(scriptFilePath);
                if (fi != null && !fi.Exists)
                {
                    rc.SetError(ReadContext, "Skriptdatei nicht gefunden",
                        $"Die Skriptdatei {scriptFilePath} konnte nicht gefunden werden. Das Skript wird abgebrochen.");
                    return null;
                }
                ScriptParser sp = new ScriptParser();
                string extendedBaseKey = ReplaceVars(rc, BaseKey).Trim();
                var newScriptinfo = sp.ReadScript(rc, scriptFilePath, Indent);
                rc.ScriptRepository[scriptFilePath] = newScriptinfo;
            }
            var sinfo = rc.ScriptRepository[scriptFilePath];
            if (sinfo.StartCommand != null)
            {
                string expandedBaseKey = ReplaceVars(rc, BaseKey).Trim();
                if (NextCommand != null) rc.ReturnStack.Push(new EntryPoint(NextCommand, rc.BaseKey));
                rc.BaseKey = expandedBaseKey;
                return sinfo.StartCommand;
            }
            return NextCommand;
        }
    }
}