using System;
using DbContext;
using DbRepos;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;

namespace Services
{
    public class csFriendsServiceDb : IFriendsService
    {
        private csFriendsDbRepos _repo = null;
        private ILogger<csFriendsServiceDb> _logger = null;

        #region only for layer verification
        private Guid _guid = Guid.NewGuid();
        private string _instanceHeartbeat;

        public string InstanceHeartbeat => _instanceHeartbeat;
 
        static public string Heartbeat { get; } = $"Heartbeat from namespace {nameof(Services)}, class {nameof(csFriendsServiceDb)}" +

            // added after project references is setup
            $"\n   - {csFriendsDbRepos.Heartbeat}" +
            $"\n   - {csMainDbContext.Heartbeat}";
        #endregion

        #region constructors
        public csFriendsServiceDb()
        {
            //only for layer verification
            _instanceHeartbeat = $"Heartbeat from class {this.GetType()} with instance Guid {_guid}.";
        }
        public csFriendsServiceDb(csFriendsDbRepos repo):this()
        {
            _repo = repo;
            if (_repo != null){
                _instanceHeartbeat += $"\nUsing repo {_repo.GetType()} with Heartbeat:" +
                $"\n{_repo.InstanceHeartbeat}";
            }
        }
        public csFriendsServiceDb(csFriendsDbRepos repo, ILogger<csFriendsServiceDb> logger):this(repo)
        {
            _logger = logger;
            _logger.LogInformation(_instanceHeartbeat);
        }
        /*
        public csFriendsServiceDb(ILogger<csFriendsServiceDb> logger)
        {
            //only for layer verification
            _instanceHeartbeat = $"Heartbeat from class {this.GetType()} with instance Guid {_guid}. ";

            _logger = logger;
            _logger.LogInformation(_instanceHeartbeat); 
        }

        public csFriendsServiceDb(csFriendsDbRepos repo, ILogger<csFriendsServiceDb> logger)
        {
            //only for layer verification
            _instanceHeartbeat = $"Heartbeat from class {this.GetType()} with instance Guid {_guid}. " +
                $"Will use repo {repo.GetType()}.";

            _logger = logger;
            _logger.LogInformation(_instanceHeartbeat);

            _repo = repo;

        }
        */
        #endregion

        #region Used for DI exploration only
        public Task<List<IPet>> DIExploration () => throw new NotImplementedException();
        #endregion
        
        #region Simple 1:1 calls in this case, but as Services expands, this will no longer be the case
        public Task<gstusrInfoAllDto> InfoAsync => _repo.InfoAsync();

        public Task<adminInfoDbDto> SeedAsync(loginUserSessionDto usr, int nrOfItems) => _repo.SeedAsync(usr, nrOfItems);
        public Task<adminInfoDbDto> RemoveSeedAsync(loginUserSessionDto usr, bool seeded) => _repo.RemoveSeedAsync(usr, seeded);

        public Task<csRespPageDTO<IFriend>> ReadFriendsAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadFriendsAsync(usr, seeded, flat, filter, pageNumber, pageSize);
        public Task<IFriend> ReadFriendAsync(loginUserSessionDto usr, Guid id, bool flat) => _repo.ReadFriendAsync(usr, id, flat);
        public Task<IFriend> DeleteFriendAsync(loginUserSessionDto usr, Guid id) => _repo.DeleteFriendAsync(usr, id);
        public Task<IFriend> UpdateFriendAsync(loginUserSessionDto usr, csFriendCUdto item) => _repo.UpdateFriendAsync(usr, item);
        public Task<IFriend> CreateFriendAsync(loginUserSessionDto usr, csFriendCUdto item) => _repo.CreateFriendAsync(usr, item);

        public Task<csRespPageDTO<IAddress>> ReadAddressesAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadAddressesAsync(usr, seeded, flat, filter, pageNumber, pageSize);
        public Task<IAddress> ReadAddressAsync(loginUserSessionDto usr, Guid id, bool flat) => _repo.ReadAddressAsync(usr, id, flat);
        public Task<IAddress> DeleteAddressAsync(loginUserSessionDto usr, Guid id) => _repo.DeleteAddressAsync(usr, id);
        public Task<IAddress> UpdateAddressAsync(loginUserSessionDto usr, csAddressCUdto item) => _repo.UpdateAddressAsync(usr, item);
        public Task<IAddress> CreateAddressAsync(loginUserSessionDto usr, csAddressCUdto item) => _repo.CreateAddressAsync(usr, item);

        public Task<csRespPageDTO<IQuote>> ReadQuotesAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadQuotesAsync(usr, seeded, flat, filter, pageNumber, pageSize);
        public Task<IQuote> ReadQuoteAsync(loginUserSessionDto usr, Guid id, bool flat) => _repo.ReadQuoteAsync(usr, id, flat);
        public Task<IQuote> DeleteQuoteAsync(loginUserSessionDto usr, Guid id) => _repo.DeleteQuoteAsync(usr, id);
        public Task<IQuote> UpdateQuoteAsync(loginUserSessionDto usr, csQuoteCUdto item) => _repo.UpdateQuoteAsync(usr, item);
        public Task<IQuote> CreateQuoteAsync(loginUserSessionDto usr, csQuoteCUdto item) => _repo.CreateQuoteAsync(usr, item);

        public Task<csRespPageDTO<IPet>> ReadPetsAsync(loginUserSessionDto usr, bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadPetsAsync(usr, seeded, flat, filter, pageNumber, pageSize);
        public Task<IPet> ReadPetAsync(loginUserSessionDto usr, Guid id, bool flat) => _repo.ReadPetAsync(usr, id, flat);
        public Task<IPet> DeletePetAsync(loginUserSessionDto usr, Guid id) => _repo.DeletePetAsync(usr, id);
        public Task<IPet> UpdatePetAsync(loginUserSessionDto usr, csPetCUdto item) => _repo.UpdatePetAsync(usr, item);
        public Task<IPet> CreatePetAsync(loginUserSessionDto usr, csPetCUdto item) => _repo.CreatePetAsync(usr, item);
        #endregion
    }
}
