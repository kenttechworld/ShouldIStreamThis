using System.IO;
using Tommy;

namespace ShouldIStreamThis.Service
{
    public class TomlConfigurationService : IConfigurationService
    {
        private readonly string _configDirectory;
        private readonly string _tomlPath;
        private TomlTable? _tomlCache;

        public TomlConfigurationService()
        {
            _configDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
            _tomlPath = Path.Combine(_configDirectory, "Config.toml");
        }

        public bool CheckIfTOMLFileExist()
        {
            return File.Exists(_tomlPath);
        }

        public void MakeTOMLFile()
        {
            if (!Directory.Exists(_configDirectory))
            {
                Directory.CreateDirectory(_configDirectory);
            }

            TomlTable defaultToml = new TomlTable
            {
                ["ClientId"] = string.Empty,
                ["ClientSecret"] = string.Empty
            };

            using (StreamWriter writer = File.CreateText(_tomlPath))
            {
                defaultToml.WriteTo(writer);
                writer.Flush();
            }

            _tomlCache = defaultToml;
        }

        public T ReadTOMLField<T>(string fieldName)
        {
            if (!CheckIfTOMLFileExist())
            {
                throw new FileNotFoundException("TOML configuration file not found.", _tomlPath);
            }

            EnsureLoaded();

            if (_tomlCache == null || !_tomlCache.HasKey(fieldName))
            {
                throw new KeyNotFoundException($"The key '{fieldName}' was not found in the TOML configuration.");
            }

            TomlNode node = _tomlCache[fieldName];

            if (typeof(T) == typeof(string))
            {
                return (T)(object)node.AsString.Value;
            }
            if (typeof(T) == typeof(int))
            {
                return (T)(object)(int)node.AsInteger.Value;
            }
            if (typeof(T) == typeof(long))
            {
                return (T)(object)node.AsInteger.Value;
            }
            if (typeof(T) == typeof(bool))
            {
                return (T)(object)node.AsBoolean.Value;
            }

            throw new NotSupportedException($"The type '{typeof(T).Name}' is not supported for TOML conversion.");
        }

        public void UpdateTOMLField<T>(string fieldName, T newValue)
        {
            if (!CheckIfTOMLFileExist())
            {
                MakeTOMLFile();
            }

            EnsureLoaded();

            if (_tomlCache == null) return;

            if (newValue is string strVal)
            {
                _tomlCache[fieldName] = strVal;
            }
            else if (newValue is int intVal)
            {
                _tomlCache[fieldName] = intVal;
            }
            else if (newValue is long longVal)
            {
                _tomlCache[fieldName] = longVal;
            }
            else if (newValue is bool boolVal)
            {
                _tomlCache[fieldName] = boolVal;
            }
            else
            {
                throw new NotSupportedException($"The type '{typeof(T).Name}' cannot be written to TOML.");
            }

            SaveConfigFile();
        }

        private void EnsureLoaded()
        {
            if (_tomlCache != null) return;

            using StreamReader reader = File.OpenText(_tomlPath);
            _tomlCache = TOML.Parse(reader);
        }

        private void SaveConfigFile()
        {
            if (_tomlCache == null) return;

            using StreamWriter writer = File.CreateText(_tomlPath);
            _tomlCache.WriteTo(writer);
            writer.Flush();
        }
    }
}
