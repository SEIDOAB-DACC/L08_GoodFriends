// See https://aka.ms/new-console-template for more information
using Configuration;
using Models;
using Seido.Utilities.SeedGenerator;

const string _seedSource = "./friends-seeds.json";

Console.WriteLine("AppTest");

var fn = Path.GetFullPath(_seedSource);
var _seeder = new csSeedGenerator(fn);

Console.WriteLine("\nFriends:");
var _friends = _seeder.ItemsToList<csFriend>(5);
foreach (var item in _friends)
{
    Console.WriteLine(item);
}

Console.WriteLine("\nAddresses:");
var __addresses = _seeder.ItemsToList<csAddress>(5);
foreach (var item in __addresses)
{
    Console.WriteLine(item);
}

Console.WriteLine("\nPets:");
var _pets = _seeder.ItemsToList<csPet>(5);
foreach (var item in _pets)
{
    Console.WriteLine(item);
}

Console.WriteLine("\nQuotes:");
var _quotes = _seeder.ItemsToList<csQuote>(5);
foreach (var item in _quotes)
{
    Console.WriteLine(item);
}

/* Exercise
Seedgenerator - friends-seed.json
1. Change the Pet names to first names of the characters from the movie Fred Flintstone
2. Add country Spain, with some spanish cities and spanish street names

3. Assign randomly between 0 and 5 pets to a person. Friends cannot share a Pet (have same PetId)
4. Assign zero or one address to a friend, several friends can live on the same address (AddressId)
5. Assign zero to 5 quotes to a friend Friends can share the same quotes (QuoteId)
*/