using LiteDB;
using System.CodeDom;
using System.Security.Policy;
using Unibox.Admin.Data.Models;

namespace Unibox.Admin.Data.LiteDb
{
    public class Collections
    {
        public LiteDatabase Connection { get; set; }

        public Collections(LiteDatabase connection)
        {
            Connection = connection;
            InitialiseCollections();
        }

        internal ILiteCollection<SsSystem> SsSystems { get; set; }

        private void InitialiseCollections()
        {
            // ⚠️ NOTE: Don't forget to add .Include where needed!! ⚠️

            SsSystems = Connection.GetCollection<SsSystem>("ssSystems");
        }
    }
}