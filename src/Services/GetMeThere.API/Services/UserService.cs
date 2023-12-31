﻿using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;
using GetMeThere.API.Repositories;

namespace GetMeThere.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> DeleteUserAccount(UserDto user)
        {
            var result = await _userRepository.DeleteUserAccount(user);
            return result;
        }

        public async Task<UserDto> GetUserInfo(string username)
        {
            var user = await _userRepository.GetUserInfo(username);
            if(user is null) { return null; }
            else
            {
                UserDto userDto = new UserDto(user.Id, user.Login, user.Login, user.Password);
                return userDto;
            }
            
        }

        public Task<UserDto> UpdateUserInfo(UserDto userDto)
        {
            throw new NotImplementedException();
        }
    }
}
