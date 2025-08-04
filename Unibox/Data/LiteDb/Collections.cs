using LiteDB;
using System.CodeDom;
using System.Security.Policy;
using Unibox.Data.Models;

namespace Unibox.Data.LiteDb
{
    public class Collections
    {
        public LiteDatabase Connection { get; set; }

        public Collections(LiteDatabase connection)
        {
            Connection = connection;
            InitialiseCollections();
        }
        public ILiteCollection<InstallationModel> Installations { get; set; }
        public ILiteCollection<PlatformModel> Platforms { get; set; }
        public ILiteCollection<PlatformFolderModel> PlatformFolders { get; set; }
     
        private void InitialiseCollections()
        {
            // ⚠️ NOTE: Don't forget to add .Include where needed!! ⚠️
             
            PlatformFolders = Connection.GetCollection<PlatformFolderModel>("platformFolders");
            Platforms = Connection.GetCollection<PlatformModel>("platforms").Include(p => p.PlatformFolders);
            Installations = Connection.GetCollection<InstallationModel>("installations").Include(i => i.Platforms);

            //Layouts = Connection.GetCollection<Layout>("layouts").Include(x => x.Components).Include("$.Components[*].Effects");



        }
    }
}