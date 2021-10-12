
namespace Helpers.Models.SharedModels
{
    /// <summary>
    ///  Simple http response class with an entity and message
    /// </summary>
    public class ApiResponse<TEntity> where TEntity : class
    {
        public TEntity Result { get; set; }
        public string Message { get; set; }
    }

}
