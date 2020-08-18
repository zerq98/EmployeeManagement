using System.Collections.Generic;
using System.Security.Claims;

namespace EmployeeManagement.Models
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Create Role", "Create role"),
            new Claim("Edit Role", "Edit role"),
            new Claim("Delete Role", "Delete role")
        };
    }
}