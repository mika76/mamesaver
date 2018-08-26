using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Serilog;

namespace Mamesaver.Configuration
{
    /// <summary>
    ///     Provides persistence and retrieval of settings to disk as XML files inside 
    ///     <see cref="Environment.SpecialFolder.LocalApplicationData"/>. Each implementation
    ///     of this store is responsible for managing a single setting file type.
    /// </summary>
    /// <remarks>
    ///     For convenience, the store manages a lazily-loaded instance of its settings.
    /// </remarks>
    /// <typeparam name="T">Settings model type</typeparam>
    public abstract class SettingsStore<T> where T : class, new()
    {
        /// <summary>
        ///     Setting file name.
        /// </summary>
        public abstract string Filename { get; }

        private readonly Lazy<T> _settings;
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(T));

        protected SettingsStore() => _settings = new Lazy<T>(Load);

        /// <summary>
        ///     Returns settings, loading from disk if not in memory.
        /// </summary>
        public T Get() => _settings.Value;

        /// <summary>
        ///     Serializes externally-managed settings to disk.
        /// </summary>
        public void Save(T settings)
        {
            var settingsFile = GetSettingsFile();

            using (var stream = new FileStream(settingsFile, FileMode.Create))
            using (var writer = new XmlTextWriter(stream, Encoding.UTF8) { Formatting = Formatting.Indented })
            {
                _serializer.Serialize(writer, settings);
            }
        }

        /// <summary>
        ///     Serializes store-managed settings to disk.
        /// </summary>
        public void Save() => Save(_settings.Value);

        /// <summary>
        ///     Reads settings from disk. If no settings are available or if the file cannot be read, 
        ///     a new instance of <see cref="T"/> is returned.
        /// </summary>
        private T Load()
        {
            var settingsFile = GetSettingsFile();
            if (!File.Exists(settingsFile)) return new T();

            using (var stream = new FileStream(settingsFile, FileMode.Open))
            using (var reader = new XmlTextReader(stream))
            {
                try
                {
                    return _serializer.Deserialize(reader) as T;
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unable to read settings file {filename}", Filename);
                    return new T();
                }
            }
        }

        /// <summary>
        ///     Returns an absolute path to the settings file managed by the store.
        /// </summary>
        public string GetSettingsFile()
        {
            var applicationFolder = GetApplicationFolder();
            return Path.Combine(applicationFolder, Filename);
        }

        /// <summary>
        ///     Returns and optionally creates folder for Mamesaver inside 
        ///     <see cref="Environment.SpecialFolder.LocalApplicationData"/> where all settings files are stored.
        /// </summary>
        private static string GetApplicationFolder()
        {
            var applicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var applicationFolder = Path.Combine(applicationData, "Mamesaver");

            if (!Directory.Exists(applicationFolder)) Directory.CreateDirectory(applicationFolder);
            return applicationFolder;
        }
    }
}