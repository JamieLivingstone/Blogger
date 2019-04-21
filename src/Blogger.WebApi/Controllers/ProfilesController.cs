using System.Threading.Tasks;
using AutoMapper;
using Blogger.Core.Entities;
using Blogger.Core.Interfaces;
using Blogger.WebApi.Resources.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blogger.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/profiles")]
    public class ProfilesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IRepository _repository;
        private readonly IFollowerRepository _followerRepository;
        private readonly IUserResolverService _userResolverService;

        public ProfilesController(
            IMapper mapper, 
            IUserRepository userRepository, 
            IRepository repository, 
            IFollowerRepository followerRepository,
            IUserResolverService userResolverService
        )
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _repository = repository;
            _followerRepository = followerRepository;
            _userResolverService = userResolverService;
        }
        
        [HttpGet("{userName}")]
        public async Task<IActionResult> GetProfileByUsername(string userName)
        {
            var profile = await GetProfileAsync(userName);

            if (profile == null)
            {
                return NotFound();
            }
            
            return Ok(profile);
        }

        [HttpPost("{userName}/follow")]
        [Authorize]
        public async Task<IActionResult> FollowUser(string userName)
        {
            var target = await _userRepository.GetByUserNameAsync(userName);

            if (target == null)
            {
                return NotFound();
            }

            if (target.Following)
            {
                return Ok(target);
            }

            var follower = new Follower
            {
                TargetId = target.Id,
                ObserverId = _userResolverService.GetUserId()
            };
            
            await _repository.AddAsync(follower);

            var result = await GetProfileAsync(userName);
            
            return Created("", result);
        }

        [HttpDelete("{userName}/follow")]
        [Authorize]
        public async Task<IActionResult> UnfollowUser(string userName)
        {
            var target = await _userRepository.GetByUserNameAsync(userName);
            
            if (target == null)
            {
                return NotFound();
            }
            
            await _followerRepository.RemoveAsync(target.Id, _userResolverService.GetUserId());

            var result = await GetProfileAsync(userName);

            return Ok(result);
        }

        private async Task<ProfileResource> GetProfileAsync(string userName)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);

            if (user == null)
            {
                return null;
            }

            var profile = _mapper.Map<ProfileResource>(user);

            var userId = _userResolverService.GetUserId();

            if (userId != null)
            {
                profile.Following = await _followerRepository.IsFollowing(user.Id, userId);
            }

            return profile;
        }
    }
}