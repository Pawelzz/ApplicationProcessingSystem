using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    public class ApplicationService
    {
        private readonly List<ApplicationRequest> _applications = new();

        public ApplicationRequest Create(string applicantName)
        {
            var application = new ApplicationRequest
            {
                Id = Guid.NewGuid(),
                ApplicantName = applicantName,
                Status = ApplicationStatus.New,
                CreatedAt = DateTime.Now
            };
            _applications.Add(application);
            return application;
        }
        public List<ApplicationRequest> GetAll()
        {
            return _applications;
        }
    };
}
