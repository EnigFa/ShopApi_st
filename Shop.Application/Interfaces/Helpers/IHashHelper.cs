using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Interfaces.Helpers;

public interface IHashHelper
{
    public string Hash(string password);
    public bool IsValidPassword(string password, string hash);
}

