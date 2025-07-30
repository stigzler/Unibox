using LiteDB;
using System;
using System.Drawing;
using System.Text;

namespace Unibox.Data.LiteDb
{
    public class Database : IDisposable
    {
        private bool disposedValue;
        public Collections Collections { get; set; }
        public LiteDatabase Connection { get; set; }
        public ConnectionParameters ConnectionParams { get; set; }
        public Exception? ConnectionException { get; set; } = null;

        public bool DatabaseOpen { get; set; } = false;

        public Database(ConnectionParameters connectionParameters)
        {
            ConnectionParams = connectionParameters;
            try
            {
                OpenDatabase();
                DatabaseOpen = true;
            }
            catch (Exception ex)
            {
                ConnectionException = ex;
            }
        }

        public enum ConnectionType
        {
            Direct,
            Shared
        }

        public void CloseDatabase()
        {
            Connection.Dispose();
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void OpenDatabase()
        {
            Connection = new LiteDatabase(ConnectionParams.ToString());

            SetupDatabase();

            Collections = new Collections(Connection);
        }

        private void SetupDatabase()
        {
            DoBsonMappings();
        }

        private void DoBsonMappings()
        {
            BsonMapper.Global.RegisterType<Color>
                (
                serialize: c => $"{c.A},{c.R},{c.G},{c.B}",
                deserialize: s =>
                {
                    string[] c = s.AsString.Split(',');
                    return Color.FromArgb(int.Parse(c[0]), int.Parse(c[1]), int.Parse(c[2]), int.Parse(c[3]));
                }
                );
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Connection.Dispose();
                disposedValue = true;
            }
        }



    }
}