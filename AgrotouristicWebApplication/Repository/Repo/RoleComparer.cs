﻿using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repository.Repo
{
    public class RoleComparer : IEqualityComparer<IdentityRole>
    {
        public bool Equals(IdentityRole x, IdentityRole y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(IdentityRole obj)
        {
            return obj.GetHashCode();
        }
    }
}