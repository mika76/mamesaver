using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace Mamesaver
{
    /// <summary>
    /// This class is used for storing settings to their respective places. The registry settings are saved
    /// to the CURRENT_USER\Software\Mamesaver key.
    /// </summary>
    public static class Settings
    {
        private static RegistryKey rKey;

        static Settings()
        {
            rKey = Registry.CurrentUser.OpenSubKey("Software", RegistryKeyPermissionCheck.ReadWriteSubTree);

            if ( rKey == null )
                throw new RegistryNotAccessableException("Cannot access the HKEY_CURRENT_USER\\Software registry key.");

            rKey = rKey.CreateSubKey("Mamesaver", RegistryKeyPermissionCheck.ReadWriteSubTree);

            if ( rKey == null )
                throw new RegistryNotAccessableException("Cannot access\\create the HKEY_CURRENT_USER\\Software\\Mamesaver registry key.");

            // save the running directory to the (Default) key of HKEY_CURRENT_USER\Software\Mamesaver
            rKey.SetValue("", AppDomain.CurrentDomain.BaseDirectory);
        }

        /// <summary>
        /// The path to the MAME executable file - including the filename and extension. eg: C:\MAME\MAME32.EXE
        /// </summary>
        public static string ExecutablePath
        {
            get 
            { 
                return rKey.GetValue("ExecutableName", "C:\\MAME\\MAME32.EXE").ToString();
            }
            set 
            {
                rKey.SetValue("ExecutableName", value, RegistryValueKind.String);
            }
        }

        /// <summary>
        /// The options to send to the command line when MAME runs the game.
        /// </summary>
        public static string CommandLineOptions
        {
            get 
            { 
                return rKey.GetValue("CommandLineOptions", "-skip_gameinfo -nowindow -noswitchres -sleep -triplebuffer -sound none").ToString();
            }
            set 
            {
                rKey.SetValue("CommandLineOptions", value, RegistryValueKind.String);
            }
        }

        /// <summary>
        /// The time to run each game
        /// </summary>
        public static int Minutes
        {
            get
            {
                return int.Parse(rKey.GetValue("Minutes", 5).ToString());
            }
            set
            {
                rKey.SetValue("Minutes", value, RegistryValueKind.DWord);
            }
        }

        /// <summary>
        /// Save the selectable game list to an XML file.
        /// </summary>
        /// <param name="gameList"></param>
        public static void SaveGameList(List<SelectableGame> gameList)
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string filename = Path.Combine(folderPath, "Mamesaver");

            if ( !Directory.Exists(filename) ) Directory.CreateDirectory(filename);

            filename = Path.Combine(filename, "gamelist.xml");

            XmlSerializer serializer = new XmlSerializer(typeof(List<SelectableGame>));
            FileStream fileList = new FileStream(filename, FileMode.Create);
            XmlTextWriter writer = new XmlTextWriter(fileList, UTF8Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            serializer.Serialize(writer, gameList);
            writer.Close();
        }

        /// <summary>
        /// Load the selectable game list from an XML file. Return an empty array if no file found.
        /// </summary>
        /// <returns><see cref="List{T}"/> of <see cref="SelectableGame"/>s</returns>
        public static List<SelectableGame> LoadGameList()
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string filename = Path.Combine(folderPath, "Mamesaver");
            filename = Path.Combine(filename, "gamelist.xml");

            if ( !File.Exists(filename) ) return new List<SelectableGame>();

            XmlSerializer serializer = new XmlSerializer(typeof(List<SelectableGame>));
            FileStream fileList = new FileStream(filename, FileMode.Open);
            XmlTextReader reader = new XmlTextReader(fileList);
            List<SelectableGame> gameList = serializer.Deserialize(reader) as List<SelectableGame>;
            reader.Close();

            return gameList;
        }
    }

    /// <summary>
    /// This exception is thrown if the registry is not accessable.
    /// </summary>
    [global::System.Serializable]
    public class RegistryNotAccessableException : ApplicationException
    {
        public RegistryNotAccessableException() { }
        public RegistryNotAccessableException(string message) : base(message) { }
        public RegistryNotAccessableException(string message, Exception inner) : base(message, inner) { }
        protected RegistryNotAccessableException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
