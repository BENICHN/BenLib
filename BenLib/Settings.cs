using System;
using System.IO;

namespace BenLib
{
    /// <summary>
    /// <para>Contient des outils pour gérér un fichier de paramètres. Il se présente comme ceci :</para>
    /// <para>------------------------------------------------------------------------------------</para>
    /// <para>Paramètre1=Valeur1</para>
    /// <para>Paramètre2=Valeur2</para>
    /// <para>Paramètre3=Valeur3</para>
    /// <para>------------------------------------------------------------------------------------</para>
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Chaîne contenant les paramètres comme ils sont actuellement stockés dans la mémoire.
        /// </summary>
        private string settings = String.Empty;

        /// <summary>
        /// Chaîne contenant les paramètres comme ils sont actuellement stockés dans la mémoire.
        /// </summary>
        public string Text { get => settings; }

        /// <summary>
        /// Représente le nom du fichier de paramètres.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Indique si l'on peut écrire des paramètres dans le fichier de paramètres.
        /// </summary>
        public bool CanWrite { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe BenLib.Settings à partir d'un fichier spécifié.
        /// </summary>
        public Settings(string filename)
        {
            FileName = filename;
            RefreshSettings();
        }

        /// <summary>
        /// Importe le contenu du fichier de paramètres dans la mémoire.
        /// </summary>
        public void RefreshSettings()
        {
            if (File.Exists(FileName))
            {
                settings = File.ReadAllText(FileName);
            }
        }

        /// <summary>
        /// Retourne la valeur du paramètre indiqué (s'il n'existe pas, retourne une chaîne vide).
        /// </summary>
        public string GetSetting(string setting)
        {
            setting += "=";

            if (settings.IndexOf(setting) != -1)
            {
                string PostSettings = settings.Substring(settings.IndexOf(setting) + setting.Length);

                if (PostSettings.Contains(Environment.NewLine))
                {
                    return PostSettings.Substring(0, PostSettings.IndexOf(Environment.NewLine));
                }
                else
                {
                    return PostSettings;
                }
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Écrit une chaîne dans le fichier de paramètres après une ligne. Écrase le contenu de celui-ci si "overwrite" est true.
        /// </summary>
        public void WriteSettings(string SettingsToWrite, bool overwrite)
        {
            if (CanWrite)
            {
                if (!overwrite && settings != String.Empty)
                {
                    SettingsToWrite = settings + Environment.NewLine + SettingsToWrite;
                }

                using (StreamWriter sr = new StreamWriter(FileName))
                {
                    sr.Write(SettingsToWrite);
                }

                RefreshSettings(); //Rechargement des paramètres
            }
        }

        /// <summary>
        /// Modifie la valeur d'un paramètre dans le fichier de paramètres ou l'écrit s'il n'est pas présent.
        /// </summary>
        public void WriteSettings(string SettingToEdit, string Value)
        {
            if (CanWrite)
            {
                SettingToEdit += "=";
                if (settings.IndexOf(SettingToEdit) != -1)
                {
                    string BeforeSettings = settings.Substring(0, settings.IndexOf(SettingToEdit) + SettingToEdit.Length);
                    string ActSetting = settings.Substring(BeforeSettings.Length);
                    string PostSettings = String.Empty;

                    if (ActSetting.Contains(Environment.NewLine))
                    {
                        PostSettings = ActSetting.Substring(ActSetting.IndexOf(Environment.NewLine));
                        ActSetting = ActSetting.Substring(0, ActSetting.IndexOf(Environment.NewLine));
                    }

                    using (StreamWriter sr = new StreamWriter(FileName))
                    {
                        sr.Write(BeforeSettings + Value + PostSettings);
                    }

                    RefreshSettings(); //Rechargement des paramètres
                }
                else
                {
                    WriteSettings(SettingToEdit + Value, false);
                }
            }
        }

        /// <summary>
        /// Efface la ligne du fichier de paramètres où est écrit le paramètre indiqué.
        /// </summary>
        public void EraseSettingLine(string setting)
        {
            if (settings.Contains(setting))
            {
                string BeforeSettings = settings.Substring(0, settings.IndexOf(setting));
                string PostSettings = settings.Substring(settings.IndexOf(setting));

                if (PostSettings.Contains(Environment.NewLine))
                {
                    PostSettings = PostSettings.Substring(PostSettings.IndexOf(Environment.NewLine));
                    WriteSettings(BeforeSettings + PostSettings.TrimStart(Environment.NewLine.ToCharArray()), true);
                }
                else
                {
                    WriteSettings(BeforeSettings.TrimEnd(Environment.NewLine.ToCharArray()), true);
                }
            }
        }

        /// <summary>
        /// Efface le contenu du fichier de paramètres.
        /// </summary>
        public void EraseSettings()
        {
            using (StreamWriter sr = new StreamWriter(FileName))
            {
                sr.Write(String.Empty);
            }

            RefreshSettings();
        }
    }
}
