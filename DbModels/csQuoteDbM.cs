using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;
using Models;
using Models.DTO;

namespace DbModels
{
    [Table("Quotes", Schema = "supusr")]
    public class csQuoteDbM : csQuote, ISeed<csQuoteDbM>, IEquatable<csQuoteDbM>
    {
        [Key]       // for EFC Code first
        public override Guid QuoteId { get; set; }

        #region correcting the Navigation properties migration error caused by using interfaces
        [NotMapped] //removed from EFC 
        public override List<IFriend> Friends { get => FriendsDbM?.ToList<IFriend>(); set => new NotImplementedException(); }

        [JsonIgnore] //do not include in any json response from the WebApi
        public virtual List<csFriendDbM> FriendsDbM { get; set; } = null;
        #endregion

        #region constructors
        public csQuoteDbM() : base() { }
        public csQuoteDbM(csSeededQuote goodQuote) : base(goodQuote) { }
        public csQuoteDbM(csQuoteCUdto org)
        {
            QuoteId = Guid.NewGuid();
            UpdateFromDTO(org);
        }
        #endregion

        #region implementing IEquatable
        public bool Equals(csQuoteDbM other) => (other != null) ? ((Quote, Author) ==
            (other.Quote, other.Author)) : false;

        public override bool Equals(object obj) => Equals(obj as csQuoteDbM);
        public override int GetHashCode() => (Quote, Author).GetHashCode();
        #endregion

        #region randomly seed this instance
        public override csQuoteDbM Seed(csSeedGenerator sgen)
        {
            base.Seed(sgen);
            return this;
        }
        #endregion

        #region Update from DTO
        public csQuote UpdateFromDTO(csQuoteCUdto org)
        {
            if (org == null) return null;

            Author = org.Author;
            Quote = org.Quote;

            return this;
        }
        #endregion
    }
}

