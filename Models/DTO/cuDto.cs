using System;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace Models.DTO
{
    //DTO is a DataTransferObject, can be instanstiated by the controller logic
    //and represents a, fully instatiable, subset of the Database models
    //for a specific purpose.

    //These DTO are simplistic and used to Update and Create objects
    public class csFriendCUdto
    {
        public virtual Guid? FriendId { get; set; }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }

        public virtual string Email { get; set; }

        public DateTime? Birthday { get; set; } = null;

        public virtual Guid? AddressId { get; set; } = null;

        public virtual List<Guid> PetsId { get; set; } = null;

        public virtual List<Guid> QuotesId { get; set; } = null;

        public csFriendCUdto() { }
        public csFriendCUdto(IFriend org)
        {
            FriendId = org.FriendId;
            FirstName = org.FirstName;
            LastName = org.LastName;
            Email = org.Email;
            Birthday = org.Birthday;

            AddressId = org?.Address?.AddressId;
            PetsId = org.Pets?.Select(i => i.PetId).ToList();
            QuotesId = org.Quotes?.Select(i => i.QuoteId).ToList();
        }
    }

    public class csAddressCUdto
    {
        public virtual Guid? AddressId { get; set; }

        public virtual string StreetAddress { get; set; }
        public virtual int ZipCode { get; set; }
        public virtual string City { get; set; }
        public virtual string Country { get; set; }

        public virtual List<Guid> FriendsId { get; set; } = null;

        public csAddressCUdto() { }
        public csAddressCUdto(IAddress org)
        {
            AddressId = org.AddressId;
            StreetAddress = org.StreetAddress;
            ZipCode = org.ZipCode;
            City = org.City;
            Country = org.Country;

            FriendsId = org.Friends?.Select(i => i.FriendId).ToList();
        }
    }

    public class csPetCUdto
    {
        public virtual Guid? PetId { get; set; }

        public virtual enAnimalKind Kind { get; set; }
        public virtual string Name { get; set; }
        public enAnimalMood Mood { get; set; }

        //cannot be nullable as a Pets has to have an owner even when created
        public virtual Guid FriendId { get; set; }

        public csPetCUdto() { }
        public csPetCUdto(IPet org)
        {
            PetId = org.PetId;
            Kind = org.Kind;
            Name = org.Name;
            Mood = org.Mood;

            FriendId = org.Friend.FriendId;
        }
    }

    public class csQuoteCUdto
    {
        public virtual Guid? QuoteId { get; set; }
        public virtual string Quote { get; set; }
        public virtual string Author { get; set; }

        public virtual List<Guid> FriendsId { get; set; } = null;


        public csQuoteCUdto() { }
        public csQuoteCUdto(IQuote org)
        {
            QuoteId = org.QuoteId;

            Quote = org.Quote;
            Author = org.Author;

            FriendsId = org.Friends?.Select(i => i.FriendId).ToList();
        }
    }
}

