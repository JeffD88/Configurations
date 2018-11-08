using System.Collections.Generic;

using Mastercam.Database;


namespace Configurations.Services
{
    public interface IConfigurationService
    {
        List<int> GetConfigurations();

        void AddToConfiguration(int configurationNumber, Operation[] selectedOperations);

        void PostConfiguration(int configurationNumber);

        void SetPosting(int configurationNumbers);

    }
}
