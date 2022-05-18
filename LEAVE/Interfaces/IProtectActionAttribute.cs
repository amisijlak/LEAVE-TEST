using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LEAVE.DAL
{
    public interface IProtectActionAttribute
    {
        SecurityModule Module { get; set; }
        SecuritySubModule SubModule { get; set; }
        SecuritySystemAction SystemAction { get; set; }
    }
}
