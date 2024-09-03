namespace MedicalAPI.DTO
{
    public class PatientEditDto
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public int SectionId { get; set; }
    }
}
