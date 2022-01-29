using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbetApi.Models
{
    public class UserWithRoles
    {
        public AbetApi.EFModels.User user { get; set; }
        public List<string> roles { get; set; }
    }
}
