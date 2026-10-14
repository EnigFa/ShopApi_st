using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Interfaces.Configurations;

public interface IFilePathProvider
{
    string Categories { get; }
    string Products { get; }
}
