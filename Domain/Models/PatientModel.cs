using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CurentaCommonCore.Enums.EnumsCollection;

namespace Domain.Models
{
    public record PatientModel
    {
        public Guid Id { get; set; }
        public PatientBasicInfoModel BasicInfo { get; set; }
        public List<PlacesOfServiceModel> PlacesOfService { get; set; }
        public List<AddressModel> Addresses { get; set; }
        public string Status { get; set; }
        public string? ProfilePicture { get; set; }
        public string DeliveryNote { get; set; }
        public bool IsBubblePackEnabled { get; set; }
    }

    public record PatientBasicInfoModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public Gender Gender { get; set; }
    }

    public record PlacesOfServiceModel
    {
        public long FacilityId { get; set; }
    }

    public record AddressModel
    {
        public Guid Id { get; set; }
        public string FullAddress { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public bool IsDefault { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
    }
}
