﻿using Inventory.API.Entities;
using MongoDB.Driver;
using Shared.Configurations;

namespace Inventory.API.Persistence
{
    public class InventoryDbSeed
    {
        public async Task SeedDataAsync(IMongoClient? mongoClient, MongoDbDatabaseSettings databaseSettings)
        {
            var databaseName = databaseSettings.DatabaseName;
            var database = mongoClient.GetDatabase(databaseName);
            var inventoryCollection = database.GetCollection<InventoryEntry>("InventoryEntries");
            if(await inventoryCollection.EstimatedDocumentCountAsync() == 0)
            {
                await inventoryCollection.InsertManyAsync(getPreconfiguredInventoryEntries());
            }
        }

        private IEnumerable<InventoryEntry> getPreconfiguredInventoryEntries()
        {
            return new List<InventoryEntry>
            {
                new()
                {
                    Quantity = 10,
                    DocumentNo = Guid.NewGuid().ToString(),
                    ItemNo = "Lotus",
                    ExternalDocumentNo = Guid.NewGuid().ToString(),
                    DocumentType = Shared.Enums.Inventory.EDocumentType.Purchase
                },
                new()
                {
                    Quantity = 10,
                    DocumentNo = Guid.NewGuid().ToString(),
                    ItemNo = "Car",
                    ExternalDocumentNo = Guid.NewGuid().ToString(),
                    DocumentType = Shared.Enums.Inventory.EDocumentType.Purchase
                }
            };
        }
    }
}
