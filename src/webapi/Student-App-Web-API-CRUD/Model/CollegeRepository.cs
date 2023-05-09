namespace DewirideCSharpSamples.WebApi.StudentAppCRUDWebApi.Model
{
	public class CollegeRepository
	{
		public static List<Student> Students { get; set; } = new List<Student>{
				new Student
			{
				Id= 1,
				Name = "Raj",
				Email = "raj@email.com"
			},
				new Student
			{
					Id = 2,
					Name = "Rahul",
					Email = "rahul@email.com"
			},
				new Student
			{
					Id = 3,
					Name = "Binod",
					Email = "binod9@email.com"
			},
				new Student
			{
					Id = 4,
					Name = "Mukesh",
					Email = "mukku@email.com"
			},
				new Student
			{
					Id = 5,
					Name = "Sonia",
					Email = "sona@email.com"
			}
			};

	}
}
