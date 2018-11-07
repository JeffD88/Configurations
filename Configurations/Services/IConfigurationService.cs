using Mastercam.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurations.Services
{
    public interface IConfigurationService
    {
        List<int> GetConfigurations(Operation[] allOperations);

        void AddToConfiguration(int configurationNumber, Operation[] selectedOperations);

        void SetPosting(int configurationNumber, Operation[] allOperations);

    }
}
