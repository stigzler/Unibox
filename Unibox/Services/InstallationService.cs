using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Models;
using Unibox.Interfaces;

namespace Unibox.Services
{
    public class InstallationService : IInstallationService
    {
        DatabaseService databaseService;

        public InstallationService(DatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public InstallationModel AddNew()
        {
            int count = 1;

      // while (databaseService.Database.Collections.Installations.FindAll().Where(i => i.Name == $"New Installation [{count}]") != null)


            while (databaseService.Database.Collections.Installations.FindAll().Where(i => i.Name == $"New Installation [{count}]").Count() > 0)
            {
                count++;
            }

            string newInstallationName = $"New Installation [{count}]";

            InstallationModel installationModel = new InstallationModel
            {
                Name = newInstallationName,
                Added = DateTime.Now,
            };

            databaseService.Database.Collections.Installations.Insert(installationModel);

            return installationModel;
        }

        public void Update(InstallationModel installationModel)
        {
            throw new NotImplementedException();
        }
    }
}
