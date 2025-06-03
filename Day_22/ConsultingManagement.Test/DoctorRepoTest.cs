using ConsultingManagement.Contexts;
using ConsultingManagement.Models;
using ConsultingManagement.Repositories;
using ConsultingManagement.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConsultingManagement.Test;
public class Tests
{
    private ConsultancyContext _context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ConsultancyContext>()
                            .UseInMemoryDatabase("TestDb")
                            .Options;

        _context = new ConsultancyContext(options);
    }

    // [Test]
    // public async Task AddDoctorTest()
    // {
    //     //arrange
    //     var email = " test@gmail.com";
    //     var password = System.Text.Encoding.UTF8.GetBytes("test123");
    //     var key = Guid.NewGuid().ToByteArray();
    //     var user = new User
    //     {
    //         Username = email,
    //         Password = password,
    //         HashKey = key,
    //         Role = "Doctor"
    //     };
    //     _context.Add(user);
    //     await _context.SaveChangesAsync();
    //     var doctor = new Doctor
    //     {
    //         Name = "test",
    //         YearsOfExperience = 2,
    //         Email = email
    //     };
    //     IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);
    //     //action
    //     var result = await _doctorRepository.Add(doctor);
    //     //assert
    //     Assert.That(result, Is.Not.Null, "Doctor is not addeed");
    //     Assert.That(result.Id, Is.EqualTo(1));
    // }

    // [TestCase(2)]
    [TestCase(1)]
    public async Task GetDoctorPassTest(int id)
    {
        //arrange
        var email = " test@gmail.com";
        var password = System.Text.Encoding.UTF8.GetBytes("test123");
        var key = Guid.NewGuid().ToByteArray();
        var user = new User
        {
            Username = email,
            Password = password,
            HashKey = key,
            Role = "Doctor"
        };
        _context.Add(user);
        await _context.SaveChangesAsync();
        var doctor = new Doctor
        {
            Name = "test",
            YearsOfExperience = 2,
            Email = email
        };
        IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);
        //action
        await _doctorRepository.Add(doctor);

        //action
        var result = _doctorRepository.Get(id);
        //assert
        Assert.That(result.Id, Is.EqualTo(id));

    }

    [TearDown]
    public void TearDown() 
    {
        _context.Dispose();
    }
        
}
