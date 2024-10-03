#define UseAzureKV          //comment out this line to use user-secrets instead of Azure Key Vault

using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Xml.Linq;

namespace Configuration;

public sealed class csAppConfig
{
    static public string Heartbeat { get; } = $"Heartbeat from namespace {nameof(Configuration)}, class {nameof(csAppConfig)}";

    #region Configuration file locations
    public const string Appsettingfile = "appsettings.json";
    public const string UserSecretsProjFile="../Configuration/Configuration.csproj";
    #endregion
 
    #region Singleton design pattern
    private static readonly object instanceLock = new();
    private static csAppConfig _instance = null;
    #endregion

    #region Configuration data structures
    //read the user secret Id from the project file. Only dduring development
    private string _userSecretId { 
        get {
            string csprojPath = Path.Combine(Directory.GetCurrentDirectory(), UserSecretsProjFile);
            XDocument csproj = XDocument.Load(csprojPath);
            var userSecretsId = csproj.Descendants("UserSecretsId").FirstOrDefault()?.Value;
            return userSecretsId;        
    }}

    private IConfigurationRoot _configuration = null;
    private DbSetDetail _dbSetActive = new DbSetDetail();
    private List<DbSetDetail> _dbSets = new List<DbSetDetail>();
    private PasswordSaltDetails _passwordSaltDetails = new PasswordSaltDetails();
    private JwtConfig _jwtConfig = new JwtConfig();

    #endregion

    private csAppConfig()
    {
#if DEBUG
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
#else        
        //Ensure that also docker environment has Development/Production detection
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
#endif

        //Create final ConfigurationRoot, _configuration which includes also AzureKeyVault
        var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory());

        //Load the user secrets file
        builder = builder.AddUserSecrets(_userSecretId, reloadOnChange: true);

        //override with any locally set configuration from appsettings.json
        builder = builder.AddJsonFile(Appsettingfile, optional: true, reloadOnChange: true);
        _configuration = builder.Build();

        //get DbSet details, Note: Bind need the NuGet package Microsoft.Extensions.Configuration.Binder
        _configuration.Bind("DbSets", _dbSets); 

        //Set the active db set and fill in location and server into Login Details
        int i = int.Parse(_configuration["DbSetActiveIdx"]);
        _dbSetActive = _dbSets[i];
        _dbSetActive.DbLogins.ForEach(i =>
        {
            i.DbLocation = _dbSetActive.DbLocation;
            i.DbServer = _dbSetActive.DbServer;
        });
        
        //get user password details
        _configuration.Bind("PasswordSaltDetails", _passwordSaltDetails);

        //get jwt configurations
        _configuration.Bind("JwtConfig", _jwtConfig);
    }

    #region Singleton design pattern
    private static csAppConfig Instance
    {
        get
        {
            lock (instanceLock)
            {
                if (_instance == null)
                {
                    _instance = new csAppConfig();
                }
                return _instance;
            }
        }
    }
    #endregion
    public static string ASPNETCOREEnvironment
    {
        get
        {
            //Just to ensure environment variable is set, by instance creation
            var _ = Instance;
            
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }
    }
    public static IConfigurationRoot ConfigurationRoot => Instance._configuration;
    public static DbSetDetail DbSetActive => Instance._dbSetActive;
    public static DbLoginDetail DbLoginDetails (string DbLogin)
    {
        if (string.IsNullOrEmpty(DbLogin) || string.IsNullOrWhiteSpace(DbLogin))
            throw new ArgumentNullException();

        var conn = Instance._dbSetActive.DbLogins.First(m => m.DbUserLogin.Trim().ToLower() == DbLogin.Trim().ToLower());
        if (conn == null)
            throw new ArgumentException("Database connection not found");

        return conn;
    }

    public static string SecretSource => $"User secret: {((Instance._userSecretId == null) ?Appsettingfile :Instance._userSecretId)}";

    public static PasswordSaltDetails PasswordSalt => Instance._passwordSaltDetails;
    public static JwtConfig JwtConfig => Instance._jwtConfig;
}

#region types instaniated with configuration content
public class DbSetDetail
{
    public string DbLocation { get; set; }
    public string DbServer { get; set; }

    public List<DbLoginDetail> DbLogins { get; set; }
}

public class DbLoginDetail
{
    //set after reading in the active DbSet
    
    public string DbLocation { get; set; } = null;
    public string DbServer { get; set; } = null;

    public string DbUserLogin { get; set; }
    public string DbConnection { get; set; }
    public string DbConnectionString => csAppConfig.ConfigurationRoot.GetConnectionString(DbConnection);
}


public class PasswordSaltDetails
{
    public string Salt { get; set; }
    public int Iterations { get; set; }
}

public class JwtConfig
{
    public int LifeTimeMinutes { get; set; }

    public bool ValidateIssuerSigningKey { get; set; }
    public string IssuerSigningKey { get; set; }

    public bool ValidateIssuer { get; set; } = true;
    public string ValidIssuer { get; set; }

    public bool ValidateAudience { get; set; } = true;
    public string ValidAudience { get; set; }

    public bool RequireExpirationTime { get; set; }
    public bool ValidateLifetime { get; set; } = true;
}
#endregion


