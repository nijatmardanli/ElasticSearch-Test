﻿using Elastic.Clients.Elasticsearch;
using ElasticSearch.WebApi.Domain;
using ElasticSearch.WebApi.Dtos;
using ElasticSearch.WebApi.Requests;
using ElasticSearch.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IElasticSearchService<UserEntity> _elasticSearchService;

        public UsersController(ILogger<UsersController> logger, IElasticSearchService<UserEntity> elasticSearchService)
        {
            _logger = logger;
            _elasticSearchService = elasticSearchService;
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _elasticSearchService.GetAsync(id, cancellationToken);
            return ProcessResult(result);
        }

        /// <summary>
        /// Get user list
        /// </summary>
        /// <param name="paginationRequest">Pagination request</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationRequest paginationRequest, CancellationToken cancellationToken)
        {
            var result = await _elasticSearchService.GetAllAsync(paginationRequest.From, paginationRequest.Size, cancellationToken);
            return ProcessCollectionResult(result);
        }

        /// <summary>
        /// Search user: FirstName = request.FirstName, LastName like 'request.LastName%', Age >= request.Age
        /// </summary>
        /// <param name="userSearchRequest">search request data</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("Search")]
        public async Task<IActionResult> SearchAsync([FromQuery] UserSearchRequestDto userSearchRequest, CancellationToken cancellationToken)
        {
            var result = await _elasticSearchService.SearchAsync(BuildSearchAction(userSearchRequest), cancellationToken);
            return ProcessCollectionResult(result);
        }

        /// <summary>
        /// Add user
        /// </summary>
        /// <param name="userAddDto">Add user data</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] UserAddDto userAddDto, CancellationToken cancellationToken)
        {
            var result = await _elasticSearchService.AddOrUpdateAsync(userAddDto, cancellationToken);
            return ProcessActionResult(result, userAddDto, "added");
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="userUpdateDto">Update user data</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] UserUpdateDto userUpdateDto, CancellationToken cancellationToken)
        {
            var userEntity = await _elasticSearchService.GetAsync(userUpdateDto.Id, cancellationToken);
            if (userEntity == null)
            {
                _logger.LogInformation("Data not found");
                return NotFound();
            }

            userEntity.FirstName = userUpdateDto.FirstName;
            userEntity.LastName = userUpdateDto.LastName;
            userEntity.Age = userUpdateDto.Age;

            var result = await _elasticSearchService.AddOrUpdateAsync(userEntity, cancellationToken);
            return ProcessActionResult(result, userEntity, "updated");
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync([FromHeader] int id, CancellationToken cancellationToken)
        {
            var result = await _elasticSearchService.DeleteAsync(id, cancellationToken);
            return ProcessActionResult(result, id, "deleted");
        }

        #region Private methods
        private IActionResult ProcessResult(UserEntity? result)
        {
            if (result == null)
            {
                _logger.LogInformation("Data not found");
                return NotFound();
            }

            _logger.LogInformation($"Data for {result.Id}: {result}");
            return Ok(result);
        }

        private IActionResult ProcessCollectionResult(IReadOnlyCollection<UserEntity> result)
        {
            if (result.Count == 0)
            {
                _logger.LogInformation("Data not found");
                return NotFound();
            }

            _logger.LogInformation($"{result.Count} data found");
            return Ok(result);
        }

        private IActionResult ProcessActionResult(bool result, object data, string action)
        {
            if (result)
            {
                _logger.LogInformation($"Data: {data} {action} successfully");
                return Ok(data);
            }

            _logger.LogInformation($"Data: {data} failed when {action}");
            return BadRequest();
        }

        private Action<SearchRequestDescriptor<UserEntity>> BuildSearchAction(UserSearchRequestDto userSearchRequest)
        {
            return s => s
                .Query(q => q.Bool(b => b.Must(
                    mm => mm.Match(m => m.Field(u => u.FirstName).Query(userSearchRequest.FirstName)),
                    mm => mm.QueryString(c => c.Query($"{userSearchRequest.LastName}*").DefaultField(u => u.LastName))
                ).Filter(f => f.Range(r => r.NumberRange(n => n.Field(u => u.Age).Gte(userSearchRequest.Age))))));
        }
        #endregion
    }
}