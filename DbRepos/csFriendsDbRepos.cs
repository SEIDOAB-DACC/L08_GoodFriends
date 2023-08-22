using Configuration;
using Models;
using Models.DTO;
using DbModels;
using DbContext;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata;

//DbRepos namespace is a layer to abstract the detailed plumming of
//retrieveing and modifying and data in the database using EFC.

//DbRepos implements database CRUD functionality using the DbContext
namespace DbRepos;

public class csFriendsDbRepos
{
    private ILogger<csFriendsDbRepos> _logger = null;

    #region used before csLoginService is implemented
    private string _dblogin = "sysadmin";
    //private string _dblogin = "gstusr";
    //private string _dblogin = "usr";
    //private string _dblogin = "supusr";
    #endregion

    #region only for layer verification
    private Guid _guid = Guid.NewGuid();
    private string _instanceHello = null;

    static public string Hello { get; } = $"Hello from namespace {nameof(DbRepos)}, class {nameof(csFriendsDbRepos)}";
    public string InstanceHello => _instanceHello;
    #endregion


    #region contructors
    public csFriendsDbRepos()
    {
        _instanceHello = $"Hello from class {this.GetType()} with instance Guid {_guid}.";
    }
    public csFriendsDbRepos(ILogger<csFriendsDbRepos> logger):this()
    {
        _logger = logger;
        _logger.LogInformation(_instanceHello);
    }
    #endregion


    #region Admin repo methods
    public async Task<gstusrInfoAllDto> InfoAsync()
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            var _info = new gstusrInfoAllDto
            {
                Db = new gstusrInfoDbDto
                {
                    #region full seeding
                    nrSeededFriends = await db.Friends.Where(f => f.Seeded).CountAsync(),
                    nrUnseededFriends = await db.Friends.Where(f => !f.Seeded).CountAsync(),
                    nrFriendsWithAddress = await db.Friends.Where(f => f.AddressId != null).CountAsync(),

                    nrSeededAddresses = await db.Addresses.Where(f => f.Seeded).CountAsync(),
                    nrUnseededAddresses = await db.Addresses.Where(f => !f.Seeded).CountAsync(),

                    nrSeededPets = await db.Pets.Where(f => f.Seeded).CountAsync(),
                    nrUnseededPets = await db.Pets.Where(f => !f.Seeded).CountAsync(),
                    #endregion

                    nrSeededQuotes = await db.Quotes.Where(f => f.Seeded).CountAsync(),
                    nrUnseededQuotes = await db.Quotes.Where(f => !f.Seeded).CountAsync(),
                }
            };

