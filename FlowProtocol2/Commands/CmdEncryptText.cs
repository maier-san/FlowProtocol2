namespace FlowProtocol2.Commands
{
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den EncryptText-Befehl
    /// </summary>
    public class CmdEncryptText : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Text { get; set; }
        public string Key { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~EncryptText ([A-Za-z0-9\$\(\)]*)\s*=([^|]*)\s*\|\s*(.*)", (rc, m) => CreateEncryptTextCommand(rc, m));
        }

        private static CmdBaseCommand CreateEncryptTextCommand(ReadContext rc, Match m)
        {
            CmdEncryptText cmd = new CmdEncryptText(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Text = m.Groups[2].Value.Trim();
            cmd.Key = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdEncryptText(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Text = string.Empty;
            Key = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedText = ReplaceVars(rc, Text);
            string expandedKey = ReplaceVars(rc, Key);
            try
            {
                if (string.IsNullOrEmpty(expandedKey))
                {
                    rc.SetError(ReadContext, "Schlüssel ist leer",
                    "Der für die Verschlüsselung angegebene Schlüssel darf nicht leer sein. Die Ausführung wird abgebrochen.");
                    return null;
                }
                string h48 = expandedKey;
                while (h48.Length < 48)
                {
                    h48 += h48;
                }
                string key32 = h48[0..32];
                string iv16 = h48[32..48];
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key =Encoding.UTF8.GetBytes(key32);
                    aesAlg.IV = Encoding.UTF8.GetBytes(iv16);
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(expandedText);
                            }
                        }
                        string code = Convert.ToBase64String(msEncrypt.ToArray());
                        rc.InternalVars[expandedVarName] = code;
                    }
                }
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedText='{expandedText}' expandedKey='{expandedKey}'");
                return null;
            }
            return NextCommand;
        }
    }
}