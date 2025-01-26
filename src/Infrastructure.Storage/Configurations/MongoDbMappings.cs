using Contract.Messages;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using static Contract.DomainEvent;

namespace Infrastructure.Storage.Configurations
{
    public static class MongoDbMappings
    {
        public static void RegisterMappings()
        {
            // Configurar mapeamento da classe base Message
            BsonClassMap.RegisterClassMap<Message>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.MapMember(c => c.Timestamp)
                  .SetSerializer(new DateTimeOffsetSerializer(BsonType.String)); // Serializar Timestamp como string
                cm.MapMember(c => c.CorrelationId)
                  .SetSerializer(new NullableSerializer<Guid>(new GuidSerializer(GuidRepresentation.Standard))); // Configura Guid como Nullable
            });

            // Configurar mapeamento da classe NewMotorcycleCreatedEvent
            BsonClassMap.RegisterClassMap<NewMotorcycleCreatedEvent>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(false); // Define como não sendo uma classe raiz
                cm.MapIdMember(c => c.Id);
                cm.MapMember(c => c.Year);
                cm.MapMember(c => c.Model);
                cm.MapMember(c => c.LicensePlate);
            });
        }
    }
}
