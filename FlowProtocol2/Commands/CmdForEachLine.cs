namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den ForEachLine-Befehl
    /// </summary>
    /// <remarks>
    /// Erstellt mit NewCC.fp2, Eingabe: ~ForEachLine (vVarName) in (sFileNameOrPath)[; Take=(iTake)][; IndexVar=(vIndexVar)][; SectionVar=(vSectionVar)][; (vNoFormatVar)]
    /// </remarks>
    public class CmdForEachLine : CmdLoopBaseCommand
    {
        public string VarName { get; set; }
        public string FileNameOrPath { get; set; }
        public string Take { get; set; }
        public string IndexVar { get; set; }
        public string SectionVar { get; set; }
        public string NoFormatVar { get; set; }
        private List<LineItem> LineItems { get; set; }
        private string ExpandedVarName { get; set; }
        private string ExpandedIndexVar { get; set; }
        private string ExpandedSectionVar { get; set; }
        private int Index { get; set; }
        private int RSeed { get; set; }

        private class LineItem
        {
            public string LineContend { get; set; }
            public string Section { get; set; }
            public LineItem(string linecontend, string section)
            {
                LineContend = linecontend;
                Section = section;
            }
        }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~ForEachLine\s+([A-Za-z0-9\$\(\)]+)\s+in\s+([^;]*)(\s*;\s*Take\s*=\s*(-?[A-Za-z0-9\$\(\)]+))?(\s*;\s*IndexVar\s*=\s*([A-Za-z0-9\$\(\)]+))?(\s*;\s*SectionVar\s*=\s*([A-Za-z0-9\$\(\)]+))?(\s*;\s*(NoFormat))?",
                                     (rc, m) => CreateForEachLineCommand(rc, m));
        }

        private static CmdBaseCommand CreateForEachLineCommand(ReadContext rc, Match m)
        {
            CmdForEachLine cmd = new CmdForEachLine(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.FileNameOrPath = m.Groups[2].Value.Trim();
            cmd.Take = m.Groups[4].Value.Trim();
            cmd.IndexVar = m.Groups[6].Value.Trim();
            cmd.SectionVar = m.Groups[8].Value.Trim();
            cmd.NoFormatVar = m.Groups[10].Value.Trim();
            return cmd;
        }

        public CmdForEachLine(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            FileNameOrPath = string.Empty;
            Take = string.Empty;
            IndexVar = string.Empty;
            SectionVar = string.Empty;
            NoFormatVar = string.Empty;
            LineItems = new List<LineItem>();
            ExpandedVarName = string.Empty;
            ExpandedIndexVar = string.Empty;
            ExpandedSectionVar = string.Empty;
            RSeed = 100;
            Index = 0;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedFileNameOrPath = ReplaceVars(rc, FileNameOrPath).Replace('|', Path.DirectorySeparatorChar);            
            string expandedTake = ReplaceVars(rc, Take);
            string expandedIndexVar = ReplaceVars(rc, IndexVar);
            string expandedSectionVar = ReplaceVars(rc, SectionVar);            
            try
            {
                LinkAssociatedLoopCommand(rc, "ForEachLine");
                if (AssociatedLoopCommand == null)
                {
                    rc.SetError(ReadContext, "ForEachLine ohne Loop",
                        "Dem ForEachLine-Befehl kann kein Loop-Befehl auf gleicher Ebene zugeordnet werden. Die Bearbeitung wird abgebrochen.");
                    return null;
                }
                if (!IsInitialized.ContainsKey(rc.BaseKey) || !IsInitialized[rc.BaseKey])
                {                
                    string absoluteFileName = ExpandPath(rc, expandedFileNameOrPath, out bool fileexists);
                    if (!fileexists)
                    {
                        rc.SetError(ReadContext, "Datei nicht gefunden",
                            $"Die Datei '{absoluteFileName}' konnte nicht gefunden werden. Die Skriptausführung wird abgebrochen.");
                        return null;
                    }
                    int take = -1;
                    if (!string.IsNullOrEmpty(expandedTake))
                    {
                        bool takeOK = Int32.TryParse(expandedTake, out take);
                        if (!takeOK || take < 0)
                        {
                            rc.SetError(ReadContext, "Ungültiger Take-Parameter",
                                $"Der Ausdruck '{expandedTake}' kann nicht als Take-Parameter interpretiert werden. Die Skriptausführung wird abgebrochen.");
                            return null;
                        }
                    }
                    RSeed = GetRSeed(rc);
                    bool noformat = NoFormatVar.Contains("NoFormat");
                    ReadLineItems(absoluteFileName, take, noformat);                    
                    Index = 0;
                    IsInitialized[rc.BaseKey] = true;                
                    AssociatedLoopCommand.LoopCounter = 0;                    
                }                
                if (LineItems.Any())
                {
                    var ret = LineItems.First();
                    LineItems.RemoveAt(0);
                    rc.InternalVars[expandedVarName] = ret.LineContend;
                    if (!string.IsNullOrEmpty(expandedSectionVar))
                    {
                        rc.InternalVars[expandedSectionVar] = ret.Section;
                    }
                    if (!string.IsNullOrEmpty(expandedIndexVar))
                    {
                        Index++;
                        rc.InternalVars[expandedIndexVar] = Index.ToString();
                    }
                    return NextCommand;
                }
                else
                {
                    IsInitialized[rc.BaseKey] = false;
                    return AssociatedLoopCommand.NextCommand;
                }                
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedFileNameOrPath='{expandedFileNameOrPath}' expandedTake='{expandedTake}' expandedIndexVar='{expandedIndexVar}' expandedSectionVar='{expandedSectionVar}' NoFormatVar='{NoFormatVar}'");
                return null;
            }
        }

        private void ReadLineItems(string absoluteFileName, int take, bool noformat)
        {
            LineItems.Clear();
            using (StreamReader sr = new StreamReader(absoluteFileName))
            {
                string section = string.Empty;
                while (sr.Peek() != -1)
                {
                    string? line = sr.ReadLine();
                    if (noformat || !string.IsNullOrWhiteSpace(line))
                    {
                        string idxline = line??string.Empty;
                        if (!noformat) idxline = idxline.Trim();                        
                        if (!noformat && idxline.StartsWith("//"))
                        {
                            // ignorieren
                        }
                        else if (!noformat && idxline.StartsWith("[") && idxline.EndsWith("]"))
                        {
                            section = idxline.Substring(1, idxline.Length - 2);
                        }
                        else
                        {
                            LineItems.Add(new LineItem(idxline, section));
                        }
                    }
                }
            }
            Random tr = new Random(RSeed);
            while (take > 0 && LineItems.Count() > take)
            {
                int remidx = tr.Next(LineItems.Count);
                LineItems.RemoveAt(remidx);
            }
        }
    }
}