using DewirideCSharpSamples.WebApi.StudentAppCRUDWebApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DewirideCSharpSamples.WebApi.StudentAppCRUDWebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StudentController : ControllerBase
	{
		[HttpGet]
		public ActionResult<IEnumerable<Student>> GetStudents()
		{
			return Ok(CollegeRepository.Students);
		}

		[HttpGet("{id:int}")]
		public ActionResult<Student> GetStudentsById(int id)
		{
			if (id <= 0)
			{
				return BadRequest();
			}

			var student = CollegeRepository.Students.Where(n => n.Id == id).FirstOrDefault();
			if (student == null)
			{
				return NotFound("Student data not found");

			}
			return Ok(student);
		}


		[HttpPost]
		public ActionResult<Student> AddStudent(Student student)
		{
			if (student == null)
			{
				return BadRequest();
			}

			// add the student to the repository
			CollegeRepository.Students.Add(student);

			// return the newly created student
			return CreatedAtAction(nameof(GetStudentsById), new { id = student.Id }, "Successfully created new data");
		}



		[HttpPut("{id}")]
		public ActionResult<Student> UpdateStudent(int id, Student updatedStudent)
		{
			if (updatedStudent == null || id != updatedStudent.Id)
			{
				return BadRequest();
			}

			var existingStudent = CollegeRepository.Students.Where(n => n.Id == id).FirstOrDefault();
			if (existingStudent == null)
			{
				return NotFound("Student not found");
			}

			// update the student in the repository
			existingStudent.Id = updatedStudent.Id;
			existingStudent.Name = updatedStudent.Name;
			existingStudent.Email = updatedStudent.Email;


			// return the updated student
			return Ok(existingStudent);
		}


		[HttpDelete("{id}")]
		public ActionResult<bool> DeleteStudents(int id)
		{
			if (id < 0)
			{
				return BadRequest();
			}
			var student = CollegeRepository.Students.Where(n => n.Id == id).FirstOrDefault();
			if (student == null)
			{
				return NotFound("Student not found");
			}

			CollegeRepository.Students.Remove(student);
			return Ok($"Successfully deleted id:{id}");
		}

	}
}
