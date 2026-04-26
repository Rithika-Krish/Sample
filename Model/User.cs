namespace SampleAuth_WebAPI.Model
{
    public class User
    {
        
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; } 

        // Store HASHED password, NOT plain text
        public string PasswordHash { get; set; }

       
    }
}
