using System;
using Raven.Client;

namespace AggressiveCaching
{
    public class TestSettingsRepository
    {
        private readonly string _dbName;
        private IDocumentStore _store;

        public TestSettingsRepository(IDocumentStore store, string dbName)
        {
            _store = store;
            _dbName = dbName;
        }

        public string Store(TestSettings settings)
        {
            return Store(settings, null);
        }
        public string Store(TestSettings settings, string id)
        {
            using (var session = OpenSession())
            {
                session.Store(settings, id);
                session.SaveChanges();
                return session.Advanced.GetDocumentId(settings);
            }
        }

        public TestSettings LoadWithoutCache(string id)
        {
            using (var session = OpenSession())
            {
                return session.Load<TestSettings>(id);
            }
        }

        public TestSettings LoadWithCache(string id)
        {
            using (var session = OpenSession())
            {
                using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromDays(1)))
                {
                    return session.Load<TestSettings>(id);
                }
            }
        }

        private IDocumentSession OpenSession()
        {
            return _store.OpenSession(_dbName);
        }
    }
}