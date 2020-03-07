using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  PatientsApp.Api.Models
{
    public class Patient
    {
        public Guid Id { get; set; }

        [JsonRequired]
        public string Gender { get; set; }
        
        [JsonRequired]
        public string Email { get; set; }
        
        [JsonRequired]
        public string Phone { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "date_of_birth")]
        public string DOB { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "is_active")]
        public bool? IsActive { get; set; }
        
        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; set; }
        
        [JsonProperty(PropertyName = "updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
