namespace MedicalAPI.Model
{
    public class Doctor
    {
        public int Id { get; set; }
        public  string FullName { get; set; }
        public int RoomId { get; set; }
        public int SpecializationId { get; set; }
        public int? SectionId { get; set; }

        public  Room Room { get; set; }
        public  Specialization Specialization { get; set; }
        public  Section Section { get; set; }
    }
}
