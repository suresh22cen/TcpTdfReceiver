using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBusinessLogic
{
    using System.Configuration;
    using Raven.Client;
    using Raven.Client.Document;

    public class RaceDB
    {
        IDocumentStore _store;

        public RaceDB(string connectionString, string database)
        {
            _store = new DocumentStore { Url = connectionString, DefaultDatabase = database };
            _store.Initialize();            
        }

        /// <summary>
		/// The save Data Record from Android App
		/// </summary>
		/// <param name="record">
		/// The Data Record
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// boolean if operation succeeds or fails
		/// </returns>
		public async Task<bool> SaveDataRecord(DataRecord record)
        {                       
            using (var session = this._store.OpenAsyncSession())
            {
                await session.StoreAsync(record);
                await session.SaveChangesAsync();
            }

            return true;
        }
    }
}
