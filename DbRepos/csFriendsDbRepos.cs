using Seido.Utilities.SeedGenerator;
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
    const string _seedSource = "./friends-seeds.json";
    private ILogger<csFriendsDbRepos> _logger = null;

    #region used before csLoginService is implemented
    private string _dblogin = "sysadmin";
    //private string _dblogin = "gstusr";
    //private string _dblogin = "usr";
    //private string _dblogin = "supusr";
    #endregion

    #region only for layer verification
    private Guid _guid = Guid.NewGuid();
    private string _instanceHeartbeat = null;

    static public string Heartbeat { get; } = $"Heartbeat from namespace {nameof(DbRepos)}, class {nameof(csFriendsDbRepos)}";
    public string InstanceHeartbeat => _instanceHeartbeat;
    #endregion

    #region contructors
    public csFriendsDbRepos()
    {
        _instanceHeartbeat = $"Heartbeat from class {this.GetType()} with instance Guid {_guid}.";
    }
    public csFriendsDbRepos(ILogger<csFriendsDbRepos> logger):this()
    {
        _logger = logger;
        _logger.LogInformation(_instanceHeartbeat);
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
                    nrSeededFriends = await db.Friends.Where(f => f.Seeded).CountAsync(),
                    nrUnseededFriends = await db.Friends.Where(f => !f.Seeded).CountAsync(),
                    nrFriendsWithAddress = await db.Friends.Where(f => f.AddressId != null).CountAsync(),

                    nrSeededAddresses = await db.Addresses.Where(f => f.Seeded).CountAsync(),
                    nrUnseededAddresses = await db.Addresses.Where(f => !f.Seeded).CountAsync(),

                    nrSeededPets = await db.Pets.Where(f => f.Seeded).CountAsync(),
                    nrUnseededPets = await db.Pets.Where(f => !f.Seeded).CountAsync(),

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
            //First of all make sure the database is cleared from all seeded data
            await RemoveSeedAsync(usr, true);

            //Create a seeder
            var fn = Path.GetFullPath(_seedSource);
            var _seeder = new csSeedGenerator(fn);

            //Seeding the  quotes table
            var _quotes = _seeder.AllQuotes.Select(q => new csQuoteDbM(q)).ToList();
            //db.Quotes.AddRange(_quotes);

            #region Full seeding
            //Generate friends and addresses
            var _friends = _seeder.ItemsToList<csFriendDbM>(nrOfItems);
            var _addresses = _seeder.UniqueItemsToList<csAddressDbM>(nrOfItems);

            //Assign Address, Pets and Quotes to all the friends
            foreach (var friend in _friends)
            {
                friend.AddressDbM = (_seeder.Bool) ? _seeder.FromList(_addresses) : null;
                friend.PetsDbM =  _seeder.ItemsToList<csPetDbM>(_seeder.Next(0, 4));
                friend.QuotesDbM = _seeder.UniqueItemsPickedFromList(_seeder.Next(0, 6), _quotes);
            }

            //Note that all other tables are automatically set through csFriendDbM Navigation properties
            db.Friends.AddRange(_friends);
            #endregion

            //ExploreChangeTracker(db);

            //Prepare retuns data structure
            var _info = new adminInfoDbDto();

            _info.nrSeededQuotes = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csQuoteDbM)
               && entry.State == EntityState.Added);

            #region full seeding
            _info.nrSeededFriends = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csFriendDbM) && entry.State == EntityState.Added);
            _info.nrSeededAddresses = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csAddressDbM) && entry.State == EntityState.Added);
            _info.nrSeededPets = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csPetDbM) && entry.State == EntityState.Added);
            #endregion


            await db.SaveChangesAsync();
            return _info;
        }
    }


    public async Task<adminInfoDbDto> RemoveSeedAsync(loginUserSessionDto usr, bool seeded)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {

            db.Quotes.RemoveRange(db.Quotes.Where(f => f.Seeded == seeded));

            #region  full seeding
            //db.Pets.RemoveRange(db.Pets.Where(f => f.Seeded == seeded)); //not needed when cascade delete
            db.Friends.RemoveRange(db.Friends.Where(f => f.Seeded == seeded));
            db.Addresses.RemoveRange(db.Addresses.Where(f => f.Seeded == seeded));
            #endregion

            //ExploreChangeTracker(db);

            var _info = new adminInfoDbDto();
            if (seeded)
            {
                //Explore the changeTrackerNr of items to be deleted
                _info.nrSeededQuotes = db.ChangeTracker.Entries().Count(entry => 
                    (entry.Entity is csQuoteDbM) && entry.State == EntityState.Deleted);
                    
                #region  full seeding
                _info.nrSeededFriends = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csFriendDbM) && entry.State == EntityState.Deleted);
                _info.nrSeededAddresses = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csAddressDbM) && entry.State == EntityState.Deleted);
                _info.nrSeededPets = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csPetDbM) && entry.State == EntityState.Deleted);
                #endregion
            }
            else
            {
                //Explore the changeTrackerNr of items to be deleted
                _info.nrUnseededQuotes = db.ChangeTracker.Entries().Count(entry => 
                    (entry.Entity is csQuoteDbM) && entry.State == EntityState.Deleted);

                #region  full seeding
                _info.nrUnseededFriends = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csFriendDbM) && entry.State == EntityState.Deleted);
                _info.nrUnseededAddresses = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csAddressDbM) && entry.State == EntityState.Deleted);
                _info.nrUnseededPets = db.ChangeTracker.Entries().Count(entry => (entry.Entity is csPetDbM) && entry.State == EntityState.Deleted);
                #endregion
            }

            //do the actual deletion
            await db.SaveChangesAsync();
            return _info;
        }
    }
    #endregion
    
    #region exploring the ChangeTracker
    private void ExploreChangeTracker(csMainDbContext db)
    {
        foreach (var e in db.ChangeTracker.Entries())
        {
            if (e.Entity is csQuote q)
            {
                _logger.LogInformation(e.State.ToString());
                _logger.LogInformation(q.QuoteId.ToString());
            }
        }
    }
    #endregion

    #region Friends repo methods
    public async Task<IFriend> ReadFriendAsync(loginUserSessionDto usr, Guid id, bool flat)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
    }

    public async Task<csRespPageDTO<IFriend>> ReadFriendsAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
           IQueryable<csFriendDbM> _query;
            if (flat)
            {
                _query = db.Friends.AsNoTracking();
            }
            else
            {
                _query = db.Friends.AsNoTracking()
                    .Include(i => i.AddressDbM)
                    .Include(i => i.PetsDbM)
                    .Include(i => i.QuotesDbM);
            }

            var _ret = new csRespPageDTO<IFriend>()
            {
                PageItems = await _query.ToListAsync<IFriend>(),
            };
            return _ret;
        }
    }

    public async Task<IFriend> DeleteFriendAsync(loginUserSessionDto usr, Guid id)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
    }

    public async Task<IFriend> UpdateFriendAsync(loginUserSessionDto usr, csFriendCUdto itemDto)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
    }

    public async Task<IFriend> CreateFriendAsync(loginUserSessionDto usr, csFriendCUdto itemDto)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
    }
    #endregion


    #region Addresses repo methods
    public async Task<IAddress> ReadAddressAsync(loginUserSessionDto usr, Guid id, bool flat)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
    }

    public async Task<csRespPageDTO<IAddress>> ReadAddressesAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            IQueryable<csAddressDbM> _query;
            if (flat)
            {
                _query = db.Addresses.AsNoTracking();
            }
            else
            {
                _query = db.Addresses.AsNoTracking()
                    .Include(i => i.FriendsDbM)
                    .ThenInclude(i => i.PetsDbM)
                    .Include(i => i.FriendsDbM)
                    .ThenInclude(i => i.QuotesDbM);
            }
            
            var _ret = new csRespPageDTO<IAddress>()
            {
                PageItems = await _query.ToListAsync<IAddress>(),
            };
            return _ret;
        }
    }

    public async Task<IAddress> DeleteAddressAsync(loginUserSessionDto usr, Guid id)
    {
        //Notice cascade delete of firends living on the address and their pets
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
    }

    public async Task<IAddress> UpdateAddressAsync(loginUserSessionDto usr, csAddressCUdto itemDto)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
    }

    public async Task<IAddress> CreateAddressAsync(loginUserSessionDto usr, csAddressCUdto itemDto)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
    }
    #endregion


    #region Quotes repo methods
    public async Task<IQuote> ReadQuoteAsync(loginUserSessionDto usr, Guid id, bool flat)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
    }

    public async Task<csRespPageDTO<IQuote>> ReadQuotesAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            IQueryable<csQuoteDbM> _query;
            if (flat)
            {
                _query = db.Quotes.AsNoTracking();
            }
            else
            {
                _query = db.Quotes.AsNoTracking()
                    .Include(i => i.FriendsDbM)
                    .ThenInclude(i => i.PetsDbM)
                    .Include(i => i.FriendsDbM)
                    .ThenInclude(i => i.AddressDbM);
            }

            var _ret = new csRespPageDTO<IQuote>()
            {
                PageItems = await _query.ToListAsync<IQuote>(),
            };
            return _ret;           
        }
    }

    public async Task<IQuote> DeleteQuoteAsync(loginUserSessionDto usr, Guid id)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
    }

    public async Task<IQuote> UpdateQuoteAsync(loginUserSessionDto usr, csQuoteCUdto itemDto)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
 }

    public async Task<IQuote> CreateQuoteAsync(loginUserSessionDto usr, csQuoteCUdto itemDto)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
    }
    #endregion


    #region Pets repo methods
    public async Task<IPet> ReadPetAsync(loginUserSessionDto usr, Guid id, bool flat)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
    }

    public async Task<csRespPageDTO<IPet>> ReadPetsAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            IQueryable<csPetDbM> _query;
            if (flat)
            {
                _query = db.Pets.AsNoTracking();
            }
            else
            {
                _query = db.Pets.AsNoTracking()
                    .Include(i => i.FriendDbM)
                    .ThenInclude(i => i.AddressDbM)
                    .Include(i => i.FriendDbM)
                    .ThenInclude(i => i.QuotesDbM);
            }

            var _ret = new csRespPageDTO<IPet>()
            {
                PageItems = await _query.ToListAsync<IPet>(),
            };
            return _ret;        
        }
    }

    public async Task<IPet> DeletePetAsync(loginUserSessionDto usr, Guid id)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
    }

    public async Task<IPet> UpdatePetAsync(loginUserSessionDto usr, csPetCUdto itemDto)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
    }

    public async Task<IPet> CreatePetAsync(loginUserSessionDto usr, csPetCUdto itemDto)
    {
        using (var db = csMainDbContext.DbContext(_dblogin))
        {
            throw new NotImplementedException();        
        }
    }
    #endregion
}
