using ConsultingManagement.Contexts;
using ConsultingManagement.Models;
using ConsultingManagement.Repositories;
using ConsultingManagement.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConsultingManagement.Tests;

public class PatientRepoTest
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

    private Patient CreateTestPatient(string email = "test@gmail.com")
    {
        return new Patient
        {
            Name = "Test Patient",
            Email = email
        };
    }

    [Test]
    public async Task AddPatientTest()
    {
        //arrange
        var email = "test@gmail.com";
        var password = System.Text.Encoding.UTF8.GetBytes("test123");
        var key = Guid.NewGuid().ToByteArray();
        var user = new User
        {
            Username = email,
            Password = password,
            HashKey = key,
            Role = "Patient"
        };
        _context.Add(user);
        await _context.SaveChangesAsync();
        var patient = CreateTestPatient(email);
        IRepository<int, Patient> _doctorRepository = new PatientRepository(_context);
        //action
        var result = await _doctorRepository.Add(patient);
        //assert
        Assert.That(result, Is.Not.Null, "Patient IS not addeed");
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Email, Is.EqualTo("test@gmail.com"));
    }

    [TestCase(1)]
    [TestCase(2)]
    public async Task GetPatientPassTest(int id)
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
            Role = "Patient"
        };
        _context.Add(user);
        await _context.SaveChangesAsync();
        var patient = CreateTestPatient(email);
        IRepository<int, Patient> _doctorRepository = new PatientRepository(_context);
        //action
        await _doctorRepository.Add(patient);

        //action
        var result = _doctorRepository.Get(id);
        //assert
        Assert.That(result.Id, Is.EqualTo(id));

    }

    [TestCase(3)]
    public async Task GetPatientExceptionTest(int id)
    {
        // Arrange
        var email = "test@gmail.com";
        var password = System.Text.Encoding.UTF8.GetBytes("test123");
        var key = Guid.NewGuid().ToByteArray();

        var user = new User
        {
            Username = email,
            Password = password,
            HashKey = key,
            Role = "Patient"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var patient = new Patient
        {
            Name = "test",
            Email = email
        };
        IRepository<int, Patient> doctorRepository = new PatientRepository(_context);
        await doctorRepository.Add(patient); 

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(async () => await doctorRepository.Get(id));
        Assert.That(ex.Message, Is.EqualTo("No Patient with the given ID"));
    }

    [Test]
    public async Task GetAllPatientsTest()
    {
        IRepository<int, Patient> _doctorRepository = new PatientRepository(_context);

        var doctors = new List<Patient>
        {
            CreateTestPatient("doc1@gmail.com"),
            CreateTestPatient("doc2@gmail.com"),
            CreateTestPatient("doc3@gmail.com"),
        };

        foreach (var doc in doctors)
        {
            await _doctorRepository.Add(doc);
        }

        var result = await _doctorRepository.GetAll();
        var results = result.ToList();
        Assert.That(results.Count, Is.EqualTo(doctors.Count));
        CollectionAssert.AreEquivalent(
            doctors.ConvertAll(d => d.Email),
            results.ConvertAll(d => d.Email));
    }

    [Test]
    public async Task UpdatePatientTest()
    {
        IRepository<int, Patient> _doctorRepository = new PatientRepository(_context);
        var patient = await _doctorRepository.Add(CreateTestPatient());

        patient.Name = "Updated Name";

        var updatedPatient = await _doctorRepository.Update(patient.Id, patient);

        Assert.That(updatedPatient, Is.Not.Null);
        Assert.That(updatedPatient.Id, Is.EqualTo(patient.Id));
        Assert.That(updatedPatient.Name, Is.EqualTo("Updated Name"));
    }

    [Test]
    public void UpdatePatient_NotFound_ThrowsException()
    {
        IRepository<int, Patient> _doctorRepository = new PatientRepository(_context);

        var patient = CreateTestPatient();
        patient.Id = 999; 

        var ex = Assert.ThrowsAsync<System.Exception>(async () =>
            await _doctorRepository.Update(patient.Id, patient));

        Assert.That(ex.Message, Is.EqualTo("No patient with the given ID"));
    }


    [Test]
    public async Task DeletePatientTest()
    {
        IRepository<int, Patient> _doctorRepository = new PatientRepository(_context);
        var patient = await _doctorRepository.Add(CreateTestPatient());

        var deleteResult = await _doctorRepository.Delete(patient.Id);

        Assert.That(deleteResult.Id, Is.EqualTo(patient.Id));

        var ex = Assert.ThrowsAsync<System.Exception>(async () => await _doctorRepository.Get(patient.Id));
        Assert.That(ex.Message, Is.EqualTo("No patient with the given ID"));
    }


    [Test]
    public void DeletePatient_NotFound_ThrowsException()
    {
        IRepository<int, Patient> _doctorRepository = new PatientRepository(_context);

        var ex = Assert.ThrowsAsync<System.Exception>(async () =>
            await _doctorRepository.Delete(999)); 

        Assert.That(ex.Message, Is.EqualTo("No patient with the given ID"));
    }


    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}