using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository addressRepository, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _userManager = userManager;
            _mapper = mapper;
        }
        #region Create
        public async Task<AddressDto> SaveAddress(string userId, CreateAddressDto dto)
        {
            await ValidateUserAsync(userId);
            await using var transaction = await _addressRepository.BeginTransactionAsync();
            try
            {
                var existingAddresses = (await _addressRepository.GetByUserIdAsync(userId)).ToList();
                var address = _mapper.Map<Address>(dto);
                address.UserId = userId;
                address.CreatedAt = DateTime.UtcNow;
                address.UpdatedAt = DateTime.UtcNow;
                if (dto.IsDefault)
                {
                    ClearExistingDefaultAddresses(existingAddresses);
                    address.IsDefault = true;
                }
                else if (!existingAddresses.Any())
                {
                    address.IsDefault = true;
                }
                await _addressRepository.AddAsync(address);
                await _addressRepository.SaveAsync();
                await transaction.CommitAsync();
                return _mapper.Map<AddressDto>(address);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        #endregion
        #region Update
        public async Task<AddressDto> UpdateAddress(string userId, UpdateAddressDto dto)
        {
            await ValidateUserAsync(userId);
            await using var transaction = await _addressRepository.BeginTransactionAsync();
            try
            {
                var address = await _addressRepository.GetByIdAndUserIdAsync(dto.Id, userId);
                if (address == null)
                    throw new KeyNotFoundException("Address not found.");
                address.AddressType = dto.AddressType;
                address.ContactPhoneNumber = dto.ContactPhoneNumber;
                address.AddressLine1 = dto.AddressLine1;
                address.AddressLine2 = dto.AddressLine2;
                address.City = dto.City;
                address.State = dto.State;
                address.PostalCode = dto.PostalCode;
                address.Country = dto.Country;
                address.UpdatedAt = DateTime.UtcNow;
                if (dto.IsDefault)
                {
                    var addresses = (await _addressRepository.GetByUserIdAsync(userId)).Where(x => x.Id != address.Id).ToList();
                    ClearExistingDefaultAddresses(addresses);
                    address.IsDefault = true;
                }
                _addressRepository.Update(address);
                await _addressRepository.SaveAsync();
                await transaction.CommitAsync();
                return _mapper.Map<AddressDto>(address);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        #endregion
        #region Delete
        public async Task DeleteAddress(string userId, int addressId)
        {
            await ValidateUserAsync(userId);
            await using var transaction = await _addressRepository.BeginTransactionAsync();
            try
            {
                var address = await _addressRepository.GetByIdAndUserIdAsync(addressId, userId);
                if (address == null)
                    throw new KeyNotFoundException("Address not found.");
                bool wasDefault = address.IsDefault;
                _addressRepository.Remove(address);
                await _addressRepository.SaveAsync();
                if (wasDefault)
                {
                    var remainingAddress = (await _addressRepository.GetByUserIdAsync(userId)).FirstOrDefault();
                    if (remainingAddress != null)
                    {
                        remainingAddress.IsDefault = true;
                        remainingAddress.UpdatedAt = DateTime.UtcNow;
                        _addressRepository.Update(remainingAddress);
                        await _addressRepository.SaveAsync();
                    }
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        #endregion
        #region Read
        public async Task<IEnumerable<AddressDto>> GetAddresses(string userId)
        {
            await ValidateUserAsync(userId);
            var addresses = await _addressRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<AddressDto>>(addresses);
        }
        public async Task<AddressDto> GetDefaultAddress(string userId)
        {
            await ValidateUserAsync(userId);
            var address = await _addressRepository.GetDefaultAddressAsync(userId);
            if (address == null)
                throw new KeyNotFoundException("Default address not found.");
            return _mapper.Map<AddressDto>(address);
        }
        #endregion
        #region Private Helpers
        private async Task ValidateUserAsync(string userId)
        {
            bool exists = await _userManager.Users.AnyAsync(x => x.Id == userId);
            if (!exists)
                throw new KeyNotFoundException("User not found.");
        }
        private void ClearExistingDefaultAddresses(IEnumerable<Address> addresses)
        {
            foreach (var address in addresses.Where(x => x.IsDefault))
            {
                address.IsDefault = false;
                address.UpdatedAt = DateTime.UtcNow;
                _addressRepository.Update(address);
            }
        }
        #endregion
    }
}