using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  PatientsApp.Api.Models
{
    public class PagedResponse
    {
        public PageMetadata Metadata { get; set; }
        public IEnumerable<Patient> Patients { get; set; }
    }

    public class PageMetadata
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
