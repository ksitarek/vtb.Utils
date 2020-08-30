using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vtb.Utils.MongoDb;

namespace vtb.Utils.Tests.Integrations
{
    public class MongoDbBaseRepositoryTests : MongoIntegrationTest
    {
        private const string _collectionName = "testDocuments";
        private IMongoCollection<TestDoc> _collection;
        private MongoDbTestRepository _repository;
        private Mock<ITenantIdProvider> _tenantIdProviderMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            CreateConnection();

            _tenantIdProviderMock = new Mock<ITenantIdProvider>();
            _tenantIdProviderMock.Setup(x => x.TenantId).Returns(Guid.Parse("49e49c89-7653-48d7-a263-899be2245eec"));
        }

        [SetUp]
        public void Setup()
        {
            _collection = _database.GetCollection<TestDoc>(_collectionName);
            _collection.InsertMany(new List<TestDoc>
            {
                new TestDoc
                {
                    Id = Guid.Parse("c3c7e5e9-6a11-407f-aeef-b0b286933eac"),
                    TenantId = Guid.Parse("49e49c89-7653-48d7-a263-899be2245eec"), Name = "Alpha"
                },
                new TestDoc
                {
                    Id = Guid.Parse("c9abb508-0fed-4d1a-82ec-da1149b9965d"),
                    TenantId = Guid.Parse("49e49c89-7653-48d7-a263-899be2245eec"), Name = "Beta"
                },
                new TestDoc
                {
                    Id = Guid.Parse("45f05cd4-5509-4bf8-b57c-efe96940e27f"),
                    TenantId = Guid.Parse("2a2d1781-6b96-4a55-9cb5-5f7c80aac27d"), Name = "Theta"
                }
            });

            _repository = new MongoDbTestRepository(_database, _tenantIdProviderMock.Object, _collectionName);
        }

        [TearDown]
        public void TearDown()
        {
            _database.DropCollection(_collectionName);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            DestroyConnection();
        }

