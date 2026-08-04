namespace DotNet_Assignment.Models.DTO
{
    public class ApiResponseDto<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
