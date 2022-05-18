using System;
using System.Collections.Generic;
using System.Text;

namespace LEAVE.DAL
{
    public interface IPersonName
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string OtherName { get; set; }
    }
}
