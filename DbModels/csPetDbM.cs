using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;
using Configuration;
using Models;
using Models.DTO;

namespace DbModels
{
     [Table("Pets", Schema = "supusr")]
    public class csPetDbM : csPet, ISeed<csPetDbM>
    {
        [Key]       // for EFC Code first
        public override Guid PetId { get; set; }

        //create own Foreign Key step 1
        [JsonIgnore]
        public virtual Guid FriendId { get; set; }  //Enforces Cascade Delete
        //public virtual Guid? FriendId { get; set; }  //Enforces Cascade Delete

        [Required]
        [StringLength(50)]     //String lenght max check constraint
        public override string Name { get; set; }
        
        #region correcting the Navigation properties migration error caused by using interfaces
        [ForeignKey("FriendId")]     //create own Foreign Key step 2
        [JsonIgnore] //do not include in any json response from the WebApi
        public virtual csFriendDbM FriendDbM { get; set; } = null;    //This is implemented in the database table        
        
        [NotMapped] //removed from EFC 
        public override IFriend Friend { get => FriendDbM; set => new NotImplementedException(); }        
        #endregion

        #region randomly seed this instance
        public override csPetDbM Seed(csSeedGenerator sgen)
        {
            base.Seed(sgen);
            return this;
        }
        #endregion

        #region Update from DTO
        public csPetDbM UpdateFromDTO(csPetCUdto org)
        {
            if (org == null) return null;

            Kind = org.Kind;
            Mood = org.Mood;
            Name = org.Name;

            //We will set this when DbM model is finished
            //FriendId = org.FriendId;

            return this;
        }
        #endregion

        #region constructors
        public csPetDbM() { }
        public csPetDbM(csPetCUdto org)
        {
            PetId = Guid.NewGuid();
            UpdateFromDTO(org);
        }
        #endregion
    }
}

