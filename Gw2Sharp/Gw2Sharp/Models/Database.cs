using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Gw2Sharp.Models.DTOs;
using System;

namespace Gw2Sharp.Models
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<ItemNamesAndIds>().Wait();
        }
        public Task<ItemNamesAndIds> GetItemAsync(string typedItemName)
        {
            //return _database.QueryAsync<ItemNamesAndIds>("SELECT * FROM ItemNamesAndIds WHERE name = ? COLLATE NOCASE", typedItemName);
            return _database.Table<ItemNamesAndIds>().Where(i => i.name == typedItemName).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemsAndIdsAsync(List<ItemNamesAndIds> itemsAndIdsList)
        {
            return _database.InsertAllAsync(itemsAndIdsList);
        }

        public void ClearItemNamesAndIdsTable()
        {
            //return _database.ExecuteAsync("DELETE FROM ItemNamesAndIds");
             _database.DropTableAsync<ItemNamesAndIds>().Wait();
             _database.CreateTableAsync<ItemNamesAndIds>().Wait();
        }
    }
}
