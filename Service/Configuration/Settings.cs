using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataAnalysis.Service.Configuration
{
    public class Settings
    {
        private static readonly Regex ENVIRONMENT_VARIABLE_REGEX = new Regex("%(.*?)%", RegexOptions.Compiled);

        // Static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<Settings> _instance = new Lazy<Settings>(() => new Settings());

        // Private to prevent direct instantiation
        private Settings()
        {
            this.AppSettingsToObject();
        }

        // Accessor for instance
        public static Settings Instance
        {
            get { return _instance.Value; }
        }

        public string DefaultInputDirectory { get; private set; }
        public string DefaultOutputDirectory { get; private set; }
        public TimeSpan SleepTime { get; private set; }

        private void AppSettingsToObject()
        {
            foreach (var property in this.GetType().GetProperties())
            {
                var value = ConfigurationManager.AppSettings[property.Name];

                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                value = ReplaceEnvironmentVariableToken(value);

                var propertyType = property.PropertyType;

                var converter = TypeDescriptor.GetConverter(propertyType);

                if (converter.CanConvertFrom(typeof(string)))
                {
                    property.SetValue(this, converter.ConvertFrom(value), null);
                }
            }
        }

        private static string ReplaceEnvironmentVariableToken(string value)
        {
            // Allows a environment variable token
            var match = ENVIRONMENT_VARIABLE_REGEX.Match(value);

            if (!match.Success)
            {
                return value;
            }

            // Skips match.Value
            var groups = match.Groups.Cast<Group>().Skip(1).ToList();

            value = groups.Select(group => Environment.GetEnvironmentVariable(group.Value))
                          .Where(variableValue => variableValue != null)
                          .Aggregate(value, (current, variableValue) => current.Replace(match.Value, variableValue));

            return value;
        }
    }
}
