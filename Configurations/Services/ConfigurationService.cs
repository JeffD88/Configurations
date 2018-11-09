using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Mastercam.Database;
using Mastercam.IO;
using Mastercam.Operations;
using Mastercam.Support;

namespace Configurations.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public List<int> GetConfigurations()
        {
            var Configurations = new List<int>();
            var configRegex = new Regex(@"(#config)(\d+)($)");

            var allOperations = SearchManager.GetOperations();
            foreach (var operation in allOperations)
            {
                var match = configRegex.Match(operation.Name);
                if (match.Success)
                {
                    var configurationNumber = Convert.ToInt32(match.Groups[2].Value);

                    if (!Configurations.Contains(configurationNumber))
                        Configurations.Add(configurationNumber);
                }
            }

            Configurations.Sort();

            return Configurations;
        }

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

        public void PostConfiguration(int configurationNumber)
        {
            var save = new SaveFileDialog
            {
                Title = "Save As",
                Filter = "All files (*.*)|*.*",
                InitialDirectory = SettingsManager.UserDirectory
            };
            if ((save.ShowDialog() == DialogResult.OK))
            {
                var otherConfigsRegex = new Regex(@"#config\d+$");
                var selectedConfigRegex = new Regex($"#config{configurationNumber}$");

                var OperationsToPost = new List<Operation>();

                var nciFile = Path.ChangeExtension(save.FileName, "nci");

                var allOperations = SearchManager.GetOperations();
                foreach (var operation in allOperations)
                {
                    if (selectedConfigRegex.IsMatch(operation.Name) || !otherConfigsRegex.IsMatch(operation.Name))
                    {
                        operation.NCIName = nciFile;
                        operation.Commit(false);
                        OperationsToPost.Add(operation);
                    }
                }
                if (OperationsToPost.Any())
                {
                    OperationsManager.PostOperations(OperationsToPost.ToArray(),
                                                     Path.GetDirectoryName(save.FileName),
                                                     true,
                                                     true
                                                    );
                }
                else
                    DialogManager.Error("No operations found in selected configuration.", "No Operations Found");
            }           
        }

        public void SetPosting(int configurationNumber)
        {
            var otherConfigsRegex = new Regex(@"#config\d+$");
            var selectedConfigRegex = new Regex($"#config{configurationNumber}$");

            var allOperations = SearchManager.GetOperations();
            foreach (var operation in allOperations)
            {
                if (selectedConfigRegex.IsMatch(operation.Name) || !otherConfigsRegex.IsMatch(operation.Name))
                    operation.Posting = true;
                else
                    operation.Posting = false;

                operation.Commit(false);
            }
        }
    }
}
