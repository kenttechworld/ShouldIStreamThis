using ShouldIStreamThis.Service;
using ShouldIStreamThis.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Tommy;

namespace ShouldIStreamThis.Handlers
{
    internal class AppStartUpHandler
    {
        TomlConfigurationService tomlConfigurationService = new();
        public void StartupChecks()
        {
            CheckConfigFile();
        }

        private void CheckConfigFile()
        {
            if (!tomlConfigurationService.CheckIfTOMLFileExist())
            {
                tomlConfigurationService.MakeTOMLFile();
            }
        }

        
    }
}
