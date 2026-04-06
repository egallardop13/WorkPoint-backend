namespace DotnetAPI.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse<T> Ok(T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                StatusCode = 200,
            };
        }

        public static ApiResponse<T> Fail(string error, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Error = error,
                StatusCode = statusCode,
            };
        }
    }
}
