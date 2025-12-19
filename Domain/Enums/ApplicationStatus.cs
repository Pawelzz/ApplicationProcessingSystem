using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{
    public enum ApplicationStatus
    {
        New = 1,
        InProgress = 2,
        Completed = 3,
        Rejected = 4,
    }
    public enum SortBy
    {
        NameAsc,
        NameDesc,
        DateAsc,
        DateDesc
    }
}
