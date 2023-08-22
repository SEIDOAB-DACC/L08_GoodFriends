using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Configuration;
using Models;
using Models.DTO;

using DbModels;
using DbContext;
using DbRepos;
using Services;
using System.Linq;

//Service namespace is an abstraction of using services without detailed knowledge
//of how the service is implemented.
//Service is used by the application layer using interfaces. Thus, the actual
//implementation of a service can be application dependent without changing code
//at application
namespace Services
{
    //IFriendsService ensures application layer can access csFriendsServiceModel
    //Friends model (without database) OR access csFriendsServiceDbRepos
    //FriendsDbM model (with database)class csFriendsServiceDbRepos without
    //without any code change
    public class csFriendsServiceModel : IFriendsService
    {
        private ILogger<csFriendsServiceModel> _logger = null;
        private object _locker = new object();

        #region only for layer verification
        private Guid _guid = Guid.NewGuid();
        private string _instanceHello;

        public string InstanceHello => _instanceHello;

        static public string Hello { get; } = $"Hello from namespace {nameof(Services)}, class {nameof(csFriendsServiceModel)}" +

            // added after project references is setup
            $"\n   - {csFriendsDbRepos.Hello}" +
            $"\n   - {csMainDbContext.Hello}";
        #endregion

        #region constructors
        public csFriendsServiceModel(ILogger<csFriendsServiceModel> logger)
        {
            _logger = logger;

            //only for layer verification
            _instanceHello = $"Hello from class {this.GetType()} with instance Guid {_guid}. " +
                $"Will use ModelOnly, no repo.";

            //Different message levels
            _logger.LogInformation($"Info: {_instanceHello}");
            _logger.LogWarning($"Warning: {_instanceHello}");
            _logger.LogError($"Error: {_instanceHello}");
            _logger.LogCritical($"Critical: {_instanceHello}");
        }
        #endregion

        private List<csFriend> _friends = new List<csFriend>();

        public Task<adminInfoDbDto> RemoveSeedAsync(loginUserSessionDto usr, bool seeded) => Task.Run(() =>
        {
            lock (_locker) { return RemoveSeed(usr, seeded); }
        });
        public adminInfoDbDto RemoveSeed(loginUserSessionDto usr, bool seeded)
        {
            //A bit of Linq refresh
            var _info = new adminInfoDbDto();
            _info.nrSeededFriends = _friends.Count(i => i.Seeded == seeded);
            _info.nrSeededAddresses = _friends.Where(i => i.Seeded == seeded && i.Address != null).Distinct().Count();
            _info.nrSeededPets = _friends.Where(i => i.Seeded == seeded && i.Pets != null).ToList().Sum(i => i.Pets.Count);

            //actually remove
            _friends.RemoveAll(f => f.Seeded == seeded);

            return _info;
        }


        public Task<adminInfoDbDto> SeedAsync(loginUserSessionDto usr, int nrOfItems) => Task.Run(() =>
        {
            lock (_locker) { return Seed(usr, nrOfItems); }
        });
        public adminInfoDbDto Seed(loginUserSessionDto usr, int nrOfItems)
        {
            var _seeder = new csSeedGenerator();

            _friends = _seeder.ToList<csFriend>(nrOfItems);

            #region extending the seeding
            var _addresses = _seeder.ToList<csAddress>(nrOfItems);

            //Now _seededquotes is always the content of the Quotes table
            var _seededquotes = _seeder.AllQuotes.Select(q => new csQuote(q)).ToList();

            for (int c = 0; c < nrOfItems; c++)
            {
                //Assign addresses. Friends could live on the same address
                _friends[c].Address = (_seeder.Bool) ? _seeder.FromList(_addresses) : null;

                //Create between 0 and 3 pets
                var _pets = new List<IPet>();
                for (int i = 0; i < _seeder.Next(0, 4); i++)
                {
                    //A Pet can only be owned by one friend
                    _pets.Add(new csPet().Seed(_seeder));
                }
                _friends[c].Pets = _pets;

                //Create between 0 and 5 quotes
                var _favoriteQuotes = _seeder.FromListUnique<csQuote>(_seeder.Next(0, 6), _seededquotes);
                _friends[c].Quotes = _favoriteQuotes.ToList<IQuote>();
            }
            #endregion

            //A bit of Linq refresh
            var _info = new adminInfoDbDto();
            _info.nrSeededFriends = _friends.Count();
            _info.nrSeededAddresses = _friends.Where(i => i.Address != null).Distinct().Count();
            _info.nrSeededPets = _friends.Where(i => i.Pets != null).ToList().Sum(i => i.Pets.Count);
            _info.nrSeededQuotes = _friends.Where(i => i.Quotes != null).ToList().Sum(i => i.Quotes.Count);

            return _info;
        }


