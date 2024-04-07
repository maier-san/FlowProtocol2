namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Implementiert den DecryptText-Befehl
    /// </summary>
    public class CmdDecryptText : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Code { get; set; }
        public string Key { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~DecryptText ([A-Za-z0-9\$\(\)]*)\s*=([^|]*)\s*\|\s*(.*)", (rc, m) => CreateDecryptTextCommand(rc, m));
        }

        private static CmdBaseCommand CreateDecryptTextCommand(ReadContext rc, Match m)
        {
            CmdDecryptText cmd = new CmdDecryptText(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Code = m.Groups[2].Value.Trim();
            cmd.Key = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdDecryptText(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Code = string.Empty;
            Key = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedCode = ReplaceVars(rc, Code);
            string expandedKey = ReplaceVars(rc, Key);
            try
            {
                if (string.IsNullOrEmpty(expandedKey))
                {
                    rc.SetError(ReadContext, "Schlüssel ist leer",
                    "Der für die Entschlüsselung angegebene Schlüssel darf nicht leer sein. Die Ausführung wird abgebrochen.");
                    return null;
                }
                string h48 = expandedKey;
                while (h48.Length < 48)
                {
                    h48 += h48;
                }
                string key32 = h48[0..32];
                string iv16 = h48[32..48];
                byte[] codeBytes = Convert.FromBase64String(expandedCode);
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(key32);
                    aesAlg.IV = Encoding.UTF8.GetBytes(iv16);
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (MemoryStream msDecrypt = new MemoryStream(codeBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                string text = srDecrypt.ReadToEnd();                                 
                                rc.InternalVars[expandedVarName] = text;
                            }                                                        
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedCode='{expandedCode}' expandedKey='{expandedKey}'");
                return null;
            }
            return NextCommand;
        }
    }
}