        [Test]
        public void Throws_When_No_Collection_Name_Set()
        {
            var repo = new MongoDbTestRepository(_database, _tenantIdProviderMock.Object, string.Empty);
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await repo.GetByIdAsync(Guid.Parse("c3c7e5e9-6a11-407f-aeef-b0b286933eac")));
        }

        [Test]
        public async Task GetByIdAsync_Will_Return_Single_Document()
        {
            var doc = await _repository.GetByIdAsync(Guid.Parse("c3c7e5e9-6a11-407f-aeef-b0b286933eac"));
            Assert.AreEqual("Alpha", doc.Name);
        }

        [Test]
        public async Task GetByIdAsync_Will_Return_Null_If_Entity_In_Another_Tenant()
        {
            var doc = await _repository.GetByIdAsync(Guid.Parse("45f05cd4-5509-4bf8-b57c-efe96940e27f"));
            Assert.IsNull(doc);
        }

        [Test]
        public void GetByIdAsync_Will_Throw_ArgumentException_When_Guid_Empty()
        {
            var e = Assert.ThrowsAsync<ArgumentException>(() => _repository.GetByIdAsync(Guid.Empty));
            Assert.AreEqual(e.Message, "id");
        }

        [Test]
        public async Task AddOneAsync_Will_Insert_Entity_To_Collection()
        {
            await _repository.AddOneAsync(new TestDoc
            { Id = Guid.Parse("08f9fefd-c16e-412d-8c95-282abc476d72"), Name = "Gamma" });
            Assert.AreEqual(3,
                await _collection.CountDocumentsAsync(Builders<TestDoc>.Filter.Eq(x => x.TenantId,
                    _tenantIdProviderMock.Object.TenantId)));

            var documents = await _collection
                .Find(Builders<TestDoc>.Filter.Eq(x => x.TenantId, _tenantIdProviderMock.Object.TenantId))
                .ToListAsync();

            Assert.AreEqual("Alpha", documents[0].Name);
            Assert.AreEqual("Beta", documents[1].Name);
            Assert.AreEqual("Gamma", documents[2].Name);
        }

        [Test]
        public async Task BatchAddAsync_Will_Insert_Multiple_Entities_To_Collection()
        {
            await _repository.BatchAddAsync(
                new List<TestDoc>
                {
                    new TestDoc {Id = Guid.Parse("08f9fefd-c16e-412d-8c95-282abc476d72"), Name = "Gamma"},
                    new TestDoc {Id = Guid.Parse("527ebcc7-8d5c-4ee7-92c5-f1d96d408fb7"), Name = "Epsilon"}
                }.ToArray());
            Assert.AreEqual(4,
                await _collection.CountDocumentsAsync(Builders<TestDoc>.Filter.Eq(x => x.TenantId,
                    _tenantIdProviderMock.Object.TenantId)));

            var documents = await _collection
                .Find(Builders<TestDoc>.Filter.Eq(x => x.TenantId, _tenantIdProviderMock.Object.TenantId))
                .ToListAsync();

            Assert.AreEqual("Alpha", documents[0].Name);
            Assert.AreEqual("Beta", documents[1].Name);
            Assert.AreEqual("Gamma", documents[2].Name);
            Assert.AreEqual("Epsilon", documents[3].Name);
        }

        [Test]
        public async Task BatchReplaceAsync_Will_Replace_Multiple_Entities_In_Collection()
        {
            await _repository.BatchReplaceAsync(
                new List<TestDoc>
                {
                    new TestDoc {Id = Guid.Parse("c3c7e5e9-6a11-407f-aeef-b0b286933eac"), Name = "Alpha_Beta"},
                    new TestDoc {Id = Guid.Parse("c9abb508-0fed-4d1a-82ec-da1149b9965d"), Name = "Beta_Gamma"}
                }.ToArray());

            Assert.AreEqual(2,
                await _collection.CountDocumentsAsync(Builders<TestDoc>.Filter.Eq(x => x.TenantId,
                    _tenantIdProviderMock.Object.TenantId)));

            var documents = await _collection
                .Find(Builders<TestDoc>.Filter.Eq(x => x.TenantId, _tenantIdProviderMock.Object.TenantId))
                .ToListAsync();
            Assert.AreEqual("Alpha_Beta", documents[0].Name);
            Assert.AreEqual("Beta_Gamma", documents[1].Name);
        }

        [Test]
        public async Task BatchReplaceAsync_Will_Insert_Not_Found_Entities()
        {
            await _repository.BatchReplaceAsync(
                new List<TestDoc>
                {
                    new TestDoc {Id = Guid.Parse("08f9fefd-c16e-412d-8c95-282abc476d72"), Name = "Gamma_Epsilon"},
                    new TestDoc {Id = Guid.Parse("c9abb508-0fed-4d1a-82ec-da1149b9965d"), Name = "Beta_Gamma"}
                }.ToArray());

            Assert.AreEqual(3,
                await _collection.CountDocumentsAsync(Builders<TestDoc>.Filter.Eq(x => x.TenantId,
                    _tenantIdProviderMock.Object.TenantId)));

            var documents = await _collection
                .Find(Builders<TestDoc>.Filter.Eq(x => x.TenantId, _tenantIdProviderMock.Object.TenantId))
                .ToListAsync();
            Assert.AreEqual("Alpha", documents[0].Name);
            Assert.AreEqual("Beta_Gamma", documents[1].Name);
            Assert.AreEqual("Gamma_Epsilon", documents[2].Name);
        }

        [Test]
        public async Task ReplaceAsync_Will_Replace_Entity_In_Collection()
        {
            await _repository.ReplaceAsync(new TestDoc
            { Id = Guid.Parse("c3c7e5e9-6a11-407f-aeef-b0b286933eac"), Name = "Alpha-Omega" });

            var doc = await _collection
                .Find(Builders<TestDoc>.Filter.Eq(e => e.Id, Guid.Parse("c3c7e5e9-6a11-407f-aeef-b0b286933eac")))
                .FirstOrDefaultAsync();

            Assert.IsNotNull(doc);
            Assert.AreEqual("Alpha-Omega", doc.Name);
        }

        [Test]
        public async Task DeleteOneAsync_Will_Remove_Entity_From_Collection()
        {
            await _repository.DeleteOneAsync(Guid.Parse("c9abb508-0fed-4d1a-82ec-da1149b9965d"));
            Assert.AreEqual(1,
                await _collection.CountDocumentsAsync(Builders<TestDoc>.Filter.Eq(x => x.TenantId,
                    _tenantIdProviderMock.Object.TenantId)));
        }

        private class TestDoc : IMongoDbEntity
        {
            public string Name { get; set; }

            [BsonId] public Guid Id { get; set; }
            [BsonRequired] public Guid TenantId { get; set; }
        }

        private class MongoDbTestRepository : MongoDbBaseRepository<TestDoc>
        {
            public MongoDbTestRepository(IMongoDatabase mongoDb, ITenantIdProvider tenantIdProvider,
                string collectionName) : base(mongoDb, tenantIdProvider, collectionName)
            {
            }
        }
    }
}