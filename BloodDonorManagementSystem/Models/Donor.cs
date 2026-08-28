using System;

namespace BloodDonorManagementSystem.Models
{
    public class Donor
    {
        public int DonorId { get; set; }

        public string FullName { get; set; }

        public string Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string BloodGroup { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Pincode { get; set; }

        public DateTime? LastDonationDate { get; set; }

        public bool IsAvailable { get; set; }

        // Additional management fields
        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }
    }
}