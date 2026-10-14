using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Infrastructure.Repositories;

public class AuthRepository(ShopDbContext _context) : IAuthRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        var userFromDb = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
        return userFromDb;
    }

    public async Task<User?> GetUserById(Guid id)
    {
        var userFromDb = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
        return userFromDb;
    }

    public async Task<bool> IsExistEmailAsync(string email)
    {
        var userFromDb = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
        if(userFromDb == null)
            return false;
        return true;
    }

    public async Task<User?> RegisterUserAsync(User user, string hash)
    {
        user.PasswordHash = hash;
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return await _context.Users.FirstOrDefaultAsync(us => (us.Email == user.Email && us.PasswordHash == user.PasswordHash)); ;
        /*
         1) Перевірити чи немає вже у БД такого email
         2) Захешувати пароль
         3) Додати користувача у БД
         4) Зробити токен, скоріше за все не тут будемо робити
         */
    }
}
