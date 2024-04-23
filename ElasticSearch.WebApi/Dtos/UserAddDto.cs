using ElasticSearch.WebApi.Domain;

namespace ElasticSearch.WebApi.Dtos
{
    public record UserAddDto(string FirstName, string LastName, int Age)
    {
        public static implicit operator UserEntity(UserAddDto userAddDto)
        {
            return new UserEntity()
            {
                FirstName = userAddDto.FirstName,
                LastName = userAddDto.LastName,
                Age = userAddDto.Age,
                CreatedDate = DateTime.UtcNow
            };
        }
    }
}
