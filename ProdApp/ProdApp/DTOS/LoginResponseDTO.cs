namespace ProdApp.DTOS
{
    public class LoginResponseDTO
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
        public string Role { get; set; }
    }
}
