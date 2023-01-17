using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace CsharpExamples.UsageExamples.InsertOne;

public class InsertOne
{
    private static IMongoCollection<Restaurant> _restaurantsCollection;
    private const string MongoConnectionString = "<Your MongoDB URI>";

    public static void Main(string[] args)
    {
        Setup();

        Console.WriteLine("Inserting a document...");
        InsertOneRestaurant();

        var filter = Builders<Restaurant>.Filter
            .Eq(r => r.Name, "Mongo's Pizza");

        // find and print newly inserted document
        var document = _restaurantsCollection.Find(filter).FirstOrDefault();

        Console.WriteLine($"Document Inserted: {document.ToBsonDocument()}");

        Cleanup();
    }

    private static void InsertOneRestaurant()
    {
        // delete sample document if already exists
        Cleanup();

        // start-insert-one
        Restaurant newRestaurant = new()
        {
            Name = "Mongo's Pizza",
            RestaurantId = "12345",
            Cuisine = "Pizza",
            Address = new BsonDocument
            {
                {"street", "Pizza St"},
                {"zipcode", "10003"}
            },
            Borough = "Manhattan",
        };

        _restaurantsCollection.InsertOne(newRestaurant);
        // end-insert-one
    }

    private static void Setup()
    {
        // This allows automapping of the camelCase database fields to our models. 
        var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
        ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);

        // Establish the connection to MongoDB and get the restaurants database
        var mongoClient = new MongoClient(MongoConnectionString);
        var restaurantsDatabase = mongoClient.GetDatabase("sample_restaurants");
        _restaurantsCollection = restaurantsDatabase.GetCollection<Restaurant>("restaurants");
    }

    private static void Cleanup()
    {
        var filter = Builders<Restaurant>.Filter
            .Eq(r => r.Name, "Mongo's Pizza");

        _restaurantsCollection.DeleteOne(filter);
    }
}