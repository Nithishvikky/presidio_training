using ConsultingManagement.Contexts;
using ConsultingManagement.Interfaces;
using ConsultingManagement.Misc;
using ConsultingManagement.Models;
using ConsultingManagement.Models.DTOs;
using Microsoft.EntityFrameworkCore;


namespace ConsultingManagement.Services
{
    public class DoctorServiceWithTransaction : IDoctorService
    {
        private readonly ConsultancyContext _consultancyContext;
        private readonly DoctorMapper _doctorMapper;
        private readonly SpecialityMapper _specialityMapper;

        public DoctorServiceWithTransaction(ConsultancyContext  consultancyContext)
        {
            _consultancyContext =  consultancyContext;
            _doctorMapper = new DoctorMapper();
            _specialityMapper = new();
        }
        public async Task<Doctor> AddDoctor(DoctorAddRequestDto doctor)
        {
            using var transaction = await _consultancyContext.Database.BeginTransactionAsync();
            var newDoctor = _doctorMapper.MapDoctorAddRequestDoctor(doctor);

            try
            {
                await _consultancyContext.AddAsync(newDoctor);
                await _consultancyContext.SaveChangesAsync();
                if (doctor.Specialities?.Count() > 0)
                {
                    var existingSpecialities = await _consultancyContext.specialities.ToListAsync();
                    foreach (var item in doctor.Specialities)
                    {

                        var speciality = await _consultancyContext.specialities.FirstOrDefaultAsync(s => s.Name.ToLower() == item.Name.ToLower());
                        if (speciality == null)
                        {
                            speciality = _specialityMapper.MapSpecialityAddRequestDoctor(item);
                            await _consultancyContext.AddAsync(speciality);
                            await _consultancyContext.SaveChangesAsync();
                        }
                        var doctorSpeciality = _specialityMapper.MapDoctorSpecility(newDoctor.Id, speciality.Id);
                        await _consultancyContext.AddAsync(doctorSpeciality);

                    }
                    await _consultancyContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return newDoctor;
                }
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            return null;
        }

        public Task<Doctor?> GetDoctByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Doctor> GetDoctByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string speciality)
        {
            throw new NotImplementedException();
        }
    }
}