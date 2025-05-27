
using ConsultingManagement.Models;
using ConsultingManagement.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ConsultingManagement.Repositories
{
    public class AppointmentRepository : Repository<int, Appointment>
    {
        public AppointmentRepository() : base()
        { 
        }

        public override ICollection<Appointment> GetAll()
        {
            if (_items.Count == 0)
            {
                throw new CollectionEmptyException("No Appointments found");
            }
            return _items;
        }

        public override Appointment GetById(int id)
        {
            // var appointment = _items.FirstOrDefault(a => a.AppointmnetNumber == id);
            // if (appointment == null)
            // {
            //     throw new KeyNotFoundException("Appointment not found");
            // }
            // return appointment;
            throw new NotImplementedException();
        }

        protected override int GenerateID()
        {
            // if (_items.Count == 0)
            // {
            //     return 1;
            // }
            // else
            // {
            //     return _items.Max(a => a.Id) + 1;
            // }
            throw new NotImplementedException();
        }
    }
}