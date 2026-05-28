namespace Ecomm.UI.Models.CoverTypeDto
{
    public class CoverTypeTrashDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
