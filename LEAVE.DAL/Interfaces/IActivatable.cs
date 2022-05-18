using System;
using System.Collections.Generic;
using System.Text;

namespace LEAVE.DAL
{
    public interface IActivatable
    {
        bool IsActive { get; set; }
    }
}
