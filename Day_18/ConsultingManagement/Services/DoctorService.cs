using ConsultingManagement.Interfaces;
using ConsultingManagement.Models;
using ConsultingManagement.Models.DTOs;

namespace FirstAPI.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IRepository<int, Doctor> _doctorRepository;
        private readonly IRepository<int, Speciality> _specialityRepository;
        private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;

        public DoctorService(IRepository<int, Doctor> doctorRepository,
                            IRepository<int, Speciality> specialityRepository,
                            IRepository<int, DoctorSpeciality> doctorSpecialityRepository)
        {
            _doctorRepository = doctorRepository;
            _doctorSpecialityRepository = doctorSpecialityRepository;
            _specialityRepository = specialityRepository;
        }

        public async Task<Doctor> AddDoctor(DoctorAddRequestDto doctor)
        {
            Doctor d = new Doctor
            {
                Name = doctor.Name,
                YearsOfExperience = doctor.YearsOfExperience
            };

            var Added_Doctor = await _doctorRepository.Add(d);

            if (doctor.Specialities != null)
            {
                var Allspecialities = await _specialityRepository.GetAll();

                foreach (var spec in doctor.Specialities)
                {
                    var speciality = Allspecialities.FirstOrDefault(s => s.Name.Equals(spec.Name));

                    if (speciality == null)
                    {
                        speciality = await _specialityRepository.Add(new Speciality
                        {
                            Name = spec.Name,
                            Status = "Active"
                        });
                    }

                    await _doctorSpecialityRepository.Add(new DoctorSpeciality
                    {
                        DoctorId = Added_Doctor.Id,
                        SpecialityId = speciality.Id
                    });
                }
            }
            return Added_Doctor;
        }

        public async Task<Doctor> GetDoctByName(string name)
        {
            try
            {
                var doctors = await _doctorRepository.GetAll(); // All doctors

                var doctor = doctors.FirstOrDefault(d => d.Name.ToLower().Equals(name,StringComparison.OrdinalIgnoreCase)); // Single Doctor

                if (doctor == null)
                {
                    throw new Exception($"No doctor found by name {name}");
                }

                return doctor;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get doctors by name...\n{e.Message}");
            }
            
        }

        public async Task<ICollection<Doctor>> GetDoctorsBySpeciality(string speciality)
        {
            var specialities = await _specialityRepository.GetAll();
            var Matchspeciality = specialities.FirstOrDefault(s => s.Name.Equals(speciality, StringComparison.OrdinalIgnoreCase));

            if (Matchspeciality != null)
            {
                var doctorSpecialities = await _doctorSpecialityRepository.GetAll();
                var doctorIds = doctorSpecialities.Where(ds => ds.SpecialityId == Matchspeciality.Id)
                                                .Select(ds => ds.DoctorId)
                                                .ToList();


                var doctors = await _doctorRepository.GetAll();

                return doctors.Where(d => doctorIds.Contains(d.Id)).ToList();
            }
            else
            {
                throw new Exception("Speciality not found");
            }                     
        }
    }
}