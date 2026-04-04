using ShopService.Core.Entities;
using ShopService.Database.Interfaces;

namespace ShopService.Application.Services
{
	public class UserService
	{
		private readonly IBaseRepository<User> _userRepository;

		public UserService(IBaseRepository<User> userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task<User?> GetUserById(Guid userId, CancellationToken cancellationToken = default)
		{
			return await _userRepository.GetByIdAsync(userId);
		}

		public async Task<User?> GetUserByVkId(long vkId, CancellationToken cancellationToken = default)
		{
			return (await _userRepository.GetAsync(x => x.VkId == vkId, null)).FirstOrDefault();
		}

		public async Task<User> CreateUser(string userName, CancellationToken cancellationToken = default)
		{
			var user = new User
			{
				Username = userName,
			};
			user = _userRepository.Create(user);
			await _userRepository.SaveChangesAsync(cancellationToken);
			return user;
		}

		public async Task<User> CreateUser(string userName, long vkId, CancellationToken cancellationToken = default)
		{
			var user = new User
			{
				Username = userName,
				VkId = vkId
			};
			user = _userRepository.Create(user);
			await _userRepository.SaveChangesAsync(cancellationToken);
			return user;
		}
	}
}
