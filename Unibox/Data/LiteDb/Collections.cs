using LiteDB;
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
        //public ILiteCollection<Region> SsRegions { get; set; }
        //public ILiteCollection<ssEntities.System> SsSystems { get; set; }

        //// Gearbox
        //public ILiteCollection<LaunchboxScreenscraperSystemMap> SystemMaps { get; set; }
        //public ILiteCollection<Platform> Platforms { get; set; }
        //public ILiteCollection<Entities.Game> Games { get; set; }
        //public ILiteCollection<Screen> Screens { get; set; }
        //public ILiteCollection<ScreenLayouts> ScreenLayouts { get; set; }
        //public ILiteCollection<Computer> Computers { get; set; }
        //public ILiteCollection<Layout> Layouts { get; set; }
        //public ILiteCollection<Component> Components { get; set; }
        //public ILiteCollection<Effect> Effects { get; set; }
        //public ILiteCollection<TextOutline> TextOutlines { get; set; }
        //public ILiteCollection<Brush> Brushes { get; set; }
        //public ILiteCollection<LedGroupSetup> LedGroupSetups { get; set; }
        //public ILiteCollection<Installation> Installations { get; set; }
        //public ILiteCollection<Emulator> Emulators { get; set; }

        /// <summary>
        /// Sets up the collections for referencing.
        /// ⚠️ NOTE: Don't forget to add .Include where needed!! ⚠️
        /// </summary>
        /// 

        private void InitialiseCollections()
        {
            Installations = Connection.GetCollection<InstallationModel>("installations");

            // Screenscraper
            //SsLanguages = Connection.GetCollection<Language>("ssLanguages");
            //SsRegions = Connection.GetCollection<Region>("ssRegions");
            //SsSystems = Connection.GetCollection<ssEntities.System>("ssSystems");

            //// Gearbox
            //Platforms = Connection.GetCollection<Platform>("platforms").Include(p => p.ScreenLayouts);
            //Games = Connection.GetCollection<Entities.Game>("games");
            //SystemMaps = Connection.GetCollection<LaunchboxScreenscraperSystemMap>("systemMaps");
            //Screens = Connection.GetCollection<Screen>("screens").Include(s => s.GameLayouts).Include(s => s.HomeLayouts);
            //ScreenLayouts = Connection.GetCollection<ScreenLayouts>("screenLayouts").Include(sl => sl.Layouts).Include(sl => sl.Screen);
            //Computers = Connection.GetCollection<Computer>("computers").Include(c => c.BigBoxSupportAppOnLaunch)
            //    .Include(c => c.BigBoxSupportAppOnRefocus);
            //Layouts = Connection.GetCollection<Layout>("layouts").Include(x => x.Components).Include("$.Components[*].Effects");

            //Components = Connection.GetCollection<Component>("components").Include(x => x.Effects);
            //Effects = Connection.GetCollection<Effect>("effects");
            //TextOutlines = Connection.GetCollection<TextOutline>("textOutlines");
            //Brushes = Connection.GetCollection<Brush>("brushes");

            //LedGroupSetups = Connection.GetCollection<LedGroupSetup>("ledGroupSetups");
            //Installations = Connection.GetCollection<Installation>("installations");
            //Emulators = Connection.GetCollection<Emulator>("emulators");


        }
    }
}