using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mastercam.Database;

namespace Configurations.Services
{
    class ConfigurationService : IConfigurationService
    {
        public void AddToConfiguration(int configurationNumber, Operation[] selectedOperations)
        {
            var regex = new Regex(@"#config\d+$");

            foreach (var operation in selectedOperations)
            {              
                if (regex.IsMatch(operation.Name))
                    operation.Name = Regex.Replace(operation.Name, @"#config\d+$", $"#config{configurationNumber}");
                else
                    operation.Name = $"{operation.Name}{Environment.NewLine}#config{configurationNumber}";

                operation.Commit(false);
            }
        }

        public void SetPosting(int configurationNumber, Operation[] allOperations)
        {
            var otherConfigsRegex = new Regex(@"#config\d+$");
            var selectedConfigRegex = new Regex($"#config{configurationNumber}$");

            foreach (var operation in allOperations)
            {
                if (selectedConfigRegex.IsMatch(operation.Name) || !otherConfigsRegex.IsMatch(operation.Name))
                    operation.Posting = true;
                else
                    operation.Posting = false;

                operation.Commit(false);
            }
        }

        public List<int> GetConfigurations(Operation[] allOperations)
        {
            var Configurations = new List<int>();
            var configRegex = new Regex(@"(#config)(\d+)($)");

            foreach (var operation in allOperations)
            {
                var match = configRegex.Match(operation.Name);
                if (match.Success)
                {
                    Configurations.Add(Convert.ToInt32(match.Groups[2].Value));
                }
            }

            return Configurations;
        }
    }
}
