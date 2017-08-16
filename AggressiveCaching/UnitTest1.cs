using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client;
using Raven.Client.Document;

namespace AggressiveCaching
{
    [TestClass]
    public class UnitTest1
    {
        
        [TestMethod]
        public void WithAggressiveCache()
        {
            var entity = new TestSettings("123");

            using (var store = NewDocumentStore())
            {
                var repository = new TestSettingsRepository(store, "Northwind - AC");

                // add new entity to db
                var entityId = repository.Store(entity);

                // check if entity was stored in db
                var testSettingsFromDb = repository.LoadWithCache(entityId);
                Assert.AreEqual("123", testSettingsFromDb.Number);

                // change value of entity and save to db
                testSettingsFromDb.Number = "456";
                repository.Store(testSettingsFromDb, entityId);

                // load entity from db with cache
                var testSettingsFromDb2 = repository.LoadWithCache(entityId);
                Assert.AreEqual("456", testSettingsFromDb2.Number, $"Expected number 456 but cached version was {testSettingsFromDb2.Number}");
            }
        }

        [TestMethod]
        public void WithoutCache()
        {
            var entity = new TestSettings("123");

            using (var store = NewDocumentStore())
            {
                var repository = new TestSettingsRepository(store, "Northwind - AC");

                // add new entity to db
                var entityId = repository.Store(entity);

                // check if entity was stored in db
                var testSettingsFromDb = repository.LoadWithoutCache(entityId);
                Assert.AreEqual("123", testSettingsFromDb.Number);

                // change value of entity and save to db
                testSettingsFromDb.Number = "456";
                repository.Store(testSettingsFromDb, entityId);

                // load entity from db with cache
                var testSettingsFromDb2 = repository.LoadWithoutCache(entityId);
                Assert.AreEqual("456", testSettingsFromDb2.Number, $"Expected number 456 but cached version was {testSettingsFromDb2.Number}");
            }
        }

        private DocumentStore NewDocumentStore()
        {
            var store = new DocumentStore
            {
                Url = "http://localhost:8080/"
            };
            store.Initialize();
            return store;
        }
    }
}
