using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LEAVE.DAL
{
    public enum SecuritySubModule
    {
        //security under 1000
        [Description("System Users")]
        SystemUsers = 2,
        [Description("Security Roles")]
        SecurityRoles = 3,
       
        //general from 1000 - 9999
        [Description("Tax Configurations")]
        TaxConfigurations = 1005,
        [Description("Hierachy Configurations")]
        HierachyConfigurations = 1006,
        Institutions = 1007,
        [Description("Territory Configurations")]
        TerritoryConfigurations = 1009,        
        [Description("Generic Lookups")]
        GenericLookups = 1015,
        Employee = 1016,
        Leave_Request = 1017,
    }
}
