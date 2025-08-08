using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Services
{
    internal class GameService
    {
        private DatabaseService databaseService;

        public GameService(DatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }
    }
}