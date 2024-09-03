using MedicalAPI.DTO;
using MedicalAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly MedicalContext _context;

        public DoctorsController(MedicalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorListDto>>> GetDoctors(
            string sortBy = "FullName", int page = 1, int pageSize = 10)
        {
            var query = _context.Doctors
                .Include(d => d.Room)
                .Include(d => d.Specialization)
                .Include(d => d.Section)
                .AsQueryable();

            query = sortBy switch
            {
                "RoomNumber" => query.OrderBy(d => d.Room.Number),
                "SpecializationName" => query.OrderBy(d => d.Specialization.Name),
                _ => query.OrderBy(d => d.FullName)
            };

            var doctors = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DoctorListDto
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    RoomNumber = d.Room != null ? d.Room.Number.ToString() : "",
                    SpecializationName = d.Specialization != null ? d.Specialization.Name : "",
                    SectionNumber = d.Section != null ? d.Section.Number.ToString() : ""
                }).ToListAsync();

            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorEditDto>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            return Ok(new DoctorEditDto
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                RoomId = doctor.RoomId,
                SpecializationId = doctor.SpecializationId,
                SectionId = doctor.SectionId
            });
        }

        [HttpPost]
        public async Task<ActionResult<Doctor>> AddDoctor(DoctorEditDto doctorDto)
        {
            var doctor = new Doctor
            {
                FullName = doctorDto.FullName,
                RoomId = doctorDto.RoomId,
                SpecializationId = doctorDto.SpecializationId,
                SectionId = doctorDto.SectionId
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, DoctorEditDto doctorDto)
        {
            if (id != doctorDto.Id)
            {
                return BadRequest();
            }

            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            doctor.FullName = doctorDto.FullName;
            doctor.RoomId = doctorDto.RoomId;
            doctor.SpecializationId = doctorDto.SpecializationId;
            doctor.SectionId = doctorDto.SectionId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
