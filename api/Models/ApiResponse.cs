namespace api.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public ApiResponse(bool success = true, string? message = null, T? data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}

