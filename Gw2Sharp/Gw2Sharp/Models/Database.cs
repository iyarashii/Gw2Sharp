﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Gw2Sharp.Models.DTOs;

namespace Gw2Sharp.Models
{
    // class responsible for local database connection and database methods
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        // constructor
        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<ItemNamesAndIds>().Wait();
        }

        // returns the item with the same name as the name passed as a parameter
        public Task<ItemNamesAndIds> GetItemAsync(string typedItemName)
        {
            //return _database.QueryAsync<ItemNamesAndIds>("SELECT * FROM ItemNamesAndIds WHERE name = ? COLLATE NOCASE", typedItemName);
            return _database.Table<ItemNamesAndIds>().Where(i => i.name == typedItemName).FirstOrDefaultAsync();
        }

        // inserts specified List<ItemNamesAndIds object into the database
        public Task<int> SaveItemsAndIdsAsync(List<ItemNamesAndIds> itemsAndIdsList)
        {
            return _database.InsertAllAsync(itemsAndIdsList);
        }

        // drops and recreates table that stores ItemNamesAndIds objects
        public void ClearItemNamesAndIdsTable()
        {
             _database.DropTableAsync<ItemNamesAndIds>().Wait();
             _database.CreateTableAsync<ItemNamesAndIds>().Wait();
        }
    }
}