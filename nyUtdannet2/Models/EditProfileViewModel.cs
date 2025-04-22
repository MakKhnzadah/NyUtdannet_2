namespace nyUtdannet2.Models
{
    public class EditProfileViewModel
    {
        public string Id { get; set; } = null!; 
        public string FirstName { get; set; } = null!; 
        public string LastName  { get; set; } = null!; 
        public DateTime DateOfBirth   { get; set; }
        public string StreetName      { get; set; } = "";
        public string StreetNumber    { get; set; } = "";
        public string PostalCode      { get; set; } = "";
        public string City            { get; set; } = "";
        public string Country         { get; set; } = "";
    }
}