namespace Leave_Managment_Sytem.Services
{
    public class GetCurrentUserId
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "userId");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new Exception("User ID claim is missing or invalid.");
            }

            return userId;
        }
    }
}