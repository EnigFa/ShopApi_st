using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Interfaces.Services;

public interface ICachingService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? exp);
    Task RemoveAsync(string key);


}