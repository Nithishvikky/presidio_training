
using Day13_Task.Exceptions;
using Day13_Task.Models;

namespace Day13_Task.Repositories
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
            var appointment = _items.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
            {
                throw new KeyNotFoundException("Appointment not found");
            }
            return appointment;
        }

        protected override int GenerateID()
        {
            if (_items.Count == 0)
            {
                return 1;
            }
            else
            {
                return _items.Max(a => a.Id) + 1;
            }
        }
    }
}
