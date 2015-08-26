namespace ManytoMany.Models
{
    public class PersonPhoto
    {
        public int PhotoId { get; set; }
        public Photo Photo { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}