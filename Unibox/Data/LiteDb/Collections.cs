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
        public ILiteCollection<GameModel> Games { get; set; }

        // Below removed because do not store these via collections - rather stored in Installation record.
        //public ILiteCollection<PlatformModel> Platforms { get; set; }
        //public ILiteCollection<PlatformFolderModel> PlatformFolders { get; set; }
        public ILiteCollection<SsMediaType> SsMediaTypes { get; set; }

        public ILiteCollection<SsSystem> SsSystems { get; set; }
        public ILiteCollection<LbPlatform> LbPlatforms { get; set; }
        public ILiteCollection<LbMediaType> LbMediaTypes { get; set; }
        public ILiteCollection<LbSsMediaTypeMap> LbSsMediaTypeMap { get; set; }
        public ILiteCollection<LbSsSystemMap> LbSsSystemsMap { get; set; }

        private void InitialiseCollections()
        {
            // ⚠️ NOTE: Don't forget to add .Include where needed!! ⚠️

            //PlatformFolders = Connection.GetCollection<PlatformFolderModel>("platformFolders");
            //Platforms = Connection.GetCollection<PlatformModel>("platforms").Include(p => p.PlatformFolders);
            Installations = Connection.GetCollection<InstallationModel>("installations");
            Games = Connection.GetCollection<GameModel>("games");
            SsMediaTypes = Connection.GetCollection<SsMediaType>("ssMediaTypes");
            SsSystems = Connection.GetCollection<SsSystem>("ssSystems");
            LbPlatforms = Connection.GetCollection<LbPlatform>("lbPlatforms");
            LbMediaTypes = Connection.GetCollection<LbMediaType>("lbMediaTypes");
            LbSsMediaTypeMap = Connection.GetCollection<LbSsMediaTypeMap>("lbSsMediaTypeMap");
            LbSsSystemsMap = Connection.GetCollection<LbSsSystemMap>("lbSsSystemsMap");
        }
    }
}