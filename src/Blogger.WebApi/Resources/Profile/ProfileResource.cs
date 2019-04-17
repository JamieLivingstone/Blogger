namespace Blogger.WebApi.Resources.Profile
{
    public class ProfileResource
    {
        public string UserName { get; set; }
        
        public string Bio { get; set; }
        
        public string Image { get; set; }
        
        public bool Following { get; set; }
    }
}