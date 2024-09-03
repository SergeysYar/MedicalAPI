using MedicalAPI.Model;
using MedicalAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly MedicalContext _context;

        public PatientsController(MedicalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientListDto>>> GetPatients(
            string sortBy = "LastName", int page = 1, int pageSize = 10)
        {
            var query = _context.Patients.Include(p => p.Section).AsQueryable();

            query = sortBy switch
            {
                "FirstName" => query.OrderBy(p => p.FirstName),
                "DateOfBirth" => query.OrderBy(p => p.DateOfBirth),
                _ => query.OrderBy(p => p.LastName)
            };

            var patients = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PatientListDto
                {
                    Id = p.Id,
                    LastName = p.LastName,
                    FirstName = p.FirstName,
                    MiddleName = p.MiddleName,
                    SectionName = p.Section != null ? p.Section.Number.ToString() : ""
                }).ToListAsync();

            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientEditDto>> GetPatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(new PatientEditDto
            {
                Id = patient.Id,
                LastName = patient.LastName,
                FirstName = patient.FirstName,
                MiddleName = patient.MiddleName,
                SectionId = patient.SectionId
            });
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> AddPatient(PatientEditDto patientDto)
        {
            var patient = new Patient
            {
                LastName = patientDto.LastName,
                FirstName = patientDto.FirstName,
                MiddleName = patientDto.MiddleName,
                SectionId = patientDto.SectionId
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, PatientEditDto patientDto)
        {
            if (id != patientDto.Id)
            {
                return BadRequest();
            }

            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            patient.LastName = patientDto.LastName;
            patient.FirstName = patientDto.FirstName;
            patient.MiddleName = patientDto.MiddleName;
            patient.SectionId = patientDto.SectionId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