            return _info;
        }
    }

    public async Task<adminInfoDbDto> SeedAsync(loginUserSessionDto usr, int nrOfItems)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            var _seeder = new csSeedGenerator();

            var qcount = await db.Quotes.CountAsync(q => q.Seeded);
            if (qcount == 0)
            {
                //Start by generating the quotes table
                var _quotes = _seeder.AllQuotes.Select(q => new csQuoteDbM(q)).ToList();
                foreach (var q in _quotes)
                {
                    db.Quotes.Add(q);
                }

                //ExploreChangeTracker(db);
                await db.SaveChangesAsync();
            }

            #region full seeding
            //Now _seededquotes is always the content of the Quotes table
            var _seededquotes = await db.Quotes.ToListAsync();

            //Generate friends and addresses
            var _friends = _seeder.ToList<csFriendDbM>(nrOfItems);

            var _existingaddresses = await db.Addresses.ToListAsync();
            var _addresses = _seeder.ToListUnique<csAddressDbM>(nrOfItems, _existingaddresses);

            for (int c = 0; c < nrOfItems; c++)
            {
                //Assign addresses. Friends could live on the same address
                _friends[c].AddressDbM = (_seeder.Bool) ? _seeder.FromList(_addresses) : null;

                //Create between 0 and 3 pets
                var _pets = new List<csPetDbM>();
                for (int i = 0; i < _seeder.Next(0, 4); i++)
                {
                    //A Pet can only be owned by one friend
                    _pets.Add(new csPetDbM().Seed(_seeder));
                }
                _friends[c].PetsDbM = _pets;


                //Create between 0 and 5 quotes
                var _favoriteQuotes = _seeder.FromListUnique(_seeder.Next(0, 6), _seededquotes);
                _friends[c].QuotesDbM = _favoriteQuotes;

            }

            //Add the seeded items to EFC, ChangeTracker will now pick it up
            foreach (var f in _friends)
            {
                db.Friends.Add(f);
            }
            #endregion


            var _info = new adminInfoDbDto();

            #region full seed
            _info.nrSeededFriends = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csFriendDbM) && entry.State == EntityState.Added);
            _info.nrSeededAddresses = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csAddressDbM) && entry.State == EntityState.Added);
            _info.nrSeededPets = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csPetDbM) && entry.State == EntityState.Added);
            #endregion

            _info.nrSeededQuotes = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csQuoteDbM)
               && entry.State == EntityState.Added);

            #region full seed
            await db.SaveChangesAsync();
            #endregion

            return _info;
        }
    }


    public async Task<adminInfoDbDto> RemoveSeedAsync(loginUserSessionDto usr, bool seeded)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            #region full seeding
            db.Friends.RemoveRange(db.Friends.Where(f => f.Seeded == seeded));

            //db.Pets.RemoveRange(db.Pets.Where(f => f.Seeded == seeded)); //not needed when cascade delete

            db.Addresses.RemoveRange(db.Addresses.Where(f => f.Seeded == seeded));
            #endregion

            db.Quotes.RemoveRange(db.Quotes.Where(f => f.Seeded == seeded));

            //ExploreChangeTracker(db);

            var _info = new adminInfoDbDto();
            if (seeded)
            {
                //Explore the changeTrackerNr of items to be deleted

                #region full seeding
                _info.nrSeededFriends = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csFriendDbM) && entry.State == EntityState.Deleted);
                _info.nrSeededAddresses = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csAddressDbM) && entry.State == EntityState.Deleted);
                _info.nrSeededPets = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csPetDbM) && entry.State == EntityState.Deleted);
                #endregion

                _info.nrSeededQuotes = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csQuoteDbM) && entry.State == EntityState.Deleted);
            }
            else
            {
                //Explore the changeTrackerNr of items to be deleted
                #region full seeding
                _info.nrUnseededFriends = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csFriendDbM) && entry.State == EntityState.Deleted);
                _info.nrUnseededAddresses = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csAddressDbM) && entry.State == EntityState.Deleted);
                _info.nrUnseededPets = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csPetDbM) && entry.State == EntityState.Deleted);
                #endregion

                _info.nrUnseededQuotes = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csQuoteDbM) && entry.State == EntityState.Deleted);
            }

            //do the actual deletion
            await db.SaveChangesAsync();
            return _info;
        }
    }

    #region exploring the ChangeTracker
    private static void ExploreChangeTracker(csMainDbContext db)
    {
        foreach (var e in db.ChangeTracker.Entries())
        {
            if (e.Entity is csQuote q)
            {
                Console.WriteLine(e.State);
                Console.WriteLine(q.QuoteId);
            }
        }
    }
    #endregion

    #endregion


    #region Friends repo methods
    public async Task<IFriend> ReadFriendAsync(loginUserSessionDto usr, Guid id, bool flat)
        => throw new NotImplementedException();

    public async Task<List<IFriend>> ReadFriendsAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            if (!flat)
            {
                //make sure the model is fully populated, try without include.
                //remove tracking for all read operations for performance and to avoid recursion/circular access
                var _query = db.Friends.AsNoTracking().Include(i => i.AddressDbM).Include(i => i.PetsDbM)
                    .Include(i => i.QuotesDbM);

                return await _query.ToListAsync<IFriend>();
            }
            else
            {
                //Not fully populated, compare the SQL Statements generated
                //remove tracking for all read operations for performance and to avoid recursion/circular access
                var _query = db.Friends.AsNoTracking();

                return await _query.ToListAsync<IFriend>();
            }
        }
    }
    public async Task<IFriend> DeleteFriendAsync(loginUserSessionDto usr, Guid id)
       => throw new NotImplementedException();

    public async Task<IFriend> UpdateFriendAsync(loginUserSessionDto usr, csFriendCUdto itemDto)
       => throw new NotImplementedException();

    public async Task<IFriend> CreateFriendAsync(loginUserSessionDto usr, csFriendCUdto itemDto)
       => throw new NotImplementedException();
    #endregion


    #region Addresses repo methods
    public async Task<IAddress> ReadAddressAsync(loginUserSessionDto usr, Guid id, bool flat)
       => throw new NotImplementedException();

    public async Task<List<IAddress>> ReadAddressesAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            if (!flat)
            {
                //make sure the model is fully populated, try without include.
                //remove tracking for all read operations for performance and to avoid recursion/circular access
                var _query = db.Addresses.AsNoTracking().Include(i => i.FriendsDbM);

                return await _query.ToListAsync<IAddress>();
            }
            else
            {
                //Not fully populated, compare the SQL Statements generated
                //remove tracking for all read operations for performance and to avoid recursion/circular access
                var _query = db.Addresses.AsNoTracking();

                return await _query.ToListAsync<IAddress>();
            }
        }
    }

    public async Task<IAddress> DeleteAddressAsync(loginUserSessionDto usr, Guid id)
       => throw new NotImplementedException();

    public async Task<IAddress> UpdateAddressAsync(loginUserSessionDto usr, csAddressCUdto itemDto)
       => throw new NotImplementedException();

    public async Task<IAddress> CreateAddressAsync(loginUserSessionDto usr, csAddressCUdto itemDto)
       => throw new NotImplementedException();
    #endregion


    #region Quotes repo methods
    public async Task<IQuote> ReadQuoteAsync(loginUserSessionDto usr, Guid id, bool flat)
       => throw new NotImplementedException();

    public async Task<List<IQuote>> ReadQuotesAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            if (!flat)
            {
                //make sure the model is fully populated, try without include.
                //remove tracking for all read operations for performance and to avoid recursion/circular access
                var _query = db.Quotes.AsNoTracking().Include(i => i.FriendsDbM);

                return await _query.ToListAsync<IQuote>();
            }
            else
            {
                //Not fully populated, compare the SQL Statements generated
                //remove tracking for all read operations for performance and to avoid recursion/circular access
                var _query = db.Quotes.AsNoTracking();

                return await _query.ToListAsync<IQuote>();
            }
        }
    }

    public async Task<IQuote> DeleteQuoteAsync(loginUserSessionDto usr, Guid id)
       => throw new NotImplementedException();

    public async Task<IQuote> UpdateQuoteAsync(loginUserSessionDto usr, csQuoteCUdto itemDto)
       => throw new NotImplementedException();

    public async Task<IQuote> CreateQuoteAsync(loginUserSessionDto usr, csQuoteCUdto itemDto)
       => throw new NotImplementedException();
    #endregion


    #region Pets repo methods
    public async Task<IPet> ReadPetAsync(loginUserSessionDto usr, Guid id, bool flat)
       => throw new NotImplementedException();

    public async Task<List<IPet>> ReadPetsAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            if (!flat)
            {
                //make sure the model is fully populated, try without include.
                //remove tracking for all read operations for performance and to avoid recursion/circular access
                var _query = db.Pets.AsNoTracking().Include(i => i.FriendDbM);

                return await _query.ToListAsync<IPet>();
            }
            else
            {
                //Not fully populated, compare the SQL Statements generated
                //remove tracking for all read operations for performance and to avoid recursion/circular access
                var _query = db.Pets.AsNoTracking();

                return await _query.ToListAsync<IPet>();
            }
        }
    }

    public async Task<IPet> DeletePetAsync(loginUserSessionDto usr, Guid id)
       => throw new NotImplementedException();

    public async Task<IPet> UpdatePetAsync(loginUserSessionDto usr, csPetCUdto itemDto)
       => throw new NotImplementedException();

    public async Task<IPet> CreatePetAsync(loginUserSessionDto usr, csPetCUdto itemDto)
       => throw new NotImplementedException();
    #endregion
}