        //In order to make ReadAsync it has to return a deep copy of _friends.
        //Otherwise another Task could seed or removeseed on the list while first
        //Task is working on the list. Use copy constructor pattern
        public Task<List<IFriend>> ReadFriendsAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize) => Task.Run(() =>
        {
            lock (_locker) {

                //to create a a copy is simple using linq and copy constructor pattern
                var list = (_friends != null) ? _friends.Select(f => new csFriend(f)).ToList<IFriend>() : null;
                return list;
            }
        });
        public List<IFriend> ReadFriends(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _friends.ToList<IFriend>();


        public Task<gstusrInfoAllDto> InfoAsync => Task.Run(() =>
        {
            lock (_locker) { return Info; }
        });
        public gstusrInfoAllDto Info => new gstusrInfoAllDto
        {
            Db = new gstusrInfoDbDto
            {
                nrSeededFriends = _friends.Count(i => i.Seeded),
                nrUnseededFriends = _friends.Count(i => !i.Seeded),
                nrFriendsWithAddress = _friends.Count(f => f.Address == null),

                nrSeededAddresses = _friends.Where(i => i.Seeded && i.Address != null).Distinct().Count(),
                nrUnseededAddresses = _friends.Where(i => !i.Seeded && i.Address != null).Distinct().Count(),

                nrSeededPets = _friends.Where(i => i.Seeded && i.Pets != null).ToList().Sum(i => i.Pets.Count),
                nrUnseededPets = _friends.Where(i => !i.Seeded && i.Pets != null).ToList().Sum(i => i.Pets.Count),

                nrSeededQuotes = _friends.Where(i => i.Seeded && i.Quotes != null).Sum(i => i.Quotes.Count()),
                nrUnseededQuotes = _friends.Where(i => !i.Seeded && i.Quotes != null).Sum(i => i.Quotes.Count())
            }
        };


        #region not implemented
        public Task<IFriend> ReadFriendAsync(loginUserSessionDto usr, Guid id, bool flat) => throw new NotImplementedException();
        public Task<IFriend> DeleteFriendAsync(loginUserSessionDto usr, Guid id) => throw new NotImplementedException();
        public Task<IFriend> UpdateFriendAsync(loginUserSessionDto usr, csFriendCUdto item) => throw new NotImplementedException();
        public Task<IFriend> CreateFriendAsync(loginUserSessionDto usr, csFriendCUdto item) => throw new NotImplementedException();

        public Task<List<IAddress>> ReadAddressesAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize) => throw new NotImplementedException();
        public Task<IAddress> ReadAddressAsync(loginUserSessionDto usr, Guid id, bool flat) => throw new NotImplementedException();
        public Task<IAddress> DeleteAddressAsync(loginUserSessionDto usr, Guid id) => throw new NotImplementedException();
        public Task<IAddress> UpdateAddressAsync(loginUserSessionDto usr, csAddressCUdto item) => throw new NotImplementedException();
        public Task<IAddress> CreateAddressAsync(loginUserSessionDto usr, csAddressCUdto item) => throw new NotImplementedException();

        public Task<List<IQuote>> ReadQuotesAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize) => throw new NotImplementedException();
        public Task<IQuote> ReadQuoteAsync(loginUserSessionDto usr, Guid id, bool flat) => throw new NotImplementedException();
        public Task<IQuote> DeleteQuoteAsync(loginUserSessionDto usr, Guid id) => throw new NotImplementedException();
        public Task<IQuote> UpdateQuoteAsync(loginUserSessionDto usr, csQuoteCUdto item) => throw new NotImplementedException();
        public Task<IQuote> CreateQuoteAsync(loginUserSessionDto usr, csQuoteCUdto item) => throw new NotImplementedException();

        public Task<List<IPet>> ReadPetsAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize) => throw new NotImplementedException();
        public Task<IPet> ReadPetAsync(loginUserSessionDto usr, Guid id, bool flat) => throw new NotImplementedException();
        public Task<IPet> DeletePetAsync(loginUserSessionDto usr, Guid id) => throw new NotImplementedException();
        public Task<IPet> UpdatePetAsync(loginUserSessionDto usr, csPetCUdto item) => throw new NotImplementedException();
        public Task<IPet> CreatePetAsync(loginUserSessionDto usr, csPetCUdto item) => throw new NotImplementedException();
        #endregion
    }
}

