using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;

using Seido.Utilities.SeedGenerator;
using Configuration;
using Models;
using Models.DTO;


namespace DbModels
{
    [Table("Addresses", Schema = "supusr")]
    [Index(nameof(StreetAddress), nameof(ZipCode), nameof(City), nameof(Country), IsUnique = true)]
    public class csAddressDbM : csAddress, ISeed<csAddressDbM>, IEquatable<csAddressDbM>
    {
        [Key]       // for EFC Code first
        public override Guid AddressId { get; set; }

        [Required]
        public override string StreetAddress { get; set; }
        [Required]
        public override int ZipCode { get; set; }
        [Required]
        public override string City { get; set; }
        [Required]
        public override string Country { get; set; }

        #region implementing IEquatable
        public bool Equals(csAddressDbM other) => (other != null) ?((StreetAddress, ZipCode, City, Country) ==
            (other.StreetAddress, other.ZipCode, other.City, other.Country)) :false;

        public override bool Equals(object obj) => Equals(obj as csAddressDbM);
        public override int GetHashCode() => (StreetAddress, ZipCode, City, Country).GetHashCode();
        #endregion

        #region correcting the Navigation properties migration error caused by using interfaces
        [NotMapped] //removed from EFC 
        public override List<IFriend> Friends { get => FriendsDbM?.ToList<IFriend>(); set => new NotImplementedException(); }
 
        [JsonIgnore] //do not include in any json response from the WebApi
        public virtual List<csFriendDbM> FriendsDbM { get; set; } = null;
        #endregion

        #region randomly seed this instance
        public override csAddressDbM Seed(csSeedGenerator sgen)
        {
            base.Seed(sgen);
            return this;
        }
        #endregion

        #region Update from DTO
        public csAddressDbM UpdateFromDTO(csAddressCUdto org)
        {
            if (org == null) return null;

            StreetAddress = org.StreetAddress;
            ZipCode = org.ZipCode;
            City = org.City;
            Country = org.Country;

            return this;
        }
        #endregion

        #region constructors
        public csAddressDbM() { }
        public csAddressDbM(csAddressCUdto org)
        {
            AddressId = Guid.NewGuid();
            UpdateFromDTO(org);
        }
        #endregion
    }
}

