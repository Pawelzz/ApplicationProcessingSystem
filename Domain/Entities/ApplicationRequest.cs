using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Domain.Entities
{
    public class ApplicationRequest
    {
        public Guid Id { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; }
        public DateTime  CreatedAt { get; set; }
    }
}
